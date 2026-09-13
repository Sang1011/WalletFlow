using MediatR;
using WalletFlow.Application.Auth.Common;
using WalletFlow.Application.Common.Models;

namespace WalletFlow.Application.Auth.Commands.Token;

public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<AuthResponse>>;