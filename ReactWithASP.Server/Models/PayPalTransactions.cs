namespace ReactWithASP.Server.Models
{
    public class PayPalTransactions
    {
        public int Id { get; set; }
        public string PaymentId { get; set; }
        public string UserGuid { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

}
