using Microsoft.AspNetCore.Mvc;
using JWTAuthentication.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ReactWithASP.Server.Models;
using Microsoft.AspNetCore.Authorization;

namespace ReactWithASP.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForgotPasswordController : Controller
    {
        private static string _verificationCode;
        private static Dictionary<string, string> _userPasswords = new Dictionary<string, string>
    {
        { "anubhav.j@mishainfotech.com", "OldPassword123" } // Simulated user data
    };
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment;
        private readonly ApplicationDbContext _context;
        public ForgotPasswordController(UserManager<ApplicationUser> userManager, IEmailSender emailSender, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment, ApplicationDbContext context)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            Environment = environment;
            _context = context;

        }
        /* public void WriteLog(string message)
         {
             System.IO.File.AppendAllText(Environment.WebRootPath + "\\PdfLog.txt", message);
         }
 */
        /*[HttpPost("request1")]
        public async Task<IActionResult> RequestPasswordReset1([FromBody] ForgotPasswordRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var code = GenerateVerificationCode();

            user.SecurityStamp = code;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return StatusCode(500, "Internal server error");
            }

            await _emailSender.SendEmailAsync(model.Email, "Password Reset Verification Code", $"Your verification code is: {code}");

            //return Ok("Verification code sent to your email", code);
            return Ok(new { message = "Verification code sent to your email", code });
        }*/


        [HttpPost("request")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] ForgotPasswordRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var code = GenerateVerificationCode();

            user.SecurityStamp = code;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return StatusCode(500, "Internal server error");
            }

            // Load and populate the email template
            string emailTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "Emails", "ForgetPassword.cshtml");
            string emailBody;
            using (StreamReader reader = new StreamReader(emailTemplatePath))
            {
                emailBody = await reader.ReadToEndAsync();
            }
            emailBody = emailBody.Replace("{{VerificationCode}}", code);

            _emailSender.SendEmailAsync(model.Email, "Password Reset Verification Code", emailBody);

            return Ok(new { message = "Verification code sent to your email", code });
        }







        //Static
        /*[HttpPost]
        [Route("request")]
        
        public async Task<IActionResult> RequestPasswordReset([FromBody] ForgotPasswordRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
           // WriteLog("request1" + DateTime.Now.ToString());

            var email = "anubhav.j@mishainfotech.com";

            var code = GenerateVerificationCode();
            _verificationCode = code;

            await _emailSender.SendEmailAsync(email, "Password Reset Verification Code", $"Your verification code is: {code}");
          //  WriteLog("request2" + DateTime.Now.ToString());
            return Ok(new { message = "Verification code sent to your email", code });
        }
*/
        /*[HttpPost("verify")]
        public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || user.SecurityStamp != model.Code)
            {
                return BadRequest("Invalid code");
            }

            return Ok("Code verified");
        }*/

        /*[HttpPost("verify")]
        public IActionResult VerifyCode([FromBody] VerifyCodeRequest model)
        {
            WriteLog("verify1" + DateTime.Now.ToString());
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (model.Code == _verificationCode)
            {
                return Ok(new { message = "Code verified" });
            }
            
            else
            {
                return BadRequest(new { message = "Invalid code" });
            }
            
        }*/
        //Static

        /*[HttpPost("verify")]
        public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var email = "Manav2202@yopmail.com";
            if (!CodeStorage.TryGetCode(email, out var storedCode) || storedCode != model.Code)
            {
                return BadRequest("Invalid code");
            }

            // Remove the code after successful verification
            CodeStorage.RemoveCode(email);

            return Ok("Code verified");
        }*/

        /*[HttpPost("reset1")]
        public async Task<IActionResult> ResetPassword1([FromBody] ResetPasswordRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || user.SecurityStamp != model.Code)
            {
                return BadRequest("Invalid code or email");
            }

            var result = await _userManager.RemovePasswordAsync(user);
            if (!result.Succeeded)
            {
                return StatusCode(500, "Internal server error");
            }

            result = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (!result.Succeeded)
            {
                return StatusCode(500, "Internal server error");
            }

            user.SecurityStamp = Guid.NewGuid().ToString();
            await _userManager.UpdateAsync(user);

            return Ok("Password reset successfully");
        }*/


        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || user.SecurityStamp != model.Code)
            {
                return BadRequest("Invalid code or email");
            }

            var result = await _userManager.RemovePasswordAsync(user);
            if (!result.Succeeded)
            {
                return StatusCode(500, "Internal server error");
            }

            result = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (!result.Succeeded)
            {
                return StatusCode(500, "Internal server error");
            }

            user.SecurityStamp = Guid.NewGuid().ToString();
            await _userManager.UpdateAsync(user);

            return Ok("Password reset successfully");
        }

        /*[HttpPost]
        [Route("reset")]
        
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest model)
        {
            //  WriteLog("reset1" + DateTime.Now.ToString());
            //var verifyCode = _context.Users.Where(x => );
           
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (model.Email != "anubhav.j@mishainfotech.com" || model.Code != _verificationCode)
                {
                    return BadRequest("Invalid code or email");
                }
           //     WriteLog("reset2" + DateTime.Now.ToString());
                if (_userPasswords.ContainsKey(model.Email))
                {
                    _userPasswords[model.Email] = model.NewPassword;
                    _verificationCode = null; // Invalidate the used verification code
                    return Ok("Password reset successfully");
                }
                else
                {
                    return BadRequest("User not found");
                }
            

        }*/





        /*private string GenerateVerificationCode()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var bytes = new byte[4];
                rng.GetBytes(bytes);
                return BitConverter.ToUInt32(bytes, 0).ToString("D4");
            }
        }*/
        private string GenerateVerificationCode()
        {

            using (var rng = new RNGCryptoServiceProvider())
            {
                var bytes = new byte[4];
                rng.GetBytes(bytes);
                var code = BitConverter.ToUInt32(bytes, 0) % 10000;
                return code.ToString("D4");
            }
        }


        [HttpPost("AdminRequest")]
        public async Task<IActionResult> RequestPasswordResetforAdmin([FromBody] ForgotPasswordRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var code = GenerateVerificationCode();

            user.SecurityStamp = code;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return StatusCode(500, "Internal server error");
            }

            // Load and populate the email template
            string emailTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "Emails", "AdminForgetPassword.cshtml");
            string emailBody;
            using (StreamReader reader = new StreamReader(emailTemplatePath))
            {
                emailBody = await reader.ReadToEndAsync();
            }
            emailBody = emailBody.Replace("{{VerificationCode}}", code);

            _emailSender.SendEmailAsync(model.Email, "Password Reset Verification Code", emailBody);

            return Ok(new { message = "Verification code sent to your email", code });
        }


        

    }
}
