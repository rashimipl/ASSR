﻿using JWTAuthentication.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayPal;
using ReactWithASP.Server.Models;
using System.Collections.Immutable;

namespace ReactWithASP.Server.Controllers
{
    [ApiController]
    [Route("api")]
    public class SocialMediaController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment;
        private readonly ApplicationDbContext _context;

        public SocialMediaController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment, ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
            Environment = environment;
            _context = context;
        }

        /*[HttpGet("GetUserSocialMedia")]
        [Authorize]
        public IActionResult GetUserSocialMedia([FromQuery] UserSocialMedia request)
        {
            var response = new List<SocialMedia>
            {
                new SocialMedia
                {
                    Id = -1,
                    SocialMediaName = "All",
                    //status = 1,
                    src = "NULL"
                },
                new SocialMedia
                {
                    Id = 1,
                    SocialMediaName = "facebook",
                    //status = 0,
                    src = "../img/facebook-fill.png"

                },
                new SocialMedia
                {
                    Id = 2,
                     SocialMediaName = "linkedin",
                     //status = 0,
                    src = "../img/linkedin-fill.png"
                },
                new SocialMedia
                {
                    Id = 3,
                    SocialMediaName = "instagram",
                    //status = 1,
                    src = "../img/instagram-fill.png"
                },
                 new SocialMedia
                {
                    Id = 4,
                    SocialMediaName = "tiktok",
                    //status = 1,
                    src = "../img/tiktok-fill.png"
                },
                  new SocialMedia
                {
                    Id = 5,
                    SocialMediaName = "whatsapp",
                    //status = 1,
                    src = "../img/whatsapp-fill.png"
                },
                   new SocialMedia
                {
                    Id = 6,
                    SocialMediaName = "telegram",
                    //status = 0,
                    src = "../img/telegram-fill.png"
                },
                 new SocialMedia
                {
                    Id = 7,
                    SocialMediaName = "youtube",
                   // status = 0,
                    src = "../img/youtube-fill.png"
                },
                 new SocialMedia
                {
                    Id = 8,
                    SocialMediaName = "imo",
                   // status = 0,
                    src = "../img/imo-fill.png"
                },

            };

            return Ok(response);
        }*/
        [HttpGet("GetUserSocialMedia")]
        //[Authorize]
        public async Task<IActionResult> GetUserSocialMedia(string UserGuid)
        {
            var results = _context.AccountConnection.Where(x => x.UserGuid == UserGuid && x.Status == true).ToList();

            var accountConnections = new List<object>();
            var baseUrl1 = $"{Request.Scheme}://{Request.Host}"; // Construct base URL
            // Check if there is more than one result
            if (results != null && results.Count > 1)
            {
                // Add the default entry for "All" only if there is more than one record
                var baseUrl = baseUrl1; // Construct base URL
                var defaultSocialMedia = new
                {
                    id = -1, // Assign a unique ID for the default entry
                    socialMediaName = "All",
                    src = " ", // Adjust the image URL if necessary
                    status = true
                };
                accountConnections.Add(defaultSocialMedia); // Add default entry to the list
            }


            if (results != null && results.Any())
            {
                foreach (var result in results)
                {
                    // Generate the relative path for the image
                    string relativePath = Path.Combine("uploads", "images", result.AccountIcon);

                    // Construct the full URL
                    string imageUrl = new Uri(new Uri(baseUrl1), relativePath).ToString();

                    var accCon = new
                    {
                        id = result.AccountId,
                        socialMediaName = result.AccountName,
                        src = imageUrl,
                        status = result.Status
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


        //public async Task<IActionResult> GetUserSocialMedia(string UserGuid)
        //{
        //    var results = _context.AccountConnection.Where(x => x.UserGuid == UserGuid && x.Status == true).ToList();

        //    if (results != null && results.Any())
        //    {
        //        var baseUrl = $"{Request.Scheme}://{Request.Host}"; // Construct base URL
        //                                                            //var baseUrl = _hostingEnvironment.ContentRootPath;

        //        var accountConnections = new List<object>();

        //        foreach (var result in results)
        //        {
        //            // Generate the relative path for the image

        //            string relativePath = Path.Combine("uploads", "images", result.AccountIcon);

        //            // Construct the full URL
        //            string imageUrl = new Uri(new Uri(baseUrl), relativePath).ToString();

        //            var accCon = new
        //            {
        //                id = result.AccountId,
        //                socialMedia = result.AccountName,
        //                src = imageUrl,
        //                status = result.Status
        //            };

        //            accountConnections.Add(accCon);
        //        }

        //        return Ok(accountConnections);
        //    }
        //    else
        //    {
        //        return NotFound(new { Status = "Error", Message = "Data Not Found" });
        //    }
        //}




        /*[HttpGet("GetUserSocialMedia")]
        public async Task<IActionResult> GetUserSocialMedia([FromQuery] string userGuid)
        {
            if (string.IsNullOrEmpty(userGuid))
            {
                return BadRequest("User is required.");
            }

            // Check if the userGuid exists in the database
            var userExists = await _context.UserSocialMediaStatus
                                            .AnyAsync(u => u.UserGuid == userGuid);

            if (!userExists)
            {
                return NotFound("User not found.");
            }

            // Fetch the data if userGuid exists
            var result = await _context.UserSocialMediaStatus
                                       .Where(u => u.UserGuid == userGuid)
                                       .Select(u => new
                                       {
                                           u.Id,
                                           SocialMediaName = u.SocialMedia.SocialMediaName,
                                           status  = u.Status,
                                           src =  u.SocialMedia.Src

                                       })
                                       .ToListAsync();

            return Ok(result);
        }*/




        //[HttpGet("Accounts")]
        //[Authorize]
        //public IActionResult GetUserAccounts([FromQuery] UserSocialMedia request)
        //{
        //    var response = new List<SocialMediaUser>
        //    {
        //        new SocialMediaUser
        //        {
        //            Id = 1,
        //            UserAccountName = "The Rock",
        //            socialMediaUrl = "http://167.86.105.98:8070/uploads/facebook-fill.png",
        //            AccountIcon = "https://i.pinimg.com/736x/f7/c5/b7/f7c5b75d09ddbeacec2e0c939513b9e7.jpg",
        //            Status = 1
        //        },
        //        new SocialMediaUser
        //        {
        //            Id = 2,
        //            UserAccountName = "The Rock",
        //            socialMediaUrl = "http://167.86.105.98:8070/uploads/instagram-fill.png",
        //            AccountIcon = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSxWI-Dn4ZHql0uWlpQo2MnU0bj147UM5AJww&s",
        //            Status = 0
        //        },
        //        new SocialMediaUser
        //        {
        //            Id = 3,
        //            UserAccountName = "Dwen Johnson",
        //            socialMediaUrl = "http://167.86.105.98:8070/uploads/facebook-fill.png",
        //            AccountIcon = "https://i.pinimg.com/736x/f7/c5/b7/f7c5b75d09ddbeacec2e0c939513b9e7.jpg",
        //            Status = 1
        //        },
        //        new SocialMediaUser
        //        {
        //            Id = 4,
        //            UserAccountName = "Dwen Johnson",
        //            socialMediaUrl = "http://167.86.105.98:8070/uploads/tiktok-fill.png",
        //            AccountIcon = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSxWI-Dn4ZHql0uWlpQo2MnU0bj147UM5AJww&s",
        //            Status = 0
        //        },

        //    };

        //    return Ok(response);
        //}

        //[HttpGet("Accounts")]
        //[Authorize]
        //public IActionResult GetUserAccounts([FromQuery] UserSocialMedia request)
        //{
        //    // Fetch user social media accounts based on UserGuid
        //    var result = _context.ConnectedSocialMediaInfo
        //                          .Where(x => x.UserId == request.UserGuid)
        //                          .ToList();

        //    // Check if any social media accounts were found
        //    if (!result.Any())
        //    {
        //        return NotFound(new { Message = "No social media accounts found for the user." });
        //    }

        //    // Map the result to the desired response format
        //    var response = result.Select(account => new UserAccounts()
        //    {
        //        Id = account.Id,
        //        UserAccountName = account.PageName,              // Use page name for the userName
        //        AccountIcon = account.PageProfile,        // Using the profile picture from the DB
        //        socialMediaUrl = _context.SocialMedia
        //                       .FirstOrDefault(x => x.Id == account.SocialMediaAccId)?.src // Assuming ImageUrl is the field for the image
        //    }).ToList();
        //    // Return the list of mapped accounts
        //    return Ok(response);
        //}

        [HttpGet("Accounts")]
        public IActionResult GetUserAccounts([FromQuery] UserSocialMedia request)
        {
            // Fetch user social media accounts based on UserGuid
           
            var result = _context.ConnectedSocialMediaInfo
                                 .Where(x => x.UserId == request.UserGuid) // Limit to 1000 records
                                 .ToList();
            // Check if any social media accounts were found
            if (!result.Any())
            {
                return NotFound(new { Message = "No social media accounts found for the user." });
            }

            // Map the result to the desired response format
            var response = result.Select(account =>
            {
                // Add null check for account
                if (account == null)
                {
                    // Log this incident if necessary
                    return null; // Skip this account
                }

                var socialMedia = _context.SocialMedia.FirstOrDefault(x => x.Id == account.SocialMediaAccId);

                // Ensure socialMedia is not null before accessing its properties
                var socialMediaUrl = socialMedia != null ? socialMedia.src : "default_url.png";

                return new UserAccounts()
                {
                  //Id = account.Id,
                  Id = account.Id,
                  accountId = socialMedia.Id,                  
                  PageId = account.PageId,
                    UserAccountName = account.PageName ?? "N/A", // Provide a default if PageName is null
                    AccountIcon = account.PageProfile ?? "default_icon.png", // Provide a default icon if null
                    socialMediaUrl = socialMediaUrl, // Use the determined socialMediaUrl
                  socialMediaAccId= account.SocialMediaAccId
                };
            }).Where(x => x != null).ToList(); // Filter out null results

            // Return the list of mapped accounts, ensuring response is not empty
            if (!response.Any())
            {
                return NotFound(new { Message = "No valid social media accounts found." });
            }

            return Ok(response);
        }




        [HttpGet("UserAccounts")]
        public IActionResult UserAccounts([FromQuery] UserSocialMedia request)
        {
            // Fetch user social media accounts based on UserGuid
            var result = _context.ConnectedSocialMediaInfo
                                  .Where(x => x.UserId == request.UserGuid)
                                  .ToList();

            // Check if any social media accounts were found
            if (!result.Any())
            {
                return NotFound(new { Message = "No social media accounts found for the user." });
            }
     
      // Map the result to the desired response format
      var response = result.Select(account => new UserMediaAccounts()
            {
        Id = account.Id,
        accountId = account.SocialMediaAccId ?? 0,
              PageId = account.PageId,
                socialMedia = account.SocialMediaAccName, // Use social media account name from DB
                userName = account.PageName,              // Use page name for the userName
                profileIcon = account.PageProfile,        // Using the profile picture from the DB
                socialMediaImage = _context.SocialMedia.FirstOrDefault(x => x.Id == account.SocialMediaAccId)?.src
                //Id= account.SocialMediaAccId??0

      }).ToList();


            //Return the list of mapped accounts
            return Ok(response);
            //return Ok(new
            //{
            //    Status = true,
            //    Message = response.Any() ? "User accounts retrieved successfully." : "No accounts found.",
            //    Data = response
            //});
        }


        //public IActionResult UserAccounts([FromQuery] UserSocialMedia request)
        //{
        //    var response = new List<UserMediaAccounts>
        //    {

        //        new UserMediaAccounts
        //        {
        //            Id = 1,
        //            socialMedia = "facebook",
        //            userName = "Manav09",
        //            status = 0,
        //            profileIcon = "https://img.freepik.com/free-photo/portrait-young-teen-tourist-visiting-great-wall-china_23-2151261879.jpg",
        //            socialMediaImage = "http://167.86.105.98:8070/uploads/facebook-fill.png"
        //        },
        //        new UserMediaAccounts
        //        {
        //            Id = 2,
        //            socialMedia = "facebook",
        //            userName = "Manavjha731",
        //            status = 0,
        //            profileIcon = "https://img.freepik.com/free-photo/portrait-young-teen-tourist-visiting-great-wall-china_23-2151261879.jpg",
        //            socialMediaImage = "http://167.86.105.98:8070/uploads/facebook-fill.png"
        //        },
        //        new UserMediaAccounts
        //        {
        //            Id = 3,
        //            socialMedia = "linkedin",
        //            userName = "Manav Jha",
        //            status = 0,
        //            profileIcon = "https://img.freepik.com/free-photo/portrait-young-teen-tourist-visiting-great-wall-china_23-2151261879.jpg",
        //            socialMediaImage = "http://167.86.105.98:8070/uploads/linkedin-fill.png"
        //        },

        //          new UserMediaAccounts
        //        {
        //            Id = 4,
        //            socialMedia = "whatsapp",
        //            userName = "Manav Singh",
        //            status = 1,
        //            profileIcon = "https://img.freepik.com/free-photo/portrait-young-teen-tourist-visiting-great-wall-china_23-2151261879.jpg",
        //            socialMediaImage = "http://167.86.105.98:8070/uploads/whatsapp-fill.png"
        //        },
        //             new UserMediaAccounts
        //        {
        //            Id = 5,
        //            socialMedia = "whatsapp",
        //            userName = "Singh Manav",
        //            status = 1,
        //            profileIcon = "https://img.freepik.com/free-photo/portrait-young-teen-tourist-visiting-great-wall-china_23-2151261879.jpg",
        //            socialMediaImage = "http://167.86.105.98:8070/uploads/whatsapp-fill.png"
        //        },
        //             new UserMediaAccounts
        //        {
        //            Id = 6,
        //            socialMedia = "telegram",
        //            userName = "ManavTech",
        //            status = 1,
        //            profileIcon = "https://img.freepik.com/free-photo/portrait-young-teen-tourist-visiting-great-wall-china_23-2151261879.jpg",
        //            socialMediaImage = "http://167.86.105.98:8070/uploads/telegram-fill.png"
        //        },
        //          new UserMediaAccounts
        //        {
        //            Id = 7,
        //            socialMedia = "telegram",
        //            userName = "ManavTech",
        //            status = 1,
        //            profileIcon = "https://img.freepik.com/free-photo/portrait-young-teen-tourist-visiting-great-wall-china_23-2151261879.jpg",
        //            socialMediaImage = "http://167.86.105.98:8070/uploads/telegram-fill.png"
        //        },
        //          new UserMediaAccounts
        //        {
        //            Id = 8,
        //            socialMedia = "youtube",
        //            userName = "Manav React tutorial",
        //            status = 1,
        //            profileIcon = "https://img.freepik.com/free-photo/portrait-young-teen-tourist-visiting-great-wall-china_23-2151261879.jpg",
        //            socialMediaImage = "http://167.86.105.98:8070/uploads/youtube-fill.png"
        //        },
        //            new UserMediaAccounts
        //        {
        //            Id = 9,
        //            socialMedia = "youtube",
        //            userName = "Manav React tutorial",
        //            status = 1,
        //            profileIcon = "https://img.freepik.com/free-photo/portrait-young-teen-tourist-visiting-great-wall-china_23-2151261879.jpg",
        //            socialMediaImage = "http://167.86.105.98:8070/uploads/youtube-fill.png"
        //        },
        //            new UserMediaAccounts
        //        {
        //            Id = 10,
        //            socialMedia = "imo",
        //            userName = "Manav",
        //            status = 1,
        //            profileIcon = "https://img.freepik.com/free-photo/portrait-young-teen-tourist-visiting-great-wall-china_23-2151261879.jpg",
        //            socialMediaImage = "http://167.86.105.98:8070/uploads/imo-fill.png"
        //        },
        //          new UserMediaAccounts
        //        {
        //            Id = 11,
        //            socialMedia = "imo",
        //            userName = "ManavJi",
        //            status = 1,
        //            profileIcon = "https://img.freepik.com/free-photo/portrait-young-teen-tourist-visiting-great-wall-china_23-2151261879.jpg",
        //            socialMediaImage = "http://167.86.105.98:8070/uploads/imo-fill.png"
        //        },

        //    };

        //    return Ok(response);
        //}


    }
}
