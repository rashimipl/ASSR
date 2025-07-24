using System.ComponentModel.DataAnnotations;

namespace ReactWithASP.Server.Models
{
    public class SocialMediaSetting
    {
        public int ID { get; set; }
        public string UserGuid { get; set; }
        public int SocialMediaId { get; set; }
        public bool AllowText { get; set; }
        public string TextLimit { get; set; }
        public bool AllowImage { get; set; }
        public string ImagefileLength { get; set; }
        public string ImageSize { get; set; }
        public string ImageMaxWidth { get; set; }
        public string ImageMaxHigth { get; set; }
        public bool AllowVideo { get; set; }
        public string VideofileLength { get; set; }
        public string VideoSize { get; set; }
        public string VideoMaxWidth { get; set; }
        public string VideoMaxHigth { get; set; }
        public DateTime CreatedOn { get; set; }
        public string SocialMediaName { get; set; }
    }

    public class SaveSocialMediaSetting
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "UserGuid is required.")]
        public string UserGuid { get; set; }
        [Required(ErrorMessage = "SocialMediaId is required.")]
        public int SocialMediaId { get; set; }
        public bool AllowText { get; set; }
        public string TextLimit { get; set; }
        public bool AllowImage { get; set; }
        public string ImagefileLength { get; set; }
        public string ImageSize { get; set; }
        public string ImageMaxWidth { get; set; }
        public string ImageMaxHigth { get; set; }
        public bool AllowVideo { get; set; }
        public string VideofileLength { get; set; }
        public string VideoSize { get; set; }
        public string VideoMaxWidth { get; set; }
        public string VideoMaxHigth { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class UpadateSocialMediaSetting
    {
        [Required(ErrorMessage = "SocialMediaId is required.")]
        public int SocialMediaId { get; set; }
        public bool AllowText { get; set; }
        public string TextLimit { get; set; }
        public bool AllowImage { get; set; }
        public string ImagefileLength { get; set; }
        public string ImageSize { get; set; }
        public string ImageMaxWidth { get; set; }
        public string ImageMaxHigth { get; set; }
        public bool AllowVideo { get; set; }
        public string VideofileLength { get; set; }
        public string VideoSize { get; set; }
        public string VideoMaxWidth { get; set; }
        public string VideoMaxHigth { get; set; }
    }
}
