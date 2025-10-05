namespace MechanicShop.Client.Services.Abstractions;

public interface ITimeZonService
{
    Task<string> GetLocalTimeZoneAsync();
}
