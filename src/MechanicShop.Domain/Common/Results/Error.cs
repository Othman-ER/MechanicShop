namespace MechanicShop.Domain.Common.Results;

public readonly record struct Error
{
    public string Code { get; }

    public string Description { get; }

    public ErrorType Type { get; }


    private Error(string code, string description, ErrorType type)
    {
        Code = code;
        Description = description;
        Type = type;
    }


    public static Error Failure(string code = nameof(Failure), string description = "General Failure.")
        => new(code, description, ErrorType.Failure);


    public static Error Unexpected(string code = nameof(Unexpected), string description = "Unexpected error.")
        => new(code, description, ErrorType.Unexpected);


    public static Error NotFound(string code = nameof(NotFound), string description = "Entity not found.")
        => new(code, description, ErrorType.NotFound);


    public static Error Validation(string code = nameof(Validation), string description = "Validation error.")
        => new(code, description, ErrorType.Validation);


    public static Error Conflict(string code = nameof(Conflict), string description = "Conflict error.")
        => new(code, description, ErrorType.Conflict);


    public static Error Unauthorized(string code = nameof(Unauthorized), string description = "Unauthorized error.")
        => new(code, description, ErrorType.Unauthorized);


    public static Error Forbidden(string code = nameof(Forbidden), string description = "Forbidden error.")
        => new(code, description, ErrorType.Forbidden);
}
