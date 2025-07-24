namespace ReactWithASP.Server.Models
{
    public class PostIdForSocialMediaPosts
    {
        public int Id { get; set; }
        public string PostId { get; set; }  // The post ID returned from Facebook
        public string userGUId { get; set; }
        public string PageId { get; set; }
        public string PageAccessToken { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
