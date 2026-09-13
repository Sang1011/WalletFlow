using FluentValidation;

namespace WalletFlow.Application.Auth.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Identifier).NotEmpty()
            .WithMessage("Vui lòng nhập tên đăng nhập hoặc số điện thoại.");
        RuleFor(x => x.Password).NotEmpty();
    }
}