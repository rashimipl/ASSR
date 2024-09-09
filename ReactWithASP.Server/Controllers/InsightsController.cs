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


namespace ReactWithASP.Server.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class InsightsController : Controller
    {
        /*[HttpGet("Posts")]*/
        [HttpGet("Posts")]
        [Authorize]
        public IActionResult Insights([FromQuery] InsightRequest request)
        {
            var response = new List<HomeScreenResponse>
            {
                new HomeScreenResponse
                {
                    Id = 1,
                    Type = "facebook",
                    Views = 20000,
                    Followers = 100000,
                    Likes = 9060,
                    Share = 2858,

                },
                new HomeScreenResponse
                {
                    Id = 2,
                    Type = "instagram",
                    Views = 25000,
                    Followers = 90000,
                    Likes = 6010,
                    Share = 2811
                },
                new HomeScreenResponse
                {
                    Id = 3,
                    Type = "tiktok",
                    Views = 7000,
                    Followers = 300,
                    Likes = 6050,
                    Share = 9232
                }

            };

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
        [Authorize]
        public IActionResult InsightsResult ([FromQuery] InsightRequest request)
        {
            var response = new List<InsightsResponse>
            {
                new InsightsResponse
                {
                    TotalViews = 540000,
                    TotalLikes = 65000,
                    TotalShares = 61000,
                    TotalFollowers = 1000000,
                    ViewStatus = true,
                    LikeStatus = false,
                    ShareStatus = true,
                    FollowerStatus = false
                }
            };

            return Ok(response);
        }

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
