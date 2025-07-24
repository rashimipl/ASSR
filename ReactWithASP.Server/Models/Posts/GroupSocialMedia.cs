namespace ReactWithASP.Server.Models.Posts
{
    /*public class GroupSocialMedia
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int SocialMediaId { get; set; }
        public group Group { get; set; }
        public SocialMedia SocialMedia { get; set; }
    }*/

    public class GroupSocialMedia
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int SocialMediaId { get; set; }
        public string? PageId { get; set; }
        public string? PageName { get; set; }
        
        public string? PageProfile { get; set; }
        
        public string? PageAccessToken { get; set; }
        public DateTime? CreatedOn { get; set; }
  }
}
