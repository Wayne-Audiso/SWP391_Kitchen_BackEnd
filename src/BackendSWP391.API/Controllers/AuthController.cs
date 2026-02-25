using Microsoft.AspNetCore.Mvc;
using BackendSWP391.Application.Models.User;
using BackendSWP391.Application.Services;

namespace BackendSWP391.API.Controllers;

[Route("api/auth")]
public class AuthController(IAuthService authService, IUserService userService) : ApiController
{
    /// <summary>
    /// POST /api/auth/login
    /// Xác thực người dùng và trả về JWT token.
    /// Trường "username" chấp nhận cả username lẫn email.
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserModel request)
    {
        var result = await authService.LoginAsync(request);

        if (result is null)
            return Unauthorized(new { message = "Tên đăng nhập hoặc mật khẩu không đúng." });

        return Ok(result);
    }

    /// <summary>
    /// POST /api/auth/register
    /// Đăng ký tài khoản mới (dùng ASP.NET Identity).
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserModel request)
    {
        if (request.Password != request.ConfirmPassword)
            return BadRequest(new { message = "Passwords do not match!" });

        var createModel = new CreateUserModel
        {
            UserName = request.UserName,
            Email    = request.Email,
            Password = request.Password
        };

        return Ok(await userService.CreateAsync(createModel));
    }
}
