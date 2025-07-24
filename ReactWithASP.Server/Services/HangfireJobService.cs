using JWTAuthentication.Authentication;
using Microsoft.AspNetCore.Identity;
using ReactWithASP.Server.Controllers;

namespace ReactWithASP.Server.Services
{
  public interface IHangfireJobService
  {
    void TestCronExpTestService();
    void RenewSubscriptions();
  }
  public class HangfireJobService : IHangfireJobService
  {
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment hostingEnvironment;

    public HangfireJobService(ApplicationDbContext context,  IWebHostEnvironment environment)
    {
      _context = context;

      hostingEnvironment = environment;
    }
    public void TestCronExpTestService()
    {      
      try
      {
        var expiredSubscriptions = _context.UserSubscriptionPlan
            .Where(s => s.EndDate < DateTime.Now && s.Status == true)
            .ToList();
        if (expiredSubscriptions.Count() > 0)
        {
          foreach (var sub in expiredSubscriptions)
          {
            sub.Status = false;  
          }
          _context.SaveChanges();
        }        
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error in TestCronExpTestService: {ex.Message}");
      }
    }
    public void RenewSubscriptions()
    {
      try
      {
        var subscriptionsToRenew = _context.UserSubscriptionPlan
            .Where(s => s.isrenew == true && s.Status == true && s.EndDate < DateTime.Now)
            .ToList();

        if (subscriptionsToRenew.Any())
        {
          foreach (var sub in subscriptionsToRenew)
          {
            if (sub.isrenew)
            {
              sub.Status = true;
              //sub.EndDate = DateTime.Now.AddMonths(1); 
            }
            else
            {
              sub.Status = false;
            }
          }
          _context.SaveChanges();
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error in RenewSubscriptions: {ex.Message}");
      }

    }
  }
}
