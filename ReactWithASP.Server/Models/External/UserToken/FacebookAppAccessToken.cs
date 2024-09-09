using Newtonsoft.Json;

namespace ReactWithASP.Server.Models.External.UserToken
{
    public class FacebookAppAccessToken
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }

    public class FacebookTokenInfo
    {
        public FacebookTokenData Data { get; set; }
    }

    public class FacebookTokenData
    {
        [JsonProperty("is_valid")]
        public bool IsValid { get; set; }
    }

    public class FacebookUserInfo
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("picture")]
        public FacebookUserPicture Picture { get; set; }
    }

    public class FacebookUserPicture
    {
        public FacebookUserPictureData Data { get; set; }
    }

    public class FacebookUserPictureData
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }

    
}
