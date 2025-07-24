using FFMpegCore;
using JWTAuthentication.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using PayPal;
using Quartz;
using ReactWithASP.Server.Models;
using ReactWithASP.Server.Models.Posts;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Mime;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Xabe.FFmpeg;


namespace ReactWithASP.Server.Controllers
{
  [ApiController]
  [Route("api")]
  public class StoryController : Controller
  {

    private readonly UserManager<ApplicationUser> userManager;
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly IConfiguration _configuration;
    private Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment;
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly HttpClient _httpClient;
    private const string FacebookApiBaseUrl = "https://graph.facebook.com/v16.0/";
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public StoryController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, HttpClient httpClient, IHttpClientFactory httpClientFactory, ISchedulerFactory schedulerFactory, IHttpContextAccessor httpContextAccessor)
    {
      this.userManager = userManager;
      this.roleManager = roleManager;
      _configuration = configuration;
      Environment = environment;
      _context = context;
      _webHostEnvironment = webHostEnvironment;
      _httpClient = httpClient;
      _httpClientFactory = httpClientFactory;
      _schedulerFactory = schedulerFactory;
      _httpContextAccessor = httpContextAccessor;
    }


    [HttpGet("Story")]
    //[Authorize]
    public IActionResult UserStory([FromQuery] HomeScreenRequest request)
    {
      var query = (from smp in _context.PostedStory
                   join ugp in _context.UserGroupPosts on smp.Id equals ugp.StoryId
                   join sm in _context.SocialMedia on ugp.AccountID equals sm.Id.ToString() into smJoin
                   from sm in smJoin.DefaultIfEmpty()
                   join g in _context.@group on ugp.GroupId equals g.Id into gJoin
      from g in gJoin.DefaultIfEmpty()
                   where smp.UserGuid == request.UserGuid
                         && (ugp.GroupId > 0 || ugp.AccountID != null)
                         //&& (request.Date == null || smp.CreatedAt.Value.Date == request.Date.Value.Date)
                   orderby smp.Id descending
                   select new
                   {
                     smp.PostIcon,
                     smp.CreatedAt,
                     smp.Id,
                     smp.Status,
                     GroupId = g != null ? g.Id : 0,
                     smp.UserGuid,
                     GroupName = g != null ? g.Name : null,
                     GroupIcon = g != null ? g.GroupIcon : null,
                     SocialMediaName = sm != null ? sm.SocialMediaName : null
                   }).ToList();
      if (query.Count == 0)
      {
        return Ok(new { Status = "true", data = "No data found" });
      }

      var response = query
.GroupBy(q => new { q.Id, q.GroupId })
.Select(g => new StoryResponse
{
Id = g.First().Id,
PostIcon = ParsePostIcon(g.First().PostIcon)
.Select(fileName => GenerateServerPathUrl(fileName))
.FirstOrDefault(),
Status = g.First().Status,
Group = g.First().GroupId > 0
? new Groupes
{
Id = g.First().GroupId,
Name = g.First().GroupName,
UserGuid = g.First().UserGuid,
GroupIcon = g.First().GroupIcon,
Platform = g.Select(x => x.SocialMediaName).Distinct().ToArray(),
}
: null,
Account = g.First().GroupId == 0
? new AccountResponses
{
SocialMediaName = g.Select(x => x.SocialMediaName).Distinct().ToArray(),
UserGuid = g.First().UserGuid
}
: null
}).ToList();
      //var response = new List<StoryResponse>
      //      {
      //          new StoryResponse
      //          {
      //              Group = new Groups
      //               {
      //                   Name = "Musemind Design",
      //                   /*Platform = new List<string> { "facebook", "instagram" },*/
      //                   groupIcon = "https://thumbs.dreamstime.com/b/teamwork-group-friends-icon-vector-illustration-teamwork-group-friends-icon-118637039.jpg"
      //               },
      //              StoryTime = "10:30:00",
      //              PostIcon = "https://www.shutterstock.com/image-photo/earth-day-environment-green-globe-260nw-2285503199.jpg",
      //              Id = 1,
      //              Likes = 1000,
      //              Views = 2000,
      //              Shares = 200,
      //              Status = "Story Published",
      //              StatusCode = 1
      //          },
      //          //new StoryResponse
      //          //{
      //          //    Group = new Groups
      //          //    {
      //          //        Name = "Musemind Design",
      //          //        /*Platform = new List<string> { "facebook", "instagram" },*/
      //          //        groupIcon = "https://thumbs.dreamstime.com/b/teamwork-group-friends-icon-vector-illustration-teamwork-group-friends-icon-118637039.jpg"
      //          //    },
      //          //    StoryTime = "05:30:00",
      //          //    PostIcon = "https://www.shutterstock.com/image-photo/earth-day-environment-green-globe-260nw-2285503199.jpg",
      //          //    Id = 2,
      //          //    Likes = 1000,
      //          //    Views = 2000,
      //          //    Shares = 200,
      //          //    Status = "Story Published",
      //          //    StatusCode = 1
      //          //}
      //      };
      return Ok(response);
    }

    //[HttpPost("PostStory")]
    ////[Authorize]
    //public IActionResult PostStory([FromBody] PostStoryRequest request)
    //{
    //  if (string.IsNullOrEmpty(request.userGUId) ||
    //     request.mediaUrl == null || request.mediaUrl.Count == 0 ||
    //      string.IsNullOrEmpty(request.accountOrGroupName) ||
    //      request.accountOrGroupId == null || request.accountOrGroupId.Count == 0)
    //  {
    //    return BadRequest(new PostStoryResponse
    //    {
    //      Message = "Fields Missing"
    //    });
    //  }






    //  return Ok(new PostStoryResponse
    //  {
    //    Message = "Story Posted successfully"
    //  });
    //}




    private async Task<FacebookUploadResult> UploadPhotoStory(string pageId, string pageAccessToken, string filePath)
    {
      try
      {
        using (var httpClient = new HttpClient())
        {
          // Step 1: Upload the photo
          var photoUploadUrl = $"https://graph.facebook.com/v21.0/{pageId}/photos";

          using (var photoForm = new MultipartFormDataContent())
          {
            var fileContent = new ByteArrayContent(await System.IO.File.ReadAllBytesAsync(filePath));
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            photoForm.Add(fileContent, "source", Path.GetFileName(filePath));
            photoForm.Add(new StringContent(pageAccessToken), "access_token");
            photoForm.Add(new StringContent("false"), "published"); // Ensure photo is not published immediately

            var photoUploadResponse = await httpClient.PostAsync(photoUploadUrl, photoForm);

            if (!photoUploadResponse.IsSuccessStatusCode)
            {
              var error = await photoUploadResponse.Content.ReadAsStringAsync();
              return new FacebookUploadResult { IsSuccess = false, ErrorMessage = $"Photo upload failed: {error}" };
            }

            var photoUploadContent = await photoUploadResponse.Content.ReadAsStringAsync();
            var photoUploadResult = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(photoUploadContent, new JsonSerializerOptions
            {
              PropertyNameCaseInsensitive = true
            });

            if (photoUploadResult == null || !photoUploadResult.ContainsKey("id"))
            {
              return new FacebookUploadResult { IsSuccess = false, ErrorMessage = "Failed to retrieve photo ID." };
            }

            var photoId = photoUploadResult["id"];

            // Step 2: Publish the photo story
            var storyUrl = $"https://graph.facebook.com/v21.0/{pageId}/photo_stories";

            var storyResponse = await httpClient.PostAsync(storyUrl, new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "photo_id", photoId },
                    { "access_token", pageAccessToken }
                }));

            if (!storyResponse.IsSuccessStatusCode)
            {
              var error = await storyResponse.Content.ReadAsStringAsync();
              return new FacebookUploadResult { IsSuccess = false, ErrorMessage = $"Story creation failed: {error}" };
            }

            var storyContent = await storyResponse.Content.ReadAsStringAsync();

            // Deserialize as a JsonDocument to work directly with JSON elements
            using var jsonDoc = System.Text.Json.JsonDocument.Parse(storyContent);
            var root = jsonDoc.RootElement;

            // Safely extract the "success" field
            if (root.TryGetProperty("success", out JsonElement successElement) &&
                successElement.ValueKind == JsonValueKind.True)
            {
              // Safely extract the "post_id" if it exists
              var postId = root.TryGetProperty("post_id", out JsonElement postIdElement)
                  ? postIdElement.GetString()
                  : null;

              return new FacebookUploadResult
              {
                IsSuccess = true,
                PostId = postId,
                Message = "Photo story successfully published!"
              };
            }
            else
            {
              return new FacebookUploadResult
              {
                IsSuccess = false,
                ErrorMessage = "Story publishing failed: Success key is missing or false."
              };
            }


          }
        }
      }
      catch (Exception ex)
      {
        return new FacebookUploadResult { IsSuccess = false, ErrorMessage = $"Exception occurred: {ex.Message}" };
      }
    }

    private List<ConnectedSocialMediaInfo> GetConnectedSocialMediaInfo(PostStoryRequest request)
    {
      if (string.Equals(request.accountOrGroupName, "Accounts", StringComparison.OrdinalIgnoreCase))
      {
        return _context.ConnectedSocialMediaInfo
            .Where(x => x.UserId == request.userGUId && request.accountOrGroupId.Contains(x.SocialMediaAccId.ToString()))
            .ToList();
      }
      else if (string.Equals(request.accountOrGroupName, "Groups", StringComparison.OrdinalIgnoreCase))
      {
        return (from gsm in _context.GroupSocialMedia
                join csi in _context.ConnectedSocialMediaInfo on gsm.SocialMediaId equals csi.SocialMediaAccId
                where request.accountOrGroupId.Contains(gsm.GroupId.ToString())
                select csi).ToList();
      }

      return new List<ConnectedSocialMediaInfo>();
    }

    public class FacebookUploadResult
    {
      public bool IsSuccess { get; set; }
      public string PostId { get; set; }
      public string Message { get; set; }
      public string ErrorMessage { get; set; }
    }


    private IFormFile ConvertPathToIFormFile(string filePath)
    {
      var fileName = Path.GetFileName(filePath);
      var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

      // Use a default content type or determine it based on the file extension
      string contentType = "application/octet-stream";

      return new FormFile(fileStream, 0, fileStream.Length, "file", fileName)
      {
        Headers = new HeaderDictionary(),
        ContentType = contentType
      };
    }


    [HttpPost("UploadAndPublishStory")]
    public async Task<IActionResult> UploadAndPublishStory([FromBody] PhotoUploadAndPublishRequest request)
    {
      if (string.IsNullOrWhiteSpace(request.AccessToken) || string.IsNullOrWhiteSpace(request.Url) || string.IsNullOrWhiteSpace(request.PageId))
      {
        return BadRequest(new { success = false, message = "AccessToken, URL, and PageId are required." });
      }

      // Step 1: Upload the photo
      var facebookApiUrl = $"https://graph.facebook.com/me/photos?access_token={request.AccessToken}";
      var content = new MultipartFormDataContent
      {
          { new StringContent(request.Url), "url" },
          { new StringContent("false"), "published" } // Photo will not be published immediately
      };

      try
      {
        var uploadResponse = await _httpClient.PostAsync(facebookApiUrl, content);

        if (!uploadResponse.IsSuccessStatusCode)
        {
          var errorResponse = await uploadResponse.Content.ReadAsStringAsync();
          return StatusCode((int)uploadResponse.StatusCode, new { success = false, message = "Facebook API error during photo upload", details = errorResponse });
        }

        // Step 2: Get the photo ID from the upload response
        var uploadResponseData = await uploadResponse.Content.ReadFromJsonAsync<FacebookPhotoResponse>();
        if (uploadResponseData == null || string.IsNullOrWhiteSpace(uploadResponseData.Id))
        {
          return StatusCode(500, new { success = false, message = "Photo ID not found in Facebook upload response." });
        }

        // Step 3: Publish the story
        var publishApiUrl = $"https://graph.facebook.com/v21.0/{request.PageId}/photo_stories";
        var publishContent = new MultipartFormDataContent
        {
            { new StringContent(request.AccessToken), "access_token" },
            { new StringContent(uploadResponseData.Id), "photo_id" } // Use the uploaded photo ID
        };

        var publishResponse = await _httpClient.PostAsync(publishApiUrl, publishContent);

        if (!publishResponse.IsSuccessStatusCode)
        {
          var errorResponse = await publishResponse.Content.ReadAsStringAsync();
          return StatusCode((int)publishResponse.StatusCode, new { success = false, message = "Facebook API error during story publication", details = errorResponse });
        }

        var publishResponseContent = await publishResponse.Content.ReadAsStringAsync();
        var publishResponseData = System.Text.Json.JsonSerializer.Deserialize<FacebookStoryResponse>(publishResponseContent, new JsonSerializerOptions
        {
          PropertyNameCaseInsensitive = true
        });

        if (publishResponseData != null && !string.IsNullOrWhiteSpace(publishResponseData.PostId))
        {
          return Ok(new { success = true, post_id = publishResponseData.PostId });
        }

        return StatusCode(500, new { success = false, message = "Post ID not found in Facebook story publication response." });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { success = false, message = "Internal server error", details = ex.Message });
      }
    }
    [HttpPost("UploadVideoStory")]
    public async Task<IActionResult> UploadVideoStory([FromForm] VideoStoryRequest request)
    {
      if (string.IsNullOrWhiteSpace(request.AccessToken) || string.IsNullOrWhiteSpace(request.PageId))
      {
        return BadRequest(new { success = false, message = "AccessToken and PageId are required." });
      }

      try
      {
        // Step 1: Start the video upload phase
        var startUploadUrl = $"https://graph.facebook.com/v21.0/{request.PageId}/video_stories?access_token={request.AccessToken}";
        var startUploadContent = new MultipartFormDataContent
        {
            { new StringContent("start"), "upload_phase" }
        };

        var startUploadResponse = await _httpClient.PostAsync(startUploadUrl, startUploadContent);

        if (!startUploadResponse.IsSuccessStatusCode)
        {
          var errorResponse = await startUploadResponse.Content.ReadAsStringAsync();
          return StatusCode((int)startUploadResponse.StatusCode, new { success = false, message = "Failed to initiate video upload", details = errorResponse });
        }

        var startUploadData = await startUploadResponse.Content.ReadFromJsonAsync<VideoUploadInitResponse>();
        if (startUploadData == null || string.IsNullOrWhiteSpace(startUploadData.UploadUrl) || string.IsNullOrWhiteSpace(startUploadData.VideoId))
        {
          return StatusCode(500, new { success = false, message = "Invalid response from Facebook during video upload initialization." });
        }
               

        return Ok(new { success = true, video_id = startUploadData.VideoId, UploadUrl = startUploadData.UploadUrl });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { success = false, message = "Internal server error", details = ex.Message });
      }
    }


    [HttpPost("post-to-feed")]
    public async Task<IActionResult> PostToFacebookFeed([FromBody] FacebookFeedRequest request)
    {
      // Endpoint URL for Facebook Graph API
      string url = "https://graph.facebook.com/me/feed";

      // Prepare the attached_media array
      var attachedMedia = new List<Dictionary<string, string>>();
      foreach (var mediaFbid in request.MediaFbIds)
      {
        attachedMedia.Add(new Dictionary<string, string> { { "media_fbid", mediaFbid } });
      }

      // Prepare the payload
      var payload = new
      {
        message = request.Message,
        attached_media = attachedMedia,
        access_token = request.AccessToken
      };

      // Serialize the payload to JSON
      var jsonPayload = System.Text.Json.JsonSerializer.Serialize(payload);
      var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

      // Send POST request
      try
      {
        var response = await _httpClient.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
          var responseData = await response.Content.ReadAsStringAsync();
          return Ok(new { success = true, data = responseData });
        }
        else
        {
          var errorData = await response.Content.ReadAsStringAsync();
          return StatusCode((int)response.StatusCode, new { success = false, error = errorData });
        }
      }
      catch (HttpRequestException ex)
      {
        return StatusCode(500, new { success = false, error = ex.Message });
      }
    }
    [HttpPost("post-feed")]
    public async Task<IActionResult> PostFeed([FromBody] FacebookPostRequest request)
    {
      if (string.IsNullOrEmpty(request.PageId) || string.IsNullOrEmpty(request.Message))
      {
        return BadRequest("PageId and Message are required.");
      }

      var accessToken = request.accessToken; // Replace with your page access token

      var postUrl = $"{FacebookApiBaseUrl}{request.PageId}/feed";
      var postData = new
      {
        message = request.Message,
        access_token = accessToken
      };

      var httpClient = _httpClientFactory.CreateClient();
      var response = await httpClient.PostAsync(postUrl, new StringContent(System.Text.Json.JsonSerializer.Serialize(postData), Encoding.UTF8, "application/json"));

      if (response.IsSuccessStatusCode)
      {
        var responseContent = await response.Content.ReadAsStringAsync();
        return Ok(new { success = true, response = System.Text.Json.JsonSerializer.Deserialize<object>(responseContent) });
      }
      else
      {
        var errorContent = await response.Content.ReadAsStringAsync();
        return StatusCode((int)response.StatusCode, new { success = false, error = System.Text.Json.JsonSerializer.Deserialize<object>(errorContent) });
      }
    }
    [HttpPost("post-multiple-photos-feed")]
    public async Task<IActionResult> PostMultiplePhotosFeed([FromBody] FacebookPostRequest request)
    {
      if (string.IsNullOrEmpty(request.PageId) || string.IsNullOrEmpty(request.Message) || request.MediaUrls == null || !request.MediaUrls.Any())
      {
        return BadRequest(new { Message = "PageId, Message, and at least one MediaUrl are required." });
      }

      var accessToken = request.accessToken; // Use your page access token here
      var baseApiUrl = "https://graph.facebook.com/v12.0/";

      var httpClient = _httpClientFactory.CreateClient();
      var uploadedPhotoIds = new List<string>();

      foreach (var mediaUrl in request.MediaUrls)
      {
        try
        {

          var mediaResponse = await httpClient.GetAsync(mediaUrl);
          if (!mediaResponse.IsSuccessStatusCode)
          {
            Console.WriteLine($"Failed to download media: {mediaUrl}");
            continue;
          }

          var mediaBytes = await mediaResponse.Content.ReadAsByteArrayAsync();
          var fileName = Path.GetFileName(mediaUrl); // Customize this if needed

          var uploadRequestContent = new MultipartFormDataContent
        {
            { new StringContent(accessToken), "access_token" },
            { new ByteArrayContent(mediaBytes), "source", fileName }
        };

          
          //// Prepare the photo upload request
          //var mediaContent = new MultipartFormDataContent();
          //mediaContent.Add(new StringContent(accessToken), "access_token");
          //mediaContent.Add(new StringContent(mediaUrl), "url");

          // Upload photo to Facebook
          var photoUploadResponse = await httpClient.PostAsync($"{baseApiUrl}{request.PageId}/photos", uploadRequestContent);

          if (!photoUploadResponse.IsSuccessStatusCode)
          {
            var errorResponse = await photoUploadResponse.Content.ReadAsStringAsync();
            return StatusCode((int)photoUploadResponse.StatusCode, new { Message = "Failed to upload photo.", Details = errorResponse });
          }

          var photoResponseContent = await photoUploadResponse.Content.ReadAsStringAsync();
          var photoResponse = System.Text.Json.JsonSerializer.Deserialize<FacebookPhotosResponse>(photoResponseContent);
          if (photoResponse != null && !string.IsNullOrEmpty(photoResponse.id))
          {
            uploadedPhotoIds.Add(photoResponse.id);
          }
        }
        catch (Exception ex)
        {
          return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error uploading photo.", Details = ex.Message });
        }
      }

      if (!uploadedPhotoIds.Any())
      {
        return BadRequest(new { Message = "No photos were successfully uploaded." });
      }

      // Prepare feed post with attached photos
      var feedData = new
      {
        message = request.Message,
        attached_media = uploadedPhotoIds.Select(id => new { media_fbid = id }),
        access_token = accessToken
      };

      try
      {
        var feedPostResponse = await httpClient.PostAsync(
            $"{baseApiUrl}{request.PageId}/feed",
            new StringContent(System.Text.Json.JsonSerializer.Serialize(feedData), Encoding.UTF8, "application/json")
        );

        if (!feedPostResponse.IsSuccessStatusCode)
        {
          var feedErrorContent = await feedPostResponse.Content.ReadAsStringAsync();
          return StatusCode((int)feedPostResponse.StatusCode, new { Message = "Failed to post feed.", Details = feedErrorContent });
        }

        var feedPostContent = await feedPostResponse.Content.ReadAsStringAsync();
        return Ok(new { Message = "Feed posted successfully.", Response = System.Text.Json.JsonSerializer.Deserialize<object>(feedPostContent) });
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error posting feed.", Details = ex.Message });
      }
    }
    [HttpPost("PostStory")]
    public async Task<IActionResult> CreateStoryPost(CreateStoryRequest request)
    {
      if (string.IsNullOrEmpty(request.userGUId) ||
          string.IsNullOrEmpty(request.MediaUrl) ||
          string.IsNullOrEmpty(request.AccountOrGroupName) ||
          request.AccountOrGroupId == null || request.AccountOrGroupId.Count == 0)
      {
        return BadRequest(new { Message = "Fields Missing" });
      }
      List<string> groupIds = request.AccountOrGroupName == "Groups"
? request.AccountOrGroupId.Select(a => !string.IsNullOrEmpty(a.Id) ? a.Id : "").ToList()
: new List<string>();

      List<string> accountIds = request.AccountOrGroupName == "Accounts"
? request.AccountOrGroupId.Select(a => !string.IsNullOrEmpty(a.accountId) ? a.accountId : "").ToList()
: new List<string>();

      List<string> accountpageId = request.AccountOrGroupName == "Accounts"
? request.AccountOrGroupId.Select(a => !string.IsNullOrEmpty(a.PageId) ? a.PageId : "").ToList()
: new List<string>();

      var groupPageIds = await (from gsm in _context.GroupSocialMedia
                                join g in _context.@group on gsm.GroupId equals g.Id
                                where g.UserGuid == request.userGUId
                                      && groupIds.Contains(gsm.GroupId.ToString())
                                select gsm.PageId)
                            .ToListAsync();

      var newstory = new PostedStory
      {
        UserGuid = request.userGUId,
        //CreatedAt = DateTime.UtcNow,
        CreatedAt = DateTime.Now,
        Status = "Published",
        AccountOrGroupName = request.AccountOrGroupName,
        AccountOrGroupId = request.AccountOrGroupName == "Groups" ? System.Text.Json.JsonSerializer.Serialize(groupIds) : System.Text.Json.JsonSerializer.Serialize(accountIds),
        PostIcon = System.Text.Json.JsonSerializer.Serialize(request.MediaUrl),
        ContentType = "Story",
        AccountPageId = request.AccountOrGroupName == "Groups" ? JsonConvert.SerializeObject(groupPageIds) : JsonConvert.SerializeObject(accountpageId),
      };

      _context.PostedStory.Add(newstory);
      _context.SaveChanges();

      if (request.AccountOrGroupName == "Groups")
      {
        foreach (var accountId in request.AccountOrGroupId)
        {
          // Directly access the `Id` property of `AccountOrPageId1`, no parsing needed
          var groupId = accountId.Id;
          var groupPagedata = await (from gsm in _context.GroupSocialMedia
                                     join g in _context.@group on gsm.GroupId equals g.Id
                                     where g.UserGuid == request.userGUId
                                           && gsm.GroupId.ToString() == groupId // Correct comparison
                                     select new { gsm.PageId, gsm.SocialMediaId })
                        .ToListAsync();
          foreach (var data in groupPagedata)
          {
            var pageId = data.PageId; // Default to 0 if null
            var accountid = data.SocialMediaId; // Default to 0 if null

            var userGroupPost = new UserGroupPosts
            {
              PostId = 0,
              GroupId = int.TryParse(groupId, out int value) ? value : 0,
              StoryId = newstory.Id,
              AccountID = accountid.ToString(),
              AccountPageId = pageId,
            };

            _context.UserGroupPosts.Add(userGroupPost);
          }
          _context.SaveChanges();
        }
      }
      else if (request.AccountOrGroupName == "Accounts")
      {
        foreach (var accountId in request.AccountOrGroupId)
        {
          
          var accountpageid = accountId.PageId;
          var accountid = accountId.accountId;
          var userGroupPost = new UserGroupPosts
          {
            PostId = 0,
            GroupId = 0,
            StoryId = newstory.Id,
            AccountID = accountid,
            AccountPageId = accountpageid,
          };

          _context.UserGroupPosts.Add(userGroupPost);
        }

        _context.SaveChanges();
      }

      var userSocialMediaStatus = new UserSocialMediaStatus
      {
        SocialMediaId = 1,
        UserGuid = request.userGUId,
        Status = 1

      };
      _context.UserSocialMediaStatus.Add(userSocialMediaStatus);

      _context.SaveChanges();

      List<ConnectedSocialMediaInfo> infoList = new List<ConnectedSocialMediaInfo>();

      var accountOrGroupIds = request.AccountOrGroupId.Select(a => a.Id).ToList();
      var socialMediaAccid = request.AccountOrGroupId.Select(a => a.accountId).ToList();
      var pageIdof_accountOrGroupId = request.AccountOrGroupId.Select(a => a.PageId).ToList();

      if (string.Equals(request.AccountOrGroupName, "Accounts", StringComparison.OrdinalIgnoreCase))
      {

        infoList = await _context.ConnectedSocialMediaInfo
   .Where(x => x.UserId == request.userGUId &&
               pageIdof_accountOrGroupId.Contains(x.PageId) &&
               socialMediaAccid.Contains(x.SocialMediaAccId.ToString()))
   .ToListAsync();
      }
      else if (string.Equals(request.AccountOrGroupName, "Groups", StringComparison.OrdinalIgnoreCase))
      {


        infoList = await (from gsm in _context.GroupSocialMedia
                          join g in _context.@group on gsm.GroupId equals g.Id
                          where g.UserGuid == request.userGUId
                                && accountOrGroupIds.Contains(gsm.GroupId.ToString())
                          select new ConnectedSocialMediaInfo
                          {
                            Id = gsm.Id,
                            SocialMediaAccId = gsm.SocialMediaId,
                            PageId = gsm.PageId,
                            PageAccessToken = gsm.PageAccessToken
                          })
              .ToListAsync();

      }
      else
      {
        return BadRequest(new { Message = "Fields Missing !..." });
      }
      if (request.MediaUrl != null && infoList.Count > 0)
      {
        if (infoList.Any(x => x.SocialMediaAccId == 1))
        {
          var infolist1 = infoList.Where(x => x.SocialMediaAccId == 1).ToList();
          foreach (var info in infolist1)
          {
            if (info.PageId != null && info.PageAccessToken != null)
            {
              
              var facebookApiUrl = $"https://graph.facebook.com/me/photos?access_token={info.PageAccessToken}";
              var content = new MultipartFormDataContent
      {
          { new StringContent(request.MediaUrl), "url" },
          { new StringContent("false"), "published" } // Photo will not be published immediately
      };

              try
              {
                var uploadResponse = await _httpClient.PostAsync(facebookApiUrl, content);

                if (!uploadResponse.IsSuccessStatusCode)
                {
                  var errorResponse = await uploadResponse.Content.ReadAsStringAsync();
                  return StatusCode((int)uploadResponse.StatusCode, new { success = false, message = "Facebook API error during photo upload", details = errorResponse });
                }

                // Step 2: Get the photo ID from the upload response
                var uploadResponseData = await uploadResponse.Content.ReadFromJsonAsync<FacebookPhotoResponse>();
                if (uploadResponseData == null || string.IsNullOrWhiteSpace(uploadResponseData.Id))
                {
                  return StatusCode(500, new { success = false, message = "Photo ID not found in Facebook upload response." });
                }

                // Step 3: Publish the story
                var publishApiUrl = $"https://graph.facebook.com/v21.0/{info.PageId}/photo_stories";
                var publishContent = new MultipartFormDataContent
                {
                    { new StringContent(info.PageAccessToken), "access_token" },
                    { new StringContent(uploadResponseData.Id), "photo_id" } // Use the uploaded photo ID
                };

                var publishResponse = await _httpClient.PostAsync(publishApiUrl, publishContent);

                if (!publishResponse.IsSuccessStatusCode)
                {
                  var errorResponse = await publishResponse.Content.ReadAsStringAsync();
                  return StatusCode((int)publishResponse.StatusCode, new { success = false, message = "Facebook API error during story publication", details = errorResponse });
                }

                var publishResponseContent = await publishResponse.Content.ReadAsStringAsync();
                var publishResponseData = System.Text.Json.JsonSerializer.Deserialize<FacebookStoryResponse>(publishResponseContent, new JsonSerializerOptions
                {
                  PropertyNameCaseInsensitive = true
                });

                if (publishResponseData != null && !string.IsNullOrWhiteSpace(publishResponseData.PostId))
                {
                  await SavePostIdToDatabase(publishResponseData.PostId, info.PageId, info.PageAccessToken, request.userGUId);
                  //return Ok(new { success = true, post_id = publishResponseData.PostId });
                  //return Ok(new { Message = "Story Uploaded successfully" });
                }

                //return StatusCode(500, new { success = false, message = "Post ID not found in Facebook story publication response." });
              }
              catch (Exception ex)
              {
                return StatusCode(500, new { success = false, message = "Internal server error", details = ex.Message });
              }
            }
          }
          
        }
      }

      else
      {
        return StatusCode(StatusCodes.Status400BadRequest, "Invalid request parameters.");
      }
            
      return Ok(new { Message = "Story Uploaded successfully" });
    }
    private async Task SavePostIdToDatabase(string postId, string pageId, string pageAccessToken, string userGUId)
    {
      var existingPost = await _context.PostIdForSocialMediaPosts
          .FirstOrDefaultAsync(p => p.PostId == postId && p.PageId == pageId);

      if (existingPost == null)
      {
        var PostIdForSocialMediaPosts = new PostIdForSocialMediaPosts
        {
          PostId = postId,
          userGUId = userGUId,
          PageId = pageId,
          PageAccessToken = pageAccessToken,
          CreatedOn = DateTime.Now
        };

        _context.PostIdForSocialMediaPosts.Add(PostIdForSocialMediaPosts);
        await _context.SaveChangesAsync();
      }
    }
    // Supporting classes
    public class FacebookPostRequest
    {
      public string PageId { get; set; }
      public string accessToken { get; set; }
      public string Message { get; set; }
      public List<string> MediaUrls { get; set; } // URLs for media files to upload
    }
     
    public class FacebookPhotosResponse
    {      
      public string id { get; set; }

      [JsonPropertyName("post_id")]
      public string PostId { get; set; }


    }

    [HttpPost("StorySaveDraft")]
    /*[Authorize]*/
    public async Task<IActionResult> CreateDraftstory(CreateStoryRequest request)
    {
      if (string.IsNullOrEmpty(request.userGUId) ||
         string.IsNullOrEmpty(request.MediaUrl) ||
          string.IsNullOrEmpty(request.AccountOrGroupName) ||
          request.AccountOrGroupId == null || request.AccountOrGroupId.Count == 0)
      {
        return BadRequest(new { Message = "Fields Missing" });
      }
      List<string> groupIds = request.AccountOrGroupName == "Groups"
? request.AccountOrGroupId.Select(a => !string.IsNullOrEmpty(a.Id) ? a.Id : "").ToList()
: new List<string>();


      List<string> accountpageIds = request.AccountOrGroupName == "Accounts"
? request.AccountOrGroupId.Select(a => !string.IsNullOrEmpty(a.PageId) ? a.PageId : "").ToList()
: new List<string>();
      List<string> accountIds = request.AccountOrGroupName == "Accounts"
? request.AccountOrGroupId.Select(a => !string.IsNullOrEmpty(a.accountId) ? a.accountId : "").ToList()
: new List<string>();
      var groupPageIds = await (from gsm in _context.GroupSocialMedia
                                join g in _context.@group on gsm.GroupId equals g.Id
                                where g.UserGuid == request.userGUId
                                      && groupIds.Contains(gsm.GroupId.ToString())
                                select gsm.PageId)
                            .ToListAsync();
      var newstory = new Drafts
      {
        UserGuid = request.userGUId,
        CreatedAt = DateTime.UtcNow,
        Status = "Draft",
        AccountOrGroupName = request.AccountOrGroupName,
        AccountOrGroupId = request.AccountOrGroupName == "Groups" ? System.Text.Json.JsonSerializer.Serialize(groupIds) : System.Text.Json.JsonSerializer.Serialize(accountIds),
        PostIcon = System.Text.Json.JsonSerializer.Serialize(request.MediaUrl),
        ContentType = "Story",
        PageId = request.AccountOrGroupName == "Groups" ? JsonConvert.SerializeObject(groupPageIds) : JsonConvert.SerializeObject(accountpageIds),
      };

      _context.Drafts.Add(newstory);
      _context.SaveChanges();

      return Ok(new { Message = "Story Draft created successfully" });

    }

    [HttpPost("ScheduledStoryPosts")]
    public async Task<IActionResult> ScheduledStoryPosts([FromBody] CreateScheduledPostRequest request)
    {
      try
      {
        if (!ModelState.IsValid)
        {
          return BadRequest(ModelState);
        }

        var user = _context.Users.FirstOrDefault(x => x.Id == request.UserGuid);
        if (user == null)
        {
          return NotFound("User not found");
        }

        var domain = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";

        // Directory to check for existing thumbnails
        var thumbnailFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

        // Prepare a list to hold the URLs of existing images
        var savedUrls = new List<string>();

        // Step 1: Check each media URL
        foreach (var mediaUrl in request.MediaUrl)
        {
          var fileName = Path.GetFileName(mediaUrl);
          var thumbnailPath = Path.Combine(thumbnailFolder, fileName);

          if (System.IO.File.Exists(thumbnailPath))
          {
            var fileUrl = $"{domain}/uploads/{fileName}";
            savedUrls.Add(fileUrl);
          }
        }

        if (savedUrls.Count == 0)
        {
          return Ok(new
          {
            Status = "false",
            Message = "Image Url Not found!",
          });
        }
        List<string> groupIds = request.AccountOrGroupName == "Groups"
? request.AccountOrGroupId.Select(a => !string.IsNullOrEmpty(a.Id) ? a.Id : "").ToList()
: new List<string>();


        List<string> accountpageIds = request.AccountOrGroupName == "Accounts"
  ? request.AccountOrGroupId.Select(a => !string.IsNullOrEmpty(a.PageId) ? a.PageId : "").ToList()
  : new List<string>();

        List<string> accountIds = request.AccountOrGroupName == "Accounts"
? request.AccountOrGroupId.Select(a => !string.IsNullOrEmpty(a.accountId) ? a.accountId : "").ToList()
: new List<string>();
        var groupPageIds = await (from gsm in _context.GroupSocialMedia
                                  join g in _context.@group on gsm.GroupId equals g.Id
                                  where g.UserGuid == request.UserGuid
                                        && groupIds.Contains(gsm.GroupId.ToString())
                                  select gsm.PageId)
                              .ToListAsync();
        // Create the ScheduledPost entity
        ScheduledPost res = new ScheduledPost
        {
          UserGuid = request.UserGuid,
          ScheduledType = request.ScheduledType == "1" ? "OneTime" :
                                     request.ScheduledType == "2" ? "Weekly" :
                                     request.ScheduledType == "3" ? "Monthly" : " ",
          Days = request.Days != null ? string.Join(",", request.Days) : null,
          Months = request.Months != null ? string.Join(",", request.Months) : null,
          ScheduledTime = request.ScheduledTime,
          ScheduledDate = request.ScheduledDate != null ? string.Join(",", request.ScheduledDate) : null,
          FromDate = !string.IsNullOrEmpty(request.FromDate) ? request.FromDate : null,
          ToDate = !string.IsNullOrEmpty(request.ToDate) ? request.ToDate : null,
          IsPublished = request.IsPublished,
          //Title = request.Title,
          //Description = request.Description,
          //MediaUrl = JsonConvert.SerializeObject(savedUrls),
          MediaUrl = string.Join(",", savedUrls),
          AccountOrGroupName = request.AccountOrGroupName,
          //AccountOrGroupId = JsonConvert.SerializeObject(request.AccountOrGroupId),
          AccountOrGroupId = request.AccountOrGroupName == "Groups" ? JsonConvert.SerializeObject(groupIds) : JsonConvert.SerializeObject(accountIds),
          //AccountOrGroupId = request.AccountOrGroupName == "Groups" ? request.AccountOrGroupId.ToString():string.Join(",", request.AccountOrGroupId.Select(e=>e.Id)),
          //PageId = string.Join(",", request.AccountOrGroupId.Select(e => e.PageId)),
          //createdOn = DateTime.Now,
          //DeviceToken = _context.NotificationSetting
          //              .Where(x => x.UserGUID == request.UserGuid)
          //              .OrderByDescending(x => x.Id)
          //              .Select(x => x.DeviceToken)
          //              .FirstOrDefault(),
          PageId = request.AccountOrGroupName == "Groups" ? JsonConvert.SerializeObject(groupPageIds) : JsonConvert.SerializeObject(accountpageIds),
          //Tags = "JsonConvert.SerializeObject(request.Tags)",
           ContentType = "Story",
          createdOn = DateTime.Now,
        };

        _context.ScheduledPost.Add(res);
        await _context.SaveChangesAsync();

        var scheduler = await _schedulerFactory.GetScheduler();

        // Schedule based on the scenario
        if (res.ScheduledType == "OneTime")
        {
          // Scenario 1: ScheduledDate and ScheduledTime
          foreach (var scheduledDate in res.ScheduledDate.Split(','))
          {
            if (DateTime.TryParse($"{scheduledDate} {res.ScheduledTime}", out DateTime scheduledDateTime))
            {
              var jobKey = new JobKey($"NotificationJob-{res.Id}-{scheduledDateTime:yyyyMMddHHmmss}", "NotificationGroup");
              var notificationJob = JobBuilder.Create<NotificationJob>()
                  .WithIdentity(jobKey)
                  .UsingJobData("ScheduledPostId", res.Id)
                  .UsingJobData("UserGuid", res.UserGuid)
                  .Build();

              var notificationTrigger = TriggerBuilder.Create()
                  .StartAt(scheduledDateTime.AddHours(-1)) // 1 hour before the scheduled time
                  .ForJob(jobKey)
                  .Build();

              await scheduler.ScheduleJob(notificationJob, notificationTrigger);

              // Schedule ScheduledOnTimeJob for the exact scheduled time
              var onTimeJobKey = new JobKey($"ScheduledOnTimeJob-{res.Id}-{scheduledDateTime:yyyyMMddHHmmss}", "ScheduledGroup");
              var onTimeJob = JobBuilder.Create<ScheduledOnTimeJob>()
                  .WithIdentity(onTimeJobKey)
                  .UsingJobData("ScheduledPostId", res.Id)
                  .UsingJobData("UserGuid", res.UserGuid)
                  .Build();

              var onTimeTrigger = TriggerBuilder.Create()
                  .StartAt(scheduledDateTime) // Exactly at the scheduled time
                  .ForJob(onTimeJobKey)
                  .Build();

              await scheduler.ScheduleJob(onTimeJob, onTimeTrigger);
            }
          }
        }
        else if (res.ScheduledType == "Weekly")
        {
          if (DateTime.TryParse(res.FromDate, out DateTime fromDate) && DateTime.TryParse(res.ToDate, out DateTime toDate))
          {
            foreach (var day in res.Days.Split(','))
            {
              var normalizedDay = NormalizeDay(day);
              if (Enum.TryParse(normalizedDay, true, out DayOfWeek dayOfWeek))
              {
                var currentDate = fromDate;
                while (currentDate <= toDate)
                {
                  if (currentDate.DayOfWeek == dayOfWeek)
                  {
                    if (DateTime.TryParse($"{currentDate:yyyy-MM-dd} {res.ScheduledTime}", out DateTime scheduledDateTime))
                    {
                      var jobKey = new JobKey($"NotificationJob-{res.Id}-{scheduledDateTime:yyyyMMddHHmmss}", "NotificationGroup");
                      var job = JobBuilder.Create<NotificationJob>()
                          .WithIdentity(jobKey)
                          .UsingJobData("ScheduledPostId", res.Id)
                          .UsingJobData("UserGuid", res.UserGuid)
                          .Build();

                      var trigger = TriggerBuilder.Create()
                          .StartAt(scheduledDateTime.AddHours(-1)) // 1 hour before the scheduled time
                          .ForJob(jobKey)
                          .Build();

                      await scheduler.ScheduleJob(job, trigger);

                      // Schedule ScheduledOnTimeJob for the exact scheduled time
                      var onTimeJobKey = new JobKey($"ScheduledOnTimeJob-{res.Id}-{scheduledDateTime:yyyyMMddHHmmss}", "ScheduledGroup");
                      var onTimeJob = JobBuilder.Create<ScheduledOnTimeJob>()
                          .WithIdentity(onTimeJobKey)
                          .UsingJobData("ScheduledPostId", res.Id)
                          .UsingJobData("UserGuid", res.UserGuid)
                          .Build();

                      var onTimeTrigger = TriggerBuilder.Create()
                          .StartAt(scheduledDateTime) // Exactly at the scheduled time
                          .ForJob(onTimeJobKey)
                          .Build();

                      await scheduler.ScheduleJob(onTimeJob, onTimeTrigger);
                    }
                  }
                  currentDate = currentDate.AddDays(1);
                }
              }
            }
          }
        }


        else if (res.ScheduledType == "Monthly")
        {
          foreach (var scheduledDate in res.ScheduledDate.Split(','))
          {
            if (DateTime.TryParseExact(scheduledDate.Trim(), "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime scheduledDateTime))
            {
              // Get the year, month, and day for the specific scheduled date
              int year = scheduledDateTime.Year;
              int month = scheduledDateTime.Month;
              int day = scheduledDateTime.Day;

              // Combine the parsed date with the scheduled time
              string dateTimeString = $"{year}-{month:D2}-{day:D2} {res.ScheduledTime}";

              // Parse the full date and time string
              if (DateTime.TryParse(dateTimeString, out DateTime ScheduledDateTime))
              {
                var jobKey = new JobKey($"NotificationJob-{res.Id}-{ScheduledDateTime:yyyyMMddHHmmss}", "NotificationGroup");

                // Create the NotificationJob
                var notificationJob = JobBuilder.Create<NotificationJob>()
                    .WithIdentity(jobKey)
                    .UsingJobData("ScheduledPostId", res.Id)
                    .UsingJobData("UserGuid", res.UserGuid)
                    .Build();

                // Create a trigger for 1 hour before the scheduled time
                var notificationTrigger = TriggerBuilder.Create()
                    .StartAt(ScheduledDateTime.AddHours(-1)) // 1 hour before the scheduled time
                    .ForJob(jobKey)
                    .Build();

                // Schedule the NotificationJob
                await scheduler.ScheduleJob(notificationJob, notificationTrigger);

                // Schedule the ScheduledOnTimeJob for the exact scheduled time
                var onTimeJobKey = new JobKey($"ScheduledOnTimeJob-{res.Id}-{ScheduledDateTime:yyyyMMddHHmmss}", "ScheduledGroup");
                var onTimeJob = JobBuilder.Create<ScheduledOnTimeJob>()
                    .WithIdentity(onTimeJobKey)
                    .UsingJobData("ScheduledPostId", res.Id)
                    .UsingJobData("UserGuid", res.UserGuid)
                    .Build();

                // Trigger for the exact scheduled time
                var onTimeTrigger = TriggerBuilder.Create()
                    .StartAt(ScheduledDateTime) // Exactly at the scheduled time
                    .ForJob(onTimeJobKey)
                    .Build();

                // Schedule the on-time job
                await scheduler.ScheduleJob(onTimeJob, onTimeTrigger);
              }
              else
              {
                // If the combined date and time string couldn't be parsed
                throw new Exception($"Invalid scheduled date and time format: {dateTimeString}");
              }
            }
            else
            {
              // If the scheduled date couldn't be parsed
              throw new Exception($"Invalid date format in ScheduledDate: {scheduledDate}");
            }
          }
        }


        return Ok(new
        {
          Status = "True",
          Message = "Story scheduled successfully",
          Data = res
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new
        {
          Status = "False",
          Message = "An error occurred while scheduling the post",
          Error = ex.Message,
          newerr = ex.InnerException?.Message
        });
      }
    }
    private string NormalizeDay(string day)
    {
      return day.ToLower() switch
      {
        "mon" => "Monday",
        "tue" => "Tuesday",
        "wed" => "Wednesday",
        "thu" => "Thursday",
        "fri" => "Friday",
        "sat" => "Saturday",
        "sun" => "Sunday",
        _ => day
      };
    }
    private List<string> ParsePostIcon(string postIcon)
    {
      // Check if the string is a valid JSON array
      if (postIcon.StartsWith("[") && postIcon.EndsWith("]"))
      {
        try
        {
          return JsonConvert.DeserializeObject<List<string>>(postIcon) ?? new List<string>();
        }
        catch (System.Text.Json.JsonException)
        {
          // If deserialization fails, return the string as a single item in the list
          return new List<string> { postIcon };
        }
      }

      // If it's not a JSON array, treat it as a single URL
      return new List<string> { postIcon };
    }
    private string GenerateServerPathUrl(string fileName)
    {
      var request = HttpContext.Request;
      var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
      return $"{baseUrl}/uploads/{fileName}";
      //return $"{baseUrl}/uploads/thumbnails/{fileName}";
    }
    [HttpGet("ScheduledStory")]
    
    public IActionResult ScheduledStory([FromQuery] StoryPlannerSchedule request)
    {
      var scheduledPostsData = (from sdp in _context.ScheduledPost
                                join gp in _context.@group on sdp.UserGuid equals gp.UserGuid
                                join gsm in _context.GroupSocialMedia on gp.Id equals gsm.GroupId
                                join sm in _context.SocialMedia on gsm.SocialMediaId equals sm.Id
                                where sdp.UserGuid == request.UserGUId && sdp.IsPublished != true && sdp.ContentType == "Story"
                                 && (gsm.GroupId > 0 || gsm.SocialMediaId != null)
                                select new
                                {
                                  scheduleedPostId = sdp.Id,
                                  id = gp.Id,
                                  Group = new GroupResponse
                                  {
                                    Name = gp.Name,
                                    GroupIcon = gp.GroupIcon,
                                    Platform = new string[] { sm.SocialMediaName },
                                  },                                  
                                  PostIcon = sdp.MediaUrl,
                                  ScheduledTimeString = sdp.ScheduledTime,
                                  Poststatus = sdp.IsPublished,
                                  ScheduledDate = sdp.ScheduledDate

                                }).ToList(); 

      
      var response = scheduledPostsData
          .GroupBy(x => x.scheduleedPostId) 
          .Select(g => g.FirstOrDefault()) 
          .OrderByDescending(x => x.scheduleedPostId) 
          .ToList();

      return Ok(response);
    }
    //[HttpGet("ScheduledStory")]
    ////[Authorize]
    //public async Task<IActionResult> ScheduledStory([FromQuery] StoryPlannerSchedule request)
    //{
    //  if (request.month == null)
    //    return BadRequest(new { Message = "Month is required." });

    //  if (request.year == null)
    //    return BadRequest(new { Message = "Year is required." });

    //  int Month = request.month;

    //  var groupSocialMedias = await _context.GroupSocialMedia.ToListAsync();
    //  var socialMedias = await _context.SocialMedia.ToListAsync();

    //  var postedStories = await _context.PostedStory
    //      .Where(sdp => sdp.UserGuid == request.userGUId &&
    //                    sdp.CreatedAt.Month == Month &&
    //                    sdp.CreatedAt.Year == request.year)
    //      .ToListAsync();

    //  var response = new List<object>();

    //  foreach (var sdp in postedStories)
    //  {
    //    List<string> pageIds;
    //    try
    //    {
    //      pageIds = JsonConvert.DeserializeObject<List<string>>(sdp.AccountPageId ?? "[]");
    //    }
    //    catch
    //    {
    //      pageIds = new List<string>();
    //    }

    //    foreach (var pageId in pageIds)
    //    {
    //      var matchedGsm = groupSocialMedias.FirstOrDefault(gsm => gsm.PageId == pageId);
    //      if (matchedGsm != null)
    //      {
    //        var sm = socialMedias.FirstOrDefault(x => x.Id == matchedGsm.SocialMediaId);
    //        response.Add(new
    //        {
    //          sdp.Id,
    //          sdp.PostIcon,
    //          ScheduledTimeString = g.Select(x => x.sdp.ScheduledTime).FirstOrDefault(),
    //          ScheduledDate = g.Select(x => x.sdp.ScheduledDate).FirstOrDefault(),
    //          PageName = matchedGsm.PageName,
    //          Platform = sm?.SocialMediaName
    //        });
    //      }
    //    }
    //  }

    //  // 🔍 Apply search filter
    //  if (!string.IsNullOrWhiteSpace(request.searchbox) && request.searchbox.ToLower() != "null")
    //  {
    //    response = response
    //        .Where(item =>
    //        {
    //          var platform = item.GetType().GetProperty("Platform")?.GetValue(item)?.ToString();
    //          return platform != null && platform.ToLower().Contains(request.searchbox.ToLower());
    //        })
    //        .ToList();
    //  }

    //  if (response.Count != 0)
    //    return Ok(response);
    //  else
    //    return BadRequest(new { Message = "Data Not Found!..." });
    //}

  }
}


