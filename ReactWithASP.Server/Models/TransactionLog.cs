using PayPal.Api;

namespace ReactWithASP.Server.Models
{
  public class TransactionLog
  {
    public int Id { get; set; }    
    public string? UserGuid { get; set; }
    public int TransactionID { get; set; }
    public string? PaymentId { get; set; }  
    public string? Event { get; set; }  
    public DateTime CreatedOn { get; set; }      
  
  }
}
