using Microsoft.EntityFrameworkCore;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.PaymentBraintree.Models;
using SimplCommerce.Module.Payments.Models;

namespace SimplCommerce.Module.PaymentBraintree.Data
{
    public class PaymentBraintreeCustomModelBuilder : ICustomModelBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PaymentProvider>().HasData(
                new PaymentProvider("Braintree") {
                    Name = PaymentProviderHelper.BraintreeProviderId,
                    LandingViewComponentName = "BraintreeLanding",
                    ConfigureUrl = "payments-braintree-config",
                    IsEnabled = true,
                    AdditionalSettings =
                    "{" +
                        "\"PublicKey\": \"zd8x6q6w3v4prkjc\", " +
                        "\"PrivateKey\" : \"d1ce3dadeead4d9a5e191b6311ecbec3\", " +
                        "\"MerchantId\" : \"yxz35b6t57m27f25\", " +
                        "\"IsProduction\" : \"false\"" +
                    "}"
                }
            );
        }
    }
}
