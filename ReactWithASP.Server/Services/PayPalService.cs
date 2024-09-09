using Microsoft.Extensions.Options;
using PayPal.Api;
using ReactWithASP.Server.Models;

namespace ReactWithASP.Server.Services
{
    public class PayPalService
    {
        private readonly PayPalConfig _payPalConfig;

        public PayPalService(IOptions<PayPalConfig> payPalConfig)
        {
            _payPalConfig = payPalConfig.Value;
        }

        private APIContext GetAPIContext()
        {
            var config = new Dictionary<string, string>
        {
            {"clientId", _payPalConfig.ClientId},
            {"clientSecret", _payPalConfig.ClientSecret},
            {"mode", _payPalConfig.Mode}
        };

            var accessToken = new OAuthTokenCredential(config).GetAccessToken();
            return new APIContext(accessToken);
        }

        public Payment CreatePayment(string baseUrl, string intent, string paymentMethod, decimal amount, string currency, string description)
        {
            var apiContext = GetAPIContext();

            var payment = new Payment
            {
                intent = intent,
                payer = new Payer { payment_method = paymentMethod },
                transactions = new List<Transaction>
            {
                new Transaction
                {
                    description = description,
                    amount = new Amount
                    {
                        currency = currency,
                        total = amount.ToString("F2")
                    }
                }
            },
                redirect_urls = new RedirectUrls
                {
                    cancel_url = $"{baseUrl}/api/paypal/cancel",
                    return_url = $"{baseUrl}/api/paypal/success"
                }
            };

            return payment.Create(apiContext);
        }

        public Payment ExecutePayment(string paymentId, string payerId)
        {
            var apiContext = GetAPIContext();
            var paymentExecution = new PaymentExecution { payer_id = payerId };
            var payment = new Payment { id = paymentId };
            return payment.Execute(apiContext, paymentExecution);
        }
    }
}
