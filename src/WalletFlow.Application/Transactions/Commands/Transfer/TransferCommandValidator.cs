using FluentValidation;

namespace WalletFlow.Application.Transactions.Commands.Transfer;

public class TransferCommandValidator : AbstractValidator<TransferCommand>
{
    public TransferCommandValidator()
    {
        RuleFor(x => x.SourceWalletId).NotEmpty();
        RuleFor(x => x.DestinationWalletId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Số tiền chuyển phải lớn hơn 0.");
        RuleFor(x => x.IdempotencyKey).NotEmpty();
        RuleFor(x => x)
            .Must(x => x.SourceWalletId != x.DestinationWalletId)
            .WithMessage("Ví nguồn và ví đích không được trùng nhau.");
    }
}