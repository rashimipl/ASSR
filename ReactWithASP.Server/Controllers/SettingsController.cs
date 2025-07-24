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
using PayPal.Api;
using SendGrid;
using static Google.Apis.Requests.BatchRequest;

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
        private readonly IHttpContextAccessor _httpContextAccessor;

    public SettingsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IConfiguration configuration, IWebHostEnvironment hostingEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _httpContextAccessor = httpContextAccessor;
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
            var response = _context.CompanySetting.Where(x => x.UserGuid == UserGuid).ToList();

            // Check if the company setting exists
            if (response == null)
            {
                return NotFound(new { Message = "Company information not found." });
            }

            // Create result based on the found company setting
            var result = response.Select(Comp => new
            {
                UserGuid = Comp.UserGuid,
                Name = Comp.Name,
                Address = Comp.Address,
                PhoneNo = Comp.PhoneNo,
                Email = Comp.Email,
                Reg_Year = Comp.Reg_Year
            }).ToList();

            return Ok(result);
        }

        [HttpPut("UpdateCompanyInfo")]
        public IActionResult UpdateCompanyInfo([FromBody] UpdateCompanyinfo model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find the group by UserGuid and GroupId
            var existingComp = _context.CompanySetting.FirstOrDefault(g => g.UserGuid == model.UserGuid);

            if (existingComp == null)
            {
                return NotFound(new
                {
                    Status = "False",
                    Message = "CompanyInfo not found !..."
                });
            }

            // Update group properties
            existingComp.Name = model.Name;
            existingComp.Address = model.Address;
            existingComp.PhoneNo = model.PhoneNo;
            existingComp.Email = model.Email;
            existingComp.Reg_Year = model.Reg_Year;
            _context.CompanySetting.Update(existingComp);
            // Save changes to the context
            _context.SaveChanges();

            return Ok(new
            {
                Status = "True",
                Message = "CompanyInfo updated successfully..."
            });
        }


        //[HttpPost("SMTP_Info")]
        //public IActionResult SMTP_Info([FromBody] SMTPsetting model)
        //{

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var result = new
        //    {
        //        Email = "mailto:anubhav.j@mishainfotech.com",
        //        Password = "sddcfgv",
        //        Host = "smtp.gmail.com",
        //        Port = "587",
        //        Enable_SSl = 1

        //    };
        //    return Ok(new { message = "data save successfully", result });

        //}

        [HttpPost("SMTP_InfoSave")]
        public IActionResult SMTP_InfoSave([FromBody] SAVESMTPsetting model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the email already exists in the database
            var existingEmail = _context.SMTPsetting.FirstOrDefault(x => x.Email == model.Email && x.UserGuid==model.UserGuid);
            if (existingEmail != null)
            {
                return BadRequest(new { status = "false", message = "Email already exists." });
            }

            SMTPsetting result = new SMTPsetting()
            {
                UserGuid = model.UserGuid,
                Email = model.Email,
                EmailFrom = "notification.mipl@gmail.com",
                Password = "sgcsertpfujdwkyn",
                Host = "smtp.gmail.com",
                Port = "587",
                Enable_SSl = true,
                UserDefaultCredentials = true,
                MailToCC = model.MailToCC,
                CreatedOn = DateTime.Now
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

            // Fetch the last inserted SMTP setting based on UserGuid
            var response = _context.SMTPsetting
                                  .Where(x => x.UserGuid == UserGuid)
                                  .OrderByDescending(x => x.CreatedOn) // Assuming 'CreatedDate' or 'Id' exists
                                  .FirstOrDefault();

            if (response == null)
            {
                return NotFound(new { Message = "SMTP information not found." });
            }

            // Map the SMTP setting to a result format
            var result = new
            {
                UserGuid = response.UserGuid,
                Email = response.Email,
                Password = response.Password,
                Host = response.Host,
                Port = response.Port,
                Enable_SSl = response.Enable_SSl,
                UserDefaultCredentials = response.UserDefaultCredentials,
                MailToCC = response.MailToCC
            };

            return Ok(result);
        }


        [HttpPut("UpdateSMTP_Info")]
        public IActionResult UpdateSMTP_Info([FromBody] UpdateSMTPsetting model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find the group by UserGuid and GroupId
            var existingSmtpinfo = _context.SMTPsetting.FirstOrDefault(g => g.UserGuid == model.UserGuid);

            if (existingSmtpinfo == null)
            {
                return NotFound(new
                {
                    Status = "False",
                    Message = "SMTP_Info not found !..."
                });
            }

            // Update group properties
            existingSmtpinfo.Email = model.Email;
            existingSmtpinfo.Password = model.Password;
            existingSmtpinfo.Host = model.Host;
            existingSmtpinfo.Port = model.Port;
            existingSmtpinfo.Enable_SSl = model.Enable_SSl;
            existingSmtpinfo.UserDefaultCredentials = model.UserDefaultCredentials;
            existingSmtpinfo.MailToCC = model.MailToCC;
            _context.SMTPsetting.Update(existingSmtpinfo);
            // Save changes to the context
            _context.SaveChanges();

            return Ok(new
            {
                Status = "True",
                Message = "SMTP_Info updated successfully..."
            });
        }


        [HttpPost("SMTP_SendinfoForTest")]
        //[Authorize]
        public IActionResult SMTP_SendinfoForTest(string Email, string UserGuid)
        {
            try
            {
                // Get the SMTP settings for the provided email and UserGuid
                var data = _context.SMTPsetting.FirstOrDefault(x => x.Email == Email && x.UserGuid == UserGuid);

                // Check if data was found
                if (data == null)
                {
                    return NotFound(new { message = "SMTP settings not found for the provided email and user." });
                }

                //try
                //{
                    using (var client = new SmtpClient(data.Host, Convert.ToInt32(data.Port)))
                    {
                        client.EnableSsl = true; // Enable SSL for secure communication
                        client.Credentials = new NetworkCredential(data.EmailFrom, data.Password); // Use the App Password
                        client.UseDefaultCredentials = false; // Don't use default credentials, since you're providing them explicitly

                        using (var mailMessage = new MailMessage())
                        {
                            mailMessage.From = new MailAddress(data.EmailFrom); // Sender's email address
                            mailMessage.Subject = "Send Test Mail."; // Email subject
                            mailMessage.Body = "Hello Dear,<br><br>This email is for testing SMTP settings.<br><br>Thanks"; // Email body
                            mailMessage.IsBodyHtml = true; // Set to true if the body contains HTML

                            mailMessage.To.Add(data.Email); // Recipient's email address

                            // Add CC (Carbon Copy) email address if provided
                            if (!string.IsNullOrEmpty(data.MailToCC))
                            {
                                mailMessage.CC.Add(data.MailToCC); // Add CC recipient
                            }

                            // Send the email
                            client.Send(mailMessage);
                            
                        }
                                                            // Add recipient emails
                    }

                    //Console.WriteLine("Email sent successfully.");
                //}
                //catch (SmtpException ex)
                //{
                //    Console.WriteLine($"Error sending email: {ex.Message}");
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine($"General error: {ex.Message}");
                //}



                //// Update the SMTPSetting.xml file with the data from the database
                //string xmlData = Path.Combine(Directory.GetCurrentDirectory(), "SMTPSetting.xml");
                //XmlDocument xmlDoc = new XmlDocument();
                //xmlDoc.Load(xmlData);

                //XmlNodeList nodeList = xmlDoc.SelectNodes("smtp");
                //foreach (XmlNode node in nodeList)
                //{
                //    node.Attributes[0].Value = data.Email;

                //    node.ChildNodes[0].Attributes[0].Value = data.Host;
                //    node.ChildNodes[0].Attributes[1].Value = data.Port;
                //    node.ChildNodes[0].Attributes[2].Value = data.UserDefaultCredentials ? "true" : "false";
                //    node.ChildNodes[0].Attributes[3].Value = data.Email;
                //    node.ChildNodes[0].Attributes[4].Value = data.Password;
                //    node.ChildNodes[0].Attributes[5].Value = data.Enable_SSl ? "true" : "false";
                //}
                //xmlDoc.Save(xmlData);

                return Ok(new { Status = "True", message = "Email sent successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while sending the email.", error = ex.Message });
            }
        }





        //// Prepare the MailMessage object
        //MailMessage msg = new MailMessage();
        //msg.To.Add(Email); // Add the recipient email

        //if (!string.IsNullOrEmpty(data.MailToCC))
        //{
        //    msg.CC.Add(data.MailToCC); // Add CC email if provided
        //}

        //MailAddress address = new MailAddress(data.Email); // Use email from data
        //msg.From = address;
        //msg.Subject = "Send Test Mail.";
        //msg.Body = "Hello Dear,<br><br>This email is for testing SMTP settings.<br><br>Thanks";
        //msg.IsBodyHtml = true;

        //// Using SMTP client to send the email

        //using (SmtpClient client = new SmtpClient())
        //{
        //    client.DeliveryMethod = SmtpDeliveryMethod.Network;
        //    client.EnableSsl = data.Enable_SSl; // Use SSL based on your data
        //    client.Host = data.Host;            // SMTP Host (e.g., smtp.gmail.com)
        //    client.Port = Convert.ToInt32(data.Port); // Port number (e.g., 465 for SSL, 587 for TLS)

        //    // Ensure you're using correct SMTP credentials
        //    NetworkCredential credentials = new NetworkCredential(data.Email, data.Password);
        //    client.UseDefaultCredentials = false; // Must be false to use explicit credentials
        //    client.Credentials = credentials;

        //    try
        //    {
        //        client.Send(msg);
        //    }
        //    catch (SmtpException smtpEx)
        //    {
        //        return StatusCode(500, new { message = "SMTP Error", error = smtpEx.Message });
        //    }
        //}

        //using (SmtpClient client = new SmtpClient())
        //{
        //    client.DeliveryMethod = SmtpDeliveryMethod.Network;
        //    client.EnableSsl = data.Enable_SSl; // SSL setting from data
        //    client.Host = data.Host; // Host from data
        //    client.Port = Convert.ToInt32(data.Port); // Port from data

        //    // Credentials for the SMTP server
        //    NetworkCredential credentials = new NetworkCredential(data.Email, data.Password);
        //    client.UseDefaultCredentials = false; // Use the provided credentials
        //    client.Credentials = credentials;

        //    // Send the email
        //    client.Send(msg);
        //}




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


        [HttpPut("UpdateDeveloperSetting_Info")]
        public IActionResult UpdateDeveloperSetting_Info([FromBody] UpdateDeveloperSetting model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find the group by UserGuid and GroupId
            var ExistingDeveloperpinfo = _context.DeveloperSetting.FirstOrDefault(g => g.UserGuid == model.UserGuid);

            if (ExistingDeveloperpinfo == null)
            {
                return NotFound(new
                {
                    Status = "False",
                    Message = "DeveloperSetting_Info not found !..."
                });
            }

            // Update group properties
            ExistingDeveloperpinfo.WebMasterEmail = model.WebMasterEmail;
            ExistingDeveloperpinfo.DeveloperEmail = model.DeveloperEmail;
            ExistingDeveloperpinfo.TestMode = model.TestMode;
            ExistingDeveloperpinfo.CopytoWebmaster = model.CopytoWebmaster;
            ExistingDeveloperpinfo.Copytodeveloper = model.Copytodeveloper;
            _context.DeveloperSetting.Update(ExistingDeveloperpinfo);
            // Save changes to the context
            _context.SaveChanges();

            return Ok(new
            {
                Status = "True",
                Message = "Developer_Info updated successfully..."
            });
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


        [HttpPut("UpdateApplicationSetting_Info")]
        public IActionResult UpdateApplicationSetting_Info([FromBody] UpdateApplicationsetting model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find the group by UserGuid and GroupId
            var ExistAppInfo = _context.Applicationsetting.FirstOrDefault(g => g.UserGuid == model.UserGuid);

            if (ExistAppInfo == null)
            {
                return NotFound(new
                {
                    Status = "False",
                    Message = "DeveloperSetting_Info not found !..."
                });
            }

            // Update group properties
                ExistAppInfo.FullName = model.FullName;
                ExistAppInfo.ApplicationURL = model.ApplicationURL;
                ExistAppInfo.APIURL = model.APIURL;
                ExistAppInfo.FacebookURL = model.FacebookURL;
                ExistAppInfo.YoutubeURL = model.YoutubeURL;
                ExistAppInfo.InstagramURL = model.InstagramURL;
                ExistAppInfo.TwitterURL = model.TwitterURL;
                ExistAppInfo.AdminURL = model.AdminURL;
                ExistAppInfo.SupervisorURL = model.SupervisorURL;
                ExistAppInfo.CompanyURL = model.CompanyURL;
                ExistAppInfo.UserImagesURL = model.UserImagesURL;
                ExistAppInfo.WorkerReportImagesURL = model.WorkerReportImagesURL;
                ExistAppInfo.WorkerDocumentImagesURL = model.WorkerDocumentImagesURL;
            _context.Applicationsetting.Update(ExistAppInfo);
             // Save changes to the context
            _context.SaveChanges();

            return Ok(new
            {
                Status = "True",
                Message = ".Application_Info updated successfully..."
            });
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
    public async Task<ActionResult<NotificationSetting>> GetNotificationSetting(string UserGUID)
    {
      var notificationSetting = await _context.NotificationSetting
    .FirstOrDefaultAsync(n => n.UserGUID == UserGUID);

      if (notificationSetting == null)
      {
        return NotFound();
      }

      return Ok(notificationSetting);
    }
    [HttpGet("GetNotifications")]
    public async Task<IActionResult> GetNotifications(string UserGUID)
    {
      var notifications = await _context.NotificationSetting
    .Where(n => n.UserGUID == UserGUID)
    .Select(n => new Notifications
    {
      Id = n.Id,
      UserGUID = n.UserGUID,
      Title = n.Title,
      Descriptions = n.Descriptions,
      ImageIcon = n.ImageIcon 
    })
    .ToListAsync();

      if (notifications == null )
      {
        return NotFound(new { Message = "No notification settings found for the provided UserGUID." });
      }

      return Ok(new { Data = notifications });
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


        [HttpPost("TestFCMNotification")]
        public async Task<IActionResult> TestFCMNotification(string userguid,int id, string deviceTokens)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == userguid);
            if (user == null)
            {
                return Unauthorized();
            }

            // Create new notification setting
            NotificationSetting Obj = new NotificationSetting();
            Obj.UserGUID = user.Id;
            Obj.PhoneNumber = user.PhoneNumber;
            Obj.Email = user.Email;
            Obj.DeviceToken = deviceTokens;

            string[] deviceTokens1 = new string[1];
            deviceTokens1[0] = deviceTokens;
            var content = "Sample notification content"; // Replace with actual content

            string fileName = _context.FCMSetting.Where(x=>x.Id==id).Select(x=>x.UploadedFile).FirstOrDefault();
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
            var credentials = GoogleCredential.FromFile(path)
            .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");

            // Check if credentials are null
            if (credentials == null)
            {
                throw new Exception("Failed to create Google credentials from the provided file.");
            }

            // Get the access token for requests
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


        [HttpPost("CreateFCMSetting")]
        public async Task<IActionResult> CreateFCMSetting([FromForm] FCMSettingModel model)
        {
            if (model.UploadedFile.FileName == null || model.UploadedFile.FileName.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Save the file
            var filePath = Path.Combine("FirebaseNotification", model.UploadedFile.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.UploadedFile.CopyToAsync(stream);
            }

            // Create FCMSetting entity
            var fcmSetting = new FCMSetting
            {
                FiberBaseId = model.FiberBaseId,
                UploadedFile = model.UploadedFile.FileName, // Store the file name or path
                CreatedOn = DateTime.UtcNow
            };

            // Add to database
            _context.FCMSetting.Add(fcmSetting);
            _context.SaveChanges();

            return Ok(new { status = true, message = "Save Successfully.." });
            //return CreatedAtAction(nameof(CreateFCMSetting), new { id = fcmSetting.Id }, fcmSetting);

        }

        [HttpGet("GetFCMSetting_Info")]
        public IActionResult GetFCMSetting_Info(int Id)
        {
            if (Id==null)
            {
                return BadRequest(new { Message = "User GUID is required." });
            }

            // Fetch FCMSetting based on UserGuid
            var response = _context.FCMSetting.FirstOrDefault(x => x.Id == Id);

            // Check if the FCMSetting exists
            if (response == null)
            {
                return NotFound(new { Message = "FCMSetting information not found." });
            }

            // Create result based on the found company setting
            FCMSetting result = new FCMSetting()
            {
                Id = response.Id,
                FiberBaseId = response.FiberBaseId,
                UploadedFile = response.UploadedFile,
                CreatedOn = response.CreatedOn
            };

            return Ok(result);
        }

        [HttpPut("UpdateFCMSetting_Info")]
        public IActionResult UpdateFCMSetting_Info([FromForm] UpdateFCMSettingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find the group by UserGuid and GroupId
            var existingFCMinfo = _context.FCMSetting.FirstOrDefault(g => g.Id == model.Id);

            if (existingFCMinfo == null)
            {
                return NotFound(new
                {
                    Status = "False",
                    Message = "FCMSetting_Info not found !..."
                });
            }
            // Save the file
            var filePath = Path.Combine("FirebaseNotification", model.UploadedFile.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                 model.UploadedFile.CopyToAsync(stream);
            }

            // Update group propertie
            existingFCMinfo.UploadedFile = model.UploadedFile.FileName;
            
            _context.FCMSetting.Update(existingFCMinfo);
            // Save changes to the context
            _context.SaveChanges();

            return Ok(new
            {
                Status = "True",
                Message = "FCM_Info updated successfully..."
            });
        }

        [HttpGet("GetUserWithDeviceToken")]
        public IActionResult GetUserWithDeviceToken()
        {

            var response = (from U in _context.Users
                            join Ns in _context.NotificationSetting on U.Id equals Ns.UserGUID
                            orderby Ns.Id descending // Specify a property to order by
                            select new
                            {
                                U.FullName,
                                Ns.UserGUID,
                                Ns.Id,
                                Ns.DeviceToken
                            }).ToList();

            if (response == null)
            {
                return NotFound(new { Message = "User info not found." });
            }

            return Ok(response);
        }
    [HttpGet("GetSettings")]
    public async Task<IActionResult> GetSettings(string userGUid)
    {
      var settings = await _context.AllNotificationSettings
          .FirstOrDefaultAsync(x => x.UserGUid == userGUid);

      if (settings == null)
      {
        return Ok(new AllNotificationSettings
        {
          AllNotifications = false,
          ActivityReminder = false,
          Errors = false,
          RemindBefore1Hour = false,
          PublishedPost = false
        });
      }

      return Ok(new AllNotificationSettings
      {
        AllNotifications = settings.AllNotifications,
        ActivityReminder = settings.ActivityReminder,
        Errors = settings.Errors,
        RemindBefore1Hour = settings.RemindBefore1Hour,
        PublishedPost = settings.PublishedPost
      });
    }
    [HttpPost("UpdateAllNotificationSettings")]   
    public async Task<IActionResult> UpdateAllNotificationSettings(string userGUid, [FromBody] UpdateNotificationSettingsRequest request)
    {
      var settings = await _context.AllNotificationSettings
          .FirstOrDefaultAsync(x => x.UserGUid == userGUid);

      if (settings == null)
      {
        settings = new AllNotificationSettings { UserGUid = userGUid };
        _context.AllNotificationSettings.Add(settings);
      }

      settings.AllNotifications = request.AllNotifications;
      settings.ActivityReminder = request.ActivityReminder;
      settings.Errors = request.Errors;
      settings.RemindBefore1Hour = request.RemindBefore1Hour;
      settings.PublishedPost = request.PublishedPost;

      await _context.SaveChangesAsync();

      return Ok(new { Message = "Notification settings updated." });
    }
    // 🔹 HELP CENTER

    [HttpGet("GetAllFAQs")]
    public async Task<IActionResult> GetAllFAQs()
    {
      var faqs = await _context.HelpArticles
          .OrderByDescending(x => x.CreatedOn)
          .ToListAsync();
      if (faqs.Count == 0)
      {
        return NotFound(new { Message = "FAQs Not Found." });
      }
      return Ok(faqs);
    }

    [HttpPost("CreateFAQ")]
    public async Task<IActionResult> AddFAQ([FromBody] HelpArticle model)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      model.CreatedOn = DateTime.UtcNow;  
      model.UpdatedOn = DateTime.UtcNow;             

      _context.HelpArticles.Add(model);
      await _context.SaveChangesAsync();

      return Ok(new { Message = "FAQ added successfully." });
    }

    // 🔹 PUT update FAQ (admin side)
    [HttpPut("UpdateFAQ")]
    public async Task<IActionResult> UpdateFAQ( [FromBody] HelpArticle model)
    {
      var faq = await _context.HelpArticles.FindAsync(model.Id);
      if (faq == null)
        return NotFound();

      faq.Question = model.Question;
      faq.Answer = model.Answer;
      faq.UpdatedOn = DateTime.UtcNow;  

      await _context.SaveChangesAsync();

      return Ok(new { Message = "FAQ updated successfully." });
    }

    // 🔹 DELETE FAQ (admin side)
    [HttpDelete("DeleteFAQ")]
    public async Task<IActionResult> DeleteFAQ(int id)
    {
      var faq = await _context.HelpArticles.FindAsync(id);
      if (faq == null) return NotFound();

      _context.HelpArticles.Remove(faq);
      await _context.SaveChangesAsync();
      return Ok(new { Message = "FAQ deleted successfully." });
    }

    // 🔹 REFER A FRIEND

    [HttpGet("GetReferralLink")]
    public async Task<IActionResult> GetReferralLink()
    {
      var request = _httpContextAccessor.HttpContext.Request;
      var referralLink = $"{request.Scheme}://{request.Host}/api/Authenticate/register";
      return Ok(new { referralLink });
    }
    // 🔹 REVIEWS

    [HttpPost("SubmitReview")]
    public async Task<IActionResult> SubmitReview([FromBody] Review model)
    {
      _context.Reviews.Add(model);
      await _context.SaveChangesAsync();
      return Ok(new { Message = "Thanks for your feedback!" });
    }

    [HttpGet("GetAllReviews")]
    public async Task<IActionResult> GetAllReviews()
    {
      var reviews = await _context.Reviews.OrderByDescending(x => x.CreatedOn).ToListAsync();
      return Ok(reviews);
    }

    [HttpDelete("DeleteReview")]
    public async Task<IActionResult> DeleteReview(int id)
    {
      var review = await _context.Reviews.FindAsync(id);
      if (review == null) return NotFound();

      _context.Reviews.Remove(review);
      await _context.SaveChangesAsync();
      return Ok(new { Message = "Review deleted." });
    }

  }
}
