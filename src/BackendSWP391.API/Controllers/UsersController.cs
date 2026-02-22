using Microsoft.AspNetCore.Mvc;
using BackendSWP391.Application.Models.User;
using BackendSWP391.Application.Services;

namespace BackendSWP391.API.Controllers;

public class UsersController(IUserService userService) : ApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserModel model)
    {
        return Ok(await userService.CreateAsync(model));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await userService.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        return Ok(await userService.GetByIdAsync(id));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserModel model)
    {
        return Ok(await userService.UpdateAsync(id, model));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        return Ok(await userService.DeleteAsync(id));
    }
}

