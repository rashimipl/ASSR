using JWTAuthentication.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ReactWithASP.Server.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Google.Apis.Auth.OAuth2;
using SendGrid.Helpers.Mail;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Xml;
using ReactWithASP.Server.Models.Settings;

namespace ReactWithASP.Server.Controllers
{
    [Route("api")]
    [ApiController]

    public class SettingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public SettingsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost("SaveCompanyInfo")]
        public IActionResult SaveCompanyInfo([FromBody] CompanySetting model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CompanySetting result = new CompanySetting()
            {
                UserGuid = model.UserGuid,
                Name = model.Name,
                Address = model.Address,
                PhoneNo = model.PhoneNo,
                Email = model.Email,
                Reg_Year = DateTime.Now.Year,
            };
            _context.CompanySetting.Add(result);
            _context.SaveChanges();
            return Ok(new { status = "true", message = "data save successfully" });

        }

        [HttpGet("GetCompanyInfo")]
        public IActionResult GetCompanyInfo(string UserGuid)
        {
            if (string.IsNullOrEmpty(UserGuid))
            {
                return BadRequest(new { Message = "User GUID is required." });
            }

            // Fetch company settings based on UserGuid
            var response = _context.CompanySetting.FirstOrDefault(x => x.UserGuid == UserGuid);

            // Check if the company setting exists
            if (response == null)
            {
                return NotFound(new { Message = "Company information not found." });
            }

            // Create result based on the found company setting
            CompanySetting result = new CompanySetting()
            {
                UserGuid = response.UserGuid,
                Name = response.Name,
                Address = response.Address,
                PhoneNo = response.PhoneNo,
                Email = response.Email,
                Reg_Year = DateTime.Now.Year
            };

            return Ok(result);
        }


        [HttpPost("SMTP_Info")]
        public IActionResult SMTP_Info([FromBody] SMTPsetting model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = new
            {
                Email = "mailto:anubhav.j@mishainfotech.com",
                Password = "sddcfgv",
                Host = "smtp.gmail.com",
                Port = "587",
                Enable_SSl = 1

            };
            return Ok(new { message = "data save successfully", result });

        }

        [HttpPost("SMTP_InfoSave")]
        public IActionResult SMTP_InfoSave([FromBody] SMTPsetting model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            SMTPsetting result = new SMTPsetting()
            {
                UserGuid = model.UserGuid,
                Email = model.Email,
                Password = model.Password,
                Host = model.Host,
                Port = model.Port,
                Enable_SSl = true
            };

            _context.SMTPsetting.Add(result);
            _context.SaveChanges();

            return Ok(new { status = "true", message = "data save successfully" });

        }


        [HttpGet("GetSMTP_Info")]
        public IActionResult GetSMTP_Info(string UserGuid)
        {
            if (string.IsNullOrEmpty(UserGuid))
            {
                return BadRequest(new { Message = "User GUID is required." });
            }

            // Fetch company settings based on UserGuid
            var response = _context.SMTPsetting.FirstOrDefault(x => x.UserGuid == UserGuid);

            // Check if the company setting exists
            if (response == null)
            {
                return NotFound(new { Message = "Company information not found." });
            }

            // Create result based on the found company setting
            SMTPsetting result = new SMTPsetting()
            {
                UserGuid = response.UserGuid,
                Email = response.Email,
                Password = response.Password,
                Host = response.Host,
                Port = response.Port,
                Enable_SSl = response.Enable_SSl
            };

            return Ok(result);
        }


        [HttpPost("SMTP_SendinfoForTest")]
        public IActionResult SMTP_SendinfoForTest([FromBody] SMTPsetting model)
        {
            try
            {
                MailMessage msg = new MailMessage();
                msg.To.Add(model.ToEmail);
                if (model.MailToCC != null)
                {
                    msg.CC.Add(model.MailToCC);
                }
                MailAddress address = new MailAddress(model.Email);
                msg.From = address;
                msg.Subject = "Send Test Mail.";
                msg.Body = "Hello Dear,<br><br>This email for test email smtp setting <br><br> Thanks";
                msg.IsBodyHtml = true;

                using (SmtpClient client = new SmtpClient())
                {
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.EnableSsl = model.Enable_SSl;
                    client.Host = model.Host;
                    client.Port = Convert.ToInt32(model.Port);

                    NetworkCredential credentials = new NetworkCredential(model.Email, model.Password);
                    client.UseDefaultCredentials = false /*model.UserDefaultCredentials*/;
                    client.Credentials = credentials;

                    client.Send(msg);
                }

                string xmlData = Path.Combine(Directory.GetCurrentDirectory(), "SMTPSetting.xml");
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlData);
                XmlNodeList nodeList = xmlDoc.SelectNodes("smtp");
                foreach (XmlNode node in nodeList)
                {
                    node.Attributes[0].Value = model.Email;

                    node.ChildNodes[0].Attributes[0].Value = model.Host;
                    node.ChildNodes[0].Attributes[1].Value = model.Port;
                    node.ChildNodes[0].Attributes[2].Value = model.UserDefaultCredentials ? "true" : "false";
                    node.ChildNodes[0].Attributes[3].Value = model.Email;
                    node.ChildNodes[0].Attributes[4].Value = model.Password;
                    node.ChildNodes[0].Attributes[5].Value = model.Enable_SSl ? "true" : "false";
                }
                xmlDoc.Save(xmlData);

                return Ok(new { Status = "True", message = "Email sent successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while sending the email.", error = ex.Message });
            }

        }

        [HttpPost("DeveloperSetting_Info")]
        public IActionResult DeveloperSetting_Info([FromBody] DeveloperSetting model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            DeveloperSetting result = new DeveloperSetting
            {
                UserGuid = model.UserGuid,
                WebMasterEmail = model.WebMasterEmail,
                DeveloperEmail = model.DeveloperEmail,
                TestMode = model.TestMode,
                CopytoWebmaster = model.CopytoWebmaster,
                Copytodeveloper = model.Copytodeveloper
            };
            _context.DeveloperSetting.Add(result);
            _context.SaveChanges();
            return Ok(new { Status = "True", message = "data save successfully" });

        }

        [HttpGet("GetDeveloperSetting_Info")]
        public IActionResult GetDeveloperSetting_Info(string UserGuid)
        {
            if (string.IsNullOrEmpty(UserGuid))
            {
                return BadRequest(new { Message = "User GUID is required." });
            }

            // Fetch company settings based on UserGuid
            var response = _context.DeveloperSetting.FirstOrDefault(x => x.UserGuid == UserGuid);

            // Check if the company setting exists
            if (response == null)
            {
                return NotFound(new { Message = "Company information not found." });
            }

            // Create result based on the found company setting
            DeveloperSetting result = new DeveloperSetting()
            {
                UserGuid = response.UserGuid,
                WebMasterEmail = response.WebMasterEmail,
                DeveloperEmail = response.DeveloperEmail,
                TestMode = response.TestMode,
                CopytoWebmaster = response.CopytoWebmaster,
                Copytodeveloper = response.Copytodeveloper
            };

            return Ok(result);
        }

        //[HttpGet("ApplicationSetting_GetInfo")]
        //public IActionResult ApplicationSetting_GetInfo([FromBody] )
        //{

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var result = new
        //    {
        //        WebMasterEmail = "mailto:anubhav.j@mishainfotech.com",
        //        DeveloperEmail = "mailto:nitin.k@mipl.us",
        //        TestMode = 1,
        //        CopytoWebmaster = 0,
        //        Copytodeveloper = 1

        //    };
        //    return Ok(result);

        //}

        [HttpPost("ApplicationSetting_Info")]
        public IActionResult ApplicationSetting_Info([FromBody] Applicationsetting model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Applicationsetting result = new Applicationsetting()
            {
                UserGuid = model.UserGuid,
                FullName = model.FullName,
                ApplicationURL = model.ApplicationURL,
                APIURL = model.APIURL,
                FacebookURL = model.FacebookURL,
                YoutubeURL = model.YoutubeURL,
                InstagramURL = model.InstagramURL,
                TwitterURL = model.TwitterURL,
                AdminURL = model.AdminURL,
                SupervisorURL = model.SupervisorURL,
                CompanyURL = model.CompanyURL,
                UserImagesURL = model.UserImagesURL,
                WorkerReportImagesURL = model.WorkerReportImagesURL,
                WorkerDocumentImagesURL = model.WorkerDocumentImagesURL

            };
            _context.Applicationsetting.Add(result);
            _context.SaveChanges();
            return Ok(new { status = "True", message = "data save successfully" });

        }

        [HttpGet("GetApplicationSetting_Info")]
        public IActionResult GetApplicationSetting_Info(string UserGuid)
        {
            if (string.IsNullOrEmpty(UserGuid))
            {
                return BadRequest(new { Message = "User GUID is required." });
            }

            var response = _context.Applicationsetting.FirstOrDefault(x => x.UserGuid == UserGuid);

            if (response == null)
            {
                return NotFound(new { Message = "Company information not found." });
            }

            Applicationsetting result = new Applicationsetting()
            {
                UserGuid = response.UserGuid,
                FullName = response.FullName,
                ApplicationURL = response.ApplicationURL,
                APIURL = response.APIURL,
                FacebookURL = response.FacebookURL,
                YoutubeURL = response.YoutubeURL,
                InstagramURL = response.InstagramURL,
                TwitterURL = response.TwitterURL,
                AdminURL = response.AdminURL,
                SupervisorURL = response.SupervisorURL,
                CompanyURL = response.CompanyURL,
                UserImagesURL = response.UserImagesURL,
                WorkerReportImagesURL = response.WorkerReportImagesURL,
                WorkerDocumentImagesURL = response.WorkerDocumentImagesURL
            };

            return Ok(result);
        }


        public static class CurrencyService
        {
            private static readonly Dictionary<string, decimal> ExchangeRates = new Dictionary<string, decimal>
        {
            { "USD", 1.0m },
            { "EUR", 0.85m },
            { "GBP", 0.75m },
            { "INR", 74.0m }
            // Add more currencies and rates as needed
        };

            public static (bool IsValid, string ErrorMessage, decimal ConvertedAmount) ConvertCurrency(CurrencyChangeRequest request)
            {
                if (!ExchangeRates.ContainsKey(request.SourceCurrency.ToUpper()))
                {
                    return (false, "Invalid source currency", 0);
                }

                if (!ExchangeRates.ContainsKey(request.TargetCurrency.ToUpper()))
                {
                    return (false, "Invalid target currency", 0);
                }

                var sourceRate = ExchangeRates[request.SourceCurrency.ToUpper()];
                var targetRate = ExchangeRates[request.TargetCurrency.ToUpper()];
                var convertedAmount = (request.Amount / sourceRate) * targetRate;

                return (true, string.Empty, convertedAmount);
            }
        }

        [HttpPost("ChangeCurrency")]
        public IActionResult ChangeCurrency([FromBody] CurrencyChangeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (isValid, errorMessage, convertedAmount) = CurrencyService.ConvertCurrency(request);

            if (!isValid)
            {
                return BadRequest(new { message = "Currency not change successfully" });
            }

            var result = new
            {
                request.Amount,
                request.SourceCurrency,
                request.TargetCurrency,
                ConvertedAmount = convertedAmount
            };

            return Ok(new { message = "Currency change successfully", result });
        }



        // GET: api/NotificationSettings
        [HttpGet("GetNotificationSetting")]
        public async Task<ActionResult<NotificationSetting>> GetNotificationSetting(int id)
        {
            var notificationSetting = await _context.NotificationSetting.FindAsync(id);

            if (notificationSetting == null)
            {
                return NotFound();
            }

            return notificationSetting;
        }



        // PUT: api/NotificationSettings
        [HttpPut("PutNotificationSetting")]
        public async Task<IActionResult> PutNotificationSetting(int id, NotificationSetting notificationSetting)
        {
            if (id != notificationSetting.Id)
            {
                return BadRequest();
            }

            _context.Entry(notificationSetting).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NotificationSettingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/NotificationSettings
        [HttpDelete("DeleteNotificationSetting")]
        public async Task<IActionResult> DeleteNotificationSetting(int id)
        {
            var notificationSetting = await _context.NotificationSetting.FindAsync(id);
            if (notificationSetting == null)
            {
                return NotFound();
            }

            _context.NotificationSetting.Remove(notificationSetting);
            _context.SaveChanges();

            return NoContent();
        }

        private bool NotificationSettingExists(int id)
        {
            return _context.NotificationSetting.Any(e => e.Id == id);
        }

        // POST: api/NotificationSettings
        [HttpPost("PostNotificationSetting")]
        public async Task<IActionResult> PostNotificationSetting(string Email, string deviceTokens)
        {
            var user = _context.Users.FirstOrDefault(x => x.Email == Email);
            if (user == null)
            {
                return Unauthorized();
            }

            //// Check if notification setting already exists for the user
            //var existingSetting = _context.NotificationSetting.FirstOrDefault(ns => ns.UserGUID == user.Id && ns.DeviceToken == deviceTokens);
            //if (existingSetting != null)
            //{
            //    return BadRequest(new { message = "Notification setting already exists for this user." });
            //}

            // Create new notification setting
            NotificationSetting Obj = new NotificationSetting();
            Obj.UserGUID = user.Id;
            Obj.PhoneNumber = user.PhoneNumber;
            Obj.Email = user.Email;
            Obj.DeviceToken = deviceTokens;

            string[] deviceTokens1 = new string[1];
            deviceTokens1[0] = deviceTokens;
            var content = "Sample notification content"; // Replace with actual content

            string fileName = "fcmpushnotificationfile.json";
            string relativePath = Path.Combine("FirebaseNotification", fileName);
            //string virtualPath = "D:\\ReactWithASP\\ReactWithASP.Server\\FirebaseNotification\\fcmpushnotificationfile.json"; // Note: no tilde (~) prefix in ASP.NET Core
            string path = Path.Combine(_hostingEnvironment.ContentRootPath, relativePath);
            try
            {
                await SendAndroidNotificationAsync2(deviceTokens1, content, path);
                // Add the notification setting to the database
                _context.NotificationSetting.Add(Obj);
                _context.SaveChanges();
                return Ok(new { status = true, message = "Notification Send Successfully.." });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { status = false, message = "Something Went Wrong" });
            }
            //await SendAndroidNotificationAsync2(deviceTokens, content, path);
            //return Ok(new { status = true });
        }

        private async Task SendAndroidNotificationAsync2(string[] deviceTokens, string content, string path)
        {
            var credentials = GoogleCredential.FromFile(path).CreateScoped("https://www.googleapis.com/auth/firebase.messaging");

            var token = await credentials.UnderlyingCredential.GetAccessTokenForRequestAsync();


            await SendNotificationsAsync(deviceTokens, token, content);
        }

        private Task SendNotificationsAsync(string[] deviceTokens, string accessToken, string content)
        {
            var tasks = deviceTokens.Select(token => SendNotificationAsync1(token, accessToken, content)).ToArray();
            return Task.WhenAll(tasks);
        }

        private async Task SendNotificationAsync1(string deviceToken, string accessToken, string content)
        {
            string FcmUrl = "https://fcm.googleapis.com/v1/projects/assr-38c3f/messages:send";
            var message = new
            {
                message = new
                {
                    token = deviceToken,
                    notification = new
                    {
                        title = "GPO News",
                        body = content
                    },
                    data = new
                    {
                        key1 = "value1",
                        key2 = "value2"
                    }
                }
            };

            var jsonBody = JsonConvert.SerializeObject(message);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {accessToken}");

                var response = await httpClient.PostAsync(FcmUrl, httpContent);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);

                    if (responseJson != null && responseJson.ContainsKey("name"))
                    {

                    }

                }
            }
        }



        [HttpPost("SaveSocialMediaAccountSettings")]
        public async Task<IActionResult> SaveSocialMediaAccountSettings([FromForm] SocialMediaAccountSettingsViewModel model)
        {
            const int MaxImageSizeInMB = 5; // Maximum allowed image size in MB
            const int MaxVideoSizeInMB = 50; // Maximum allowed video size in MB

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var socialMediaAccountSettings = new SocialMediaAccountSettings
            {
                SocialMediaId = model.SocialMediaId,
                TimeLimit = model.TimeLimit,
                CreatedOn = DateTime.Now,
                IsImageAllow = model.IsImageAllow,
                IsVedioAllow = model.IsVedioAllow,
                IsTextAllow = model.IsTextAllow,
                //Text = model.Text
            };

            if (model.IsImageAllow)
            {
                if (model.Image == null || model.Image.Length == 0)
                {
                    return BadRequest(new { status = "false", message = "Image file is required." });
                }

                if (model.Image.Length > MaxImageSizeInMB * 1024 * 1024)
                {
                    return BadRequest(new { status = "false", message = $"Image size must be at most {MaxImageSizeInMB} MB." });
                }

                var imagePath = Path.Combine("uploads", "images", model.Image.FileName);
                Directory.CreateDirectory(Path.GetDirectoryName(imagePath)); // Ensure directory exists

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await model.Image.CopyToAsync(stream);
                }
                socialMediaAccountSettings.ImagePath = imagePath; // Store the path in ImagePath property
            }

            if (model.IsVedioAllow)
            {
                if (model.Video == null || model.Video.Length == 0)
                {
                    return BadRequest(new { status = "false", message = "Video file is required." });
                }

                if (model.Video.Length > MaxVideoSizeInMB * 1024 * 1024)
                {
                    return BadRequest(new { status = "false", message = $"Video size must be at most {MaxVideoSizeInMB} MB." });
                }

                var videoPath = Path.Combine("uploads", "videos", model.Video.FileName);
                Directory.CreateDirectory(Path.GetDirectoryName(videoPath)); // Ensure directory exists

                using (var stream = new FileStream(videoPath, FileMode.Create))
                {
                    await model.Video.CopyToAsync(stream);
                }
                socialMediaAccountSettings.VideoPath = videoPath; // Store the path in VideoPath property
            }

            if (model.IsTextAllow)
            {
                if (model.Text == null)
                {
                    return BadRequest(new { status = "false", message = "Text field is required." });
                }

                if (model.Text.Length > 300)
                {
                    return BadRequest(new { status = "false", message = $"Text Field must be smaller then" });
                }
                socialMediaAccountSettings.Text = model.Text; // Store the path in ImagePath property
            }

            _context.SocialMediaAccountSettings.Add(socialMediaAccountSettings);
            _context.SaveChanges();

            return Ok(new { status = "true", message = "Data saved successfully" });
        }


    }
}
