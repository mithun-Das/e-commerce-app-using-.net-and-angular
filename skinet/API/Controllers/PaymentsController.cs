using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using API.Errors;
using System.IO;
using Stripe;
using Core.Entities.OrderAggregate;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    public class PaymentsController : BaseApiController
    {
        private const string whSecret = "";
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger) 
        {
            _logger = logger;
            _paymentService = paymentService;
        }

        [Authorize]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var basket = await this._paymentService.CreateOrUpdatePaymentIntent(basketId);

            if (basket == null) return BadRequest(new ApiResponse(500, "Problem in your basket"));

            return basket;
        }

        [HttpPost]
        public async Task<ActionResult> StripeWebhook()
        {
            var json = await new StreamReader(Request.Body).ReadToEndAsync();

            var stripeEvent = EventUtility.ConstructEvent(json, 
                                            Request.Headers["Stripe-Signature"], whSecret);

            PaymentIntent intent;
            Order order;

            switch(stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                    intent = (PaymentIntent) stripeEvent.Data.Object;
                    this._logger.LogInformation("Payment Succeeded: ", intent.Id);
                    order = await this._paymentService.UpdateOrderPaymentSucceeded(intent.Id);
                    this._logger.LogInformation("Order updated to payment received: ", order.Id);
                    break;
                case "payment_intent.payment_failed":
                    intent = (PaymentIntent)stripeEvent.Data.Object;
                    this._logger.LogInformation("Payment Failed: ", intent.Id);
                    order = await this._paymentService.UpdateOrderPaymentFailed(intent.Id);
                    this._logger.LogInformation("Order updated to payment failed: ", order.Id);
                    break;
            }

            return new EmptyResult();
        }
    }
}
