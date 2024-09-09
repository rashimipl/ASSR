
using ReactWithASP.Server.Models.External.FacebookContracts;
using System.Threading.Tasks;

namespace ASSR.Server.Services
{
    public interface IIdentityService
    {
        Task<AuthenticationResult> LoginWithFacebookAsync(string accessToken);
    }
}
