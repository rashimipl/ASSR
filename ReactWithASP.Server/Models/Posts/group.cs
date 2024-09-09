namespace ReactWithASP.Server.Models.Posts
{
    /*public class group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string GroupIcon { get; set; }
        public string UserGuid { get; set; }

        //public int PostId { get; set; }
        //public ICollection<GroupSocialMedia> GroupSocialMedias { get; set; }
        //public ICollection<Post> Posts { get; set; } // Add this relationship
    }*/

    public class group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? GroupIcon { get; set; }
        public string UserGuid { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedOn { get; set; }
    }


}
