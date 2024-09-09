using JWTAuthentication.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using ReactWithASP.Server.Models;
using System.Text.RegularExpressions;
using ReactWithASP.Server.Models.Posts;


namespace ReactWithASP.Server.Controllers
{
    [ApiController]
    [Route("api")]
    public class AnalyticsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public AnalyticsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }


        [HttpGet("analytics")]
        public IActionResult Analytics([FromQuery] AnalyticsRequest request)
        {

            // Calculate the start date as 7 days before the current date
            var startDate = DateTime.Now.AddDays(-7);
            var endDate = DateTime.Now;
            // Assuming you have DbSet<Like>, DbSet<Share>, and DbSet<View> in your DbContext
            var totalLikes = _context.PostLikes
                .Where(l => l.UserGuid == request.userGUId && l.CreatedAt >= startDate && l.CreatedAt <= endDate)
                .Count();

            var totalShares = _context.PostViews
                .Where(s => s.UserGuid == request.userGUId && s.CreatedAt >= startDate && s.CreatedAt <= endDate)
                .Count();

            var totalViews = _context.PostShares
                .Where(v => v.UserGuid == request.userGUId && v.CreatedAt >= startDate && v.CreatedAt <= endDate)
                .Count();

            // Assuming "Reach" is a custom calculation based on your business logic
            var totalReach = CalculateReach(totalLikes, totalShares, totalViews);

            var response = new AnalyticsResponse
            {
                TotalLikes = totalLikes,
                TotalShares = totalShares,
                TotalViews = totalViews,
                TotalReach = totalReach
            };

            return Ok(response);
        }

        private int CalculateReach(int likes, int shares, int views)
        {
            // Example logic for calculating reach
            return views + (shares * 2) + (likes * 3);
        }



        //public IActionResult Analytics([FromQuery] AnalyticsRequest request)
        //{
        //    var response = new List<AnalyticsResponse>
        //    {
        //        new AnalyticsResponse
        //        {
        //            TotalViews = 540000,
        //            TotalLikes = 65000,
        //            TotalShares = 61000,
        //            TotalReach = 1000000
        //        }
        //    };

        //    return Ok(response);
        //}

        [HttpGet("Audience")]

        public IActionResult Audience([FromQuery] AudienceRequest request)
        {
            var response = new List<AudienceResponse>
            {
                new AudienceResponse
                {
                    Man = 540000,
                    Woman = 65000,
                    
                }
            };

            return Ok(response);
        }
    }
}
