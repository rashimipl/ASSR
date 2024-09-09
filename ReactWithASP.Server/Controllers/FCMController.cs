using JWTAuthentication.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using ReactWithASP.Server.Models;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cors;
using System.Text;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;




namespace ReactWithASP.Server.Controllers
{
    public class FCMController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly GoogleCredential _googleCredential;
        private readonly string _projectId;

        public FCMController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();

            var serviceAccountKeyPath = Path.Combine(Directory.GetCurrentDirectory(), "serviceAccount.json");
            _googleCredential = GoogleCredential.FromFile(serviceAccountKeyPath).CreateScoped("https://www.googleapis.com/auth/firebase.messaging");
            _projectId = _configuration["Firebase:ProjectId"];
        }
        [HttpPost("send")]
        public async Task<IActionResult> SendFCMMessage()
        {
            try
            {
                var accessToken = await _googleCredential.UnderlyingCredential.GetAccessTokenForRequestAsync();
                var registrationToken = "fzXV6dYcSH6xhG4DsAgtSY:APA91bFV6FrbzTVD-ZsLKfd29Rax1GrF0YnFrjbk44FZ_k2xqBdVJBOMg-qEjMBXtHQ3KeweIV6O3czuAPOmdxYtckXFbiBgDR1zq2zlFSI_xefvewmorDY4Z2KoY9oUXSKEZzFG77e7";

                var message = new
                {
                    message = new
                    {
                        token = registrationToken,
                        notification = new
                        {
                            title = "FCM Message",
                            body = "Hello Anubhav"
                        }
                    }
                };

                var requestContent = new StringContent(JsonSerializer.Serialize(message), Encoding.UTF8, "application/json");
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                var response = await _httpClient.PostAsync($"https://fcm.googleapis.com/v1/projects/{_projectId}/messages:send", requestContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    return Ok(new { Message = "Message sent successfully", Response = responseData });
                }

                var errorData = await response.Content.ReadAsStringAsync();
                return BadRequest(new { Message = "Error sending message", Error = errorData });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
            }
        }
    
    }
}
