using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.ShoppingCart.Models;
using SimplCommerce.Module.Catalog.Models;

namespace SimplCommerce.OrderService.Controllers;

/// <summary>
/// Controller for shopping cart management
/// </summary>
[ApiController]
[Route("api/microservices/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly IRepository<CartItem> _cartItemRepository;
    private readonly IRepository<Product> _productRepository;
    private readonly ILogger<CartController> _logger;

    public CartController(
        IRepository<CartItem> cartItemRepository,
        IRepository<Product> productRepository,
        ILogger<CartController> logger)
    {
        _cartItemRepository = cartItemRepository;
        _productRepository = productRepository;
        _logger = logger;
    }

    /// <summary>
    /// Get current user's cart
    /// </summary>
    /// <returns>Cart items</returns>
    [HttpGet]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<CartResponse>> GetCart()
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var cartItems = await _cartItemRepository.Query()
            .Include(ci => ci.Product)
            .ThenInclude(p => p.ThumbnailImage)
            .Where(ci => ci.CustomerId == userId)
            .ToListAsync();

        var response = new CartResponse
        {
            Items = cartItems.Select(ci => new CartItemDto
            {
                Id = ci.Id,
                ProductId = ci.ProductId,
                ProductName = ci.Product?.Name ?? "",
                ProductImage = ci.Product?.ThumbnailImage?.FileName,
                ProductPrice = ci.Product?.Price ?? 0,
                Quantity = ci.Quantity,
                Total = (ci.Product?.Price ?? 0) * ci.Quantity
            }).ToList()
        };

        response.SubTotal = response.Items.Sum(i => i.Total);
        response.ItemCount = response.Items.Sum(i => i.Quantity);

        return Ok(response);
    }

    /// <summary>
    /// Add item to cart
    /// </summary>
    /// <param name="request">Add to cart request</param>
    /// <returns>Updated cart</returns>
    [HttpPost("items")]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CartResponse>> AddToCart([FromBody] AddToCartRequest request)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        // Validate product
        var product = await _productRepository.Query()
            .FirstOrDefaultAsync(p => p.Id == request.ProductId && !p.IsDeleted && p.IsPublished);

        if (product == null)
        {
            return BadRequest(new { message = "Product not found or not available" });
        }

        if (!product.IsAllowToOrder)
        {
            return BadRequest(new { message = "Product is not available for ordering" });
        }

        // Check stock
        if (product.StockTrackingIsEnabled && product.StockQuantity < request.Quantity)
        {
            return BadRequest(new { message = $"Only {product.StockQuantity} items available" });
        }

        // Check if item already in cart
        var existingItem = await _cartItemRepository.Query()
            .FirstOrDefaultAsync(ci => ci.CustomerId == userId && ci.ProductId == request.ProductId);

        if (existingItem != null)
        {
            existingItem.Quantity += request.Quantity;
        }
        else
        {
            var cartItem = new CartItem
            {
                CustomerId = userId,
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                CreatedOn = DateTimeOffset.Now
            };
            _cartItemRepository.Add(cartItem);
        }

        await _cartItemRepository.SaveChangesAsync();

        _logger.LogInformation("Added product {ProductId} to cart for user {UserId}", request.ProductId, userId);

        return await GetCart();
    }

    /// <summary>
    /// Update cart item quantity
    /// </summary>
    /// <param name="itemId">Cart item ID</param>
    /// <param name="request">Update request</param>
    /// <returns>Updated cart</returns>
    [HttpPut("items/{itemId:long}")]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CartResponse>> UpdateCartItem(long itemId, [FromBody] UpdateCartItemRequest request)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var cartItem = await _cartItemRepository.Query()
            .Include(ci => ci.Product)
            .FirstOrDefaultAsync(ci => ci.Id == itemId && ci.CustomerId == userId);

        if (cartItem == null)
        {
            return NotFound(new { message = "Cart item not found" });
        }

        if (request.Quantity <= 0)
        {
            _cartItemRepository.Remove(cartItem);
        }
        else
        {
            // Check stock
            if (cartItem.Product.StockTrackingIsEnabled && cartItem.Product.StockQuantity < request.Quantity)
            {
                return BadRequest(new { message = $"Only {cartItem.Product.StockQuantity} items available" });
            }
            cartItem.Quantity = request.Quantity;
        }

        await _cartItemRepository.SaveChangesAsync();

        return await GetCart();
    }

    /// <summary>
    /// Remove item from cart
    /// </summary>
    /// <param name="itemId">Cart item ID</param>
    /// <returns>Updated cart</returns>
    [HttpDelete("items/{itemId:long}")]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CartResponse>> RemoveFromCart(long itemId)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var cartItem = await _cartItemRepository.Query()
            .FirstOrDefaultAsync(ci => ci.Id == itemId && ci.CustomerId == userId);

        if (cartItem == null)
        {
            return NotFound(new { message = "Cart item not found" });
        }

        _cartItemRepository.Remove(cartItem);
        await _cartItemRepository.SaveChangesAsync();

        _logger.LogInformation("Removed item {ItemId} from cart for user {UserId}", itemId, userId);

        return await GetCart();
    }

    /// <summary>
    /// Clear all items from cart
    /// </summary>
    /// <returns>Empty cart</returns>
    [HttpDelete]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<CartResponse>> ClearCart()
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var cartItems = await _cartItemRepository.Query()
            .Where(ci => ci.CustomerId == userId)
            .ToListAsync();

        foreach (var item in cartItems)
        {
            _cartItemRepository.Remove(item);
        }

        await _cartItemRepository.SaveChangesAsync();

        _logger.LogInformation("Cleared cart for user {UserId}", userId);

        return Ok(new CartResponse());
    }
}

#region DTOs

public class CartResponse
{
    public List<CartItemDto> Items { get; set; } = new();
    public decimal SubTotal { get; set; }
    public int ItemCount { get; set; }
}

public class CartItemDto
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductImage { get; set; }
    public decimal ProductPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Total { get; set; }
}

public class AddToCartRequest
{
    public long ProductId { get; set; }
    public int Quantity { get; set; } = 1;
}

public class UpdateCartItemRequest
{
    public int Quantity { get; set; }
}

#endregion

