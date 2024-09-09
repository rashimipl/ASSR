using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ReactWithASP.Server.Models;
using System.Linq;

namespace ReactWithASP.Server.Controllers
{
    [ApiController]
    [Route("api")]
    public class StoryController : Controller
    {
        [HttpGet("Story")]
        [Authorize]
        public IActionResult UserStory([FromQuery] Schedule request)
        {
            var response = new List<StoryResponse>
            {
                new StoryResponse
                {
                    Group = new Groups
                     {
                         Name = "Musemind Design",
                         /*Platform = new List<string> { "facebook", "instagram" },*/
                         groupIcon = "https://thumbs.dreamstime.com/b/teamwork-group-friends-icon-vector-illustration-teamwork-group-friends-icon-118637039.jpg"
                     },
                    StoryTime = "10:30:00",
                    PostIcon = "https://www.shutterstock.com/image-photo/earth-day-environment-green-globe-260nw-2285503199.jpg",
                    Id = 1,
                    Likes = 1000,
                    Views = 2000,
                    Shares = 200,
                    Status = "Story Published",
                    StatusCode = 1
                },
                new StoryResponse
                {
                    Group = new Groups
                    {
                        Name = "Musemind Design",
                        /*Platform = new List<string> { "facebook", "instagram" },*/
                        groupIcon = "https://thumbs.dreamstime.com/b/teamwork-group-friends-icon-vector-illustration-teamwork-group-friends-icon-118637039.jpg"
                    },
                    StoryTime = "05:30:00",
                    PostIcon = "https://www.shutterstock.com/image-photo/earth-day-environment-green-globe-260nw-2285503199.jpg",
                    Id = 2,
                    Likes = 1000,
                    Views = 2000,
                    Shares = 200,
                    Status = "Story Published",
                    StatusCode = 1
                }
            };
            return Ok(response);
        }

        [HttpPost("PostStory")]
        [Authorize]
        public IActionResult PostStory([FromBody] PostStoryRequest request)
        {
            if (string.IsNullOrEmpty(request.userGUId) ||
                string.IsNullOrEmpty(request.mediaUrl) ||
                string.IsNullOrEmpty(request.accountOrGroupName) ||
                request.accountOrGroupId == null || request.accountOrGroupId.Count == 0)
            {
                return BadRequest(new PostStoryResponse
                {
                    Message = "Fields Missing"
                });
            }

            /*if (!new List<string> { "Publish now", "Schedule", "Save as draft", "Cancel" }.Contains(request.Action))
            {
                return BadRequest(new PostStoryResponse
                {
                    Message = "Invalid Action"
                });
            }*/

            

            return Ok(new PostStoryResponse
            {
                Message = "Story Posted successfully"
            });
        }
    }
}
