namespace SimplCommerce.ServiceCommunication.Configuration;

/// <summary>
/// Configuration class for service URLs
/// </summary>
public class ServiceUrls
{
    public const string SectionName = "ServiceUrls";
    
    /// <summary>
    /// Base URL for Identity Service (e.g., https://app-identity-service.azurewebsites.net)
    /// </summary>
    public string IdentityService { get; set; } = "https://localhost:5001";
    
    /// <summary>
    /// Base URL for Order Service (e.g., https://app-order-service.azurewebsites.net)
    /// </summary>
    public string OrderService { get; set; } = "https://localhost:5002";
}

