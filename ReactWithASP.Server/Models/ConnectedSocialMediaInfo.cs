namespace ReactWithASP.Server.Models
{
    public class ConnectedSocialMediaInfo
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int? SocialMediaAccId { get; set; }

        public string? SocialMediaAccName { get; set; }

        public string? PageId { get; set; }

        public string? PageName { get; set; }

        public string? PageProfile { get; set; }

        public string? PageAccessToken { get; set; }

   
  }


  //public class ConnectedSocialMediaDetail
  //{

  //    public string UserId { get; set; }

  //    public string PageId { get; set; }

  //    public string PageName { get; set; }

  //    public string PageProfile { get; set; }

  //    public string PageAccessToken { get; set; }
  //}
  public class ConnectedSocialMediaDetail
  {
    public string UserId { get; set; }
    public string socialMediaId { get; set; }
    public List<PageDetail> Pages { get; set; }
  }
  public class ConnectedSocialMediaAccDetail
    {
        public string UserId { get; set; }
        public string socialMediaId { get; set; }
        public string AccessToken { get; set; }
  }

    public class PageDetail
    {
        public string PageId { get; set; }
        public string PageName { get; set; }
        public string PageProfile { get; set; }
        public string PageAccessToken { get; set; }
    }

    public class LinkedInPostStatistics
    {
        public int ShareCount { get; set; }
        public int LikeCount { get; set; }
        public int ViewCount { get; set; }
        public int CommentCount { get; set; }
    }

    public class PostEngagementStats
    {
        public int Likes { get; set; }
        public int Shares { get; set; }
        public int Views { get; set; }
    }
  public class TwitterTokenResponse
  {
    public string token_type { get; set; }
    public string access_token { get; set; }
    public int expires_in { get; set; }
    public string scope { get; set; }
  }
  public class SocialMediaUsersModels
  {
    public int Id { get; set; }
    public string UserId { get; set; }               // UserGuid
    public string SocialMediaAccountId { get; set; } // Twitter Account Id
    public string AccessToken { get; set; }
            // e.g., "Twitter"
  }
}
