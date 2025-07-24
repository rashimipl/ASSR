using PayPal.Api;

namespace ReactWithASP.Server.Models
{
  public class Review
  {
    public int Id { get; set; }
    public string UserGuid { get; set; }
    public int Rating { get; set; }
    public string Comments { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
  }
}
