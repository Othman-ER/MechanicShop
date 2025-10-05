namespace MechanicShop.Application.Scheduling.DTOs;

public class ScheduleDto
{
    public DateOnly OnDate { get; set; }
    public bool EndOfDay { get; set; }
    public List<SpotDto> Spots { get; set; } = [];
}
