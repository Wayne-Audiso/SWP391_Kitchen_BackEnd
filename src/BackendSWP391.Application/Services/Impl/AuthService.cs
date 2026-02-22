using Microsoft.EntityFrameworkCore;
using BackendSWP391.Application.Models.User;
using BackendSWP391.Core.Models;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackendSWP391.DataAccess.Persistence;

namespace BackendSWP391.Application.Services.Impl
{
    public class AuthService : IAuthService
    {
        private readonly DatabaseContext _context;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

        public AuthService(DatabaseContext context, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }       

        public async Task<LoginResponseModel> LoginAsync(LoginUserModel request)
        {
            var user = await _context.UserInfos.Include(u => u.RoleNameNavigation)
                .FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null)
            {
                return null;
            }
            if (user.Password?.Trim() != request.Password?.Trim())
            {
                return null;
            }
            
            var token = Helpers.JwtHelper.GenerateToken(user, _configuration);

            return new LoginResponseModel
            {
                UserId = user.UserId,
                Username = user.Username,
                Role = user.RoleName,
                Email = user.Email,
                Token = token,
            };
        }
    }
}

