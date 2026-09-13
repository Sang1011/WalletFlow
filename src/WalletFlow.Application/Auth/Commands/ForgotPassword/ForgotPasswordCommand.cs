using MediatR;
using WalletFlow.Application.Common.Models;

namespace WalletFlow.Application.Auth.Commands.ForgotPassword;

public record ForgotPasswordCommand(string PhoneNumber) : IRequest<Result>;