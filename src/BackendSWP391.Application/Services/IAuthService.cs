using BackendSWP391.Application.Models.User;

namespace BackendSWP391.Application.Services;

public interface IAuthService
{
    Task<LoginResponseModel?> LoginAsync(LoginUserModel request);
}
