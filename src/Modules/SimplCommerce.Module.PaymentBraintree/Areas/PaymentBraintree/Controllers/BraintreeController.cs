using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading;
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
        private readonly IRepository<Order> _orderRepository;
        private readonly IBraintreeConfiguration _braintreeConfiguration;
        private readonly ICurrencyService _currencyService;
        private readonly IPaymentMessageSender _paymentMessageSender;
        private static readonly ConcurrentDictionary<Guid, SemaphoreSlim> _checkoutLocks = new ConcurrentDictionary<Guid, SemaphoreSlim>();

        public BraintreeController(
            ICheckoutService checkoutService,
            IOrderService orderService,
            IWorkContext workContext,
            IRepositoryWithTypedId<PaymentProvider, string> paymentProviderRepository,
            IRepository<Payment> paymentRepository,
            IRepository<Order> orderRepository,
            IBraintreeConfiguration braintreeConfiguration,
            ICurrencyService currencyService,
            IPaymentMessageSender paymentMessageSender)
        {
            _checkoutService = checkoutService;
            _orderService = orderService;
            _workContext = workContext;
            _paymentProviderRepository = paymentProviderRepository;
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
            _braintreeConfiguration = braintreeConfiguration;
            _currencyService = currencyService;
            _paymentMessageSender = paymentMessageSender;
        }

        //[HttpPost]
        //public async Task<IActionResult> Charge(string nonce, Guid checkoutId)
        //{
        //    var curentUser = await _workContext.GetCurrentUser();
        //    var cart = await _checkoutService.GetCheckoutDetails(checkoutId);
        //    if (cart == null) return NotFound();

        //    var semaphore = _checkoutLocks.GetOrAdd(checkoutId, _ => new SemaphoreSlim(1, 1));
        //    await semaphore.WaitAsync();
        //    try
        //    {
        //        // try to find a recent order for same customer/payment/amount to prevent duplicates
        //        var tenMinutesAgo = DateTimeOffset.UtcNow.AddMinutes(-10);
        //        var existingOrder = await _orderRepository.Query()
        //            .Where(o => o.CustomerId == curentUser.Id
        //                        && o.PaymentMethod == PaymentProviderHelper.BraintreeProviderId
        //                        && o.OrderTotal == cart.OrderTotal
        //                        && (o.OrderStatus == OrderStatus.PendingPayment || o.OrderStatus == OrderStatus.New)
        //                        && o.CreatedOn >= tenMinutesAgo)
        //            .OrderByDescending(o => o.CreatedOn)
        //            .FirstOrDefaultAsync();

        //        Order order;
        //        if (existingOrder != null)
        //        {
        //            order = existingOrder;
        //        }
        //        else
        //        {
        //            var orderCreateResult = await _orderService.CreateOrder(checkoutId, PaymentProviderHelper.BraintreeProviderId, 0, OrderStatus.PendingPayment);
        //            if (!orderCreateResult.Success) return BadRequest(orderCreateResult.Error);
        //            order = orderCreateResult.Value;
        //        }

        //        // Avoid enqueueing duplicate messages for the same order by checking existing payments
        //        var existingPayment = await _paymentRepository.Query().FirstOrDefaultAsync(p => p.OrderId == order.Id);
        //        if (existingPayment == null)
        //        {
        //            var message = new PaymentMessage
        //            {
        //                OrderId = order.Id,
        //                CheckoutId = checkoutId,
        //                Amount = order.OrderTotal,
        //                PaymentProvider = PaymentProviderHelper.BraintreeProviderId,
        //                PaymentNonce = nonce,
        //                CreatedById = curentUser?.Id ?? 0
        //            };

        //            await _paymentMessageSender.EnqueueAsync(message);
        //        }

        //        // Return accepted — processing will happen asynchronously by the queue consumer
        //        return Accepted(new { Status = "queued", OrderId = order.Id });
        //    }
        //    finally
        //    {
        //        semaphore.Release();
        //        _checkoutLocks.TryRemove(checkoutId, out _);
        //    }
        //}
        [HttpPost]
        public async Task<IActionResult> Charge(string nonce, Guid checkoutId)
        {
            var curentUser = await _workContext.GetCurrentUser();
            var cart = await _checkoutService.GetCheckoutDetails(checkoutId);
            if (cart == null) return NotFound();

            var semaphore = _checkoutLocks.GetOrAdd(checkoutId, _ => new SemaphoreSlim(1, 1));
            await semaphore.WaitAsync();
            try
            {
                var existingOrder = await _orderRepository.Query()
                    .Where(o => o.CustomerId == curentUser.Id
                                && o.PaymentMethod == PaymentProviderHelper.BraintreeProviderId
                                && o.OrderTotal == cart.OrderTotal
                                && (o.OrderStatus == OrderStatus.PendingPayment
                                    || o.OrderStatus == OrderStatus.New))
                    .OrderByDescending(o => o.CreatedOn)
                    .FirstOrDefaultAsync();

                Order order;
                if (existingOrder != null)
                {
                    // Order đã tồn tại -> đã có payment/message -> return ngay
                    return Accepted(new { Status = "queued", OrderId = existingOrder.Id });
                }
                else
                {
                    // Chưa có order -> tạo mới
                    var orderCreateResult = await _orderService.CreateOrder(
                        checkoutId,
                        PaymentProviderHelper.BraintreeProviderId,
                        0,
                        OrderStatus.PendingPayment);

                    if (!orderCreateResult.Success)
                        return BadRequest(orderCreateResult.Error);

                    order = orderCreateResult.Value;
                }

                // Chỉ enqueue khi order mới được tạo
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

                return Accepted(new { Status = "queued", OrderId = order.Id });
            }
            finally
            {
                semaphore.Release();
                _checkoutLocks.TryRemove(checkoutId, out _);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetClientToken()
        {
            return Ok(await _braintreeConfiguration.GetClientToken());
        }
    }
}
