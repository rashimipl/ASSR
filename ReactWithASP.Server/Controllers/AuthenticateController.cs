using JWTAuthentication.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using PayPal.Api;
using ReactWithASP.Server;
using ReactWithASP.Server.Models;
using ReactWithASP.Server.Models.External.UserToken;
using ReactWithASP.Server.Models.Settings;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace JWTAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment;
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _hostingEnvironment;


        public AuthenticateController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment, ApplicationDbContext context, IEmailSender emailSender, IWebHostEnvironment hostingEnvironment)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
            Environment = environment;
            _context = context;
            _emailSender = emailSender;
            _hostingEnvironment = hostingEnvironment;
        }   
        /*public void WriteLog(string message)
        {
            System.IO.File.AppendAllText(Environment.WebRootPath + "\\PdfLog.txt", message);
        }
*/


        /*[HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            // WriteLog("Start Function1" + DateTime.Now.ToString());

            ApplicationUser user = await userManager.FindByNameAsync(model.Username)
                                   ?? await userManager.FindByEmailAsync(model.Username);

            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
         {
             new Claim(ClaimTypes.Name, user.UserName),
             new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
             new Claim("FullName", user.FullName),
             new Claim("Email", user.Email),
             new Claim("PhoneNumber", user.PhoneNumber),
             new Claim("PhotoUrl", user.PhotoUrl),
             new Claim("userGUId", user.Id)
         };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                // WriteLog("Start Function2" + DateTime.Now.ToString());

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddMinutes(5),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new
                {
                    userGUId = user.Id,
                    fullName = user.FullName,
                    username = user.UserName,
                    photo = user.PhotoUrl,
                    email = user.Email,
                    phoneNumber = user.PhoneNumber,
                    AccessToken = tokenString,
                    // Optionally include static or dynamic user properties if needed
                    // Subscription = user.Subscription,
                    // SubscriptionStatus = user.SubscriptionStatus,
                    // Language = user.Language
                });
            }
            else
            {
                // WriteLog("Start Function3" + DateTime.Now.ToString());
                return Unauthorized(new { Status = "Error", Message = "Invalid username or password." });
            }
        }*/


        /* [HttpPost]
         [Route("login")]
         [EnableCors("AllowAll")]
         public async Task<IActionResult> Login([FromBody] LoginModel model)
         {
             try
             {
                 *//*ApplicationUser user = await userManager.FindByNameAsync(model.Username)
                                         ?? await userManager.FindByEmailAsync(model.Username);*//*
                 ApplicationUser user = await userManager.FindByNameAsync(model.Username)
                                   ?? await userManager.FindByEmailAsync(model.Username);


                 if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
                 {
                     var userRoles = await userManager.GetRolesAsync(user);

                     var authClaims = new List<Claim>
             {
                 new Claim(ClaimTypes.Name, user.UserName),
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
             };

                     // Safely add nullable claims
                     if (!string.IsNullOrEmpty(user.FullName))
                     {
                         authClaims.Add(new Claim("FullName", user.FullName));
                     }
                     if (!string.IsNullOrEmpty(user.Email))
                     {
                         authClaims.Add(new Claim("Email", user.Email));
                     }
                     if (!string.IsNullOrEmpty(user.PhoneNumber))
                     {
                         authClaims.Add(new Claim("PhoneNumber", user.PhoneNumber));
                     }
                     *//*if (!string.IsNullOrEmpty(user.PhotoUrl))
                     {
                         authClaims.Add(new Claim("PhotoUrl", user.PhotoUrl));
                     }*//*
                     var photoUrl = !string.IsNullOrEmpty(user.PhotoUrl) ? user.PhotoUrl : "https://powerusers.microsoft.com/t5/image/serverpage/image-id/98171iCC9A58CAF1C9B5B9/image-size/large/is-moderation-mode/true?v=v2&px=999";
                     authClaims.Add(new Claim("PhotoUrl", photoUrl));
                     authClaims.Add(new Claim("userGUId", user.Id));


                     // Add roles as claims
                     foreach (var userRole in userRoles)
                     {
                         authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                     }

                     var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                     var token = new JwtSecurityToken(
                         issuer: _configuration["JWT:ValidIssuer"],
                         audience: _configuration["JWT:ValidAudience"],
                         expires: DateTime.Now.AddMinutes(5),
                         claims: authClaims,
                         signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                     );

                     var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                     *//*var UserDetails = _context.NotificationSetting.FindAsync(user.Id);
                     var notification = new NotificationSetting
                     {
                        Title = "Welcome",
                        Descriptions = "Welcome to ASSR",
                        CreatedOn = DateTime.UtcNow,
                        UserGUID = user.Id,
                        ImageIcon = photoUrl,

                         PhoneNumber = user.PhoneNumber,
                         Email = user.Email
                     };

                     _context.NotificationSetting.Add(notification);
                     await _context.SaveChangesAsync();
 *//*

                     var userDetails =  _context.NotificationSetting.Find(user.Id);

                     if (userDetails != null)
                     {
                         // Update existing notification details
                         userDetails.Title = "Welcome";
                         userDetails.Descriptions = "Welcome to ASSR";
                         userDetails.CreatedOn = DateTime.UtcNow;
                         userDetails.ImageIcon = photoUrl;
                         userDetails.PhoneNumber = user.PhoneNumber;
                         userDetails.Email = user.Email;

                         _context.NotificationSetting.Update(userDetails);
                          _context.SaveChanges();

                     }



                     return Ok(new
                     {
                         userGUId = user.Id,
                         fullName = user.FullName,
                         username = user.UserName,
                         photo = user.PhotoUrl,
                         email = user.Email,
                         phoneNumber = user.PhoneNumber,
                         AccessToken = tokenString,
                         // Optionally include static or dynamic user properties if needed
                         // Subscription = user.Subscription,
                         // SubscriptionStatus = user.SubscriptionStatus,
                         // Language = user.Language
                     });
                 }
                 else
                 {
                     return Unauthorized(new { Status = "Error", Message = "Invalid username or password." });
                 }
             }
             catch (Exception ex)
             {
                 // Log the exception for debugging purposes
                 Console.WriteLine($"Exception during login: {ex}");
                 return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
             }
         }*/

        [HttpPost]
        [Route("login")]
        //[EnableCors("AllowAll")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                ApplicationUser user = await userManager.FindByNameAsync(model.Username)
                                      ?? await userManager.FindByEmailAsync(model.Username);

                if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
                {
                    var userRoles = await userManager.GetRolesAsync(user);

                    var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

                    // Safely add nullable claims
                    if (!string.IsNullOrEmpty(user.FullName))
                    {
                        authClaims.Add(new Claim("FullName", user.FullName));
                    }
                    if (!string.IsNullOrEmpty(user.Email))
                    {
                        authClaims.Add(new Claim("Email", user.Email));
                    }
                    if (!string.IsNullOrEmpty(user.PhoneNumber))
                    {
                        authClaims.Add(new Claim("PhoneNumber", user.PhoneNumber));
                    }
                    var photoUrl = !string.IsNullOrEmpty(user.PhotoUrl) ? user.PhotoUrl : "https://powerusers.microsoft.com/t5/image/serverpage/image-id/98171iCC9A58CAF1C9B5B9/image-size/large/is-moderation-mode/true?v=v2&px=999";
                    authClaims.Add(new Claim("PhotoUrl", photoUrl));
                    authClaims.Add(new Claim("userGUId", user.Id));

                    // Add roles as claims
                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                    var token = new JwtSecurityToken(
                        issuer: _configuration["JWT:ValidIssuer"],
                        audience: _configuration["JWT:ValidAudience"],
                        expires: DateTime.Now.AddMonths(6),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                    
                    var userDetails = await _context.NotificationSetting
                                            .FirstOrDefaultAsync(n => n.UserGUID == user.Id);
                    if (userDetails != null)
                    {
                        // Update existing notification details
                        userDetails.Title = "Welcome";
                        userDetails.Descriptions = "Welcome to ASSR";
                        userDetails.CreatedOn = DateTime.UtcNow;
                        userDetails.ImageIcon = photoUrl;
                        userDetails.PhoneNumber = user.PhoneNumber;
                        userDetails.Email = user.Email;

                        _context.NotificationSetting.Update(userDetails);
                    }
                    else
                    {
                        // Add new notification details
                        var notification = new NotificationSetting
                        {
                            Title = "Welcome",
                            Descriptions = "Welcome to ASSR",
                            CreatedOn = DateTime.UtcNow,
                            UserGUID = user.Id,
                            ImageIcon = photoUrl,
                            PhoneNumber = user.PhoneNumber,
                            Email = user.Email
                        };

                        _context.NotificationSetting.Add(notification);
                        
                    }
                    _context.SaveChanges();


                    return Ok(new
                    {
                        userGUId = user.Id,
                        fullName = user.FullName,
                        username = user.UserName,
                        photo = user.PhotoUrl,
                        email = user.Email,
                        phoneNumber = user.PhoneNumber,
                        AccessToken = tokenString
                    });
                }
                else
                {
                    return Unauthorized(new { Status = "Error", Message = "Invalid username or password." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during login: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        /* [HttpPost]
         [Route("login2")]
         public async Task<IActionResult> Login2([FromBody] LoginModel model)

         {
             ApplicationUser user = await userManager.FindByNameAsync(model.Username)
                                    ?? await userManager.FindByEmailAsync(model.Username);

             if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
             {
                 var userRoles = await userManager.GetRolesAsync(user);

             var authClaims = new List<Claim>
          {
     new Claim(ClaimTypes.Name, user.UserName),
     new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
           };

             foreach (var userRole in userRoles)
             {
                 authClaims.Add(new Claim(ClaimTypes.Role, userRole));
             }

             var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

             var token = new JwtSecurityToken(
                 issuer: _configuration["JWT:ValidIssuer"],
                 audience: _configuration["JWT:ValidAudience"],
                 expires: DateTime.Now.AddHours(3),
                 claims: authClaims,
                 signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
             );

             var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
             return Ok(new
             {
                 userGUId = user.Id,
                 fullName = user.FullName,
                 username = user.UserName,
                 photo = user.PhotoUrl,
                 email = user.Email,
                 phoneNumber = user.PhoneNumber,
                 AccessToken = tokenString
                 *//*Subscription = user.Subscription 
                 SubscriptionStatus = user.SubscriptionStatus 
                 Language = user.Language *//*

             });
         }

             return Unauthorized(new { Status = "Error", Message = "Invalid username or password." });
         }*/


        /* [HttpPost]
         [Route("login")]
         [EnableCors("AllowAll")]
         public async Task<IActionResult> Login([FromBody] LoginModel model)
         {
             // WriteLog("Start Function1" + DateTime.Now.ToString());
             // Hardcoded user data for static token and response
             var staticUser = new
             {
                 userGUId = 10,
                 FullName = "Manav Jha",
                 UserName = "Manav",
                 Email = "Manav2022@yopmail.com",
                 PhoneNumber = "8178971409",
                 PhotoUrl = "https://www.shutterstock.com/shutterstock/photos/1949415778/display_1500/stock-photo-successful-asian-businessman-in-a-black-business-suit-works-on-a-laptop-relaxes-in-a-restaurant-1949415778.jpg"
             };

             if (model.Username == "Manav" || model.Username == "Manav2022@yopmail.com" && model.Password == "Manav@123")
             {
                 var authClaims = new List<Claim>
         {
             new Claim("UserName", staticUser.UserName),
             new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
             new Claim("Email", staticUser.Email),
             new Claim("PhoneNumber", staticUser.PhoneNumber),
             new Claim("FullName", staticUser.FullName),
             new Claim("PhotoUrl", staticUser.PhotoUrl),
             new Claim("userGUId", staticUser.userGUId.ToString())
         };
                 //   WriteLog("Start Function2" + DateTime.Now.ToString());

                 var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                 var token = new JwtSecurityToken(
                     issuer: _configuration["JWT:ValidIssuer"],
                     audience: _configuration["JWT:ValidAudience"],
                     expires: DateTime.Now.AddMonths(6),
                     claims: authClaims,
                     signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                 );

                 var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                 return Ok(new
                 {

                     fullName = staticUser.FullName,
                     username = staticUser.UserName,
                     photo = staticUser.PhotoUrl,
                     email = staticUser.Email,
                     phoneNumber = staticUser.PhoneNumber,
                     AccessToken = tokenString
                     // Add other static user properties if needed
                     // Subscription = "StaticSubscription",
                     // SubscriptionStatus = "Active",
                     // Language = "en"
                 });
             }
             else
             {
                 //  WriteLog("Start Function3" + DateTime.Now.ToString());
                 return Unauthorized(new { Status = "Error", Message = "Invalid username or password." });
             }
         }
 */
        /*[HttpPost]
        [Route("register1")]
        [EnableCors("AllowAll")]

        public async Task<IActionResult> Register1([FromBody] RegisterModel model)
        {

            if (model.Password != model.ConfirmPassword)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "Password and Confirm Password do not match!" });
            }
            var userExists = await userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });


            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                PhoneNumber = model.PhoneNumber,
                FullName = model.FullName
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            var userData = new
            {
                user.UserName,
                user.Email,
                user.PhoneNumber,
                user.FullName
            };
            return Ok(new Response { Status = "Success", Message = "User created successfully!", Data = userData });
        }*/

        /*[HttpPost]
        [Route("register")]
        [EnableCors("AllowAll")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response { Status = "Error", Message = "Invalid input data", Data = ModelState });
            }

            // Constructing the user object with data from the model
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                FullName = model.FullName,
                //CreatedOn = DateTime.Now
            };

            // Creating the user using UserManager
            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new Response { Status = "Error", Message = "User creation failed", Data = result.Errors });
            }

            


            // Preparing the user data for the response
            var userData = new
            {
                user.UserName,
                user.Email,
                user.PhoneNumber,
                user.FullName,
                //user.CreatedOn
            };



            // Returning the user data as part of the response
            return Ok(new Response { Status = "Success", Message = "User created successfully!", Data = userData });
        }*/


        /*[HttpPost]
        [Route("register")]
        [EnableCors("AllowAll")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response { Status = "Error", Message = "Invalid input data", Data = ModelState });
            }

            // Constructing the user object with data from the model
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                FullName = model.FullName,
                //CreatedOn = DateTime.Now
            };

            // Creating the user using UserManager
            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new Response { Status = "Error", Message = "User creation failed", Data = result.Errors });
            }

            // Preparing the user data for the response
            var userData = new
            {
                user.UserName,
                user.Email,
                user.PhoneNumber,
                user.FullName,
                //user.CreatedOn
            };

            // Load and populate the email template
            string emailTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "Emails", "WelcomeTemp.cshtml");
            string emailBody;
            using (StreamReader reader = new StreamReader(emailTemplatePath))
            {
                emailBody = await reader.ReadToEndAsync();
            }
            emailBody = emailBody.Replace("{{UserName}}", user.UserName);

            _emailSender.SendEmailAsync(model.Email, "Welcome Mail", emailBody);

            // Returning the user data as part of the response
            return Ok(new Response { Status = "Success", Message = "User created successfully!", Data = userData });
        }*/


        /*[HttpPost]
        [Route("register")]
        [EnableCors("AllowAll")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response { Status = "Error", Message = "Invalid input data", Data = ModelState });
            }

            // Constructing the user object with data from the model
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                FullName = model.FullName,
                CreatedOn = DateTime.Now
            };

            // Creating the user using UserManager
            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new Response { Status = "Error", Message = "User creation failed", Data = result.Errors });
            }

            // Preparing the user data for the response
            var userData = new
            {
                user.UserName,
                user.Email,
                user.PhoneNumber,
                user.FullName,
                user.CreatedOn
            };

            var userguid = _context.Users.OrderByDescending(x => x.CreatedOn).Select(x => x.Id).FirstOrDefault();

            // Load and populate the email template
            string emailTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "Emails", "WelcomeTemp.cshtml");
            string emailBody;
            using (StreamReader reader = new StreamReader(emailTemplatePath))
            {
                emailBody = await reader.ReadToEndAsync();
            }
            emailBody = emailBody.Replace("{{UserName}}", user.UserName);

            _emailSender.SendEmailAsync(model.Email, "Welcome Mail", emailBody);
            var Gid = userguid;

            var templist = new List<string>()
    {
        "Facebook", "imo", "Instagram", "Linkedin",
        "Telegram", "TikTok", "Whatsapp", "Youtube"
    };

            foreach (var platform in templist)
            {
                var accountId = GetAccountId(platform);
                var accCon = new AccountConnection
                {
                    UserGuid = Gid,
                    AccountName = platform,
                    AccountIcon = $"{platform}.png",
                    AccountId = accountId,
                    Status = false
                };
                _context.AccountConnection.Add(accCon);
            }
            _context.SaveChanges();


            // Returning the user data as part of the response
            return Ok(new Response { Status = "Success", Message = "User created successfully!", Data = userData });
        }


        private int GetAccountId(string platformName)
        {
            // Example: Fetch AccountId from a predefined dictionary or database
            var accountMap = new Dictionary<string, int>
     {
         { "Facebook", 1 },
         { "imo", 2 },
         { "Instagram", 3 },
         { "Linkedin", 4 },
         { "Telegram", 5 },
         { "TikTok", 6 },
         { "Whatsapp", 7 },
         { "Youtube", 8 }
     };

            return accountMap.TryGetValue(platformName, out var accountId) ? accountId : 0; // Default 0 if not found
        }*/



        [HttpPost]
        [Route("register")]
        [EnableCors("AllowAll")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
         {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response { Status = "Error", Message = "Invalid input data", Data = ModelState });
            }

            // Constructing the user object with data from the model
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                FullName = model.FullName,
                CreatedOn = DateTime.Now,
                IsActive= true
            };

            // Creating the user using UserManager
            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new Response { Status = "Error", Message = "User creation failed", Data = result.Errors });
            }

            // Preparing the user data for the response
            var userData = new
            {
                user.UserName,
                user.Email,
                user.PhoneNumber,
                user.FullName,
                user.CreatedOn,
                user.IsActive
            };

            var userguid = _context.Users.OrderByDescending(x => x.CreatedOn).Select(x => x.Id).FirstOrDefault();

            // Load and populate the email template
            string emailTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "Emails", "WelcomeTemp.cshtml");
            string emailBody;
            using (StreamReader reader = new StreamReader(emailTemplatePath))
            {
                emailBody = await reader.ReadToEndAsync();
            }
            emailBody = emailBody.Replace("{{UserName}}", user.UserName);

            _emailSender.SendEmailAsync(model.Email, "Welcome Mail", emailBody);
            var Gid = userguid;

            var templist = new List<string>()
            {
                "Facebook", "Linkedin", "Instagram", "TikTok", "Whatsapp", "Telegram", "Youtube", "imo"
            };

            foreach (var platform in templist)
            {
                var accountId = GetAccountId(platform);
                var accCon = new AccountConnection
                {
                    UserGuid = Gid,
                    AccountName = platform,
                    AccountIcon = $"{platform}.png",
                    AccountId = accountId,
                    Status = false
                };
                _context.AccountConnection.Add(accCon);
            }
            _context.SaveChanges();
            var templist1 = new List<string>()
            {
                "Errors", "Remind Before 1 hours", "Published Post"
            };

            foreach (var item in templist1)
            {
                string Name = item;
                var NotificationId = GetNotificationId(Name);
                var obj = new ReactWithASP.Server.Models.Notification
                {
                    UserGuid = Gid,
                    Name = item,
                    CreatedOn = DateTime.Now,
                    NotificationId = NotificationId,
                    Title = " ",
                    Message = " ",
                    Status = false
                };
                _context.Notification.Add(obj);
            }
            _context.SaveChanges();


            // Returning the user data as part of the response
            return Ok(new Response { Status = "Success", Message = "User created successfully!", Data = userData });
        }

        private int GetAccountId(string platformName)
        {
            // Example: Fetch AccountId from a predefined dictionary or database
            var accountMap = new Dictionary<string, int>
            {
                { "Facebook", 1 },
                { "Linkedin", 2 },
                { "Instagram", 3 },
                { "TikTok", 4 },
                { "Whatsapp", 5 },
                { "Telegram", 6 },
                { "Youtube", 7 },
                { "imo", 8 }
            };

            return accountMap.TryGetValue(platformName, out var accountId) ? accountId : 0; // Default 0 if not found
        }

        private int GetNotificationId(string Name)
        {
            // Example: Fetch AccountId from a predefined dictionary or database
            var accountMap1 = new Dictionary<string, int>
            {
                { "Errors", 1 },
                { "Remind Before 1 hours", 2 },
                { "Published Post", 3 }
            };

            return accountMap1.TryGetValue(Name, out var NotificationId) ? NotificationId : 0; // Default 0 if not found
        }




        /*[HttpPost]
        [Route("register")]
        [EnableCors("AllowAll")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            // WriteLog("Start Function4" + DateTime.Now.ToString());
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response { Status = "Error", Message = "Invalid input data", Data = ModelState });
            }

            // Constructing the user object with data from the model (not saving to database)
            var user = new
            {
                model.Username,
                model.Email,
                model.PhoneNumber,
                model.FullName
            };

            // Preparing the user data for the response
            var userData = new
            {
                user.Username,
                user.Email,
                user.PhoneNumber,
                user.FullName
            };
            //  WriteLog("Start Function5" + DateTime.Now.ToString());

            // Returning the user data as part of the response
            return Ok(new Response { Status = "Success", Message = "User created successfully!", Data = userData });
        }*/

        /*[HttpGet("{userGUId}")]
        [EnableCors("AllowAll")]
        [Authorize]
        public async Task<IActionResult> Get(string userGUId)
        {
            //var user = await userManager.FindByIdAsync(userGuid);
            // var user1 = await userManager.Users.FirstOrDefault(userGuid);
            *//*if (user == null)
            {
                return NotFound(new { Status = "Error", Message = "User Not Found" });
            }*//*
           // WriteLog("Start Function6" + DateTime.Now.ToString());
            var response = new
            {
                UserGUId = 10,//user.Id
                FullName = "Manav Jha",//user.FullName,
                UserName = "Manav", //user.UserName,
                photo = "https://www.shutterstock.com/shutterstock/photos/1949415778/display_1500/stock-photo-successful-asian-businessman-in-a-black-business-suit-works-on-a-laptop-relaxes-in-a-restaurant-1949415778.jpg",//user.PhotoUrl,
                Email = "Manav2022@yopmail.com",//user.Email,
                PhoneNumber = 8178971409//user.PhoneNumber,
            };
            return Ok(response);
            *//*var response = new
            {
                UserGUId = model.Id,
                FullName = model.FullName,
                UserName = model.UserName,
                photo = "https://www.shutterstock.com/shutterstock/photos/1949415778/display_1500/stock-photo-successful-asian-businessman-in-a-black-business-suit-works-on-a-laptop-relaxes-in-a-restaurant-1949415778.jpg",//user.PhotoUrl,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
            };
            return Ok(response);*//*
        }*/




        [HttpGet("{userGUId}")]
        [EnableCors("AllowAll")]
        [Authorize]
        public async Task<IActionResult> Get(string userGUId)
        {
            var user = await userManager.FindByIdAsync(userGUId);
            
            if (user == null)
            {
                return NotFound(new { Status = "Error", Message = "User Not Found" });
            }
           
            var response = new
            {
                UserGUId = user.Id,
                FullName = user.FullName,
                UserName = user.UserName,
                photo = "https://www.shutterstock.com/shutterstock/photos/1949415778/display_1500/stock-photo-successful-asian-businessman-in-a-black-business-suit-works-on-a-laptop-relaxes-in-a-restaurant-1949415778.jpg",//user.PhotoUrl,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
            };
            return Ok(response);
        }



        [HttpGet("Get1")]
        [EnableCors("AllowAll")]
        [Authorize]
        public async Task<IActionResult> Get1(string userGUId)
        {
            var user = await userManager.FindByIdAsync(userGUId);
             //var user1 = await userManager.Users.FirstOrDefault(userGUId);
            if (user == null)
            {
                return NotFound(new { Status = "Error", Message = "User Not Found" });
            }
            // WriteLog("Start Function6" + DateTime.Now.ToString());
            var response = new
            {
                UserGUId = user.Id,
                FullName = user.FullName,
                UserName = user.UserName,
                photo = user.PhotoUrl,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
            };
            return Ok(response);
            /*var response = new
            {
                UserGUId = model.Id,
                FullName = model.FullName,
                UserName = model.UserName,
                photo = "https://www.shutterstock.com/shutterstock/photos/1949415778/display_1500/stock-photo-successful-asian-businessman-in-a-black-business-suit-works-on-a-laptop-relaxes-in-a-restaurant-1949415778.jpg",//user.PhotoUrl,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
            };
            return Ok(response);*/
        }

        /*[HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            var userExists = await userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!await roleManager.RoleExistsAsync(UserRoles.User))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            if (await roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await userManager.AddToRoleAsync(user, UserRoles.Admin);
            }

            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }*/

        [HttpPost]
        [Route("GoogleLogin")]
        [EnableCors("AllowAll")]
        public async Task<IActionResult> GoogleLogin([FromBody] string accessToken)
        {
            try
            {
                var externalUser = await ValidateGoogleTokenAsync(accessToken);
                if (externalUser == null)
                {
                    return Unauthorized(new { Status = "Error", Message = "Invalid external access token." });
                }

                var user = await userManager.FindByEmailAsync(externalUser.Email);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = externalUser.Email,
                        Email = externalUser.Email,
                        FullName = externalUser.FullName,
                        PhotoUrl = externalUser.PhotoUrl,
    
                    };

                    var result = await userManager.CreateAsync(user);
                    if (!result.Succeeded)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "User creation failed");
                    }
                }

                var userRoles = await userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    };

                if (!string.IsNullOrEmpty(user.FullName))
                {
                    authClaims.Add(new Claim("FullName", user.FullName));
                }
                if (!string.IsNullOrEmpty(user.Email))
                {
                    authClaims.Add(new Claim("Email", user.Email));
                }
                if (!string.IsNullOrEmpty(user.PhoneNumber))
                {
                    authClaims.Add(new Claim("PhoneNumber", user.PhoneNumber));
                }
                var photoUrl = !string.IsNullOrEmpty(user.PhotoUrl) ? user.PhotoUrl : "https://powerusers.microsoft.com/t5/image/serverpage/image-id/98171iCC9A58CAF1C9B5B9/image-size/large/is-moderation-mode/true?v=v2&px=999";
                authClaims.Add(new Claim("PhotoUrl", photoUrl));
                authClaims.Add(new Claim("userGUId", user.Id));

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddMonths(6),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new
                {
                    AccessToken = tokenString
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during login: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        private async Task<ExternalUser> ValidateGoogleTokenAsync(string accessToken)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"https://www.googleapis.com/oauth2/v3/tokeninfo?id_token={accessToken}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var tokenInfo = JsonConvert.DeserializeObject<GoogleTokenInfo>(jsonResponse);

                    if (tokenInfo != null && !string.IsNullOrEmpty(tokenInfo.Email))
                    {
                        return new ExternalUser
                        {
                            Email = tokenInfo.Email,
                            FullName = tokenInfo.Name,
                            PhotoUrl = tokenInfo.Picture
                        };
                    }
                }
            }
            return null;
        }



        [HttpPost]
        [Route("FacebookLogin")]
        [EnableCors("AllowAll")]
        public async Task<IActionResult> FacebookLogin([FromBody] string accessToken)
        {
            try
            {
                // Validate the external access token and get the user info
                var externalUser = await ValidateFacebookTokenAsync(accessToken);
                if (externalUser == null)
                {
                    return Unauthorized(new { Status = "Error", Message = "Invalid external access token." });
                }

                // Find the user in your database
                var user = await userManager.FindByEmailAsync(externalUser.Email);
                if (user == null)
                {
                    // If the user doesn't exist, create a new user
                    user = new ApplicationUser
                    {
                        UserName = externalUser.Email,
                        Email = externalUser.Email,
                        FullName = externalUser.FullName,
                        PhotoUrl = externalUser.PhotoUrl,
                        // Set other properties as needed
                    };

                    var result = await userManager.CreateAsync(user);
                    if (!result.Succeeded)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "User creation failed");
                    }
                }

                // Generate JWT token
                var userRoles = await userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

                if (!string.IsNullOrEmpty(user.FullName))
                {
                    authClaims.Add(new Claim("FullName", user.FullName));
                }
                if (!string.IsNullOrEmpty(user.Email))
                {
                    authClaims.Add(new Claim("Email", user.Email));
                }
                if (!string.IsNullOrEmpty(user.PhoneNumber))
                {
                    authClaims.Add(new Claim("PhoneNumber", user.PhoneNumber));
                }
                var photoUrl = !string.IsNullOrEmpty(user.PhotoUrl) ? user.PhotoUrl : "https://powerusers.microsoft.com/t5/image/serverpage/image-id/98171iCC9A58CAF1C9B5B9/image-size/large/is-moderation-mode/true?v=v2&px=999";
                authClaims.Add(new Claim("PhotoUrl", photoUrl));
                authClaims.Add(new Claim("userGUId", user.Id));

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddMonths(6),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new
                {
                    AccessToken = tokenString
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during login: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        private async Task<ExternalUser> ValidateFacebookTokenAsync(string accessToken)
        {
            using (var httpClient = new HttpClient())
            {
                var appAccessTokenResponse = await httpClient.GetStringAsync($"https://graph.facebook.com/oauth/access_token?client_id={_configuration["Facebook:AppId"]}&client_secret={_configuration["Facebook:AppSecret"]}&grant_type=client_credentials");
                var appAccessToken = JsonConvert.DeserializeObject<FacebookAppAccessToken>(appAccessTokenResponse);

                var response = await httpClient.GetStringAsync($"https://graph.facebook.com/debug_token?input_token={accessToken}&access_token={appAccessToken.AccessToken}");
                var tokenInfo = JsonConvert.DeserializeObject<FacebookTokenInfo>(response);

                if (tokenInfo.Data.IsValid)
                {
                    var userInfoResponse = await httpClient.GetStringAsync($"https://graph.facebook.com/me?fields=email,name,picture&access_token={accessToken}");
                    var userInfo = JsonConvert.DeserializeObject<FacebookUserInfo>(userInfoResponse);

                    return new ExternalUser
                    {
                        Email = userInfo.Email,
                        FullName = userInfo.Name,
                        PhotoUrl = userInfo.Picture.Data.Url
                    };
                }
            }
            return null;
        }

        [HttpPost]
        [Route("UpdateNotificationStatus")]
        public async Task<IActionResult> UpdateNotification(string UserGuid, bool status, string[] NotificationIds, bool Allnotification)
        {
            List<ReactWithASP.Server.Models.Notification> Result;

            if (Allnotification)
            {
                // Fetch all notifications for the user
                Result = _context.Notification.Where(x => x.UserGuid == UserGuid).ToList();
            }
            else
            {
                // Fetch specific notifications for the user
                Result = _context.Notification.Where(x => x.UserGuid == UserGuid && NotificationIds.Contains(x.NotificationId.ToString())).ToList();
            }

            if (Result != null && Result.Count > 0)
            {
                // Update the status for each notification
                foreach (var notification in Result)
                {
                    notification.Status = status;
                }

                // Save changes to the database
                _context.Notification.UpdateRange(Result);
                await _context.SaveChangesAsync();

                // Return the updated records
                return Ok(new { Status = "True", Message = "Update Successfully" });
            }
            else
            {
                return NotFound(new { Status = "Error", Message = "Data Not Found" });
            }
        }

        [HttpGet]
        [Route("GetNotificationwithStatus")]
        public async Task<IActionResult> GetNotification(string UserGuid)
        {
            var results = _context.Notification.Where(x => x.UserGuid == UserGuid).ToList();
            if (results != null && results.Any())
            {
                
                var Notification1 = new List<object>();

                foreach (var result in results)
                {
                    
                    var notificationObj = new 
                    {
                        UserGuid = result.UserGuid,
                        Name = result.Name,
                        Status = result.Status,
                        CreatedOn =result.CreatedOn,
                        Title = result.Title,
                        Message = result.Message,
                        NotificationId = result.NotificationId
                    };

                    Notification1.Add(notificationObj);
                }

                return Ok(Notification1);
            }
            else
            {
                return NotFound(new { Status = "Error", Message = "Data Not Found" });
            }
        }


        [HttpPost]
        [Route("UpdateAcc_Connection")]
        public async Task<IActionResult> UpdateAcc_Connection(string UserGuid, int AccountId, bool status)
        {
            var Result = _context.AccountConnection.FirstOrDefault(x => x.UserGuid == UserGuid && x.AccountId == AccountId);
            if (Result != null)
            {
                Result.Status = status;

                // Save changes to the database
                _context.AccountConnection.Update(Result);
                _context.SaveChanges();

                // Return the updated record
                return Ok(new { Status = "True", Message = "Update Successfully" });

            }

            else
            {
                return NotFound(new { Status = "Error", Message = "Data Not Found" });
            }
        }



        [HttpGet]
        [Route("GetAcc_Connection")]
        public async Task<IActionResult> GetAcc_Connection(string UserGuid)
        {
            var results = _context.AccountConnection.Where(x => x.UserGuid == UserGuid).ToList();

            if (results != null && results.Any())
            {
                var baseUrl = $"{Request.Scheme}://{Request.Host}"; // Construct base URL
                //var baseUrl = _hostingEnvironment.ContentRootPath;
                var accountConnections = new List<object>();

                foreach (var result in results)
                {
                    // Generate the relative path for the image

                    string relativePath = Path.Combine("uploads", "images", result.AccountIcon);

                    // Construct the full URL
                    string imageUrl = new Uri(new Uri(baseUrl), relativePath).ToString();

                    var accCon = new
                    {
                        UserGuid = result.UserGuid,
                        AccountName = result.AccountName,
                        AccountIcon = imageUrl,
                        AccountId = result.AccountId,
                        Status = result.Status
                    };

                    accountConnections.Add(accCon);
                }

                return Ok(accountConnections);
            }
            else
            {
                return NotFound(new { Status = "Error", Message = "Data Not Found" });
            }
        }

    }
}