using MechanicShop.Client.Services;
using MechanicShop.Client.Services.Abstractions;

namespace MechanicShop.Client.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddClientServices(this IServiceCollection services)
    {
        services.AddScoped<ICustomerService, CustomerService>()
            .AddScoped<IInvoiceService, InvoiceService>()
            .AddScoped<IRepairTaskservice, RepairTaskService>()
            .AddScoped<IScheduleService, ScheduleService>()
            .AddScoped<ISettingService, SettingsService>()
            .AddScoped<ITimeZonService, TimeZoneService>()
            .AddScoped<IWorkOrderService, WorkOrderService>()
            .AddScoped<ILaborService, LaborService>();

        return services;
    }
}
