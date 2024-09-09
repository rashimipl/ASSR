using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ReactWithASP.Server.Models
{
    public class CreatePostRequest
    {
        [Required(ErrorMessage = "UserGuid is required.")]
        public string userGUId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title length can't be more than 100.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description length can't be more than 500.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "At least one MediaUrl is required.")]
        [MinLength(1, ErrorMessage = "At least one MediaUrl is required.")]

        public List<string> MediaUrl { get; set; }
        //public List<IFormFile> MediaUrl { get; set; }

        [Required(ErrorMessage = "At least one Tag is required.")]
        [MinLength(1, ErrorMessage = "At least one Tag is required.")]
        public List<string> Tags { get; set; }

        [Required(ErrorMessage = "AccountOrGroupName is required.")]
        public string AccountOrGroupName { get; set; }
        public string deviceTokens { get; set; }
        public string[] NotificationIds { get; set; }

        /*[Required(ErrorMessage = "At least one AccountOrGroupId is required.")]
        [MinLength(1, ErrorMessage = "At least one AccountOrGroupId is required.")]*/
        public List<string> AccountOrGroupId { get; set; }
    }


    public class CreateDraftRequest
    {
        //public int Id { get; set; }
        [Required(ErrorMessage = "UserGuid is required.")]
        public string userGUId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title length can't be more than 100.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description length can't be more than 500.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "At least one MediaUrl is required.")]
        [MinLength(1, ErrorMessage = "At least one MediaUrl is required.")]

        public List<string> MediaUrl { get; set; }
        //public List<IFormFile> MediaUrl { get; set; }

        [Required(ErrorMessage = "At least one Tag is required.")]
        [MinLength(1, ErrorMessage = "At least one Tag is required.")]
        public List<string> Tags { get; set; }

        [Required(ErrorMessage = "AccountOrGroupName is required.")]
        public string AccountOrGroupName { get; set; }

        /*[Required(ErrorMessage = "At least one AccountOrGroupId is required.")]
        [MinLength(1, ErrorMessage = "At least one AccountOrGroupId is required.")]*/
        public List<string> AccountOrGroupId { get; set; }
    }

    public class CreatePostResponse
    {
        public string Message { get; set; }
    }

    public class MediaSelectionRequest
    {
        public List<IFormFile> MediaFiles { get; set; }
    }
    public class MediaFileResponse
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string ContentType { get; set; }
        public string ThumbnailUrl { get; set; }
    }
    public class MediaSelectionResponse
    {
        public string Message { get; set; }
        public List<MediaFileResponse> ProcessedMediaFiles { get; set; }
    }



    /*public class ScheduledPost
    {
        public int Id { get; set; }
        public string UserGuid { get; set; }
      //  public string UserName { get; set; }
        public string ScheduledType { get; set; }
        public string? Days { get; set; }
        public string? Months { get; set; }
        public string ScheduledTime { get; set; }
        public string? ScheduledDate { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public bool IsPublished { get; set; }
        //public bool IsActive { get; set; }
    }*/

    public class ScheduledPost
    {
        public int Id { get; set; }
        public string UserGuid { get; set; }
        public string ScheduledType { get; set; }
        public string? Days { get; set; }
        public string? Months { get; set; }
        public string ScheduledTime { get; set; }
        public string? ScheduledDate { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public bool IsPublished { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string MediaUrl { get; set; }
        public string Tags { get; set; }
        public string AccountOrGroupName { get; set; }
        public string AccountOrGroupId { get; set; }
        public string DeviceToken { get; set; }
        public DateTime createdOn { get; set; }
    }

    public class CreateScheduledPostRequest
    {
        public int Id { get; set; }
        public string UserGuid { get; set; }
        public string ScheduledType { get; set; }
        public List<string>? Days { get; set; }
        public List<string>? Months { get; set; }
        public string ScheduledTime { get; set; }
        public string DeviceToken { get; set; }
        public List<string>? ScheduledDate { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public bool IsPublished { get; set; }
    }
}
