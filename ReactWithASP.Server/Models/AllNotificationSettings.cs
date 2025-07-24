using PayPal.Api;

namespace ReactWithASP.Server.Models
{
  public class AllNotificationSettings
  {
    public int Id { get; set; }
    public string UserGUid { get; set; }
    public bool AllNotifications { get; set; }
    public bool ActivityReminder { get; set; }
    public bool Errors { get; set; }
    public bool RemindBefore1Hour { get; set; }
    public bool PublishedPost { get; set; }

  }
  public class UpdateNotificationSettingsRequest
  {
    public bool AllNotifications { get; set; }
    public bool ActivityReminder { get; set; }
    public bool Errors { get; set; }
    public bool RemindBefore1Hour { get; set; }
    public bool PublishedPost { get; set; }
  }
}
