namespace ReactWithASP.Server.Models.Settings
{
    public class NotificationSetting
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UserGUID { get; set; }
        public string? DeviceToken { get; set; } 

        public string? Title { get; set; }
        public string? Descriptions { get; set; }
        public DateTime? CreatedOn { get; set; }

        public string? ImageIcon { get; set; }
    }
  public class Notifications
  {
    public int Id { get; set; }   
    public string UserGUID { get; set; }   
    public string? Title { get; set; }
    public string? Descriptions { get; set; } 
    public string? ImageIcon { get; set; }
  }
}
