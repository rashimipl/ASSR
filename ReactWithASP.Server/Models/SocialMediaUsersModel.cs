using System.ComponentModel.DataAnnotations;

namespace ReactWithASP.Server.Models
{
    public class SocialMediaUsersModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string AccessToken { get; set; }
        public string SocialMedia { get; set; }
    }
}
