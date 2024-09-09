using System.ComponentModel.DataAnnotations;

namespace ReactWithASP.Server.Models
{
    public class ChangePasswordRequest
    {
        [Required]
        public string userGUId { get; set; }
        [Required]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [RegularExpression(@"^(?=.*[A-Z]).*$", ErrorMessage = "The new password must contain at least one uppercase letter.")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
