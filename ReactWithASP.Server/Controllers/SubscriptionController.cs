using Hangfire;
using JWTAuthentication.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PayPal;
using PayPal.Api;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using Quartz;
using ReactWithASP.Server.Models;
using ReactWithASP.Server.Models.Posts;
using SendGrid;
using System.Diagnostics.Eventing.Reader;
using System.Net;
using System.Numerics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Hangfire;
using Hangfire.SqlServer;
using ReactWithASP.Server.Services;

namespace ReactWithASP.Server.Controllers
{
  [Route("api")]
  [ApiController]
  public class SubscriptionController : Controller
  {
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly PayPalHttpClient _client;
    public SubscriptionController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
      _userManager = userManager;
      _context = context;

      var environment = new SandboxEnvironment
("ASCl0oiXb66wv3PQDCc5KvmVWlGKeddNYCXqYD6dGbvhHkknIrGFxm3iUVNfVfThnu_VnEy8uqw2ePK-", "EDsxAZr5H6vXq5jY4vHw7GemqPRvfKOYLiw-rTcUrxh7a0r3H5cPnkGmqIr4tgc22kU9FXFBtuxld2IB");

      // var environment = new LiveEnvironment("YourClientID", "YourSecret");

      _client = new PayPalHttpClient(environment);
    }
    [HttpGet]
    [Route("GetPlans")]
    [Authorize]
    public async Task<IActionResult> GetPlans()
    {
      var plans = _context.SubscriptionPlans.ToList();
      if (plans == null)
      {
        return NotFound("Plan not found");
      }
      return Ok(plans);
    }
    [HttpGet]
    [Route("Plans")]
    //[Authorize]
    public async Task<IActionResult> Plans(int PlanId)
    {
      var Plan = _context.SubscriptionPlans.Where(x => x.PlanId == PlanId).ToList();
      if (Plan == null)
      {
        return NotFound("Subscription plan not found");
      }
      return Ok(Plan);
    }
    [HttpPost("status")]
    [Authorize]
    public async Task<IActionResult> GetUserStatus([FromBody] UserStatusRequest model)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var user = await _userManager.FindByIdAsync(model.UserGUID);
      if (user == null)
      {
        return NotFound("User not found");
      }

      var userSubscription = _context.UserSubscriptions.FirstOrDefault(us => us.UserGuid == model.UserGUID);
      if (userSubscription == null)
      {
        return NotFound("User subscription not found");
      }

      var subscriptionPlan = _context.SubscriptionPlans.FirstOrDefault(sp => sp.Id == userSubscription.SubsPlanId);
      if (subscriptionPlan == null)
      {
        return NotFound("Subscription plan not found");
      }

      var response = new UserStatusResponse
      {
        UserGUID = model.UserGUID,
        Status = userSubscription.Status,
        Subscription = subscriptionPlan.PlanName,
        SubscriptionStatus = userSubscription.EndDate > DateTime.Now ? "Valid" : "Expired",
        SubscriptionExpiringOn = userSubscription.EndDate,
      };

      return Ok(response);
    }


    //Static
    [HttpPost("GetUserSubscriptionStatus")]
    [Authorize]
    public IActionResult GetUserSubscriptionStatus([FromQuery] UserSubscriptionRequest request)
    {
      if (string.IsNullOrEmpty(request.UserGUID))
      {
        return BadRequest(new { Message = "UserGUID is required" });
      }

      // This is just an example response. In a real application, you would retrieve this data from your database.
      var response = new UserSubscriptionResponse
      {
        UserGUID = request.UserGUID,
        Status = "E",
        Subscription = "2",
        SubscriptionStatus = "Valid",
        SubscriptionExpiringOn = DateTime.UtcNow.AddDays(30)
      };

      return Ok(response);
    }


    [HttpPost("CreatePlan")]
    public async Task<IActionResult> CreatePlan([FromBody] CreatePlanRequest request)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var SubscriptionPlans = new SubscriptionPlans()
      {
        Id = request.Id,
        PlanId = request.PlanId,
        PlanName = request.Name,
        Price = request.Price,
        ConnectedChannels = request.ConnectedChannels,
        SmartContentSuggestionsMonthly = request.SmartContentSuggestionsMonthly,
        ImageSuggestionsMonthly = request.ImageSuggestionsMonthly,
        DailyPostInspirations = request.DailyPostInspirations,
        DraftedPosts = request.DraftedPosts,
        PostsDaily = request.PostsDaily,
        ScheduledPostsQueue = request.ScheduledPostsQueue,
        MultiImageVideoPosts = request.MultiImageVideoPosts,
        RecurringPosts = request.RecurringPosts,
        PremiumSupport = request.PremiumSupport
      };

      _context.Add(SubscriptionPlans);
      _context.SaveChanges();

      return Ok(new
      {
        Status = "Success",
        Message = "Plan created successfully",
        Data = SubscriptionPlans
      });
    }


    [HttpGet("GetAllSubscriptionPlans")]
    public IActionResult GetAllSubscriptionPlans(int? planid)
    {
      // Declare the query variable before the conditional logic
      List<SubscriptionPlans> query;
      if (planid.HasValue) // Check if planid has a value
      {
        query = (from sp in _context.SubscriptionPlans
                 where sp.PlanId == planid.Value
                 select new SubscriptionPlans
                 {
                   Id = sp.Id,
                   PlanId = sp.PlanId,
                   PlanName = sp.PlanName,
                   Price = sp.Price,
                   ConnectedChannels = sp.ConnectedChannels,
                   SmartContentSuggestionsMonthly = sp.SmartContentSuggestionsMonthly,
                   ImageSuggestionsMonthly = sp.ImageSuggestionsMonthly,
                   DailyPostInspirations = sp.DailyPostInspirations,
                   DraftedPosts = sp.DraftedPosts,
                   PostsDaily = sp.PostsDaily,
                   ScheduledPostsQueue = sp.ScheduledPostsQueue,
                   MultiImageVideoPosts = sp.MultiImageVideoPosts,
                   RecurringPosts = sp.RecurringPosts,
                   PremiumSupport = sp.PremiumSupport
                 }).ToList();
      }
      else
      {
        query = _context.SubscriptionPlans
        .Select(sp => new SubscriptionPlans
        {
          Id = sp.Id,
          PlanId = sp.PlanId,
          PlanName = sp.PlanName,
          Price = sp.Price,
          ConnectedChannels = sp.ConnectedChannels,
          SmartContentSuggestionsMonthly = sp.SmartContentSuggestionsMonthly,
          ImageSuggestionsMonthly = sp.ImageSuggestionsMonthly,
          DailyPostInspirations = sp.DailyPostInspirations,
          DraftedPosts = sp.DraftedPosts,
          PostsDaily = sp.PostsDaily,
          ScheduledPostsQueue = sp.ScheduledPostsQueue,
          MultiImageVideoPosts = sp.MultiImageVideoPosts,
          RecurringPosts = sp.RecurringPosts,
          PremiumSupport = sp.PremiumSupport
        }).ToList();
      }

      // Check if query result is empty
      if (query == null || query.Count == 0)
      {
        return BadRequest(new { Message = "Data Not Found!..." });
      }

      return Ok(query);
    }



    [HttpGet]
    [Route("GetAllUsersUnderSelectedSubscription")]
    public IActionResult GetAllUsersUnderSelectedSubscription(string PlanName)
    {
      var query = from sp in _context.SubscriptionPlans
                  join us in _context.UserSubscriptions on sp.Id equals us.SubsPlanId
                  join u in _context.Users on us.UserGuid equals u.Id
                  where sp.PlanName.ToLower() == PlanName.ToLower()
                  select new subscriptionplanbyuserid
                  {
                    UserGUID = u.Id,
                    PlanName = sp.PlanName,
                    Price = sp.Price,
                    SubsPlanID = us.SubsPlanId,
                    FullName = u.FullName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    CreatedOn = u.CreatedOn,
                  };

      var result = query.ToList();
      if (result.Count == 0)
      {
        return BadRequest(new { Message = " Data Not Found!..." });
      }
      return Ok(result);
    }


    [HttpPost("UpdateUserSubscriptionsDuration")]
    [Authorize]
    public IActionResult UpdateUserSubscriptionsDuration([FromBody] UserSubsDuration model)
    {
      var result = _context.UserSubscriptions.Where(u => u.UserGuid == model.UserGuid).ToList();

      if (result.Count == 0)
      {
        return BadRequest(new { Message = " Data Not Found!..." });
      }
      else
      {
        foreach (var subscription in result)
        {
          subscription.StartDate = model.Startdate;
          subscription.EndDate = model.Enddate;
        }

        _context.SaveChanges();
        return Ok(new { Message = " Plan Duretion update Successfully" });

      }
    }


    [HttpPost("UpdateSubscriptionsPlanPrice")]
    [Authorize]
    public IActionResult UpdateSubscriptionsPlanPrice(int PlanId, decimal price)
    {
      var result = _context.SubscriptionPlans.Where(u => u.Id == PlanId).ToList();

      if (result.Count == 0)
      {
        return BadRequest(new { Message = " Data Not Found!..." });
      }
      else
      {
        foreach (var subscription in result)
        {
          subscription.Price = price;
        }

        _context.SaveChanges();
        return Ok(new { Message = " Price update Successfully" });

      }
    }

    //[HttpPost("CreateSubscriptionFeatures")]
    //public async Task<IActionResult> CreateSubscriptionPlans([FromBody] CreateSubFeatureRequest request)
    //{
    //  if (!ModelState.IsValid)
    //  {
    //    return BadRequest(ModelState);
    //  }

    //  if (string.IsNullOrWhiteSpace(request.FeatureName))
    //  {
    //    return BadRequest(new { message = "FeatureName cannot be empty" });
    //  }
    //  string featureCode = GenerateFeatureCode(request.FeatureName);

    //  var SubscriptionFeatures = new SubscriptionFeatures()
    //  {
    //    FeatureCode = featureCode,
    //    FeatureName = request.FeatureName,
    //    Free = string.IsNullOrWhiteSpace(request.Free) ? "NA" : request.Free,
    //    Standard = string.IsNullOrWhiteSpace(request.Standard) ? "NA" : request.Standard,
    //    Premium = string.IsNullOrWhiteSpace(request.Premium) ? "NA" : request.Premium,
    //    Status = request.Status,

    //  };
    //  _context.SubscriptionFeatures.Add(SubscriptionFeatures);
    //  _context.SaveChanges();

    //  return Ok(new
    //  {
    //    Status = "Success",
    //    Message = "Plan created successfully",
    //    Data = SubscriptionFeatures
    //  });
    //}
    //private string GenerateFeatureCode(string featureName)
    //{
    //  var words = featureName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    //  if (words.Length == 0) return "NA";

    //  // Take the first two letters of each word and convert them to uppercase
    //  string featureCode = string.Concat(words.Select(word => word.Substring(0, Math.Min(2, word.Length)).ToUpper()));

    //  return featureCode;
    //}
    [HttpGet("GetSubscriptionplans")]
    public async Task<IActionResult> GetSubscriptionplans()
    {
      var subscriptionFeatures = _context.SubscriptionFeatures
          .Select(sf => new
          {
            sf.ID,
            sf.FeatureCode,
            sf.FeatureName,
            sf.Free,
            sf.Standard,
            sf.Premium,
            Status = sf.Status ? true : false,
          }).ToList();

      return Ok(new
      {
        Status = "Success",
        Message = "Data fetched successfully",
        Data = subscriptionFeatures
      });
    }

    [HttpGet("GetSubscriptionFeatures")]
    public async Task<IActionResult> GetSubscriptionFeatures(int packageId)
    {
      var query = _context.SubscriptionPackage
          .Join(_context.SubscriptionPlanFeatures,
              smp => smp.PackageId,
              ugp => ugp.PackageId,
              (smp, ugp) => new
              {
                smp.ID,
                smp.PackageId,
                smp.PackageName,
                smp.Price,
                ugp.FeatureCode,
                ugp.FeatureName,
                ugp.Allowedcount,
                ugp.Isunlimited,
                ugp.Status,
                smp.Validity_days
                

              })
          .Where(x => x.PackageId == packageId)
          .OrderBy(smp => smp.ID)
          .ToList();

      var response = query
     .GroupBy(x => new { x.ID, x.PackageId, x.PackageName, x.Price })
     .Select(g => new subscriptionplanResponse
     {
       PackageId = g.Key.PackageId,
       PackageName = g.Key.PackageName,
       Price = g.Key.Price.ToString(),
       subscriptionFeature = g.Select(x => new subscriptionFeature
       {
         FeatureCode = x.FeatureCode,
         FeatureName = x.FeatureName,
         Allowedcount = x.Allowedcount,
         Isunlimited = x.Isunlimited,
         Status = x.Status,
         Validity_days=x.Validity_days
       }).ToList()
     }).ToList();

      return Ok(response);

    }

    [HttpPost("UserSubscriptionsPlan")]
    public async Task<IActionResult> UserSubscriptionsPlan(string UserGUID, int Subscriptionid)
    {
      var paymentUrl = "";
      //var approvalUrl = "";
      var package = _context.SubscriptionPackage.FirstOrDefault(u => u.PackageId == Subscriptionid);

      var existingSubscription = _context.UserSubscriptionPlan
        .Where(u => u.UserGuid == UserGUID && u.SubsPlanId == Subscriptionid)
        .OrderByDescending(u => u.EndDate)
        .FirstOrDefault();

      if (existingSubscription?.Status == true)
      {
        paymentUrl = "Already active Subscription.";
      }
      else
      {
        if (package?.PackageName == "Free")
        {
          // add User Subscription Plan
          var UserSubscriptionPlan = new UserSubscriptionPlan()
          {
            UserGuid = UserGUID,
            SubsPlanId = Subscriptionid,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(14),
            Status = true,
            isrenew = false,
          };
          _context.UserSubscriptionPlan.Add(UserSubscriptionPlan);
          _context.SaveChanges();

          string freePaymentId = $"FREEPAY-{Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16)}";
          string PayerId = $"FREEPayerId -{Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16)}";
          string TransactionId = $"FREETransactionId -{Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16)}";

          // add User Pay Pal Transactions
          var Paymentsds = new Payments
          {
            PaymentId = freePaymentId,
            UserGuid = UserSubscriptionPlan.UserGuid,
            UserSubsPlanId = UserSubscriptionPlan.Id,
            SubsPlanID = UserSubscriptionPlan.SubsPlanId,
            Amount = 0,
            Currency = "USD",
            PaymentStatus = "Created",
            PaymentMethod = "Paypal",
            CreatedOn = DateTime.UtcNow
          };
          _context.Payments.Add(Paymentsds);
          _context.SaveChanges();

          // add User Transection Details       

          var transactiondetails = new Transection
          {
            UserGuid = Paymentsds.UserGuid,
            PayPalTranId = Paymentsds.Id,
            PaymentId = Paymentsds.PaymentId,
            PayerId = PayerId,
            TransactionId = TransactionId,
            Price = Paymentsds.Amount,
            CreatedOn = DateTime.UtcNow,
            PaymentStatus = true
          };
          _context.Transection.Add(transactiondetails);
          _context.SaveChanges();

          paymentUrl = "Free Plan Activated Successfully.";
        }
        else if (package?.PackageName == "Standard" || package?.PackageName == "Premium")
        {
          try
          {
            decimal packageAmount = package.Price;
            // Step 1: Create the order request
            var order = new OrderRequest()
            {
              CheckoutPaymentIntent = "CAPTURE",
              PurchaseUnits = new List<PurchaseUnitRequest>
                    {
                        new PurchaseUnitRequest
                        {
                            AmountWithBreakdown = new AmountWithBreakdown
                            {
                                CurrencyCode = "USD",
                                Value = packageAmount.ToString("F2"),
                            },
                        }
                    },
              ApplicationContext = new PayPalCheckoutSdk.Orders.ApplicationContext
              {
                //ReturnUrl = "http://localhost:59013/Account/PaymentSuccess",
                //CancelUrl = "http://localhost:59013/Company/Paymentfailure",
                ReturnUrl = "https://design.mishainfotech.com/design/ayush-work/paypal/success.html",
                CancelUrl = "https://assr.digitalnoticeboard.biz/Company/Paymentfailure",
                UserAction = "PAY_NOW",
                ShippingPreference = "NO_SHIPPING",
                LandingPage = "BILLING"
              }
            };

            // Step 2: Create the OrdersCreateRequest
            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(order);

            // Step 3: Execute the request
            var paymentResponse = await _client.Execute(request);

            if (paymentResponse.StatusCode == HttpStatusCode.Created)
            {
              var result = paymentResponse.Result<PayPalCheckoutSdk.Orders.Order>();
              if (result != null)
              {
                // Log transaction to the database
                var PaymentData = new PaymentDetails
                {
                  UserGuid = UserGUID,
                  SubPlanId = Subscriptionid,
                  PaymentId = result.Id,
                  CreatedOn = DateTime.UtcNow,
                };

                _context.PaymentDetails.Add(PaymentData);
                _context.SaveChanges();

              }
              // Step 4: Extract the approval URL for the user to complete the payment
              paymentUrl = result.Links.FirstOrDefault(link => link.Rel == "approve")?.Href;
            }
          }
          catch (Exception ex)
          {
            //throw new Exception(ex.Message);
            return BadRequest(new { message = ex.Message });
          }
        }
      }
      //return approvalUrl;
      return Ok(new { paymentUrl });
    }

    [HttpPost("IsRenewUserSubscriptions")]
    public async Task<IActionResult> IsRenewUserSubscriptions(string UserGUID, int Subscriptionid)
    {
      var package = _context.SubscriptionPackage.FirstOrDefault(u => u.PackageId == Subscriptionid);

      if (package?.PackageName == "Free")
      {
        return BadRequest(new
        {
          Status = false,
          Message = "Subscription Can't Renew."
        });
      }
      var existingSubscription = _context.UserSubscriptionPlan
        .Where(u => u.UserGuid == UserGUID && u.SubsPlanId == Subscriptionid)
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

      //// Calculate remaining days if subscription has not expired
      //int remainingDays = (existingSubscription.EndDate > DateTime.UtcNow)
      //                    ? (existingSubscription.EndDate - DateTime.UtcNow).Days
      //                    : 0;

      //existingSubscription.EndDate = DateTime.UtcNow.AddDays(365 + remainingDays);
      //existingSubscription.Status = true;
      //existingSubscription.isrenew = false ;
      //existingSubscription.ModifiedOn = DateTime.UtcNow;

      //_context.UserSubscriptionPlan.Update(existingSubscription);
      //_context.SaveChanges();

      // Payment Processing
      try
      {
        decimal packageAmount = package.Price;
        var order = new OrderRequest()
        {
          CheckoutPaymentIntent = "CAPTURE",
          PurchaseUnits = new List<PurchaseUnitRequest>
            {
                new PurchaseUnitRequest
                {
                    AmountWithBreakdown = new AmountWithBreakdown
                    {
                        CurrencyCode = "USD",
                        Value = packageAmount.ToString("F2"),
                    },
                }
            },
          ApplicationContext = new PayPalCheckoutSdk.Orders.ApplicationContext
          {
            //ReturnUrl = "http://localhost:59013/Account/IsRenewPaymentSuccess",
            //CancelUrl = "http://localhost:59013/Company/Paymentfailure",
            ReturnUrl = "https://assr.digitalnoticeboard.biz/Account/IsRenewPaymentSuccess",
            CancelUrl = "https://assr.digitalnoticeboard.biz/Company/Paymentfailure",
            UserAction = "PAY_NOW",
            ShippingPreference = "NO_SHIPPING",
            LandingPage = "BILLING"
          }
        };

        var request = new OrdersCreateRequest();
        request.Prefer("return=representation");
        request.RequestBody(order);
        var paymentResponse = await _client.Execute(request);

        if (paymentResponse.StatusCode == HttpStatusCode.Created)
        {
          var result = paymentResponse.Result<PayPalCheckoutSdk.Orders.Order>();
          if (result != null)
          {
            var PaymentData = new PaymentDetails
            {
              UserGuid = UserGUID,
              SubPlanId = Subscriptionid,
              PaymentId = result.Id,
              CreatedOn = DateTime.UtcNow,
            };
            _context.PaymentDetails.Add(PaymentData);
            await _context.SaveChangesAsync();

            var approvalUrl = result.Links.FirstOrDefault(link => link.Rel == "approve")?.Href;
            return Ok(new { Status = true, Message = $"Subscription renew successfully.", ApprovalUrl = approvalUrl });
          }
        }
        return StatusCode(500, new { Status = false, Message = "Payment processing failed." });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { Status = false, Message = "Payment processing error.", Error = ex.Message });
      }
    }

    [HttpPost("UpgradeUserSubscriptions")]
    public async Task<IActionResult> UpgradeUserSubscriptions(string UserGUID, int Subscriptionid)
    {
      if (string.IsNullOrEmpty(UserGUID) || Subscriptionid <= 0)
      {
        return BadRequest(new
        {
          Status = false,
          Message = "Invalid parameters. Please provide a valid UserGUID and SubscriptionId."
        });
      }

      var newPackage = await _context.SubscriptionPackage.FirstOrDefaultAsync(p => p.PackageId == Subscriptionid);
      if (newPackage == null)
      {
        return BadRequest(new { Status = false, Message = "Selected subscription package not found." });
      }

      var existingSubscription = await _context.UserSubscriptionPlan
          .Where(u => u.UserGuid == UserGUID && u.Status == true)
          .OrderByDescending(u => u.EndDate)
          .FirstOrDefaultAsync();

      if (existingSubscription != null)
      {
        var currentPackage = await _context.SubscriptionPackage.FirstOrDefaultAsync(p => p.PackageId == existingSubscription.SubsPlanId);
        if (currentPackage == null)
        {
          return BadRequest(new { Status = false, Message = "Existing subscription package details not found." });
        }

        if (currentPackage.PackageName == newPackage.PackageName)
        {
          return BadRequest(new { Status = false, Message = $"You are already on the {newPackage.PackageName} plan. No upgrade needed." });
        }

        if (currentPackage.PackageName == "Premium")
        {
          return BadRequest(new { Status = false, Message = "You are already on the highest subscription plan (Premium)." });
        }
        if (currentPackage.PackageName == "Standard" && newPackage.PackageName == "Free")
        {
          return BadRequest(new { Status = false, Message = "Downgrading from Standard to Free is not allowed." });
        }
        if (currentPackage.PackageName == "Free" && newPackage.PackageName == "Free")
        {
          return BadRequest(new { Status = false, Message = "You already have a Free subscription. Please upgrade to Standard or Premium." });
        }
        if (currentPackage.PackageName == "Standard" && newPackage.PackageName == "Standard")
        {
          return BadRequest(new { Status = false, Message = "You are already on the Standard plan. Upgrade to Premium if needed." });
        }
        // Handle valid upgrades
        if ((currentPackage.PackageName == "Free" && (newPackage.PackageName == "Standard" || newPackage.PackageName == "Premium")) ||
            (currentPackage.PackageName == "Standard" && newPackage.PackageName == "Premium"))
        {
          existingSubscription.Status = false;
          existingSubscription.ModifiedOn = DateTime.UtcNow;
          _context.UserSubscriptionPlan.Update(existingSubscription);
        }
      }
      // Payment Processing
      try
      {
        decimal packageAmount = newPackage.Price;
        var order = new OrderRequest()
        {
          CheckoutPaymentIntent = "CAPTURE",
          PurchaseUnits = new List<PurchaseUnitRequest>
            {
                new PurchaseUnitRequest
                {
                    AmountWithBreakdown = new AmountWithBreakdown
                    {
                        CurrencyCode = "USD",
                        Value = packageAmount.ToString("F2"),
                    },
                }
            },
          ApplicationContext = new PayPalCheckoutSdk.Orders.ApplicationContext
          {
            //ReturnUrl = "http://localhost:59013/Account/UpgradePaymentSuccess",
            //CancelUrl = "http://localhost:59013/Company/Paymentfailure",
            ReturnUrl = "https://assr.digitalnoticeboard.biz/Account/UpgradePaymentSuccess",
            CancelUrl = "https://assr.digitalnoticeboard.biz/Company/Paymentfailure",
            UserAction = "PAY_NOW",
            ShippingPreference = "NO_SHIPPING",
            LandingPage = "BILLING"
          }
        };

        var request = new OrdersCreateRequest();
        request.Prefer("return=representation");
        request.RequestBody(order);
        var paymentResponse = await _client.Execute(request);

        if (paymentResponse.StatusCode == HttpStatusCode.Created)
        {
          var result = paymentResponse.Result<PayPalCheckoutSdk.Orders.Order>();
          if (result != null)
          {
            var PaymentData = new PaymentDetails
            {
              UserGuid = UserGUID,
              SubPlanId = Subscriptionid,
              PaymentId = result.Id,
              CreatedOn = DateTime.UtcNow,
            };
            _context.PaymentDetails.Add(PaymentData);
            await _context.SaveChangesAsync();

            var approvalUrl = result.Links.FirstOrDefault(link => link.Rel == "approve")?.Href;
            return Ok(new { Status = true, Message = $"Subscription upgraded to {newPackage.PackageName} successfully.", ApprovalUrl = approvalUrl });
          }
        }
        return StatusCode(500, new { Status = false, Message = "Payment processing failed." });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { Status = false, Message = "Payment processing error.", Error = ex.Message });
      }
    }
    [HttpGet("GetActiveSubscription")]
    public IActionResult GetActiveSubscription(string UserGUID)
    {
      if (string.IsNullOrEmpty(UserGUID))
      {
        return BadRequest(new
        {
          Status = false,
          Message = "Invalid parameter. Please provide a valid UserGUID."
        });
      }

      var activeSubscription = _context.UserSubscriptionPlan
          .Where(u => u.UserGuid == UserGUID && u.Status == true)
          .OrderByDescending(u => u.EndDate)
          .FirstOrDefault();

      if (activeSubscription == null)
      {
        return NotFound(new
        {
          Status = false,
          Message = "No active subscription found for this user."
        });
      }

      var subscriptionPackage = _context.SubscriptionPackage
          .FirstOrDefault(p => p.PackageId == activeSubscription.SubsPlanId);

      if (subscriptionPackage == null)
      {
        return NotFound(new
        {
          Status = false,
          Message = "Subscription package details not found."
        });
      }

      return Ok(new
      {
        Status = true,
        Message = "Active subscription found.",
        SubscriptionDetails = new
        {
          UserGUID = activeSubscription.UserGuid,
          SubscriptionId = activeSubscription.SubsPlanId,
          SubscriptionName = subscriptionPackage.PackageName,
          StartDate = activeSubscription.StartDate,
          EndDate = activeSubscription.EndDate,
          Status = activeSubscription.Status
        }
      });
    }

    //[HttpGet("GetSchedule")]
    //public void TestCronExpTestService(string cronExpression) 
    //{
    //  RecurringJob.AddOrUpdate<HangfireJobService>("TestCronExpTestService", x => x.TestCronExpTestService(), cronExpression);
    //}
    //[HttpGet("GetRenewSubscriptions")]
    //public void RenewSubscriptions(string cronExpression)
    //{
    //  RecurringJob.AddOrUpdate<HangfireJobService>("RenewSubscriptions", x => x.RenewSubscriptions(), cronExpression);
    //}
  }
}