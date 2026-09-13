using FluentValidation;

namespace WalletFlow.Application.Common.Validation;

public static class PasswordValidationExtensions
{
    public static IRuleBuilderOptions<T, string> MustBeStrongPassword<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Mật khẩu cần ít nhất 1 chữ hoa.")
            .Matches("[0-9]").WithMessage("Mật khẩu cần ít nhất 1 chữ số.")
            .Matches(@"[!@#$%^&*(),.?"":{}|<>_\-+=~`\[\];'/\\]").WithMessage("Mật khẩu cần ít nhất 1 ký tự đặc biệt.");
    }
}