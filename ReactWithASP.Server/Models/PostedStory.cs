using System.ComponentModel.DataAnnotations;

namespace ReactWithASP.Server.Models
{

    public class PostedStory
    {
        public int Id { get; set; }
        public string UserGuid { get; set; }
        public DateTime CreatedAt { get; set; } 
        public string Status { get; set; } 
        public string AccountOrGroupName { get; set; } 
        public string AccountOrGroupId { get; set; } 
        public string PostIcon { get; set; }
        public string ContentType { get; set; }
        public string? AccountPageId { get; set; }
  }
}
