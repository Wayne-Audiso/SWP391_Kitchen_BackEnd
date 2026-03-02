using Microsoft.AspNetCore.Mvc;
using BackendSWP391.Application.Models;
using BackendSWP391.Application.Models.User;
using BackendSWP391.Application.Services;

namespace BackendSWP391.API.Controllers;

[Route("api/auth")]
public class AuthController(IUserService userService) : ApiController
{
    /// <summary>
    /// POST /api/auth/login
    /// Xác thực người dùng và trả về JWT token.
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserModel request)
    {
        var result = await userService.LoginAsync(request);
        return Ok(ApiResult<LoginResponseModel>.Ok(result, "Đăng nhập thành công"));
    }

    /// <summary>
    /// POST /api/auth/register
    /// Đăng ký tài khoản mới. Role mặc định: "Franchise Store Staff" nếu không truyền.
    /// Các role hợp lệ: Admin, Manager, Franchise Store Staff, Central Kitchen Staff, Supply Coordinator
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserModel request)
    {
        if (request.Password != request.ConfirmPassword)
            return BadRequest(ApiResult<object>.BadRequest(
                "Mật khẩu không khớp",
                new[] { "Password và ConfirmPassword phải giống nhau." }));

        var createModel = new CreateUserModel
        {
            UserName = request.UserName,
            Email    = request.Email,
            Password = request.Password,
            Role     = request.Role
        };

        var result = await userService.CreateAsync(createModel);
        return StatusCode(201, ApiResult<CreateUserResponseModel>.Created(result, "Đăng ký tài khoản thành công"));
    }
}
