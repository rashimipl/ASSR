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
using ReactWithASP.Server.Models.Posts;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static YourNamespace.Controllers.AccountController;

namespace ReactWithASP.Server.Controllers
{
    [Route("api")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        [Route("ChangeStatus")]
        [Authorize]
        public IActionResult ChangeStatus(string status, string UserGUID)
        {
            var result = _context.UserSubscriptions.Where(u => u.UserGuid == UserGUID).ToList();

            if (result.Count == 0)
            {
                return BadRequest(new { Message = " Data Not Found!..." });
            }
            else
            {
                foreach (var subscription in result)
                {
                    subscription.Status = status;
                }

                _context.SaveChanges();
                return Ok(new { Message = " Status Update Successfully" });

            }
        }
    [HttpGet]
    [Route("UserDetailList")]
    public async Task<IActionResult> UserDetailList(
    string? sortColumn = null,
    string? sortOrder = null,
    int? pageNumber = null,
    int? pageSize = null)
    {
      // Apply fallback defaults dynamically
      string sortBy = string.IsNullOrWhiteSpace(sortColumn) ? "Id" : sortColumn;
      string order = string.IsNullOrWhiteSpace(sortOrder) ? "asc" : sortOrder.ToLower();
      int currentPage = pageNumber.GetValueOrDefault(1);
      int currentPageSize = pageSize.GetValueOrDefault(10);

      // Base query
      var query = _context.Users.Where(x => x.IsActive);

      // Dynamic sorting
      switch (sortBy.ToLower())
      {
        case "name":
          query = order == "asc"
              ? query.OrderBy(u => u.FullName)
              : query.OrderByDescending(u => u.FullName);
          break;
        case "id":
        default:
          query = order == "asc"
              ? query.OrderBy(u => u.Id)
              : query.OrderByDescending(u => u.Id);
          break;
      }

      // Total count before pagination
      var totalRecords = await query.CountAsync();

      // Pagination
      var users = await query
          .Skip((currentPage - 1) * currentPageSize)
          .Take(currentPageSize)
          .ToListAsync();

      // Handle empty result
      if (users == null || !users.Any())
      {
        return NotFound(new
        {
          status = false,
          message = "No users found for the given criteria."
        });
      }

      // Final response
      return Ok(new
      {
        status = true,
        paginationMetadata = new
        {
          totalRecords = totalRecords,
          pageSize = currentPageSize,
          currentPage = currentPage,
          totalPages = (int)Math.Ceiling(totalRecords / (double)currentPageSize)
        },
        data = users
      });
    }

    //[HttpGet]
    //[Route("UserDetailList")]
    ////[Authorize]
    //public async Task<IActionResult> UserDetailList(string sortColumn, string sortOrder, int pageNumber, int pageSize)
    //{

    //  var usersQuery = _context.Users.Where(x => x.IsActive).ToList();

    //  if (usersQuery != null)
    //  {
    //    // Sorting
    //    switch (sortColumn.ToLower())
    //    {
    //      case "name":
    //        usersQuery = sortOrder.ToLower() == "asc" ? usersQuery.OrderBy(u => u.FullName).ToList() : usersQuery.OrderByDescending(u => u.FullName).ToList();
    //        break;
    //      case "id":
    //      default:
    //        usersQuery = sortOrder.ToLower() == "asc" ? usersQuery.OrderBy(u => u.Id).ToList() : usersQuery.OrderByDescending(u => u.Id).ToList(); ;
    //        break;
    //    }

    //    // Pagination
    //    var totalRecords = usersQuery.Count;
    //    var usersold = usersQuery.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
    //    var users = usersQuery.ToList();
    //    if (users == null || !users.Any())
    //    {
    //      return NotFound("No user found for the given criteria.");
    //    }

    //    // Creating pagination metadata
    //    var paginationMetadata = new
    //    {
    //      totalRecords = totalRecords,
    //      pageSize = pageSize,
    //      currentPage = pageNumber,
    //      totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
    //      data = users
    //    };

    //    // Return results along with pagination metadata
    //    return Ok(new { status = true, paginationMetadata });
    //  }
    //  else
    //  {
    //    return NotFound("Data not found !..");
    //  }
    //}

    [HttpGet]
        [Route("UserDetailListById")]
        //[Authorize]
        public async Task<IActionResult> UserDetailListById(string UserGuid)
        {
            // Fetch the active users based on the UserGuid
            var usersQuery = _context.Users.Where(x => x.Id == UserGuid && x.IsActive);

            if (usersQuery.Any())
            {
                
                return Ok(usersQuery);
            }
            else
            {
                return NotFound("Data not found!..");
            }
        }


        [HttpGet]
        [Route("GetAllSubscriptionPlanbyUserId")]
        public IActionResult GetAllSubscriptionPlanbyUserId(string UserGUID)
        {
            var query = from sp in _context.SubscriptionPlans
                        join us in _context.UserSubscriptions on sp.Id equals us.SubsPlanId
                        join u in _context.Users on us.UserGuid equals u.Id
                        where u.Id == UserGUID
                        select new subscriptionplanbyuserid
                        {
                            UserGUID = u.Id,
                            PlanName = sp.PlanName,
                            Price = sp.Price,
                            SubsPlanID = us.SubsPlanId,
                            FullName = u.FullName,
                            Email = u.Email,
                            PhoneNumber = u.PhoneNumber,
                            CreatedOn = u.CreatedOn,
                        };

            var result = query.ToList();
            if (result.Count == 0)
            {
                return BadRequest(new { Message = " Data Not Found!..." });
            }
            return Ok(result);
        }



        [HttpDelete("DeleteUser")]
        public IActionResult DeleteUser(string userGuid)
        {
            if (string.IsNullOrEmpty(userGuid))
            {
                return BadRequest(new { Message = "User GUID is required." });
            }
            var ExitingUser = _context.Users.FirstOrDefault(x => x.Id == userGuid && x.IsActive);

            ExitingUser.IsActive = false;

            _context.Users.Update(ExitingUser);
            _context.SaveChanges();

            return Ok(new
            {
                Status = "True",
                Message = "User deleted successfully"
            });
        }


        [HttpPut("UpdateUserStatus")]
        public IActionResult UpdateUserStatus(string userguid, bool Status)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find the group by UserGuid and GroupId
            var existingUser = _context.Users.FirstOrDefault(g => g.Id == userguid && g.IsActive);

            if (existingUser == null)
            {
                return NotFound(new
                {
                    Status = "False",
                    Message = "User not found"
                });
            }

            // Update group properties
            existingUser.LockoutEnabled = Status;
            _context.Users.Update(existingUser);
            // Save changes to the context
            _context.SaveChanges();

            return Ok(new
            {
                Status = "True",
                Message = "Status updated successfully"
            });
        }


    }
}