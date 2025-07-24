using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ReactWithASP.Server.Models
{
  public class Transection
  {
    
    public int Id { get; set; }
    
    public string UserGuid { get; set; }    
    public int PayPalTranId { get; set; }
    public string PaymentId { get; set; }

    public string PayerId { get; set; }

    public string TransactionId { get; set; }
    
    public decimal Price { get; set; }
    public DateTime CreatedOn { get; set; }
    public bool PaymentStatus { get; set; }
  }
}
