namespace ReactWithASP.Server.Models
{
    public class Notification
    {
      public int Id { get; set; }
      public int NotificationId { get; set; }
      public string UserGuid { get; set; } 
      public string Name { get; set; }
      public bool Status { get; set; } = false; 
      public string Title { get; set; } 
      public string Message { get; set; }
      public DateTime CreatedOn { get; set; } = DateTime.Now; 

    }
}
