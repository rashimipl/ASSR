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
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public DashboardController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet("GetAllUsersRegisterThisMonth")]
        [EnableCors("AllowAll")]
        [Authorize]
        public async Task<IActionResult> GetAllUsersRegisterThisMonth()
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var usersQuery = _context.Users.Where(x => x.CreatedOn.Month == currentMonth && x.CreatedOn.Year == currentYear).ToList();

            if (usersQuery == null || !usersQuery.Any())
            {
                return NotFound("No users found for the current month.");
            }

            return Ok(usersQuery);
        }


        [HttpGet("GetTotalPostThisMonths")]
        [EnableCors("AllowAll")]
        [Authorize]
        public async Task<IActionResult> GetTotalPostthisMonths()
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var data = _context.SocialMediaPosts.Where(x => x.CreatedAt.Month == currentMonth && x.CreatedAt.Year == currentYear).ToList();

            if (data == null || !data.Any())
            {
                return NotFound("Post not found for the current month.");
            }

            return Ok(data);
        }


        [HttpGet("Registered_Users_Comparing_Monthwise")]
        public async Task<IActionResult> Registered_Users_Comparing_Monthwise()
        {
            var users = await _context.Users.Where(x => x.CreatedOn != null).ToListAsync();

            var data = users
                .GroupBy(x => new { x.CreatedOn.Year, x.CreatedOn.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count(),
                    Users = g.ToList()
                })
                .ToList();

            if (!users.Any())
            {
                return NotFound("No users found.");
            }

            return Ok(users);
        }




        [HttpGet("GetAllRegisterUserThisMonths")]
        [EnableCors("AllowAll")]
        [Authorize]
        public async Task<IActionResult> GetAllregisterUserthisMonths()
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var data = _context.Users.Where(x => x.CreatedOn.Month == currentMonth && x.CreatedOn.Year == currentYear).ToList();

            if (data == null || !data.Any())
            {
                return NotFound("Users not found for the current month.");
            }

            return Ok(data);
        }



        [HttpGet("GetAllRecentlySubscribedUsers")]
        [EnableCors("AllowAll")]
        [Authorize]
        public async Task<IActionResult> GetAllRecentlySubscribedUsers()
        {
            var currentday = DateTime.Now.Date;
            var currentYear = DateTime.Now.Year;

            var data = _context.UserSubscriptions.Where(x => x.StartDate.Date == currentday && x.StartDate.Year == currentYear).ToList();

            if (data == null || !data.Any())
            {
                return NotFound("No Users Recently Subscribed .");
            }

            return Ok(data);
        }


        [HttpGet("GetAllExpireSubscribition")]
        [EnableCors("AllowAll")]
        [Authorize]
        public async Task<IActionResult> GetAllExpiredSubscriptions(DateTime currentDate)
        {
            var data = _context.UserSubscriptions
                .Where(x => x.EndDate > currentDate)
                .Select(x => new
                {
                    x.UserGUID,
                    x.StartDate,
                    x.EndDate,
                    Duration = (x.EndDate - x.StartDate).TotalDays // Calculate duration in days
                })
                .ToList();

            if (data == null || !data.Any())
            {
                return NotFound("No expired subscriptions found.");
            }

            return Ok(data);
        }


        [HttpGet("Posts_In_Trend")]
        public async Task<IActionResult> Posts_In_Trend()
        {
            int days = 7;
            var endDate = DateTime.UtcNow.Date; // Current UTC date
            var startDate = endDate.AddDays(-days); // Start date is endDate minus days
            var maxPostViewsCount = _context.PostViews.Max(pv => pv.PostViewsCount);

            var data = (from smp in _context.SocialMediaPosts
                        join pv in _context.PostViews on smp.Id equals pv.PostId
                        where pv.PostViewsCount == maxPostViewsCount && smp.CreatedAt >= startDate && smp.CreatedAt <= endDate
                        select new MediaPostViewsmodel
                        {
                            UserGuid = pv.UserGuid,
                            createdAt = smp.CreatedAt,
                            PostId = pv.PostId,
                            PostViewsCount = pv.PostViewsCount,
                            status = smp.Status,
                            title = smp.Title,
                            description = smp.Description,
                            postIcon = smp.PostIcon
                        }).FirstOrDefault();

            if (data != null)
            {
                return Ok(data);
            }
            return BadRequest(new { status = "false", Message = " Data Not Found!..." });
        }



        [HttpGet("Platform_In_Trend")]
        public async Task<IActionResult> Platform_In_Trend()
        {
            int days = 7;
            var endDate = DateTime.UtcNow.Date; // Current UTC date
            var startDate = endDate.AddDays(-days); // Start date is endDate minus days
            var maxPostViewsCount = _context.PostViews.Max(pv => pv.PostViewsCount);

            var data = (from smp in _context.SocialMediaPosts
                        join pv in _context.PostViews on smp.Id equals pv.PostId
                        join ugp in _context.UserGroupPosts on smp.Id equals ugp.PostId
                        join gsm in _context.GroupSocialMedia on ugp.GroupId equals gsm.GroupId
                        join sm in _context.SocialMedia on gsm.SocialMediaId equals sm.Id
                        where pv.PostViewsCount == maxPostViewsCount && smp.CreatedAt >= startDate && smp.CreatedAt <= endDate
                        select new MediaPostViewsmodel
                        {
                            UserGuid = pv.UserGuid,
                            createdAt = smp.CreatedAt,
                            PostId = pv.PostId,
                            PostViewsCount = pv.PostViewsCount,
                            status = smp.Status,
                            title = smp.Title,
                            description = smp.Description,
                            postIcon = smp.PostIcon,
                            SocialMediaName = sm.SocialMediaName
                        }).FirstOrDefault();

            if (data != null)
            {
                return Ok(data);
            }
            return BadRequest(new { status = "false", Message = " Data Not Found!..." });
        }



        [HttpGet("Tag_In_Trend")]
        [EnableCors("AllowAll")]
        [Authorize]
        public async Task<IActionResult> Tag_In_Trend()
        {
            var oneWeekAgo = DateTime.Now.AddDays(-7);

            var data = _context.Hashtag
                 .Where(h => h.CreatedOn >= oneWeekAgo)
                 .GroupBy(h => h.HashtagName)
                 .Select(g => new
                 {
                     HashtagName = g.Key,
                     UsageCount = g.Count()
                 })
                 .OrderByDescending(result => result.UsageCount)
                 .FirstOrDefault();


            if (data == null)
            {
                return NotFound("No data found.");
            }

            return Ok(data);
        }


        [HttpPost("Create_HashtagGroup")]
        public IActionResult Create_HashtagGroup([FromBody] HashtagGroup model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            HashtagGroup result = new HashtagGroup()
            {
                HashtagGroupName = model.HashtagGroupName,
                CreatedOn = DateTime.Now,


            };
            _context.HashtagGroup.Add(result);
            _context.SaveChanges();
            return Ok(new { status = "True", message = "HashtagGroup Created successfully" });

        }


    }

}