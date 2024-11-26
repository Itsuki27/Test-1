using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class MyPassword
    {   
        [Required]
        public string ResetCode { get; set; }

        [Required(ErrorMessage = "New Password required" , AllowEmptyStrings =false)]
        [DataType(DataType.Password)]
        public string PasswordHash { get; set; }

        [DataType(DataType.Password)]
        [Compare("PasswordHash", ErrorMessage = "New Password and Confirm Password does not match")]
        public string ConfirmPassword { get; set; }

    }
}