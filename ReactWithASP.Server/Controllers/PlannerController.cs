using JWTAuthentication.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReactWithASP.Server.Models;
using ReactWithASP.Server.Models.Posts;


namespace ReactWithASP.Server.Controllers
{
    [ApiController]
    [Route("api")]
    public class PlannerController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment;
        private readonly ApplicationDbContext _context;


        public PlannerController(IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment, ApplicationDbContext context)
        {
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            Environment = environment;
            _context = context;
            //FFmpeg.ExecutablesPath = @"C:\path\to\ffmpeg\bin"; // Set the path to the FFmpeg executables
        }

        //[HttpGet("Calendar")]
        //public IActionResult PlannerCalendar([FromQuery] Calendar request)
        //{
        //    var response = new List<ScheduledPostResponse>
        //    {
        //        new ScheduledPostResponse
        //        {
        //            Group = new Groups
        //             {
        //                 Name = "Musemind Design",
        //                /* Platform = new List<string> { "facebook", "instagram" },*/
        //                 groupIcon = "https://thumbs.dreamstime.com/b/teamwork-group-friends-icon-vector-illustration-teamwork-group-friends-icon-118637039.jpg"
        //             },
        //            Title = "Musmind Design Group",
        //            Description = "We are looking for Visual designer.",
        //            PostIcon = "https://www.shutterstock.com/image-photo/earth-day-environment-green-globe-260nw-2285503199.jpg",
        //            Id = 4,
        //            ScheduledTime = DateTime.Parse("2024-12-06T05:30:13.920")
        //        },
        //        new ScheduledPostResponse
        //        {
        //            Group = new Groups
        //             {
        //                 Name = "Musemind Design",
        //                 /*Platform = new List<string> { "facebook", "instagram" },*/
        //                 groupIcon = "https://thumbs.dreamstime.com/b/teamwork-group-friends-icon-vector-illustration-teamwork-group-friends-icon-118637039.jpg"
        //             },
        //            Title = "Think like a monk.",
        //            Description = "Monk from inside is a great feeling I ever imagined.",
        //            PostIcon = "https://www.shutterstock.com/image-photo/earth-day-environment-green-globe-260nw-2285503199.jpg",
        //            Id =3,
        //            ScheduledTime = DateTime.Parse("2024-12-07T06:45:00.000")
        //        }
        //    };
        //    return Ok(response);
        //}

        [HttpGet("Calendar")]
        public IActionResult PlannerCalendar([FromQuery] Calendar request)
        {
            if (request.Date == null)
            {
                return BadRequest(new { Message = "Date is required." });
            }

            var response = (from sdp in _context.ScheduledPost
                            join gp in _context.@group on sdp.UserGuid equals gp.UserGuid
                            join gsm in _context.GroupSocialMedia on gp.Id equals gsm.GroupId
                            join sm in _context.SocialMedia on gsm.SocialMediaId equals sm.Id
                            where sdp.UserGuid == request.userGUId && sdp.createdOn.Date == request.Date.Value.Date
                            orderby sdp.Id descending
                            group new { sdp, gp, sm } by new { sdp.Id, sdp.Title, sdp.Description, sdp.MediaUrl, gp.Name, gp.GroupIcon } into g
                            select new
                            {
                                Group = new GroupResponse
                                {
                                    Name = g.Key.Name,
                                    GroupIcon = g.Key.GroupIcon,
                                    Platform = g.Select(x => x.sm.SocialMediaName).Distinct().ToArray(),  // Return as a string array
                                },
                                Title = g.Key.Title,
                                Description = g.Key.Description,
                                PostIcon = g.Key.MediaUrl,
                                Id = g.Key.Id,
                                ScheduledTimeString = g.Select(x => x.sdp.ScheduledTime).FirstOrDefault(),
                            }).ToList();

            if (response.Count!=0)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(new { Message = "Data Not Found !...." });
            }

            
        }





        [HttpGet("PlannerScheduled")]
        [Authorize]
        public IActionResult PlannerScheduled([FromQuery] PlannerSchedule request)
        {
            if (request.month == null)
            {
                return BadRequest(new { Message = "Moth is required." });
            }

            if (request.year == null)
            {
                return BadRequest(new { Message = "year is required." });
            }
            int Month = request.month+1;


            var response = (from sdp in _context.ScheduledPost
                            join gp in _context.@group on sdp.UserGuid equals gp.UserGuid
                            join gsm in _context.GroupSocialMedia on gp.Id equals gsm.GroupId
                            join sm in _context.SocialMedia on gsm.SocialMediaId equals sm.Id
                            where sdp.UserGuid == request.userGUId && sdp.createdOn.Date.Month == Month && sdp.createdOn.Date.Year==request.year
                            orderby sdp.Id descending
                            group new { sdp, gp, sm } by new { sdp.Id, sdp.Title, sdp.Description, sdp.MediaUrl, gp.Name, gp.GroupIcon } into g
                            select new
                            {
                                Group = new GroupResponse
                                {
                                    Name = g.Key.Name,
                                    GroupIcon = g.Key.GroupIcon,
                                    Platform = g.Select(x => x.sm.SocialMediaName).Distinct().ToArray(),  // Return as a string array
                                },
                                Title = g.Key.Title,
                                Description = g.Key.Description,
                                PostIcon = g.Key.MediaUrl,
                                Id = g.Key.Id,
                                ScheduledTimeString = g.Select(x => x.sdp.ScheduledTime).FirstOrDefault(),
                            }).ToList();


                            if (!string.IsNullOrWhiteSpace(request.searchbox) && request.searchbox != "null")
                            {
                                var data = response.Where(d =>
                                                  d.Title.Contains(request.searchbox) ||
                                                  d.Description.Contains(request.searchbox) ||
                                                  d.Group.Name.Contains(request.searchbox) ||
                                                  d.Group.Platform.Contains(request.searchbox)).ToList();
                return Ok(data);
            }
            if (response.Count != 0)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(new { Message = "Data Not Found !...." });
            }
        }
        //public IActionResult PlannerScheduled([FromQuery] PlannerSchedule request)
        //{
        //    var response = new List<ScheduledPostResponse>
        //    {
        //        new ScheduledPostResponse
        //        {
        //            Group = new Groups
        //             {
        //                 Name = "Musemind Design",
        //                /* Platform = new List<string> { "facebook", "instagram" },*/
        //                 groupIcon = "https://thumbs.dreamstime.com/b/teamwork-group-friends-icon-vector-illustration-teamwork-group-friends-icon-118637039.jpg"
        //             },
        //            Title = "Musmind Design Group",
        //            Description = "We are looking for Visual designer.",
        //            PostIcon = "https://www.shutterstock.com/image-photo/earth-day-environment-green-globe-260nw-2285503199.jpg",
        //            Id = 4,
        //            ScheduledTime = DateTime.Parse("2024-12-06T05:30:13.920")
        //        },
        //        new ScheduledPostResponse
        //        {
        //            Group = new Groups
        //             {
        //                 Name = "Musemind Design",
        //                /* Platform = new List<string> { "facebook", "instagram" },*/
        //                 groupIcon = "https://thumbs.dreamstime.com/b/teamwork-group-friends-icon-vector-illustration-teamwork-group-friends-icon-118637039.jpg"
        //             },
        //            Title = "Think like a monk.",
        //            Description = "Monk from inside is a great feeling I ever imagined.",
        //            PostIcon = "https://www.shutterstock.com/image-photo/earth-day-environment-green-globe-260nw-2285503199.jpg",
        //            Id =3,
        //            ScheduledTime = DateTime.Parse("2024-12-07T06:45:00.000")
        //        }
        //    };
        //    return Ok(response);
        //}

        [HttpGet("PlannerDraft")]
        [Authorize]
        public IActionResult PlannerDraft([FromQuery] string searchbox, string UserGuid)
        {
            if (string.IsNullOrWhiteSpace(UserGuid))
            {
                return BadRequest(new { Message = "UserGuid is required." });
            }

            var response = (from df in _context.Drafts
                            join gp in _context.@group on df.UserGuid equals gp.UserGuid
                            join gsm in _context.GroupSocialMedia on gp.Id equals gsm.GroupId
                            join sm in _context.SocialMedia on gsm.SocialMediaId equals sm.Id
                            where df.UserGuid == UserGuid
                            orderby df.Id descending
                            group new { df, gp, sm } by new { df.Id, df.Title, df.Description, df.PostIcon, gp.Name, gp.GroupIcon } into g
                            select new
                            {
                                Group = new GroupResponse
                                {
                                    Name = g.Key.Name,
                                    GroupIcon = g.Key.GroupIcon,
                                    Platform = g.Select(x => x.sm.SocialMediaName).Distinct().ToArray(),  // Return as a string array
                                },
                                Title = g.Key.Title,
                                Description = g.Key.Description,
                                PostIcon = g.Key.PostIcon,
                                Id = g.Key.Id,
                            }).ToList();

            if (!string.IsNullOrWhiteSpace(searchbox) && searchbox != "null")
            {
                response = response.Where(d =>
                                  d.Title.Contains(searchbox) ||
                                  d.Description.Contains(searchbox) ||
                                  d.Group.Name.Contains(searchbox) ||
                                  d.Group.Platform.Any(p => p.Contains(searchbox)))
                                  .ToList();
            }

            if (response.Count!=0)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(new { Message = "Data Not Found !...." });
            }
        }



        //public IActionResult PlannerDraft([FromQuery] Schedule request)
        //{
        //    var response = new List<PostResponse>
        //    {
        //        new PostResponse
        //        {
        //            Group = new Groups
        //             {
        //                 Name = "Musemind Design",
        //                 /*Platform = new List<string> { "facebook", "instagram" },*/
        //                 groupIcon = "https://thumbs.dreamstime.com/b/teamwork-group-friends-icon-vector-illustration-teamwork-group-friends-icon-118637039.jpg"
        //             },

        //            Title = "Musmind Design Hiring",
        //            Description = "We are looking for Visual designer.",
        //            PostIcon = "https://www.shutterstock.com/image-photo/earth-day-environment-green-globe-260nw-2285503199.jpg",
        //            Id = 1,
        //            Likes = 1000,
        //            Views = 2000,
        //            Comments = 2000,
        //            Shares = 200,
        //            Status = "Draft",
        //            StatusCode = 1
        //        },
        //        new PostResponse
        //        {
        //            Group = new Groups
        //            {
        //                Name = "Musemind Design",
        //                /*Platform = new List<string> { "facebook", "instagram" },*/
        //                groupIcon = "https://thumbs.dreamstime.com/b/teamwork-group-friends-icon-vector-illustration-teamwork-group-friends-icon-118637039.jpg"
        //            },
        //            Title = "Think like a monk.",
        //            Description = "Monk from inside is a great feeling i ever Imagined.",
        //            PostIcon = "https://www.shutterstock.com/image-photo/earth-day-environment-green-globe-260nw-2285503199.jpg",
        //            Id = 2,
        //            Likes = 1000,
        //            Views = 2000,
        //            Comments = 2000,
        //            Shares = 200,
        //            Status = "Draft",
        //            StatusCode = 1
        //        }
        //    };
        //    return Ok(response);
        //}
    }
}
