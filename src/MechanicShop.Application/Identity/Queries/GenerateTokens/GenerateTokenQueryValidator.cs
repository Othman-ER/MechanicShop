using FluentValidation;

namespace MechanicShop.Application.Identity.Queries.GenerateTokens;

public sealed class GenerateTokenQueryValidator : AbstractValidator<GenerateTokenQuery>
{
    public GenerateTokenQueryValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty()
            .WithErrorCode("Email_Null_Or_Empty")
            .WithMessage("Email cannot be null or empty");

        RuleFor(request => request.Password)
            .NotEmpty()
            .WithErrorCode("Password_Null_Or_Empty")
            .WithMessage("Password cannot be null or empty.");
    }
}