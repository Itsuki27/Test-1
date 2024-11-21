using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class MyUserAll
    {
        public int UserId { get; set; }
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string PasswordHash { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Compare("PasswordHash", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; } = DateTime.Now;
        public Nullable<bool> IsActive { get; set; } = true;
        public string ResetPasswordCode { get; set; }

    }
}