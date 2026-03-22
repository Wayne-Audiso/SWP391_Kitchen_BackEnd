using FluentValidation;
using Microsoft.AspNetCore.Identity;
using BackendSWP391.Application.Models.User;
using BackendSWP391.DataAccess.Identity;

namespace BackendSWP391.Application.Models.Validators.User;

public class CreateUserModelValidator : AbstractValidator<CreateUserModel>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public CreateUserModelValidator(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;

        RuleFor(u => u.UserName)
            .MinimumLength(UserValidatorConfiguration.MinimumUsernameLength)
            .WithMessage($"Username should have minimum {UserValidatorConfiguration.MinimumUsernameLength} characters")
            .MaximumLength(UserValidatorConfiguration.MaximumUsernameLength)
            .WithMessage($"Username should have maximum {UserValidatorConfiguration.MaximumUsernameLength} characters")
            .Must(UsernameIsUnique)
            .WithMessage("Username is not available");

        RuleFor(u => u.Password)
            .MinimumLength(UserValidatorConfiguration.MinimumPasswordLength)
            .WithMessage($"Password should have minimum {UserValidatorConfiguration.MinimumPasswordLength} characters")
            .MaximumLength(UserValidatorConfiguration.MaximumPasswordLength)
            .WithMessage($"Password should have maximum {UserValidatorConfiguration.MaximumPasswordLength} characters");

        RuleFor(u => u.Email)
            .EmailAddress()
            .WithMessage("Email address is not valid")
            .Must(EmailAddressIsUnique)
            .WithMessage("Email address is already in use");

        When(u => !string.IsNullOrWhiteSpace(u.PhoneNumber), () =>
        {
            RuleFor(u => u.PhoneNumber)
                .Matches(UserValidatorConfiguration.VietnamPhonePattern)
                .WithMessage("Số điện thoại không đúng định dạng Việt Nam (VD: 0901234567 hoặc +84901234567).");
        });

        When(u => !string.IsNullOrWhiteSpace(u.Address), () =>
        {
            RuleFor(u => u.Address)
                .MinimumLength(UserValidatorConfiguration.MinimumAddressLength)
                .WithMessage($"Địa chỉ phải có ít nhất {UserValidatorConfiguration.MinimumAddressLength} ký tự.");
        });
    }

    private bool EmailAddressIsUnique(string email)
    {
        var user = _userManager.FindByEmailAsync(email).GetAwaiter().GetResult();

        return user == null;
    }

    private bool UsernameIsUnique(string username)
    {
        var user = _userManager.FindByNameAsync(username).GetAwaiter().GetResult();

        return user == null;
    }
}

