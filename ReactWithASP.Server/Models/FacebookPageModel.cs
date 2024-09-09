using System.ComponentModel.DataAnnotations;

namespace ReactWithASP.Server.Authentication
{
    public class FacebookPageModel
    {
        [Key]
        public int Id { get; set; }  // Primary key property
        public string UserId { get; set; }
        public string AccessToken { get; set; }

    }
    public class FacebookPageData
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Profile { get; set; }
        public string AccessToken { get; set; }
    }
}