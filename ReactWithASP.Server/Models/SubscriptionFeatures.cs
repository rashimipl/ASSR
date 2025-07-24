using System.ComponentModel.DataAnnotations;

namespace ReactWithASP.Server.Models
{
  public class SubscriptionFeatures
  {    
    public int ID { get; set; }
    public string FeatureCode { get; set; }
      public string FeatureName { get; set; }      
      public string Free { get; set; }
      public string Standard { get; set; }
      public string Premium { get; set; }   
      public bool Status { get; set; }
      
  }
  public class SubscriptionPackage
  {
    public int ID { get; set; }
    public int PackageId { get; set; }
    public string PackageName { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; }
    public int Validity_days { get; set; }
    public bool Status { get; set; }
    

  }
  public class SubscriptionPlanFeatures
  {
    public int ID { get; set; }
    public string FeatureCode { get; set; }
    public string FeatureName { get; set; }
    public int PackageId { get; set; }
    public int Allowedcount { get; set; }
    public int Isunlimited { get; set; }    
    public bool Status { get; set; }

  }

 
}
