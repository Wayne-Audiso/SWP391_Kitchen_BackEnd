using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BackendSWP391.DataAccess.Identity;
using BackendSWP391.DataAccess.Persistence;
using BackendSWP391.DataAccess.Repositories;
using BackendSWP391.DataAccess.Repositories.Impl;

namespace BackendSWP391.DataAccess;

public static class DataAccessDependencyInjection
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DatabaseContext>(options =>
            options.UseSqlServer(configuration["Database:ConnectionString"],
                builder => builder.MigrationsAssembly(typeof(DatabaseContext).Assembly.FullName)));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<DatabaseContext>()
            .AddDefaultTokenProviders();

        // Đăng ký Generic Repository dưới dạng open-generic — một dòng cho tất cả entity
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        return services;
    }
}

