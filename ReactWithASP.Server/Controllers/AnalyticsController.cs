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
using System.Globalization;
using Microsoft.EntityFrameworkCore;


namespace ReactWithASP.Server.Controllers
{
    [ApiController]
    [Route("api")]
    public class AnalyticsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
    private readonly HttpClient _httpClient;
    public AnalyticsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
      _httpClient = new HttpClient();
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


    [HttpGet("GetPagePosts")]
    public async Task<IActionResult> GetFacebookPagePosts(string userguid, string pageId, string accessToken, string date)
    {
      try
      {
        var url = $"https://graph.facebook.com/v19.0/{pageId}/posts?access_token={accessToken}";

        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
          return BadRequest("Failed to fetch posts from Facebook.");

        var content = await response.Content.ReadFromJsonAsync<FacebookPostsResponse>();
        if (content?.data == null || content.data.Count == 0)
          return NotFound("No posts found.");

        // Parse date as date only
        var targetDate = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture).Date;
        var todayDate = DateTime.UtcNow.Date;

        var posts = content.data
            .Where(p =>
            {
              var createdTimeUtc = DateTime.Parse(p.created_time, null, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
              return createdTimeUtc.Date >= targetDate && createdTimeUtc.Date <= todayDate;
            })
            .Select(p => new
            {
              PostId = p.id,
              CreatedAt = DateTime.Parse(p.created_time).ToString("yyyy-MM-dd") // formatted without time
            })
            .ToList();
        return Ok(posts);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Error: {ex.Message}");
      }
    }
    [HttpGet("GetFacebookPostAnalytics")]
    public async Task<FacebookPostAnalytics> GetFacebookPostAnalytics(string postId)
    {
      var existuser = await _context.PostIdForSocialMediaPosts.FirstOrDefaultAsync(p => p.PostId == postId);

      using var http = new HttpClient();
      var baseUrl = "https://graph.facebook.com/v19.0";

      // 1. Get Reach
      var reachUrl = $"{baseUrl}/{postId}/insights?metric=post_impressions&access_token={existuser.PageAccessToken}";
      var reachData = await http.GetFromJsonAsync<FacebookInsightsResponse>(reachUrl);
      int reach = reachData?.data?.FirstOrDefault()?.values?.FirstOrDefault()?.value ?? 0;

      // 2. Get Likes
      var likesUrl = $"{baseUrl}/{postId}/likes?summary=true&access_token={existuser.PageAccessToken}";
      var likesData = await http.GetFromJsonAsync<FacebookLikesResponse>(likesUrl);
      int likes = likesData?.summary?.total_count ?? 0;

      // 3. Get Shares
      var sharesUrl = $"{baseUrl}/{postId}?fields=shares&access_token={existuser.PageAccessToken}";
      var sharesData = await http.GetFromJsonAsync<FacebookSharesResponse>(sharesUrl);
      int shares = sharesData?.shares?.count ?? 0;

      // 4. Get Video Views (optional)
      var viewsUrl = $"{baseUrl}/{postId}/video_insights?metric=total_video_views&access_token={existuser.PageAccessToken}";
      var viewsData = await http.GetFromJsonAsync<FacebookVideoViewsResponse>(viewsUrl);
      int views = viewsData?.data?.FirstOrDefault()?.values?.FirstOrDefault()?.value ?? 0;

      return new FacebookPostAnalytics
      {
        PostId = postId,
        Reach = reach,
        Likes = likes,
        Shares = shares

      };
    }    
    //[HttpGet("audience-insights")]
    //public async Task<IActionResult> GetAudienceInsights(string userAccessToken)
    //{
    //  // Step 1: Get list of Pages the user manages
    //  var accountsUrl = $"https://graph.facebook.com/v19.0/me/accounts?access_token={userAccessToken}";
    //  var accountsResp = await _httpClient.GetAsync(accountsUrl);
    //  if (!accountsResp.IsSuccessStatusCode)
    //    return StatusCode((int)accountsResp.StatusCode, "Failed to fetch user pages");

    //  var accountData = await accountsResp.Content.ReadFromJsonAsync<FacebookPageResponse>();

    //  var firstPage = accountData?.data?.FirstOrDefault();
    //  if (firstPage == null)
    //    return BadRequest("No pages found for this user.");

    //  var pageId = firstPage.id;
    //  var pageAccessToken = firstPage.access_token;

    //  // Step 2: Fetch Insights for the Page
    //  var insightsUrl = $"https://graph.facebook.com/v19.0/{pageId}/insights/page_fans_gender_age?access_token={pageAccessToken}";
    //  var insightsResp = await _httpClient.GetAsync(insightsUrl);
    //  if (!insightsResp.IsSuccessStatusCode)
    //  {
    //    var err = await insightsResp.Content.ReadAsStringAsync();
    //    return StatusCode((int)insightsResp.StatusCode, $"Failed to fetch insights: {err}");
    //  }

    //  var insightsData = await insightsResp.Content.ReadFromJsonAsync<FacebookInsightsMW>();

    //  int men = 0, women = 0;
    //  var values = insightsData?.data?.FirstOrDefault()?.Values?.FirstOrDefault()?.Value;
    //  if (values != null)
    //  {
    //    foreach (var item in values)
    //    {
    //      if (item.Key.StartsWith("M.")) men += item.Value;
    //      else if (item.Key.StartsWith("F.")) women += item.Value;
    //    }
    //  }

    //  return Ok(new
    //  {
    //    platform = "Facebook",
    //    accountId = pageId,
    //    audience = new { men, women }
    //  });
    //}


  }
}
