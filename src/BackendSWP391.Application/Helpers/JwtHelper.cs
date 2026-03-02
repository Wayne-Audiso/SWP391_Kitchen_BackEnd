using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BackendSWP391.DataAccess.Identity;

namespace BackendSWP391.Application.Helpers;

public static class JwtHelper
{
    public static string GenerateToken(ApplicationUser user, IEnumerable<string> roles, IConfiguration configuration)
    {
        var (key, issuer, audience, expiryDays) = ReadConfig(configuration);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name,           user.UserName ?? ""),
            new Claim(ClaimTypes.Email,          user.Email    ?? "")
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject            = new ClaimsIdentity(claims),
            Issuer             = issuer,
            Audience           = audience,
            Expires            = DateTime.UtcNow.AddDays(expiryDays),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(handler.CreateToken(tokenDescriptor));
    }

    private static (byte[] key, string issuer, string audience, int expiryDays)
        ReadConfig(IConfiguration configuration)
    {
        var secretKey  = configuration["JwtConfiguration:SecretKey"]
                         ?? throw new InvalidOperationException("JwtConfiguration:SecretKey is missing.");
        var issuer     = configuration["JwtConfiguration:Issuer"]   ?? "FranchiseKitchenAPI";
        var audience   = configuration["JwtConfiguration:Audience"] ?? "FranchiseKitchenClients";
        var expiryDays = configuration.GetValue<int>("JwtConfiguration:ExpiryInDays", 7);

        return (Encoding.ASCII.GetBytes(secretKey), issuer, audience, expiryDays);
    }
}
