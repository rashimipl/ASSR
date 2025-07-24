namespace ReactWithASP.Server.Models
{
    /*public class SocialMediaGroup
    {
        public int Id { get; set; }
        public string name { get; set; }

        public string groupIcon { get; set; }
        public List<string> socialMediaUrl { get; set; }
    }*/

    public class SocialMediaGroup
    {
        public int Id { get; set; }
        public string PageId { get; set; }
        public string name { get; set; }
        public string UserGuid { get; set; }
        public string groupIcon { get; set; }
    public List<int> socialMediaId { get; set; }
    public List<string> socialMediaName { get; set; }
        public List<string> socialMediaUrl { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public int accountId { get; set; }
  }

    public class CreateGroupRequest
    {
        public string GroupName { get; set; }
        public string GroupIcon { get; set; }
        public List<string> SocialMediaIcons { get; set; }
    }

    public class CreateGroupResponse
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupIcon { get; set; }
        public List<string> SocialMediaIcons { get; set; }
    }
}
