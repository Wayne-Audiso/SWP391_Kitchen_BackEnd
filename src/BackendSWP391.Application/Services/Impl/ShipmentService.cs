using Microsoft.EntityFrameworkCore;
using BackendSWP391.Application.Models.Shipment;
using BackendSWP391.Core.Models;
using BackendSWP391.DataAccess.Repositories;

namespace BackendSWP391.Application.Services.Impl;

public class ShipmentService(
    IGenericRepository<Shipment>     shipmentRepo,
    IGenericRepository<ShipmentLine> lineRepo) : IShipmentService
{
    private IQueryable<ShipmentDto> ProjectedQuery =>
        shipmentRepo.Queryable
            .Include(s => s.CentralKitchen)
            .Include(s => s.ShipmentLines)
                .ThenInclude(l => l.Product)
            .Select(s => new ShipmentDto
            {
                ShipmentId       = s.ShipmentId,
                StoreOrderId     = s.StoreOrderId,
                CentralKitchenId = s.CentralKitchenId,
                KitchenName      = s.CentralKitchen != null ? s.CentralKitchen.Name : null,
                ShipmentDate     = s.ShipmentDate,
                DeliveryStatus   = s.DeliveryStatus,
                ReceivedDate     = s.ReceivedDate,
                Lines            = s.ShipmentLines.Select(l => new ShipmentLineDto
                {
                    ShipmentLineId   = l.ShipmentLineId,
                    ProductId        = l.ProductId,
                    ProductName      = l.Product != null ? l.Product.ProductName : null,
                    ShippedQuantity  = l.ShippedQuantity,
                    ReceivedQuantity = l.ReceivedQuantity,
                    DamagedQuantity  = l.DamagedQuantity
                }).ToList()
            });

    public async Task<List<ShipmentDto>> GetAllShipmentsAsync()
        => await ProjectedQuery.ToListAsync();

    public async Task<ShipmentDto?> GetShipmentByIdAsync(int id)
        => await ProjectedQuery.FirstOrDefaultAsync(s => s.ShipmentId == id);

    public async Task<List<ShipmentDto>> GetShipmentsByOrderAsync(int orderId)
        => await ProjectedQuery.Where(s => s.StoreOrderId == orderId).ToListAsync();

    public async Task<ShipmentDto> CreateShipmentAsync(CreateShipmentModel model)
    {
        var entity = new Shipment
        {
            StoreOrderId     = model.StoreOrderId,
            CentralKitchenId = model.CentralKitchenId,
            ShipmentDate     = DateTime.UtcNow,
            DeliveryStatus   = "Pending"
        };

        await shipmentRepo.AddAsync(entity);

        foreach (var lineModel in model.Lines)
        {
            var line = new ShipmentLine
            {
                ShipmentId      = entity.ShipmentId,
                ProductId       = lineModel.ProductId,
                ShippedQuantity = lineModel.ShippedQuantity
            };
            await lineRepo.AddAsync(line);
        }

        return (await GetShipmentByIdAsync(entity.ShipmentId))!;
    }

    public async Task<ShipmentDto?> UpdateShipmentStatusAsync(int id, UpdateShipmentStatusModel model)
    {
        var entity = await shipmentRepo.FindAsync(id);
        if (entity is null) return null;

        entity.DeliveryStatus = model.DeliveryStatus;
        await shipmentRepo.UpdateAsync(entity);
        return await GetShipmentByIdAsync(id);
    }

    public async Task<ShipmentDto?> ReceiveShipmentAsync(int id, ReceiveShipmentModel model)
    {
        var entity = await shipmentRepo.FindAsync(id);
        if (entity is null) return null;

        foreach (var lineModel in model.Lines)
        {
            var line = await lineRepo.FindAsync(lineModel.ShipmentLineId);
            if (line is null || line.ShipmentId != id) continue;

            line.ReceivedQuantity = lineModel.ReceivedQuantity;
            line.DamagedQuantity  = lineModel.DamagedQuantity;
            await lineRepo.UpdateAsync(line);
        }

        entity.ReceivedDate   = DateTime.UtcNow;
        entity.DeliveryStatus = "Delivered";
        await shipmentRepo.UpdateAsync(entity);

        return await GetShipmentByIdAsync(id);
    }

    public async Task<bool> DeleteShipmentAsync(int id)
    {
        var entity = await shipmentRepo.FindAsync(id);
        if (entity is null) return false;

        entity.DeliveryStatus = "Cancelled";
        await shipmentRepo.UpdateAsync(entity);
        return true;
    }
}
