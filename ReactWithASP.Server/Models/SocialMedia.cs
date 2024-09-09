namespace ReactWithASP.Server.Models
{
    public class UserSocialMedia
    {
        public string UserGuid { get; set; } 
    }
    /*public class SocialMedia
    {
        public int Id { get; set; }
        public string SocialMediaName { get; set; }
        public string Src { get; set; }
        public virtual ICollection<GroupSocialMedia> GroupSocialMedias { get; set; }
    }*/
    public class SocialMedia
    {
        public int Id { get; set; }
        //public string? socialMedia { get; set; }
        public string SocialMediaName { get; set; }  // Ensure this property exists
        public string src { get; set; }
        //public int? status { get; set; }
        public   ICollection<GroupSocialMedias> GroupSocialMedias { get; set; }
    }
    public class UserSocialMediaStatus
    {
        public int Id { get; set; }
        public string UserGuid { get; set; }
        public int SocialMediaId { get; set; }
        public int Status { get; set; }
        public SocialMedia SocialMedia { get; set; }
    }
    public class SocialMediaResponse
    {
        public int Id { get; set; }
        public string SocialMediaName { get; set; }
        public string Src { get; set; }
        public int Status { get; set; }
    }
    public class SocialMediaUser
    {
        public int Id { get; set; }
        public string socialMediaUrl { get; set; }
        public string UserAccountName { get; set; }
        public string AccountIcon { get; set; }
        public int Status { get; set; }
    }

    public class TagsGroup
    {
        public int Id { get; set; }
        public string name { get; set; }
    }

    public class UserMediaAccounts
    {
        public int Id { get; set; }
        public string socialMedia { get; set; }
        public string userName { get; set; }
        public int status { get; set; }
        public string profileIcon { get; set; }
        public string socialMediaImage { get; set; }
    }

    /* public class GroupRequest 
     {
         public string UserGuid { get; set; }
         *//*public List<int> Id { get; set; } //UserMediaAccounts Id*//*
         public string groupName { get; set; }

     }*/
    /*public class GroupRequest
    {
        public string UserGuid { get; set; }
        public List<int> Id { get; set; } //UserMediaAccounts Id
        public string Name { get; set; }
        public bool Status { get; set; }
        public String GroupIcon { get; set; }
        public DateTime CreatedOn { get; set; }

    }*/

    public class GroupRequest
    {
        public string UserGuid { get; set; }
        public List<int> Id { get; set; } //UserMediaAccounts Id
        public string GroupName { get; set; }
        public string? GroupIcon { get; set; }

    }

    public class UpdateGroupRequest
    {
        public string UserGuid { get; set; }
        public List<int> Id { get; set; } //UserMediaAccounts Id
        public string GroupName { get; set; }
        public string? GroupIcon { get; set; }
        public int GroupId { get; set; }
        public bool Status { get; set; }

    }

    /*public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string GroupIcon { get; set; }
        public string UserGuid { get; set; }  // Foreign Key
        public int SocialMediaId { get; set; }  // Foreign Key
        public virtual SocialMedia SocialMedia { get; set; }
    }*/








}
