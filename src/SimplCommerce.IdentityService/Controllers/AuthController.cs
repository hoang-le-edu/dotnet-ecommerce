using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimplCommerce.IdentityService.Services;
using SimplCommerce.Module.Core.Models;
using SimplCommerce.ServiceCommunication.DTOs;

namespace SimplCommerce.IdentityService.Controllers;

/// <summary>
/// Controller for authentication operations (Register, Login, Logout)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ITokenService tokenService,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user account
    /// </summary>
    /// <param name="request">Registration details</param>
    /// <returns>Authentication response with JWT token</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterUserRequest request)
    {
        _logger.LogInformation("Registration attempt for email: {Email}", request.Email);

        // Check if user already exists
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                ErrorMessage = "A user with this email already exists."
            });
        }

        // Create new user
        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            UserGuid = Guid.NewGuid()
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogWarning("Registration failed for {Email}: {Errors}", request.Email, errors);
            
            return BadRequest(new AuthResponse
            {
                Success = false,
                ErrorMessage = errors
            });
        }

        // Add default customer role
        await _userManager.AddToRoleAsync(user, "customer");
        
        _logger.LogInformation("User registered successfully: {Email}, ID: {UserId}", request.Email, user.Id);

        // Generate token
        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateAccessToken(user, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();

        return Ok(new AuthResponse
        {
            Success = true,
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = _tokenService.GetTokenExpiration(),
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
    /// Login with email and password
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>Authentication response with JWT token</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Login attempt for email: {Email}", request.Email);

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Unauthorized(new AuthResponse
            {
                Success = false,
                ErrorMessage = "Invalid email or password."
            });
        }

        if (user.IsDeleted)
        {
            return Unauthorized(new AuthResponse
            {
                Success = false,
                ErrorMessage = "This account has been deactivated."
            });
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Login failed for {Email}", request.Email);
            return Unauthorized(new AuthResponse
            {
                Success = false,
                ErrorMessage = "Invalid email or password."
            });
        }

        _logger.LogInformation("User logged in successfully: {Email}, ID: {UserId}", request.Email, user.Id);

        // Generate token
        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateAccessToken(user, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Update last login
        user.LatestUpdatedOn = DateTimeOffset.Now;
        await _userManager.UpdateAsync(user);

        return Ok(new AuthResponse
        {
            Success = true,
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = _tokenService.GetTokenExpiration(),
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                IsActive = !user.IsDeleted,
                CreatedOn = user.CreatedOn
            }
        });
    }

    /// <summary>
    /// Get current authenticated user info
    /// </summary>
    /// <returns>Current user information</returns>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Unauthorized();
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

