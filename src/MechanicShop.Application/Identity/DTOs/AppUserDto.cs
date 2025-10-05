using System.Security.Claims;

namespace MechanicShop.Application.Identity.DTOs;

public sealed record ApplicationUserDTo
(
    string UserId,
    string Email,
    IList<string> Roles,
    IList<Claim> Claims
);