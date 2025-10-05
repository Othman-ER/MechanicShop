using FluentValidation;

namespace MechanicShop.Application.Workorders.Commands.RelocateworkOrder;

public sealed class RescheduleAppointmentCommandValidator : AbstractValidator<RelocateWorkOrderCommand>
{
    public RescheduleAppointmentCommandValidator()
    {
        RuleFor(x => x.WorkOrderId)
            .NotEmpty();

        RuleFor(x => x.NewStartAt)
            .GreaterThan(DateTimeOffset.UtcNow)
            .WithMessage("New start time must be in the future.");

        RuleFor(x => x.NewSpot)
            .IsInEnum();
    }
}
