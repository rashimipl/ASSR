using System.ComponentModel.DataAnnotations;

namespace ReactWithASP.Server.Models
{
    public class StoryResponse
    {
        public Groups Group { get; set; }
        public string StoryTime { get; set; } // Changed to TimeSpan
        public string PostIcon { get; set; }
        public int Id { get; set; }
        public int Likes { get; set; }
        public int Views { get; set; }
        public int Comments { get; set; }
        public int Shares { get; set; }
        public string Status { get; set; }
        public int StatusCode { get; set; } // 0, 1, 2 or pp, ds, en
    }
    public class PostStoryRequest
    {
        [Required(ErrorMessage = "UserGuid is required.")]
        public string userGUId { get; set; }

        [Required(ErrorMessage = "MediaUrl is required.")]
        [Url(ErrorMessage = "MediaUrl must be a valid URL.")]
        public string mediaUrl { get; set; }

        [Required(ErrorMessage = "AccountOrGroupName is required.")]
        public string accountOrGroupName { get; set; }

        /*[Required(ErrorMessage = "At least one AccountOrGroupId is required.")]
        [MinLength(1, ErrorMessage = "At least one AccountOrGroupId is required.")]*/
        public List<string> accountOrGroupId { get; set; }
    }

    public class PostStoryResponse
    {
        public string Message { get; set; }
    }
}
