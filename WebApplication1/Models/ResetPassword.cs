using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class ResetPassword
    {
        [Required]
        public string ResetCode { get; set; }

        [Required(ErrorMessage = "New Password required", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        [StringLength(12, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 12 characters.")]
        public string PasswordHash { get; set; }

        [DataType(DataType.Password)]
        [Compare("PasswordHash", ErrorMessage = "New Password and Confirm Password does not match")]
        [StringLength(12, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 12 characters.")]
        public string ConfirmPassword { get; set; }

    }
}