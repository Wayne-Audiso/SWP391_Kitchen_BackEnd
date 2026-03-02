using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BackendSWP391.Application.Models;
using BackendSWP391.Application.Models.StoreOrder;
using BackendSWP391.Application.Services;

namespace BackendSWP391.API.Controllers;

[Route("api/store-orders")]
public class StoreOrdersController(IStoreOrderService orderService) : ApiController
{
    /// <summary>GET /api/store-orders — Lấy danh sách đơn hàng kèm tên bếp và tên cửa hàng.</summary>
    [Authorize(Roles = "Admin,Manager,Supply Coordinator")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await orderService.GetAllOrdersAsync();
        return Ok(ApiResult<List<StoreOrderDto>>.Ok(data, "Lấy danh sách đơn hàng thành công"));
    }

    /// <summary>GET /api/store-orders/{id}</summary>
    [Authorize(Roles = "Admin,Manager,Supply Coordinator,Central Kitchen Staff")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await orderService.GetOrderByIdAsync(id);
        if (result is null)
            return NotFound(ApiResult<StoreOrderDto>.NotFound($"Không tìm thấy đơn hàng với Id = {id}"));
        return Ok(ApiResult<StoreOrderDto>.Ok(result, "Lấy thông tin đơn hàng thành công"));
    }

    /// <summary>GET /api/store-orders/by-store/{storeId} — Lọc theo cửa hàng.</summary>
    [Authorize(Roles = "Admin,Manager,Supply Coordinator,Franchise Store Staff")]
    [HttpGet("by-store/{storeId:int}")]
    public async Task<IActionResult> GetByStore(int storeId)
    {
        var data = await orderService.GetOrdersByStoreAsync(storeId);
        return Ok(ApiResult<List<StoreOrderDto>>.Ok(data, "Lấy đơn hàng theo cửa hàng thành công"));
    }

    /// <summary>POST /api/store-orders — Tạo đơn hàng mới. Yêu cầu role Franchise Store Staff hoặc Supply Coordinator.</summary>
    [Authorize(Roles = "Franchise Store Staff,Supply Coordinator")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStoreOrderModel model)
    {
        var result = await orderService.CreateOrderAsync(model);
        return StatusCode(201, ApiResult<StoreOrderDto>.Created(result, "Tạo đơn hàng thành công"));
    }

    /// <summary>
    /// PUT /api/store-orders/{id}/status — Cập nhật trạng thái đơn hàng.
    /// Luồng: Pending → Approved/Rejected → InProduction → InDelivery → Completed
    /// Yêu cầu role Supply Coordinator hoặc Manager.
    /// </summary>
    [Authorize(Roles = "Supply Coordinator,Manager")]
    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStoreOrderStatusModel model)
    {
        var result = await orderService.UpdateOrderStatusAsync(id, model);
        if (result is null)
            return NotFound(ApiResult<StoreOrderDto>.NotFound($"Không tìm thấy đơn hàng với Id = {id}"));
        return Ok(ApiResult<StoreOrderDto>.Ok(result, "Cập nhật trạng thái đơn hàng thành công"));
    }

    /// <summary>DELETE /api/store-orders/{id} — Xóa mềm (Status = Inactive). Yêu cầu role Admin.</summary>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await orderService.DeleteOrderAsync(id);
        if (!success)
            return NotFound(ApiResult<bool>.NotFound($"Không tìm thấy đơn hàng với Id = {id}"));
        return Ok(ApiResult<bool>.Ok(true, "Xóa đơn hàng thành công"));
    }
}
