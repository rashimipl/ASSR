using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JWTAuthentication.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ReactWithASP.Server.Controllers
{
  public class SubscriptionExpirationChecker : BackgroundService
  {
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<SubscriptionExpirationChecker> _logger;

    public SubscriptionExpirationChecker(IServiceScopeFactory serviceScopeFactory, ILogger<SubscriptionExpirationChecker> logger)
    {
      _serviceScopeFactory = serviceScopeFactory;
      _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      _logger.LogInformation("Subscription Expiration Checker started.");

      while (!stoppingToken.IsCancellationRequested)
      {
        try
        {
          using (var scope = _serviceScopeFactory.CreateScope())
          {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var expiredSubscriptions = dbContext.UserSubscriptionPlan
                .Where(s => s.Status == true && s.EndDate <= DateTime.UtcNow)
                .ToList();

            if (expiredSubscriptions.Any())
            {
              foreach (var subscription in expiredSubscriptions)
              {
                subscription.Status = false;  // Mark as inactive
                subscription.ModifiedOn = DateTime.UtcNow;
              }

              dbContext.SaveChanges();
              _logger.LogInformation($"Marked {expiredSubscriptions.Count} subscriptions as expired.");
            }
          }
        }
        catch (Exception ex)
        {
          _logger.LogError($"Error in Subscription Expiration Checker: {ex.Message}");
        }

        // Wait for 24 hours before checking again
        await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
      }
    }
  }

}
