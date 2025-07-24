namespace ReactWithASP.Server.Models
{
    public class FCMSetting
    {
        public int Id { get; set; } 
        public string FiberBaseId { get; set; } 
        public string UploadedFile { get; set; } 
        public DateTime CreatedOn { get; set; } 
    }

    public class UserWithDeviceToken
    {
        public int  Id { get; set; }
        public int  UserGuid { get; set; }
        public string FullName { get; set; }
        public string DeviceToken { get; set; }
    }

    public class FCMSettingModel
    {
        public int Id { get; set; }
        public string FiberBaseId { get; set; }
        public IFormFile UploadedFile { get; set; }
    }

    public class UpdateFCMSettingModel
    {
        public int Id { get; set; }
        public IFormFile UploadedFile { get; set; }
    }

}
