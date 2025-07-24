using JWTAuthentication.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ReactWithASP.Server.InterfaceServices;
using ReactWithASP.Server.Models;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Tweetinvi;
using Tweetinvi.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ReactWithASP.Server.Controllers
{
  [ApiController]
  [Route("api")]
  public class SocialMediaConnectionController : Controller
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
    private readonly string? apiUrl;
    private readonly IHttpContextAccessor _httpContextAccessor;
        
    public SocialMediaConnectionController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment, ApplicationDbContext context, ILinkedInService linkedInService, IHttpContextAccessor httpContextAccessor)
    {
      this.userManager = userManager;
      this.roleManager = roleManager;
      _configuration = configuration;
      Environment = environment;
      _context = context;
      _linkedInService = linkedInService;
      _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet("FacebookLogin1")]
    public IActionResult FacebookLogin1(string userId, string SocialMediaId)
    {
       var request = _httpContextAccessor.HttpContext.Request;
      HttpContext.Session.SetString("FacebookUserId", userId);
      HttpContext.Session.SetString("FacebookSocialMediaId", SocialMediaId);
      var clientId = "962008238748108";
      //var redirectUri = $"{request.Scheme}://{request.Host}/api/FacebookCallBack";// Your callback URL
      var redirectUri = $"{request.Scheme}://{request.Host}/api/FacebookCallBack";
       //var redirectUri = $"https://localhost:7189/api/FacebookCallBack";
      var state = Guid.NewGuid().ToString("N");
      var scope = "pages_manage_posts,pages_read_engagement"; 
      var config_id = "947573997475934";
      var authorizationUrl = $"https://www.facebook.com/v17.0/dialog/oauth?client_id={clientId}&redirect_uri={Uri.EscapeUriString(redirectUri)}&state={state}&scope={scope}&config_id={config_id}";
      //string userId = userid;
      return Redirect(authorizationUrl);
    }
    [HttpGet("FacebookCallBack1")]
    public async Task<IActionResult> FacebookCallback1(string code, string state)
    {
      var request = _httpContextAccessor.HttpContext.Request;

      if (string.IsNullOrEmpty(code))
      {
        return BadRequest(new { error = "Authorization code is missing." });
      }

      var clientId = "947573997475934";
      var clientSecret = "1b9fa4dc16562e0afbe4fa99cc7d1a5e";
      var redirectUri = $"{request.Scheme}://{request.Host}/api/FacebookCallBack";

      //var redirectUri = $"https://assr.digitalnoticeboard.biz/api/FacebookCallBack";
      //var redirectUri = $"https://assr.digitalnoticeboard.biz/api/FacebookCallBack";
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

    [HttpGet("TwitterLogin")]
    public IActionResult TwitterLogin(string userId, string SocialMediaId)
    {
      var request = _httpContextAccessor.HttpContext.Request;
      HttpContext.Session.SetString("TwitterUserId", userId);
      HttpContext.Session.SetString("TwitterSocialMediaId", SocialMediaId);
      var clientId = "TldoejVJNWRNNkFiVGtia21QWmQ6MTpjaQ";
      //var redirectUri = "https://assr.digitalnoticeboard.biz/api/TwitterCallBack";
      var state = Guid.NewGuid().ToString(); // Used for CSRF protection
      var redirectUri = $"{request.Scheme}://{request.Host}/api/TwitterCallBack";
      // Generate Code Verifier (Plain text)
      var codeVerifier = GenerateCodeVerifier();

      // Generate Code Challenge (SHA256 hash of the code verifier)
      var codeChallenge = GenerateCodeChallenge(codeVerifier);

      HttpContext.Session.SetString("Twitter_CodeVerifier", codeVerifier);

      var authUrl = $"https://twitter.com/i/oauth2/authorize" +
                    $"?response_type=code" +
                    $"&client_id={clientId}" +
                    $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                    $"&scope=tweet.read tweet.write users.read offline.access" +
                    $"&state={state}" +
                    $"&code_challenge={codeChallenge}" +
                    $"&code_challenge_method=S256";

      return Redirect(authUrl);
    }
    [HttpGet("TwitterCallBack")]
    public async Task<IActionResult> TwitterCallBack([FromQuery] string code, [FromQuery] string state)
    {
      if (string.IsNullOrEmpty(code))
      {
        return BadRequest("Authorization code not found.");
      }
      var request = _httpContextAccessor.HttpContext.Request;
      var clientId = "TldoejVJNWRNNkFiVGtia21QWmQ6MTpjaQ";
      var clientSecret = "XHxN3ekZjGJO2TP--qSE7tHGuEVpDvCAupkIeXt11uJUCKjy61";
      //var redirectUri = "https://assr.digitalnoticeboard.biz/api/TwitterCallBack";
      var redirectUri = $"{request.Scheme}://{request.Host}/api/TwitterCallBack";
      var codeVerifier = HttpContext.Session.GetString("Twitter_CodeVerifier");
      if (string.IsNullOrEmpty(codeVerifier))
      {
        return BadRequest("Code Verifier not found.");
      }

      using (var client = new HttpClient())
      {
        var tokenRequest = new Dictionary<string, string>
        {
            { "client_id", clientId },
            //{ "client_secret", clientSecret },
            { "code", code },
            { "grant_type", "authorization_code" },
            { "redirect_uri", redirectUri },
            { "code_verifier", codeVerifier  }  // Replace with actual PKCE verifier
        };

        // Twitter requires Base64 encoded "client_id:client_secret" for authentication
        var authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);

        var response = await client.PostAsync("https://api.twitter.com/2/oauth2/token", new FormUrlEncodedContent(tokenRequest));
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
          return BadRequest($"Failed to get access token: {responseContent}");
        }

        string accessToken;
        using (JsonDocument doc = JsonDocument.Parse(responseContent))
        {
          if (!doc.RootElement.TryGetProperty("access_token", out JsonElement accessTokenElement))
          {
            return BadRequest("Access token not found in response.");
          }

          accessToken = accessTokenElement.GetString();
        }

        // Get session data
        var userId = HttpContext.Session.GetString("TwitterUserId");
        var socialMediaId = HttpContext.Session.GetString("TwitterSocialMediaId");

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(socialMediaId))
        {
          return BadRequest("Session data is missing.");
        }

        // Save or update token in database
        var existingRecord = await _context.SocialMediaUser
            .FirstOrDefaultAsync(s => s.UserId == userId && s.SocialMedia == socialMediaId);

        if (existingRecord == null)
        {
          var newRecord = new SocialMediaUsersModel
          {
            UserId = userId,
            SocialMedia = socialMediaId,
            AccessToken = accessToken
          };

          _context.SocialMediaUser.Add(newRecord);
        }
        else
        {
          existingRecord.AccessToken = accessToken;
          _context.SocialMediaUser.Update(existingRecord);
        }

        await _context.SaveChangesAsync();

        // Access token successfully received
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
        //return Ok(responseContent); // Return access token and other details
      }
    }

    [HttpGet("GetTwitterUserInfodata")]
    public async Task<IActionResult> GetTwitterUserInfodata(string accessToken)
    {
      /*var accessToken = "1883277003262144512-U8q7H0elKO7ylgiFpdym9KJqEE3c2P"; */// Replace with your stored access token
      //var accesstokensecret = "fhoWvXBnr7KQmZ6sxwptR13dfFfleLZ0vm3FMconMHmsR";
      using (var client = new HttpClient())
      {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await client.GetAsync("https://api.twitter.com/2/users/me");
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
          return BadRequest($"Failed to fetch user info: {content}");
        }

        return Ok(content);
      }
    }
    // Function to generate a secure Code Verifier
    private static string GenerateCodeVerifier()
    {
      var bytes = new byte[32];
      using (var rng = new RNGCryptoServiceProvider())
      {
        rng.GetBytes(bytes);
      }
      return Convert.ToBase64String(bytes)
          .TrimEnd('=').Replace('+', '-').Replace('/', '_'); // Make it URL-safe
    }

    // Function to generate a SHA256 Code Challenge from the Code Verifier
    private static string GenerateCodeChallenge(string codeVerifier)
    {
      using (var sha256 = SHA256.Create())
      {
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
        return Convert.ToBase64String(bytes)
            .TrimEnd('=').Replace('+', '-').Replace('/', '_'); // Make it URL-safe
      }
    }

    [HttpPost("UploadMediaToTwitter")]
    public async Task<IActionResult> UploadMediaToTwitter(IFormFile mediaFile, string accessToken, string accessTokenSecret)
    {
      if (mediaFile == null || mediaFile.Length == 0)
        return BadRequest("Invalid media file.");

      var mediaCategory = mediaFile.ContentType.StartsWith("video") ? "tweet_video" : "tweet_image";

      using (var client = new HttpClient())
      {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        // Step 1: Initialize Upload
        var initParams = new Dictionary<string, string>
        {
            { "command", "INIT" },
            { "total_bytes", mediaFile.Length.ToString() },
            { "media_type", mediaFile.ContentType },
            { "media_category", mediaCategory }
        };

        var initResponse = await client.PostAsync("https://upload.twitter.com/1.1/media/upload.json",
            new FormUrlEncodedContent(initParams));
        var initContent = await initResponse.Content.ReadAsStringAsync();
        if (!initResponse.IsSuccessStatusCode)
          return BadRequest($"INIT failed: {initContent}");

        var mediaId = JObject.Parse(initContent)["media_id_string"].ToString();

        // Step 2: Upload Media (Single Chunk)
        using (var fileStream = mediaFile.OpenReadStream())
        {
          var fileBytes = new byte[mediaFile.Length];
          fileStream.Read(fileBytes, 0, fileBytes.Length);

          var mediaContent = new MultipartFormDataContent
            {
                { new ByteArrayContent(fileBytes), "media", mediaFile.FileName }
            };

          var uploadResponse = await client.PostAsync($"https://upload.twitter.com/1.1/media/upload.json?command=APPEND&media_id={mediaId}&segment_index=0", mediaContent);
          if (!uploadResponse.IsSuccessStatusCode)
            return BadRequest("Failed to upload media.");
        }

        // Step 3: Finalize Upload
        var finalizeParams = new Dictionary<string, string>
        {
            { "command", "FINALIZE" },
            { "media_id", mediaId }
        };

        var finalizeResponse = await client.PostAsync("https://upload.twitter.com/1.1/media/upload.json",
            new FormUrlEncodedContent(finalizeParams));
        var finalizeContent = await finalizeResponse.Content.ReadAsStringAsync();
        if (!finalizeResponse.IsSuccessStatusCode)
          return BadRequest($"FINALIZE failed: {finalizeContent}");

        return Ok(new { mediaId });
      }
    }
    [HttpPost("PostTweet")]
    public async Task<IActionResult> PostTweet(PostTweetReqestDto newTweet)
    {
      var client = new TwitterClient("kBStK36rPiO3VInEk9abJFO9s", "VsmgszGSHrXvciyuzP1BVP7cWEe2PCEDJfWB59c4xElnSU5h7s", "1883277003262144512-hhtltesdJzWcKlHtSeiG36nPfZ6Kr4", "0TvdABBkBvJ6TaSroSMAc1SuaYenBubkKHyf9zIhz8Gry");

      var result = await client.Execute.AdvanceRequestAsync(
        BuildTwitterRequest(newTweet, client));
      return Ok(result.Content);
    }


    private static Action<ITwitterRequest> BuildTwitterRequest(
      PostTweetReqestDto newTweet, TwitterClient client)
    {
      return (ITwitterRequest request) =>
      {
        var jsonBody = client.Json.Serialize(newTweet);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        request.Query.Url = "https://api.twitter.com/2/tweets";
        request.Query.HttpMethod = Tweetinvi.Models.HttpMethod.POST;
        request.Query.HttpContent = content;
      };
    }

    [HttpPost("UploadMedia")]
    public async Task<IActionResult> UploadMedia(IFormFile mediaFile, string accessToken)
    {
      if (mediaFile == null || mediaFile.Length == 0)
        return BadRequest("Invalid media file.");

      using (var client = new HttpClient())
      {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using (var content = new MultipartFormDataContent())
        {
          var fileContent = new StreamContent(mediaFile.OpenReadStream());
          fileContent.Headers.ContentType = new MediaTypeHeaderValue(mediaFile.ContentType);
          content.Add(fileContent, "media", mediaFile.FileName);
          content.Add(new StringContent("tweet_image"), "media_category");

          var response = await client.PostAsync("https://upload.twitter.com/1.1/media/upload.json", content);
          var responseContent = await response.Content.ReadAsStringAsync();

          if (!response.IsSuccessStatusCode)
            return BadRequest($"Failed to upload media: {responseContent}");

          return Ok(responseContent); // Returns media_id_string
        }
      }
    }
    [HttpPost("twitter/init-video-upload")]
    public async Task<IActionResult> InitVideoUpload(long mediaSize, string mediaType, string accessToken)
    {
      using (var client = new HttpClient())
      {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var requestData = new Dictionary<string, string>
        {
            { "command", "INIT" },
            { "total_bytes", mediaSize.ToString() },
            { "media_type", mediaType },
            { "media_category", "tweet_video" }
        };

        var response = await client.PostAsync("https://upload.twitter.com/1.1/media/upload.json", new FormUrlEncodedContent(requestData));
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
          return BadRequest($"Failed to initialize upload: {responseContent}");

        return Ok(responseContent);
      }
    }
    [HttpPost("twitter/upload-video-chunk")]
    public async Task<IActionResult> UploadVideoChunk(IFormFile chunk, string mediaId, int segmentIndex, string accessToken)
    {
      using (var client = new HttpClient())
      {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using (var content = new MultipartFormDataContent())
        {
          var fileContent = new StreamContent(chunk.OpenReadStream());
          content.Add(fileContent, "media", chunk.FileName);
          content.Add(new StringContent("APPEND"), "command");
          content.Add(new StringContent(mediaId), "media_id");
          content.Add(new StringContent(segmentIndex.ToString()), "segment_index");

          var response = await client.PostAsync("https://upload.twitter.com/1.1/media/upload.json", content);
          var responseContent = await response.Content.ReadAsStringAsync();

          if (!response.IsSuccessStatusCode)
            return BadRequest($"Failed to upload chunk: {responseContent}");

          return Ok("Chunk uploaded successfully.");
        }
      }
    }
    [HttpPost("twitter/finalize-video-upload")]
    public async Task<IActionResult> FinalizeVideoUpload(string mediaId, string accessToken)
    {
      using (var client = new HttpClient())
      {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var requestData = new Dictionary<string, string>
        {
            { "command", "FINALIZE" },
            { "media_id", mediaId }
        };

        var response = await client.PostAsync("https://upload.twitter.com/1.1/media/upload.json", new FormUrlEncodedContent(requestData));
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
          return BadRequest($"Failed to finalize upload: {responseContent}");

        return Ok(responseContent);
      }
    }

    [HttpPost("twitter/post-media-tweet")]
    public async Task<IActionResult> PostTweetWithMedia([FromBody] string tweetText, [FromQuery] string mediaId, string accessToken)
    {
      using (var client = new HttpClient())
      {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var requestData = new
        {
          text = tweetText,
          media = new { media_ids = new[] { mediaId } }
        };
        var json = JsonConvert.SerializeObject(requestData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("https://api.twitter.com/2/tweets", content);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
          return BadRequest($"Failed to post tweet: {responseContent}");

        return Ok(responseContent);
      }
    }
  }
}
