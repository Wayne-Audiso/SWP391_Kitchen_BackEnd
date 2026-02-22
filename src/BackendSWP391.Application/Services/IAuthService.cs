using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using BackendSWP391.Application.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendSWP391.Application.Services
{
    public interface IAuthService
    {
        Task<LoginResponseModel> LoginAsync(LoginUserModel request);
    }
}

