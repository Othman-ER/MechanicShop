using FluentValidation;

namespace MechanicShop.Application.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("invalid email.")
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?\d{7-15}$").WithMessage("Invalid phone number format.");

        RuleFor(x => x.Vehicles)
            .NotNull().WithMessage("Vehicles list cannot be null.")
            .Must(v => v.Count > 0).WithMessage("At least one vehicle is required.");

        RuleForEach(x => x.Vehicles)
            .SetValidator(new CreateVehicleCommandValidator());
    }
}
