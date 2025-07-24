using ReactWithASP.Server.Models.Posts;
using System.ComponentModel.DataAnnotations;

namespace ReactWithASP.Server.Models
{
    public class UserStatusRequest
    {
        [Required]
        public string UserGUID { get; set; }
    }
    public class UserStatusResponse
    {
        public string UserGUID { get; set; }
        public string Status { get; set; }
        public string Subscription { get; set; }
        public string SubscriptionStatus { get; set; }
        public DateTime SubscriptionExpiringOn { get; set; }

    }
    public class UserSubscriptionRequest
    {
        public string UserGUID { get; set; }
    }

    public class UserSubscriptionResponse
    {
        public string UserGUID { get; set; }
        public string Status { get; set; } // "E" for Enabled, "D" for Disabled
        public string Subscription { get; set; } // "1" for Free, "2" for Standard, "3" for Premium
        public string SubscriptionStatus { get; set; } // "Valid" or "Expired"
        public DateTime SubscriptionExpiringOn { get; set; }
    }


    public class SubscriptionPlan
    {
        public int Id { get; set; }
        public int PlanId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int ConnectedChannels { get; set; }
        public int SmartContentSuggestionsMonthly { get; set; }
        public int ImageSuggestionsMonthly { get; set; }
        public string DailyPostInspirations { get; set; }
        public string DraftedPosts { get; set; }
        public string PostsDaily { get; set; }
        public string ScheduledPostsQueue { get; set; }
        public bool MultiImageVideoPosts { get; set; }
        public bool RecurringPosts { get; set; }
        public bool PremiumSupport { get; set; }
    }

    public class subscriptionplanbyuserid
    {
        public string UserGUID { get; set; }
        public string PlanName { get; set; }
        public decimal Price { get; set; }
        public int SubsPlanID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class CreatePlanRequest
    {
        public int Id { get; set; }
        public int PlanId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int ConnectedChannels { get; set; }
        public int SmartContentSuggestionsMonthly { get; set; }
        public string ImageSuggestionsMonthly { get; set; }
        public string DailyPostInspirations { get; set; }
        public string DraftedPosts { get; set; }
        public string PostsDaily { get; set; }
        public string ScheduledPostsQueue { get; set; }
        public bool MultiImageVideoPosts { get; set; }
        public bool RecurringPosts { get; set; }
        public bool PremiumSupport { get; set; }
    }
  public class CreateSubFeatureRequest
  {
    //public string FeatureCode { get; set; }
    public string FeatureName { get; set; }
    public string Free { get; set; }
    public string Standard { get; set; }
    public string Premium { get; set; }
    public bool Status { get; set; }    
  }

  public class subscriptionplanResponse
  {
    public int PackageId { get; set; }
    public string PackageName { get; set; }
    public string Price { get; set; }
    public List<subscriptionFeature> subscriptionFeature { get; set; }
  }
  public class subscriptionFeature
  {
    public string FeatureCode { get; set; }
    public string FeatureName { get; set; }
    public int Allowedcount { get; set; }
    public int Isunlimited { get; set; }
    public bool Status { get; set; }
    public int Validity_days { get; set; }
  }

  
}
