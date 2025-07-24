using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReactWithASP.Server.InterfaceServices;
using System.Net.Http.Headers;
using System.Text;

namespace LinkedApiIntegration.Services
{

    public class LinkedInService : ILinkedInService
    {
        //private readonly HttpClient _client;
        private static readonly HttpClient _client = new HttpClient();

        private readonly IConfiguration _configuration;

        public LinkedInService(HttpClient client, IConfiguration configuration)
        {
            //_client = client;
            _configuration = configuration;
        }
        /// <summary>
        /// Retrieves an OAuth 2.0 access token 
        /// from LinkedIn using the provided
        /// authorization code.
        /// </summary>
        /// <param name="authorizationCode"></param>
        /// <returns></returns>
        public async Task<string> GetAccessToken(string authorizationCode)
        {

            try
            {
                var values = new Dictionary<string, string>
            {
            { "grant_type", "authorization_code" },
            { "code", authorizationCode },
            { "redirect_uri", "https://localhost:7189/api/HandleRedirect" },
            { "client_id", "860d0h5icf09uj"},
            { "client_secret","N5il1MVY3zQzxyaL"}
            };

                var content = new FormUrlEncodedContent(values);
                var response = await _client.PostAsync("https://www.linkedin.com/oauth/v2/accessToken", content);
                var responseString = await response.Content.ReadAsStringAsync();

                Console.WriteLine("Response: " + responseString);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Error getting access token: " + responseString);
                }

                var tokenResponse = JObject.Parse(responseString);
                if (tokenResponse["access_token"] == null)
                {
                    throw new Exception("Access token not found in response.");
                }

                var token = tokenResponse["access_token"].ToString();
                var personId = await GetPersonIdAsync(token);
                return string.Join(",", token, personId);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                throw;
            }
        }

        /// <summary>
    /// Uploads an image to LinkedIn using the 
    /// provided access token.
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="imageBytes"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    //public async Task<string> UploadImageAsync(string accessToken, byte[] imageBytes, string fileName)
    //{
    //    try
    //    {
    //        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    //        var personId = await GetPersonIdAsync(accessToken);
    //        var requestBody = new
    //        {
    //            registerUploadRequest = new
    //            {
    //                recipes = new[] { "urn:li:digitalmediaRecipe:feedshare-image" },
    //                owner = personId,
    //                serviceRelationships = new[]
    //                {
    //            new { relationshipType = "OWNER", identifier = "urn:li:userGeneratedContent" }
    //        }
    //            }
    //        };
    //        var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
    //        var response = await _client.PostAsync("https://api.linkedin.com/v2/assets?action=registerUpload", content);
    //        response.EnsureSuccessStatusCode();
    //        var responseContent = await response.Content.ReadAsStringAsync();
    //        var uploadResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);
    //        var uploadUrl = (string)uploadResponse.value.uploadMechanism["com.linkedin.digitalmedia.uploading.MediaUploadHttpRequest"].uploadUrl;
    //        using (var imageContent = new ByteArrayContent(imageBytes))
    //        {
    //            imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
    //            var uploadResponseImage = await _client.PostAsync(uploadUrl, imageContent);
    //            uploadResponseImage.EnsureSuccessStatusCode();
    //        }
    //        return uploadResponse.value.asset.ToString();
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new ApplicationException("Error uploading image to LinkedIn", ex);
    //    }
    //}
    

         public async Task<string> UploadImageAsync(string accessToken, byte[] imageBytes, string fileName)
        {
            try
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var personId = await GetPersonIdAsync(accessToken);

                if (string.IsNullOrEmpty(personId))
                {
                    throw new ApplicationException("PersonId is null or empty.");
                }

                var requestBody = new
                {
                    registerUploadRequest = new
                    {
                        recipes = new[] { "urn:li:digitalmediaRecipe:feedshare-image" },
                        owner = personId,
                        serviceRelationships = new[] { new { relationshipType = "OWNER", identifier = "urn:li:userGeneratedContent" } }
                    }
                };

                var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

                var response = await _client.PostAsync("https://api.linkedin.com/v2/assets?action=registerUpload", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var uploadResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);

                var uploadUrl = (string)uploadResponse.value.uploadMechanism["com.linkedin.digitalmedia.uploading.MediaUploadHttpRequest"].uploadUrl;

                using (var imageContent = new ByteArrayContent(imageBytes))
                {
                    imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                    var uploadResponseImage = await _client.PostAsync(uploadUrl, imageContent);
                    uploadResponseImage.EnsureSuccessStatusCode();
                }

                return uploadResponse.value.asset.ToString();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error uploading image to LinkedIn", ex);
            }
        }


        /// <summary>
        /// Retrieves the LinkedIn person
        /// profile ID using the provided 
        /// access token.
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async Task<string> GetPersonIdAsync(string accessToken)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //var response = await _client.GetAsync("https://api.linkedin.com/v2/me");
            var response = await _client.GetAsync("https://api.linkedin.com/v2/userinfo");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var profile = JsonConvert.DeserializeObject<dynamic>(responseContent);
            var email = profile.email;


            return $"urn:li:person:{profile!.sub}";
        }
        /// <summary>
        /// Creates a LinkedIn post with an image 
        /// using the provided access token,
        /// asset ID, and post text.
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="assetId"></param>
        /// <param name="postText"></param>
        /// <returns></returns>
        public async Task CreatePostAsync(string accessToken, string assetId, string postText)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var personId = await GetPersonIdAsync(accessToken);

            var requestBody = new
            {
                author = personId,
                lifecycleState = "PUBLISHED",
                specificContent = new Dictionary<string, object>
            {
                {
                    "com.linkedin.ugc.ShareContent", new
                    {
                        shareCommentary = new { text = postText },
                        shareMediaCategory = "IMAGE",
                        media = new[]
                        {
                            new
                            {
                                status = "READY",
                                media = assetId
                            }
                        }
                    }
                }
            },
                visibility = new Dictionary<string, string>
            {
                { "com.linkedin.ugc.MemberNetworkVisibility", "PUBLIC" }
            }
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("https://api.linkedin.com/v2/ugcPosts", content);
            response.EnsureSuccessStatusCode();
        }
    //public async Task<string> GetUserProfile(string accessToken)
    //{
    //  var request = new HttpRequestMessage(HttpMethod.Get, "https://api.linkedin.com/v2/me");
    //  request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

    //  var response = await _client.SendAsync(request);
    //  var content = await response.Content.ReadAsStringAsync();
    //  return content;
    //}
  }


}

