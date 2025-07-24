namespace ReactWithASP.Server.Models
{
    public class Payments
  {
        public int Id { get; set; }
        public string PaymentId { get; set; }
        public string UserGuid { get; set; }
        public int UserSubsPlanId { get; set; }
        public int SubsPlanID { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string PaymentMethod { get; set; }
  }

}
