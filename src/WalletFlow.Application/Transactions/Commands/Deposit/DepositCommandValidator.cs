using FluentValidation;

namespace WalletFlow.Application.Transactions.Commands.Deposit;

public class DepositCommandValidator : AbstractValidator<DepositCommand>
{
    public DepositCommandValidator()
    {
        RuleFor(x => x.WalletId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Số tiền nạp phải lớn hơn 0.");
        RuleFor(x => x.IdempotencyKey).NotEmpty();
    }
}