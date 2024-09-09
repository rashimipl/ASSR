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


        public PostController(IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment, ApplicationDbContext context, ISchedulerFactory schedulerFactory)
        {
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            Environment = environment;
            _context = context;
            _schedulerFactory = schedulerFactory;
            //FFmpeg.ExecutablesPath = @"C:\path\to\ffmpeg\bin"; // Set the path to the FFmpeg executables
        }
        /*[HttpPost]
        [Authorize]*/

        /* public IActionResult CreatePost([FromBody] CreatePostRequest request)
         {
             if (string.IsNullOrEmpty(request.userGUId) ||
                 string.IsNullOrEmpty(request.Title) ||
                 string.IsNullOrEmpty(request.Description) ||
                 request.MediaUrl == null || request.MediaUrl.Count == 0 ||
                 request.Tags == null || request.Tags.Count == 0 ||
                 string.IsNullOrEmpty(request.AccountOrGroupName) ||
                 request.AccountOrGroupId == null || request.AccountOrGroupId.Count == 0)
             {
                 return BadRequest(new CreatePostResponse
                 {
                     Message = "Fields Missing"
                 });
             }

             return Ok(new CreatePostResponse
             {
                 Message = "Post created successfully"
             });
         }*/

        //[HttpPost]
        //[Authorize]
        //public async Task<IActionResult> CreatePost(CreatePostRequest request)
        //{
        //    var processedFiles = new List<MediaFileResponse>();

        //    foreach (var formFile in request.MediaUrl)
        //    {
        //        if (formFile.Length > 0)
        //        {
        //            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
        //            if (!Directory.Exists(uploadsFolder))
        //            {
        //                Directory.CreateDirectory(uploadsFolder);
        //            }

        //            var uniqueFileName = Guid.NewGuid().ToString() + "_" + formFile.FileName;
        //            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        //            using (var stream = new FileStream(filePath, FileMode.Create))
        //            {
        //                await formFile.CopyToAsync(stream)
        //;
        //            }

        //            var thumbnailFolder = Path.Combine(uploadsFolder, "thumbnails");
        //            if (!Directory.Exists(thumbnailFolder))
        //            {
        //                Directory.CreateDirectory(thumbnailFolder);
        //            }

        //            var thumbnailFileName = "thumb_" + Path.GetFileNameWithoutExtension(uniqueFileName) + ".jpg";
        //            var thumbnailPath = Path.Combine(thumbnailFolder, thumbnailFileName);

        //            if (formFile.ContentType.StartsWith("image"))
        //            {
        //                await GenerateImageThumbnailAsync(filePath, thumbnailPath);
        //            }
        //            else if (formFile.ContentType.StartsWith("video"))
        //            {
        //                GenerateVideoThumbnailAsync(filePath, thumbnailPath);
        //            }

        //            var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{uniqueFileName}";
        //            var thumbnailUrl = $"{Request.Scheme}://{Request.Host}/uploads/thumbnails/{thumbnailFileName}";

        //            //processedFiles.Add(new MediaFileResponse
        //            //{
        //            //    FileName = uniqueFileName,
        //            //    FilePath = filePath,
        //            //    ContentType = formFile.ContentType,
        //            //    ThumbnailUrl = thumbnailUrl
        //            //});
        //            processedFiles.Add(new MediaFileResponse
        //            {
        //                FileName = thumbnailFileName,
        //                FilePath = filePath,
        //                ContentType = formFile.ContentType,
        //                ThumbnailUrl = thumbnailUrl
        //            });
        //        }
        //    }


        //    if (string.IsNullOrEmpty(request.userGUId) ||
        //        string.IsNullOrEmpty(request.Title) ||
        //        string.IsNullOrEmpty(request.Description) ||
        //        request.MediaUrl == null  ||
        //        request.Tags == null || request.Tags.Count == 0 ||
        //        string.IsNullOrEmpty(request.AccountOrGroupName) ||
        //        request.AccountOrGroupId == null || request.AccountOrGroupId.Count == 0)
        //    {
        //        return BadRequest(new { Message = "Fields Missing" });
        //    }



        //    var newPost = new SocialMediaPosts
        //    {
        //        UserGuid = request.userGUId,
        //        Title = request.Title,
        //        Description = request.Description,
        //        CreatedAt = DateTime.UtcNow,
        //        Status = "Published",
        //        AccountOrGroupName = request.AccountOrGroupName,
        //        AccountOrGroupId = JsonConvert.SerializeObject(request.AccountOrGroupId),
        //        PostIcon = JsonConvert.SerializeObject(request.MediaUrl),
        //    };

        //    _context.SocialMediaPosts.Add(newPost);
        //    _context.SaveChanges();

        //    foreach (var accountId in request.AccountOrGroupId)
        //    {
        //        if (int.TryParse(accountId, out int groupId))
        //        {
        //            var userGroupPost = new UserGroupPosts
        //            {
        //                GroupId = groupId,
        //                PostId = newPost.Id
        //            };

        //            _context.UserGroupPosts.Add(userGroupPost);

        //        }
        //    }
        //    _context.SaveChanges();
        //    // Initializing likes, shares, and views with 0 for the new post
        //    var postLikes = new PostLikes
        //    {
        //        PostId = newPost.Id,
        //        UserGuid = request.userGUId,
        //        PostLikesCount = 0

        //    };
        //    _context.PostLikes.Add(postLikes);
        //    _context.SaveChanges();
        //    var postShares = new PostShares
        //    {
        //        PostId = newPost.Id,
        //        UserGuid = request.userGUId,
        //        PostSharesCount = 0
        //    };

        //    _context.PostShares.Add(postShares);
        //    _context.SaveChanges();

        //    var postViews = new PostViews
        //    {
        //        PostId = newPost.Id,
        //        UserGuid = request.userGUId,
        //        PostViewsCount = 0
        //    };

        //    _context.PostViews.Add(postViews);
        //    _context.SaveChanges();
        //    var userSocialMediaStatus = new UserSocialMediaStatus
        //    {
        //        SocialMediaId = 1,
        //        UserGuid = request.userGUId,
        //        Status = 1

        //    };
        //    _context.UserSocialMediaStatus.Add(userSocialMediaStatus);
        //    _context.SaveChanges();



        //    return Ok(new { Message = "Post created successfully" });
        //}


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

            var newPost = new SocialMediaPosts
            {
                UserGuid = request.userGUId,
                Title = request.Title,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
                Status = "Published",
                AccountOrGroupName = request.AccountOrGroupName,
                AccountOrGroupId = JsonConvert.SerializeObject(request.AccountOrGroupId),
                PostIcon = JsonConvert.SerializeObject(request.MediaUrl),
            };

            _context.SocialMediaPosts.Add(newPost);
            _context.SaveChanges();

            foreach (var accountId in request.AccountOrGroupId)
            {
                //if (int.TryParse(accountId, out int groupId))
                //{
                //    // Check if GroupId exists in the group table
                //    bool groupExists = _context.group.Any(g => g.Id == groupId);
                //    if (groupExists)
                //    {
                //        var userGroupPost = new UserGroupPosts
                //        {
                //            GroupId = groupId,
                //            PostId = newPost.Id
                //        };

                //        _context.UserGroupPosts.Add(userGroupPost);
                //    }
                //    else
                //    {
                //        return BadRequest(new { Message = $"GroupId {groupId} does not exist." });
                //    }
                //}
                if (int.TryParse(accountId, out int groupId))
                {
                    var userGroupPost = new UserGroupPosts
                    {
                        GroupId = groupId,
                        PostId = newPost.Id
                    };

                    _context.UserGroupPosts.Add(userGroupPost);
                }
            }
            _context.SaveChanges();
            // Initializing likes, shares, and views with 0 for the new post
            var postLikes = new PostLikes
            {
                PostId = newPost.Id,
                UserGuid = request.userGUId,
                PostLikesCount = 0,
                CreatedAt= DateTime.UtcNow

            };
            _context.PostLikes.Add(postLikes);
            _context.SaveChanges();
            var postShares = new PostShares
            {
                PostId = newPost.Id,
                UserGuid = request.userGUId,
                PostSharesCount = 0,
                CreatedAt = DateTime.UtcNow
            };

            _context.PostShares.Add(postShares);
            _context.SaveChanges();

            var postViews = new PostViews
            {
                PostId = newPost.Id,
                UserGuid = request.userGUId,
                PostViewsCount = 0,
                CreatedAt = DateTime.UtcNow
            };

            _context.PostViews.Add(postViews);
            _context.SaveChanges();
            var userSocialMediaStatus = new UserSocialMediaStatus
            {
                SocialMediaId = 1,
                UserGuid = request.userGUId,
                Status = 1

            };
            _context.UserSocialMediaStatus.Add(userSocialMediaStatus);
            _context.SaveChanges();

            // Check if notifications are allowed
            var allowedNotifications = _context.Notification
                .Where(x => x.UserGuid == request.userGUId && x.Status == true && x.Name == "Remind Before 1 hours")
                .ToList();

            if (allowedNotifications.Any()) // If notifications are allowed
            {
                await SendPostCreationNotification(request.userGUId, request.deviceTokens);
            }

            return Ok(new { Message = "Post created successfully" });
        }

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



        /*[HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostRequest request)
        {
            // Validate the request fields first
            if (string.IsNullOrEmpty(request.userGUId) ||
                string.IsNullOrEmpty(request.Title) ||
                string.IsNullOrEmpty(request.Description) ||
                request.MediaUrl == null ||
                request.Tags == null || request.Tags.Count == 0 ||
                string.IsNullOrEmpty(request.AccountOrGroupName) ||
                request.AccountOrGroupId == null || request.AccountOrGroupId.Count == 0)
            {
                return BadRequest(new { Message = "Fields Missing" });
            }

            var processedFiles = new List<MediaFileResponse>();
            string postIcon = null;

            foreach (var mediaUrl in request.MediaUrl)
            {
                if (mediaUrl.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + mediaUrl.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await mediaUrl.CopyToAsync(stream);
                    }

                    var thumbnailFolder = Path.Combine(uploadsFolder, "thumbnails");
                    if (!Directory.Exists(thumbnailFolder))
                    {
                        Directory.CreateDirectory(thumbnailFolder);
                    }

                    var thumbnailFileName = "thumb_" + Path.GetFileNameWithoutExtension(uniqueFileName) + ".jpg";
                    var thumbnailPath = Path.Combine(thumbnailFolder, thumbnailFileName);

                    if (mediaUrl.ContentType.StartsWith("image"))
                    {
                        await GenerateImageThumbnailAsync(filePath, thumbnailPath);
                    }
                    else if (mediaUrl.ContentType.StartsWith("video"))
                    {
                        await GenerateVideoThumbnailAsync(filePath, thumbnailPath);
                    }

                    var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{uniqueFileName}";
                    var thumbnailUrl = $"{Request.Scheme}://{Request.Host}/uploads/thumbnails/{thumbnailFileName}";

                    processedFiles.Add(new MediaFileResponse
                    {
                        FileName = thumbnailFileName,
                        FilePath = filePath,
                        ContentType = mediaUrl.ContentType,
                        ThumbnailUrl = thumbnailUrl
                    });

                    // Use the first thumbnail as the post icon
                    if (postIcon == null)
                    {
                        postIcon = thumbnailFileName;
                    }
                }
            }

            var newPost = new SocialMediaPosts
            {
                UserGuid = request.userGUId,
                Title = request.Title,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
                Status = "Published",
                AccountOrGroupName = request.AccountOrGroupName,
                AccountOrGroupId = JsonConvert.SerializeObject(request.AccountOrGroupId),
                PostIcon = postIcon // Store the first thumbnail name
            };

            _context.SocialMediaPosts.Add(newPost);
            await _context.SaveChangesAsync();

            foreach (var accountId in request.AccountOrGroupId)
            {
                if (int.TryParse(accountId, out int groupId))
                {
                    var userGroupPost = new UserGroupPosts
                    {
                        GroupId = groupId,
                        PostId = newPost.Id
                    };

                    _context.UserGroupPosts.Add(userGroupPost);
                }
            }
            await _context.SaveChangesAsync();

            // Initializing likes, shares, and views with 0 for the new post
            var postLikes = new PostLikes
            {
                PostId = newPost.Id,
                UserGuid = request.userGUId,
                PostLikesCount = 0
            };
            _context.PostLikes.Add(postLikes);

            var postShares = new PostShares
            {
                PostId = newPost.Id,
                UserGuid = request.userGUId,
                PostSharesCount = 0
            };
            _context.PostShares.Add(postShares);

            var postViews = new PostViews
            {
                PostId = newPost.Id,
                UserGuid = request.userGUId,
                PostViewsCount = 0
            };
            _context.PostViews.Add(postViews);

            var userSocialMediaStatus = new UserSocialMediaStatus
            {
                SocialMediaId = 1,
                UserGuid = request.userGUId,
                Status = 1
            };
            _context.UserSocialMediaStatus.Add(userSocialMediaStatus);

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Post created successfully" });
        }*/



        //[HttpPost("CreatePost")]
        //[Authorize]
        //public async Task<IActionResult> CreatePost([FromForm] CreatePostRequest request)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (request.MediaFiles == null || request.MediaFiles.Count == 0)
        //    {
        //        return BadRequest(new CreatePostResponse
        //        {
        //            Message = "No media files selected"
        //        });
        //    }

        //    var processedFiles = new List<MediaFileResponse>();
        //    string postIcon = null;

        //    foreach (var formFile in request.MediaFiles)
        //    {
        //        if (formFile.Length > 0)
        //        {
        //            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
        //            if (!Directory.Exists(uploadsFolder))
        //            {
        //                Directory.CreateDirectory(uploadsFolder);
        //            }

        //            var uniqueFileName = Guid.NewGuid().ToString() + "_" + formFile.FileName;
        //            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        //            using (var stream = new FileStream(filePath, FileMode.Create))
        //            {
        //                await formFile.CopyToAsync(stream);
        //            }

        //            var thumbnailFolder = Path.Combine(uploadsFolder, "thumbnails");
        //            if (!Directory.Exists(thumbnailFolder))
        //            {
        //                Directory.CreateDirectory(thumbnailFolder);
        //            }

        //            var thumbnailFileName = "thumb_" + Path.GetFileNameWithoutExtension(uniqueFileName) + ".jpg";
        //            var thumbnailPath = Path.Combine(thumbnailFolder, thumbnailFileName);

        //            if (formFile.ContentType.StartsWith("image"))
        //            {
        //                await GenerateImageThumbnailAsync(filePath, thumbnailPath);
        //            }
        //            else if (formFile.ContentType.StartsWith("video"))
        //            {
        //                await GenerateVideoThumbnailAsync(filePath, thumbnailPath);
        //            }

        //            var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{uniqueFileName}";
        //            var thumbnailUrl = $"{Request.Scheme}://{Request.Host}/uploads/thumbnails/{thumbnailFileName}";

        //            processedFiles.Add(new MediaFileResponse
        //            {
        //                FileName = thumbnailFileName,
        //                FilePath = filePath,
        //                ContentType = formFile.ContentType,
        //                ThumbnailUrl = thumbnailUrl
        //            });

        //            // Store the thumbnail name in postIcon (assuming the first file's thumbnail is used)
        //            if (postIcon == null)
        //            {
        //                postIcon = thumbnailFileName;
        //            }
        //        }
        //    }

        //    // Save the post with the postIcon and other properties
        //    var newPost = new SocialMediaPosts
        //    {
        //        UserGuid = request.userGUId,
        //        Title = request.Title,
        //        Description = request.Description,
        //        PostIcon = postIcon,
        //        //MediaUrl = request.MediaUrl,
        //        Tags = request.Tags,
        //        AccountOrGroupName = request.AccountOrGroupName,
        //        AccountOrGroupId = request.AccountOrGroupId,
        //        // Add other properties as needed
        //    };

        //    _dbContext.Posts.Add(newPost);
        //    await _dbContext.SaveChangesAsync();

        //    return Ok(new CreatePostResponse
        //    {
        //        Message = "Post created successfully",
        //        PostId = newPost.Id,
        //        ProcessedMediaFiles = processedFiles
        //    });
        //}



        /*[HttpPost("CreatePost")]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostRequest request)
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

            var mediaUrls = new List<string>();
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            foreach (var file in request.MediaUrl)
            {
                if (file.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
                    mediaUrls.Add(fileUrl);
                }
            }

            var newPost = new SocialMediaPosts
            {
                UserGuid = request.userGUId,
                Title = request.Title,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
                Status = "Published",
                AccountOrGroupName = request.AccountOrGroupName,
                AccountOrGroupId = JsonConvert.SerializeObject(request.AccountOrGroupId),
                PostIcon = JsonConvert.SerializeObject(mediaUrls),
            };

            _context.SocialMediaPosts.Add(newPost);
            await _context.SaveChangesAsync();

            foreach (var accountId in request.AccountOrGroupId)
            {
                if (int.TryParse(accountId, out int groupId))
                {
                    var userGroupPost = new UserGroupPosts
                    {
                        GroupId = groupId,
                        PostId = newPost.Id
                    };

                    _context.UserGroupPosts.Add(userGroupPost);
                }
            }
            await _context.SaveChangesAsync();

            var postLikes = new PostLikes
            {
                PostId = newPost.Id,
                UserGuid = request.userGUId,
                PostLikesCount = 0
            };
            _context.PostLikes.Add(postLikes);

            var postShares = new PostShares
            {
                PostId = newPost.Id,
                UserGuid = request.userGUId,
                PostSharesCount = 0
            };
            _context.PostShares.Add(postShares);

            var postViews = new PostViews
            {
                PostId = newPost.Id,
                UserGuid = request.userGUId,
                PostViewsCount = 0
            };
            _context.PostViews.Add(postViews);

            var userSocialMediaStatus = new UserSocialMediaStatus
            {
                SocialMediaId = 1,
                UserGuid = request.userGUId,
                Status = 1
            };
            _context.UserSocialMediaStatus.Add(userSocialMediaStatus);

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Post created successfully" });
        }*/







        /*[HttpPost("upload")]
        [Authorize]
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
                    var filePath = Path.Combine("Uploads", formFile.FileName);

                    // Ensure the upload directory exists
                    if (!Directory.Exists("Uploads"))
                    {
                        Directory.CreateDirectory("Uploads");
                    }

                    // Save the file to the server
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }

                    // Add file details to the response
                    processedFiles.Add(new MediaFileResponse
                    {
                        FileName = formFile.FileName,
                        FilePath = filePath,
                        ContentType = formFile.ContentType
                    });
                }
            }

            return Ok(new MediaSelectionResponse
            {
                Message = "Media files uploaded successfully",
                ProcessedMediaFiles = processedFiles
            });
        }*/

        /* [HttpPost("upload")]
         [Authorize]
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

                     var uniqueFileName = Guid.NewGuid().ToString() + "_" + formFile.FileName;
                     var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                     using (var stream = new FileStream(filePath, FileMode.Create))
                     {
                         await formFile.CopyToAsync(stream);
                     }

                     var thumbnailFolder = Path.Combine(uploadsFolder, "thumbnails");
                     if (!Directory.Exists(thumbnailFolder))
                     {
                         Directory.CreateDirectory(thumbnailFolder);
                     }

                     var thumbnailFileName = "thumb_" + uniqueFileName;
                     var thumbnailPath = Path.Combine(thumbnailFolder, thumbnailFileName);
                     await GenerateThumbnailAsync(filePath, thumbnailPath);

                     var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{uniqueFileName}";
                     var thumbnailUrl = $"{Request.Scheme}://{Request.Host}/uploads/thumbnails/{thumbnailFileName}";

                     processedFiles.Add(new MediaFileResponse
                     {
                         FileName = uniqueFileName,
                         FilePath = filePath,
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
 */

        /*[HttpPost("upload")]
        [Authorize]
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

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + formFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }

                    var thumbnailFolder = Path.Combine(uploadsFolder, "thumbnails");
                    if (!Directory.Exists(thumbnailFolder))
                    {
                        Directory.CreateDirectory(thumbnailFolder);
                    }


                    var thumbnailFileName = "thumb_" + Path.GetFileNameWithoutExtension(uniqueFileName) + ".jpg";
                    var thumbnailPath = Path.Combine(thumbnailFolder, thumbnailFileName);

                    using (var stream = new FileStream(thumbnailPath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }

                    if (formFile.ContentType.StartsWith("image"))
                    {
                        GenerateImageThumbnailAsync(filePath, thumbnailPath);
                    }
                    else if (formFile.ContentType.StartsWith("video"))
                    {

                        GenerateVideoThumbnailAsync(filePath, thumbnailPath);
                    }

                    var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{uniqueFileName}";
                    var thumbnailUrl = $"{Request.Scheme}://{Request.Host}/uploads/thumbnails/{thumbnailFileName}";

                    processedFiles.Add(new MediaFileResponse
                    {
                        FileName = uniqueFileName,
                        FilePath = filePath,
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

                FFmpeg.SetExecutablesPath(Path.Combine(_webHostEnvironment.WebRootPath, "ffmpeg", "bin"));
                var conversion = await FFmpeg.Conversions.FromSnippet.Snapshot(videoPath, thumbnailPath, TimeSpan.FromSeconds(1));
                await conversion.Start();
            }
            catch (Exception ex)
            {
                throw;
            }
            try
            {
                // Extract a frame from the video and save it as an image
                IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(videoPath);
                IConversion conversion = FFmpeg.Conversions.New()
                    .AddStream(mediaInfo.VideoStreams.First())
                    .SetSeek(TimeSpan.FromSeconds(1))
                    .AddParameter("-vframes 1") // Extract only one frame
                    .SetOutput(thumbnailPath);
                await conversion.Start();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }*/


        /*public async Task<string> GenerateThumbnailAsync(string imagePath, string thumbnailPath)
        {
            using (ImageSharp.Image image = await ImageSharp.Image.LoadAsync(imagePath))
            {
                image.Mutate(x => x.Resize(new ImageSharp.Processing.ResizeOptions
                {
                    Mode = ImageSharp.Processing.ResizeMode.Crop,
                    Size = new ImageSharp.Size(100, 100) // Set your desired thumbnail size here
                }));

                await image.SaveAsync(thumbnailPath, new JpegEncoder()); // Save as JPEG or any format you prefer
            }

            return thumbnailPath;
        }*/


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

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + formFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream)
        ;
                    }

                    var thumbnailFolder = Path.Combine(uploadsFolder, "thumbnails");
                    if (!Directory.Exists(thumbnailFolder))
                    {
                        Directory.CreateDirectory(thumbnailFolder);
                    }

                    var thumbnailFileName = "thumb_" + Path.GetFileNameWithoutExtension(uniqueFileName) + ".jpg";
                    var thumbnailPath = Path.Combine(thumbnailFolder, thumbnailFileName);

                    if (formFile.ContentType.StartsWith("image"))
                    {
                        await GenerateImageThumbnailAsync(filePath, thumbnailPath);
                    }
                    else if (formFile.ContentType.StartsWith("video"))
                    {
                        GenerateVideoThumbnailAsync(filePath, thumbnailPath);
                    }

                    var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{uniqueFileName}";
                    var thumbnailUrl = $"{Request.Scheme}://{Request.Host}/uploads/thumbnails/{thumbnailFileName}";

                    //processedFiles.Add(new MediaFileResponse
                    //{
                    //    FileName = uniqueFileName,
                    //    FilePath = filePath,
                    //    ContentType = formFile.ContentType,
                    //    ThumbnailUrl = thumbnailUrl
                    //});
                    processedFiles.Add(new MediaFileResponse
                    {
                        FileName = thumbnailFileName,
                        FilePath = filePath,
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
                ffMpeg.GetVideoThumbnail(videoPath, thumbnailPath, 1); // Capture frame at 1 second
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
        [Authorize]
        public IActionResult CreateHashtagGroup([FromBody] CreateHashtagGroupRequest request)
        {
            if (string.IsNullOrEmpty(request.userGUId) ||
                string.IsNullOrEmpty(request.HashtagGroupName) ||
                request.Hashtags == null || request.Hashtags.Count == 0)
            {
                return BadRequest(new CreateHashtagGroupResponse
                {
                    Message = "Fields Missing"
                });
            }

            var hashtagGroup = new HashtagGroup
            {
                UserGuid = request.userGUId,
                HashtagGroupName = request.HashtagGroupName,
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
                    HashtagCount =1000,
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
                .GroupBy(htg => new { htg.Id, htg.HashtagGroupName, htg.CreatedOn })
                .Select(group => new
                {
                    HashtagGroupName = group.Key.HashtagGroupName,
                    Titles = string.Join(", ", group.SelectMany(g => _context.Hashtag.Where(tg => tg.HashtagGroupId == group.Key.Id).Select(tg => tg.HashtagName))),
                    CreatedOn = group.Key.CreatedOn
                })
                .ToList();

            return Ok(response);
        }

        [HttpGet("GetAllHashtagGroupDetails")]
        public IActionResult AllHashtagGroupDetails()
        {
            var response = _context.HashtagGroup
                .GroupBy(htg => new { htg.Id, htg.HashtagGroupName, htg.CreatedOn })
                .Select(group => new
                {
                    HashtagGroupName = group.Key.HashtagGroupName,
                    Titles = string.Join(", ", group.SelectMany(g => _context.Hashtag.Where(tg => tg.HashtagGroupId == group.Key.Id).Select(tg => tg.HashtagName))),
                    CreatedOn = group.Key.CreatedOn
                })
                .ToList();

            return Ok(response);
        }

        [HttpGet("GetAllHashtagDetails")]
        public IActionResult GetAllHashtagDetails()
        {
            var response = _context.Hashtag
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
                string.IsNullOrEmpty(request.HashtagGroupName) ||
                request.Hashtags == null || request.Hashtags.Count == 0)
            {
                return BadRequest(new { Message = "Fields Missing" });
            }

            // Find the existing HashtagGroup
            var hashtagGroup = _context.HashtagGroup
                                       .FirstOrDefault(hg => hg.UserGuid == request.userGUId && hg.Id==request.GroupId);

            if (hashtagGroup == null)
            {
                return NotFound(new { Message = "Hashtag Group not found." });
            }

            // Update HashtagGroup details
            hashtagGroup.HashtagGroupName = request.HashtagGroupName;
            hashtagGroup.CreatedOn = DateTime.UtcNow;

            // Remove existing Hashtags associated with the HashtagGroup
            var existingHashtags = _context.Hashtag
                                           .Where(h => h.HashtagGroupId == hashtagGroup.Id)
                                           .ToList();

            _context.Hashtag.RemoveRange(existingHashtags);

            // Add the new Hashtags
            foreach (var item in request.Hashtags)
            {
                var hashtag = new Hashtag
                {
                    HashtagGroupId = hashtagGroup.Id,
                    HashtagName = item,
                    CreatedOn = DateTime.UtcNow,
                };

                _context.Hashtag.Add(hashtag);
            }

            // Save all changes
            _context.SaveChanges();

            return Ok(new { Message = "Hashtag Group updated successfully." });
        }


        /*[HttpPost("ScheduledPosts")]
        public IActionResult ScheduledPosts([FromBody] CreateScheduledPostRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = _context.Users
                    .FirstOrDefault(x => x.Id == request.UserGuid);
                if (user == null)
                {
                    return NotFound("User not found");
                }

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
                    IsPublished = request.IsPublished
                };

                _context.ScheduledPost.Add(res);
                _context.SaveChanges();

                return Ok(new
                {
                    Status = "True",
                    Message = "Post Scheduled successfully",
                    Data = res
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "False",
                    Message = "An error occurred while scheduling the post",
                    Error = ex.Message
                });
            }
        }*/

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
                var postdetails = _context.SocialMediaPosts.FirstOrDefault(x => x.UserGuid == request.UserGuid);
                if (user == null)
                {
                    return NotFound("User not found");
                }

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
                    Title = postdetails.Title,
                    Description = postdetails.Description,
                    MediaUrl = postdetails.PostIcon,
                    AccountOrGroupName = postdetails.AccountOrGroupName,
                    AccountOrGroupId = postdetails.AccountOrGroupId,
                    createdOn = DateTime.Now,
                    DeviceToken = request.DeviceToken,
                    Tags = "abcd"
                };

                _context.ScheduledPost.Add(res);
                await _context.SaveChangesAsync();

                // Schedule the notification job
                var scheduler = await _schedulerFactory.GetScheduler();

                // Schedule based on the scenario
                if (res.ScheduledType == "OneTime")
                {
                    // Scenario 1: ScheduledDate and ScheduledTime
                    foreach (var scheduledDate in res.ScheduledDate.Split(','))
                    {
                        if (DateTime.TryParse($"{res.ScheduledDate} {res.ScheduledTime}", out DateTime scheduledDateTime))
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
                        }
                    }
                }
                else if (res.ScheduledType == "Weekly")
                {
                    // Scenario 2: Days, ScheduledTime, FromDate, and ToDate
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
                    // Scenario 3: Months, ScheduledDate, and ScheduledTime
                    foreach (var scheduledDate in res.ScheduledDate.Split(','))
                    {
                        // Parse the scheduled date into a DateTime object
                        if (DateTime.TryParseExact(scheduledDate.Trim(), "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime scheduledDateTime))
                        {
                            int year = scheduledDateTime.Year;
                            int month = scheduledDateTime.Month; // Extract the month from the scheduled date
                            int day = scheduledDateTime.Day; // Extract the day from the scheduled date

                            // Construct the full datetime string with the scheduled time
                            string dateTimeString = $"{year}-{month:D2}-{day:D2} {res.ScheduledTime}";

                            // Now you can use this dateTimeString for scheduling your job or other logic
                        }
                        else
                        {
                            throw new Exception($"Invalid date format in ScheduledDate: {scheduledDate}");
                        }
                    }
                }






                //else if (res.ScheduledType == "Monthly")
                //{
                //    // Scenario 3: Months, ScheduledDate, and ScheduledTime
                //    foreach (var month in res.Months.Split(','))
                //    {
                //        if (int.TryParse(month, out int monthIndex))
                //        {

                //            // Convert zero-based index to month name
                //            string monthName = GetMonthNameFromIndex(monthIndex);

                //            // Get the month number from the month name
                //            if (MonthAbbreviations.TryGetValue(monthName, out int monthNumber))
                //            {
                //                var year = DateTime.Now.Year;
                //                if (DateTime.TryParse($"{year}-{monthNumber:D2}-{res.ScheduledDate} {res.ScheduledTime}", out DateTime scheduledDateTime))
                //                {
                //                    var jobKey = new JobKey($"NotificationJob-{res.Id}-{scheduledDateTime:yyyyMMddHHmmss}", "NotificationGroup");
                //                    var job = JobBuilder.Create<NotificationJob>()
                //                                        .WithIdentity(jobKey)
                //                                        .UsingJobData("ScheduledPostId", res.Id)
                //                                        .UsingJobData("UserGuid", res.UserGuid)
                //                                        .Build();

                //                    var trigger = TriggerBuilder.Create()
                //                                                .StartAt(scheduledDateTime.AddHours(-1)) // 1 hour before the scheduled time
                //                                                .ForJob(jobKey)
                //                                                .Build();
                //                    await scheduler.ScheduleJob(job, trigger);
                //                }
                //            }
                //        }  
                //    }
                //}
                else
                {
                    throw new Exception("Invalid scheduling data.");
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

        //11[HttpPost("ScheduledPosts")]
        //public async Task<IActionResult> ScheduledPosts([FromBody] CreateScheduledPostRequest request)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }

        //        var user = _context.Users.FirstOrDefault(x => x.Id == request.UserGuid);
        //        var postdetails = _context.SocialMediaPosts.FirstOrDefault(x => x.UserGuid == request.UserGuid);
        //        if (user == null)
        //        {
        //            return NotFound("User not found");
        //        }

        //        ScheduledPost res = new ScheduledPost
        //        {
        //            UserGuid = request.UserGuid,
        //            ScheduledType = request.ScheduledType == "1" ? "OneTime" :
        //                            request.ScheduledType == "2" ? "Weekly" :
        //                            request.ScheduledType == "3" ? "Monthly" : " ",
        //            Days = request.Days != null ? string.Join(",", request.Days) : null,
        //            Months = request.Months != null ? string.Join(",", request.Months) : null,
        //            ScheduledTime = request.ScheduledTime,
        //            ScheduledDate = request.ScheduledDate != null ? string.Join(",", request.ScheduledDate) : null,
        //            FromDate = !string.IsNullOrEmpty(request.FromDate) ? request.FromDate : null,
        //            ToDate = !string.IsNullOrEmpty(request.ToDate) ? request.ToDate : null,
        //            IsPublished = request.IsPublished,
        //            Title = postdetails.Title,
        //            Description = postdetails.Description,
        //            MediaUrl = postdetails.PostIcon,
        //            AccountOrGroupName = postdetails.AccountOrGroupName,
        //            AccountOrGroupId = postdetails.AccountOrGroupId,
        //            createdOn = DateTime.Now,
        //            DeviceToken = request.DeviceToken,
        //            Tags = "abcd"
        //        };

        //        _context.ScheduledPost.Add(res);
        //        await _context.SaveChangesAsync();

        //        // Schedule the notification job
        //        var scheduler = await _schedulerFactory.GetScheduler();
        //        var job = JobBuilder.Create<NotificationJob>()
        //                            .UsingJobData("ScheduledPostId", res.Id)
        //                            .UsingJobData("UserGuid", res.UserGuid) // Pass additional data if needed
        //                            .Build();

        //        //// Check if the job already exists; if not, add it to the scheduler
        //        //if (!await scheduler.CheckExists(job.Key))
        //        //{
        //        //    await scheduler.AddJob(job, true);
        //        //}

        //        // Parse the ScheduledTime string to a DateTime
        //        if (DateTime.TryParse(res.ScheduledTime, out DateTime scheduledTime))
        //        {
        //            var trigger = TriggerBuilder.Create()
        //                                        .StartAt(scheduledTime.AddHours(-1)) // 1 hour before the scheduled time
        //                                        .Build();

        //            await scheduler.ScheduleJob(job, trigger);
        //        }
        //        else
        //        {
        //            // Handle the case where ScheduledTime cannot be parsed
        //            throw new Exception("Scheduled time is not in a valid format.");
        //        }

        //        return Ok(new
        //        {
        //            Status = "True",
        //            Message = "Post Scheduled successfully",
        //            Data = res
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            Status = "False",
        //            Message = "An error occurred while scheduling the post",
        //            Error = ex,
        //            newerr = ex.InnerException
        //        });
        //    }
        //}


        //[HttpPost("ScheduledPosts")]
        //public async Task<IActionResult> ScheduledPosts([FromBody] CreateScheduledPostRequest request)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }

        //        var user = _context.Users.FirstOrDefault(x => x.Id == request.UserGuid);
        //        var postdetails = _context.SocialMediaPosts.FirstOrDefault(x => x.UserGuid == request.UserGuid);
        //        if (user == null)
        //        {
        //            return NotFound("User not found");
        //        }

        //        ScheduledPost res = new ScheduledPost
        //        {
        //            UserGuid = request.UserGuid,
        //            ScheduledType = request.ScheduledType == "1" ? "OneTime" :
        //                            request.ScheduledType == "2" ? "Weekly" :
        //                            request.ScheduledType == "3" ? "Monthly" : " ",
        //            Days = request.Days != null ? string.Join(",", request.Days) : null,
        //            Months = request.Months != null ? string.Join(",", request.Months) : null,
        //            ScheduledTime = request.ScheduledTime,
        //            ScheduledDate = request.ScheduledDate != null ? string.Join(",", request.ScheduledDate) : null,
        //            FromDate = !string.IsNullOrEmpty(request.FromDate) ? request.FromDate : null,
        //            ToDate = !string.IsNullOrEmpty(request.ToDate) ? request.ToDate : null,
        //            IsPublished = request.IsPublished,
        //            Title = postdetails.Title,
        //            Description = postdetails.Description,
        //            MediaUrl = postdetails.PostIcon,
        //            AccountOrGroupName = postdetails.AccountOrGroupName,
        //            AccountOrGroupId = postdetails.AccountOrGroupId,
        //            createdOn = DateTime.Now,
        //            Tags = "abcd"
        //        };

        //        _context.ScheduledPost.Add(res);
        //        _context.SaveChanges();

        //        // Schedule the notification job
        //        var scheduler = await _schedulerFactory.GetScheduler();
        //        var job = JobBuilder.Create<NotificationJob>()
        //                            .UsingJobData("ScheduledPostId", res.Id)
        //                            .Build();

        //        // Parse the ScheduledTime string to a DateTime
        //        if (DateTime.TryParse(res.ScheduledTime, out DateTime scheduledTime))
        //        {
        //            var trigger = TriggerBuilder.Create()
        //                                        .StartAt(scheduledTime.AddHours(-1)) // 1 hour before the scheduled time
        //                                        .Build();

        //            await scheduler.ScheduleJob(job, trigger);
        //        }
        //        else
        //        {
        //            // Handle the case where ScheduledTime cannot be parsed
        //            throw new Exception("Scheduled time is not in a valid format.");
        //        }

        //        //var scheduler = await _schedulerFactory.GetScheduler();
        //        //var job = JobBuilder.Create<NotificationJob>()
        //        //                    .UsingJobData("ScheduledPostId", res.Id)
        //        //                    .Build();

        //        //var trigger = TriggerBuilder.Create()
        //        //                            .StartAt(res.ScheduledTime.AddHours(-1)) // 1 hour before the scheduled time
        //        //                            .Build();

        //        //await scheduler.ScheduleJob(job, trigger);

        //        return Ok(new
        //        {
        //            Status = "True",
        //            Message = "Post Scheduled successfully",
        //            Data = res
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            Status = "False",
        //            Message = "An error occurred while scheduling the post",
        //            Error = ex,
        //            newerr = ex.InnerException
        //        });
        //    }
        //}

        //public IActionResult ScheduledPosts([FromBody] CreateScheduledPostRequest request)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }

        //        var user = _context.Users.FirstOrDefault(x => x.Id == request.UserGuid);
        //        var postdetails = _context.SocialMediaPosts.FirstOrDefault(x => x.UserGuid == request.UserGuid);
        //        if (user == null)
        //        {
        //            return NotFound("User not found");
        //        }

        //        ScheduledPost res = new ScheduledPost
        //        {
        //            UserGuid = request.UserGuid,
        //            ScheduledType = request.ScheduledType == "1" ? "OneTime" :
        //                            request.ScheduledType == "2" ? "Weekly" :
        //                            request.ScheduledType == "3" ? "Monthly" : " ",
        //            Days = request.Days != null ? string.Join(",", request.Days) : null,
        //            Months = request.Months != null ? string.Join(",", request.Months) : null,
        //            ScheduledTime = request.ScheduledTime,
        //            ScheduledDate = request.ScheduledDate != null ? string.Join(",", request.ScheduledDate) : null,
        //            FromDate = !string.IsNullOrEmpty(request.FromDate) ? request.FromDate : null,
        //            ToDate = !string.IsNullOrEmpty(request.ToDate) ? request.ToDate : null,
        //            IsPublished = request.IsPublished,
        //            Title = postdetails.Title,
        //            Description = postdetails.Description,
        //            MediaUrl = postdetails.PostIcon,
        //            AccountOrGroupName = postdetails.AccountOrGroupName,
        //            AccountOrGroupId = postdetails.AccountOrGroupId,
        //            createdOn = DateTime.Now,
        //            Tags = "abcd"
        //        };

        //        _context.ScheduledPost.Add(res);
        //        _context.SaveChanges();

        //        return Ok(new
        //        {
        //            Status = "True",
        //            Message = "Post Scheduled successfully",
        //            Data = res
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            Status = "False",
        //            Message = "An error occurred while scheduling the post",
        //            Error = ex,
        //            newerr = ex.InnerException
        //        });
        //    }
        //}




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

            var newPost = new Drafts
            {
                UserGuid = request.userGUId,
                Title = request.Title,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
                Status = "Draft",
                AccountOrGroupName = request.AccountOrGroupName,
                AccountOrGroupId = JsonConvert.SerializeObject(request.AccountOrGroupId),
                PostIcon = JsonConvert.SerializeObject(request.MediaUrl),
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

    }
}
