using ReactWithASP.Server.Models.External.FacebookContracts;
using System.Threading.Tasks;

namespace ReactWithASP.Server.Services
{
    public interface IFacebookAuthService
    {
        Task<FacebookTokenValidationResult> ValidateAccessTokenAsync(string accessToken);
        Task<FacebookUserInfoResult> GetUserInfoAsync(string accessToken);
    }

}
