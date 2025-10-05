using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Identity.Queries.GenerateTokens;

public record GenerateTokenQuery(
    string Email,
    string Password) :
    IRequest<Result<TokenResponse>>;
