using MediatR;
using WalletFlow.Application.Auth.Common;
using WalletFlow.Application.Common.Models;

namespace WalletFlow.Application.Auth.Commands.Login;

public record LoginCommand(string Identifier, string Password) : IRequest<Result<AuthResponse>>;