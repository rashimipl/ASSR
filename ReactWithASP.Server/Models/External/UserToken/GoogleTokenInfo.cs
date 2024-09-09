using Newtonsoft.Json;

namespace ReactWithASP.Server.Models.External.UserToken
{
    public class GoogleTokenInfo
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("picture")]
        public string Picture { get; set; }
    }

    public class ExternalUser
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhotoUrl { get; set; }
    }
}
