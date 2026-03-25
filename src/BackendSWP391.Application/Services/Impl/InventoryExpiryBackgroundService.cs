using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BackendSWP391.Core.Models;
using BackendSWP391.DataAccess.Repositories;

namespace BackendSWP391.Application.Services.Impl;

/// <summary>
/// Background service chạy hàng ngày lúc 00:00 UTC để kiểm tra hàng hết hạn
/// trong kho của tất cả Franchise Stores và chuyển sang WasteCost.
/// </summary>
public class InventoryExpiryBackgroundService(IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Tính thời gian đến 00:00 UTC ngày mai
            var now     = DateTime.UtcNow;
            var nextRun = now.Date.AddDays(1);
            var delay   = nextRun - now;

            await Task.Delay(delay, stoppingToken);

            if (stoppingToken.IsCancellationRequested) break;

            await RunExpiryCheckAsync(stoppingToken);
        }
    }

    private async Task RunExpiryCheckAsync(CancellationToken cancellationToken)
    {
        using var scope        = scopeFactory.CreateScope();
        var storeInventorySvc  = scope.ServiceProvider.GetRequiredService<IStoreInventoryService>();
        var storeRepo          = scope.ServiceProvider.GetRequiredService<IGenericRepository<FranchiseStore>>();

        var storeIds = await storeRepo.Queryable
            .Select(s => s.StoreId)
            .ToListAsync(cancellationToken);

        foreach (var storeId in storeIds)
        {
            try
            {
                await storeInventorySvc.ProcessExpiredItemsAsync(storeId);
            }
            catch
            {
                // Không để lỗi 1 store ảnh hưởng các store khác
            }
        }
    }
}
