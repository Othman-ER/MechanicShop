using FluentValidation;

namespace MechanicShop.Application.Customers.Commands.CreateCustomer;

public class CreateVehicleCommandValidator : AbstractValidator<CreateVehicleCommand>
{
    public CreateVehicleCommandValidator()
    {
        RuleFor(x => x.Make)
            .NotEmpty()
            .WithMessage("Make is required.")
            .MaximumLength(50)
            .WithMessage("Make must not exceed 50 characters.");

        RuleFor(x => x.Model)
            .NotEmpty()
            .WithMessage("Model is required.")
            .MaximumLength(50)
            .WithMessage("Model must not exceed 50 characters.");

        RuleFor(x => x.LicensePlate)
            .NotEmpty()
            .WithMessage("License plate is required.")
            .MaximumLength(10)
            .WithMessage("License plate must not exceed 10 characters.");
    }
}