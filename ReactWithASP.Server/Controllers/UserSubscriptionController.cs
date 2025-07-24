using JWTAuthentication.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ReactWithASP.Server.Models;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ReactWithASP.Server.Controllers
{
    [Route("api")]
    [ApiController]
    public class UserSubscriptionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserSubscriptionController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        [Route("UserSubscriptionListbyId")]
        public IActionResult UserSubscriptionListbyId(int Id)
        {
            var usersQuery = (from userSub in _context.UserSubscriptions
                              join plan in _context.SubscriptionPlans on userSub.SubsPlanId equals plan.PlanId
                              join U in _context.Users on userSub.UserGuid equals U.Id where userSub.Id==Id
                              select new
                              {
                                  userSub.Id,
                                  userSub.UserGuid,
                                  userSub.StartDate,
                                  userSub.EndDate,
                                  userSub.Status,
                                  userSub.SubsPlanId,
                                  userSub.ManualSubscription,
                                  plan.PlanId,
                                  plan.Price,
                                  U.UserName
                              }).ToList();


             return Ok(new { status = true, usersQuery });

        }

        [HttpGet]
        [Route("UserSubscriptionDetailList")]
        //[Authorize]
        public async Task<IActionResult> UserSubscriptionDetailList(string? Status, string? Username, DateTime SubsStartDate, DateTime SubsEndDate, string sortColumn = "Id", string sortOrder = "asc", int pageNumber = 1, int pageSize = 10)
        {

            var usersQuery = (from userSub in _context.UserSubscriptions
                              join plan in _context.SubscriptionPlans on userSub.SubsPlanId equals plan.PlanId
                              join U in _context.Users on userSub.UserGuid equals U.Id
                              select new
                              {
                                  userSub.Id,
                                  userSub.UserGuid,
                                  userSub.StartDate,
                                  userSub.EndDate,
                                  userSub.Status,
                                  userSub.SubsPlanId,
                                  userSub.ManualSubscription,
                                  plan.PlanId,
                                  plan.Price,
                                  U.UserName
                              }).ToList();

            if (usersQuery != null)
            {
                // Sorting
                switch (sortColumn.ToLower())
                {
                    case "SubsPlanID":
                        usersQuery = sortOrder.ToLower() == "asc" ? usersQuery.OrderBy(u => u.SubsPlanId).ToList() : usersQuery.OrderByDescending(u => u.SubsPlanId).ToList();
                        break;
                    case "id":
                    default:
                        usersQuery = sortOrder.ToLower() == "asc" ? usersQuery.OrderBy(u => u.Id).ToList() : usersQuery.OrderByDescending(u => u.Id).ToList(); ;
                        break;
                }

                if (Status != null)
                {
                    usersQuery = usersQuery.Where(x => x.Status == Status).ToList();
                }
                //if (usersQuery != null)
                //{
                //    usersQuery = usersQuery.Where(x => x.paymentStatus == paymentStatus).ToList();
                //}
                if (Username != null)
                {
                    usersQuery = usersQuery.Where(x => x.UserName == Username).ToList();
                }
                // Filtering by Subscription Start Date
                if (SubsStartDate != DateTime.MinValue)
                {
                    usersQuery = usersQuery.Where(x => x.StartDate >= SubsStartDate.Date).ToList();
                }

                // Filtering by Subscription End Date
                if (SubsEndDate != DateTime.MinValue)
                {
                    usersQuery = usersQuery.Where(x => x.EndDate <= SubsEndDate.Date).ToList();
                }

                // Check if start date is before end date
                if (SubsStartDate != DateTime.MinValue && SubsEndDate != DateTime.MinValue && SubsStartDate > SubsEndDate)
                {
                    return BadRequest("Start Date cannot be greater than End Date.");
                }

                // Pagination
                var totalRecords = usersQuery.Count;
                var users = usersQuery.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                if (users == null || !users.Any())
                {
                    return NotFound("User not found");
                }

                // Creating pagination metadata
                var paginationMetadata = new
                {
                    totalRecords = totalRecords,
                    pageSize = pageSize,
                    currentPage = pageNumber,
                    totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                    data = users
                };

                // Return results along with pagination metadata
                return Ok(new { status = true, paginationMetadata });
            }
            else
            {
                return NotFound("Data not found !..");
            }
        }


        [HttpPost("SaveUserSubscriptionInfo")]
        public async Task<IActionResult> SaveUserSubscriptionInfo([FromForm] SaveUserSubscription model)
        {
            // Validate the input
            if (string.IsNullOrEmpty(model.UserGuid) || string.IsNullOrEmpty(model.PlanName))
            {
                return BadRequest("Invalid data provided.");
            }

            var planId = await _context.SubscriptionPlans
                .Where(x => x.PlanName == model.PlanName)
                .Select(x => x.PlanId)
                .FirstOrDefaultAsync();

            // Check if the planId was found
            if (planId == 0) // Assuming 0 indicates no plan found
            {
                return BadRequest("Invalid plan name provided.");
            }

            // Create UserSubscription entity
            var userSubs = new UserSubscriptions
            {
                //Id = model.Id,
                UserGuid = model.UserGuid,
                SubsPlanId = planId,
                StartDate = model.StartDate ?? DateTime.Now,
                EndDate = model.EndDate ?? DateTime.Now,
                Status = model.Status,
                ManualSubscription = model.ManualSubscription,
                CreatedOn = DateTime.Now
            };

            // Add to database
            await _context.UserSubscriptions.AddAsync(userSubs);

            // Save changes asynchronously
            await _context.SaveChangesAsync();

            // Return a success response
            return Ok(new { status = true, message = "Saved successfully." });
        }


        [HttpGet]
        [Route("GetUserSubscriptionPlanDeailsbyId")]
        public async Task<IActionResult> GetUserSubscriptionPlanDeailsbyId(string Userguid)
        {
            var result = (from U in _context.Users
                              join us in _context.UserSubscriptions on U.Id equals us.UserGuid
                              join usp in _context.SubscriptionPlans on us.SubsPlanId equals usp.PlanId 
                              where U.Id==Userguid
                              select new UserSubscriptionPlanDeailsbyId
                              {
                                  UserGuid = U.Id,
                                  UserName = U.FullName,
                                  PlanId = usp.PlanId,
                                  PlanName = usp.PlanName,
                                  Price = usp.Price,
                                  ConnectedChannels = usp.ConnectedChannels,
                                  SmartContentSuggestionsMonthly = usp.SmartContentSuggestionsMonthly,
                                  ImageSuggestionsMonthly = usp.ImageSuggestionsMonthly,
                                  DailyPostInspirations = usp.DailyPostInspirations,
                                  DraftedPosts = usp.DraftedPosts,
                                  PostsDaily = usp.PostsDaily,
                                  ScheduledPostsQueue = usp.ScheduledPostsQueue,
                                  MultiImageVideoPosts = usp.MultiImageVideoPosts,
                                  RecurringPosts = usp.RecurringPosts,
                                  PremiumSupport = usp.PremiumSupport
                              }).ToList();

            if (result != null)
            {
                return Ok(new { status = true, result });
            }
            else
            {
                return NotFound("Data not found !..");
            }
        }



    }
}