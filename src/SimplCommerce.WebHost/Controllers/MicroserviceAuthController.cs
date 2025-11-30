using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SimplCommerce.Module.Core.Models;

namespace SimplCommerce.WebHost.Controllers
{
    /// <summary>
    /// Demo: Microservice Integration - Admin Login via IdentityService
    /// Route: /api/microservice-auth/admin-login
    /// </summary>
    [Area("Core")]
    [Route("api/microservice-auth")]
    [ApiController]
    public class MicroserviceAuthController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<MicroserviceAuthController> _logger;

        public MicroserviceAuthController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            ILogger<MicroserviceAuthController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpPost("admin-login")]
        [AllowAnonymous]
        public async Task<IActionResult> AdminLogin([FromBody] AdminLoginRequest request)
        {
            try
            {
                // Step 1: Call IdentityService API
                var identityServiceUrl = _configuration["ServiceUrls:IdentityService"] ?? "https://localhost:5001";
                var client = _httpClientFactory.CreateClient();
                
                // Skip SSL validation for localhost (development only!)
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };
                var httpClient = new HttpClient(handler);

                var loginPayload = new
                {
                    email = request.Email,
                    password = request.Password
                };

                var jsonContent = JsonSerializer.Serialize(loginPayload);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                _logger.LogInformation($"Calling IdentityService at {identityServiceUrl}/api/auth/login");
                
                var response = await httpClient.PostAsync($"{identityServiceUrl}/api/auth/login", httpContent);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning($"IdentityService login failed: {response.StatusCode} - {errorContent}");
                    
                    return BadRequest(new
                    {
                        success = false,
                        message = "Login failed via IdentityService",
                        details = errorContent
                    });
                }

                // Step 2: Parse response from IdentityService
                var responseContent = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonSerializer.Deserialize<IdentityServiceLoginResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                _logger.LogInformation($"IdentityService login successful for user: {loginResponse?.Email}");

                // Step 3: Sign in to WebHost session (for admin panel to work)
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user != null)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation($"WebHost session created for admin: {user.Email}");
                }
                else
                {
                    _logger.LogWarning($"User {request.Email} not found in WebHost database. Session not created.");
                }

                // Step 4: Return success with JWT token
                return Ok(new
                {
                    success = true,
                    message = "✅ Login successful via IdentityService Microservice!",
                    token = loginResponse?.Token,
                    user = new
                    {
                        id = loginResponse?.Id,
                        email = loginResponse?.Email,
                        fullName = loginResponse?.FullName
                    },
                    microserviceUsed = "IdentityService",
                    microserviceUrl = identityServiceUrl
                });
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error calling IdentityService");
                return StatusCode(503, new
                {
                    success = false,
                    message = "IdentityService is not available. Make sure it's running on https://localhost:5001",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in microservice admin login");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error",
                    error = ex.Message
                });
            }
        }

        [HttpGet("test")]
        [AllowAnonymous]
        public IActionResult Test()
        {
            var identityServiceUrl = _configuration["ServiceUrls:IdentityService"] ?? "https://localhost:5001";
            return Ok(new
            {
                message = "Microservice Auth Controller is working!",
                identityServiceUrl = identityServiceUrl,
                endpoints = new[]
                {
                    "POST /api/microservice-auth/admin-login - Login via IdentityService",
                    "GET /api/microservice-auth/test - This test endpoint"
                }
            });
        }
    }

    // DTOs
    public class AdminLoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class IdentityServiceLoginResponse
    {
        public string Token { get; set; }
        public long Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
    }
}

