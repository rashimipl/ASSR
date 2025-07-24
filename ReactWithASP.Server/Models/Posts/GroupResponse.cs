namespace ReactWithASP.Server.Models.Posts
{
    /*public class GroupResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        *//*public List<string> Platform { get; set; }*//*
        public string GroupIcon { get; set; }
        public string UserGuid { get; set; }
    }
*/
    public class GroupResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        /*public List<string> Platform { get; set; }*/
        public string GroupIcon { get; set; }
        public string UserGuid { get; set; }
        public string[] Platform { get; set; }
    }
  public class AccountResponse
  {
    public string[] SocialMediaName { get; set; }
    public string UserGuid { get; set; }
  }
}
