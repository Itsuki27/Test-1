
using System.Security.Policy;

namespace WebApplication1.Models
{
    using System.Web.Mvc;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class DEPT
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DEPT()
        {
            this.Users = new HashSet<User>();
        }
        public int DEPT_ID { get; set; }


        [Required(ErrorMessage = "This Field Is Required")]
        [Display(Name = "Department")]
        public string DEPT1 { get; set; }


        [Required(ErrorMessage = "This Field Is Required")]
        [Display(Name = "Department Description")]
        public string DEPT_DESC { get; set; }

        [Display(Name = "Department Building")]
        public string DEPT_BLDG { get; set; }

        [Display(Name = "Department Floor")]
        public string DEPT_FLOOR { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<User> Users { get; set; }
    }
}