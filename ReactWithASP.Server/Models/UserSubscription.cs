using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReactWithASP.Server.Models
{
    public class UserSubscriptions
    {
        [Key] // This annotation marks the property as the primary key
        public int Id { get; set; } // Assuming you want an auto-incrementing ID

        public string UserGuid { get; set; }
        public int SubsPlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Status { get; set; }
        public bool ManualSubscription { get; set; }
    }

    public partial class SaveUserSubscription
    {
       // public int Id { get; set; }
        public string UserGuid { get; set; } = null!;
        public string PlanName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = null!;
        public bool ManualSubscription { get; set; }


    }

    public class GetUserTransectionsDeailsbyId
    {
        
        public string UserGuid { get; set; }
        public string UserName { get; set; }
        public string PaymentId { get; set; }
        public string PayerId { get; set; }
        public string Token { get; set; }
        public int PlanId { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool PaymentStatus { get; set; }
    }

    public class UserSubscriptionPlanDeailsbyId
    { 
        public int PlanId { get; set; }
        public string PlanName { get; set; }
        public string UserName { get; set; }
        public string UserGuid { get; set; }
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

  public class UserSubscriptionPlan
  {
    [Key] 
    public int Id { get; set; } 
    public string UserGuid { get; set; }
    public int SubsPlanId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime CreatedOn { get; set; }
    public bool Status { get; set; }
    public bool isrenew { get; set; }
    public DateTime? ModifiedOn { get; set; }

  }

}
