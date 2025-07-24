using Google.Apis.Auth.OAuth2;
using JWTAuthentication.Authentication;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Quartz;
using ReactWithASP.Server.Models;
using ReactWithASP.Server.Models.Settings;
using System.Net.Http;
using System.Security.Policy;
using System.Text;

public class ScheduledOnTimeJob : IJob
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly HttpClient _httpClient;

    public ScheduledOnTimeJob(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment,HttpClient httpClient)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
        _httpClient = httpClient;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var jobData = context.JobDetail.JobDataMap;
        var scheduledPostId = jobData.GetInt("ScheduledPostId");
        var userGuid = jobData.GetString("UserGuid");

        // Fetch the scheduled post from the database
        var scheduledPost = await _context.ScheduledPost
        .Where(x => x.IsPublished != true && x.Id == scheduledPostId)
        .FirstOrDefaultAsync();

        if (scheduledPost == null)
        {
            return;
        }

        // Split the media URLs and prepare file names
        string[] urlArray = scheduledPost.MediaUrl.Split(',');
        List<string> fileNames = new List<string>();

        foreach (var url1 in urlArray)
        {
            string fileName = url1.Substring(url1.LastIndexOf('/') + 1);
            fileNames.Add(fileName);
        }

        // Prepare the model for the post request
        var model = new CreatePostRequest
        {
            userGUId = scheduledPost.UserGuid,
            Title = scheduledPost.Title,
            Description = scheduledPost.Description,
            AccountOrGroupName = scheduledPost.AccountOrGroupName,
            //AccountOrGroupIdStrings = scheduledPost.AccountOrGroupName=="Groups"?scheduledPost.AccountOrGroupId:null,
            AccountOrGroupId = scheduledPost.AccountOrGroupId
        .Split(",") // Split the string by commas
        .Select(x => new AccountOrPageId1
        {
            //Id = int.TryParse(x, out int value) ? value : 0, // Parse to integer, handle invalid entries
            Id = scheduledPost.AccountOrGroupId,
            PageId = scheduledPost.PageId // Use PageId as provided
        }) 
        .ToList(),
            MediaUrl = fileNames, // Assuming fileNames is a list of strings
            Tags = scheduledPost.Tags.Split(",").ToList() // Split and convert tags to a list
        };


        string url = "https://localhost:7189/api/Post";
        //string url = "https://assr.digitalnoticeboard.biz/api/Post";
        

        // Track the number of successful publishes
        int successfulPublishes = 0;

        // Get the total number of scheduled dates

        List<DateTime> scheduled1 = new List<DateTime>();
        if (scheduledPost.ScheduledType== "Weekly")
        {
            var fromDate = DateTime.Parse(scheduledPost.FromDate); // Example start date
            var toDate = DateTime.Parse(scheduledPost.ToDate);   // Example end date

            var scheduledDaysDb = await _context.ScheduledPost
           .Where(sd => sd.IsPublished!=true && sd.Id==scheduledPostId)  // Assuming a column 'IsActive' to filter relevant days
           .Select(sd => sd.Days)   // 'DayName' is the column storing days in a comma-separated string format, e.g., "thu,fri"
           .ToListAsync();

            var scheduledDays = new List<DayOfWeek>();

            // Split each entry by comma and map to DayOfWeek
            foreach (var dayString in scheduledDaysDb)
            {
                var days = dayString.Split(',')
                                    .Select(d => d.Trim().ToLower()); // Split by comma and trim each day

                foreach (var day in days)
                {
                    switch (day)
                    {
                        case "mon":
                            scheduledDays.Add(DayOfWeek.Monday);
                            break;
                        case "tue":
                            scheduledDays.Add(DayOfWeek.Tuesday);
                            break;
                        case "wed":
                            scheduledDays.Add(DayOfWeek.Wednesday);
                            break;
                        case "thu":
                            scheduledDays.Add(DayOfWeek.Thursday);
                            break;
                        case "fri":
                            scheduledDays.Add(DayOfWeek.Friday);
                            break;
                        case "sat":
                            scheduledDays.Add(DayOfWeek.Saturday);
                            break;
                        case "sun":
                            scheduledDays.Add(DayOfWeek.Sunday);
                            break;
                        default:
                            Console.WriteLine($"Invalid day format found in database: {day}");
                            break;
                    }
                }
            }


            // Iterate over the date range and add dates that match the specified days
            for (var date = fromDate; date <= toDate; date = date.AddDays(1))
            {
                if (scheduledDays.Contains(date.DayOfWeek))
                {
                    scheduled1.Add(date); // Add the date if it matches one of the specified days
                }
            }

        }
        else
        {
            scheduled1 = scheduledPost.ScheduledDate
            .Split(',')  // Split the string by commas
            .Select(dateStr => DateTime.Parse(dateStr.Trim()))  // Parse each date string into DateTime
            .ToList();
        }
            int totalScheduledDates = scheduled1.Count();
        // Iterate over the list of scheduled dates and try to publish at each time
        foreach (var scheduledDate in scheduled1)
        {
            // Calculate the delay until the scheduled date
            var delay = scheduledDate - DateTime.Now;

            if (delay > TimeSpan.Zero)
            {
                // Wait for the scheduled date
                await Task.Delay(delay);
            }

            // Send the POST request to publish the post
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(url, model);

            if (response.IsSuccessStatusCode)
            {
                // Increment the count of successful publishes
                successfulPublishes++;
            }

            // Ensure the request succeeded
            response.EnsureSuccessStatusCode();
        }

        // After all the scheduled dates have been processed, check if the post is fully published
        if (successfulPublishes == totalScheduledDates)
        {
            // Update the ScheduledPost to mark it as published
            scheduledPost.IsPublished = true;

            // Save changes to the database
            await _context.SaveChangesAsync();
        }
    }


}
