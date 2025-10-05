using MechanicShop.Client.Common;
using MechanicShop.Client.Models;

namespace MechanicShop.Client.Services.Abstractions;

public interface IScheduleService
{
    Task<ApiResult<ScheduleModel>> GetDailyScheduleAsync(DateOnly date, Guid? laborId = null);
}
