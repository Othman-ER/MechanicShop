using System.Security.Claims;
using MechanicShop.Application.Identity;
using MechanicShop.Application.Identity.DTOs;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Application.Common.Interfaces;

public interface ITokenProvider
{
    Task<Result<TokenResponse>> GenerateJwtTokenAsync(
        ApplicationUserDTo user,
        CancellationToken cancellationToken = default
    );

    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}