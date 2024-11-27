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
        [StringLength(12, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 12 characters.")]
        [DataType(DataType.Password)]

        public string PasswordHash { get; set; }

        [DataType(DataType.Password)]
        [Compare("PasswordHash", ErrorMessage = "Password does not match")]
        [StringLength(12, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 12 characters.")]
        public string ConfirmPassword { get; set; }

    }
}

