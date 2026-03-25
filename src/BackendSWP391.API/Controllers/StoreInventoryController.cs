using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BackendSWP391.Application.Models;
using BackendSWP391.Application.Models.StoreInventory;
using BackendSWP391.Application.Services;

namespace BackendSWP391.API.Controllers;

[Route("api/store-inventory")]
public class StoreInventoryController(IStoreInventoryService storeInventoryService) : ApiController
{
    /// <summary>
    /// GET /api/store-inventory/{storeId}/stock
    /// Lấy danh sách tồn kho nguyên liệu của Franchise Store.
    /// </summary>
    [Authorize(Roles = "Franchise Store Staff,Supply Coordinator,Admin")]
    [HttpGet("{storeId:int}/stock")]
    public async Task<IActionResult> GetStock(int storeId)
    {
        var data = await storeInventoryService.GetStoreStockAsync(storeId);
        return Ok(ApiResult<List<StoreStockDto>>.Ok(data, "Lấy tồn kho cửa hàng thành công"));
    }

    /// <summary>
    /// POST /api/store-inventory/{storeId}/sell
    /// Ghi nhận bán hàng: trừ nguyên liệu theo Recipe và tạo bản ghi OperatingCost.
    /// Body: { productId: int, quantity: int }
    /// </summary>
    [Authorize(Roles = "Franchise Store Staff")]
    [HttpPost("{storeId:int}/sell")]
    public async Task<IActionResult> Sell(int storeId, [FromBody] SellAtStoreModel model)
    {
        await storeInventoryService.SellAtStoreAsync(storeId, model.ProductId, model.Quantity);
        return Ok(ApiResult<object>.Ok(null, "Ghi nhận bán hàng thành công, kho đã được cập nhật"));
    }

    /// <summary>
    /// GET /api/store-inventory/{storeId}/costs?costType=WasteCost|OperatingCost
    /// Lấy danh sách bản ghi chi phí. costType không bắt buộc.
    /// </summary>
    [Authorize(Roles = "Franchise Store Staff,Manager,Admin")]
    [HttpGet("{storeId:int}/costs")]
    public async Task<IActionResult> GetCosts(int storeId, [FromQuery] string? costType)
    {
        var data = await storeInventoryService.GetCostRecordsAsync(storeId, costType);
        return Ok(ApiResult<List<StoreCostRecordDto>>.Ok(data, "Lấy danh sách chi phí thành công"));
    }

    /// <summary>
    /// POST /api/store-inventory/{storeId}/process-expired
    /// Kiểm tra và xử lý nguyên liệu hết hạn: về 0 tồn kho + tạo WasteCost.
    /// </summary>
    [Authorize(Roles = "Franchise Store Staff,Admin")]
    [HttpPost("{storeId:int}/process-expired")]
    public async Task<IActionResult> ProcessExpired(int storeId)
    {
        await storeInventoryService.ProcessExpiredItemsAsync(storeId);
        return Ok(ApiResult<object>.Ok(null, "Kiểm tra hàng hết hạn hoàn tất"));
    }
}
