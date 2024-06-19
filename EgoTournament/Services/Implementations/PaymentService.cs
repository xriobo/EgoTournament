using EgoTournament.Models.Paypal;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;

namespace EgoTournament.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private const string ClientId = "AYJHBM-ZGJ7UoUCZHic5jaKFs4DECFrrrG74X8ksTNKASsxz4ccF7NIkVhICNOLwm50ueYWqkB-X5kjY";
        private const string ClientSecret = "ECYTvu81Nzm0oD4O2HD48SZm4iP7MIqeKirBO4tDDxsoCYm4qyettq3tlSyO_Q1HNMWbXn2gNrU6BnxO";
        private PayPalEnvironment _environment;
        private PayPalHttpClient _client;

        public PaymentService()
        {
            _environment = new SandboxEnvironment(ClientId, ClientSecret);
            _client = new PayPalHttpClient(_environment);
        }

        public async Task<string> CreateOrderAsync(PaymentDto payment)
        {
            var orderRequest = new OrderRequest()
            {
                CheckoutPaymentIntent = "CAPTURE",
                PurchaseUnits = new List<PurchaseUnitRequest>()
            {
                new PurchaseUnitRequest()
                {
                    AmountWithBreakdown = new AmountWithBreakdown()
                    {
                        CurrencyCode = payment.Currency,
                        Value = payment.Amount
                    }
                }
            },
                ApplicationContext = new ApplicationContext()
                {
                    ReturnUrl = "ego-tournament://paypal-success",
                    CancelUrl = "ego-tournament://paypal-cancel"
                }
            };

            var request = new OrdersCreateRequest();
            request.RequestBody(orderRequest);

            var response = await _client.Execute(request);
            var result = response.Result<Order>();

            var approvalLink = result.Links.FirstOrDefault(link => link.Rel == "approve")?.Href;

            return approvalLink; // Return the approval link
        }

        public async Task<bool> CaptureOrderAsync(string orderId)
        {
            var request = new OrdersCaptureRequest(orderId);
            request.RequestBody(new OrderActionRequest());

            var response = await _client.Execute(request);
            var result = response.Result<Order>();

            return result.Status == "COMPLETED";
        }
    }
}