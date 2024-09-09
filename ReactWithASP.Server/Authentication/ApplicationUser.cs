using Microsoft.AspNetCore.Identity;

namespace JWTAuthentication.Authentication
{
    public class ApplicationUser : IdentityUser
    {
        
        public string? FullName { get; set; }
        public string? PhotoUrl { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsActive { get; set; }
    }
}