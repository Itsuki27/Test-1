//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }

        [Required(ErrorMessage = "One or more fields are empty. Please fill in all required fields.")]
        public string PasswordHash { get; set; }
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; } = DateTime.Now;
        public Nullable<bool> IsActive { get; set; } = true;
        public string ResetPasswordCode { get; set; }
        public Nullable<int> DEPT_ID { get; set; }
    
        public virtual DEPT DEPT { get; set; }
    }
}
