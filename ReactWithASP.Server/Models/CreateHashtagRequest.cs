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
        public string userGUId { get; set; }
        public string HashtagGroupName { get; set; }
        public List<string> Hashtags { get; set; }
    }

    public class UpdateHashtagGroupRequest
    {
        public string userGUId { get; set; }
        public string HashtagGroupName { get; set; }
        public List<string> Hashtags { get; set; }
        public int GroupId { get; set; }
    }

    public class CreateHashtagGroupResponse
    {
        public string Message { get; set; }
    }

}
