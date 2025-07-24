using Newtonsoft.Json;

namespace ReactWithASP.Server.Models
{
  public class PostTweetReqestDto
  {
    [JsonProperty("text")]
    public string Text { get; set; } = string.Empty;
  }
}
