/*namespace ReactWithASP.Server.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly IFacebookAuthService _facebookAuthService;

        public IdentityService(IFacebookAuthService facebookAuthService)
        {
            _facebookAuthService = facebookAuthService;
        }

        public async Task<AuthenticationResult> LoginWithFacebookAsync(string accessToken)
        {
            var validateTokenResult = await _facebookAuthService.ValidateAccessTokenAsync(accessToken);
            if (!validateTokenResult.Data.IsValid)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "Invalid facebook token" }
                };
            }

            var userInfo = await _facebookAuthService.GetUserInfoAsync(accessToken);
            var user = await _userManager.FindByEmailAsync(userInfo.Email);

            if (user == null)
            {
                var identityUser = new Identityuser
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = userInfo.Email,
                    UserName = userInfo.Email
                };

                var createResult = await _userManager.CreateAsync(identityUser);
                if (!createResult.Succeeded)
                {*//**//*
                    return new AuthenticationResult
                    {
                        Errors = new[] { "Something went wrong" }
                    };
                }

                return await GenerateAuthenticationResultForUserAsync(identityUser);
            }

            return await GenerateAuthenticationResultForUserAsync(user);
        }
    }
}
*/

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ReactWithASP.Server.Models.External.FacebookContracts;
using ReactWithASP.Server.Services;

namespace ASSR.Server.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly IFacebookAuthService _facebookAuthService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public IdentityService(IFacebookAuthService facebookAuthService, UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _facebookAuthService = facebookAuthService;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<AuthenticationResult> LoginWithFacebookAsync(string accessToken)
        {
            var validateTokenResult = await _facebookAuthService.ValidateAccessTokenAsync(accessToken);
            if (!validateTokenResult.Data.IsValid)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "Invalid facebook token" }
                };
            }

            var userInfo = await _facebookAuthService.GetUserInfoAsync(accessToken);
            var user = await _userManager.FindByEmailAsync(userInfo.Email);

            if (user == null)
            {
                var identityUser = new IdentityUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = userInfo.Email,
                    UserName = userInfo.Email
                };

                var createResult = await _userManager.CreateAsync(identityUser);
                if (!createResult.Succeeded)
                {
                    return new AuthenticationResult
                    {
                        Errors = new[] { "Something went wrong" }
                    };
                }

                return await GenerateAuthenticationResultForUserAsync(identityUser);
            }

            return await GenerateAuthenticationResultForUserAsync(user);
        }

        /*private Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(IdentityUser user)
        {
            // Implement token generation logic

            throw new NotImplementedException();
        }*/

        private async Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(IdentityUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            //var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var authClaims = new List<Claim>
         {
    new Claim(ClaimTypes.Name, user.UserName),
    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
          };
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };


            var token = new JwtSecurityToken(
                        issuer: _configuration["JWT:ValidIssuer"],
                        audience: _configuration["JWT:ValidAudience"],
                        expires: DateTime.Now.AddMonths(6),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new AuthenticationResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = GenerateRefreshToken() // Implement this method if you need refresh tokens
            };
        }
        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

    }
}
