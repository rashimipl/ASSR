using JWTAuthentication.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReactWithASP.Server.Models;

namespace ReactWithASP.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChangePasswordController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment;

        public ChangePasswordController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
            Environment = environment;
        }
        /* public void WriteLog(string message)
         {
             System.IO.File.AppendAllText(Environment.WebRootPath + "\\PdfLog.txt", message);
         }*/
        /*[HttpPost]
        [Route("ChangePassword1")]
        public async Task<ResponseModel> ChangePassword1([FromBody] ChangePasswordRequest model)
        {
            ResponseModel _objResponseModel = new ResponseModel();



            if (!ModelState.IsValid)
            {
                var aa = ModelState.SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage).FirstOrDefault();
                _objResponseModel.Data = "";
                _objResponseModel.Status = false;
                _objResponseModel.Message = aa;
                return _objResponseModel;
            }
            var checkuser = await userManager.FindByIdAsync(model.userGUId);
            if (checkuser != null)
            {
                var result = await userManager.ChangePasswordAsync(checkuser, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await userManager.FindByIdAsync(model.userGUId);
                    if (user != null)
                    {

                        _objResponseModel.Data = "";
                        _objResponseModel.Status = true;
                        _objResponseModel.Message = "Password changed successfully";
                    }
                }
                else
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    _objResponseModel.Data = "";
                    _objResponseModel.Status = false;
                    _objResponseModel.Message = result.Errors.FirstOrDefault()string.Join("; ", errors);
                }
            }
            else
            {
                _objResponseModel.Data = "";
                _objResponseModel.Status = false;
                _objResponseModel.Message = "UserId does not exist";
            }
            return _objResponseModel;
        }
*/

        //Static

        /*[HttpPost]
        [Authorize]
        public async Task<ResponseModel> ChangePassword([FromBody] ChangePasswordRequest model)
        {
         //   WriteLog("changePassword1" + DateTime.Now.ToString());
            ResponseModel _objResponseModel = new ResponseModel();

            if (!ModelState.IsValid)
            {
                var errorMessage = ModelState.SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage).FirstOrDefault();
                _objResponseModel.Data = "";
                _objResponseModel.Status = false;
                _objResponseModel.Message = errorMessage;
                return _objResponseModel;
            }

            var staticUserId = "10";
            var staticOldPassword = "Manav@123";
            var staticNewPassword = "Manav@1234"; // Hardcoded new password

            if (model.userGUId != staticUserId)
            {
                _objResponseModel.Data = "";
                _objResponseModel.Status = false;
                _objResponseModel.Message = "UserId does not exist";
                return _objResponseModel;
            }

            if (model.OldPassword != staticOldPassword)
            {
                _objResponseModel.Data = "";
                _objResponseModel.Status = false;
                _objResponseModel.Message = "Old password is incorrect";
                return _objResponseModel;
            }
         //   WriteLog("changePassword2" + DateTime.Now.ToString());
            if (string.IsNullOrEmpty(model.NewPassword) || model.NewPassword != model.ConfirmPassword)
            {
                _objResponseModel.Data = "";
                _objResponseModel.Status = false;
                _objResponseModel.Message = "New password and confirm password do not match or are empty";
                return _objResponseModel;
            }

            // Simulate password change logic
            staticOldPassword = model.NewPassword;

            _objResponseModel.Data = "";
            _objResponseModel.Status = true;
            _objResponseModel.Message = "Password changed successfully";
          //  WriteLog("changePassword3" + DateTime.Now.ToString());
            return _objResponseModel;
        }*/


        [HttpPost]
        [Route("ChangePassword")]
        public async Task<ResponseModel> ChangePassword([FromBody] ChangePasswordRequest model)
        {
            ResponseModel _objResponseModel = new ResponseModel();



            if (!ModelState.IsValid)
            {
                var aa = ModelState.SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage).FirstOrDefault();
                _objResponseModel.Data = "";
                _objResponseModel.Status = false;
                _objResponseModel.Message = aa;
                return _objResponseModel;
            }
            var checkuser = await userManager.FindByIdAsync(model.userGUId);
            if (checkuser != null)
            {
                var result = await userManager.ChangePasswordAsync(checkuser, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await userManager.FindByIdAsync(model.userGUId);
                    if (user != null)
                    {

                        _objResponseModel.Data = "";
                        _objResponseModel.Status = true;
                        _objResponseModel.Message = "Password changed successfully";
                    }
                }
                else
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    _objResponseModel.Data = "";
                    _objResponseModel.Status = false;
                    _objResponseModel.Message = "Password does not changed..";
                }
            }
            else
            {
                _objResponseModel.Data = "";
                _objResponseModel.Status = false;
                _objResponseModel.Message = "UserId does not exist";
            }
            return _objResponseModel;
        }


    }
}
