using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.WorkOrders.Enums;
using MechanicShop.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MechanicShop.Infrastructure.BackgroundJobs;

public class OverdueBookingCleanupService(
    IServiceScopeFactory scopeFactory,
    ILogger<OverdueBookingCleanupService> logger,
    IOptions<AppSettings> options,
    TimeProvider dateTime) :
    BackgroundService
{
    private readonly AppSettings _appSettings = options.Value;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(
            TimeSpan.FromMinutes(_appSettings.OverdueBookingCleanupFrequencyMinutes)
        );

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            logger.LogInformation("Checking overdue work orders at {Now}",
                dateTime.GetUtcNow());

            try
            {
                using var scope = scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

                var cutoff = dateTime.GetUtcNow().AddMinutes(-_appSettings.BookingCancellationThresholdMinutes);

                var overdue = await db.WorkOrders
                    .Where(w => w.State == WorkOrderState.Scheduled && w.StartAtUtc <= cutoff)
                    .ToListAsync(stoppingToken);

                if (overdue.Count > 0)
                {
                    foreach (var wo in overdue)
                    {
                        var result = wo.Cancel();

                        if (result.IsError)
                        {
                            logger.LogWarning("Failed to cancel WorkOrder {Id}: {Error}", wo.Id, result.Errors);
                        }
                    }

                    await db.SaveChangesAsync(stoppingToken);

                    logger.LogInformation(
                        "Cancelled {Count} overdue work orders: {Ids}",
                        overdue.Count, overdue.Select(w => w.Id)
                    );
                }
                else
                {
                    logger.LogInformation("No overdue work orders found.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error cleaning up overdue work orders.");
            }
        }
    }
}
