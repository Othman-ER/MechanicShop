using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Identity.DTOs;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Identity.Queries.GetUserInfo;

public class GetUserByIdQueryHanlder(
    ILogger<GetUserByIdQueryHanlder> logger,
    IIdentityService identityService)
    : IRequestHandler<GetUserByIdQuery, Result<ApplicationUserDTo>>
{
    public async Task<Result<ApplicationUserDTo>> Handle(
        GetUserByIdQuery request,
        CancellationToken ct)
    {
        var getUserByIdResult = await identityService.GetUserByIdAsync(request.UserId!);

        if (getUserByIdResult.IsError)
        {
            logger.LogError("User with Id { UserId }{ErrorDetails}", request.UserId, getUserByIdResult.TopError.Description);

            return getUserByIdResult.Errors;
        }

        return getUserByIdResult.Value;
    }
}