using JWTAuthentication.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using ReactWithASP.Server.Models;
using System.Numerics;

namespace ReactWithASP.Server.Controllers
{
    [Route("api")]
    [ApiController]
    public class SubscriptionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
       
        public SubscriptionController(UserManager<ApplicationUser> userManager,ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
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
        [Authorize]
        public async Task<IActionResult> Plans(int Id)
        {
            var Plan = _context.SubscriptionPlans.Where(x => x.Id == Id).ToList();
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

            var userSubscription = _context.UserSubscriptions.FirstOrDefault(us => us.UserGUID == model.UserGUID);
            if (userSubscription == null)
            {
                return NotFound("User subscription not found");
            }

            var subscriptionPlan = _context.SubscriptionPlans.FirstOrDefault(sp => sp.Id == userSubscription.SubsPlanID);
            if (subscriptionPlan == null)
            {
                return NotFound("Subscription plan not found");
            }

            var response = new UserStatusResponse
            {
                UserGUID = model.UserGUID,
                Status = userSubscription.Status ,
                Subscription = subscriptionPlan.PlanName,
                SubscriptionStatus = userSubscription.EndDate > DateTime.Now ? "Valid" : "Expired",
                SubscriptionExpiringOn = userSubscription.EndDate
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


        //Static
        [HttpGet("GetAllSubscriptionPlans")]
        [Authorize]
        public IActionResult GetAllSubscriptionPlans()
        {
            var plans = new List<SubscriptionPlan>
            {
                new SubscriptionPlan
                {
                    ID = 1,
                    Name = "Free",
                    ConnectedChannels = 3,
                    SmartContentSuggestionsMonthly = 5,
                    ImageSuggestionsMonthly = 10,
                    DailyPostInspirations = "5",
                    DraftedPosts = "3",
                    PostsDaily = "10",
                    ScheduledPostsQueue = "5",
                    MultiImageVideoPosts = true,
                    RecurringPosts = true,
                    PremiumSupport = false
                },
                new SubscriptionPlan
                {
                    ID = 2,
                    Name = "Standard",
                    ConnectedChannels = 20,
                    SmartContentSuggestionsMonthly = 200,
                    ImageSuggestionsMonthly = 200,
                    DailyPostInspirations = "Unlimited",
                    DraftedPosts = "Unlimited",
                    PostsDaily = "Unlimited",
                    ScheduledPostsQueue = "Unlimited",
                    MultiImageVideoPosts = true,
                    RecurringPosts = true,
                    PremiumSupport = true
                },
                new SubscriptionPlan
                {
                    ID = 3,
                    Name = "Premium",
                    ConnectedChannels = 20,
                    SmartContentSuggestionsMonthly = 200,
                    ImageSuggestionsMonthly = 200,
                    DailyPostInspirations = "Unlimited",
                    DraftedPosts = "Unlimited",
                    PostsDaily = "Unlimited",
                    ScheduledPostsQueue = "Unlimited",
                    MultiImageVideoPosts = true,
                    RecurringPosts = true,
                    PremiumSupport = true
                }
            };

            return Ok(plans);
        }


        [HttpGet]
        [Route("GetAllUsersUnderSelectedSubscription")]
        public IActionResult GetAllUsersUnderSelectedSubscription(string PlanName)
        {
            var query = from sp in _context.SubscriptionPlans
                        join us in _context.UserSubscriptions on sp.Id equals us.SubsPlanID
                        join u in _context.Users on us.UserGUID equals u.Id
                        where sp.PlanName.ToLower() == PlanName.ToLower()
                        select new subscriptionplanbyuserid
                        {
                            UserGUID = u.Id,
                            PlanName = sp.PlanName,
                            Price = sp.Price,
                            SubsPlanID = us.SubsPlanID,
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
            var result = _context.UserSubscriptions.Where(u => u.UserGUID == model.UserGuid).ToList();

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


    }
}
