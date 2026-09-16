using FluentValidation;

namespace WalletFlow.Application.Transactions.Commands.Withdraw;

public class WithdrawCommandValidator : AbstractValidator<WithdrawCommand>
{
    public WithdrawCommandValidator()
    {
        RuleFor(x => x.WalletId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Số tiền rút phải lớn hơn 0.");
        RuleFor(x => x.IdempotencyKey).NotEmpty();
    }
}