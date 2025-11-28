namespace SimplCommerce.ServiceCommunication.DTOs;

/// <summary>
/// Data Transfer Object for User information shared between services
/// </summary>
public class UserDto
{
    public long Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
}

/// <summary>
/// Request model for user registration
/// </summary>
public class RegisterUserRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
}

/// <summary>
/// Request model for user login
/// </summary>
public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Response model for authentication (login/register)
/// </summary>
public class AuthResponse
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public UserDto? User { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

/// <summary>
/// Request model for validating user from OrderService
/// </summary>
public class ValidateUserRequest
{
    public long UserId { get; set; }
}

/// <summary>
/// Response model for user validation
/// </summary>
public class ValidateUserResponse
{
    public bool IsValid { get; set; }
    public UserDto? User { get; set; }
    public string? ErrorMessage { get; set; }
}

