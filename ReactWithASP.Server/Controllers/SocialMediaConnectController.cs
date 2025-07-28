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
using Microsoft.AspNetCore.Authentication.Twitter;
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
using System.Security.Cryptography;
using System.Text;
using ReactWithASP.Server.InterfaceServices;
using Org.BouncyCastle.Asn1.Ocsp;

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
        private readonly object client;
        private readonly ILinkedInService _linkedInService;
        private readonly IHttpContextAccessor _httpContextAccessor;


    public SocialMediaConnectController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment, ApplicationDbContext context, ILinkedInService linkedInService, IHttpContextAccessor httpContextAccessor)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
            Environment = environment;
            _context = context;
            _linkedInService = linkedInService;
      _httpContextAccessor = httpContextAccessor;

    }

    [HttpGet("FacebookLogin")]
        public IActionResult FacebookLogin(string userId, string SocialMediaId)
        
    {
          var request = _httpContextAccessor.HttpContext.Request;

            HttpContext.Session.SetString("FacebookUserId", userId);
            HttpContext.Session.SetString("FacebookSocialMediaId", SocialMediaId);
            var clientId = "483927064226359";
          var redirectUri = $"{request.Scheme}://{request.Host}/api/FacebookCallBack";

      //var redirectUri = $"https://assr.digitalnoticeboard.biz/api/FacebookCallBack";// Your callback URL
     // var redirectUri = $"https://assr.digitalnoticeboard.biz/api/FacebookCallBack";
           // var redirectUri = $"https://localhost:7189/api/FacebookCallBack";
            var state = Guid.NewGuid().ToString("N");
            var scope = "pages_manage_posts,pages_read_engagement"; // Facebook page permissions  
            var config_id = "871588441670167";
            var authorizationUrl = $"https://www.facebook.com/v17.0/dialog/oauth?client_id={clientId}&redirect_uri={Uri.EscapeUriString(redirectUri)}&state={state}&scope={scope}&config_id={config_id}";
            //string userId = userid;
            return Redirect(authorizationUrl);
        }
        [HttpGet("FacebookCallBack")]
        public async Task<IActionResult> FacebookCallback(string code, string state)
        {
      var request = _httpContextAccessor.HttpContext.Request;

      if (string.IsNullOrEmpty(code))
            {
                return BadRequest(new { error = "Authorization code is missing." });
            }

            var clientId = "483927064226359";
            var clientSecret = "66725e0d3573bbd8e47bcc92e9173892";
      var redirectUri = $"{request.Scheme}://{request.Host}/api/FacebookCallBack";

      // var redirectUri = $"https://assr.digitalnoticeboard.biz/api/FacebookCallBack";
      // var redirectUri = $"https://assr.digitalnoticeboard.biz/api/FacebookCallBack";
      // var redirectUri = $"https://localhost:7189/api/FacebookCallBack";
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
                var requestUri = QueryHelpers.AddQueryString(tokenRequestUrl, parameters);
                var response = await httpClient.GetAsync(requestUri);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest(new { error = "Error retrieving access token", details = responseString });
                }

                var tokenResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseString);
                if (!tokenResponse.TryGetValue("access_token", out var accessToken))
                {
                    return BadRequest(new { error = "Error parsing access token" });
                }

                var userInfoUrl = $"https://graph.facebook.com/me?fields=id,name,picture&access_token={accessToken}";
                var userResponseMessage = await httpClient.GetAsync(userInfoUrl);
                var userResponseString = await userResponseMessage.Content.ReadAsStringAsync();

                if (!userResponseMessage.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error retrieving Facebook user details: {userResponseString}");
                    return BadRequest(new { error = "Failed to retrieve Facebook user details" });
                }

                var jsonResponse = JObject.Parse(userResponseString);
                var userId = HttpContext.Session.GetString("FacebookUserId");
                var SocialMediaId = HttpContext.Session.GetString("FacebookSocialMediaId");

                var existingUser = _context.Users.FirstOrDefault(f => f.Id == userId);
                SocialMediaUsersModel FacebookModel = null;
                if (existingUser.Id == userId)
                {
                    FacebookModel = new SocialMediaUsersModel
                    {
                        UserId = userId,
                        AccessToken = accessToken,
                        SocialMedia = SocialMediaId,

                    };
                    _context.SocialMediaUser.Add(FacebookModel);
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
<script>
       if (window.opener) {
  // Send a message to the parent window (make sure the status is 'cancel' or other states as needed)
  window.opener.postMessage({
    status: 'cancel'
  }, 'https://assrweb.digitalnoticeboard.biz/'); // Send it only to the expected domain
}
    </script>
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

        [HttpGet("FacebookUserPages1")]
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
                        AccessToken = pageAccessToken,
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
            //var existingUser = await _context.FacebookUsers.FirstOrDefaultAsync(u => u.UserId == userId);
            var existingUser = await _context.SocialMediaUser.FirstOrDefaultAsync(u => u.UserId == userId);

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
                        UserId = userId,
                    },
                    Pages = formattedPages
                }
            });

            //return Ok(response);
        }
        [HttpPost("FacebookUpload")]
        public async Task<IActionResult> FacebookUpload(IFormFile videoFile, IFormFile imageFile, string pageId, string accessToken, string message)
        {
            //if (videoFile == null)
            //{
            //  return BadRequest("Video file is required.");
            //}

            using (var client = new HttpClient())
            {
                // Start the video upload session
                var startResponse = await client.PostAsync(
                    $"https://graph.facebook.com/v12.0/{pageId}/videos?upload_phase=start&file_size={videoFile.Length}&access_token={accessToken}",
                    null
                );

                if (!startResponse.IsSuccessStatusCode)
                {
                    var startError = await startResponse.Content.ReadAsStringAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to start video upload: " + startError);
                }

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

                            if (!uploadResponse.IsSuccessStatusCode)
                            {
                                var uploadError = await uploadResponse.Content.ReadAsStringAsync();
                                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to upload video chunk: " + uploadError);
                            }

                            var uploadContent = await uploadResponse.Content.ReadAsStringAsync();
                            dynamic uploadJson = Newtonsoft.Json.JsonConvert.DeserializeObject(uploadContent);
                            startOffset = uploadJson.start_offset;
                        }
                    }
                }

                // Finish the video upload session
                var finishResponse = await client.PostAsync(
                    $"https://graph.facebook.com/v12.0/{pageId}/videos?upload_phase=finish&upload_session_id={uploadSessionId}&access_token={accessToken}",
                    null
                );

                if (!finishResponse.IsSuccessStatusCode)
                {
                    var finishError = await finishResponse.Content.ReadAsStringAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to finish video upload: " + finishError);
                }

                // Publish the video post
                var publishResponse = await client.PostAsync(
                    $"https://graph.facebook.com/v12.0/{pageId}/feed?message={Uri.EscapeDataString(message)}&video_id={videoId}&access_token={accessToken}",
                    null
                );

                if (!publishResponse.IsSuccessStatusCode)
                {
                    var publishError = await publishResponse.Content.ReadAsStringAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to publish video: " + publishError);
                }

                // If an image is provided, upload it
                if (imageFile != null)
                {
                    using (var imageStream = imageFile.OpenReadStream())
                    {
                        // Convert the image stream to a byte array inline
                        byte[] imageBytes;
                        using (var ms = new MemoryStream())
                        {
                            imageStream.CopyTo(ms);
                            imageBytes = ms.ToArray();
                        }

                        var imageContent = new MultipartFormDataContent();
                        imageContent.Add(new ByteArrayContent(imageBytes), "source", imageFile.FileName);
                        imageContent.Add(new StringContent(message), "caption");

                        var imageUploadResponse = await client.PostAsync(
                            $"https://graph.facebook.com/v12.0/{pageId}/photos?access_token={accessToken}",
                            imageContent
                        );

                        if (!imageUploadResponse.IsSuccessStatusCode)
                        {
                            var imageErrorContent = await imageUploadResponse.Content.ReadAsStringAsync();
                            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to upload image: " + imageErrorContent);
                        }
                    }
                }

                return Ok("Video and image posted successfully.");
            }
        }
       

        [HttpGet("LinkedInLogin")]
        public IActionResult LinkedInLogin(string userId, string SocialMediaId)
        
         {
            try
            {
        var request = _httpContextAccessor.HttpContext.Request;
        HttpContext.Session.SetString("LinkedInUserId", userId);
                HttpContext.Session.SetString("LinkedInSocialMediaId", SocialMediaId);
        var clientId = "860d0h5icf09uj";
        var redirectUri = $"{request.Scheme}://{request.Host}/api/HandleRedirect";        
        var state = Guid.NewGuid().ToString();
        var scope = "openid profile email w_member_social";
        var authorizationUrl = $"https://www.linkedin.com/oauth/v2/authorization?response_type=code&client_id={clientId}&redirect_uri={Uri.EscapeDataString(redirectUri)}&state={state}&scope={Uri.EscapeDataString(scope)}";        
        return Redirect(authorizationUrl);
      }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    [HttpGet("HandleRedirect")]
    public async Task<IActionResult> HandleRedirect(string code, string state)
    {
      if (string.IsNullOrEmpty(code))
      {
        return BadRequest("Authorization code not found.");
      }
      string accessToken = await _linkedInService.GetAccessToken(code);
  

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
        <script>
            if (window.opener) {
                window.opener.postMessage({
                    status: 'cancel'
                }, 'https://assrweb.digitalnoticeboard.biz/');
            }
        </script>
    </body>
    </html>";

      return Content(htmlContent, "text/html");
    }
    

    [HttpPost("upload")]
        public async Task<IActionResult> UploadImageAndPost([FromHeader] string accessToken, [FromForm] IFormFile image, [FromForm] string postText)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("Image is required");
            }
            using (var ms = new MemoryStream())
            {
                await image.CopyToAsync(ms);
                var imageBytes = ms.ToArray();
                var assetId = await _linkedInService.UploadImageAsync(accessToken, imageBytes, image.FileName);

                await _linkedInService.CreatePostAsync(accessToken, assetId, postText);
            }
            return Ok("Post created successfully");
        }


    [HttpGet("twitter/login1")]
    public IActionResult TwitterLogin1()
    {
      var clientId = "cGdPRzRwcGtMR0hLTncxbUZvSjg6MTpjaQ";
      var redirectUri = "https://assr.digitalnoticeboard.biz/api/TwitterCallBack";
      var state = Guid.NewGuid().ToString(); // Used for CSRF protection

      var authUrl = $"https://twitter.com/i/oauth2/authorize" +
                    $"?response_type=code" +
                    $"&client_id={clientId}" +
                    $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                    $"&scope=tweet.read users.read offline.access" +
                    $"&state={state}" +
                    $"&code_challenge=challenge" +  // Replace with actual PKCE challenge
                    $"&code_challenge_method=plain";

      return Redirect(authUrl);
    }
    [HttpGet("twitter/callback1")]
    public async Task<IActionResult> TwitterCallback1([FromQuery] string code, [FromQuery] string state)
    {
      if (string.IsNullOrEmpty(code))
      {
        return BadRequest("Authorization code not found.");
      }

      var clientId = "cGdPRzRwcGtMR0hLTncxbUZvSjg6MTpjaQ";
      var clientSecret = "bwTRE4qiFUIxToJNw8786vSQEu5MgHEP8Oc7HO5RTtapKWNPfh";
      var redirectUri = "https://assr.digitalnoticeboard.biz/api/TwitterCallBack";

      using (var client = new HttpClient())
      {
        var tokenRequest = new Dictionary<string, string>
        {
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "code", code },
            { "grant_type", "authorization_code" },
            { "redirect_uri", redirectUri },
            { "code_verifier", "challenge" }  // Replace with actual PKCE verifier
        };

        var response = await client.PostAsync("https://api.twitter.com/2/oauth2/token", new FormUrlEncodedContent(tokenRequest));
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
          return BadRequest($"Failed to get access token: {responseContent}");
        }

        return Ok(responseContent); // Return access token and other details
      }
    }
    [HttpGet("twitter/userinfo")]
    public async Task<IActionResult> GetTwitterUserInfo([FromQuery] string accessToken)
    {
      using (var client = new HttpClient())
      {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await client.GetAsync("https://api.twitter.com/2/users/me");
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
          return BadRequest($"Failed to fetch user data: {responseContent}");
        }

        return Ok(responseContent);
      }
    }


  }
}
