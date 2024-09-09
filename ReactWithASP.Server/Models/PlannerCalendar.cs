namespace ReactWithASP.Server.Models
{
    public class Calendar
    {
        public string userGUId { get; set; }
        public DateTime? Date { get; set; }
    }
    public class Schedule
    {
        public string userGUId { get; set; }
        public DateTime? Date { get; set; }
        public string? searchbox { get; set; }
        public string? SocialMedia { get; set; }
        public string? TagsGroup { get; set; }
        public string? Group { get; set; }
    }

    public class PlannerSchedule
    {
        public string userGUId { get; set; }
        public int month { get; set; }
        public int? year { get; set; }
        public string? searchbox { get; set; }
    }
}
