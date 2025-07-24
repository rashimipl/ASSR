namespace ReactWithASP.Server.Models.Posts
{
    public class Post
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ContentType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string MediaURL { get; set; }
        public DateTime CreatedAt { get; set; }

        public int GroupId { get; set; }
        public group Group { get; set; } // Ensure this relationship is correctly set
        public int Likes { get; set; }
        public int Views { get; set; }
        public int Comments { get; set; }
        public int Shares { get; set; }
        public string Status { get; set; }
        public int StatusCode { get; set; }
    }


    public class Drafts
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PostIcon { get; set; }
        public string PageId { get; set; }

        /* public int PostLikesCount { get; set; }
         public int PostSharesCount { get; set; }
         public int PostViewsCount { get; set; }*/
        public string Status { get; set; }
        public string UserGuid { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? AccountOrGroupName { get; set; }
        public string? AccountOrGroupId { get; set; }
        //public string? MediaUrl { get; set; }

        //public ICollection<PostLikes> PostLikes { get; set; }
        //public ICollection<PostShares> PostShares { get; set; }
        //public ICollection<PostViews> PostViews { get; set; }

     public string ContentType { get; set; }
    }
    public class SocialMediaPosts
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PostIcon { get; set; }
      
        /* public int PostLikesCount { get; set; }
         public int PostSharesCount { get; set; }
         public int PostViewsCount { get; set; }*/
        public string Status { get; set; }
        public string UserGuid { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? AccountOrGroupName { get; set; }
        public string? AccountOrGroupId { get; set; }
    //public string? MediaUrl { get; set; }

    //public ICollection<PostLikes> PostLikes { get; set; }
    //public ICollection<PostShares> PostShares { get; set; }
    //public ICollection<PostViews> PostViews { get; set; }
    //public List<string> Tags { get; set; }
    public string ContentType { get; set; }
    public string? AccountPageId { get; set; }

  }

  public class PostLikes
    {
        public int Id { get; set; }
        public string PostId { get; set; }
        public string UserGuid { get; set; }
        public string ReactionType { get; set; }
        // public SocialMediaPosts Post { get; set; }
        public int PostLikesCount { get; set; }
        public DateTime CreatedAt { get; set; }
        //public SocialMediaPosts SocialMediaPosts { get; set; }
    }

    public class PostShares
    {
        public int Id { get; set; }
        public string PostId { get; set; }
        public string UserGuid { get; set; }
        public int PostSharesCount { get; set; }
        public DateTime CreatedAt { get; set; }
        //public SocialMediaPosts SocialMediaPosts { get; set; }
    }
    public class PostViews
    {
        public int Id { get; set; }
        public string PostId { get; set; }
        public string UserGuid { get; set; }
        public int PostViewsCount { get; set; }
        public DateTime CreatedAt { get; set; }
        //public SocialMediaPosts SocialMediaPosts { get; set; }
    }

    public class UserGroupPosts
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int GroupId { get; set; }
        public int StoryId { get; set; }
       public string AccountID { get; set; }
       public string AccountPageId { get; set; }

  }
}
