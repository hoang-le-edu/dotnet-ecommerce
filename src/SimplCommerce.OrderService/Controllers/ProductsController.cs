using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Catalog.Models;

namespace SimplCommerce.OrderService.Controllers;

/// <summary>
/// Controller for product catalog (read-only for order service)
/// </summary>
[ApiController]
[Route("api/microservices/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IRepository<Product> _productRepository;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IRepository<Product> productRepository,
        ILogger<ProductsController> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    /// <summary>
    /// Get list of available products
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="categoryId">Filter by category</param>
    /// <returns>List of products</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ProductListResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ProductListResponse>> GetProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] long? categoryId = null)
    {
        var query = _productRepository.Query()
            .Include(p => p.ThumbnailImage)
            .Where(p => p.IsPublished && !p.IsDeleted && p.IsAllowToOrder);

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.Categories.Any(c => c.CategoryId == categoryId.Value));
        }

        var totalCount = await query.CountAsync();

        var products = await query
            .OrderByDescending(p => p.CreatedOn)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                ShortDescription = p.ShortDescription,
                Price = p.Price,
                OldPrice = p.OldPrice,
                SpecialPrice = p.SpecialPrice,
                StockQuantity = p.StockQuantity,
                IsInStock = !p.StockTrackingIsEnabled || p.StockQuantity > 0,
                ThumbnailUrl = p.ThumbnailImage != null ? p.ThumbnailImage.FileName : null,
                RatingAverage = p.RatingAverage,
                ReviewsCount = p.ReviewsCount
            })
            .ToListAsync();

        return Ok(new ProductListResponse
        {
            Products = products,
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = pageSize
        });
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Product details</returns>
    [HttpGet("{id:long}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetById(long id)
    {
        var product = await _productRepository.Query()
            .Include(p => p.ThumbnailImage)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        if (product == null)
        {
            return NotFound(new { message = $"Product {id} not found" });
        }

        return Ok(new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Slug = product.Slug,
            ShortDescription = product.ShortDescription,
            Description = product.Description,
            Price = product.Price,
            OldPrice = product.OldPrice,
            SpecialPrice = product.SpecialPrice,
            StockQuantity = product.StockQuantity,
            IsInStock = !product.StockTrackingIsEnabled || product.StockQuantity > 0,
            ThumbnailUrl = product.ThumbnailImage?.FileName,
            RatingAverage = product.RatingAverage,
            ReviewsCount = product.ReviewsCount,
            Sku = product.Sku,
            IsPublished = product.IsPublished,
            IsAllowToOrder = product.IsAllowToOrder
        });
    }
}

/// <summary>
/// Product DTO for API responses
/// </summary>
public class ProductDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? OldPrice { get; set; }
    public decimal? SpecialPrice { get; set; }
    public int StockQuantity { get; set; }
    public bool IsInStock { get; set; }
    public string? ThumbnailUrl { get; set; }
    public double? RatingAverage { get; set; }
    public int ReviewsCount { get; set; }
    public string? Sku { get; set; }
    public bool IsPublished { get; set; }
    public bool IsAllowToOrder { get; set; }
}

/// <summary>
/// Product list response
/// </summary>
public class ProductListResponse
{
    public List<ProductDto> Products { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

