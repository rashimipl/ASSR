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
using Microsoft.EntityFrameworkCore;


namespace ReactWithASP.Server.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class InsightsController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment;

        public InsightsController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment, ApplicationDbContext context)
        {
            _userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
            Environment = environment;
            _context = context;
        }


       
        [HttpGet("Posts")]
        public IActionResult Insights([FromQuery] InsightRequest request)
        {

      //         var response = (
      //         from likes in _context.PostLikes
      //         join shares in _context.PostShares on likes.UserGuid equals shares.UserGuid
      //         join views in _context.PostViews on shares.UserGuid equals views.UserGuid
      //         join cinfo in _context.ConnectedSocialMediaInfo on views.UserGuid equals cinfo.UserId
      //         join sm in _context.SocialMedia on cinfo.SocialMediaAccId equals sm.Id
      //         group new { likes, shares, views } by new { likes.UserGuid, sm.SocialMediaName } into grouped
      //         select new HomeScreenResponse
      //         {
      //             UserGuid = grouped.Key.UserGuid,
      //             Type = grouped.Key.SocialMediaName,
      //             Likes = grouped.Select(g => g.likes).Distinct().Sum(l => l.PostLikesCount),
      //             Share = grouped.Select(g => g.shares).Distinct().Sum(s => s.PostSharesCount),
      //             Views = grouped.Select(g => g.views).Distinct().Sum(v => v.PostViewsCount),
      //             Followers = 100000 // Placeholder value; adjust as needed
      //         }
      //).ToList();

      var response =
    (from likes in _context.PostLikes
     join shares in _context.PostShares
         on likes.UserGuid equals shares.UserGuid
     join views in _context.PostViews
         on likes.UserGuid equals views.UserGuid // Ensured join consistency
     join cinfo in _context.ConnectedSocialMediaInfo
         on likes.UserGuid equals cinfo.UserId // Ensured join consistency
     join sm in _context.SocialMedia
         on cinfo.SocialMediaAccId equals sm.Id
     where likes.UserGuid == request.UserGuid
     group new { likes, shares, views } by new
     {
       likes.UserGuid,
       sm.SocialMediaName
     } into g
     select new
     {
       UserGuid = g.Key.UserGuid,
       Type = g.Key.SocialMediaName,
       Likes = g.Select(x => x.likes.PostLikesCount).Sum(), // Removed Distinct, as it's unnecessary in sums
       Share = g.Select(x => x.shares.PostSharesCount).Sum(),
       Views = g.Select(x => x.views.PostViewsCount).Sum(),
       Followers = 100000, // Placeholder value
       TotalLikes = g.Select(x => x.likes.PostLikesCount).Sum(),
       TotalShares = g.Select(x => x.shares.PostSharesCount).Sum(),
       TotalViews = g.Select(x => x.views.PostViewsCount).Sum(),
       TotalFollowers = 1000000 // Placeholder value
     }).ToList();

      //var response = new List<HomeScreenResponse>
      //{
      //    new HomeScreenResponse
      //    {
      //        Id = 1,
      //        Type = "facebook",
      //        Views = 20000,
      //        Followers = 100000,
      //        Likes = 9060,
      //        Share = 2858,

      //    },
      //    new HomeScreenResponse
      //    {
      //        Id = 2,
      //        Type = "instagram",
      //        Views = 25000,
      //        Followers = 90000,
      //        Likes = 6010,
      //        Share = 2811
      //    },
      //    new HomeScreenResponse
      //    {
      //        Id = 3,
      //        Type = "tiktok",
      //        Views = 7000,
      //        Followers = 300,
      //        Likes = 6050,
      //        Share = 9232
      //    }

      //};

      return Ok(response);
        }


        [HttpGet("Stories")]
        [Authorize]
        public IActionResult InsightsStories([FromQuery] InsightRequest request)
        {
            var response = new List<HomeScreenResponse>
            {
                new HomeScreenResponse
                {
                    Id = 1,
                    Type = "facebook",
                    Views = 19000,
                    Followers = 200000,
                    Likes = 9055,
                    Share = 2879
                },
                new HomeScreenResponse
                {
                    Id = 2,
                    Type = "instagram",
                    Views = 30000,
                    Followers = 65000,
                    Likes = 7610,
                    Share = 3086
                },
                new HomeScreenResponse
                {
                    Id = 3,
                    Type = "tiktok",
                    Views = 80000,
                    Followers = 5700,
                    Likes = 54050,
                    Share = 5632
                }
            };

            return Ok(response);
        }

        [HttpGet("Total")]
        public IActionResult InsightsResult([FromQuery] InsightRequest request)
        {
            if (string.IsNullOrEmpty(request.UserGuid))
            {
                return BadRequest(new { Message = "UserGuid is required" });
            }

            var response = (
                from likes in _context.PostLikes
                where likes.UserGuid == request.UserGuid
                join shares in _context.PostShares on likes.UserGuid equals shares.UserGuid
                join views in _context.PostViews on likes.UserGuid equals views.UserGuid
                group new { likes, shares, views } by likes.UserGuid into grouped
                select new InsightsResponse
                {
                    TotalLikes = grouped.Select(g => g.likes).Distinct().Sum(l => l.PostLikesCount),
                    TotalShares = grouped.Select(g => g.shares).Distinct().Sum(s => s.PostSharesCount),
                    TotalViews = grouped.Select(g => g.views).Distinct().Sum(v => v.PostViewsCount),
                    TotalFollowers = 1000000, // Replace with real follower logic if needed
                    ViewStatus = grouped.Sum(g => g.views.PostViewsCount) > 0,
                    LikeStatus = grouped.Sum(g => g.likes.PostLikesCount) > 0,
                    ShareStatus = grouped.Sum(g => g.shares.PostSharesCount) > 0,
                    FollowerStatus = true // Update based on actual conditions
                }
            ).ToList();

            if (response.Count == 0)
            {
                return NotFound(new { Message = "No data found for the specified UserGuid" });
            }

            return Ok(response);
        }

        //[Authorize]
        //public IActionResult InsightsResult ([FromQuery] InsightRequest request)
        //{
        //    var response = new List<InsightsResponse>
        //    {
        //        new InsightsResponse
        //        {
        //            TotalViews = 540000,
        //            TotalLikes = 65000,
        //            TotalShares = 61000,
        //            TotalFollowers = 1000000,
        //            ViewStatus = true,
        //            LikeStatus = false,
        //            ShareStatus = true,
        //            FollowerStatus = false
        //        }
        //    };

        //    return Ok(response);
        //}

        [HttpGet("Stories/Total")]
        [Authorize]
        public IActionResult StoriesTotal([FromQuery] InsightRequest request)
        {
            var response = new List<InsightsResponse>
            {
                new InsightsResponse
                {
                    TotalViews = 920000,
                    TotalLikes = 98000,
                    TotalShares = 11000,
                    TotalFollowers = 2300000,
                    ViewStatus = false,
                    LikeStatus = true,
                    ShareStatus = false,
                    FollowerStatus = true
                }
            };

            return Ok(response);
        }


    }
}
