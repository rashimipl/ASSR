namespace ReactWithASP.Server.Models
{
    public class AnalyticsRequest
    {
        public string userGUId { get; set; }
        public string? SocialMedia { get; set; }
        //public string Searchbox { get; set; }
        public int Days  { get; set; }
        //public DateTime? Date { get; set; }

    }
    public class AnalyticsResponse
    {
        public int TotalViews { get; set; }
        public int TotalLikes { get; set; }
        public int TotalShares { get; set; }
        public int TotalReach { get; set; }

    }

    public class InsightsResponse
    {
        public int TotalViews { get; set; }
        public int TotalLikes { get; set; }
        public int TotalShares { get; set; }
        public int TotalFollowers { get; set; }
        public bool ViewStatus { get; set; }
        public bool LikeStatus { get; set; }
        public bool ShareStatus { get; set; }
        public bool FollowerStatus { get; set; }
    }
    public class AudienceRequest
    {
        public string userGUId { get; set; }
        public string? SocialMedia { get; set; }
        public int Days { get; set; }

    }
    public class AudienceResponse
    {
        public int Man { get; set; }
        public int Woman { get; set; }
    }
}
