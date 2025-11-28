using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimplCommerce.ServiceCommunication.Clients;
using SimplCommerce.ServiceCommunication.Configuration;

namespace SimplCommerce.ServiceCommunication.Extensions;

/// <summary>
/// Extension methods for registering service communication dependencies
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add Identity Service client for inter-service communication
    /// </summary>
    public static IServiceCollection AddIdentityServiceClient(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.Configure<ServiceUrls>(configuration.GetSection(ServiceUrls.SectionName));
        
        services.AddHttpClient<IIdentityServiceClient, IdentityServiceClient>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        return services;
    }
}

