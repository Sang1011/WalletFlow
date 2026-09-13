using MediatR;
using WalletFlow.Application.Common.Models;

namespace WalletFlow.Application.Auth.Commands.ResetPassword;

public record ResetPasswordCommand(
    string PhoneNumber,
    string OtpCode,
    string NewPassword) : IRequest<Result>;