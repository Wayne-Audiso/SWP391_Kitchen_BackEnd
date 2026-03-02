using FluentValidation;
using BackendSWP391.Application.Models.User;

namespace BackendSWP391.Application.Models.Validators.User;

public class ConfirmEmailModelValidator : AbstractValidator<ConfirmEmailModel>
{
    public ConfirmEmailModelValidator()
    {
        RuleFor(ce => ce.Token)
            .NotEmpty()
            .WithMessage("Your verification link is not valid");

        RuleFor(ce => ce.UserId)
            .NotEmpty()
            .WithMessage("Your verification link is not valid");
    }
}

