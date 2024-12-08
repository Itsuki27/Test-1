//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.ComponentModel.DataAnnotations;
//using System.Runtime.Remoting.Lifetime;

//namespace WebApplication1.Models
//{
//    [MetadataType(typeof(UserForm))]
//    public  partial class User
//    {
//        public  string ConfirmPassword { get; set; }
//    }

//    public class UserForm
//    {

//        public int UserId { get; set; }

//        [Required(AllowEmptyStrings = false, ErrorMessage = "Username is Required")]
//        [Display(Name = "Username")]
//        public string Username { get; set; }

//        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is Required")]
//        [Display(Name = "Password")]
//        [DataType(DataType.Password)]
//        [MinLength(8,ErrorMessage = "Must be at least 8 characters")]
//        public string PasswordHash { get; set; }

//        [Display(Name = "Confirm Password")]
//        [Required(AllowEmptyStrings = false, ErrorMessage = " Confirm Password is Required")]
//        [MinLength(8, ErrorMessage = "Must be at least 8 characters")]
//        [DataType(DataType.Password)]
//        [Compare("PasswordHash", ErrorMessage = "Password does not match")]
//        public string ConfirmPassword { get; set; }

//        [Display(Name = "Email")]
//        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required")]
//        [EmailAddress(ErrorMessage = "Invalid Email Address")]
//        public string Email { get; set; }
//        public Nullable<System.DateTime> CreatedDate { get; set; } = DateTime.Now;
//        public Nullable<bool> IsActive { get; set; } = true;
//        public string ResetPasswordCode { get; set; }
//        public Nullable<int> DEPT_ID { get; set; } = 1;

//        public DateTime? ResetPasswordExpiry { get; set; }

//        public virtual DEPT DEPT { get; set; }
//    }
//}