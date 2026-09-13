using FluentValidation;
using WalletFlow.Application.Common.Validation;

namespace WalletFlow.Application.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MinimumLength(3).MaximumLength(50)
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("Tên đăng nhập chỉ được chứa chữ, số và dấu gạch dưới.");

        RuleFor(x => x.PhoneNumber).NotEmpty()
            .Matches(@"^(0|\+84)[0-9]{9,10}$").WithMessage("Số điện thoại không hợp lệ.");

        RuleFor(x => x.Email).NotEmpty().EmailAddress();

        RuleFor(x => x.Password).NotEmpty().MustBeStrongPassword();

        RuleFor(x => x.FullName).NotEmpty().MaximumLength(100);
    }
}