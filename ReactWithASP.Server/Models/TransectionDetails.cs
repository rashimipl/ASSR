using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ReactWithASP.Server.Models
{
    public class TransectionDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string UserGuid { get; set; }
        [Required]
        public string PaymentId { get; set; }
        [Required]
        public string PayerId { get; set; }
        [Required]
        public string Token { get; set; }
        public int PlanId { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool PaymentStatus { get; set; }
    }


    public class SaveTransectionDetails
    {
        public string UserGuid { get; set; }
        public string PaymentId { get; set; }
        public string PayerId { get; set; }
        public string Token { get; set; }
    }



}

