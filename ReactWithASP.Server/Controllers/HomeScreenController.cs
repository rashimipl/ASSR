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
using ReactWithASP.Server.Models.Posts;
using Newtonsoft.Json;
using SixLabors.ImageSharp.ColorSpaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;



namespace ReactWithASP.Server.Controllers
{
    [Route("api")]
    [ApiController]
    public class HomeScreenController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeScreenController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /*[HttpGet("Posts")]
        [Authorize]

        public IActionResult Posts([FromQuery] HomeScreenRequest request)
        {
            var response = new List<PostResponse>
    {
        new PostResponse
        {
            Group = new Groups
             {
                 Name = "Evolution",
                 //Platform = new List<string> { "linkedin", "twitter" },
                 groupIcon = "https://thumbs.dreamstime.com/b/teamwork-group-friends-icon-vector-illustration-teamwork-group-friends-icon-118637039.jpg"
             },

            Title = "Join Our Webinar on AI",
            Description = "We are hosting a webinar on the latest advancements in AI technology.",
            PostIcon = "https://www.shutterstock.com/image-photo/earth-day-environment-green-globe-260nw-2285503199.jpg",
            Id = 101,
            Likes = 1500,
            Views = 3000,
            Comments = 350,
            Shares = 500,
            Status = "Published",
            StatusCode = 1
        },
        new PostResponse
        {
            Group = new Groups
            {
                Name = "Beautiful Life",
                //Platform = new List<string> { "facebook", "instagram" },
                groupIcon = "https://thumbs.dreamstime.com/b/teamwork-group-friends-icon-vector-illustration-teamwork-group-friends-icon-118637039.jpg"
            },
            Title = "5 Tips for a Healthier Lifestyle",
            Description = "Learn how to maintain a healthy lifestyle with these 5 simple tips.",
            PostIcon = "https://www.shutterstock.com/image-photo/earth-day-environment-green-globe-260nw-2285503199.jpg",
            Id = 102,
            Likes = 2000,
            Views = 4000,
            Comments = 450,
            Shares = 600,
            Status = "Published",
            StatusCode = 1
        }
    };

            // Apply the NoOfPosts filter if provided
            if (request.NoOfPosts.HasValue)
            {
                response = response.Take(request.NoOfPosts.Value).ToList();
            }

            return Ok(response);
        }
*/


        /*[HttpGet("Posts")]
        public IActionResult Posts([FromQuery] RRequest request)
        {
            *//*if (!int.TryParse(request.Days, out int days))
            {
                return BadRequest(new { Status = "Error", Message = "Invalid value for days." });
            }*//*

            // Calculate the date range
            *//*var endDate = DateTime.UtcNow.Date; // Current UTC date
            var startDate = endDate.AddDays(-days); // Start date is endDate minus days*//*
            try
            {
                var query = (from smp in _context.SocialMediaPosts
                             join ugp in _context.UserGroupPosts on smp.Id equals ugp.PostId
                             join g in _context.@group on ugp.GroupId equals g.Id
                             join gsm in _context.GroupSocialMedia on g.Id equals gsm.Id
                             join sm in _context.SocialMedia on gsm.SocialMediaId equals sm.Id
                             join pl in _context.PostLikes on smp.Id equals pl.PostId
                             join pv in _context.PostViews on smp.Id equals pv.PostId
                             join ps in _context.PostShares on smp.Id equals ps.PostId
                             where smp.UserGuid == request.UserGuid *//*&& smp.CreatedAt >= startDate && smp.CreatedAt <= endDate*//*
                             select new PostResponse1
                             {

                                 Title = smp.Title,
                                 Description = smp.Description,
                                 PostIcon = JsonConvert.DeserializeObject<List<string>>(smp.PostIcon).FirstOrDefault(),
                                 CreatedAt = smp.CreatedAt,
                                 Id = smp.Id,
                                 Likes = pl != null ? pl.PostLikesCount : 0,
                                 Shares = ps != null ? ps.PostSharesCount : 0,
                                 Views = pv != null ? pv.PostViewsCount : 0,
                                 Group = ugp != null && g != null ? new GroupResponse
                                 {
                                     Id = g.Id,
                                     UserGuid = g.UserGuid,
                                     Name = g.Name,
                                     GroupIcon = g.GroupIcon,
                                     //Platform = new string[] { sm.SocialMediaName },
                                 } : null,
                                 Status = smp.Status,

                             };

                var querylist = query.ToList();
                if (query.Count() == 0)
                {
                    return NotFound(new { Status = "Error", Message = "User Not Found" });
                }

            }
            catch (Exception ex)
            {

                throw;
            }
            return Ok();

        }*/


        /*  [HttpGet("Posts")]
          public IActionResult Posts([FromQuery] RRequest request)
          {
              try
              {
                  var query = (from smp in _context.SocialMediaPosts
                               join ugp in _context.UserGroupPosts on smp.Id equals ugp.PostId
                               join g in _context.@group on ugp.GroupId equals g.Id
                               join gsm in _context.GroupSocialMedia on g.Id equals gsm.GroupId
                               join sm in _context.SocialMedia on gsm.SocialMediaId equals sm.Id
                               join pl in _context.PostLikes on smp.Id equals pl.PostId into plGroup
                               from pl in plGroup.DefaultIfEmpty()
                               join pv in _context.PostViews on smp.Id equals pv.PostId into pvGroup
                               from pv in pvGroup.DefaultIfEmpty()
                               join ps in _context.PostShares on smp.Id equals ps.PostId into psGroup
                               from ps in psGroup.DefaultIfEmpty()
                               where smp.UserGuid == request.UserGuid
                               select new PostResponse1
                               {
                                   Title = smp.Title,
                                   Description = smp.Description,
                                   PostIcon = JsonConvert.DeserializeObject<List<string>>(smp.PostIcon).FirstOrDefault(),
                                   CreatedAt = smp.CreatedAt,
                                   Id = smp.Id,
                                   Likes = pl != null ? pl.PostLikesCount : 0,
                                   Shares = ps != null ? ps.PostSharesCount : 0,
                                   Views = pv != null ? pv.PostViewsCount : 0,
                                   Group = ugp != null && g != null ? new GroupResponse
                                   {
                                       Id = g.Id,
                                       UserGuid = g.UserGuid,
                                       Name = g.Name,
                                       GroupIcon = g.GroupIcon,
                                       // Platform = new string[] { sm.SocialMediaName },
                                   } : null,
                                   Status = smp.Status,
                               }).ToList();

                  *//*var groupedQuery = query.GroupBy(x => x.Id)
                                 .Select(g => new
                                 {
                                     Group = g.First().Group,
                                     Posts = g.Select(x => x.Group).ToList()
                                 }).ToList();*//*

                  if (query.Count == 0)
                  {
                      return NotFound(new { Status = "Error", Message = "User Not Found" });
                  }

                  return Ok(query);
              }
              catch (Exception ex)
              {
                  // Log the exception
                  return StatusCode(500, new { Status = "Error", Message = ex.Message });
              }
          }*/

        /*[HttpGet("Posts")]
        public IActionResult Posts([FromQuery] RRequest request)
        {
            try
            {
                var query = (from smp in _context.SocialMediaPosts
                             join ugp in _context.UserGroupPosts on smp.Id equals ugp.PostId
                             join g in _context.@group on ugp.GroupId equals g.Id
                             join gsm in _context.GroupSocialMedia on g.Id equals gsm.GroupId
                             join sm in _context.SocialMedia on gsm.SocialMediaId equals sm.Id
                             join pl in _context.PostLikes on smp.Id equals pl.PostId into plGroup
                             from pl in plGroup.DefaultIfEmpty()
                             join pv in _context.PostViews on smp.Id equals pv.PostId into pvGroup
                             from pv in pvGroup.DefaultIfEmpty()
                             join ps in _context.PostShares on smp.Id equals ps.PostId into psGroup
                             from ps in psGroup.DefaultIfEmpty()
                             where smp.UserGuid == request.UserGuid
                             select new PostResponse1
                             {
                                 Title = smp.Title,
                                 Description = smp.Description,
                                 PostIcon = JsonConvert.DeserializeObject<List<string>>(smp.PostIcon)
                                      .Select(fileName => GenerateServerPathUrl(fileName)).FirstOrDefault(),
                                 CreatedAt = smp.CreatedAt,
                                 Id = smp.Id,
                                 Likes = pl != null ? pl.PostLikesCount : 0,
                                 Shares = ps != null ? ps.PostSharesCount : 0,
                                 Views = pv != null ? pv.PostViewsCount : 0,
                                 Status = smp.Status,
                                 Group = new GroupResponse
                                 {
                                     Id = g.Id,
                                     UserGuid = g.UserGuid,
                                     Name = g.Name,
                                     GroupIcon = g.GroupIcon,
                                      Platform = new string[] { sm.SocialMediaName },
                                 }
                             }).ToList();

                if (query.Count == 0)
                {
                    return NotFound(new { Status = "Error", Message = "User Not Found" });
                }

                return Ok(query);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { Status = "Error", Message = ex.Message });
            }
        }

        private string GenerateServerPathUrl(string fileName)
        {
            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            return $"{baseUrl}/path-to-your-images/{fileName}";
        }*/


        /*[HttpGet("Posts")]
        public IActionResult Posts([FromQuery] RRequest request)
        {
            try
            {
                var query = (from smp in _context.SocialMediaPosts
                             join ugp in _context.UserGroupPosts on smp.Id equals ugp.PostId
                             join g in _context.@group on ugp.GroupId equals g.Id
                             join gsm in _context.GroupSocialMedia on g.Id equals gsm.GroupId
                             join sm in _context.SocialMedia on gsm.SocialMediaId equals sm.Id
                             join pl in _context.PostLikes on smp.Id equals pl.PostId into plGroup
                             from pl in plGroup.DefaultIfEmpty()
                             join pv in _context.PostViews on smp.Id equals pv.PostId into pvGroup
                             from pv in pvGroup.DefaultIfEmpty()
                             join ps in _context.PostShares on smp.Id equals ps.PostId into psGroup
                             from ps in psGroup.DefaultIfEmpty()
                             where smp.UserGuid == request.UserGuid
                             select new
                             {
                                 smp.Title,
                                 smp.Description,
                                 smp.PostIcon,
                                 smp.CreatedAt,
                                 smp.Id,
                                 Likes = pl != null ? pl.PostLikesCount : 0,
                                 Shares = ps != null ? ps.PostSharesCount : 0,
                                 Views = pv != null ? pv.PostViewsCount : 0,
                                 smp.Status,
                                 Group = new
                                 {
                                     g.Id,
                                     g.UserGuid,
                                     g.Name,
                                     g.GroupIcon,
                                     Platform = new string[] { sm.SocialMediaName }
                                 }
                             }).ToList();

                if (query.Count == 0)
                {
                    return NotFound(new { Status = "Error", Message = "User Not Found" });
                }

                // Process the data in memory to generate the full URLs
                var response = query.Select(q => new PostResponse1
                {
                    Title = q.Title,
                    Description = q.Description,
                    PostIcon = JsonConvert.DeserializeObject<List<string>>(q.PostIcon)
                                  .Select(fileName => GenerateServerPathUrl(fileName)).FirstOrDefault(),
                    CreatedAt = q.CreatedAt,
                    Id = q.Id,
                    Likes = q.Likes,
                    Shares = q.Shares,
                    Views = q.Views,
                    Status = q.Status,
                    Group = new GroupResponse
                    {
                        Id = q.Group.Id,
                        UserGuid = q.Group.UserGuid,
                        Name = q.Group.Name,
                        GroupIcon = q.Group.GroupIcon,
                        Platform = q.Group.Platform
                    }
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { Status = "Error", Message = ex.Message });
            }
        }

        private string GenerateServerPathUrl(string fileName)
        {
            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            return $"{baseUrl}/uploads/{fileName}";
        }
*/


        [HttpGet("Posts")]
        public IActionResult Posts([FromQuery] RRequest request)
        {
            try
            {
                // Step 1: Fetch all necessary data from the database and group by group ID
                var query = (from smp in _context.SocialMediaPosts
                             join ugp in _context.UserGroupPosts on smp.Id equals ugp.PostId
                             join g in _context.@group on ugp.GroupId equals g.Id
                             join gsm in _context.GroupSocialMedia on g.Id equals gsm.GroupId
                             join sm in _context.SocialMedia on gsm.SocialMediaId equals sm.Id
                             join pl in _context.PostLikes on smp.Id equals pl.PostId into plGroup
                             from pl in plGroup.DefaultIfEmpty()
                             join pv in _context.PostViews on smp.Id equals pv.PostId into pvGroup
                             from pv in pvGroup.DefaultIfEmpty()
                             join ps in _context.PostShares on smp.Id equals ps.PostId into psGroup
                             from ps in psGroup.DefaultIfEmpty()
                             where smp.UserGuid == request.UserGuid
                             select new
                             {
                                 smp.Title,
                                 smp.Description,
                                 smp.PostIcon,
                                 smp.CreatedAt,
                                 smp.Id,
                                 Likes = pl != null ? pl.PostLikesCount : 0,
                                 Shares = ps != null ? ps.PostSharesCount : 0,
                                 Views = pv != null ? pv.PostViewsCount : 0,
                                 smp.Status,
                                 GroupId = g.Id,
                                 g.UserGuid,
                                 g.Name,
                                 g.GroupIcon,
                                 sm.SocialMediaName
                             }).ToList();

                if (query.Count == 0)
                {
                    return Ok(new { Status = "true", data = new List<PostResponse1>() });
                }

                // Step 2: Group the data by post and group ID, and aggregate the platforms
                var response = query
                    .GroupBy(q => new { q.Id, q.GroupId })
                    .Select(g => new PostResponse1
                    {
                        Title = g.First().Title,
                        Description = g.First().Description,
                        PostIcon = JsonConvert.DeserializeObject<List<string>>(g.First().PostIcon)
                                      .Select(fileName => GenerateServerPathUrl(fileName)).FirstOrDefault(),
                        CreatedAt = g.First().CreatedAt,
                        Id = g.First().Id,
                        Likes = g.Sum(x => x.Likes),
                        Shares = g.Sum(x => x.Shares),
                        Views = g.Sum(x => x.Views),
                        Status = g.First().Status,
                        Group = new GroupResponse
                        {
                            Id = g.First().GroupId,
                            UserGuid = g.First().UserGuid,
                            Name = g.First().Name,
                            GroupIcon = g.First().GroupIcon,
                            Platform = g.Select(x => x.SocialMediaName).Distinct().ToArray()
                        }
                    }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { Status = "Error", Message = ex.Message });
            }
        }

        private string GenerateServerPathUrl(string fileName)
        {
            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            //return $"{baseUrl}/uploads/{fileName}";
            return $"{baseUrl}/uploads/thumbnails/{fileName}";
        }


        /*[HttpGet("Posts")]
        public IActionResult Posts([FromQuery] RRequest request)
        {
            try
            {
                // Step 1: Fetch all necessary data from the database and group by group ID
                var query = (from smp in _context.SocialMediaPosts
                             join ugp in _context.UserGroupPosts on smp.Id equals ugp.PostId
                             join g in _context.@group on ugp.GroupId equals g.Id
                             join gsm in _context.GroupSocialMedia on g.Id equals gsm.GroupId
                             join sm in _context.SocialMedia on gsm.SocialMediaId equals sm.Id
                             join pl in _context.PostLikes on smp.Id equals pl.PostId into plGroup
                             from pl in plGroup.DefaultIfEmpty()
                             join pv in _context.PostViews on smp.Id equals pv.PostId into pvGroup
                             from pv in pvGroup.DefaultIfEmpty()
                             join ps in _context.PostShares on smp.Id equals ps.PostId into psGroup
                             from ps in psGroup.DefaultIfEmpty()
                             where smp.UserGuid == request.UserGuid
                             select new
                             {
                                 smp.Title,
                                 smp.Description,
                                 smp.PostIcon,
                                 smp.CreatedAt,
                                 smp.Id,
                                 Likes = pl != null ? pl.PostLikesCount : 0,
                                 Shares = ps != null ? ps.PostSharesCount : 0,
                                 Views = pv != null ? pv.PostViewsCount : 0,
                                 smp.Status,
                                 GroupId = g.Id,
                                 g.UserGuid,
                                 g.Name,
                                 g.GroupIcon,
                                 sm.SocialMediaName
                             }).ToList();

                if (query.Count == 0)
                {
                    return Ok(new { Status = "true", Data = new List<PostResponse1>() });
                }

                // Step 2: Group the data by post and group ID, and aggregate the platforms
                var response = query
                    .GroupBy(q => new { q.Id, q.GroupId })
                    .Select(g => new PostResponse1
                    {
                        Title = g.First().Title,
                        Description = g.First().Description,
                        PostIcon = JsonConvert.DeserializeObject<List<string>>(g.First().PostIcon)
                                      .Select(fileName => GenerateServerPathUrl(fileName)).FirstOrDefault(),
                        CreatedAt = g.First().CreatedAt,
                        Id = g.First().Id,
                        Likes = g.Sum(x => x.Likes),
                        Shares = g.Sum(x => x.Shares),
                        Views = g.Sum(x => x.Views),
                        Status = g.First().Status,
                        Group = new GroupResponse
                        {
                            Id = g.First().GroupId,
                            UserGuid = g.First().UserGuid,
                            Name = g.First().Name,
                            GroupIcon = g.First().GroupIcon,
                            Platform = g.Select(x => x.SocialMediaName).Distinct().ToArray()
                        }
                    }).ToList();

                return Ok(new { Status = "true", Data = response });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { Status = "Error", Message = ex.Message });
            }
        }

        private string GenerateServerPathUrl(string fileName)
        {
            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            return $"{baseUrl}/path-to-your-images/{fileName}";
        }
*/

        /*[HttpGet("ScheduledPosts")]
        [Authorize]

        public IActionResult ScheduledPosts([FromQuery] ScheduledPostsRequest request)
        {
            var response = new List<ScheduledPostResponse>
            {
                new ScheduledPostResponse
                {
                    Group = new Groups
                     {
                         Name = "Musemind Design",
                         Platform = new List<string> { "facebook", "instagram" },
                         groupIcon = "https://thumbs.dreamstime.com/b/teamwork-group-friends-icon-vector-illustration-teamwork-group-friends-icon-118637039.jpg"
                     },
                    Title = "Musmind Design Group",
                    Description = "We are looking for Visual designer.",
                    PostIcon = "https://www.shutterstock.com/image-photo/earth-day-environment-green-globe-260nw-2285503199.jpg",
                    Id = 4,
                    Status = "Scheduled",
                    ScheduledTime = DateTime.Parse("2024-12-06T05:30:13.920")
                },
                new ScheduledPostResponse
                {
                    Group = new Groups
                     {
                         Name = "Musemind Design",
                         Platform = new List<string> { "facebook", "instagram" },
                         groupIcon = "https://thumbs.dreamstime.com/b/teamwork-group-friends-icon-vector-illustration-teamwork-group-friends-icon-118637039.jpg"
                     },
                    Title = "Think like a monk.",
                    Description = "Monk from inside is a great feeling I ever imagined.",
                    PostIcon = "https://www.shutterstock.com/image-photo/earth-day-environment-green-globe-260nw-2285503199.jpg",
                    Id =3,
                    Status = "Scheduled",
                    ScheduledTime = DateTime.Parse("2024-12-07T06:45:00.000")
                }
            };

            // Apply the NoOfPosts filter if provided
            if (request.NoOfPosts.HasValue)
            {
                response = response.Take(request.NoOfPosts.Value).ToList();
            }

            return Ok(response);
        }*/
        /*[HttpGet("ScheduledPosts")]
        [Authorize]
        public IActionResult ScheduledPosts([FromQuery] ScheduledPostsRequest request)
        {
            var response = new List<ScheduledPostResponse>
    {
        new ScheduledPostResponse
        {
            Group = new Groups
            {
                Name = "Eco Warriors",
               *//* Platform = new List<string> { "facebook", "twitter" },*//*
                groupIcon = "https://thumbs.dreamstime.com/b/teamwork-group-friends-icon-vector-illustration-teamwork-group-friends-icon-118637039.jpg"
            },
            Title = "Save the Planet",
            Description = "Join us in our mission to protect the environment.",
            PostIcon = "https://www.shutterstock.com/image-photo/earth-day-environment-green-globe-260nw-2285503199.jpg",
            Id = 5,
            Status = "Scheduled",
            ScheduledTime = DateTime.Parse("2024-12-10T09:15:00.000")
        },
        new ScheduledPostResponse
        {
            Group = new Groups
            {
                Name = "Foodies United",
                *//*Platform = new List<string> { "instagram", "pinterest" },*//*
                groupIcon = "https://thumbs.dreamstime.com/b/teamwork-group-friends-icon-vector-illustration-teamwork-group-friends-icon-118637039.jpg"
            },
            Title = "Delicious Recipes for Everyone",
            Description = "Discover mouth-watering recipes and cooking tips.",
            PostIcon = "https://www.shutterstock.com/image-photo/earth-day-environment-green-globe-260nw-2285503199.jpg",
            Id = 6,
            Status = "Scheduled",
            ScheduledTime = DateTime.Parse("2024-12-11T11:00:00.000")
        },
        new ScheduledPostResponse
        {
            Group = new Groups
            {
                Name = "Fitness Fanatics",
                *//*Platform = new List<string> { "facebook", "linkedin" },*//*
                groupIcon = "https://thumbs.dreamstime.com/b/teamwork-group-friends-icon-vector-illustration-teamwork-group-friends-icon-118637039.jpg"
            },
            Title = "Get Fit in 2024",
            Description = "Tips and tricks to stay fit and healthy in the new year.",
            PostIcon = "https://www.shutterstock.com/image-photo/earth-day-environment-green-globe-260nw-2285503199.jpg",
            Id = 7,
            Status = "Scheduled",
            ScheduledTime = DateTime.Parse("2024-12-12T07:45:00.000")
        }
    };

            // Apply the NoOfPosts filter if provided
            if (request.NoOfPosts.HasValue)
            {
                response = response.Take(request.NoOfPosts.Value).ToList();
            }

            return Ok(response);
        }*/



        /*public IActionResult filteredPost([FromQuery] PostsFilterRequest request)
        {
            var response = new List<PostResponse>
            {
                new PostResponse
                {
                    Group = new Groups
                     {
                         Name = "Musemind Design",
                         Platform = new List<string> { "facebook", "instagram" },
                         groupIcon = "https://thumbs.dreamstime.com/b/teamwork-group-friends-icon-vector-illustration-teamwork-group-friends-icon-118637039.jpg"
                     },

                    Title = "Musmind Design Hiring",
                    Description = "We are looking for Visual designer.",
                    PostIcon = "https://www.shutterstock.com/image-photo/earth-day-environment-green-globe-260nw-2285503199.jpg",
                    Id = 1,
                    Likes = 1000,
                    Views = 2000,
                    Comments = 2000,
                    Shares = 200,
                    Status = "Published",
                    StatusCode = 1
                },
                new PostResponse
                {
                    Group = new Groups
                    {
                        Name = "Musemind Design",
                        Platform = new List<string> { "facebook", "instagram" },
                        groupIcon = "https://thumbs.dreamstime.com/b/teamwork-group-friends-icon-vector-illustration-teamwork-group-friends-icon-118637039.jpg"
                    },
                    Title = "Think like a monk.",
                    Description = "Monk from inside is a great feeling i ever Imagined.",
                    PostIcon = "https://www.shutterstock.com/image-photo/earth-day-environment-green-globe-260nw-2285503199.jpg",
                    Id = 2,
                    Likes = 1000,
                    Views = 2000,
                    Comments = 2000,
                    Shares = 200,
                    Status = "Published",
                    StatusCode = 1
                }
            };

            return Ok(response);
        }*/


        /*public IActionResult filteredPost([FromQuery] PostsFilterRequest request)
        {
            // Define static response data
            var postDetails1 = new List<PostResponse>
        {
            new PostResponse
            {
                Group = new Groups
                {
                    Name = "Musemind Design",
                    Platform = new List<string> { "facebook", "instagram" },
                    groupIcon = "https://thumbs.dreamstime.com/b/teamwork-group-friends-icon-vector-illustration-teamwork-group-friends-icon-118637039.jpg"
                },
                Title = "Musmind Design Hiring",
                Description = "We are looking for Visual designer.",
                PostIcon = "https://www.shutterstock.com/image-photo/earth-day-environment-green-globe-260nw-2285503199.jpg",
                Id = 1,
                Likes = 1000,
                Views = 2000,
                Comments = 2000,
                Shares = 200,
                Status = "Published",
                StatusCode = 1
            },
            new PostResponse
            {
                Group = new Groups
                {
                    Name = "Musemind Design",
                    Platform = new List<string> { "facebook", "instagram" },
                    groupIcon = "https://thumbs.dreamstime.com/b/teamwork-group-friends-icon-vector-illustration-teamwork-group-friends-icon-118637039.jpg"
                },
                Title = "Think like a monk.",
                Description = "Monk from inside is a great feeling I ever Imagined.",
                PostIcon = "https://www.shutterstock.com/image-photo/earth-day-environment-green-globe-260nw-2285503199.jpg",
                Id = 2,
                Likes = 1000,
                Views = 2000,
                Comments = 2000,
                Shares = 200,
                Status = "Published",
                StatusCode = 1
            }
        };

            var response = new List<PostDateWise>
        {
            new PostDateWise
            {
                 Date = new DateGroup
                {
                    Value = "2024-05-07T17:24:18.1066667",
                    Data = postDetails1
                }
            },
            new PostDateWise
            {
                Date = new DateGroup
                {
                    Value = "2024-05-06T17:24:18.1066667",
                    Data = postDetails1
                }
            }
        };



            return Ok(response);
        }*/


        /* public IActionResult filteredPost([FromQuery] PostsFilterRequest request)
         {
             // Define static response data
             var postDetails1 = new List<PostResponse>
     {
         new PostResponse
         {
             Group = new Groups
             {
                 Name = "Musemind Design",
                 Platform = new List<string> { "facebook", "instagram" },
                 groupIcon = "https://thumbs.dreamstime.com/b/teamwork-group-friends-icon-vector-illustration-teamwork-group-friends-icon-118637039.jpg"
             },
             Title = "Musmind Design Hiring",
             Description = "We are looking for Visual designer.",
             PostIcon = "https://www.shutterstock.com/image-photo/earth-day-environment-green-globe-260nw-2285503199.jpg",
             Id = 1,
             Likes = 1000,
             Views = 2000,
             Comments = 2000,
             Shares = 200,
             Status = "Published",
             StatusCode = 1
         },
         new PostResponse
         {
             Group = new Groups
             {
                 Name = "Musemind Design",
                 Platform = new List<string> { "facebook", "instagram" },
                 groupIcon = "https://thumbs.dreamstime.com/b/teamwork-group-friends-icon-vector-illustration-teamwork-group-friends-icon-118637039.jpg"
             },
             Title = "Think like a monk.",
             Description = "Monk from inside is a great feeling I ever Imagined.",
             PostIcon = "https://www.shutterstock.com/image-photo/earth-day-environment-green-globe-260nw-2285503199.jpg",
             Id = 2,
             Likes = 1000,
             Views = 2000,
             Comments = 2000,
             Shares = 200,
             Status = "Published",
             StatusCode = 1
         }
     };

             var response = new List<PostDateWise>
     {
         new PostDateWise
         {
             Date = new DateGroup
             {
                 Value = "2024-05-07T17:24:18.1066667",
                 Data = postDetails1
             }
         },
         new PostDateWise
         {
             Date = new DateGroup
             {
                 Value = "2024-05-06T17:24:18.1066667",
                 Data = postDetails1
             }
         }
     };

             // Apply filters
             if (request != null)
             {
                 // Filter by specific dates
                 var specificDates = new List<string>
         {
             "2024-05-07T17:24:18.1066667",

         };
                 response = response.Where(r => specificDates.Contains(r.Date.Value)).ToList();

                 // Filter by platform
                 if (!string.IsNullOrEmpty(request.Platform) && request.Platform.ToLower() != "all")
                 {
                     response.ForEach(r => r.Date.Data = r.Date.Data
                         .Where(p => p.Group.Platform.Contains(request.Platform.ToLower()))
                         .ToList());
                 }

                 // Filter by search keyword
                 if (!string.IsNullOrEmpty(request.searchKeyword))
                 {
                     response.ForEach(r => r.Date.Data = r.Date.Data
                         .Where(p => p.Group.Name.Contains(request.searchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                     p.Title.Contains(request.searchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                     p.Description.Contains(request.searchKeyword, StringComparison.OrdinalIgnoreCase))
                         .ToList());
                 }
             }

             // Remove any empty DateGroups
             response = response.Where(r => r.Date.Data.Any()).ToList();

             // Keep only one date
             if (response.Count > 0)
             {
                 response = new List<PostDateWise> { response[0] };
             }

             return Ok(response);
         }*/


        /*public IActionResult filteredPost([FromQuery] PostsFilterRequest request)
        {
            // Define static response data
            var postDetails1 = new List<PostResponse>
    {
        new PostResponse
        {
            Group = new Groups
            {
                Name = "Musemind Design",
                Platform = new List<string> { "facebook", "instagram" },
                groupIcon = "https://thumbs.dreamstime.com/b/teamwork-group-friends-icon-vector-illustration-teamwork-group-friends-icon-118637039.jpg"
            },
            Title = "Musmind Design Hiring",
            Description = "We are looking for Visual designer.",
            PostIcon = "https://www.shutterstock.com/image-photo/earth-day-environment-green-globe-260nw-2285503199.jpg",
            Id = 1,
            Likes = 1000,
            Views = 2000,
            Comments = 2000,
            Shares = 200,
            Status = "Published",
            StatusCode = 1,
            TagsGroup = "Influencers", // Add this
            SelectedGroup = "Group 2" // Add this
        },
        new PostResponse
        {
            Group = new Groups
            {
                Name = "Musemind Design",
                Platform = new List<string> { "facebook", "instagram" },
                groupIcon = "https://thumbs.dreamstime.com/b/teamwork-group-friends-icon-vector-illustration-teamwork-group-friends-icon-118637039.jpg"
            },
            Title = "Think like a monk.",
            Description = "Monk from inside is a great feeling I ever Imagined.",
            PostIcon = "https://www.shutterstock.com/image-photo/earth-day-environment-green-globe-260nw-2285503199.jpg",
            Id = 2,
            Likes = 1000,
            Views = 2000,
            Comments = 2000,
            Shares = 200,
            Status = "Published",
            StatusCode = 1,
            TagsGroup = "Influencers", // Add this
            SelectedGroup = "Group 2" // Add this
        }
    };

            var response = new List<PostDateWise>
    {
        new PostDateWise
        {
            Date = new DateGroup
            {
                Value = "2024-05-07T17:24:18.1066667",
                Data = postDetails1
            }
        },
        new PostDateWise
        {
            Date = new DateGroup
            {
                Value = "2024-05-06T17:24:18.1066667",
                Data = postDetails1
            }
        }
    };

            // Apply filters
            if (request != null)
            {
                // Filter by specific dates if provided
                if (request.Date.HasValue)
                {
                    response = response.Where(r => r.Date.Value == request.Date.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffffff")).ToList();
                }

                // Filter by platform
                if (!string.IsNullOrEmpty(request.Platform) && request.Platform.ToLower() != "all")
                {
                    response.ForEach(r => r.Date.Data = r.Date.Data
                        .Where(p => p.Group.Platform.Contains(request.Platform.ToLower()))
                        .ToList());
                }

                // Filter by search keyword
                if (!string.IsNullOrEmpty(request.searchKeyword))
                {
                    response.ForEach(r => r.Date.Data = r.Date.Data
                        .Where(p => p.Group.Name.Contains(request.searchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                    p.Title.Contains(request.searchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                    p.Description.Contains(request.searchKeyword, StringComparison.OrdinalIgnoreCase))
                        .ToList());
                }

                // Filter by tagsGroup
                if (!string.IsNullOrEmpty(request.tagsGroup))
                {
                    response.ForEach(r => r.Date.Data = r.Date.Data
                        .Where(p => p.TagsGroup.Equals(request.tagsGroup, StringComparison.OrdinalIgnoreCase))
                        .ToList());
                }

                // Filter by selectedGroup
                if (!string.IsNullOrEmpty(request.selectedGroup))
                {
                    response.ForEach(r => r.Date.Data = r.Date.Data
                        .Where(p => p.SelectedGroup.Equals(request.selectedGroup, StringComparison.OrdinalIgnoreCase))
                        .ToList());
                }
            }

            // Remove any empty DateGroups
            response = response.Where(r => r.Date.Data.Any()).ToList();

            // Keep only one date if filtering by date
            if (request != null && request.Date.HasValue && response.Count > 0)
            {
                response = new List<PostDateWise> { response[0] };
            }

            return Ok(response);
        }*/
        /*[HttpGet("filteredPost")]
        [Authorize]

        public IActionResult filteredPost([FromQuery] PostsFilterRequest request)
        {
            // Define static response data
            var postDetails1 = new List<PostResponse>
    {
        new PostResponse
        {
            Group = new Groups
            {
                Name = "Musemind Design",
               *//* Platform = new List<string> { "facebook", "instagram" },*//*
                groupIcon = "https://thumbs.dreamstime.com/b/teamwork-group-friends-icon-vector-illustration-teamwork-group-friends-icon-118637039.jpg"
            },
            Title = "Musmind Design Hiring",
            Description = "We are looking for Visual designer.",
            PostIcon = "https://www.shutterstock.com/image-photo/earth-day-environment-green-globe-260nw-2285503199.jpg",
            Id = 1,
            Likes = 1000,
            Views = 2000,
            Comments = 2000,
            Shares = 200,
            Status = "Published",
            StatusCode = 1,
            TagsGroup = "Influencers",
            SelectedGroup = "Group 2"
        },
        new PostResponse
        {
            Group = new Groups
            {
                Name = "Musemind Design",
               *//* Platform = new List<string> { "facebook", "instagram" },*//*
                groupIcon = "https://thumbs.dreamstime.com/b/teamwork-group-friends-icon-vector-illustration-teamwork-group-friends-icon-118637039.jpg"
            },
            Title = "Think like a monk.",
            Description = "Monk from inside is a great feeling I ever Imagined.",
            PostIcon = "https://www.shutterstock.com/image-photo/earth-day-environment-green-globe-260nw-2285503199.jpg",
            Id = 2,
            Likes = 1000,
            Views = 2000,
            Comments = 2000,
            Shares = 200,
            Status = "Published",
            StatusCode = 1,
            TagsGroup = "Influencers",
            SelectedGroup = "Group 2"
        }
    };

            var response = new List<PostDateWise>
    {
        new PostDateWise
        {
            Date = new DateGroup
            {
                Value = "2024-05-07T17:24:18.1066667",
                Data = postDetails1
            }
        },
        new PostDateWise
        {
            Date = new DateGroup
            {
                Value = "2024-05-06T17:24:18.1066667",
                Data = postDetails1
            }
        }
    };

            // Apply filters
            if (request != null)
            {
                // Filter by specific dates if provided
                if (request.Date.HasValue)
                {
                    response = response.Where(r => r.Date.Value == request.Date.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffffff")).ToList();
                }
                //Platform error
                // Filter by platform
                *//*if (!string.IsNullOrEmpty(request.Platform) && request.Platform.ToLower() != "all" && request.Platform.ToLower() != "null")
                {
                    response.ForEach(r => r.Date.Data = r.Date.Data
                        .Where(p => p.Group.Platform.Contains(request.Platform.ToLower()))
                        .ToList());
                }*//*

                // Filter by search keyword
                if (!string.IsNullOrEmpty(request.searchKeyword))
                {
                    response.ForEach(r => r.Date.Data = r.Date.Data
                        .Where(p => p.Group.Name.Contains(request.searchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                    p.Title.Contains(request.searchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                    p.Description.Contains(request.searchKeyword, StringComparison.OrdinalIgnoreCase))
                        .ToList());
                }

                // Filter by tagsGroup
                if (!string.IsNullOrEmpty(request.tagsGroup) && request.tagsGroup.ToLower() != "null")
                {
                    response.ForEach(r => r.Date.Data = r.Date.Data
                        .Where(p => p.TagsGroup.Equals(request.tagsGroup, StringComparison.OrdinalIgnoreCase))
                        .ToList());
                }

                // Filter by selectedGroup
                if (!string.IsNullOrEmpty(request.selectedGroup) && request.selectedGroup.ToLower() != "null")
                {
                    response.ForEach(r => r.Date.Data = r.Date.Data
                        .Where(p => p.SelectedGroup.Equals(request.selectedGroup, StringComparison.OrdinalIgnoreCase))
                        .ToList());
                }
            }

            // Remove any empty DateGroups
            response = response.Where(r => r.Date.Data.Any()).ToList();

            // Keep only one date if filtering by date
            if (request != null && request.Date.HasValue && response.Count > 0)
            {
                response = new List<PostDateWise> { response[0] };
            }

            return Ok(response);
        }*/

        /*[HttpGet("filteredPost")]
        public IActionResult FilteredPost([FromQuery] PostsFilterRequest request)
        {
            var data = (from sdp in _context.ScheduledPost
                        join gp in _context.@group on sdp.UserGuid equals gp.UserGuid
                        join gsm in _context.GroupSocialMedia on gp.Id equals gsm.GroupId
                        join sm in _context.SocialMedia on gsm.SocialMediaId equals sm.Id
                        where (sdp.UserGuid == request.userGUId)
                        select new PostResponse1
                        {
                            Group = new GroupResponse
                            {
                                Name = gp.Name,
                                Id = gp.Id,
                                GroupIcon = gp.GroupIcon,
                                Platform = new string[] { sm.SocialMediaName },
                            },
                            UserGuid = sdp.UserGuid,
                            platform = sm.SocialMediaName,
                            CreatedAt = sdp.createdOn,
                            Title = sdp.Title,
                            Description = sdp.Description,
                            PostIcon = sdp.MediaUrl,
                            Id = sdp.Id,
                            Likes = 1000,
                            Views = 2000,
                            Comments = 2000,
                            Shares = 200,
                            StatusCode = 1,
                            TagsGroup = "Influencers",
                            SelectedGroup = "Group 2",
                            PostStatus = sdp.IsPublished
                        }).ToList();

            // Initialize response with data
            var response = data;

            if (!string.IsNullOrWhiteSpace(request.Platform) && request.Platform != "null")
            {
                response = response.Where(d => d.platform == request.Platform).ToList();
            }

            //if (!string.IsNullOrEmpty(request.userGUId))
            //{
            //    response = response.Where(d => d.UserGuid == request.userGUId).ToList();
            //}
            //if (request.Date != null)
            //{
            //    response = response.Where(d => d.CreatedAt.Date == request.Date.Value.Date).ToList();
            //}

            if (!string.IsNullOrWhiteSpace(request.Date) && request.Date != "null")
            {
                DateTime parsedDate;
                if (DateTime.TryParse(request.Date, out parsedDate))
                {
                    response = response.Where(d => d.CreatedAt.Date == parsedDate.Date).ToList();
                }
                else
                {
                    // Handle invalid date format if needed
                }
            }

            if ((request.searchKeyword != null && request.searchKeyword != "null"))
            {
                response = response.Where(d =>
                                           d.Title.Contains(request.searchKeyword) ||
                                           d.Description.Contains(request.searchKeyword) ||
                                           d.Group.Name.Contains(request.searchKeyword) ||
                                           d.TagsGroup.Contains(request.searchKeyword) ||
                                           d.SelectedGroup.Contains(request.searchKeyword) ||
                                           d.platform.Contains(request.searchKeyword)).ToList();
            }
            // Uncomment and add filtering for TagsGroup and SelectedGroup if needed
            if ((request.tagsGroup != null && request.tagsGroup != "null"))
            {
                response = response.Where(d => d.TagsGroup == request.tagsGroup).ToList();
            }
            if ((request.selectedGroup != null && request.selectedGroup != "null"))
            {
                response = response.Where(d => d.SelectedGroup == request.selectedGroup).ToList();
            }

            return Ok(response);
        }*/



        /*[HttpGet("filteredPost")]
        public IActionResult FilteredPost([FromQuery] PostsFilterRequest request)
        {
            var data = (from sdp in _context.ScheduledPost
                        join gp in _context.@group on sdp.UserGuid equals gp.UserGuid
                        join gsm in _context.GroupSocialMedia on gp.Id equals gsm.GroupId
                        join sm in _context.SocialMedia on gsm.SocialMediaId equals sm.Id
                        where sdp.UserGuid == request.userGUId
                        select new
                        {
                            CreatedAt = sdp.createdOn,
                            Group = new
                            {
                                Name = gp.Name,
                                GroupIcon = gp.GroupIcon,
                                Platform = new string[] { sm.SocialMediaName },
                            },
                            UserGuid = sdp.UserGuid,
                            platform = sm.SocialMediaName,
                            Title = sdp.Title,
                            Description = sdp.Description,
                            PostIcon = sdp.MediaUrl,
                            Id = sdp.Id,
                            Likes = 1000,
                            Views = 2000,
                            Comments = 2000,
                            Shares = 200,
                            StatusCode = 1,
                            TagsGroup = "Influencers",
                            SelectedGroup = "Group 2",
                            Status = sdp.IsPublished ? "Published" : "Unpublished"
                        }).ToList();

            // Filtering based on platform, date, and keywords as needed
            if (!string.IsNullOrWhiteSpace(request.Platform) && request.Platform != "null")
            {
                data = data.Where(d => d.platform.Contains(request.Platform)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(request.Date) && request.Date != "null")
            {
                if (DateTime.TryParse(request.Date, out DateTime parsedDate))
                {
                    data = data.Where(d => d.CreatedAt.Date == parsedDate.Date).ToList();
                }
            }

            if (!string.IsNullOrWhiteSpace(request.searchKeyword) && request.searchKeyword != "null")
            {
                data = data.Where(d =>
                                  d.Title.Contains(request.searchKeyword) ||
                                  d.Description.Contains(request.searchKeyword) ||
                                  d.Group.Name.Contains(request.searchKeyword) ||
                                  d.TagsGroup.Contains(request.searchKeyword) ||
                                  d.SelectedGroup.Contains(request.searchKeyword) ||
                                  d.platform.Contains(request.searchKeyword)).ToList();
            }

            // Grouping by date
            var groupedData = data
                .GroupBy(d => d.CreatedAt.Date)
                .Select(g => new
                {
                    Date = new
                    {
                        Value = g.Key.ToString("yyyy-MM-dd"),
                        Data = g.Select(post => new
                        {
                            post.Group,
                            post.Title,
                            post.Description,
                            post.PostIcon,
                            post.Id,
                            post.Likes,
                            post.Views,
                            post.Comments,
                            post.Shares,
                            Status = post.Status,
                            post.StatusCode
                        }).ToList()
                    }
                })
                .ToList();

            return Ok(groupedData);
        }*/


        [HttpGet("filteredPost")]
        public IActionResult FilteredPost([FromQuery] PostsFilterRequest request)
        {
            var data = (from sdp in _context.ScheduledPost
                        join gp in _context.@group on sdp.UserGuid equals gp.UserGuid
                        join gsm in _context.GroupSocialMedia on gp.Id equals gsm.GroupId
                        join sm in _context.SocialMedia on gsm.SocialMediaId equals sm.Id
                        where sdp.UserGuid == request.userGUId
                        select new
                        {
                            CreatedAt = sdp.createdOn,
                            @group = new
                            {
                                name = gp.Name,
                                groupIcon = gp.GroupIcon,
                                platform = new string[] { sm.SocialMediaName },
                            },
                            UserGuid = sdp.UserGuid,
                            platform = sm.SocialMediaName,
                            title = sdp.Title,
                            description = sdp.Description,
                            postIcon = sdp.MediaUrl,
                            id = sdp.Id,
                            likes = 1000,
                            views = 2000,
                            comments = 2000,
                            shares = 200,
                            statusCode = 1,
                            TagsGroup = "Influencers",
                            SelectedGroup = "Group 2",
                            status = sdp.IsPublished ? "Published" : "Unpublished"
                        }).ToList();

            // Filtering based on platform, date, and keywords as needed
            if (!string.IsNullOrWhiteSpace(request.Platform) && request.Platform != "null")
            {
                data = data.Where(d => d.platform.Contains(request.Platform)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(request.Date) && request.Date != "null")
            {
                if (DateTime.TryParse(request.Date, out DateTime parsedDate))
                {
                    data = data.Where(d => d.CreatedAt.Date == parsedDate.Date).ToList();
                }
            }

            if (!string.IsNullOrWhiteSpace(request.searchKeyword) && request.searchKeyword != "null")
            {
                data = data.Where(d =>
                                  d.title.Contains(request.searchKeyword) ||
                                  d.description.Contains(request.searchKeyword) ||
                                  d.@group.name.Contains(request.searchKeyword) ||
                                  d.TagsGroup.Contains(request.searchKeyword) ||
                                  d.SelectedGroup.Contains(request.searchKeyword) ||
                                  d.platform.Contains(request.searchKeyword)).ToList();
            }

            // Grouping by date
            var groupedData = data
                .GroupBy(d => d.CreatedAt.Date)
                .Select(g => new
                {
                    date = new
                    {
                        value = g.Key.ToString("yyyy-MM-dd"),
                        data = g.Select(post => new
                        {
                            post.@group,
                            post.title,
                            post.description,
                            post.postIcon,
                            post.id,
                            post.likes,
                            post.views,
                            post.comments,
                            post.shares,
                            Status = post.status,
                            post.statusCode
                        }).ToList()
                    }
                })
                .ToList();

            return Ok(groupedData);
        }


        [HttpGet("ScheduledPosts")]
        public IActionResult ScheduledPosts([FromQuery] ScheduledPostsRequest request)
        {
            var response = (from sdp in _context.ScheduledPost
                            join gp in _context.@group on sdp.UserGuid equals gp.UserGuid
                            join gsm in _context.GroupSocialMedia on gp.Id equals gsm.GroupId
                            join sm in _context.SocialMedia on gsm.SocialMediaId equals sm.Id
                            where sdp.UserGuid == request.UserGUId
                            orderby sdp.Id descending
                            select new
                            {
                                id = sdp.Id,
                                Group = new GroupResponse
                                {
                                    Name = gp.Name,
                                    GroupIcon = gp.GroupIcon,
                                    Platform = new string[] { sm.SocialMediaName },
                                },
                                // Post = new ScheduledPostResponse
                                // {
                                Title = sdp.Title,
                                Description = sdp.Description,
                                //PostIcon = HomeScreenController.GetFirstMediaUrl(sdp.MediaUrl),
                                PostIcon = sdp.MediaUrl,
                                ScheduledTimeString = sdp.ScheduledTime,
                                Poststatus = sdp.IsPublished
                                //}
                            }).ToList();
            return Ok(response);
        }



    }
}

