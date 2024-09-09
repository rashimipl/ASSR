using JWTAuthentication.Authentication;
using Microsoft.AspNetCore.Mvc;
using ReactWithASP.Server.Models;
using ReactWithASP.Server.Services;
using System.Linq;

[ApiController]
[Route("api/paypal")]
public class PayPalController : ControllerBase
{
    private readonly PayPalService _payPalService;
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;

    public PayPalController(PayPalService payPalService, IConfiguration configuration, ApplicationDbContext context)
    {
        _payPalService = payPalService;
        _configuration = configuration;
        _context = context;
    }

    [HttpPost("create")]
    public IActionResult CreatePayment([FromBody] decimal amount)
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var payment = _payPalService.CreatePayment(baseUrl, "sale", "paypal", amount, "USD", "Payment description");

        var approvalUrl = payment.links.FirstOrDefault(link => link.rel == "approval_url")?.href;

        // Log transaction to the database
        var transaction = new PayPalTransactions
        {
            PaymentId = payment.id,
            Amount = amount,
            Currency = "USD",
            PaymentStatus = "Created",
            CreatedAt = DateTime.UtcNow
        };

        _context.PayPalTransactions.Add(transaction);
        _context.SaveChanges();

        return Ok(new { approvalUrl });
    }

    [HttpGet("success")]
    public IActionResult ExecutePayment(string paymentId, string UserGuid)
    {
        var payment = _payPalService.ExecutePayment(paymentId, UserGuid);

        if (payment.state.ToLower() != "approved")
        {
            return BadRequest("Payment not approved.");
        }

        // Update transaction status in the database
        var transaction = _context.PayPalTransactions.FirstOrDefault(t => t.PaymentId == paymentId);
        if (transaction != null)
        {
            transaction.UserGuid = UserGuid;
            transaction.PaymentStatus = "Approved";
            transaction.UpdatedAt = DateTime.UtcNow;
            _context.SaveChanges();
        }

        return Ok("Payment successful.");
    }

    [HttpGet("cancel")]
    public IActionResult Cancel(string paymentId)
    {
        // Update transaction status in the database if necessary
        var transaction = _context.PayPalTransactions.FirstOrDefault(t => t.PaymentId == paymentId);
        if (transaction != null)
        {
            transaction.PaymentStatus = "Cancelled";
            transaction.UpdatedAt = DateTime.UtcNow;
            _context.SaveChanges();
        }

        return BadRequest("Payment cancelled.");
    }
}
