using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SimplCommerce.Module.Core.Models;

namespace SimplCommerce.IdentityService.Services;

/// <summary>
/// Implementation of JWT token service
/// </summary>
public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationMinutes;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
        _secretKey = _configuration["JwtSettings:SecretKey"] ?? "YourSuperSecretKeyForJwtToken2024!@#";
        _issuer = _configuration["JwtSettings:Issuer"] ?? "SimplCommerce.IdentityService";
        _audience = _configuration["JwtSettings:Audience"] ?? "SimplCommerce.Services";
        _expirationMinutes = int.Parse(_configuration["JwtSettings:ExpirationMinutes"] ?? "60");
    }

    public string GenerateAccessToken(User user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Name, user.FullName ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("userId", user.Id.ToString()),
            new Claim("fullName", user.FullName ?? string.Empty)
        };

        // Add roles
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: GetTokenExpiration(),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public DateTime GetTokenExpiration()
    {
        return DateTime.UtcNow.AddMinutes(_expirationMinutes);
    }
}

