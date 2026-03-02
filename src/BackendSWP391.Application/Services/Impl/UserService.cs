using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using BackendSWP391.Application.Common.Email;
using BackendSWP391.Application.Exceptions;
using BackendSWP391.Application.Helpers;
using BackendSWP391.Application.Models;
using BackendSWP391.Application.Models.User;
using BackendSWP391.Application.Templates;
using BackendSWP391.DataAccess.Identity;

namespace BackendSWP391.Application.Services.Impl;

public class UserService(
    IMapper mapper,
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IConfiguration configuration,
    ITemplateService templateService,
    IEmailService emailService)
    : IUserService
{
    private const string DefaultRole = "Franchise Store Staff";

    public async Task<CreateUserResponseModel> CreateAsync(CreateUserModel createUserModel)
    {
        var user = mapper.Map<ApplicationUser>(createUserModel);

        var result = await userManager.CreateAsync(user, createUserModel.Password);

        if (!result.Succeeded) throw new BadRequestException(result.Errors.FirstOrDefault()?.Description);

        // Assign role — dùng role được chỉ định hoặc default
        var roleName = string.IsNullOrWhiteSpace(createUserModel.Role) ? DefaultRole : createUserModel.Role;
        var roleResult = await userManager.AddToRoleAsync(user, roleName);
        if (!roleResult.Succeeded)
            throw new BadRequestException($"Role '{roleName}' không tồn tại. Vui lòng kiểm tra lại tên role.");

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

        var emailTemplate = await templateService.GetTemplateAsync(TemplateConstants.ConfirmationEmail);

        var emailBody = templateService.ReplaceInTemplate(emailTemplate,
            new Dictionary<string, string> { { "{UserId}", user.Id }, { "{Token}", token } });

        await emailService.SendEmailAsync(EmailMessage.Create(user.Email, emailBody, "[BackendSWP391]Confirm your email"));

        return new CreateUserResponseModel
        {
            Id = Guid.Parse((await userManager.FindByNameAsync(user.UserName)).Id)
        };
    }

    public async Task<LoginResponseModel> LoginAsync(LoginUserModel loginUserModel)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.UserName == loginUserModel.Username);

        if (user == null)
            throw new NotFoundException("Username or password is incorrect");

        var signInResult = await signInManager.PasswordSignInAsync(user, loginUserModel.Password, false, false);

        if (!signInResult.Succeeded)
            throw new BadRequestException("Username or password is incorrect");

        var roles = await userManager.GetRolesAsync(user);

        var jwtToken = JwtHelper.GenerateToken(user, roles, configuration);

        return new LoginResponseModel
        {
            UserId   = user.Id,
            Username = user.UserName,
            Role     = roles.FirstOrDefault(),
            Email    = user.Email,
            Token    = jwtToken
        };
    }

    public async Task<ConfirmEmailResponseModel> ConfirmEmailAsync(ConfirmEmailModel confirmEmailModel)
    {
        var user = await userManager.FindByIdAsync(confirmEmailModel.UserId);

        if (user == null)
            throw new UnprocessableRequestException("Your verification link is incorrect");

        var result = await userManager.ConfirmEmailAsync(user, confirmEmailModel.Token);

        if (!result.Succeeded)
            throw new UnprocessableRequestException("Your verification link has expired");

        return new ConfirmEmailResponseModel
        {
            Confirmed = true
        };
    }

    public async Task<BaseResponseModel> ChangePasswordAsync(Guid userId, ChangePasswordModel changePasswordModel)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user == null)
            throw new NotFoundException("User does not exist anymore");

        var result =
            await userManager.ChangePasswordAsync(user, changePasswordModel.OldPassword,
                changePasswordModel.NewPassword);

        if (!result.Succeeded)
            throw new BadRequestException(result.Errors.FirstOrDefault()?.Description);

        return new BaseResponseModel
        {
            Id = Guid.Parse(user.Id)
        };
    }

    public async Task<IEnumerable<UserResponseModel>> GetAllAsync(string? currentUserId)
    {
        var query = userManager.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(currentUserId))
            query = query.Where(u => u.Id != currentUserId);

        var users = await query.ToListAsync();

        var result = new List<UserResponseModel>();
        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            result.Add(new UserResponseModel
            {
                Id       = Guid.Parse(user.Id),
                UserName = user.UserName,
                Email    = user.Email,
                Role     = roles.FirstOrDefault()
            });
        }
        return result;
    }

    public async Task<UserResponseModel> GetByIdAsync(Guid id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());

        if (user == null)
            throw new NotFoundException($"User with ID {id} not found");

        var roles = await userManager.GetRolesAsync(user);

        return new UserResponseModel
        {
            Id       = Guid.Parse(user.Id),
            UserName = user.UserName,
            Email    = user.Email,
            Role     = roles.FirstOrDefault()
        };
    }

    public async Task<UpdateUserResponseModel> UpdateAsync(Guid id, UpdateUserModel updateUserModel)
    {
        var user = await userManager.FindByIdAsync(id.ToString());

        if (user == null)
            throw new NotFoundException($"User with ID {id} not found");

        user.Email    = updateUserModel.Email;
        user.UserName = updateUserModel.UserName;

        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
            throw new BadRequestException(result.Errors.FirstOrDefault()?.Description);

        return new UpdateUserResponseModel
        {
            Id = Guid.Parse(user.Id)
        };
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());

        if (user == null)
            throw new NotFoundException($"User with ID {id} not found");

        var result = await userManager.DeleteAsync(user);

        if (!result.Succeeded)
            throw new BadRequestException(result.Errors.FirstOrDefault()?.Description);

        return true;
    }
}
