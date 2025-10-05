using MechanicShop.Domain.WorkOrders.Enums;

namespace MechanicShop.Application.Scheduling.DTOs;

public class SpotDto
{
    public Spot Spot { get; set; }
    public List<AvailabilitySlotDto> Slots { get; set; } = [];
}