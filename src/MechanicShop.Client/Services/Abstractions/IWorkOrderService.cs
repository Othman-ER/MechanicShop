using MechanicShop.Client.Common;
using MechanicShop.Client.Models;
using MechanicShop.Contracts.Requests.WorkOrders;

namespace MechanicShop.Client.Services.Abstractions;

public interface IWorkOrderService
{
    Task<ApiResult<PaginatedList<WorkOrderListItemModel>>> GetWorkOrdersAsync(
        WorkOrderFilterRequest request,
        PageRequest pageRequest,
        CancellationToken cancellationToken = default
    );

    Task<ApiResult<WorkOrderModel>> GetByIdAsync(Guid workOrderId);

    Task<ApiResult> CreateAsync(CreateWorkOrderRequest workOrderRequest);

    Task<ApiResult> RelocateWorkOrderAsync(Guid workOrderId, RelocateWorkOrderRequest request);

    Task<ApiResult> UpdateWorkOrderLaborAsync(Guid workOrderId, AssignLaborRequest laborUpdateRequest);

    Task<ApiResult> UpdateWorkOrderStateAsync(Guid workOrderId, UpdateWorkOrderStateRequest statusUpdateRequest);

    Task<ApiResult> UpdateWorkOrderRepairTasksAsync(Guid workOrderId, ModifyRepairTaskRequest updateWorkOrderRepairTasks);

    Task<ApiResult<TodayWorkOrderStatsModel>> GetTodayWorkOrderStatsAsync(DateOnly? date = null);

    Task<ApiResult> DeleteByIdAsync(Guid workOrderId);
}
