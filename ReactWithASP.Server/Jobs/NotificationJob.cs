using Google.Apis.Auth.OAuth2;
using JWTAuthentication.Authentication;
using Newtonsoft.Json;
using Quartz;
using ReactWithASP.Server.Models;
using ReactWithASP.Server.Models.Settings;
using System.Text;

public class NotificationJob : IJob
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public NotificationJob(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var jobData = context.JobDetail.JobDataMap;
        var scheduledPostId = jobData.GetInt("ScheduledPostId");
        var userGuid = jobData.GetString("UserGuid");

        var scheduledPost = await _context.ScheduledPost.FindAsync(scheduledPostId);

        if (scheduledPost != null)
        {
            var user = await _context.Users.FindAsync(userGuid);
            if (user != null)
            {
                var allowedNotifications = _context.Notification
                .Where(x => x.UserGuid == userGuid && x.Status==true && x.Name== "Remind Before 1 hours")
                .ToList();

                if (allowedNotifications.Any()) // If notifications are allowed
                { 
                  // Send the notification
                  await SendNotification(user.Id, scheduledPost.DeviceToken);
                }
                else
                {
                    // Handle the case where ScheduledTime cannot be parsed
                    throw new Exception("Notifications are disabled in the settings");
                }

            }
        }
    }

    private async Task SendNotification(string userGuid, string deviceTokens)
    {
        var user = _context.Users.FirstOrDefault(x => x.Id == userGuid);

        if (user == null)
        {
            // Handle the case where the user does not exist, if needed
            return;
        }

        var notificationSetting = new NotificationSetting
        {
            UserGUID = user.Id,
            PhoneNumber = user.PhoneNumber,
            Email = user.Email,
            DeviceToken = deviceTokens
        };

        string[] deviceTokensArray = new string[1];
        deviceTokensArray[0] = deviceTokens;
        var content = "your post scheduled after 1 hour !..."; // Customize the notification content

        string fileName = "fcmpushnotificationfile.json";
        string relativePath = Path.Combine("FirebaseNotification", fileName);
        string path = Path.Combine(_webHostEnvironment.ContentRootPath, relativePath);

        try
        {
            await SendAndroidNotificationAsync2(deviceTokensArray, content, path);

            // Add the notification setting to the database
            _context.NotificationSetting.Add(notificationSetting);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log the exception or handle the error appropriately
        }
    }

    private async Task SendAndroidNotificationAsync2(string[] deviceTokens, string content, string path)
    {
        var credentials = GoogleCredential.FromFile(path).CreateScoped("https://www.googleapis.com/auth/firebase.messaging");

        var token = await credentials.UnderlyingCredential.GetAccessTokenForRequestAsync();

        await SendNotificationsAsync(deviceTokens, token, content);
    }

    private Task SendNotificationsAsync(string[] deviceTokens, string accessToken, string content)
    {
        var tasks = deviceTokens.Select(token => SendNotificationAsync1(token, accessToken, content)).ToArray();
        return Task.WhenAll(tasks);
    }

    private async Task SendNotificationAsync1(string deviceToken, string accessToken, string content)
    {
        string FcmUrl = "https://fcm.googleapis.com/v1/projects/assr-38c3f/messages:send";
        var message = new
        {
            message = new
            {
                token = deviceToken,
                notification = new
                {
                    title = "GPO News",
                    body = content
                },
                data = new
                {
                    key1 = "value1",
                    key2 = "value2"
                }
            }
        };

        var jsonBody = JsonConvert.SerializeObject(message);
        var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        using (var httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {accessToken}");

            var response = await httpClient.PostAsync(FcmUrl, httpContent);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);

                if (responseJson != null && responseJson.ContainsKey("name"))
                {
                    // Successfully sent notification
                }
            }
        }
    }
}
