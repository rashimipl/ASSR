using JWTAuthentication.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PayPal.Api;
using ReactWithASP.Server.Models;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ReactWithASP.Server.Controllers
{
    [Route("api")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public PaymentsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }


        [HttpGet]
        [Route("AllPaymentsReceiveList")]
        //[Authorize]
        public async Task<IActionResult> AllPaymentsReceiveList(bool? paymentStatus, DateTime SubsStartDate, DateTime SubsEndDate, string sortColumn = "Id", string sortOrder = "asc", int pageNumber = 1, int pageSize = 10)
        {
            var usersQuery = _context.TransectionDetails.ToList();

            if (usersQuery != null)
            {
                // Sorting
                switch (sortColumn.ToLower())
                {
                    case "SubsPlanID":
                        usersQuery = sortOrder.ToLower() == "asc" ? usersQuery.OrderBy(u => u.PlanId).ToList() : usersQuery.OrderByDescending(u => u.PlanId).ToList();
                        break;
                    case "id":
                    default:
                        usersQuery = sortOrder.ToLower() == "asc" ? usersQuery.OrderBy(u => u.Id).ToList() : usersQuery.OrderByDescending(u => u.Id).ToList(); ;
                        break;
                }


                if (paymentStatus != null)
                {
                    usersQuery = usersQuery.Where(x => x.PaymentStatus == paymentStatus).ToList();
                }

                // Filtering by Subscription Start Date
                if (SubsStartDate != DateTime.MinValue)
                {
                    usersQuery = usersQuery.Where(x => x.CreatedOn >= SubsStartDate.Date).ToList();
                }

                // Filtering by Subscription End Date
                if (SubsEndDate != DateTime.MinValue)
                {
                    usersQuery = usersQuery.Where(x => x.CreatedOn <= SubsEndDate.Date).ToList();
                }

                // Check if start date is before end date
                if (SubsStartDate != DateTime.MinValue && SubsEndDate != DateTime.MinValue && SubsStartDate > SubsEndDate)
                {
                    return BadRequest("Start Date cannot be greater than End Date.");
                }

                // Pagination
                var totalRecords = usersQuery.Count;
                var users = usersQuery.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                if (users == null || !users.Any())
                {
                    return NotFound("Data not found");
                }

                // Creating pagination metadata
                var paginationMetadata = new
                {
                    totalRecords = totalRecords,
                    pageSize = pageSize,
                    currentPage = pageNumber,
                    totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                    data = users
                };

                // Return results along with pagination metadata
                return Ok(new { status = true, paginationMetadata });
            }
            else
            {
                return NotFound("Data not found !..");
            }
        }

        [HttpPost]
        [Route("SaveTransectionDetails")]
        public async Task<IActionResult> SaveTransectionDetails([FromBody] SaveTransectionDetails model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.PayerId) ||
                    string.IsNullOrEmpty(model.PaymentId) ||
                    string.IsNullOrEmpty(model.Token))
                {
                    return BadRequest(new { Message = "Field Missing" });
                }
                var info1 = _context.UserSubscriptions.FirstOrDefault(x => x.UserGuid == model.UserGuid);
                var info2 = _context.SubscriptionPlans.FirstOrDefault(x => x.PlanId == info1.SubsPlanId);

                var transDetails = new TransectionDetails
                {
                    UserGuid = model.UserGuid,
                    PaymentId = model.PaymentId,
                    PayerId = model.PayerId,
                    Token = model.Token,
                    PaymentStatus = true,
                    PlanId = info1.SubsPlanId,
                    Price = info2.Price,
                    CreatedOn = DateTime.Now
                };

                await _context.TransectionDetails.AddAsync(transDetails);
                await _context.SaveChangesAsync();

                return Ok(new { Status = "true", Message = "Transaction details saved successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while saving transaction details.", Error = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetTransectionDetailsbyId")]
        public async Task<IActionResult> GetTransectionDetailsbyId(int id)
        {
            var usersQuery = _context.TransectionDetails.FirstOrDefault(x=>x.Id==id);

            if (usersQuery != null)
            {
                return Ok(new { status = true, usersQuery });
            }
            else
            {
                return NotFound("Data not found !..");
            }
        }

        [HttpGet]
        [Route("GetUserTransectionsDeailsbyId")]
        public async Task<IActionResult> GetUserTransectionsDeailsbyId(string Userguid)
        {
            var result = (from us in _context.Users
                          join td in _context.TransectionDetails on us.Id equals td.UserGuid
                          where us.Id == Userguid
                          select new GetUserTransectionsDeailsbyId
                          {
                              UserGuid = us.Id,
                              UserName = us.FullName,
                              PaymentId = td.PaymentId,
                              PayerId = td.PayerId,
                              Token = td.Token,
                              PaymentStatus = td.PaymentStatus,
                              PlanId = td.PlanId,
                              Price = td.Price,
                              CreatedOn = td.CreatedOn
                          });

            if (result != null)
            {
                return Ok(new { status = true, result });
            }
            else
            {
                return NotFound("Data not found !..");
            }
        }

        [HttpGet]
        [Route("GetRecentTransectionDetails")]
        public async Task<IActionResult> GetRecentTransectionDetails()
        {
            var usersQuery = _context.TransectionDetails.OrderByDescending(x=>x.Id).Take(5).ToList();

            if (usersQuery != null)
            {
                return Ok(new { status = true, usersQuery });
            }
            else
            {
                return NotFound("Data not found !..");
            }
        }

    }
}