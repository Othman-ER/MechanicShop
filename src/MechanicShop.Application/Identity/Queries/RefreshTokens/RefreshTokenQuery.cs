using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Identity.Queries.RefreshTokens;

public record RefreshTokenQuery(string RefreshToken, string ExpiredAccessToken) :
    IRequest<Result<TokenResponse>>;
