using Microsoft.AspNetCore.Mvc;
using BackendSWP391.Application.Models.User;
using BackendSWP391.Application.Services;

namespace BackendSWP391.API.Controllers;

public class AuthController(IUserService userService) : ApiController
{
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginUserModel request)
    {
        return Ok(await userService.LoginAsync(request));
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserModel request)
    {
        if (request.Password != request.ConfirmPassword)
        {
            return BadRequest(new { message = "Passwords do not match" });
        }

        var createModel = new CreateUserModel
        {
            UserName = request.UserName,
            Email = request.Email,
            Password = request.Password
        };

        return Ok(await userService.CreateAsync(createModel));
    }
}

