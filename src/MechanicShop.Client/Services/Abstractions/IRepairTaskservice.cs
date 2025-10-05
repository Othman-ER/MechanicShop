using MechanicShop.Client.Common;
using MechanicShop.Client.Models;
using MechanicShop.Contracts.Requests.RepairTasks;

namespace MechanicShop.Client.Services.Abstractions;

public interface IRepairTaskservice
{
    Task<ApiResult<List<RepairTaskModel>>> GetRepairTasksAsync();

    Task<ApiResult<RepairTaskModel>> CreateAsync(CreateRepairTaskRequest request);

    Task<ApiResult<RepairTaskModel>> UpdateAsync(Guid repairTaskId, UpdateRepairTaskRequest request);

    Task<ApiResult> DeleteByIdAsync(Guid repairTaskId);
}
