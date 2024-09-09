using JWTAuthentication.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReactWithASP.Server.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
//using Microsoft.AspNetCore.Authentication.Twitter;
using Newtonsoft.Json;
using Microsoft.AspNetCore.WebUtilities;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using JWTAuthentication;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Cors;
using ReactWithASP.Server.Authentication;
using System;
using static YourNamespace.Controllers.AccountController;
using System.Reflection.Emit;

namespace ReactWithASP.Server.Controllers
{
    [ApiController]
    [Route("api")]
    public class SocialMediaConnectController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment;
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly string _accessToken;
        private readonly IHttpClientFactory _httpClientFactory;
        //try code 
        //private readonly string clientId = "483927064226359";
        //private readonly string state = "some_string";
        //private readonly string redirectUri = "https://localhost:7189/api/facebook-callback";
        //private readonly string scope = "pages_manage_posts, pages_read_engagement";
        //private readonly string config_id = "871588441670167";
        //private CustomWebApplicationFactory<Startup> _factory;
        //private HttpClient _client;



        public SocialMediaConnectController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment, ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
            Environment = environment;
            _context = context;
        }

        [HttpGet("FacebookLogin")]
        public IActionResult LoginWithFacebook(string userId)
        {
            var clientId = "483927064226359";
            var redirectUri = $"https://assr.digitalnoticeboard.biz/api/FacebookCallBack?userId={userId}"; // Pass userId to the callback
            var state = Guid.NewGuid().ToString("N");
            var scope = "pages_manage_posts,pages_read_engagement"; // Facebook page permissions

            var authorizationUrl = $"https://www.facebook.com/v17.0/dialog/oauth?client_id={clientId}&redirect_uri={Uri.EscapeDataString(redirectUri)}&state={state}&scope={Uri.EscapeDataString(scope)}";

            return Redirect(authorizationUrl);
        }
        [HttpGet("FacebookCallBack")]
        public async Task<IActionResult> FacebookCallback(string code, string state, string userid)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest(new { error = "Missing authorization code" });
            }

            var clientId = "483927064226359";
            var clientSecret = "66725e0d3573bbd8e47bcc92e9173892";
            //var redirectUri = "https://167.86.105.98.8070/api/FacebookCallBack";
            var redirectUri = $"https://assr.digitalnoticeboard.biz/api/FacebookCallBack?userId={userid}";
            var tokenRequestUrl = "https://graph.facebook.com/v17.0/oauth/access_token";

            // Add required parameters
            var parameters = new Dictionary<string, string>
    {
        { "client_id", clientId },
        { "client_secret", clientSecret },
        { "redirect_uri", redirectUri },
        { "code", code }
    };

            using (var httpClient = new HttpClient())
            {
                // Build the request URI with query parameters
                var requestUri = QueryHelpers.AddQueryString(tokenRequestUrl, parameters);
                var response = await httpClient.GetAsync(requestUri);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    // Handle error response
                    return BadRequest(new { error = "Error retrieving access token", details = responseString });
                }
                // Parse the response to get the access token
                var tokenResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseString);
                if (!tokenResponse.TryGetValue("access_token", out var accessToken))
                {
                    return BadRequest(new { error = "Error parsing access token" });
                }

                //Console.WriteLine(responseString);
                var userInfoUrl = $"https://graph.facebook.com/me?fields=id,name,picture&access_token={accessToken}";
                var userResponseMessage = await httpClient.GetAsync(userInfoUrl);
                var userResponseString = await userResponseMessage.Content.ReadAsStringAsync();

                if (!userResponseMessage.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error retrieving Facebook user details: {userResponseString}");
                    return BadRequest(new { error = "Failed to retrieve Facebook user details" });
                }

                var jsonResponse = JObject.Parse(userResponseString);
                //string userId = jsonResponse["id"]?.ToString();
                string userId = userid;
                var existingUser = _context.FacebookUsers.FirstOrDefault(f => f.UserId == userId);
                FacebookPageModel facebookPageModel = null;
                if (existingUser == null || existingUser.UserId != userId)
                {
                    facebookPageModel = new FacebookPageModel
                    {
                        UserId = userId,
                        AccessToken = accessToken
                    };
                    _context.FacebookUsers.Add(facebookPageModel);
                    var result = await _context.SaveChangesAsync();
                }

                var htmlContent = @"
    <!DOCTYPE html>
    <html lang='en'>
    <head>
        <meta charset='UTF-8'>
        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
        <title>Authenticated</title>
        <style>
            body {
                display: flex;
                justify-content: center;
                align-items: center;
                height: 100vh;
                margin: 0;
                font-family: Arial, sans-serif;
                background-color: #f0f0f0;
            }
            .message {
                padding: 20px;
                background-color: #ffffff;
                border: 1px solid #ddd;
                border-radius: 5px;
                box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
                text-align: center;
            }
          </style>
          </head>
          <body>
           <div class='message'>
            You are authenticated. Please close this tab.
          </div>
            </body>
          </html>";
                return new ContentResult
                {
                    Content = htmlContent,
                    ContentType = "text/html",
                    StatusCode = 200
                };
            }
        }

        [HttpGet("FacebookUserPages")]
        public async Task<List<FacebookPageData>> GetFacebookPages(string userId, string accessToken)
        {

            using (var httpClient = new HttpClient())
            {
                var userInfoUrl = $"https://graph.facebook.com/me?fields=id,name,picture&access_token={accessToken}";
                var userResponseMessage = await httpClient.GetAsync(userInfoUrl);
                var userResponseString = await userResponseMessage.Content.ReadAsStringAsync();
                var jsonResponse = JObject.Parse(userResponseString);
                string userIds = jsonResponse["id"]?.ToString();

                var response = await httpClient.GetAsync($"https://graph.facebook.com/v20.0/{userIds}/accounts?access_token={accessToken}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var pagesData = JObject.Parse(content)["data"] as JArray;

                var pages = new List<FacebookPageData>();
                foreach (var page in pagesData)
                {
                    var pageId = page["id"]?.ToString();
                    var pageName = page["name"]?.ToString();
                    var pageAccessToken = page["access_token"]?.ToString();

                    // Fetch the profile picture
                    var profileResponse = await httpClient.GetAsync($"https://graph.facebook.com/v20.0/{pageId}/picture?redirect=false&access_token={accessToken}");
                    profileResponse.EnsureSuccessStatusCode();

                    var profileContent = await profileResponse.Content.ReadAsStringAsync();
                    var profileData = JObject.Parse(profileContent)["data"];
                    var profileUrl = profileData["url"]?.ToString();

                    pages.Add(new FacebookPageData
                    {
                        Id = pageId,
                        Name = pageName,
                        Profile = profileUrl,
                        AccessToken = pageAccessToken
                    });
                }

                return pages;
            }
        }
        [HttpPost("facebookAuth")]
        public async Task<IActionResult> FacebookAuth(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("UserId is missing or invalid.");
            }
            var existingUser = await _context.FacebookUsers.FirstOrDefaultAsync(u => u.UserId == userId);
            var dta = (await GetFacebookPages(existingUser.UserId, existingUser.AccessToken)).ToList();
            if (dta == null)
            {
                return NotFound(new { message = "UserId not found." });
            }
            //var response = new
            //{
            //  message = "Facebook auth successful",
            //  userId = existingUser.UserId
            //};
            Console.WriteLine(JsonConvert.SerializeObject(dta));

            var formattedPages = new List<object>();
            foreach (var page in dta)
            {
                formattedPages.Add(new
                {
                    PageId = page.Id,
                    PageName = page.Name,
                    PageProfile = page.Profile,
                    pageAccessToken = page.AccessToken

                });
            }

            return Ok(new
            {
                Status = "Success",
                Message = "Facebook auth successful",
                Data = new
                {
                    User = new
                    {
                        UserId = userId
                    },
                    Pages = formattedPages
                }
            });

            //return Ok(response);
        }
        
        [HttpPost("FacebookUpload")]
        public async Task<IActionResult> FacebookUpload(IFormFile videoFile, string pageId, string accessToken)
        {
            if (videoFile == null)
            {
                return BadRequest("Video file is required.");
            }

            using (var client = new HttpClient())
            {
                // Start the upload session
                var startResponse = await client.PostAsync(
                    $"https://graph.facebook.com/v12.0/{pageId}/videos?upload_phase=start&file_size={videoFile.Length}&access_token={accessToken}",
                    null
                );

                var startContent = await startResponse.Content.ReadAsStringAsync();
                dynamic startJson = Newtonsoft.Json.JsonConvert.DeserializeObject(startContent);
                string uploadSessionId = startJson.upload_session_id;
                string videoId = startJson.video_id;
                string startOffset = startJson.start_offset;

                // Upload the video in chunks
                using (var videoStream = videoFile.OpenReadStream())
                {
                    byte[] buffer = new byte[1024 * 1024 * 4]; // 4MB chunk size
                    int bytesRead;
                    while ((bytesRead = await videoStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        using (var content = new MultipartFormDataContent())
                        {
                            content.Add(new StringContent("transfer"), "upload_phase");
                            content.Add(new StringContent(uploadSessionId), "upload_session_id");
                            content.Add(new StringContent(startOffset), "start_offset");
                            content.Add(new ByteArrayContent(buffer, 0, bytesRead), "video_file_chunk", videoFile.FileName);

                            var uploadResponse = await client.PostAsync(
                                $"https://graph.facebook.com/v12.0/{pageId}/videos?access_token={accessToken}",
                                content
                            );

                            var uploadContent = await uploadResponse.Content.ReadAsStringAsync();
                            dynamic uploadJson = Newtonsoft.Json.JsonConvert.DeserializeObject(uploadContent);
                            startOffset = uploadJson.start_offset;
                        }
                    }
                }

                // Finish the upload session
                var finishResponse = await client.PostAsync(
                    $"https://graph.facebook.com/v12.0/{pageId}/videos?upload_phase=finish&upload_session_id={uploadSessionId}&access_token={accessToken}",
                    null
                );

                if (!finishResponse.IsSuccessStatusCode)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to finish video upload.");
                }

                // Publish the video post
                var publishResponse = await client.PostAsync(
                    $"https://graph.facebook.com/v12.0/{pageId}/feed?message=Your+Video+Description&video_id={videoId}&access_token={accessToken}",
                    null
                );

                if (publishResponse.IsSuccessStatusCode)
                {
                    return Ok("Video posted successfully.");
                }
                else
                {
                    var publishContent = await publishResponse.Content.ReadAsStringAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to publish video: " + publishContent);
                }
            }
        }




    }
}
