using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Braintree;
using Microsoft.AspNetCore.Mvc;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Infrastructure.Helpers;
using SimplCommerce.Module.Checkouts.Services;
using SimplCommerce.Module.Core.Extensions;
using SimplCommerce.Module.Core.Services;
using SimplCommerce.Module.Orders.Models;
using SimplCommerce.Module.Orders.Services;
using SimplCommerce.Module.PaymentBraintree.Models;
using SimplCommerce.Module.PaymentBraintree.Services;
using SimplCommerce.Module.Payments.Models;
using SimplCommerce.Module.Payments.MessageContracts;
using SimplCommerce.Module.Payments.Services;
using SimplCommerce.Module.ShoppingCart.Services;

namespace SimplCommerce.Module.PaymentBraintree.Areas.PaymentBraintree.Controllers
{
    [Area("PaymentBraintree")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class BraintreeController : Controller
    {
        private readonly ICheckoutService _checkoutService;
        private readonly IOrderService _orderService;
        private readonly IWorkContext _workContext;
        private readonly IRepositoryWithTypedId<PaymentProvider, string> _paymentProviderRepository;
        private readonly IRepository<Payment> _paymentRepository;
        private readonly IBraintreeConfiguration _braintreeConfiguration;
        private readonly ICurrencyService _currencyService;
        private readonly IPaymentMessageSender _paymentMessageSender;

        public BraintreeController(
            ICheckoutService checkoutService,
            IOrderService orderService,
            IWorkContext workContext,
            IRepositoryWithTypedId<PaymentProvider, string> paymentProviderRepository,
            IRepository<Payment> paymentRepository,
            IBraintreeConfiguration braintreeConfiguration,
            ICurrencyService currencyService,
            IPaymentMessageSender paymentMessageSender)
        {
            _checkoutService = checkoutService;
            _orderService = orderService;
            _workContext = workContext;
            _paymentProviderRepository = paymentProviderRepository;
            _paymentRepository = paymentRepository;
            _braintreeConfiguration = braintreeConfiguration;
            _currencyService = currencyService;
            _paymentMessageSender = paymentMessageSender;
        }

        [HttpPost]
        public async Task<IActionResult> Charge(string nonce, Guid checkoutId)
        {
            var curentUser = await _workContext.GetCurrentUser();
            var cart = await _checkoutService.GetCheckoutDetails(checkoutId);
            if (cart == null) return NotFound();

            var orderCreateResult = await _orderService.CreateOrder(checkoutId, PaymentProviderHelper.BraintreeProviderId, 0, OrderStatus.PendingPayment);
            if (!orderCreateResult.Success) return BadRequest(orderCreateResult.Error);

            var order = orderCreateResult.Value;

            var message = new PaymentMessage
            {
                OrderId = order.Id,
                CheckoutId = checkoutId,
                Amount = order.OrderTotal,
                PaymentProvider = PaymentProviderHelper.BraintreeProviderId,
                PaymentNonce = nonce,
                CreatedById = curentUser?.Id ?? 0
            };

            await _paymentMessageSender.EnqueueAsync(message);

            // Return accepted — processing will happen asynchronously by the queue consumer
            return Accepted(new { Status = "queued", OrderId = order.Id });
        }

        [HttpPost]
        public async Task<IActionResult> GetClientToken()
        {
            return Ok(await _braintreeConfiguration.GetClientToken());
        }
    }
}
