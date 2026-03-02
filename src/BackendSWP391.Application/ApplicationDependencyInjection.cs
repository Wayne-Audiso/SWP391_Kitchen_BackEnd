using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BackendSWP391.Application.Common.Email;
using BackendSWP391.Application.MappingProfiles;
using BackendSWP391.Application.Services;
using BackendSWP391.Application.Services.DevImpl;
using BackendSWP391.Application.Services.Impl;
using BackendSWP391.Shared.Services;
using BackendSWP391.Shared.Services.Impl;

namespace BackendSWP391.Application;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddServices(env);

        services.RegisterMapper();

        return services;
    }

    private static void AddServices(this IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IClaimService, ClaimService>();
        services.AddScoped<ITemplateService, TemplateService>();

        // ── Master Data services ─────────────────────────────────────────────
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IStoreService, StoreService>();
        services.AddScoped<IIngredientService, IngredientService>();
        services.AddScoped<IRecipeService, RecipeService>();

        // ── Inventory & Order services ────────────────────────────────────────
        services.AddScoped<IInventoryLocationService, InventoryLocationService>();
        services.AddScoped<IStoreOrderService, StoreOrderService>();
        services.AddScoped<IShipmentService, ShipmentService>();

        if (env.IsDevelopment())
            services.AddScoped<IEmailService, DevEmailService>();
        else
            services.AddScoped<IEmailService, EmailService>();
    }

    private static void RegisterMapper(this IServiceCollection services)
    {
        services.AddMapster();
    }

    public static void AddEmailConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration.GetSection("SmtpSettings").Get<SmtpSettings>());
    }
}

