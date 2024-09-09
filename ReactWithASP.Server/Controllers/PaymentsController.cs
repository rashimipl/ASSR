using JWTAuthentication.Authentication;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Cors;using Microsoft.AspNetCore.Http;using Microsoft.AspNetCore.Identity;using Microsoft.AspNetCore.Mvc;using Microsoft.EntityFrameworkCore;using Microsoft.Extensions.Configuration;using Microsoft.IdentityModel.Tokens;using ReactWithASP.Server.Models;using SendGrid.Helpers.Mail;using System;using System.Collections.Generic;using System.IdentityModel.Tokens.Jwt;using System.Linq;using System.Security.Claims;using System.Text;using System.Threading.Tasks;using System.Xml.Linq;namespace ReactWithASP.Server.Controllers{    [Route("api")]    [ApiController]    public class PaymentsController : ControllerBase    {        private readonly ApplicationDbContext _context;        private readonly UserManager<ApplicationUser> _userManager;        public PaymentsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)        {            _userManager = userManager;            _context = context;        }        [HttpGet]        [Route("AllPaymentsReceiveList")]        [Authorize]        public async Task<IActionResult> AllPaymentsReceiveList(string paymentStatus, string Username, DateTime SubsStartDate, DateTime SubsEndDate, string sortColumn = "Id", string sortOrder = "asc", int pageNumber = 1, int pageSize = 10)        {            var usersQuery = _context.UserSubscriptions.ToList();            if (usersQuery != null)            {
                // Sorting
                switch (sortColumn.ToLower())                {                    case "SubsPlanID":                        usersQuery = sortOrder.ToLower() == "asc" ? usersQuery.OrderBy(u => u.SubsPlanID).ToList() : usersQuery.OrderByDescending(u => u.SubsPlanID).ToList();                        break;                    case "id":                    default:                        usersQuery = sortOrder.ToLower() == "asc" ? usersQuery.OrderBy(u => u.Id).ToList() : usersQuery.OrderByDescending(u => u.Id).ToList(); ;                        break;                }


                //if (usersQuery != null)
                //{
                //    usersQuery = usersQuery.Where(x => x.paymentStatus == paymentStatus).ToList();
                //}
                //if (usersQuery != null)
                //{
                //    usersQuery = usersQuery.Where(x => x.UserName == Username).ToList();
                //}
                if (usersQuery != null)                {                    if (SubsStartDate > SubsEndDate)                    {                        return NotFound("Start Date not greater than End date ");                    }                    usersQuery = usersQuery.Where(x => x.StartDate >= SubsStartDate).ToList();                }                if (usersQuery != null)                {                    if (SubsEndDate < SubsStartDate)                    {                        return NotFound("End date not Smaller than Start Date");                    }                    usersQuery = usersQuery.Where(x => x.EndDate <= SubsEndDate).ToList();                }

                // Pagination
                var totalRecords = usersQuery.Count;                var users = usersQuery.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();                if (users == null || !users.Any())                {                    return NotFound("User not found");                }

                // Creating pagination metadata
                var paginationMetadata = new                {                    totalRecords = totalRecords,                    pageSize = pageSize,                    currentPage = pageNumber,                    totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),                    data = users                };

                // Return results along with pagination metadata
                return Ok(new { status = true, paginationMetadata });            }            else            {                return NotFound("Data not found !..");            }        }    }}