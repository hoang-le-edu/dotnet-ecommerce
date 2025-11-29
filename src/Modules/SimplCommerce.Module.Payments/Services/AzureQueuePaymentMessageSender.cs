using System.Text.Json;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Options;
using SimplCommerce.Module.Payments.MessageContracts;

namespace SimplCommerce.Module.Payments.Services
{
    public class AzureQueuePaymentMessageSender : IPaymentMessageSender
    {
        private readonly QueueClient _queueClient;

        public AzureQueuePaymentMessageSender(IOptions<AzureQueueOptions> options)
        {
            var o = options.Value;
            var clientOptions = new QueueClientOptions
            {
                MessageEncoding = QueueMessageEncoding.None
            };
            _queueClient = new QueueClient(o.ConnectionString, o.QueueName, clientOptions);
            _queueClient.CreateIfNotExists();
        }

        public async Task EnqueueAsync(PaymentMessage message)
        {
            var json = JsonSerializer.Serialize(message);
            // The SDK will handle encoding; the message will be base64 encoded by the service.
            await _queueClient.SendMessageAsync(json);
        }
    }
}
