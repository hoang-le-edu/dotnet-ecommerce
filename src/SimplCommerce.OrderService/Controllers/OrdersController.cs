using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Orders.Models;
using SimplCommerce.ServiceCommunication.Clients;
using SimplCommerce.ServiceCommunication.DTOs;

namespace SimplCommerce.OrderService.Controllers;

/// <summary>
/// Controller for order management - Microservice endpoint
/// </summary>
[ApiController]
[Route("api/microservices/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IIdentityServiceClient _identityServiceClient;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(
        IRepository<Order> orderRepository,
        IIdentityServiceClient identityServiceClient,
        ILogger<OrdersController> logger)
    {
        _orderRepository = orderRepository;
        _identityServiceClient = identityServiceClient;
        _logger = logger;
    }

    /// <summary>
    /// Get all orders (for admin) - returns all orders without user filter
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>List of all orders</returns>
    [HttpGet("all")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(OrderListResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<OrderListResponse>> GetAllOrders(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 20)
    {
        _logger.LogInformation("Getting ALL orders (admin) - page: {Page}, pageSize: {PageSize}", page, pageSize);

        var query = _orderRepository.Query()
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .Include(o => o.Customer)
            .OrderByDescending(o => o.CreatedOn);

        var totalCount = await query.CountAsync();
        
        var orders = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var orderDtos = orders.Select(o => MapToOrderDto(o)).ToList();

        _logger.LogInformation("Retrieved {Count} orders out of {Total}", orderDtos.Count, totalCount);

        return Ok(new OrderListResponse
        {
            Orders = orderDtos,
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = pageSize
        });
    }

    /// <summary>
    /// Get all orders for the current user
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>List of orders</returns>
    [HttpGet]
    [ProducesResponseType(typeof(OrderListResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<OrderListResponse>> GetMyOrders(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Invalid user token" });
        }

        _logger.LogInformation("Getting orders for user {UserId}", userId);

        var query = _orderRepository.Query()
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .Where(o => o.CustomerId == userId)
            .OrderByDescending(o => o.CreatedOn);

        var totalCount = await query.CountAsync();
        
        var orders = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var orderDtos = orders.Select(o => MapToOrderDto(o)).ToList();

        return Ok(new OrderListResponse
        {
            Orders = orderDtos,
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = pageSize
        });
    }

    /// <summary>
    /// Get order by ID
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <returns>Order details</returns>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<OrderDto>> GetById(long id)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Invalid user token" });
        }

        var order = await _orderRepository.Query()
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .ThenInclude(p => p.ThumbnailImage)
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound(new { message = $"Order {id} not found" });
        }

        // Check if user owns this order or is admin
        var isAdmin = User.IsInRole("admin");
        if (order.CustomerId != userId && !isAdmin)
        {
            return Forbid();
        }

        return Ok(MapToOrderDto(order));
    }

    /// <summary>
    /// Create a new order (simplified version for demo)
    /// </summary>
    /// <param name="request">Order creation request</param>
    /// <returns>Created order</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CreateOrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CreateOrderResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateOrderResponse>> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var tokenUserId))
        {
            return Unauthorized(new CreateOrderResponse
            {
                Success = false,
                ErrorMessage = "Invalid user token"
            });
        }

        // Use customerId from request or from token
        var customerId = request.CustomerId > 0 ? request.CustomerId : tokenUserId;

        _logger.LogInformation("Creating order for customer {CustomerId}", customerId);

        // INTER-SERVICE COMMUNICATION: Validate user with Identity Service
        var validationResult = await _identityServiceClient.ValidateUserAsync(customerId);
        
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("User validation failed for customer {CustomerId}: {Error}", 
                customerId, validationResult.ErrorMessage);
            
            return BadRequest(new CreateOrderResponse
            {
                Success = false,
                ErrorMessage = $"User validation failed: {validationResult.ErrorMessage}"
            });
        }

        _logger.LogInformation("User {CustomerId} validated successfully via Identity Service", customerId);

        // Validate order items
        if (request.Items == null || !request.Items.Any())
        {
            return BadRequest(new CreateOrderResponse
            {
                Success = false,
                ErrorMessage = "Order must contain at least one item"
            });
        }

        // Create the order
        var order = new Order
        {
            CustomerId = customerId,
            CreatedById = tokenUserId,
            CreatedOn = DateTimeOffset.Now,
            LatestUpdatedOn = DateTimeOffset.Now,
            LatestUpdatedById = tokenUserId,
            OrderStatus = OrderStatus.New,
            PaymentMethod = request.PaymentMethod,
            ShippingMethod = request.ShippingMethod
        };

        // Add order items (simplified - in real scenario would validate products)
        decimal subTotal = 0;
        foreach (var item in request.Items)
        {
            var orderItem = new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                ProductPrice = 100, // Simplified - should fetch from product
            };
            order.AddOrderItem(orderItem);
            subTotal += orderItem.ProductPrice * orderItem.Quantity;
        }

        order.SubTotal = subTotal;
        order.OrderTotal = subTotal; // Simplified - no tax/shipping for demo

        _orderRepository.Add(order);
        await _orderRepository.SaveChangesAsync();

        _logger.LogInformation("Order {OrderId} created successfully for customer {CustomerId}", 
            order.Id, customerId);

        return CreatedAtAction(nameof(GetById), new { id = order.Id }, new CreateOrderResponse
        {
            Success = true,
            Order = new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                CustomerName = validationResult.User?.FullName ?? "",
                CustomerEmail = validationResult.User?.Email ?? "",
                OrderStatus = order.OrderStatus.ToString(),
                SubTotal = order.SubTotal,
                OrderTotal = order.OrderTotal,
                PaymentMethod = order.PaymentMethod,
                ShippingMethod = order.ShippingMethod,
                CreatedOn = order.CreatedOn,
                Items = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    ProductPrice = oi.ProductPrice
                }).ToList()
            }
        });
    }

    /// <summary>
    /// Cancel an order
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <returns>Result</returns>
    [HttpPost("{id:long}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelOrder(long id)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var order = await _orderRepository.Query()
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound(new { message = $"Order {id} not found" });
        }

        // Check ownership
        var isAdmin = User.IsInRole("admin");
        if (order.CustomerId != userId && !isAdmin)
        {
            return Forbid();
        }

        // Check if order can be cancelled
        if (order.OrderStatus != OrderStatus.New && order.OrderStatus != OrderStatus.PendingPayment)
        {
            return BadRequest(new { message = "Order cannot be cancelled in its current status" });
        }

        order.OrderStatus = OrderStatus.Canceled;
        order.LatestUpdatedOn = DateTimeOffset.Now;
        order.LatestUpdatedById = userId;

        await _orderRepository.SaveChangesAsync();

        _logger.LogInformation("Order {OrderId} cancelled by user {UserId}", id, userId);

        return Ok(new { message = "Order cancelled successfully" });
    }

    private static OrderDto MapToOrderDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            CustomerName = order.Customer?.FullName ?? "",
            CustomerEmail = order.Customer?.Email ?? "",
            OrderStatus = order.OrderStatus.ToString(),
            SubTotal = order.SubTotal,
            TaxAmount = order.TaxAmount,
            ShippingAmount = order.ShippingFeeAmount,
            DiscountAmount = order.DiscountAmount,
            OrderTotal = order.OrderTotal,
            PaymentMethod = order.PaymentMethod,
            ShippingMethod = order.ShippingMethod,
            CreatedOn = order.CreatedOn,
            Items = order.OrderItems.Select(oi => new OrderItemDto
            {
                Id = oi.Id,
                ProductId = oi.ProductId,
                ProductName = oi.Product?.Name ?? "",
                ProductImage = oi.Product?.ThumbnailImage?.FileName,
                Quantity = oi.Quantity,
                ProductPrice = oi.ProductPrice,
                DiscountAmount = oi.DiscountAmount,
                TaxAmount = oi.TaxAmount
            }).ToList()
        };
    }
}

