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



namespace YourNamespace.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment;

        public AccountController(IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment)
        {
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            Environment = environment;
        }
        /*public void WriteLog(string message)
        {
            System.IO.File.AppendAllText(Environment.WebRootPath + "\\PdfLog.txt", message);
        }*/

        /*[HttpPost("updatepicture/{userGUId}")]
        
        public async Task<IActionResult> UpdatePicture(string userGUId,[FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is not selected or is empty.");
            }
            *//*var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { error = "User not found." });
            }*//*

            var user = await _userManager.FindByIdAsync(userGUId);
            if (user == null)
            {
                return NotFound(new { error = "User not found." });
            }

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{uniqueFileName}";

            user.PhotoUrl = fileUrl;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(new { ImageURL = fileUrl });
            }
            //return Ok(new { ImageURL = fileUrl });
            return StatusCode(500, new { error = "An error occurred while updating the profile picture." });

        }*/

        public class User
        {
        public string Id { get; set; }
        public string PhotoUrl { get; set; }
        }

        /* [HttpPost("updatepicture/{userGUId}")]
         [Authorize]
         [EnableCors("AllowAll")]

         public async Task<IActionResult> UpdatePicture(string userGUId, [FromForm] IFormFile file)
         {
            // WriteLog("UpdatePicture1" + DateTime.Now.ToString());
             if (file == null || file.Length == 0)
             {
                 return BadRequest("File is not selected or is empty.");
             }

             var staticUserId = "10";
             if (userGUId != staticUserId)
             {
                 return NotFound(new { error = "User not found." });
             }

             var user = new User { Id = staticUserId };

             var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
             if (!Directory.Exists(uploadsFolder))
             {
                 Directory.CreateDirectory(uploadsFolder);
             }
            // WriteLog("UpdatePicture2" + DateTime.Now.ToString());
             var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
             var filePath = Path.Combine(uploadsFolder, uniqueFileName);

             using (var fileStream = new FileStream(filePath, FileMode.Create))
             {
                 await file.CopyToAsync(fileStream);
             }

             var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{uniqueFileName}";

             // Simulate updating the user object
             user.PhotoUrl = fileUrl;
           //  WriteLog("UpdatePicture3" + DateTime.Now.ToString());
             // Since there's no database, just return success
             return Ok(new { ImageURL = fileUrl });
         }
 */


        [HttpPost("updatepicture/{userGUId}")]
        [Authorize]
        //[EnableCors("AllowAll")]

        public async Task<IActionResult> UpdatePicture(string userGUId, [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is not selected or is empty.");
            }
            //*//var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var user = await _userManager.FindByIdAsync(userId);
            //if (user == null)
            //{
            //    return NotFound(new { error = "User not found." });
            //}
            //*//*

            var user = await _userManager.FindByIdAsync(userGUId);
            if (user == null)
            {
                return NotFound(new { error = "User not found." });
            }

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{uniqueFileName}";

            user.PhotoUrl = fileUrl;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(new { ImageURL = fileUrl });
            }
            //return Ok(new { ImageURL = fileUrl });
            return StatusCode(500, new { error = "An error occurred while updating the profile picture." });

        }
        [HttpGet("signin-facebook")]
        [EnableCors("AllowAll")]
        public IActionResult SignInFacebook()
        {
            //var redirectUrl = Url.Action(nameof(HandleExternalLogin));
            var redirectUrl = Url.Action("HandleExternalLogin", "Auth");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, FacebookDefaults.AuthenticationScheme);
        }
        [HttpGet("handleExternalLogin")]
        [EnableCors("AllowAll")]
        public async Task<IActionResult> HandleExternalLogin()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
                return BadRequest("External authentication error");


            // You can get the Facebook user information here.
            var claims = authenticateResult.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            {
                claim.Type,
                claim.Value
            });

            return Ok(claims);
        }

        [HttpGet("GetUserProfileUrl")]
        public async Task<IActionResult> GetUserProfileUrl(string userGUId)
        {
         var user = await _userManager.FindByIdAsync(userGUId);
         if (user == null)
               return NotFound(new { message = "User not found" });
          
             return Ok(user);
        }
      
    }
}