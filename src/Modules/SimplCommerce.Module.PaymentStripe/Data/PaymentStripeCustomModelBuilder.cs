using Microsoft.EntityFrameworkCore;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Payments.Models;

namespace SimplCommerce.Module.PaymentStripe.Data
{
    public class PaymentStripeCustomModelBuilder : ICustomModelBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PaymentProvider>().HasData(
                new PaymentProvider("Stripe") { Name = "Stripe", LandingViewComponentName = "StripeLanding", ConfigureUrl = "payments-stripe-config", IsEnabled = true, AdditionalSettings = "{\"PublicKey\": \"YOUR_STRIPE_PUBLIC_KEY\", \"PrivateKey\" : \"YOUR_STRIPE_SECRET_KEY\"}" }
            );
        }
    }
}
