using FluentValidation;
using BackendSWP391.Application.Models.User;

namespace BackendSWP391.Application.Models.Validators.User;

/// <summary>
/// Validator cho LoginUserModel.
/// - Username: bắt buộc, không được rỗng.
///   Nếu username chứa ký tự '@' thì phải đúng định dạng email.
/// - Password: bắt buộc, độ dài tối thiểu 6 ký tự.
/// </summary>
public class LoginUserModelValidator : AbstractValidator<LoginUserModel>
{
    public LoginUserModelValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Tên đăng nhập không được để trống.");

        // Nếu người dùng đăng nhập bằng email, kiểm tra định dạng hợp lệ
        When(x => !string.IsNullOrWhiteSpace(x.Username) && x.Username.Contains('@'), () =>
        {
            RuleFor(x => x.Username)
                .EmailAddress()
                .WithMessage("Địa chỉ email không đúng định dạng.");
        });

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Mật khẩu không được để trống.")
            .MinimumLength(6)
            .WithMessage("Mật khẩu phải có ít nhất 6 ký tự.");
    }
}
