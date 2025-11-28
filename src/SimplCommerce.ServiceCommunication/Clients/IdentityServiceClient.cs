using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimplCommerce.ServiceCommunication.Configuration;
using SimplCommerce.ServiceCommunication.DTOs;

namespace SimplCommerce.ServiceCommunication.Clients;

/// <summary>
/// HTTP Client implementation for communicating with Identity Service
/// </summary>
public class IdentityServiceClient : IIdentityServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<IdentityServiceClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public IdentityServiceClient(
        HttpClient httpClient,
        IOptions<ServiceUrls> serviceUrls,
        ILogger<IdentityServiceClient> logger)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(serviceUrls.Value.IdentityService);
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<ValidateUserResponse> ValidateUserAsync(long userId)
    {
        try
        {
            _logger.LogInformation("Validating user {UserId} with Identity Service", userId);
            
            var response = await _httpClient.GetAsync($"/api/users/{userId}/validate");
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ValidateUserResponse>(_jsonOptions);
                return result ?? new ValidateUserResponse 
                { 
                    IsValid = false, 
                    ErrorMessage = "Failed to deserialize response" 
                };
            }
            
            _logger.LogWarning("User validation failed for user {UserId}. Status: {StatusCode}", 
                userId, response.StatusCode);
            
            return new ValidateUserResponse
            {
                IsValid = false,
                ErrorMessage = $"User validation failed with status {response.StatusCode}"
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error while validating user {UserId}", userId);
            return new ValidateUserResponse
            {
                IsValid = false,
                ErrorMessage = $"Service communication error: {ex.Message}"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while validating user {UserId}", userId);
            return new ValidateUserResponse
            {
                IsValid = false,
                ErrorMessage = $"Unexpected error: {ex.Message}"
            };
        }
    }

    public async Task<UserDto?> GetUserByIdAsync(long userId)
    {
        try
        {
            _logger.LogInformation("Getting user {UserId} from Identity Service", userId);
            
            var response = await _httpClient.GetAsync($"/api/users/{userId}");
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserDto>(_jsonOptions);
            }
            
            _logger.LogWarning("Failed to get user {UserId}. Status: {StatusCode}", 
                userId, response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user {UserId}", userId);
            return null;
        }
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        try
        {
            _logger.LogInformation("Getting user by email from Identity Service");
            
            var response = await _httpClient.GetAsync($"/api/users/by-email?email={Uri.EscapeDataString(email)}");
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserDto>(_jsonOptions);
            }
            
            _logger.LogWarning("Failed to get user by email. Status: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by email");
            return null;
        }
    }
}

