using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using WalletFlow.Application.Common.Interfaces;

namespace WalletFlow.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?.User?
                .FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);

            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public bool IsAdmin =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue("isAdmin") == "True";
}