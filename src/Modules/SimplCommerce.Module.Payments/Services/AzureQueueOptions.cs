using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplCommerce.Module.Payments.Services
{
    public class AzureQueueOptions
    {
        public string ConnectionString { get; set; } = null!;
        public string QueueName { get; set; } = "bakeryqueuestorage";
    }
}
