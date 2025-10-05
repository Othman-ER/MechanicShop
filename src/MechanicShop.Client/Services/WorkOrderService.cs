using System.Net.Http.Json;
using MechanicShop.Client.Common;
using MechanicShop.Client.Extensions;
using MechanicShop.Client.Models;
using MechanicShop.Client.Services.Abstractions;
using MechanicShop.Contracts.Requests.WorkOrders;

namespace MechanicShop.Client.Services;

public class WorkOrderService(IHttpClientFactory httpClientFactory) : IWorkOrderService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("MechanicShopApi");

    public async Task<ApiResult<PaginatedList<WorkOrderListItemModel>>> GetWorkOrdersAsync(
        WorkOrderFilterRequest request,
        PageRequest pageRequest,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var queryString = SD.BuildQueryString(request, pageRequest);
            var url = $"api/v1/WorkOrders?{queryString}";

            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var workOrders = await response.Content.ReadFromJsonAsync<PaginatedList<WorkOrderListItemModel>>(cancellationToken: cancellationToken);

                workOrders?.Items.ForEach(item => item.AdjustTimeToLocal());

                return ApiResult<PaginatedList<WorkOrderListItemModel>>.Success(workOrders!);
            }

            return await SD.HandleErrorResponseAsync<PaginatedList<WorkOrderListItemModel>>(response);
        }
        catch (OperationCanceledException)
        {
            return ApiResult<PaginatedList<WorkOrderListItemModel>>.Failure("Operation was cancelled");
        }
        catch (Exception ex)
        {
            return await SD.HandleExceptionAsync<PaginatedList<WorkOrderListItemModel>>
                (ex, "Failed to retrieve work orders");
        }
    }


    public async Task<ApiResult<WorkOrderModel>> GetByIdAsync(Guid workOrderId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/v1/WorkOrders/{workOrderId}");

            if (response.IsSuccessStatusCode)
            {
                var workOrder = await response.Content.ReadFromJsonAsync<WorkOrderModel>();
                workOrder?.AdjustTimeToLocal();

                return ApiResult<WorkOrderModel>.Success(workOrder!);
            }

            return await SD.HandleErrorResponseAsync<WorkOrderModel>(response);
        }
        catch (Exception ex)
        {
            return await SD.HandleExceptionAsync<WorkOrderModel>
                (ex, $"Failed to retrieve work order {workOrderId}");
        }
    }


    public async Task<ApiResult> CreateAsync(CreateWorkOrderRequest workOrderRequest)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/v1/WorkOrders", workOrderRequest);

            if (response.IsSuccessStatusCode)
            {
                return ApiResult.Success();
            }

            return await SD.HandleErrorResponseAsync(response);
        }
        catch (Exception ex)
        {
            return await SD.HandleExceptionAsync(ex, "Failed to create work order");
        }
    }


    public async Task<ApiResult> RelocateWorkOrderAsync(Guid workOrderId, RelocateWorkOrderRequest request)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/v1/WorkOrders/{workOrderId}/relocation", request);

            if (response.IsSuccessStatusCode)
            {
                return ApiResult.Success();
            }

            return await SD.HandleErrorResponseAsync(response);
        }
        catch (Exception ex)
        {
            return await SD.HandleExceptionAsync(ex, $"Failed to relocate work order timing for {workOrderId}");
        }
    }


    public async Task<ApiResult> UpdateWorkOrderLaborAsync(Guid workOrderId, AssignLaborRequest laborUpdateRequest)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/v1/WorkOrders/{workOrderId}/labor", laborUpdateRequest);

            if (response.IsSuccessStatusCode)
            {
                return ApiResult.Success();
            }

            return await SD.HandleErrorResponseAsync(response);
        }
        catch (Exception ex)
        {
            return await SD.HandleExceptionAsync(ex, $"Failed to update work order labor for {workOrderId}");
        }
    }


    public async Task<ApiResult> UpdateWorkOrderStateAsync(Guid workOrderId, UpdateWorkOrderStateRequest statusUpdateRequest)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/v1/WorkOrders/{workOrderId}/state", statusUpdateRequest);

            if (response.IsSuccessStatusCode)
            {
                return ApiResult.Success();
            }

            return await SD.HandleErrorResponseAsync(response);
        }
        catch (Exception ex)
        {
            return await SD.HandleExceptionAsync(ex, $"Failed to update work order state for {workOrderId}");
        }
    }


    public async Task<ApiResult> UpdateWorkOrderRepairTasksAsync(
        Guid workOrderId,
        ModifyRepairTaskRequest updateWorkOrderRepairTasks)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/v1/WorkOrders/{workOrderId}/repair-task", updateWorkOrderRepairTasks);

            if (response.IsSuccessStatusCode)
            {
                return ApiResult.Success();
            }

            return await SD.HandleErrorResponseAsync(response);
        }
        catch (Exception ex)
        {
            return await SD.HandleExceptionAsync(ex, $"Failed to update work order repair tasks for {workOrderId}");
        }
    }


    public async Task<ApiResult> DeleteByIdAsync(Guid workOrderId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/v1/WorkOrders/{workOrderId}");

            if (response.IsSuccessStatusCode)
            {
                return ApiResult.Success();
            }

            return await SD.HandleErrorResponseAsync(response);
        }
        catch (Exception ex)
        {
            return await SD.HandleExceptionAsync(ex, $"Failed to delete work order {workOrderId}");
        }
    }

    public async Task<ApiResult<TodayWorkOrderStatsModel>> GetTodayWorkOrderStatsAsync(DateOnly? date = null)
    {
        try
        {
            var url = "api/v1/dashboard/stats";

            if (date.HasValue)
            {
                url += $"?date={date:yyyy-MM-dd}";
            }

            var response = await _httpClient.GetFromJsonAsync<TodayWorkOrderStatsModel>(url);

            return ApiResult<TodayWorkOrderStatsModel>.Success(response!);
        }
        catch (HttpRequestException ex)
        {
            return ApiResult<TodayWorkOrderStatsModel>.Failure(
                "Network error while fetching work order stats.",
                ex.Message,
                (int?)ex.StatusCode ?? 500);
        }
        catch (Exception ex)
        {
            return await SD.HandleExceptionAsync<TodayWorkOrderStatsModel>(ex, "Unexpected error occurred while fetching dashboard stats.");
        }
    }
}
