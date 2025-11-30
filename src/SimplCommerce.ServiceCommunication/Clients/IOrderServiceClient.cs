using SimplCommerce.ServiceCommunication.DTOs;

namespace SimplCommerce.ServiceCommunication.Clients;

/// <summary>
/// HTTP Client interface for communicating with Order Service
/// </summary>
public interface IOrderServiceClient
{
    /// <summary>
    /// Get all orders (Admin)
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>List of orders</returns>
    Task<OrderListResponse?> GetOrdersAsync(int page = 1, int pageSize = 10);
}
