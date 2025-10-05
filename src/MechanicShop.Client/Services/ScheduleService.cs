using System.Net.Http.Json;
using MechanicShop.Client.Common;
using MechanicShop.Client.Models;
using MechanicShop.Client.Services.Abstractions;

namespace MechanicShop.Client.Services;

public class ScheduleService(IHttpClientFactory httpClientFactory, TimeZoneService timeZoneService) : IScheduleService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("MechanicShopApi");
    private readonly TimeZoneService _timeZoneService = timeZoneService;


    public async Task<ApiResult<ScheduleModel>> GetDailyScheduleAsync(DateOnly date, Guid? laborId = null)
    {
        try
        {
            var url = $"api/v1/workorders/schedule/{date:yyyy-MM-dd}";

            if (laborId.HasValue)
            {
                url += $"?laborId={laborId.Value}";
            }

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var tz = await _timeZoneService.GetLocalTimeZoneAsync();

            request.Headers.Add("X-TimeZone", tz);

            var response = await _httpClient
                .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None)
                .ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var schedule = await response.Content.ReadFromJsonAsync<ScheduleModel>().ConfigureAwait(false);

                if (schedule is null)
                {
                    return ApiResult<ScheduleModel>.Failure("Schedule data is null");
                }

                try
                {
                    var timeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Montreal");

                    foreach (var slot in schedule.Spots.SelectMany(s => s.Slots))
                    {
                        slot.StartAt = TimeZoneInfo.ConvertTime(slot.StartAt, timeZone);
                        slot.EndAt = TimeZoneInfo.ConvertTime(slot.EndAt, timeZone);
                    }
                }
                catch (TimeZoneNotFoundException)
                {
                    return ApiResult<ScheduleModel>.Failure("Time zone 'America/Montreal' not found on this system.");
                }

                return ApiResult<ScheduleModel>.Success(schedule);
            }

            return await SD.HandleErrorResponseAsync<ScheduleModel>(response).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return await SD
                .HandleExceptionAsync<ScheduleModel>(ex, $"Failed to retrieve schedule for {date}")
                .ConfigureAwait(false);
        }
    }
}
