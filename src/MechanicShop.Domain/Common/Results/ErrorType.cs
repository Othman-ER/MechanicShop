namespace MechanicShop.Domain.Common.Results;

public enum ErrorType
{
    Failure,
    Unexpected,
    NotFound,
    Validation,
    Conflict,
    Unauthorized,
    Forbidden
}