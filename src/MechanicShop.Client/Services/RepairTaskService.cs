using System.Net.Http.Json;
using MechanicShop.Client.Common;
using MechanicShop.Client.Models;
using MechanicShop.Client.Services.Abstractions;
using MechanicShop.Contracts.Requests.RepairTasks;

namespace MechanicShop.Client.Services;

public class RepairTaskService(IHttpClientFactory httpClientFactory) : IRepairTaskservice
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("MechanicShopApi");

    public async Task<ApiResult<List<RepairTaskModel>>> GetRepairTasksAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/v1/repair-tasks");

            if (response.IsSuccessStatusCode)
            {
                var repairTasks = await response.Content.ReadFromJsonAsync<List<RepairTaskModel>>();
                return ApiResult<List<RepairTaskModel>>.Success(repairTasks ?? []);
            }

            return await SD.HandleErrorResponseAsync<List<RepairTaskModel>>(response);
        }
        catch (Exception ex)
        {
            return await SD.HandleExceptionAsync<List<RepairTaskModel>>(ex, "Failed to retrieve repair tasks");
        }
    }

    public async Task<ApiResult<RepairTaskModel>> CreateAsync(CreateRepairTaskRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/v1/repair-tasks", request);

            if (response.IsSuccessStatusCode)
            {
                var repairTask = await response.Content.ReadFromJsonAsync<RepairTaskModel>();
                return ApiResult<RepairTaskModel>.Success(repairTask!);
            }

            return await SD.HandleErrorResponseAsync<RepairTaskModel>(response);
        }
        catch (Exception ex)
        {
            return await SD.HandleExceptionAsync<RepairTaskModel>(ex, "Failed to create repair task");
        }
    }

    public async Task<ApiResult<RepairTaskModel>> UpdateAsync(Guid repairTaskId, UpdateRepairTaskRequest request)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/v1/repair-tasks/{repairTaskId}", request);

            if (response.IsSuccessStatusCode)
            {
                var repairTask = await response.Content.ReadFromJsonAsync<RepairTaskModel>();
                return ApiResult<RepairTaskModel>.Success(repairTask!);
            }

            return await SD.HandleErrorResponseAsync<RepairTaskModel>(response);
        }
        catch (Exception ex)
        {
            return await SD.HandleExceptionAsync<RepairTaskModel>(ex, $"Failed to update repair task {repairTaskId}");
        }
    }

    public async Task<ApiResult> DeleteByIdAsync(Guid repairTaskId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/v1/repair-tasks/{repairTaskId}");

            if (response.IsSuccessStatusCode)
            {
                return ApiResult.Success();
            }

            return await SD.HandleErrorResponseAsync(response);
        }
        catch (Exception ex)
        {
            return await SD.HandleExceptionAsync(ex, $"Failed to delete repair task {repairTaskId}");
        }
    }

}
