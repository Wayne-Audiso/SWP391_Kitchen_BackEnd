using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BackendSWP391.Application.Models;
using BackendSWP391.Application.Models.Shipment;
using BackendSWP391.Application.Services;

namespace BackendSWP391.API.Controllers;

[Route("api/shipments")]
public class ShipmentsController(IShipmentService shipmentService) : ApiController
{
    /// <summary>GET /api/shipments — Lấy danh sách chuyến giao hàng kèm thông tin chi tiết.</summary>
    [Authorize(Roles = "Admin,Manager,Supply Coordinator")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await shipmentService.GetAllShipmentsAsync();
        return Ok(ApiResult<List<ShipmentDto>>.Ok(data, "Lấy danh sách chuyến giao hàng thành công"));
    }

    /// <summary>GET /api/shipments/{id} — Lấy chi tiết chuyến giao hàng kèm danh sách sản phẩm.</summary>
    [Authorize(Roles = "Admin,Manager,Supply Coordinator,Central Kitchen Staff")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await shipmentService.GetShipmentByIdAsync(id);
        if (result is null)
            return NotFound(ApiResult<ShipmentDto>.NotFound($"Không tìm thấy chuyến giao hàng với Id = {id}"));
        return Ok(ApiResult<ShipmentDto>.Ok(result, "Lấy thông tin chuyến giao hàng thành công"));
    }

    /// <summary>GET /api/shipments/by-order/{orderId} — Lọc chuyến giao hàng theo đơn hàng.</summary>
    [Authorize(Roles = "Admin,Manager,Supply Coordinator,Central Kitchen Staff")]
    [HttpGet("by-order/{orderId:int}")]
    public async Task<IActionResult> GetByOrder(int orderId)
    {
        var data = await shipmentService.GetShipmentsByOrderAsync(orderId);
        return Ok(ApiResult<List<ShipmentDto>>.Ok(data, "Lấy chuyến giao hàng theo đơn hàng thành công"));
    }

    /// <summary>
    /// POST /api/shipments — Tạo chuyến giao hàng kèm danh sách sản phẩm.
    /// Yêu cầu role Supply Coordinator hoặc Central Kitchen Staff.
    /// </summary>
    [Authorize(Roles = "Supply Coordinator,Central Kitchen Staff")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateShipmentModel model)
    {
        var result = await shipmentService.CreateShipmentAsync(model);
        return StatusCode(201, ApiResult<ShipmentDto>.Created(result, "Tạo chuyến giao hàng thành công"));
    }

    /// <summary>
    /// PUT /api/shipments/{id}/status — Cập nhật trạng thái giao hàng.
    /// Yêu cầu role Supply Coordinator hoặc Manager.
    /// </summary>
    [Authorize(Roles = "Supply Coordinator,Manager")]
    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateShipmentStatusModel model)
    {
        var result = await shipmentService.UpdateShipmentStatusAsync(id, model);
        if (result is null)
            return NotFound(ApiResult<ShipmentDto>.NotFound($"Không tìm thấy chuyến giao hàng với Id = {id}"));
        return Ok(ApiResult<ShipmentDto>.Ok(result, "Cập nhật trạng thái giao hàng thành công"));
    }

    /// <summary>
    /// PUT /api/shipments/{id}/receive — Xác nhận nhận hàng, cập nhật số lượng thực nhận và hàng hỏng.
    /// Yêu cầu role Franchise Store Staff hoặc Supply Coordinator.
    /// </summary>
    [Authorize(Roles = "Franchise Store Staff,Supply Coordinator")]
    [HttpPut("{id:int}/receive")]
    public async Task<IActionResult> Receive(int id, [FromBody] ReceiveShipmentModel model)
    {
        var result = await shipmentService.ReceiveShipmentAsync(id, model);
        if (result is null)
            return NotFound(ApiResult<ShipmentDto>.NotFound($"Không tìm thấy chuyến giao hàng với Id = {id}"));
        return Ok(ApiResult<ShipmentDto>.Ok(result, "Xác nhận nhận hàng thành công"));
    }

    /// <summary>DELETE /api/shipments/{id} — Hủy chuyến giao hàng (DeliveryStatus = Cancelled). Yêu cầu role Admin.</summary>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await shipmentService.DeleteShipmentAsync(id);
        if (!success)
            return NotFound(ApiResult<bool>.NotFound($"Không tìm thấy chuyến giao hàng với Id = {id}"));
        return Ok(ApiResult<bool>.Ok(true, "Hủy chuyến giao hàng thành công"));
    }
}
