using System.ComponentModel.DataAnnotations;

namespace ReactWithASP.Server.Models
{
    public class CreateHashtagRequest
    {
        public string userGUId { get; set; }
        public List<string> Hashtags { get; set; }
    }

    public class CreateHashtagResponse
    {
        public string Message { get; set; }
    }

    public class CreateHashtagGroupRequest
    {
        [Required(ErrorMessage = "userGUId is required.")]
        public string userGUId { get; set; }

        [Required(ErrorMessage = "name is required.")]
        public string name { get; set; }

        [Required(ErrorMessage = "Hashtags is required.")]
        public List<string> Hashtags { get; set; }
    }

    public class UpdateHashtagGroupRequest
    {
        public string userGUId { get; set; }
        public string name { get; set; }
        public List<string> Hashtags { get; set; }
        public int Id { get; set; }
    }

    public class CreateHashtagGroupResponse
    {
        public string Message { get; set; }
    }

}
