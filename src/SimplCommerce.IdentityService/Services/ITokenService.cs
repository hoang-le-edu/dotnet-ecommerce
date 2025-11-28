using SimplCommerce.Module.Core.Models;

namespace SimplCommerce.IdentityService.Services;

/// <summary>
/// Service for JWT token generation and validation
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generate JWT access token for a user
    /// </summary>
    /// <param name="user">User to generate token for</param>
    /// <param name="roles">User roles</param>
    /// <returns>JWT token string</returns>
    string GenerateAccessToken(User user, IList<string> roles);
    
    /// <summary>
    /// Generate refresh token
    /// </summary>
    /// <returns>Refresh token string</returns>
    string GenerateRefreshToken();
    
    /// <summary>
    /// Get token expiration time
    /// </summary>
    DateTime GetTokenExpiration();
}

