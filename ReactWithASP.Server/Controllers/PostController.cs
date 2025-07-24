using JWTAuthentication.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ReactWithASP.Server.Models;
using static System.Net.Mime.MediaTypeNames;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using System.IO;
using System.Threading.Tasks;
using SystemImage = System.Net.Mime.MediaTypeNames.Image;
using ImageSharp = SixLabors.ImageSharp;
using Newtonsoft.Json;
using ReactWithASP.Server.Models.Posts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Xabe.FFmpeg;
using System.IO;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using ReactWithASP.Server.Models.Settings;
using Google.Apis.Auth.OAuth2;
using System.Text;
using Quartz;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using ReactWithASP.Server.InterfaceServices;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Twitter;
using System;
using System.Net.Mime;
using PayPal;

namespace ReactWithASP.Server.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class PostController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment;
        private readonly ApplicationDbContext _context;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILinkedInService _linkedInService;
        private readonly IHttpClientFactory _httpClientFactory;



        public PostController(IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment, ApplicationDbContext context, ISchedulerFactory schedulerFactory, HttpClient httpClient, IHttpContextAccessor httpContextAccessor,ILinkedInService linkedInService, IHttpClientFactory httpClientFactory)
        {
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            Environment = environment;
            _linkedInService = linkedInService;
            _context = context;
            _schedulerFactory = schedulerFactory;
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
        }
       


        [HttpPost]
        /*[Authorize]*/
        public async Task<IActionResult> CreatePost(CreatePostRequest request)
            {
            if (string.IsNullOrEmpty(request.userGUId) ||
                string.IsNullOrEmpty(request.Title) ||
                string.IsNullOrEmpty(request.Description) ||
                request.MediaUrl == null || request.MediaUrl.Count == 0 ||
                request.Tags == null || request.Tags.Count == 0 ||
                string.IsNullOrEmpty(request.AccountOrGroupName) ||
                request.AccountOrGroupId == null || request.AccountOrGroupId.Count == 0)
            {
                return BadRequest(new { Message = "Fields Missing" });
            }
      List<string> groupIds = request.AccountOrGroupName == "Groups"
? request.AccountOrGroupId.Select(a => !string.IsNullOrEmpty(a.Id) ? a.Id : "").ToList()
: new List<string>();


      List<string> accountpageId = request.AccountOrGroupName == "Accounts"
? request.AccountOrGroupId.Select(a => !string.IsNullOrEmpty(a.PageId) ? a.PageId : "").ToList()
: new List<string>();
      //  // Process account IDs
      //  List<string> accountIds = request.AccountOrGroupName == "Account"
      //? request.AccountOrGroupId.Where(a => !string.IsNullOrEmpty(a.PageId)).Select(a => a.PageId).ToList()
      //: new List<string>();
      List<string> accountIds = request.AccountOrGroupName == "Accounts"
? request.AccountOrGroupId.Select(a => !string.IsNullOrEmpty(a.accountId) ? a.accountId : "").ToList()
: new List<string>();
      var groupPageIds = await (from gsm in _context.GroupSocialMedia
                                join g in _context.@group on gsm.GroupId equals g.Id
                                where g.UserGuid == request.userGUId
                                      && groupIds.Contains(gsm.GroupId.ToString())
                                select gsm.PageId)
                            .ToListAsync();

      var newPost = new SocialMediaPosts
            {
                UserGuid = request.userGUId,
                Title = request.Title,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
                Status = "Published",
                AccountOrGroupName = request.AccountOrGroupName,
        //AccountOrGroupId =request.AccountOrGroupName=="Groups"? request.AccountOrGroupId.Select(a => a.Id).ToString(): JsonConvert.SerializeObject(request.AccountOrGroupId.Select(x => x.Id).ToList()),
        AccountOrGroupId = request.AccountOrGroupName == "Groups" ? JsonConvert.SerializeObject(groupIds) : JsonConvert.SerializeObject(accountIds),
        //AccountOrGroupId = request.AccountOrGroupName == "Groups" ? request.AccountOrGroupId.Select(a => a.Id).ToString() : JsonConvert.SerializeObject(request.AccountOrGroupId.Select(x => x.Id).ToList()),
        PostIcon = JsonConvert.SerializeObject(request.MediaUrl),
              //Tags = request.Tags,
              ContentType ="Post",
              AccountPageId= request.AccountOrGroupName == "Groups" ? JsonConvert.SerializeObject(groupPageIds) : JsonConvert.SerializeObject(accountpageId),
        
      };

            _context.SocialMediaPosts.Add(newPost);
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
            var pageId = data.PageId ; // Default to 0 if null
            var accountid = data.SocialMediaId ; // Default to 0 if null

            var userGroupPost = new UserGroupPosts
            {
              GroupId = int.TryParse(groupId, out int value) ? value : 0,
              PostId = newPost.Id,
              StoryId = 0,
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
              GroupId = 0, 
              PostId = newPost.Id, 
              StoryId = 0, 
              AccountID = accountid,
              AccountPageId = accountpageid,
            };

            _context.UserGroupPosts.Add(userGroupPost);
          }
         
          _context.SaveChanges();
          }


      //foreach (var accountId in request.AccountOrGroupId)
      //{
      //        var groupId = accountId.Id;
      //        var userGroupPost = new UserGroupPosts
      //        {
      //            GroupId = groupId,
      //            PostId = newPost.Id
      //        };

      //        _context.UserGroupPosts.Add(userGroupPost);
      //}

      // Initializing likes, shares, and views with 0 for the new post
      //var postLikes = new PostLikes
      //{
      //    PostId = newPost.Id,
      //    UserGuid = request.userGUId,
      //    PostLikesCount = 0,
      //    CreatedAt= DateTime.UtcNow

      //};
      //_context.PostLikes.Add(postLikes);
      //_context.SaveChanges();
      //var postShares = new PostShares
      //{
      //    PostId = newPost.Id,
      //    UserGuid = request.userGUId,
      //    PostSharesCount = 0,
      //    CreatedAt = DateTime.UtcNow
      //};

      //_context.PostShares.Add(postShares);
      //_context.SaveChanges();

      //var postViews = new PostViews
      //{
      //    PostId = newPost.Id,
      //    UserGuid = request.userGUId,
      //    PostViewsCount = 0,
      //    CreatedAt = DateTime.UtcNow
      //};

      //_context.PostViews.Add(postViews);
      //_context.SaveChanges();

      var userSocialMediaStatus = new UserSocialMediaStatus
        {
          SocialMediaId = 1,
          UserGuid = request.userGUId,
          Status = 1

        };
        _context.UserSocialMediaStatus.Add(userSocialMediaStatus);
     
            _context.SaveChanges();
            //if (request.AccountOrGroupName == "Account")
            //{
            //    var infoList = _context.ConnectedSocialMediaInfo.Where(x => x.UserId == request.userGUId && request.AccountOrGroupId.Contains(x.Id.ToString())).ToList();
            //}
            //if (request.AccountOrGroupName == "group")
            //{
            //    var infoList1 = _context.group.Where(x => x.UserGuid == request.userGUId && request.AccountOrGroupId.Contains(x.Id.ToString())).ToList();
            //}

            List<ConnectedSocialMediaInfo> infoList = new List<ConnectedSocialMediaInfo>();

            var accountOrGroupIds = request.AccountOrGroupId.Select(a => a.Id).ToList();
            var socialMediaAccid = request.AccountOrGroupId.Select(a => a.accountId).ToList();
            var pageIdof_accountOrGroupId = request.AccountOrGroupId.Select(a => a.PageId).ToList();

            if (string.Equals(request.AccountOrGroupName, "Accounts", StringComparison.OrdinalIgnoreCase))
            {
                //infoList = await _context. ConnectedSocialMediaInfo
                //.Where(x => x.UserId == request.userGUId && request.AccountOrGroupId.Contains(x.SocialMediaAccId.ToString()))
                //.GroupBy(x => x.Id) // Group by the unique table Id
                //.Select(group => group.First()) // Select the first record in each group
                //.ToListAsync();

                //infoList = await _context.ConnectedSocialMediaInfo
                //   .Where(x => x.UserId == request.userGUId && pageIdof_accountOrGroupId.Contains(x.PageId) && accountOrGroupIds.Contains(x.SocialMediaAccId.ToString()))
                //   .ToListAsync();
        infoList = await _context.ConnectedSocialMediaInfo
   .Where(x => x.UserId == request.userGUId &&
               pageIdof_accountOrGroupId.Contains(x.PageId) &&
               socialMediaAccid.Contains(x.SocialMediaAccId.ToString()))
   .ToListAsync();
      }
            else if (string.Equals(request.AccountOrGroupName, "Groups", StringComparison.OrdinalIgnoreCase))
            {

        //infoList = (from gsm in _context.GroupSocialMedia
        //            join csi in _context.ConnectedSocialMediaInfo
        //            on gsm.SocialMediaId equals csi.SocialMediaAccId into csiGroup
        //            from csi in csiGroup.DefaultIfEmpty() // This handles nulls from the join
        //            where request.AccountOrGroupId.Contains(gsm.GroupId.ToString())
        //            //where request.AccountOrGroupId.Contains(gsm.GroupId.ToString()) && csi.SocialMediaAccId == request.socialmediaId
        //            select new ConnectedSocialMediaInfo
        //            {
        //                Id = csi.Id,
        //                SocialMediaAccId = csi != null ? csi.SocialMediaAccId : 0,  // Handle possible null
        //                SocialMediaAccName = csi != null ? csi.SocialMediaAccName : string.Empty,
        //                PageId = csi != null ? csi.PageId : string.Empty,
        //                PageAccessToken = csi != null ? csi.PageAccessToken : string.Empty
        //            }).ToList();       

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

        //infoList = await (from gsm in _context.GroupSocialMedia                                 
        //                          join g in _context.@group on gsm.GroupId equals g.Id
        //                  where g.UserGuid == request.userGUId && request.AccountOrGroupId.Split(',').Select(int.Parse).Contains(gsm.GroupId)
                         
        //                          /*where request.userGUId==g.UserGuid && request.AccountOrGroupId.Select(a => a.Id).ToString().Contains(gsm.GroupId.ToString()) */// Assuming you need to check UserGuid as well
        //                          select new ConnectedSocialMediaInfo
        //                          {
        //                              // Populate relevant fields from both tables if needed
        //                              Id = gsm.Id,// You can add other properties from GroupSocialMedia
        //                              SocialMediaAccId = gsm.SocialMediaId,
        //                              //SocialMediaAccName = gsm.SocialMediaAccName,
        //                              PageId = gsm.PageId,
        //                              PageAccessToken = gsm.PageAccessToken,
        //                              // Add more fields if necessary
        //                          })
        //              .ToListAsync();

            }
            else
            {
                return BadRequest(new { Message = "Something Went wrong !..." });
            }



            //Facebook Upload Logic(if video or image is provided)

            if (request.MediaUrl != null &&infoList.Count > 0)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                foreach (var mediaFile in request.MediaUrl)
                {
                    // Combine the path dynamically for each media file
                    //string filePath2 = Path.Combine(wwwRootPath, "uploads", "thumbnails", mediaFile);
                    string filePath2 = Path.Combine(wwwRootPath, "uploads", mediaFile);

                    IFormFile fullfile = ConvertPathToIFormFile(filePath2);

                    if (fullfile == null)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, $"Invalid file path or file not found: {mediaFile}");
                    }

                    IFormFile videoFile = null;
                    IFormFile imageFile = null;

                    // Check the content type of the file
                    string fileExtension = Path.GetExtension(fullfile.FileName).ToLower();

                    bool isVideo = fileExtension == ".mp4";
                    bool isImage = fileExtension == ".png" || fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".jfif";

                    if (isVideo)
                    {
                        videoFile = fullfile;
                    }
                    else if (isImage)
                    {
                        imageFile = fullfile;
                    }

                    // Loop through all the ConnectedSocialMediaInfo records
                    if (infoList.Any(x => x.SocialMediaAccId == 1))
                    {
                        var infolist1 = infoList.Where(x => x.SocialMediaAccId == 1).ToList();
                        foreach (var info in infolist1)
                        {
                            if (info.PageId != null && info.PageAccessToken != null)
                            {
                                using (var client = new HttpClient())
                                {
                                    // Video upload logic (if video file exists)
                                    if (videoFile != null && videoFile.Length > 0)
                                    {
                                        try
                                        {
                                            // Step 1: Start video upload
                                            var startResponse = await client.PostAsync(
                                                $"https://graph.facebook.com/v12.0/{info.PageId}/videos?upload_phase=start&file_size={videoFile.Length}&access_token={info.PageAccessToken}",
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

                                            // Step 2: Upload video in chunks
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
                                                            $"https://graph.facebook.com/v12.0/{info.PageId}/videos?access_token={info.PageAccessToken}",
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
                      string hashtags = string.Join(" ", request.Tags.Select(tag => tag.StartsWith("#") ? tag : $"#{tag}"));
                      string finalDescription = $"{request.Description} {hashtags}";

                      // Step 3: Finish video upload and publish with description
                      var finishAndPublishResponse = await client.PostAsync(
                                                $"https://graph.facebook.com/v12.0/{info.PageId}/videos" +
                                                $"?upload_phase=finish" +
                                                $"&upload_session_id={uploadSessionId}" +
                                                $"&access_token={info.PageAccessToken}" +
                                                $"&description={Uri.EscapeDataString(finalDescription)}", // Include the description here
                                                null
                                            );

                                            if (!finishAndPublishResponse.IsSuccessStatusCode)
                                            {
                                                var finishAndPublishError = await finishAndPublishResponse.Content.ReadAsStringAsync();
                                                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to finish video upload and publish: " + finishAndPublishError);
                                            }

                                            // Parse the publish response to get the post ID
                                            var publishContent = await finishAndPublishResponse.Content.ReadAsStringAsync();
                                            dynamic publishJson = Newtonsoft.Json.JsonConvert.DeserializeObject(publishContent);
                                            string postId = publishJson.postid;


                                            if (string.IsNullOrEmpty(postId))
                                            {
                                                // Retrieve video details if postId is not included in finish response
                                                var videoDetailsResponse = await client.GetAsync(
                                                    $"https://graph.facebook.com/v12.0/{videoId}?fields=id,post_id&access_token={info.PageAccessToken}"
                                                );

                                                if (!videoDetailsResponse.IsSuccessStatusCode)
                                                {
                                                    var videoDetailsError = await videoDetailsResponse.Content.ReadAsStringAsync();
                                                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to retrieve video details: " + videoDetailsError);
                                                }

                                                var videoDetailsContent = await videoDetailsResponse.Content.ReadAsStringAsync();
                                                dynamic videoDetailsJson = Newtonsoft.Json.JsonConvert.DeserializeObject(videoDetailsContent);
                                                postId = videoDetailsJson.post_id;
                                            }


                                            // Save the post details to the database
                                            await SavePostIdToDatabase(postId, info.PageId, info.PageAccessToken, request.userGUId);

                                            //return Ok(new { message = "Video uploaded and published successfully", postId });
                                        }
                                        catch (Exception ex)
                                        {
                                            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred: " + ex.Message);
                                        }
                                    }

                                    // Image upload logic (if image file exists)
                                    if (imageFile != null && imageFile.Length > 0)
                                    {
                                        using (var imageStream = imageFile.OpenReadStream())
                                        {
                                            byte[] imageBytes;
                                            using (var ms = new MemoryStream())
                                            {
                                                imageStream.CopyTo(ms);
                                                imageBytes = ms.ToArray();
                                            }

                                            var imageContent = new MultipartFormDataContent();
                                            imageContent.Add(new ByteArrayContent(imageBytes), "source", imageFile.FileName);
                                            //imageContent.Add(new StringContent(request.Description), "caption");

                      // Prepare hashtags and caption
                      string hashtags = string.Join(" ", request.Tags.Select(tag => tag.StartsWith("#") ? tag : $"#{tag}"));
                      string finalCaption = $"{request.Description} {hashtags}";

                      // Add caption with hashtags
                      imageContent.Add(new StringContent(finalCaption), "caption");

                      var imageUploadResponse = await client.PostAsync(
                                                $"https://graph.facebook.com/v12.0/{info.PageId}/photos?access_token={info.PageAccessToken}",
                                                imageContent
                                            );

                                            var publishContent = await imageUploadResponse.Content.ReadAsStringAsync();
                                            dynamic publishJson = Newtonsoft.Json.JsonConvert.DeserializeObject(publishContent);
                                            string postId = publishJson.post_id;

                                            if (!imageUploadResponse.IsSuccessStatusCode)
                                            {
                                                var imageErrorContent = await imageUploadResponse.Content.ReadAsStringAsync();
                                                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to upload image: " + imageErrorContent);
                                            }

                                            await SavePostIdToDatabase(postId, info.PageId, info.PageAccessToken, request.userGUId);

                                        }
                                    }

                                }
                            }
                        }
                    }

                    


                }
            }


            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Invalid request parameters.");
            }





            // Check if notifications are allowed
            var allowedNotifications = _context.Notification
                .Where(x => x.UserGuid == request.userGUId && x.Status == true && x.Name == "Remind Before 1 hours")
                .ToList();
            var Dtoken = _context.NotificationSetting.FirstOrDefault(x => x.UserGUID == request.userGUId);
            if (allowedNotifications.Any()) // If notifications are allowed
            {
                await SendPostCreationNotification(request.userGUId, Dtoken.DeviceToken);
            }


            return Ok(new { Message = "Post created and Media Uploaded successfully" });
        }

      
        // Method to save PostId to database
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


        


        [HttpGet("CountByPostId")]
        public async Task<IActionResult> GetPostMetrics(string userGuid)
        {
            if (string.IsNullOrWhiteSpace(userGuid))
            {
                return BadRequest("User GUID is required.");
            }

            // Fetch access token for the given user GUID
            var userAccessToken = await _context.PostIdForSocialMediaPosts
                .Where(x => x.userGUId == userGuid)
                .Select(x => x.PageAccessToken)
                .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(userAccessToken))
            {
                return NotFound("Access token not found for the specified User GUID.");
            }

            var postInfo = _context.PostIdForSocialMediaPosts.Where(x => x.userGUId == userGuid).ToList();
            if (postInfo == null || !postInfo.Any())
            {
                return NotFound("No posts found for the specified User GUID.");
            }

            try
            {
                foreach (var post in postInfo)
                {
                    string postId = post.PostId;

                    // Save reaction counts
                    var reactionTypes = new[] { "like", "love", "haha", "wow", "sad", "angry" };
                    foreach (var type in reactionTypes)
                    {
                        var reactionUrl = $"https://graph.facebook.com/v21.0/{postId}/insights?metric=post_reactions_{type}_total&access_token={userAccessToken}";
                        var reactionCount = await GetMetricCountAsync(reactionUrl) ?? 0;

                        // Save to LikesTable
                        var likeRecord = new PostLikes
                        {
                            PostId = postId,
                            UserGuid = userGuid,
                            CreatedAt = DateTime.Now,
                            ReactionType = type,
                            PostLikesCount = reactionCount
                        };
                        _context.PostLikes.Add(likeRecord);
                    }

                    // Save share count
                    var shareUrl = $"https://graph.facebook.com/v17.0/{postId}?fields=shares&access_token={userAccessToken}";
                    var shareCount = await GetShareCountAsync(shareUrl);
                    var shareRecord = new PostShares
                    {
                        PostId = postId,
                        UserGuid = userGuid,
                        CreatedAt = DateTime.Now,
                        PostSharesCount = shareCount
                    };
                    _context.PostShares.Add(shareRecord);

                    // Save view count
                    var viewsUrl = $"https://graph.facebook.com/v17.0/{postId}/insights?metric=total_video_views&period=lifetime&access_token={userAccessToken}";
                    var viewCount = await GetMetricCountAsync(viewsUrl) ?? 0;
                    var viewRecord = new PostViews
                    {
                        PostId = postId,
                        UserGuid = userGuid,
                        CreatedAt = DateTime.Now,
                        PostViewsCount = viewCount
                    };
                    _context.PostViews.Add(viewRecord);
                }

                // Save all changes to the database
                await _context.SaveChangesAsync();

                return Ok("Metrics saved successfully.");
            }
            catch (HttpRequestException httpEx)
            {
                return StatusCode(500, $"HTTP Request error: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }

        // Helper method to fetch metric count
        private async Task<int?> GetMetricCountAsync(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var json = JsonDocument.Parse(content);

                    if (json.RootElement.TryGetProperty("data", out var dataArray) && dataArray.GetArrayLength() > 0)
                    {
                        var dataObject = dataArray[0];
                        if (dataObject.TryGetProperty("values", out var valuesArray) && valuesArray.GetArrayLength() > 0)
                        {
                            var valueObject = valuesArray[0];
                            if (valueObject.TryGetProperty("value", out var countValue))
                            {
                                return countValue.GetInt32();
                            }
                        }
                    }
                }
                return 0; // Default to 0 if data is missing
            }
            catch
            {
                return null; // Return null to indicate failure
            }
        }

        // Helper method to fetch share count
        private async Task<int> GetShareCountAsync(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var json = JsonDocument.Parse(content);

                    if (json.RootElement.TryGetProperty("shares", out var shareElement) &&
                        shareElement.TryGetProperty("count", out var shareCount))
                    {
                        return shareCount.GetInt32();
                    }
                }
                return 0; // Default to 0 if data is missing
            }
            catch
            {
                return 0; // Default to 0 if request fails
            }
        }




        [HttpGet("LinkedInCount")]
        public async Task<IActionResult> GetLinkedInPostMetadata(string postURN, string accessToken)
        {
            if (string.IsNullOrEmpty(postURN))
            {
                return BadRequest("Post URN is required.");
            }

            // Updated endpoint using LinkedIn REST API for social actions
            string requestUrl = $"https://api.linkedin.com/rest/socialActions/{postURN}";

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            client.DefaultRequestHeaders.Add("LinkedIn-Version", "202306");

            try
            {
                var response = await client.GetAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Error retrieving data from LinkedIn API: {errorContent}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var metadata = JsonConvert.DeserializeObject<LinkedInPostStatistics>(content);

                return Ok(metadata);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Request error: {ex.Message}");
            }
        }


        //[HttpGet("CountByPostId")]
        //public async Task<IActionResult> GetPostMetrics(string postId, string accessToken)
        //{
        //    if (string.IsNullOrWhiteSpace(postId) || string.IsNullOrWhiteSpace(accessToken))
        //    {
        //        return BadRequest("Post ID and access token are required.");
        //    }

        //    try
        //    {
        //        // Construct the Graph API URL

        //        var url = $"https://graph.facebook.com/v20.0/{postId}/insights?access_token={accessToken}&period=lifetime&metric=post_reactions_like_total,post_shares";
        //        var shareUrl = $"https://graph.facebook.com/v20.0/{postId}/insights?access_token={accessToken}&period=lifetime&metric=post_shares";
        //        var viewsUrl = $"https://graph.facebook.com/v20.0/{postId}/insights?access_token={accessToken}&period=lifetime&metric=video_views";



        //        var response = await _httpClient.GetAsync(url);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            var result = await response.Content.ReadAsStringAsync();
        //            return Ok(result); // Return the metrics as JSON response
        //        }
        //        else
        //        {
        //            var errorContent = await response.Content.ReadAsStringAsync();
        //            return BadRequest($"Error fetching post metrics: {errorContent}");
        //        }
        //    }
        //    catch (HttpRequestException httpEx)
        //    {
        //        return StatusCode(500, $"HTTP Request error: {httpEx.Message}");
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception if necessary
        //        return StatusCode(500, $"Unexpected error: {ex.Message}");
        //    }
        //}


        //public async Task<IActionResult> GetPostEngagement(string pageid)
        //{
        //    if (string.IsNullOrEmpty(pageid))
        //    {
        //        return BadRequest("Post ID and access token are required.");
        //    }
        //    var result = _context.PostIdForSocialMediaPosts.FirstOrDefault(x => x.PageId == pageid);

        //    var engagementStats = GetPostEngagementStatsAsync(result.PostId, result.PageAccessToken);
        //    return Ok(engagementStats);
        //}


        //private async Task<PostEngagementStats> GetPostEngagementStatsAsync(string postId, string accessToken)
        //{
        //    var url = $"https://graph.facebook.com/v12.0/{postId}?fields=likes.summary(true),shares,insights.metric(post_impressions)&access_token={accessToken}";

        //    var response = await _httpClient.GetAsync(url);
        //    response.EnsureSuccessStatusCode();

        //    var jsonResponse = await response.Content.ReadAsStringAsync();

        //    // Parse the response here
        //    dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse);

        //    var likes = data.likes.summary.total_count;
        //    var shares = data.shares != null ? data.shares : 0;
        //    var views = data.insights.data[0].values[0].value; // Assuming `post_impressions` represents views

        //    return new PostEngagementStats
        //    {
        //        Likes = likes,
        //        Shares = shares,
        //        Views = views
        //    };
        //}




        private async Task SendPostCreationNotification(string userGuid, string deviceTokens)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == userGuid);

            if (user == null)
            {
                // Handle the case where the user does not exist, if needed
                return;
            }

            var notificationSetting = new NotificationSetting
            {
                UserGUID = user.Id,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                DeviceToken = deviceTokens
            };

            string[] deviceTokensArray = new string[1];
            deviceTokensArray[0] = deviceTokens;
            var content = "New post created"; // Customize the notification content

            string fileName = "fcmpushnotificationfile.json";
            string relativePath = Path.Combine("FirebaseNotification", fileName);
            string path = Path.Combine(_webHostEnvironment.ContentRootPath, relativePath);

            try
            {
                await SendAndroidNotificationAsync2(deviceTokensArray, content, path);

                // Add the notification setting to the database
                _context.NotificationSetting.Add(notificationSetting);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception or handle the error appropriately
            }
        } 

        private async Task SendAndroidNotificationAsync2(string[] deviceTokens, string content, string path)
        {
            var credentials = GoogleCredential.FromFile(path).CreateScoped("https://www.googleapis.com/auth/firebase.messaging");

            var token = await credentials.UnderlyingCredential.GetAccessTokenForRequestAsync();


            await SendNotificationsAsync(deviceTokens, token, content);
        }

        private Task SendNotificationsAsync(string[] deviceTokens, string accessToken, string content)
        {
            var tasks = deviceTokens.Select(token => SendNotificationAsync1(token, accessToken, content)).ToArray();
            return Task.WhenAll(tasks);
        }

        private async Task SendNotificationAsync1(string deviceToken, string accessToken, string content)
        {
            string FcmUrl = "https://fcm.googleapis.com/v1/projects/assr-38c3f/messages:send";
            var message = new
            {
                message = new
                {
                    token = deviceToken,
                    notification = new
                    {
                        title = "GPO News",
                        body = content
                    },
                    data = new
                    {
                        key1 = "value1",
                        key2 = "value2"
                    }
                }
            };

            var jsonBody = JsonConvert.SerializeObject(message);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {accessToken}");

                var response = await httpClient.PostAsync(FcmUrl, httpContent);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);

                    if (responseJson != null && responseJson.ContainsKey("name"))
                    {

                    }

                }
            }
        }






















    //[HttpPost("upload")]
    ///*[Authorize]*/
    //public async Task<IActionResult> UploadMedia([FromForm] MediaSelectionRequest request)
    //{
    //    if (request.MediaFiles == null || request.MediaFiles.Count == 0)
    //    {
    //        return BadRequest(new MediaSelectionResponse
    //        {
    //            Message = "No media files selected"
    //        });
    //    }

    //    var processedFiles = new List<MediaFileResponse>();

    //    foreach (var formFile in request.MediaFiles)
    //    {
    //        if (formFile.Length > 0)
    //        {
    //            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
    //            if (!Directory.Exists(uploadsFolder))
    //            {
    //                Directory.CreateDirectory(uploadsFolder);
    //            }

    //            var cleanedFileName = Guid.NewGuid().ToString() + "_" + formFile.FileName;
    //  string originalFileName = cleanedFileName;

    //  // Decode URL-encoded characters
    //  string decodedFileName = Uri.UnescapeDataString(originalFileName);

    //  // Remove the space represented by "%20"
    //  string uniqueFileName = decodedFileName.Replace(" ", "");
    //  //var uniqueFileName = Guid.NewGuid().ToString() + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
    //  var filePath = Path.Combine(uploadsFolder, uniqueFileName);

    //            // Save the uploaded file to the server
    //            using (var stream = new FileStream(filePath, FileMode.Create))
    //            {
    //                await formFile.CopyToAsync(stream);
    //            }

    //            var thumbnailFolder = Path.Combine(uploadsFolder, "thumbnails");
    //            if (!Directory.Exists(thumbnailFolder))
    //            {
    //                Directory.CreateDirectory(thumbnailFolder);
    //            }

    //            string thumbnailFileName;
    //            string thumbnailPath;
    //            string newFileName;
    //            var fileUrl ="";
    //            // Check if the uploaded file is an image or video
    //            if (formFile.ContentType.StartsWith("image"))
    //            {
    //                // Create image thumbnail (use .jpg for image thumbnails)
    //                thumbnailFileName = "thumb_" + Path.GetFileName(uniqueFileName);
    //                thumbnailPath = Path.Combine(thumbnailFolder, thumbnailFileName);

    //                await GenerateImageThumbnailAsync(filePath, thumbnailPath);
    //            }
    //            else if (formFile.ContentType.StartsWith("video"))
    //            {
    //                // Create video thumbnail (use .jpg for video thumbnails)
    //                thumbnailFileName = "thumb_" + Path.GetFileName(uniqueFileName);
    //                thumbnailPath = Path.Combine(thumbnailFolder, thumbnailFileName);

    //                await GenerateVideoThumbnailAsync(filePath, thumbnailPath);
    //            }
    //            else
    //            {
    //                continue; // Skip unsupported media types


    //            }

    //              newFileName = "" + Path.GetFileName(uniqueFileName);
    //              fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{newFileName}";
    //              var thumbnailUrl = $"{Request.Scheme}://{Request.Host}/uploads/thumbnails/{thumbnailFileName}";

    //            // Processed file response
    //            processedFiles.Add(new MediaFileResponse
    //            {
    //                //FileName = thumbnailFileName,
    //                FileName = uniqueFileName,
    //                ImageName = uniqueFileName,
    //                FilePath = fileUrl,
    //                ContentType = formFile.ContentType,
    //                ThumbnailUrl = thumbnailUrl
    //            });
    //        }
    //    }

    //    return Ok(new MediaSelectionResponse
    //    {
    //        Message = "Media files uploaded successfully",
    //        ProcessedMediaFiles = processedFiles
    //    });
    //}
    [HttpPost("upload")]
    /*[Authorize]*/
    public async Task<IActionResult> UploadMedia([FromForm] MediaSelectionRequest request)
    {
      if (request.MediaFiles == null || request.MediaFiles.Count == 0)
      {
        return BadRequest(new MediaSelectionResponse
        {
          Message = "No media files selected"
        });
      }

      var processedFiles = new List<MediaFileResponse>();

      foreach (var formFile in request.MediaFiles)
      {
        if (formFile.Length > 0)
        {
          var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
          if (!Directory.Exists(uploadsFolder))
          {
            Directory.CreateDirectory(uploadsFolder);
          }

          var cleanedFileName = Guid.NewGuid().ToString() + "_" + formFile.FileName;
          string originalFileName = cleanedFileName;

          // Decode URL-encoded characters
          string decodedFileName = Uri.UnescapeDataString(originalFileName);

          // Remove the space represented by "%20"
          string uniqueFileName = decodedFileName.Replace(" ", "");
          //var uniqueFileName = Guid.NewGuid().ToString() + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
          var filePath = Path.Combine(uploadsFolder, uniqueFileName);

          // Save the uploaded file to the server
          using (var stream = new FileStream(filePath, FileMode.Create))
          {
            await formFile.CopyToAsync(stream);
          }

          var thumbnailFolder = Path.Combine(uploadsFolder, "thumbnails");
          if (!Directory.Exists(thumbnailFolder))
          {
            Directory.CreateDirectory(thumbnailFolder);
          }

          string thumbnailFileName;
          string thumbnailPath;
          string newFileName;
          var fileUrl = "";
          // Check if the uploaded file is an image or video
          if (formFile.ContentType.StartsWith("image"))
          {
            // Create image thumbnail (use .jpg for image thumbnails)
            thumbnailFileName = "thumb_" + Path.GetFileName(uniqueFileName);
            thumbnailPath = Path.Combine(thumbnailFolder, thumbnailFileName);

            await GenerateImageThumbnailAsync(filePath, thumbnailPath);
          }
          else if (formFile.ContentType.StartsWith("video"))
          {
            // Change the thumbnail file extension to .jpg
            var baseName = Path.GetFileNameWithoutExtension(uniqueFileName); // remove .mp4 or any extension
            thumbnailFileName = "thumb_" + baseName + ".jpg"; // now it's thumb_filename.jpg
            thumbnailPath = Path.Combine(thumbnailFolder, thumbnailFileName);

            await GenerateVideoThumbnailAsync(filePath, thumbnailPath);
          }
          else
          {
            continue; // Skip unsupported media types


          }

          newFileName = "" + Path.GetFileName(uniqueFileName);
          fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{newFileName}";
          var thumbnailUrl = $"{Request.Scheme}://{Request.Host}/uploads/thumbnails/{thumbnailFileName}";

          // Processed file response
          processedFiles.Add(new MediaFileResponse
          {
            //FileName = thumbnailFileName,
            FileName = uniqueFileName,
            ImageName = uniqueFileName,
            FilePath = fileUrl,
            ContentType = formFile.ContentType,
            ThumbnailUrl = thumbnailUrl
          });
        }
      }

      return Ok(new MediaSelectionResponse
      {
        Message = "Media files uploaded successfully",
        ProcessedMediaFiles = processedFiles
      });
    }

    private async Task GenerateImageThumbnailAsync(string imagePath, string thumbnailPath)
        {
            using (var image = SixLabors.ImageSharp.Image.Load(imagePath))
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Crop,
                    Size = new Size(150, 150)
                }));
                await image.SaveAsync(thumbnailPath);
            }
        }
        private async Task GenerateVideoThumbnailAsync(string videoPath, string thumbnailPath)
        {
      try
      {
        var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
        ffMpeg.GetVideoThumbnail(videoPath, thumbnailPath, 1); // Saves to .jpg or .png based on thumbnailPath extension
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error generating video thumbnail: {ex.Message}");
        throw;
      }
    }



        //[HttpPost("CreateHashtags")]
        //[Authorize]
        //public IActionResult CreateHashtags([FromBody] CreateHashtagRequest request)
        //{
        //    if (string.IsNullOrEmpty(request.userGUId) || request.Hashtags == null || request.Hashtags.Count == 0)
        //    {
        //        return BadRequest(new CreateHashtagResponse
        //        {
        //            Message = "Hashtag not created"
        //        });
        //    }

        //    // Here you can add the logic to save the hashtags to your data store

        //    return Ok(new CreateHashtagResponse
        //    {
        //        Message = "Hashtags created successfully"
        //    });
        //}


        [HttpPost("CreateHashtagGroup")]
        //[Authorize]
        public IActionResult CreateHashtagGroup([FromBody] CreateHashtagGroupRequest request)
        {
            if (string.IsNullOrEmpty(request.userGUId) || request.userGUId.Trim().Equals("string", StringComparison.OrdinalIgnoreCase)||
                string.IsNullOrEmpty(request.name) || request.name.Trim().Equals("string", StringComparison.OrdinalIgnoreCase)||
               request.Hashtags.Any(h => h.Trim().Equals("string", StringComparison.OrdinalIgnoreCase))|| request.Hashtags == null || request.Hashtags.Count == 0)
            {
                return BadRequest(new CreateHashtagGroupResponse
                {
                    Message = "Fields Missing"
                });
            }

            var hashtagGroup = new HashtagGroup
            {
                UserGuid = request.userGUId,
                name = request.name,
                CreatedOn = DateTime.UtcNow,
            };

            _context.HashtagGroup.Add(hashtagGroup);
            _context.SaveChanges();

            var hashtagGroupId = hashtagGroup.Id;

            foreach (var item in request.Hashtags)
            {
                var hashtag = new Hashtag
                {
                    UserGuid = request.userGUId,
                    HashtagGroupId = hashtagGroupId,
                    HashtagName = item,
                    HashtagCount = request.Hashtags.Count,
                    CreatedOn = DateTime.UtcNow,
                };

                _context.Hashtag.Add(hashtag);
            }

            _context.SaveChanges(); // Save all hashtags at once

            return Ok(new CreateHashtagGroupResponse
            {
                Message = "Hashtag Group created successfully"
            });
        }

        [HttpGet("GetHashtagGroupDetailsbyID")]
        public IActionResult HashtagGroupDetails(string userguid)
        {
            if (userguid == null)
            {
                return BadRequest(new { Message = "User GUID is required." });
            }

            var response = _context.HashtagGroup
                .Where(htg => htg.UserGuid == userguid)
                .GroupBy(htg => new { htg.Id, htg.name, htg.CreatedOn })
                .Select(group => new
                {
                 Id =group.Key.Id,
                    name = group.Key.name,
                    Titles = string.Join(", ", group.SelectMany(g => _context.Hashtag.Where(tg => tg.HashtagGroupId == group.Key.Id).Select(tg => tg.HashtagName))),
                    CreatedOn = group.Key.CreatedOn,
                })
                .ToList();

            return Ok(response);
        }

        [HttpGet("GetAllHashtagGroupDetails")]
        public IActionResult AllHashtagGroupDetails()
        {
            var response = _context.HashtagGroup
                .GroupBy(htg => new { htg.Id, htg.name, htg.CreatedOn })
                .Select(group => new
                {
                    name = group.Key.name,
                    Titles = string.Join(", ", group.SelectMany(g => _context.Hashtag.Where(tg => tg.HashtagGroupId == group.Key.Id).Select(tg => tg.HashtagName))),
                    CreatedOn = group.Key.CreatedOn
                })
                .ToList();

            return Ok(response);
        }

        //[HttpGet("GetAllHashtagDetails")]
        //public IActionResult GetAllHashtagDetails()
        //{
        //    var response = _context.Hashtag
        //        .GroupBy(htg => new { htg.Id,htg.UserGuid, htg.HashtagName, htg.CreatedOn,htg.HashtagCount })
        //        .Select(group => new
        //        {
        //            userid = group.Key.UserGuid,
        //            HashtagName = group.Key.HashtagName,
        //            CreatedOn = group.Key.CreatedOn,
        //            HashtagCount= group.Key.HashtagCount

        //        })
        //        .ToList();

        //    return Ok(response);
        //}

        [HttpGet("GetAllHashtagDetails")]
        public IActionResult GetAllHashtagDetails(int pageNumber = 1, int pageSize = 10)
        {
            // Ensure pageNumber and pageSize have valid values
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            // Calculate the number of records to skip based on the current page
            var skip = (pageNumber - 1) * pageSize;

            var totalRecords = _context.Hashtag.Count(); // Total records count for pagination metadata

            var response = _context.Hashtag
                .GroupBy(htg => new { htg.Id, htg.UserGuid, htg.HashtagName, htg.CreatedOn, htg.HashtagCount })
                .Select(group => new
                {
                    userid = group.Key.UserGuid,
                    HashtagName = group.Key.HashtagName,
                    CreatedOn = group.Key.CreatedOn,
                    HashtagCount = group.Key.HashtagCount
                })
                .Skip(skip)  // Skip the records based on the page number
                .Take(pageSize)  // Take the number of records for the current page
                .ToList();

            // Return the paginated result along with the pagination metadata
            var paginationMetadata = new
            {
                TotalRecords = totalRecords,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
            };

            return Ok(new { Data = response, Pagination = paginationMetadata });
        }

        [HttpGet("GetAllHashtagDetailsforAdmin")]
        public IActionResult GetAllHashtagDetailsforAdmin(int pageNumber = 1, int pageSize = 10)
        {
            // Ensure pageNumber and pageSize have valid values
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            // Calculate the number of records to skip based on the current page
            var skip = (pageNumber - 1) * pageSize;

            var totalRecords = _context.Hashtag.Count(); // Total records count for pagination metadata

            var response = _context.Hashtag
                .GroupBy(htg => new { htg.Id, htg.UserGuid, htg.HashtagName, htg.CreatedOn, htg.HashtagCount })
                .Select(group => new
                {
                    id=group.Key.Id,
                    userid = group.Key.UserGuid,
                    HashtagName = group.Key.HashtagName,
                    CreatedOn = group.Key.CreatedOn,
                    HashtagCount = group.Key.HashtagCount
                })
                .Skip(skip)  // Skip the records based on the page number
                .Take(pageSize)  // Take the number of records for the current page
                .ToList();

            // Return the paginated result along with the pagination metadata
            var paginationMetadata = new
            {
                TotalRecords = totalRecords,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
            };

            return Ok(new { Data = response, Pagination = paginationMetadata });
        }



        [HttpGet("GetHashtagDetailsbyID")]
        public IActionResult GetHashtagDetailsbyID(string userguid, int id)
        {
            if (userguid == null)
            {
                return BadRequest(new { Message = "User GUID is required." });
            }

             var response = _context.Hashtag
                .Where(htg => htg.UserGuid == userguid && htg.Id==id)
                .GroupBy(htg => new { htg.Id,htg.UserGuid, htg.HashtagName, htg.CreatedOn,htg.HashtagCount })
                .Select(group => new
                {
                    userid = group.Key.UserGuid,
                    HashtagName = group.Key.HashtagName,
                    CreatedOn = group.Key.CreatedOn,
                    HashtagCount= group.Key.HashtagCount

                })
                .ToList();

            return Ok(response);
        }

       
        [HttpGet("GetHashtagDetailsbyIDForAdmin")]
        public IActionResult GetHashtagDetailsbyIDForAdmin(int id)
        {
            if (id == null)
            {
                return BadRequest(new { Message = "ID is required." });
            }

            var response = _context.Hashtag
               .Where(htg => htg.Id == id)
               .GroupBy(htg => new { htg.Id, htg.UserGuid, htg.HashtagName, htg.CreatedOn, htg.HashtagCount })
               .Select(group => new
               {
                   id = group.Key.Id,
                   userid = group.Key.UserGuid,
                   HashtagName = group.Key.HashtagName,
                   CreatedOn = group.Key.CreatedOn,
                   HashtagCount = group.Key.HashtagCount

               })
               .ToList();

            return Ok(response);
        }



        [HttpDelete("DeleteHashtagGroup")]
        public IActionResult DeleteHashtagGroup(string userguid,int id)
        {
            if (string.IsNullOrEmpty(userguid))
            {
                return BadRequest(new { Message = "User GUID is required." });
            }

            // Find the HashtagGroups associated with the given userguid
            var hashtagGroups = _context.HashtagGroup
                                        .Where(hg => hg.UserGuid == userguid && hg.Id ==id )
                                        .ToList();

            if (hashtagGroups.Count == 0)
            {
                return NotFound(new { Message = "No Hashtag Groups found for the given User GUID." });
            }

            // Find the Hashtags associated with these HashtagGroups
            var hashtagGroupIds = hashtagGroups.Select(hg => hg.Id).ToList();
            var hashtags = _context.Hashtag
                                   .Where(h => hashtagGroupIds.Contains(h.HashtagGroupId))
                                   .ToList();

            // Remove the Hashtags
            _context.Hashtag.RemoveRange(hashtags);

            // Remove the HashtagGroups
            _context.HashtagGroup.RemoveRange(hashtagGroups);

            _context.SaveChanges();

            return Ok(new { Message = "Hashtag Groups and associated Hashtags deleted successfully." });
        }


        [HttpPut("UpdateHashtagGroup")]
        [Authorize]
        public IActionResult UpdateHashtagGroup([FromBody] UpdateHashtagGroupRequest request)
        {
            if (string.IsNullOrEmpty(request.userGUId) ||
                string.IsNullOrEmpty(request.name) ||
                request.Hashtags == null || request.Hashtags.Count == 0)
            {
                return BadRequest(new { Message = "Fields Missing" });
            }

            // Find the existing HashtagGroup
            var hashtagGroup = _context.HashtagGroup .FirstOrDefault(hg => hg.UserGuid == request.userGUId && hg.Id==request.Id);

            if (hashtagGroup == null)
            {
                return NotFound(new { Message = "Hashtag Group not found." });
            }

            // Update HashtagGroup details
            hashtagGroup.name = request.name;        
            hashtagGroup.ModifiedOn = DateTime.UtcNow;

            // Remove existing Hashtags associated with the HashtagGroup
            var existingHashtags = _context.Hashtag
                                           .Where(h => h.HashtagGroupId == hashtagGroup.Id)
                                           .ToList();
      _context.Hashtag.RemoveRange(existingHashtags);
      //_context.Hashtag.RemoveRange(existingHashtags);

      // Add the new Hashtags
      foreach (var item in request.Hashtags)
            {
                var hashtag = new Hashtag
                {
                    HashtagGroupId = hashtagGroup.Id,
                    HashtagName = item,
                    HashtagCount = 1,
                    ModifiedOn = DateTime.Now,
                };

        _context.Hashtag.Add(hashtag);
      }

            // Save all changes
            _context.SaveChanges();

            return Ok(new { Message = "Hashtag Group updated successfully." });
        }



        [HttpPost("ScheduledPosts")]
        public async Task<IActionResult> ScheduledPosts([FromBody] CreateScheduledPostRequest request)
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
                    Title = request.Title,
                    Description = request.Description,
                    //MediaUrl = JsonConvert.SerializeObject(savedUrls),
                    MediaUrl = string.Join(",", savedUrls),
                    AccountOrGroupName = request.AccountOrGroupName,
          //AccountOrGroupId = JsonConvert.SerializeObject(request.AccountOrGroupId),
          AccountOrGroupId = request.AccountOrGroupName == "Groups" ? JsonConvert.SerializeObject(groupIds) : JsonConvert.SerializeObject(accountIds),
          //AccountOrGroupId = request.AccountOrGroupName == "Groups" ? request.AccountOrGroupId.ToString():string.Join(",", request.AccountOrGroupId.Select(e=>e.Id)),
          //PageId = string.Join(",", request.AccountOrGroupId.Select(e=>e.PageId)),
          //          createdOn = DateTime.Now,
          //          DeviceToken = _context.NotificationSetting
          //              .Where(x => x.UserGUID == request.UserGuid)
          //              .OrderByDescending(x => x.Id)
          //              .Select(x => x.DeviceToken)
          //              .FirstOrDefault(),
          PageId = request.AccountOrGroupName == "Groups" ? JsonConvert.SerializeObject(groupPageIds) : JsonConvert.SerializeObject(accountpageIds),
          Tags = JsonConvert.SerializeObject(request.Tags),
          ContentType ="Post",
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
                    Message = "Post scheduled successfully",
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

        private static readonly Dictionary<string, int> MonthAbbreviations = new()
        {
            { "January", 1 },
            { "February", 2 },
            { "March", 3 },
            { "April", 4 },
            { "May", 5 },
            { "June", 6 },
            { "July", 7 },
            { "August", 8 },
            { "September", 9 },
            { "October", 10 },
            { "November", 11 },
            { "December", 12 }
        };

        private static string GetMonthNameFromIndex(int monthIndex)
        {
            if (monthIndex < 0 || monthIndex > 11) // Ensure index is within valid range
            {
                throw new ArgumentOutOfRangeException(nameof(monthIndex), "Month index must be between 0 and 11.");
            }

            // Convert zero-based index to month name
            var monthName = new DateTime(DateTime.Now.Year, monthIndex, 1).ToString("MMMM");
            return monthName;
        }

       



        [HttpPost("SaveDraft")]
        /*[Authorize]*/
        public async Task<IActionResult> CreateDraft(CreateDraftRequest request)
        {
            if (string.IsNullOrEmpty(request.userGUId) ||
                string.IsNullOrEmpty(request.Title) ||
                string.IsNullOrEmpty(request.Description) ||
                request.MediaUrl == null || request.MediaUrl.Count == 0 ||
                request.Tags == null || request.Tags.Count == 0 ||
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

      var newPost = new Drafts
            {
                UserGuid = request.userGUId,
                Title = request.Title,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
                Status = "Draft",
                AccountOrGroupName = request.AccountOrGroupName,
        //AccountOrGroupId = string.Join(",", request.AccountOrGroupId.Select(e => e.Id)),
        //PageId = string.Join(",", request.AccountOrGroupId.Select(e => e.PageId)),
        AccountOrGroupId = request.AccountOrGroupName == "Groups" ? JsonConvert.SerializeObject(groupIds) : JsonConvert.SerializeObject(accountIds),
        PageId = request.AccountOrGroupName == "Groups" ? JsonConvert.SerializeObject(groupPageIds) : JsonConvert.SerializeObject(accountpageIds),
        PostIcon = JsonConvert.SerializeObject(request.MediaUrl),
                ContentType = "Post",
            };

            _context.Drafts.Add(newPost);
            _context.SaveChanges();

            //foreach (var accountId in request.AccountOrGroupId)
            //{
            //    if (int.TryParse(accountId, out int groupId))
            //    {
            //        var userGroupPost = new UserGroupPosts
            //        {
            //            GroupId = groupId,
            //            PostId = newPost.Id
            //        };

            //        _context.UserGroupPosts.Add(userGroupPost);
            //    }
            //}
            //_context.SaveChanges();
            // Initializing likes, shares, and views with 0 for the new post
            //var postLikes = new PostLikes
            //{
            //    PostId = newPost.Id,
            //    UserGuid = request.userGUId,
            //    PostLikesCount = 0

            //};
            //_context.PostLikes.Add(postLikes);
            //_context.SaveChanges();
            //var postShares = new PostShares
            //{
            //    PostId = newPost.Id,
            //    UserGuid = request.userGUId,
            //    PostSharesCount = 0
            //};

            //_context.PostShares.Add(postShares);
            //_context.SaveChanges();

            //var postViews = new PostViews
            //{
            //    PostId = newPost.Id,
            //    UserGuid = request.userGUId,
            //    PostViewsCount = 0
            //};

            //_context.PostViews.Add(postViews);
            //_context.SaveChanges();
            //var userSocialMediaStatus = new UserSocialMediaStatus
            //{
            //    SocialMediaId = 1,
            //    UserGuid = request.userGUId,
            //    Status = 1

            //};
            //_context.UserSocialMediaStatus.Add(userSocialMediaStatus);
            //_context.SaveChanges();



            return Ok(new { Message = "Draft created successfully" });
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

        private async Task<IFormFile> DownloadImageAsIFormFile(string imageUrl)
        {
            using (var client = new HttpClient())
            {
                // Download image as byte array
                var imageBytes = await client.GetByteArrayAsync(imageUrl);

                // Convert byte array into a stream
                var stream = new MemoryStream(imageBytes);

                // Create an IFormFile from the stream
                var fileName = Path.GetFileName(imageUrl); // Extract file name from the URL
                return new FormFile(stream, 0, stream.Length, "image", fileName)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/jpeg" // Adjust the content type based on your image type
                };
            }
        }

        private async Task<IActionResult> UploadImageAndPost([FromHeader] string accessToken,[FromForm] IFormFile image,[FromForm] string postText)
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

            return Ok();
        }


    }
}
