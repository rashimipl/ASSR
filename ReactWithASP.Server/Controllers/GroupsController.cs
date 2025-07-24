using JWTAuthentication.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using PayPal;
using ReactWithASP.Server.Models;
using ReactWithASP.Server.Models.Posts;

using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text.RegularExpressions;

namespace ReactWithASP.Server.Controllers
{
    [ApiController]
    [Route("api")]
    public class GroupsController : ControllerBase
    {            
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment;

        public GroupsController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment, ApplicationDbContext context)
        {
            _userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
            Environment = environment;
            _context = context;
        }

        /*[HttpGet("CreateGroup")]
        public IActionResult CreateGroup([FromBody] CreateGroupRequest request)
        {
            // Generate new GroupId
            var newGroupId = SampleData.Groups.Max(g => g.GroupId) + 1;

            // Create new group
            var newGroup = new SocialMediaGroup
            {
                GroupId = 1,
                GroupName = "Group 1",
                GroupIcon = "../icon.jpg",
                SocialMediaIcons = new List<string>
            {
                "https://example.com/icon1.jpg",
                "https://example.com/icon2.jpg",
                "https://example.com/icon3.jpg"
            }
            },

            // Add to sample data
            SampleData.Groups.Add(newGroup);

            // Create response
            var response = new CreateGroupResponse
            {
                GroupId = newGroup.GroupId,
                GroupName = newGroup.GroupName,
                GroupIcon = newGroup.GroupIcon,
                SocialMediaIcons = newGroup.SocialMediaIcons
            };

            return Ok(response);
        }*/
        /*[HttpGet("GroupList")]
        [HttpGet("Groups")]
        [Authorize]*/
        /*public IActionResult GroupList([FromQuery] UserSocialMedia request)
        {
            var response = new List<SocialMediaGroup>
            {
                new SocialMediaGroup
                {
                    Id = 1,
                    name = "Evolution",
                    groupIcon = "https://thumbs.dreamstime.com/b/teamwork-group-friends-icon-vector-illustration-teamwork-group-friends-icon-118637039.jpg",
                    socialMediaUrl = new List<string>
                    {
                        "http://167.86.105.98:8070/uploads/facebook-fill.png",
                        "http://167.86.105.98:8070/uploads/whatsapp-fill.png",
                        "http://167.86.105.98:8070/uploads/telegram-fill.png"
                    }
                }
                *//*new SocialMediaGroup
                {
                    Id = 2,
                    name = "Beautiful Life",
                    groupIcon = "https://thumbs.dreamstime.com/b/teamwork-group-friends-icon-vector-illustration-teamwork-group-friends-icon-118637039.jpg",
                    socialMediaUrl = new List<string>
                    {
                       "http://167.86.105.98:8070/uploads/facebook-fill.png",
                        "http://167.86.105.98:8070/uploads/instagram-fill.png"
                    }
                },
                new SocialMediaGroup
                {
                    Id = 3,
                    name = "Misterious World",
                    groupIcon = "https://thumbs.dreamstime.com/b/teamwork-group-friends-icon-vector-illustration-teamwork-group-friends-icon-118637039.jpg",
                    socialMediaUrl = new List<string>
                    {
                        "http://167.86.105.98:8070/uploads/facebook-fill.png",
                        "http://167.86.105.98:8070/uploads/instagram-fill.png",
                        "http://167.86.105.98:8070/uploads/tiktok-fill.png"
                    }
                }*//*
            };

            return Ok(response);
        }*/

        /* [HttpGet("GroupList")]
         [Authorize]
         public IActionResult GroupList()
         {

             var data = (from g in _context.@group
                         join gsm in _context.GroupSocialMedia on g.Id equals gsm.GroupId
                         join sm in _context.SocialMedia on gsm.SocialMediaId equals sm.Id
                         select new SocialMediaGroup
                         {
                             Id = g.Id,
                             name = g.Name,
                             groupIcon = g.GroupIcon,
                             socialMediaName = new List<string>() { sm.SocialMediaName }
                         }).ToList();

             if (data != null)
             {
                 return Ok(data);
             }
             return BadRequest(new { status = "false", Message = " Data Not Found!..." });
         }*/

        [HttpGet("AdminGroupList")]
        public async Task<IActionResult> GroupList(int? id, int pageNumber = 1, int pageSize = 10)
        {
            if (id.HasValue && id.Value != 0)
            {
                // Fetch data for a specific group
                var data = await (from g in _context.@group
                                  join gsm in _context.GroupSocialMedia on g.Id equals gsm.GroupId
                                  join sm in _context.SocialMedia on gsm.SocialMediaId equals sm.Id
                                  where g.Id == id // Use where to filter by id directly
                                  orderby g.Id descending
                                  select new SocialMediaGroup
                                  {
                                      Id = g.Id,
                                      UserGuid = g.UserGuid,
                                      name = g.Name,
                                      groupIcon = g.GroupIcon,
                                      CreatedOn = g.CreatedOn,
                                      socialMediaName = new List<string> { sm.SocialMediaName }
                                  }).ToListAsync();

                if (data.Any())
                {
                    var groupedData = data
                        .GroupBy(g => g.Id)
                        .Select(x => new
                        {
                            Id = x.Key,
                            UserGuid = x.First().UserGuid,
                            Name = x.First().name,
                            GroupIcon = x.First().groupIcon,
                            CreatedOn = x.First().CreatedOn,
                            SocialMediaName = x.SelectMany(s => s.socialMediaName).Distinct().ToList()
                        }).ToList();

                    return Ok(new { data = groupedData });
                }

                return BadRequest(new { status = "false", Message = "Data Not Found!..." });
            }
            else
            {
                // Pagination logic starts here for all groups
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1) pageSize = 10;

                // Calculate the number of records to skip based on the current page
                var skip = (pageNumber - 1) * pageSize;

                var groupedData = await (from g in _context.@group
                                         join gsm in _context.GroupSocialMedia on g.Id equals gsm.GroupId
                                         join sm in _context.SocialMedia on gsm.SocialMediaId equals sm.Id
                                         group sm by new
                                         {
                                             g.Id,
                                             g.UserGuid,
                                             g.Name,
                                             g.GroupIcon,
                                             g.CreatedOn
                                         } into gGroup
                                         select new SocialMediaGroup
                                         {
                                             Id = gGroup.Key.Id,
                                             UserGuid = gGroup.Key.UserGuid,
                                             name = gGroup.Key.Name,
                                             groupIcon = gGroup.Key.GroupIcon,
                                             CreatedOn = gGroup.Key.CreatedOn,
                                             socialMediaName = gGroup.Select(sm => sm.SocialMediaName).ToList()
                                         })
                                         .OrderByDescending(g => g.Id)
                                         .Skip(skip)
                                         .Take(pageSize)
                                         .ToListAsync();

                var totalCount = await (from g in _context.@group
                                        join gsm in _context.GroupSocialMedia on g.Id equals gsm.GroupId
                                        join sm in _context.SocialMedia on gsm.SocialMediaId equals sm.Id
                                        group sm by g.Id into gGroup
                                        select gGroup.Key).CountAsync();

                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                // Create the pagination metadata
                var paginationMetadata = new
                {
                    TotalRecords = totalCount,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return Ok(new { Data = groupedData, Pagination = paginationMetadata });

            }
        }



        //public IActionResult GroupList(int id, int page = 1, int pageSize = 10)
        //{
        //    if (id != 0 && id != null)
        //    {
        //        var data = (from g in _context.@group
        //                    join gsm in _context.GroupSocialMedia on g.Id equals gsm.GroupId
        //                    join sm in _context.SocialMedia on gsm.SocialMediaId equals sm.Id
        //                    orderby g.Id descending
        //                    select new SocialMediaGroup
        //                    {
        //                        Id = g.Id,
        //                        UserGuid = g.UserGuid,
        //                        name = g.Name,
        //                        groupIcon = g.GroupIcon,
        //                        CreatedOn = g.CreatedOn,
        //                        socialMediaName = new List<string> { sm.SocialMediaName }
        //                    }).Where(x => x.Id == id).ToList();

        //        if (data != null)
        //        {
        //            var data1 = data
        //                .GroupBy(g => g.Id)
        //                .Select(x => new
        //                {
        //                    Id = x.Key,
        //                    UserGuid = x.First().UserGuid,
        //                    Name = x.First().name, // Assuming the name is the same across the group
        //                    GroupIcon = x.First().groupIcon, // Assuming the groupIcon is the same across the group
        //                    CreatedOn = x.First().CreatedOn,
        //                    SocialMediaName = x.SelectMany(s => s.socialMediaName).Distinct().ToList() // Aggregate all social media names and ensure they are distinct
        //                })
        //                .ToList();

        //            return Ok(data1);
        //        }
        //        return BadRequest(new { status = "false", Message = "Data Not Found!..." });
        //    }
        //    else
        //    {
        //        // Pagination logic starts here
        //        var data = (from g in _context.@group
        //                    join gsm in _context.GroupSocialMedia on g.Id equals gsm.GroupId
        //                    join sm in _context.SocialMedia on gsm.SocialMediaId equals sm.Id
        //                    orderby g.Id descending
        //                    select new SocialMediaGroup
        //                    {
        //                        Id = g.Id,
        //                        UserGuid = g.UserGuid,
        //                        name = g.Name,
        //                        groupIcon = g.GroupIcon,
        //                        CreatedOn = g.CreatedOn,
        //                        socialMediaName = new List<string> { sm.SocialMediaName }
        //                    })
        //                    .Skip((page - 1) * pageSize)  // Skip records based on page
        //                    .Take(pageSize)               // Take a specific number of records
        //                    .ToList();

        //        if (data != null)
        //        {
        //            var data1 = data
        //                .GroupBy(g => g.Id)
        //                .Select(x => new
        //                {
        //                    Id = x.Key,
        //                    UserGuid = x.First().UserGuid, 
        //                    Name = x.First().name, // Assuming the name is the same across the group
        //                    GroupIcon = x.First().groupIcon, // Assuming the groupIcon is the same across the group
        //                    CreatedOn = x.First().CreatedOn,
        //                    SocialMediaName = x.SelectMany(s => s.socialMediaName).Distinct().ToList() // Aggregate all social media names and ensure they are distinct
        //                })
        //                .ToList();

        //            return Ok(new
        //            {
        //                Data = data1,
        //                Page = page,
        //                PageSize = pageSize,
        //                TotalCount = _context.@group.Count() // Get the total number of records for pagination
        //            });
        //        }
        //        return BadRequest(new { status = "false", Message = "Data Not Found!..." });
        //    }
        //}


        //public IActionResult GroupList(int id )
        //{
        //    if (id != 0 && id != null)
        //    {
        //        var data = (from g in _context.@group
        //                    join gsm in _context.GroupSocialMedia on g.Id equals gsm.GroupId
        //                    join sm in _context.SocialMedia on gsm.SocialMediaId equals sm.Id
        //                    orderby g.Id descending
        //                    select new SocialMediaGroup
        //                    {
        //                        Id = g.Id,
        //                        name = g.Name,
        //                        groupIcon = g.GroupIcon,
        //                        socialMediaName = new List<string> { sm.SocialMediaName }
        //                    }).Where(x=>x.Id == id).ToList();

        //        if (data != null)
        //        {
        //            var data1 = data
        //                .GroupBy(g => g.Id)
        //                .Select(x => new
        //                {
        //                    Id = x.Key,
        //                    Name = x.First().name, // Assuming the name is the same across the group
        //                    GroupIcon = x.First().groupIcon, // Assuming the groupIcon is the same across the group
        //                    SocialMediaName = x.SelectMany(s => s.socialMediaName).Distinct().ToList() // Aggregate all social media names and ensure they are distinct
        //                })
        //                .ToList();

        //            return Ok(data1);
        //        }
        //        return BadRequest(new { status = "false", Message = "Data Not Found!..." });
        //    }

        //    else 
        //    {
        //        var data = (from g in _context.@group
        //                    join gsm in _context.GroupSocialMedia on g.Id equals gsm.GroupId
        //                    join sm in _context.SocialMedia on gsm.SocialMediaId equals sm.Id
        //                    orderby g.Id descending
        //                    select new SocialMediaGroup
        //                    {
        //                        Id = g.Id,
        //                        name = g.Name,
        //                        groupIcon = g.GroupIcon,
        //                        socialMediaName = new List<string> { sm.SocialMediaName }
        //                    }).ToList();

        //        if (data != null)
        //        {
        //            var data1 = data
        //                .GroupBy(g => g.Id)
        //                .Select(x => new
        //                {
        //                    Id = x.Key,
        //                    Name = x.First().name, // Assuming the name is the same across the group
        //                    GroupIcon = x.First().groupIcon, // Assuming the groupIcon is the same across the group
        //                    SocialMediaName = x.SelectMany(s => s.socialMediaName).Distinct().ToList() // Aggregate all social media names and ensure they are distinct
        //                })
        //                .ToList();

        //            return Ok(data1);
        //        }
        //        return BadRequest(new { status = "false", Message = "Data Not Found!..." });

        //    }


        //}


        //[HttpGet("Groups")]
        //public async Task<IActionResult> GetGroups([FromQuery] UserStatusRequest model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var data = (from g in _context.@group
        //                join gsm in _context.GroupSocialMedia on g.Id equals gsm.GroupId
        //                join sm in _context.SocialMedia on gsm.SocialMediaId equals sm.Id
        //                orderby g.Id descending
        //                select new SocialMediaGroup
        //                {
        //                    Id = g.Id,
        //                    UserGuid = g.UserGuid,
        //                    name = g.Name,
        //                    groupIcon = g.GroupIcon,
        //                    CreatedOn = g.CreatedOn,
        //                    socialMediaName = new List<string> { sm.SocialMediaName }
        //                }).ToList();

        //    if (data != null)
        //    {
        //        var data1 = data
        //            .GroupBy(g => g.Id)
        //            .Select(x => new
        //            {
        //                Id = x.Key,
        //                UserGuid = x.First().UserGuid,
        //                Name = x.First().name,
        //                GroupIcon = x.First().groupIcon,
        //                CreatedOn = x.First().CreatedOn,
        //                SocialMediaName = x.SelectMany(s => s.socialMediaName).Distinct().ToList()
        //            })
        //            .ToList();

        //        return Ok(new { data = data1 });
        //    }

        //    return BadRequest(new { status = "false", Message = "Data Not Found!..." });


        //    //     var userGroups = _context.@group.FirstOrDefault(us => us.UserGuid == model.UserGUID);
        //    //     if (userGroups == null)
        //    //     {
        //    //         return NotFound("User Group not found");
        //    //     }

        //    //     var groups = await _context.@group
        //    //.Where(g => g.UserGuid == model.UserGUID)
        //    //.Join(_context.SocialMedia, // Join with SocialMedias table
        //    //      g => g.,  // Join condition on Groups table
        //    //      s => s.Id,             // Join condition on SocialMedias table
        //    //      (g, s) => new           // Result selector
        //    //      {
        //    //          g.Id,
        //    //          g.Name,
        //    //          g.groupIcon,
        //    //          SocialMediaName = s.SocialMediaName
        //    //      })
        //    //.ToListAsync();

        //    //     if (@group.Count == 0)
        //    //     {
        //    //         return NotFound("No groups found for the user");
        //    //     }


        //}


        [HttpGet("GroupList")]
        [HttpGet("Groups")]
        public async Task<IActionResult> GetGroups([FromQuery] UserStatusRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var data = await (from g in _context.@group
                              join gsm in _context.GroupSocialMedia on g.Id equals gsm.GroupId
                              join sm in _context.SocialMedia on gsm.SocialMediaId equals sm.Id
                              where g.UserGuid == model.UserGUID // Filter by UserGUID directly
                              orderby g.Id descending
                              select new SocialMediaGroup
                              {
                                  Id = g.Id,
                                  PageId = gsm.PageId,
                                  UserGuid = g.UserGuid,
                                  name = g.Name,
                                  groupIcon = g.GroupIcon,
                                  CreatedOn = g.CreatedOn,
                                  socialMediaId = new List<int> {sm.Id},
                                  socialMediaName = new List<string> { sm.SocialMediaName },
                                  socialMediaUrl = new List<string>(), // Initialize as empty list
                                  accountId =  sm.Id 
                              })
                              .ToListAsync(); // Asynchronously fetch data

            // Define the folder path for images
            string folderPath = Path.Combine(Environment.WebRootPath, "uploads", "images");

            // Check if the folder exists; if not, create it
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            if (data.Any())
            {
                var groupedData = data
                    .GroupBy(g => g.Id)
                    .Select(x => new
                    {
                        Id = x.Key,
                        PageId = x.First().PageId,
                        UserGuid = x.First().UserGuid,
                        Name = x.First().name,
                        GroupIcon = x.First().groupIcon,
                        CreatedOn = x.First().CreatedOn,
                      socialMediaId = x.SelectMany(s => s.socialMediaId).Distinct().ToList(),
                      SocialMediaName = x.SelectMany(s => s.socialMediaName).Distinct().ToList(),
                        socialMediaUrl = new List<string>(), 
                        accountId = x.First().accountId,
                    })
                    .ToList();

                // Populate socialMediaUrl based on social media names
                var request = HttpContext.Request;
                string baseUrl = $"{request.Scheme}://{request.Host}";

                foreach (var group in groupedData)
                {
                    // Get all files in the folder
                    var files = Directory.GetFiles(folderPath);

                    // Match images to social media names
                    foreach (var mediaName in group.SocialMediaName)
                    {
                        // Assuming images are named like "Facebook.png", "Twitter.png", etc.
                        var matchedFile = files.FirstOrDefault(file =>
                            Path.GetFileNameWithoutExtension(file).Equals(mediaName, StringComparison.OrdinalIgnoreCase));

                        if (matchedFile != null)
                        {
                            // Add the corresponding image URL with HTTPS
                            group.socialMediaUrl.Add($"{baseUrl}/uploads/images/{Path.GetFileName(matchedFile)}");
                        }
                    }
                }

                return Ok(groupedData);
            }

            return Ok(data);
        }




        //public async Task<IActionResult> GetGroups([FromQuery] UserStatusRequest model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var data = await (from g in _context.@group
        //                      join gsm in _context.GroupSocialMedia on g.Id equals gsm.GroupId
        //                      join sm in _context.SocialMedia on gsm.SocialMediaId equals sm.Id
        //                      where g.UserGuid == model.UserGUID // Filter by UserGUID directly
        //                      orderby g.Id descending
        //                      select new SocialMediaGroup
        //                      {
        //                          Id = g.Id,
        //                          UserGuid = g.UserGuid,
        //                          name = g.Name,
        //                          groupIcon = g.GroupIcon,
        //                          CreatedOn = g.CreatedOn,
        //                          socialMediaName = new List<string> { sm.SocialMediaName },
        //                          socialMediaUrl = new List<string> { sm.src },

        //                      })
        //                      .ToListAsync(); // Asynchronously fetch data

        //    if (data.Any())
        //    {
        //        var groupedData = data
        //            .GroupBy(g => g.Id)
        //            .Select(x => new
        //            {
        //                Id = x.Key,
        //                UserGuid = x.First().UserGuid,
        //                Name = x.First().name,
        //                GroupIcon = x.First().groupIcon,
        //                CreatedOn = x.First().CreatedOn,
        //                SocialMediaName = x.SelectMany(s => s.socialMediaName).Distinct().ToList(),
        //                socialMediaUrl = x.SelectMany(s => s.socialMediaUrl).Distinct().ToList()
        //            })
        //            .ToList();

        //        return Ok(groupedData );
        //    }
        //    return Ok(data);
        //}






        [HttpGet("TagsGroup")]
        [Authorize]
        public IActionResult GetTagsGroup([FromQuery] UserSocialMedia request)
        {
            var response = new List<TagsGroup>
            {
                new TagsGroup
                {
                    Id = 1,
                    name = "Influencers",
                },
                new TagsGroup
                {
                    Id = 2,
                     name = "Successful World",
                },
                new TagsGroup
                {
                    Id = 3,
                    name = "Creaters Of Year",
                }

            };

            return Ok(response);
        }



        /*[HttpPost("CreateGroup")]
        [Authorize]
        public IActionResult CreateGroup([FromBody] GroupRequest request)
        {
            // Here, you can add logic to handle the group creation using request.UserGuid and request.Id
            // For example, saving the group information to a database.

            // Returning a static success response for demonstration purposes.
            return Ok("Group created successfully");
        }*/
        /*[HttpPost("CreateGroup")]
        [Authorize]
        public IActionResult CreateGroup([FromBody] GroupRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            group res = new group
            {
                Name = request.Name,
                CreatedOn = DateTime.Now,
                Status = request.Status,
                GroupIcon = request.GroupIcon,
                UserGuid = request.UserGuid
            };
            _context.group.Add(res);
            _context.SaveChanges();

            return Ok(new
            {
                Status = "True",
                Message = "Group created successfully",
                Data = res
            });
        }*/


        /*[HttpPost("CreateGroup")]
        [Authorize]
        public IActionResult CreateGroup([FromBody] GroupRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            group res = new group
            {
                Name = request.Name,
                CreatedOn = DateTime.Now,
                Status = request.Status,
                GroupIcon = request.GroupIcon,
                UserGuid = request.UserGuid
            };
            _context.group.Add(res);
            _context.SaveChanges();

            return Ok(new
            {
                Status = "True",
                Message = "Group created successfully",
                Data = res
            });
        }*/


        //[HttpPost("CreateGroup")]
        //public IActionResult CreateGroup([FromBody] GroupRequest request)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    // Create a new group
        //    group res = new group
        //    {
        //        Name = request.GroupName,
        //        CreatedOn = DateTime.Now,
        //        Status = true,
        //        GroupIcon = "https://thumbs.dreamstime.com/b/teamwork-group-friends-icon-vector-illustration-teamwork-group-friends-icon-118637039.jpg",
        //        UserGuid = request.UserGuid
        //    };

        //    // Add the group to the context
        //    _context.group.Add(res);
        //    _context.SaveChanges();

        //    // Get the group ID after saving
        //    int groupId = res.Id;

        //    // Iterate over the IDs in the array and create GroupSocialMediaUser entries
        //    foreach (var id in request.Id)
        //    {
        //        GroupSocialMedia GroupSocialMedia = new GroupSocialMedia
        //        {
        //            GroupId = groupId,
        //            SocialMediaId = id
        //        };

        //        _context.GroupSocialMedia.Add(GroupSocialMedia);
        //    }

        //    // Save changes to the context
        //    _context.SaveChanges();

        //    return Ok(new
        //    {
        //        Status = "True",
        //        Message = "Group created successfully",
        //        Data = res
        //    });
        //}


        [HttpPost("CreateGroup")]
        public IActionResult CreateGroup([FromBody] GroupRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Create a new group
            group res = new group
            {
                Name = request.GroupName,
                CreatedOn = DateTime.UtcNow,
                Status = true,
                GroupIcon = "https://thumbs.dreamstime.com/b/teamwork-group-friends-icon-vector-illustration-teamwork-group-friends-icon-118637039.jpg",
                UserGuid = request.UserGuid,

            };

            // Add the group to the context
            _context.group.Add(res);
            _context.SaveChanges();

            // Get the group ID after saving
            int groupId = res.Id;

      var groupSocialMediaEntries = new List<GroupSocialMedia>();
      // Iterate over the IDs in the array and create GroupSocialMediaUser entries
      foreach (var gupdata in request.GroupDetails)
      {
        var existingPageIds = _context.ConnectedSocialMediaInfo
            .Where(x => x.UserId == request.UserGuid && x.PageId == gupdata.PageId)
            .ToList();
        foreach (var socialMediaInfo in existingPageIds)
        {
          var groupSocialMedia = new GroupSocialMedia
          {
            GroupId = groupId,
            SocialMediaId = gupdata.accountId,
            PageId = socialMediaInfo.PageId,
            PageName = socialMediaInfo.PageName,
            PageProfile = socialMediaInfo.PageProfile,
            PageAccessToken = socialMediaInfo.PageAccessToken, 
            CreatedOn = DateTime.UtcNow
          };

          //_context.GroupSocialMedia.Add(groupSocialMedia);
          groupSocialMediaEntries.Add(groupSocialMedia);

        }
      }

        
        if (groupSocialMediaEntries.Any())
        {
          _context.GroupSocialMedia.AddRange(groupSocialMediaEntries);
          _context.SaveChanges();
        }
        return Ok(new
            {
                Status = "True",
                Message = "Group created successfully",
                Data = res
            });
        }

        [HttpDelete("DeleteGroup")]
        public IActionResult DeleteGroup(string userGuid,int GroupId)
        {
            if (string.IsNullOrEmpty(userGuid))
            {
                return BadRequest(new { Message = "User GUID is required." });
            }

            // Find the group associated with the given UserGuid
            var group1 = _context.group.FirstOrDefault(g => g.UserGuid == userGuid && g.Id==GroupId);

            if (group1 == null)
            {
                return NotFound(new { Message = "Group not found." });
            }

            // Find the GroupSocialMedia entries associated with this group
            var groupSocialMedia = _context.GroupSocialMedia.Where(gsm => gsm.GroupId == group1.Id).ToList();

            // Remove the GroupSocialMedia entries
            _context.GroupSocialMedia.RemoveRange(groupSocialMedia);

            // Remove the group
            _context.group.Remove(group1);

            // Save changes to the context
            _context.SaveChanges();

            return Ok(new
            {
                Status = "True",
                Message = "Group and associated social media entries deleted successfully"
            });
        }
     [HttpPut("UpdateGroup")]
     public IActionResult UpdateGroup([FromBody] UpdateGroupRequest request)
     {
         if (!ModelState.IsValid)
         {
             return BadRequest(ModelState);
         }

         // Find the group by UserGuid and GroupId
         var existingGroup = _context.group.FirstOrDefault(g => g.UserGuid == request.UserGuid && g.Id == request.GroupId);

            if (existingGroup == null)
            {
                return NotFound(new
                {
                    Status = "False",
                    Message = "Group not found"
                });
            }

          // Update group properties
          existingGroup.Name = request.GroupName;
          existingGroup.CreatedOn = DateTime.Now; // Assuming you have an UpdatedOn field
          existingGroup.Status = request.Status;
          existingGroup.GroupIcon = request.GroupIcon ?? existingGroup.GroupIcon; // Update if provided, else keep existing
          existingGroup.ModifiedOn = DateTime.Now;
    // Update GroupSocialMedia entries
    // First, remove existing entries
            var existingGroupSocialMedia = _context.GroupSocialMedia.Where(gsm => gsm.GroupId == existingGroup.Id).ToList();

      var groupSocialMediaEntries = new List<GroupSocialMedia>();

      foreach (var groupDetail in request.GroupDetails)
      {
        var existingSocialMedia = _context.ConnectedSocialMediaInfo
            .Where(x => x.UserId == request.UserGuid && x.PageId == groupDetail.PageId)
            .ToList();

        foreach (var socialMediaInfo in existingSocialMedia)
        {
          groupSocialMediaEntries.Add(new GroupSocialMedia
          {
            GroupId = request.GroupId,
            SocialMediaId = groupDetail.accountId,
            PageId = socialMediaInfo.PageId,
            PageName = socialMediaInfo.PageName,
            PageProfile = socialMediaInfo.PageProfile,
            PageAccessToken = socialMediaInfo.PageAccessToken,
            CreatedOn = DateTime.UtcNow
          });
          _context.GroupSocialMedia.AddRange(groupSocialMediaEntries);

        }
      }
      //_context.GroupSocialMedia.RemoveRange(existingGroupSocialMedia);

      // Add the new entries
      //foreach (var id in request.Id)
      //    {
      //        var groupSocialMedia = new GroupSocialMedia
      //        {
      //            GroupId = existingGroup.Id,
      //            SocialMediaId = id,
      //            CreatedOn = DateTime.Now
      //        };

      //        _context.GroupSocialMedia.Add(groupSocialMedia);
      //    }

      // Save changes to the context
      _context.SaveChanges();

          return Ok(new
          {
              Status = "True",
              Message = "Group updated successfully",
              Data = existingGroup
          });
      }



    // get all account in the perticuller group 
    [HttpGet("AllAccountListonGroupId")]
    
    public async Task<IActionResult> AllAccountlistongroupsid(string userguid, int groupid)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var data = await (from g in _context.@group
                        join gsm in _context.GroupSocialMedia on g.Id equals gsm.GroupId
                        join sm in _context.SocialMedia on gsm.SocialMediaId equals sm.Id
                        where g.UserGuid == userguid && g.Id == groupid
                        orderby g.Id descending
                        select new SocialMediaGroup
                        {
                          Id = g.Id,
                          PageId = gsm.PageId,
                          UserGuid = g.UserGuid,
                          name = g.Name,
                          groupIcon = g.GroupIcon,
                          CreatedOn = g.CreatedOn,
                          socialMediaName = new List<string> { sm.SocialMediaName },
                          socialMediaUrl = new List<string>(), 
                          accountId = sm.Id
                        })
                        .ToListAsync(); // Asynchronously fetch data

      // Define the folder path for images
      string folderPath = Path.Combine(Environment.WebRootPath, "uploads", "images");

      // Check if the folder exists; if not, create it
      if (!Directory.Exists(folderPath))
      {
        Directory.CreateDirectory(folderPath);
      }

      if (data.Any())
      {
        var groupedData = data
            .GroupBy(g => g.Id)
            .Select(x => new
            {
              Id = x.Key,
              PageIds = x.Select(p => p.PageId).Distinct().ToList(),
              UserGuid = x.First().UserGuid,
              Name = x.First().name,
              GroupIcon = x.First().groupIcon,
              CreatedOn = x.First().CreatedOn,
              SocialMediaName = x.SelectMany(s => s.socialMediaName).Distinct().ToList(),
              socialMediaUrl = new List<string>(), 
              accountId = x.First().accountId,
            })
            .ToList();

        // Populate socialMediaUrl based on social media names
        var request = HttpContext.Request;
        string baseUrl = $"{request.Scheme}://{request.Host}";

        foreach (var group in groupedData)
        {
          // Get all files in the folder
          var files = Directory.GetFiles(folderPath);

          // Match images to social media names
          foreach (var mediaName in group.SocialMediaName)
          {
            // Assuming images are named like "Facebook.png", "Twitter.png", etc.
            var matchedFile = files.FirstOrDefault(file =>
                Path.GetFileNameWithoutExtension(file).Equals(mediaName, StringComparison.OrdinalIgnoreCase));

            if (matchedFile != null)
            {
              // Add the corresponding image URL with HTTPS
              group.socialMediaUrl.Add($"{baseUrl}/uploads/images/{Path.GetFileName(matchedFile)}");
            }
          }
        }

        return Ok(groupedData);
      }

      return Ok(data);
    }
  }

}



