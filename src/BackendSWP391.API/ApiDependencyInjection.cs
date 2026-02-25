using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace BackendSWP391.API;

public static class ApiDependencyInjection
{
    public static void AddJwt(this IServiceCollection services, IConfiguration configuration)
    {
        var secretKey = configuration["JwtConfiguration:SecretKey"]!;
        var issuer    = configuration["JwtConfiguration:Issuer"]!;
        var audience  = configuration["JwtConfiguration:Audience"]!;

        var key = Encoding.ASCII.GetBytes(secretKey);

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken            = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey         = new SymmetricSecurityKey(key),

                    ValidateIssuer   = true,
                    ValidIssuer      = issuer,

                    ValidateAudience = true,
                    ValidAudience    = audience,

                    ValidateLifetime = true,
                    ClockSkew        = TimeSpan.Zero   // không cho phép trễ đồng hồ
                };
            });

        services.AddAuthorization();
    }

    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(s =>
        {
            s.SwaggerDoc("v1", new OpenApiInfo
            {
                Title   = "FranchiseStore & CentralKitchen API",
                Version = "v1"
            });

            // Thêm ô nhập Bearer token trong Swagger UI
            s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization. Nhập: Bearer {token}",
                Name        = "Authorization",
                In          = ParameterLocation.Header,
                Type        = SecuritySchemeType.ApiKey,
                Scheme      = "Bearer"
            });

            s.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id   = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }
}
