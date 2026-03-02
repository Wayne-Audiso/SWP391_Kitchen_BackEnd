using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using BackendSWP391.Application.Models;
using BackendSWP391.Application.Models.User;
using BackendSWP391.Application.Services;

namespace BackendSWP391.API.Controllers;

public class UsersController(IUserService userService) : ApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserModel model)
    {
        var result = await userService.CreateAsync(model);
        return StatusCode(201, ApiResult<CreateUserResponseModel>.Created(result, "Tạo người dùng thành công"));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(currentUserId))
            currentUserId = Request.Headers["X-User-ID"].ToString();

        var data = await userService.GetAllAsync(currentUserId);
        return Ok(ApiResult<IEnumerable<UserResponseModel>>.Ok(data, "Lấy danh sách người dùng thành công"));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await userService.GetByIdAsync(id);
        return Ok(ApiResult<UserResponseModel>.Ok(result, "Lấy thông tin người dùng thành công"));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserModel model)
    {
        var result = await userService.UpdateAsync(id, model);
        return Ok(ApiResult<UpdateUserResponseModel>.Ok(result, "Cập nhật thông tin người dùng thành công"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await userService.DeleteAsync(id);
        if (!success)
            return NotFound(ApiResult<bool>.NotFound($"Không tìm thấy người dùng với Id = {id}"));

        return Ok(ApiResult<bool>.Ok(true, "Xóa người dùng thành công"));
    }
}
