using PayPal.Api;
using System.ComponentModel.DataAnnotations;

namespace ReactWithASP.Server.Models
{
  public class HelpArticle
  {
    public int Id { get; set; }
    [Required]
    public string Question { get; set; }
    [Required]
    public string Answer { get; set; }
    public DateTime? CreatedOn { get; set; } 
    public DateTime? UpdatedOn { get; set; } 

  }
}
