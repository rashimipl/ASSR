using PayPal.Api;

namespace ReactWithASP.Server.Models
{
  public class PolicyContents
  {
    public int Id { get; set; }
    public string Type { get; set; } // "Terms" or "Privacy"
    public string Content { get; set; }
    public DateTime CreatedOn { get; set; } 
    public DateTime? UpdatedOn { get; set; }

  }
  public class ContentRequest
  {
    public int Index { get; set; }     // 0 = Terms, 1 = Privacy
    public string Content { get; set; }
  }
}
