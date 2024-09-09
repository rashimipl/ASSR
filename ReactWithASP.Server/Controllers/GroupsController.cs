using JWTAuthentication.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
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


        [HttpGet("GroupList")]
        [HttpGet("Groups")]
        public IActionResult GroupList(int id )
        {
            if (id != 0 && id != null)
            {
                var data = (from g in _context.@group
                            join gsm in _context.GroupSocialMedia on g.Id equals gsm.GroupId
                            join sm in _context.SocialMedia on gsm.SocialMediaId equals sm.Id
                            orderby g.Id descending
                            select new SocialMediaGroup
                            {
                                Id = g.Id,
                                name = g.Name,
                                groupIcon = g.GroupIcon,
                                socialMediaName = new List<string> { sm.SocialMediaName }
                            }).Where(x=>x.Id == id).ToList();

                if (data != null)
                {
                    var data1 = data
                        .GroupBy(g => g.Id)
                        .Select(x => new
                        {
                            Id = x.Key,
                            Name = x.First().name, // Assuming the name is the same across the group
                            GroupIcon = x.First().groupIcon, // Assuming the groupIcon is the same across the group
                            SocialMediaName = x.SelectMany(s => s.socialMediaName).Distinct().ToList() // Aggregate all social media names and ensure they are distinct
                        })
                        .ToList();

                    return Ok(data1);
                }
                return BadRequest(new { status = "false", Message = "Data Not Found!..." });
            }

            else 
            {
                var data = (from g in _context.@group
                            join gsm in _context.GroupSocialMedia on g.Id equals gsm.GroupId
                            join sm in _context.SocialMedia on gsm.SocialMediaId equals sm.Id
                            orderby g.Id descending
                            select new SocialMediaGroup
                            {
                                Id = g.Id,
                                name = g.Name,
                                groupIcon = g.GroupIcon,
                                socialMediaName = new List<string> { sm.SocialMediaName }
                            }).ToList();

                if (data != null)
                {
                    var data1 = data
                        .GroupBy(g => g.Id)
                        .Select(x => new
                        {
                            Id = x.Key,
                            Name = x.First().name, // Assuming the name is the same across the group
                            GroupIcon = x.First().groupIcon, // Assuming the groupIcon is the same across the group
                            SocialMediaName = x.SelectMany(s => s.socialMediaName).Distinct().ToList() // Aggregate all social media names and ensure they are distinct
                        })
                        .ToList();

                    return Ok(data1);
                }
                return BadRequest(new { status = "false", Message = "Data Not Found!..." });

            }

            
        }
        /*[HttpGet("GroupList")]
        [HttpGet("Groups")]
        public async Task<IActionResult> GetGroups([FromQuery] UserStatusRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }



            var userGroups = _context.Group.FirstOrDefault(us => us.UserGuid == model.UserGUID);
            if (userGroups == null)
            {
                return NotFound("User Group not found");
            }

            var groups = await _context.Group
       .Where(g => g.UserGuid == model.UserGUID)
       .Join(_context.SocialMedia, // Join with SocialMedias table
             g => g.SocialMediaId,  // Join condition on Groups table
             s => s.Id,             // Join condition on SocialMedias table
             (g, s) => new           // Result selector
             {
                 g.Id,
                 g.Name,
                 g.groupIcon,
                 SocialMediaName = s.SocialMediaName
             })
       .ToListAsync();

            if (groups.Count == 0)
            {
                return NotFound("No groups found for the user");
            }

            return Ok(groups);
        }*/


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
                CreatedOn = DateTime.Now,
                Status = true,
                GroupIcon = "https://thumbs.dreamstime.com/b/teamwork-group-friends-icon-vector-illustration-teamwork-group-friends-icon-118637039.jpg",
                UserGuid = request.UserGuid
            };

            // Add the group to the context
            _context.group.Add(res);
            _context.SaveChanges();

            // Get the group ID after saving
            int groupId = res.Id;

            // Iterate over the IDs in the array and create GroupSocialMediaUser entries
            foreach (var id in request.Id)
            {
                GroupSocialMedia GroupSocialMedia = new GroupSocialMedia
                {
                    GroupId = groupId,
                    SocialMediaId = id
                };

                _context.GroupSocialMedia.Add(GroupSocialMedia);
            }

            // Save changes to the context
            _context.SaveChanges();

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

            // Update GroupSocialMedia entries
            // First, remove existing entries
            var existingGroupSocialMedia = _context.GroupSocialMedia.Where(gsm => gsm.GroupId == existingGroup.Id).ToList();
            _context.GroupSocialMedia.RemoveRange(existingGroupSocialMedia);

            // Add the new entries
            foreach (var id in request.Id)
            {
                var groupSocialMedia = new GroupSocialMedia
                {
                    GroupId = existingGroup.Id,
                    SocialMediaId = id
                };

                _context.GroupSocialMedia.Add(groupSocialMedia);
            }

            // Save changes to the context
            _context.SaveChanges();

            return Ok(new
            {
                Status = "True",
                Message = "Group updated successfully",
                Data = existingGroup
            });
        }


    }

}



