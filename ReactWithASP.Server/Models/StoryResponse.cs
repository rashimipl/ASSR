using ReactWithASP.Server.Models.Posts;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ReactWithASP.Server.Models
{

    public class StoryResponse
    {
        public Groupes Group { get; set; }
        public string StoryTime { get; set; } // Changed to TimeSpan
        public string PostIcon { get; set; }
        public int Id { get; set; }
        public int Likes { get; set; }
        public int Views { get; set; }
        public int Comments { get; set; }
        public int Shares { get; set; }
        public string Status { get; set; }
        public int StatusCode { get; set; } // 0, 1, 2 or pp, ds, en
    public string UserGuid { get; set; }
    //public string platform { get; set; }
    public AccountResponses Account { get; set; }
  }

  public class AccountResponses
  {
    public string[] SocialMediaName { get; set; }
    public string UserGuid { get; set; }
  }
  public class PostStoryRequest
  {
    [Required(ErrorMessage = "UserGuid is required.")]
    public string userGUId { get; set; }

    [Required(ErrorMessage = "MediaUrl is required.")]
    //[Url(ErrorMessage = "MediaUrl must be a valid URL.")]
    //public string mediaUrl { get; set; }
    public List<string> mediaUrl { get; set; }

    [Required(ErrorMessage = "AccountOrGroupName is required.")]
    public string accountOrGroupName { get; set; }

    /*[Required(ErrorMessage = "At least one AccountOrGroupId is required.")]
    [MinLength(1, ErrorMessage = "At least one AccountOrGroupId is required.")]*/
    public List<string> accountOrGroupId { get; set; }
  }

  public class PostStoryResponse
    {
        public string Message { get; set; }
    }

  public class PhotoUploadAndPublishRequest
  {
    public string AccessToken { get; set; }
    public string Url { get; set; }
    public string PageId { get; set; }
  }

  public class FacebookPhotoResponse
  {
    public string Id { get; set; }
  }

  public class FacebookStoryResponse
  {
    [JsonPropertyName("post_id")]
    public string PostId { get; set; }
  }
  public class VideoStoryRequest
  {
    public string AccessToken { get; set; }
    public string PageId { get; set; }

  }

  public class VideoUploadInitResponse
  {
    [JsonPropertyName("video_id")]
    public string VideoId { get; set; }

    [JsonPropertyName("upload_url")]
    public string UploadUrl { get; set; }
  }


  public class FacebookFeedRequest
  {
    public string Message { get; set; } // Message for the feed post
    public List<string> MediaFbIds { get; set; } // List of media_fbid strings
    public string AccessToken { get; set; } // Access token
  }

  ////public class FacebookPostRequest
  ////{
  ////  public string PageId { get; set; }
  ////  public string accessToken { get; set; }    
  ////  public string Message { get; set; }
  ////  public List<IFormFile> MediaFiles { get; set; }
  ////}

  //public class FacebookPostRequest
  //{
  //  public string PageId { get; set; }
  //  public string Message { get; set; }
  //  public string accessToken { get; set; }

  //}
  //public class StoryPostRequest
  //{
  //  [Required(ErrorMessage = "UserGuid is required.")]
  //  public string userGUId { get; set; }

  //  [Required(ErrorMessage = "At least one MediaUrl is required.")]
  //  [MinLength(1, ErrorMessage = "At least one MediaUrl is required.")]
  //  public List<string> MediaUrl { get; set; }
    
  //  [Required(ErrorMessage = "AccountOrGroupName is required.")]
  //  public string AccountOrGroupName { get; set; }    
  //  public List<AccountOrPageId1> AccountOrGroupId { get; set; }
  
  //}
  public class CreateStoryRequest
  {
    [Required(ErrorMessage = "UserGuid is required.")]
    public string userGUId { get; set; }

    [Required(ErrorMessage = "At least one MediaUrl is required.")]
    [MinLength(1, ErrorMessage = "At least one MediaUrl is required.")]
    //public List<string> MediaUrl { get; set; }
    public string MediaUrl { get; set; }
    [Required(ErrorMessage = "AccountOrGroupName is required.")]
    public string AccountOrGroupName { get; set; }
    public List<AccountOrPageId2> AccountOrGroupId { get; set; }

  }

  public class AccountOrPageId2
  {
    public string Id { get; set; }
    public string? PageId { get; set; }
    public string? accountId { get; set; }

  }
  public class Groupes
  {
    public int Id { get; set; }
    public string Name { get; set; }
    /*public List<string> Platform { get; set; }*/
    public string GroupIcon { get; set; }
    public string UserGuid { get; set; }
    public string[] Platform { get; set; }

  }
  
  public class StoryPlannerSchedule
  {
    public string UserGUId { get; set; }
    public int? NoOfPosts { get; set; } = 5; // Default value is 5
  }
}
