using System.Threading.Tasks;
using SimplCommerce.Module.Payments.MessageContracts;

namespace SimplCommerce.Module.Payments.Services
{
    public interface IPaymentMessageSender
    {
        Task EnqueueAsync(PaymentMessage message);
    }
}
