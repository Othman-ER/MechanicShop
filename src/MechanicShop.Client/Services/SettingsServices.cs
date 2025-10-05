using System.Net.Http.Json;
using MechanicShop.Client.Common;
using MechanicShop.Client.Services.Abstractions;
using MechanicShop.Contracts.Responses;

namespace MechanicShop.Client.Services;

public class SettingsService(IHttpClientFactory httpClientFactory) : ISettingService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("MechanicShop.Api");
    public async Task<ApiResult<OperatingHoursResponse>> GetOperateHoursAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/settings/operating-hours");

            if (response.IsSuccessStatusCode)
            {
                var operateHours = await response.Content.ReadFromJsonAsync<OperatingHoursResponse>();

                return ApiResult<OperatingHoursResponse>.Success(operateHours!);
            }

            return await SD.HandleErrorResponseAsync<OperatingHoursResponse>(response);
        }
        catch (Exception ex)
        {
            return await SD.HandleExceptionAsync<OperatingHoursResponse>
                (ex, "Failed to retrieve operating hours");
        }
    }

}
