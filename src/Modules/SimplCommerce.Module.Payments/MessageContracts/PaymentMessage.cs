using System;

namespace SimplCommerce.Module.Payments.MessageContracts
{
    public record PaymentMessage
    {
        public long OrderId { get; init; }
        public Guid CheckoutId { get; init; }
        public decimal Amount { get; init; }
        public string PaymentProvider { get; init; } = null!;
        public string? PaymentNonce { get; init; }
        public long CreatedById { get; init; }
        public DateTimeOffset CreatedOn { get; init; } = DateTimeOffset.UtcNow;
    }
}
