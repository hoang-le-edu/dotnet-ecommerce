using SimplCommerce.ServiceCommunication.DTOs;

namespace SimplCommerce.ServiceCommunication.Clients;

/// <summary>
/// HTTP Client interface for communicating with Identity Service
/// </summary>
public interface IIdentityServiceClient
{
    /// <summary>
    /// Validate if a user exists and is active
    /// </summary>
    /// <param name="userId">User ID to validate</param>
    /// <returns>Validation result with user info if valid</returns>
    Task<ValidateUserResponse> ValidateUserAsync(long userId);
    
    /// <summary>
    /// Get user information by ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>User information or null if not found</returns>
    Task<UserDto?> GetUserByIdAsync(long userId);
    
    /// <summary>
    /// Get user information by email
    /// </summary>
    /// <param name="email">User email</param>
    /// <returns>User information or null if not found</returns>
    Task<UserDto?> GetUserByEmailAsync(string email);
}

