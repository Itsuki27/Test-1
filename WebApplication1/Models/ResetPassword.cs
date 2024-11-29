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
        [StringLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [DataType(DataType.Password)]

        public string PasswordHash { get; set; }

        [DataType(DataType.Password)]
        [Compare("PasswordHash", ErrorMessage = "Password does not match")]
        [StringLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        public string ConfirmPassword { get; set; }

    }
}

