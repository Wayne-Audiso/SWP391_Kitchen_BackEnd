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
    public async Task<CreateUserResponseModel> CreateAsync(CreateUserModel createUserModel)
    {
        var user = mapper.Map<ApplicationUser>(createUserModel);

        var result = await userManager.CreateAsync(user, createUserModel.Password);

        if (!result.Succeeded) throw new BadRequestException(result.Errors.FirstOrDefault()?.Description);

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

        var token = JwtHelper.GenerateToken(user, configuration);

        var roles = await userManager.GetRolesAsync(user);

        return new LoginResponseModel
        {
            UserId = user.Id,
            Username = user.UserName,
            Role = roles.FirstOrDefault(),
            Email = user.Email,
            Token = token
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

    public async Task<IEnumerable<UserResponseModel>> GetAllAsync()
    {
        var users = await userManager.Users.ToListAsync();

        return mapper.Map<IEnumerable<UserResponseModel>>(users);
    }

    public async Task<UserResponseModel> GetByIdAsync(Guid id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());

        if (user == null)
            throw new NotFoundException($"User with ID {id} not found");

        return mapper.Map<UserResponseModel>(user);
    }

    public async Task<UpdateUserResponseModel> UpdateAsync(Guid id, UpdateUserModel updateUserModel)
    {
        var user = await userManager.FindByIdAsync(id.ToString());

        if (user == null)
            throw new NotFoundException($"User with ID {id} not found");

        user.Email = updateUserModel.Email;
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

