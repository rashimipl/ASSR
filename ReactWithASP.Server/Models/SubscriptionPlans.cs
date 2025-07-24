using System.ComponentModel.DataAnnotations;

namespace ReactWithASP.Server.Models
{
    public class SubscriptionPlans
    {
        [Key]
        public int Id { get; set; }
        public int PlanId { get; set; }
        public string PlanName { get; set; }
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
    
    public class UserSubsDuration
    {
        public string UserGuid { get; set; }
        public DateTime Startdate { get; set; }
        public DateTime Enddate { get; set; }
    }
}
