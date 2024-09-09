using Newtonsoft.Json;

namespace ReactWithASP.Server.Models.External.FacebookContracts

{
    public class FacebookTokenValidationResult
    {
        [JsonProperty("data")]
        public FacebookTokenValidationData Data { get; set; }
    }

public class FacebookTokenValidationData
{
    [JsonProperty("app_id")]
    public string  AppId { get; set; }
    [JsonProperty("type")]
    public string Type { get; set; }
    [JsonProperty("application")]
    public string Application { get; set; }
    [JsonProperty("data_access_expires_At")]
    public long DataAccessExpiresAt { get; set; }
    [JsonProperty("expiresAt")]
    public long ExpiresAt { get; set; }
    [JsonProperty("is_valid")]
    public bool  IsValid { get; set; }
    [JsonProperty("Scopes")]
    public string[] Scopes { get; set; }
    [JsonProperty("user_id")]
    public string UserId { get; set; }

}

}
