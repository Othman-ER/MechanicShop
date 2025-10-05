using System.Net.Http.Json;
using MechanicShop.Client.Common;
using MechanicShop.Client.Models;
using MechanicShop.Client.Services.Abstractions;

namespace MechanicShop.Client.Services;

public class LaborService(IHttpClientFactory httpClientFactory) : ILaborService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("MechanicShopApi");

    public async Task<ApiResult<List<LaborModel>>> GetLaborsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/v1/labors");

            if (response.IsSuccessStatusCode)
            {
                var labors = await response.Content.ReadFromJsonAsync<List<LaborModel>>();
                return ApiResult<List<LaborModel>>.Success(labors ?? []);
            }

            return await SD.HandleErrorResponseAsync<List<LaborModel>>(response);
        }
        catch (Exception ex)
        {
            return await SD.HandleExceptionAsync<List<LaborModel>>
                (ex, "Failed to retrieve labors");
        }
    }
}
