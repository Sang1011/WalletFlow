using MediatR;
using WalletFlow.Application.Auth.Common;
using WalletFlow.Application.Common.Models;

namespace WalletFlow.Application.Auth.Commands.Register;

public record RegisterCommand(
    string Username,
    string PhoneNumber,
    string Email,
    string Password,
    string FullName) : IRequest<Result<AuthResponse>>;