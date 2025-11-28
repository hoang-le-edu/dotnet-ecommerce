using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimplCommerce.Module.Core.Models;
using SimplCommerce.ServiceCommunication.DTOs;

namespace SimplCommerce.IdentityService.Controllers;

/// <summary>
/// Controller for user management and inter-service user validation
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        UserManager<User> userManager,
        ILogger<UsersController> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User information</returns>
    [HttpGet("{id:long}")]
    [AllowAnonymous] // Allow inter-service communication without token
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetById(long id)
    {
        _logger.LogInformation("Getting user by ID: {UserId}", id);

        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null || user.IsDeleted)
        {
            return NotFound(new { message = $"User with ID {id} not found." });
        }

        return Ok(new UserDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            IsActive = !user.IsDeleted,
            CreatedOn = user.CreatedOn
        });
    }

    /// <summary>
    /// Validate user for inter-service communication (called by OrderService)
    /// </summary>
    /// <param name="id">User ID to validate</param>
    /// <returns>Validation result with user info</returns>
    [HttpGet("{id:long}/validate")]
    [AllowAnonymous] // Allow inter-service communication
    [ProducesResponseType(typeof(ValidateUserResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ValidateUserResponse>> ValidateUser(long id)
    {
        _logger.LogInformation("Validating user ID: {UserId} (inter-service call)", id);

        var user = await _userManager.FindByIdAsync(id.ToString());
        
        if (user == null)
        {
            _logger.LogWarning("User validation failed: User {UserId} not found", id);
            return Ok(new ValidateUserResponse
            {
                IsValid = false,
                ErrorMessage = $"User with ID {id} not found."
            });
        }

        if (user.IsDeleted)
        {
            _logger.LogWarning("User validation failed: User {UserId} is deleted", id);
            return Ok(new ValidateUserResponse
            {
                IsValid = false,
                ErrorMessage = "User account has been deactivated."
            });
        }

        _logger.LogInformation("User {UserId} validated successfully", id);
        return Ok(new ValidateUserResponse
        {
            IsValid = true,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                IsActive = true,
                CreatedOn = user.CreatedOn
            }
        });
    }

    /// <summary>
    /// Get user by email
    /// </summary>
    /// <param name="email">User email</param>
    /// <returns>User information</returns>
    [HttpGet("by-email")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetByEmail([FromQuery] string email)
    {
        _logger.LogInformation("Getting user by email");

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || user.IsDeleted)
        {
            return NotFound(new { message = "User not found." });
        }

        return Ok(new UserDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            IsActive = !user.IsDeleted,
            CreatedOn = user.CreatedOn
        });
    }

    /// <summary>
    /// List all users (admin only)
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>List of users</returns>
    [HttpGet]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 20)
    {
        var users = await _userManager.Users
            .Where(u => !u.IsDeleted)
            .OrderByDescending(u => u.CreatedOn)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email ?? string.Empty,
                FullName = u.FullName,
                PhoneNumber = u.PhoneNumber,
                IsActive = !u.IsDeleted,
                CreatedOn = u.CreatedOn
            })
            .ToListAsync();

        return Ok(users);
    }

    /// <summary>
    /// Update user profile
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="request">Update data</param>
    /// <returns>Updated user</returns>
    [HttpPut("{id:long}")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<UserDto>> UpdateUser(long id, [FromBody] UpdateUserRequest request)
    {
        // Check if user is updating their own profile or is admin
        var currentUserId = User.FindFirst("userId")?.Value;
        var isAdmin = User.IsInRole("admin");
        
        if (currentUserId != id.ToString() && !isAdmin)
        {
            return Forbid();
        }

        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null || user.IsDeleted)
        {
            return NotFound(new { message = $"User with ID {id} not found." });
        }

        // Update fields
        if (!string.IsNullOrEmpty(request.FullName))
            user.FullName = request.FullName;
        
        if (!string.IsNullOrEmpty(request.PhoneNumber))
            user.PhoneNumber = request.PhoneNumber;

        user.LatestUpdatedOn = DateTimeOffset.Now;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
        }

        return Ok(new UserDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            IsActive = !user.IsDeleted,
            CreatedOn = user.CreatedOn
        });
    }
}

/// <summary>
/// Request model for updating user
/// </summary>
public class UpdateUserRequest
{
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
}

