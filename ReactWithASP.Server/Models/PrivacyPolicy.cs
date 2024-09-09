using System.ComponentModel.DataAnnotations;
//using System.Web.MVC;


namespace ReactWithASP.Server.Models
{
    public class PrivacyPolicy
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        //[AllowHtml]
        [DataType(DataType.Html)]
        public string Content { get; set; }
        //[AllowHtml]
        public string Meta_Description { get; set; }
        public string Username { get; set; }
    }
}
