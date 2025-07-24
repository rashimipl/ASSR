using Google;
using JWTAuthentication.Authentication;
using Microsoft.AspNetCore.Mvc;
using PayPal.Api;
using ReactWithASP.Server.Models;
using ReactWithASP.Server.Services;
using System.Linq;
using System.Net;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using ReactWithASP.Server;

[ApiController]
[Route("api/paypal")]
public class PayPalController : ControllerBase
{
  private readonly PayPalHttpClient _client;
  private readonly ApplicationDbContext _context;
  public PayPalController(ApplicationDbContext context)
  {
    // Replace with your actual credentials
    var environment = new SandboxEnvironment
("ASCl0oiXb66wv3PQDCc5KvmVWlGKeddNYCXqYD6dGbvhHkknIrGFxm3iUVNfVfThnu_VnEy8uqw2ePK-", "EDsxAZr5H6vXq5jY4vHw7GemqPRvfKOYLiw-rTcUrxh7a0r3H5cPnkGmqIr4tgc22kU9FXFBtuxld2IB");

    // var environment = new LiveEnvironment("YourClientID", "YourSecret");

    _client = new PayPalHttpClient(environment);
    _context = context;
  }
  //private readonly ApplicationDbContext _context;
  //private readonly string _clientId = "";
  //private readonly string _clientSecret = "";
  //private readonly string _mode = "sandbox"; 


  //[HttpPost("create")]
  //public IActionResult CreatePayment_old([FromBody] decimal amount)
  //{
  //  try
  //  {
  //    var baseUrl = $"{Request.Scheme}://{Request.Host}";
  //    var payment = CreatePayment(baseUrl, "sale", "paypal", amount, "USD", "Payment description");

  //    var approvalUrl = payment.links.FirstOrDefault(link => link.rel == "approval_url")?.href;

  //    // Log transaction to the database
  //    var Paymentsds = new Payments
  //    {
  //      PaymentId = payment.id,
  //      Amount = amount,
  //      Currency = "USD",
  //      PaymentStatus = "Created",
  //      CreatedAt = DateTime.UtcNow
  //    };

  //    _context.Payments.Add(Paymentsds);
  //    _context.SaveChanges();

  //    return Ok(new { approvalUrl });
  //  }
  //  catch (Exception ex)
  //  {
  //    return BadRequest(new { error = ex.Message });
  //  }
  //}

  //private APIContext GetAPIContext()
  //{
  //  var config = new Dictionary<string, string>
  //      {
  //          {"mode", _mode}
  //      };

  //  var accessToken = new OAuthTokenCredential(_clientId, _clientSecret, config).GetAccessToken();
  //  return new APIContext(accessToken);
  //}

  //private Payment CreatePayment(string baseUrl, string intent, string paymentMethod, decimal amount, string currency, string description)
  //{
  //  var apiContext = GetAPIContext();

  //  var payment = new Payment
  //  {
  //    intent = intent,
  //    payer = new Payer { payment_method = paymentMethod },
  //    transactions = new List<Transaction>
  //          {
  //              new Transaction
  //              {
  //                  description = description,
  //                  amount = new Amount
  //                  {
  //                      currency = currency,
  //                      total = amount.ToString("F2")
  //                  }
  //              }
  //          },
  //    redirect_urls = new RedirectUrls
  //    {
  //      cancel_url = $"{baseUrl}/api/paypal/cancel",
  //      return_url = $"{baseUrl}/api/paypal/success"
  //    }
  //  };

  //  return payment.Create(apiContext);
  //}



  [HttpGet("Account/PaymentSuccess")]
  public async Task<IActionResult> PaymentSuccess([FromQuery] string token, [FromQuery] string PayerID)
  {
    var PaymentSuccess = "";
    try
    {
      if (string.IsNullOrEmpty(token))
      {
        return BadRequest(new { message = "Invalid payment token" });
      }
      // Step 1: Get payment details from PayPal
      var request = new OrdersGetRequest(token);
      var payPalResponse = await _client.Execute(request);

      if (payPalResponse == null || payPalResponse.StatusCode != HttpStatusCode.OK)
      {
        return BadRequest(new { message = "Failed to retrieve payment details from PayPal." });        
      }
      var orderDetails = payPalResponse.Result<PayPalCheckoutSdk.Orders.Order>();
      if (orderDetails == null)
      {
        return BadRequest(new { message = "PayPal order details are null." });        
      }
      var paymentDetail = _context.PaymentDetails.Where(usp => usp.PaymentId == orderDetails.Id)
       .Select(usp => new
       {
         usp.Id,
         usp.UserGuid,         
         usp.SubPlanId,
         usp.PaymentId         
       }).FirstOrDefault();

      var UserSubscriptionPlan = new UserSubscriptionPlan()
      {
        UserGuid = paymentDetail.UserGuid,
        SubsPlanId = paymentDetail.SubPlanId,
        StartDate = DateTime.UtcNow,
        EndDate = DateTime.UtcNow.AddDays(14),
        Status = true,
        isrenew = false,
      };
      _context.UserSubscriptionPlan.Add(UserSubscriptionPlan);
      _context.SaveChanges();

      var Paymentsds = new Payments
      {
        PaymentId = orderDetails.Id,
        UserGuid = paymentDetail.UserGuid,
        UserSubsPlanId = UserSubscriptionPlan.Id,
        SubsPlanID = paymentDetail.SubPlanId,
        Amount = decimal.TryParse(orderDetails.PurchaseUnits?.FirstOrDefault()?.AmountWithBreakdown?.Value, out var amount) ? amount : 0,
        Currency = orderDetails.PurchaseUnits?.FirstOrDefault()?.AmountWithBreakdown?.CurrencyCode ?? "Unknown",
        PaymentStatus = orderDetails.Status,
        PaymentMethod = "Paypal",
        CreatedOn = DateTime.UtcNow,
      };
      _context.Payments.Add(Paymentsds);
      _context.SaveChanges();
      
      string transactionId = Guid.NewGuid().ToString("N").Substring(0, 10);
      string paystatus = orderDetails.Status;
      
      var transactiondetails = new Transection
      {
        UserGuid = paymentDetail.UserGuid,
        PayPalTranId = Paymentsds.Id,
        PaymentId = Paymentsds.PaymentId,
        PayerId = PayerID,
        TransactionId = transactionId,
        Price = decimal.TryParse(orderDetails.PurchaseUnits?.FirstOrDefault()?.AmountWithBreakdown?.Value, out var Amount) ? amount : 0, 
        PaymentStatus = paystatus.Equals("APPROVED", StringComparison.OrdinalIgnoreCase) ? true : false,
        CreatedOn = DateTime.UtcNow,
      };
      _context.Transection.Add(transactiondetails);
      _context.SaveChanges();

      // Log: Transaction details stored
      var TransactionLog = new TransactionLog
      {
        UserGuid = paymentDetail.UserGuid,
        TransactionID = Paymentsds.Id,
        PaymentId = Paymentsds.PaymentId,
        Event = orderDetails.Status,
        CreatedOn = DateTime.UtcNow,
      };
      _context.TransactionLog.Add(TransactionLog);
      _context.SaveChanges();
      PaymentSuccess = "Payment Processed successfully.";
      
    }
    catch (Exception ex)
    {
      throw new Exception(ex.Message);
    }

    return Ok(new { PaymentSuccess });
  
  }
  [HttpGet("Account/IsRenewPaymentSuccess")]
  public async Task<IActionResult> IsRenewPaymentSuccess([FromQuery] string token, [FromQuery] string PayerID)
  {
    var PaymentSuccess = "";
    try
    {
      if (string.IsNullOrEmpty(token))
      {
        return BadRequest(new { message = "Invalid payment token" });
      }
      // Step 1: Get payment details from PayPal
      var request = new OrdersGetRequest(token);
      var payPalResponse = await _client.Execute(request);

      if (payPalResponse == null || payPalResponse.StatusCode != HttpStatusCode.OK)
      {
        return BadRequest(new { message = "Failed to retrieve payment details from PayPal." });
      }
      var orderDetails = payPalResponse.Result<PayPalCheckoutSdk.Orders.Order>();
      if (orderDetails == null)
      {
        return BadRequest(new { message = "PayPal order details are null." });
      }
      var paymentDetail = _context.PaymentDetails.Where(usp => usp.PaymentId == orderDetails.Id)
       .Select(usp => new
       {
         usp.Id,
         usp.UserGuid,
         usp.SubPlanId,
         usp.PaymentId
       }).FirstOrDefault();


      var existingSubscription = _context.UserSubscriptionPlan
        .Where(u => u.UserGuid == paymentDetail.UserGuid && u.SubsPlanId == paymentDetail.SubPlanId)
        .OrderByDescending(u => u.EndDate)
        .FirstOrDefault();

      if (existingSubscription == null)
      {
        return BadRequest(new
        {
          Status = false,
          Message = "No active subscription found. Please purchase a new subscription."
        });
      }

      existingSubscription.EndDate = DateTime.UtcNow.AddDays(365);
      existingSubscription.Status = true;
      existingSubscription.isrenew = true;
      existingSubscription.ModifiedOn = DateTime.UtcNow;

      _context.UserSubscriptionPlan.Update(existingSubscription);
      _context.SaveChanges();



      var Paymentsds = new Payments
      {
        PaymentId = orderDetails.Id,
        UserGuid = paymentDetail.UserGuid,
        UserSubsPlanId = existingSubscription.Id,
        SubsPlanID = paymentDetail.SubPlanId,
        Amount = decimal.TryParse(orderDetails.PurchaseUnits?.FirstOrDefault()?.AmountWithBreakdown?.Value, out var amount) ? amount : 0,
        Currency = orderDetails.PurchaseUnits?.FirstOrDefault()?.AmountWithBreakdown?.CurrencyCode ?? "Unknown",
        PaymentStatus = orderDetails.Status,
        PaymentMethod = "Paypal",
        CreatedOn = DateTime.UtcNow,
      };
      _context.Payments.Add(Paymentsds);
      _context.SaveChanges();

      string transactionId = Guid.NewGuid().ToString("N").Substring(0, 10);
      string paystatus = orderDetails.Status;

      var transactiondetails = new Transection
      {
        UserGuid = paymentDetail.UserGuid,
        PayPalTranId = Paymentsds.Id,
        PaymentId = Paymentsds.PaymentId,
        PayerId = PayerID,
        TransactionId = transactionId,
        Price = decimal.TryParse(orderDetails.PurchaseUnits?.FirstOrDefault()?.AmountWithBreakdown?.Value, out var Amount) ? amount : 0,
        PaymentStatus = paystatus.Equals("APPROVED", StringComparison.OrdinalIgnoreCase) ? true : false,
        CreatedOn = DateTime.UtcNow,
      };
      _context.Transection.Add(transactiondetails);
      _context.SaveChanges();

      // Log: Transaction details stored
      var TransactionLog = new TransactionLog
      {
        UserGuid = paymentDetail.UserGuid,
        TransactionID = Paymentsds.Id,
        PaymentId = Paymentsds.PaymentId,
        Event = orderDetails.Status,
        CreatedOn = DateTime.UtcNow,
      };
      _context.TransactionLog.Add(TransactionLog);
      _context.SaveChanges();
      PaymentSuccess = "Payment Processed successfully.";

    }
    catch (Exception ex)
    {
      throw new Exception(ex.Message);
    }

    return Ok(new { PaymentSuccess });

  }
  [HttpGet("Account/UpgradePaymentSuccess")]
  public async Task<IActionResult> UpgradePaymentSuccess([FromQuery] string token, [FromQuery] string PayerID)
  {
    var PaymentSuccess = "";
    try
    {
      if (string.IsNullOrEmpty(token))
      {
        return BadRequest(new { message = "Invalid payment token" });
      }
      // Step 1: Get payment details from PayPal
      var request = new OrdersGetRequest(token);
      var payPalResponse = await _client.Execute(request);

      if (payPalResponse == null || payPalResponse.StatusCode != HttpStatusCode.OK)
      {
        return BadRequest(new { message = "Failed to retrieve payment details from PayPal." });
      }
      var orderDetails = payPalResponse.Result<PayPalCheckoutSdk.Orders.Order>();
      if (orderDetails == null)
      {
        return BadRequest(new { message = "PayPal order details are null." });
      }
      var paymentDetail = _context.PaymentDetails.Where(usp => usp.PaymentId == orderDetails.Id)
       .Select(usp => new
       {
         usp.Id,
         usp.UserGuid,
         usp.SubPlanId,
         usp.PaymentId
       }).FirstOrDefault();

      var UserSubscriptionPlan = new UserSubscriptionPlan()
      {
        UserGuid = paymentDetail.UserGuid,
        SubsPlanId = paymentDetail.SubPlanId,
        StartDate = DateTime.UtcNow,
        EndDate = DateTime.UtcNow.AddDays(365),
        Status = true,
        isrenew = false,
      };
      _context.UserSubscriptionPlan.Add(UserSubscriptionPlan);
      _context.SaveChanges();

      var Paymentsds = new Payments
      {
        PaymentId = orderDetails.Id,
        UserGuid = paymentDetail.UserGuid,
        UserSubsPlanId = UserSubscriptionPlan.Id,
        SubsPlanID = paymentDetail.SubPlanId,
        Amount = decimal.TryParse(orderDetails.PurchaseUnits?.FirstOrDefault()?.AmountWithBreakdown?.Value, out var amount) ? amount : 0,
        Currency = orderDetails.PurchaseUnits?.FirstOrDefault()?.AmountWithBreakdown?.CurrencyCode ?? "Unknown",
        PaymentStatus = orderDetails.Status,
        PaymentMethod = "Paypal",
        CreatedOn = DateTime.UtcNow,
      };
      _context.Payments.Add(Paymentsds);
      _context.SaveChanges();

      string transactionId = Guid.NewGuid().ToString("N").Substring(0, 10);
      string paystatus = orderDetails.Status;

      var transactiondetails = new Transection
      {
        UserGuid = paymentDetail.UserGuid,
        PayPalTranId = Paymentsds.Id,
        PaymentId = Paymentsds.PaymentId,
        PayerId = PayerID,
        TransactionId = transactionId,
        Price = decimal.TryParse(orderDetails.PurchaseUnits?.FirstOrDefault()?.AmountWithBreakdown?.Value, out var Amount) ? amount : 0,
        PaymentStatus = paystatus.Equals("APPROVED", StringComparison.OrdinalIgnoreCase) ? true : false,
        CreatedOn = DateTime.UtcNow,
      };
      _context.Transection.Add(transactiondetails);
      _context.SaveChanges();

      var TransactionLog = new TransactionLog
      {
        UserGuid = paymentDetail.UserGuid,
        TransactionID = Paymentsds.Id,
        PaymentId = Paymentsds.PaymentId,
        Event = orderDetails.Status,
        CreatedOn = DateTime.UtcNow,
      };
      _context.TransactionLog.Add(TransactionLog);
      _context.SaveChanges();
      PaymentSuccess = "Payment Processed successfully.";

    }
    catch (Exception ex)
    {
      throw new Exception(ex.Message);
    }

    return Ok(new { PaymentSuccess });
  }

}
