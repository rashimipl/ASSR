using PayPal.Api;

namespace ReactWithASP.Server.Models
{
  public class PaymentDetails
  {
    public int Id { get; set; }
    public string UserGuid { get; set; }
    public int SubPlanId { get; set; }
    public string PaymentId { get; set; }
    public DateTime CreatedOn { get; set; }

  }
}
