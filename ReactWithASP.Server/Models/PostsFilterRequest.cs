namespace ReactWithASP.Server.Models
{
    public class PostsFilterRequest
    {
        public string? userGUId { get; set; }
        public string? Platform { get; set; } // e.g., "All", "Facebook", "Instagram", "TikTok"
        public string? searchKeyword { get; set; }
        public string? Date { get; set; }
        public string? tagsGroup { get; set; }
        public string? selectedGroup { get; set; }
    }
}
