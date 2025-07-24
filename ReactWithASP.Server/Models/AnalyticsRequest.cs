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

  public class FacebookPostAnalytics
  {
    public string PostId { get; set; }
    public int Reach { get; set; }
    public int Likes { get; set; }
    public int Shares { get; set; }
    public int Views { get; set; }
  }
  public class FacebookInsightsResponse
  {
    public List<MetricData> data { get; set; }
  }

  public class MetricData
  {
    public string name { get; set; }
    public List<MetricValue> values { get; set; }
  }

  public class MetricValue
  {
    public int value { get; set; }
  }

  public class FacebookLikesResponse
  {
    public FacebookLikeSummary summary { get; set; }
  }

  public class FacebookLikeSummary
  {
    public int total_count { get; set; }
  }

  public class FacebookSharesResponse
  {
    public FacebookShares shares { get; set; }
  }

  public class FacebookShares
  {
    public int count { get; set; }
  }

  public class FacebookVideoViewsResponse
  {
    public List<MetricData> data { get; set; }
  }
  public class FacebookPostsResponse
  {
    public List<FacebookPostData> data { get; set; }
  }

  public class FacebookPostData
  {
    public string id { get; set; }
    public string created_time { get; set; }
  }
  public class FacebookPageResponse
  {
    public List<FacebookPage> data { get; set; }
  }

  public class FacebookPage
  {
    public string name { get; set; }
    public string id { get; set; }
    public string access_token { get; set; }
  }

  //public class FacebookInsightsMW
  //{
  //  public List<FacebookInsightData> data { get; set; }
  //}

  //public class FacebookInsightData
  //{
  //  public List<FacebookInsightValueWrapper> Values { get; set; }
  //}

  //public class FacebookInsightValueWrapper
  //{
  //  public Dictionary<string, int> Value { get; set; }
  //}
}
