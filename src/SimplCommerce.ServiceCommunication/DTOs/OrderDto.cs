namespace SimplCommerce.ServiceCommunication.DTOs;

/// <summary>
/// Data Transfer Object for Order information
/// </summary>
public class OrderDto
{
    public long Id { get; set; }
    public long CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string OrderStatus { get; set; } = string.Empty;
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ShippingAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal OrderTotal { get; set; }
    public string? PaymentMethod { get; set; }
    public string? ShippingMethod { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}

/// <summary>
/// Data Transfer Object for Order Item
/// </summary>
public class OrderItemDto
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductImage { get; set; }
    public int Quantity { get; set; }
    public decimal ProductPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total => (ProductPrice * Quantity) - DiscountAmount + TaxAmount;
}

/// <summary>
/// Request model for creating an order
/// </summary>
public class CreateOrderRequest
{
    public long CustomerId { get; set; }
    public List<CreateOrderItemRequest> Items { get; set; } = new();
    public string? CouponCode { get; set; }
    public string PaymentMethod { get; set; } = "CoD";
    public string ShippingMethod { get; set; } = "Free";
    public ShippingAddressRequest? ShippingAddress { get; set; }
}

/// <summary>
/// Request model for order item
/// </summary>
public class CreateOrderItemRequest
{
    public long ProductId { get; set; }
    public int Quantity { get; set; }
}

/// <summary>
/// Request model for shipping address
/// </summary>
public class ShippingAddressRequest
{
    public string ContactName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string? ZipCode { get; set; }
    public string CountryId { get; set; } = "VN";
    public long StateOrProvinceId { get; set; }
    public long? DistrictId { get; set; }
}

/// <summary>
/// Response model for order creation
/// </summary>
public class CreateOrderResponse
{
    public bool Success { get; set; }
    public OrderDto? Order { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Response model for order list
/// </summary>
public class OrderListResponse
{
    public List<OrderDto> Orders { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

