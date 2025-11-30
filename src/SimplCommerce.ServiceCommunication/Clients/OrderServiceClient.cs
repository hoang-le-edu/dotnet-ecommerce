using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimplCommerce.ServiceCommunication.Configuration;
using SimplCommerce.ServiceCommunication.DTOs;

namespace SimplCommerce.ServiceCommunication.Clients;

/// <summary>
/// HTTP Client implementation for communicating with Order Service
/// </summary>
public class OrderServiceClient : IOrderServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OrderServiceClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public OrderServiceClient(
        HttpClient httpClient,
        IOptions<ServiceUrls> serviceUrls,
        ILogger<OrderServiceClient> logger)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(serviceUrls.Value.OrderService);
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<OrderListResponse?> GetOrdersAsync(int page = 1, int pageSize = 10)
    {
        try
        {
            _logger.LogInformation("Getting orders from Order Service (Page: {Page}, Size: {PageSize})", page, pageSize);
            
            // Note: This endpoint requires Admin role. The HttpClient should be configured with an Admin token.
            // For this demo, we assume the OrderService allows this call or the token is propagated.
            var response = await _httpClient.GetAsync($"/api/microservices/orders/admin?page={page}&pageSize={pageSize}");
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<OrderListResponse>(_jsonOptions);
            }
            
            _logger.LogWarning("Failed to get orders. Status: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting orders from Order Service");
            return null;
        }
    }
}
