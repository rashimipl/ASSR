/*using Microsoft.AspNetCore.Mvc;
using ReactWithASP.Server.Models.External.FacebookContracts;

namespace ReactWithASP.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : Controller
    {

        [HttpPost("FacebookAuth")]
        public async Task<IActionResult> FacebookAuth([FromBody] UserFacebookAuthRequest request)
        {
            var authResponse = await _identityService.LoginWithFacebookAsync(request.AccessToken);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                });
            }

            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }
    }
}
*/


using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ASSR.Server.Services;
using ReactWithASP.Server.Models.External.FacebookContracts;
using ReactWithASP.Server.Services;

namespace ASSR.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpPost("FacebookAuth")]
        public async Task<IActionResult> FacebookAuth([FromBody] UserFacebookAuthRequest request)
        {
            var authResponse = await _identityService.LoginWithFacebookAsync(request.AccessToken);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                });
            }

            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }
    }
}
