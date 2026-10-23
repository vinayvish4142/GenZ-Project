using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GenZApp.Models
{
    public class Authentication
    {
    }
    public class RegisterDto
    {
        [Required]
        public string FullName { get; set; } = "";

        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [RegularExpression(@"^[0-9]{10}$",ErrorMessage = "Enter a valid 10-digit mobile number.")]
        public string Mobile { get; set; } = "";

        [Required, MinLength(8)]
        public string Password { get; set; } = "";
    }

    public class LoginDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";
    }
    public class RefreshTokenDto
    {
        [Required]
        public string RefreshToken { get; set; } = "";
    }

    public class AuthResponseDto
    {
        public bool SuccessMsg { get; set; }
        public string Message { get; set; }
        public string AccessToken { get; set; } = "";
        public string RefreshToken { get; set; } = "";
        public DateTime ExpiresAt { get; set; }
        public string Email { get; set; } = "";
        public IList<string> Roles { get; set; } = new List<string>();
    }
    public class ResponseMsg
    {
        public Boolean SuccessMsg{ get; set; }
        public string Message { get; set; }

    }
}
