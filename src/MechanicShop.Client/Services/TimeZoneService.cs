using MechanicShop.Client.Services.Abstractions;
using Microsoft.JSInterop;

namespace MechanicShop.Client.Services;

public sealed class TimeZoneService(IJSRuntime js) : ITimeZonService
{
    private readonly IJSRuntime _js = js;

    public async Task<string> GetLocalTimeZoneAsync()
    {
        return await _js.InvokeAsync<string>("getLocalTimeZone");
    }
}