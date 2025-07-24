using ReactWithASP.Server.Models.Posts;

namespace ReactWithASP.Server.Models
{
    public class InsightRequest
    {
        public string Days { get; set; }
        public string UserGuid { get; set; }
       
    }
    /*public class InsightDataResponse
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int Views { get; set; }
        public int Followers { get; set; }
        public int Likes { get; set; }
        public int Share { get; set; }
        public InsightsResponse InsightResponse { get; set; }

    }*/
    public class HomeScreenRequest
    {
        public string Days { get; set; }
        public string UserGuid { get; set; }
        public int? NoOfPosts { get; set; } = 5;
    }

    public class RRequest
    {
        public string Days { get; set; }
        public string UserGuid { get; set; }
        public int? NoOfPosts { get; set; } = 5;
    }

    public class HomeScreenResponse
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int Views { get; set; }
        public int Followers { get; set; }
        public int Likes { get; set; }
        public int Share { get; set; }
        public string UserGuid { get; set; }

    }
    public class PostResponse
    {
        public Groups Group { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PostIcon { get; set; }
        public int Id { get; set; }
        public int Likes { get; set; }
        public int Views { get; set; }
        public int Comments { get; set; }
        public int Shares { get; set; }
        public string Status { get; set; }
        public int StatusCode { get; set; } // 0, 1, 2 or pp, ds, en
        public string TagsGroup { get; set; }
        public string SelectedGroup { get; set; }

    }

    /*public class PostResponse1
    {
        public GroupResponse Group { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PostIcon { get; set; }
        public int Id { get; set; }
        public int Likes { get; set; }
        public int Views { get; set; }
        public int Comments { get; set; }
        public int Shares { get; set; }
        public string Status { get; set; }
        public int StatusCode { get; set; } // 0, 1, 2 or pp, ds, en
        public string TagsGroup { get; set; }
        public string SelectedGroup { get; set; }

    }*/

    public class PostResponse1
    {
        public GroupResponse Group { get; set; }
        public string Title { get; set; }
        public int Id { get; set; }
        public string Description { get; set; }
        public string PostIcon { get; set; }
        public int SocialMediaPostId { get; set; }
        public int Likes { get; set; }
        public int Views { get; set; }
        public int Comments { get; set; }
        public int Shares { get; set; }
        public string Status { get; set; }
        public int StatusCode { get; set; } // 0, 1, 2 or pp, ds, en
        public string TagsGroup { get; set; }
        public string SelectedGroup { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserGuid { get; set; }
        public string platform { get; set; }
        public bool PostStatus { get; set; }
   
    public AccountResponse Account { get; set; }
  }


    public class PostDateWise
    {
        public DateGroup Date { get; set; }
    }
    public class DateGroup
    {
        public string Value { get; set; }
        public List<PostResponse> Data { get; set; }
    }
    public class Groups
    {

    public int Id { get; set; }
    public string UserGuid { get; set; }
    public string Name { get; set; }
    public string groupIcon { get; set; }
    public int SocialMediaId { get; set; }
    /*public List<string> Platform { get; set; }*/
    public virtual ICollection<GroupSocialMedias> GroupSocialMedias { get; set; }

    public ICollection<GroupPlatform> Platforms { get; set; } = new List<GroupPlatform>();
  }
    public class GroupPlatform
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string Platform { get; set; }

        public Groups Group { get; set; }
    }
    public class GroupSocialMedias
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        /*public Group Group { get; set; }*/
        public int SocialMediaId { get; set; }
        public SocialMedia SocialMedia { get; set; }
    }
    public class ScheduledPostsRequest
    {
        public string UserGUId { get; set; }
        public int? NoOfPosts { get; set; } = 5; // Default value is 5
    }
    public class ScheduledPostResponse
    {
        public Groups Group { get; set; }
        public List<string> AccountIcon { get; set; }
        
        public string Title { get; set; }
        public string Description { get; set; }
        public string PostIcon { get; set; }
        public int Id { get; set; }
        public string Status { get; set; }
        public string ScheduledTimeString { get; set; }
        public bool Poststatus { get; set; }
        public DateTime ScheduledTime { get; set; }
    public DateTime ScheduledDate { get; set; }
  }
}
