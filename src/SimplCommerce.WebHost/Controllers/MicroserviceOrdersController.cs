using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SimplCommerce.WebHost.Controllers
{
    /// <summary>
    /// Demo: Microservice Integration - Admin Orders List via OrderService
    /// Route: /api/microservice-orders/admin-list
    /// </summary>
    [Area("Orders")]
    [Authorize(Roles = "admin")]
    [Route("api/microservice-orders")]
    [ApiController]
    public class MicroserviceOrdersController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MicroserviceOrdersController> _logger;

        public MicroserviceOrdersController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<MicroserviceOrdersController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("admin-list")]
        public async Task<IActionResult> GetAdminOrdersList(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 20)
        {
            try
            {
                // Step 1: Get JWT token from request header (if available)
                var jwtToken = Request.Headers["X-JWT-Token"].FirstOrDefault();
                
                _logger.LogInformation($"Getting orders list from OrderService (page: {page}, pageSize: {pageSize})");

                // Step 2: Call OrderService API
                var orderServiceUrl = _configuration["ServiceUrls:OrderService"] ?? "https://localhost:5002";
                
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };
                var httpClient = new HttpClient(handler);

                // Add JWT token if available
                if (!string.IsNullOrEmpty(jwtToken))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                }

                // Call the /all endpoint for admin to get all orders
                var url = $"{orderServiceUrl}/api/microservices/Orders/all?page={page}&pageSize={pageSize}";
                _logger.LogInformation($"Calling OrderService at {url}");

                var response = await httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning($"OrderService call failed: {response.StatusCode} - {errorContent}");

                    return StatusCode((int)response.StatusCode, new
                    {
                        success = false,
                        message = "Failed to get orders from OrderService",
                        details = errorContent
                    });
                }

                // Step 3: Parse and return response
                var responseContent = await response.Content.ReadAsStringAsync();
                var ordersResponse = JsonSerializer.Deserialize<OrderServiceResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                _logger.LogInformation($"Retrieved {ordersResponse?.TotalCount} orders from OrderService");

                return Ok(new
                {
                    success = true,
                    message = "✅ Orders loaded from OrderService Microservice!",
                    microserviceUsed = "OrderService",
                    microserviceUrl = orderServiceUrl,
                    data = ordersResponse
                });
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error calling OrderService");
                return StatusCode(503, new
                {
                    success = false,
                    message = "OrderService is not available. Make sure it's running on https://localhost:5002",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders from microservice");
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
            var orderServiceUrl = _configuration["ServiceUrls:OrderService"] ?? "https://localhost:5002";
            return Ok(new
            {
                message = "Microservice Orders Controller is working!",
                orderServiceUrl = orderServiceUrl,
                userRole = User.IsInRole("admin") ? "admin" : "guest",
                isAuthenticated = User.Identity?.IsAuthenticated ?? false,
                endpoints = new[]
                {
                    "GET /api/microservice-orders/admin-list - Get orders from OrderService",
                    "GET /api/microservice-orders/test - This test endpoint"
                }
            });
        }

        [HttpGet("simple-list")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSimpleOrdersList()
        {
            try
            {
                // Simple endpoint for testing without auth
                var orderServiceUrl = _configuration["ServiceUrls:OrderService"] ?? "https://localhost:5002";
                
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };
                var httpClient = new HttpClient(handler);

                var url = $"{orderServiceUrl}/api/microservices/Orders?page=1&pageSize=10";
                var response = await httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, new
                    {
                        success = false,
                        message = "Failed to get orders"
                    });
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var ordersResponse = JsonSerializer.Deserialize<OrderServiceResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return Ok(new
                {
                    success = true,
                    message = $"✅ Retrieved {ordersResponse?.TotalCount} orders from OrderService",
                    orders = ordersResponse?.Orders?.Take(5).ToList(), // Show first 5 for demo
                    totalCount = ordersResponse?.TotalCount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }
    }

    // DTOs
    public class OrderServiceResponse
    {
        public List<OrderDto> Orders { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class OrderDto
    {
        public long Id { get; set; }
        public string OrderNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public decimal OrderTotal { get; set; }
        public string OrderStatus { get; set; }
        public DateTime CreatedOn { get; set; }
        public List<OrderItemDto> Items { get; set; }
    }

    public class OrderItemDto
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal ProductPrice { get; set; }
    }
}

