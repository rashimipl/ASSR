using System.ComponentModel.DataAnnotations;

namespace ReactWithASP.Server.Models
{
    public class ForgotPasswordRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
    }
    public class VerifyCodeRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Code { get; set; }
    }
    public class ResetPasswordRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
