using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using BackendSWP391.Application.Helpers;
using BackendSWP391.Application.Models.User;
using BackendSWP391.DataAccess.Persistence;

namespace BackendSWP391.Application.Services.Impl;

public class AuthService : IAuthService
{
    private readonly DatabaseContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(DatabaseContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<LoginResponseModel?> LoginAsync(LoginUserModel request)
    {
        // 1. Tìm user theo username (hoặc email)
        var user = await _context.UserInfos
            .Include(u => u.RoleNameNavigation)
            .FirstOrDefaultAsync(u => u.Username == request.Username
                                   || u.Email == request.Username);

        if (user is null)
            return null;

        // 2. Xác thực mật khẩu với BCrypt
        var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
        if (!isPasswordValid)
            return null;

        // 3. Sinh JWT token
        var token = JwtHelper.GenerateToken(user, _configuration);

        return new LoginResponseModel
        {
            UserId    = user.UserId,
            Username  = user.Username,
            Role      = user.RoleName,
            Email     = user.Email,
            Token     = token,
        };
    }
}
