using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Infrastructure.Helpers;
using SimplCommerce.Module.Checkouts.Services;
using SimplCommerce.Module.Core.Extensions;
using SimplCommerce.Module.Core.Services;
using SimplCommerce.Module.Orders.Models;
using SimplCommerce.Module.Orders.Services;
using SimplCommerce.Module.Payments.Models;
using SimplCommerce.Module.PaymentStripe.Areas.PaymentStripe.ViewModels;
using SimplCommerce.Module.PaymentStripe.Models;
using SimplCommerce.Module.ShoppingCart.Services;
using SimplCommerce.Module.Payments.MessageContracts; 
using SimplCommerce.Module.Payments.Services;         
using Stripe;

namespace SimplCommerce.Module.PaymentStripe.Areas.PaymentStripe.Controllers
{
    [Area("PaymentStripe")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class StripeController : Controller
    {
        private readonly ICheckoutService _checkoutService;
        private readonly IOrderService _orderService;
        private readonly IWorkContext _workContext;
        private readonly IRepositoryWithTypedId<PaymentProvider, string> _paymentProviderRepository;
        private readonly IRepository<Payment> _paymentRepository;
        private readonly ICurrencyService _currencyService;
        private readonly IPaymentMessageSender _paymentMessageSender; // <-- DEPENDENCY MỚI

        public StripeController(
            ICheckoutService checkoutService,
            IOrderService orderService,
            IWorkContext workContext,
            IRepositoryWithTypedId<PaymentProvider, string> paymentProviderRepository,
            IRepository<Payment> paymentRepository,
            ICurrencyService currencyService,
            IPaymentMessageSender paymentMessageSender) // <-- THÊM VÀO CONSTRUCTOR
        {
            _checkoutService = checkoutService;
            _orderService = orderService;
            _workContext = workContext;
            _paymentProviderRepository = paymentProviderRepository;
            _paymentRepository = paymentRepository;
            _currencyService = currencyService;
            _paymentMessageSender = paymentMessageSender; // <-- GÁN
        }

        // Action này nhận token từ Stripe.js/Checkout và gửi vào Queue
        [HttpPost]
        public async Task<IActionResult> Charge(string stripeEmail, string stripeToken, Guid checkoutId)
        {
            var currentUser = await _workContext.GetCurrentUser();
            var cart = await _checkoutService.GetCheckoutDetails(checkoutId);

            if (cart == null)
            {
                TempData["Error"] = "Cart not found.";
                return Redirect("~/checkout/payment");
            }

            // 1. TẠO ORDER BAN ĐẦU (Đồng bộ)
            // Giữ lại logic tạo Order với trạng thái PENDING PAYMENT
            var orderCreationResult = await _orderService.CreateOrder(checkoutId, PaymentProviderHelper.StripeProviderId, 0, OrderStatus.PendingPayment);

            if (!orderCreationResult.Success)
            {
                TempData["Error"] = orderCreationResult.Error;
                return Redirect("~/checkout/payment");
            }

            var order = orderCreationResult.Value;

            // 2. TẠO MESSAGE VÀ GỬI VÀO QUEUE (Bất đồng bộ)
            var message = new PaymentMessage
            {
                OrderId = order.Id,
                CheckoutId = checkoutId,
                Amount = order.OrderTotal,
                PaymentProvider = PaymentProviderHelper.StripeProviderId,
                PaymentNonce = stripeToken, // Dùng StripeToken làm PaymentNonce/Payload
                CreatedById = currentUser?.Id ?? 0
            };

            await _paymentMessageSender.EnqueueAsync(message);
            return Accepted(new { Status = "queued", OrderId = order.Id });
        }
    }
}
