//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Web;

//namespace WebApplication1.Models
//{

//    [MetadataType(typeof(Department))]
//    public  partial class DEPT
//    {

//    }

//    public class Department
//    {
//        public int DEPT_ID { get; set; }


//        [Required(ErrorMessage = "This Field Is Required", AllowEmptyStrings = false)]
//        [Display(Name = "Department")]
//        public string DEPT1 { get; set; }


//        [Required(ErrorMessage = "This Field Is Required", AllowEmptyStrings = false)]
//        [Display(Name = "Department Description")]
//        public string DEPT_DESC { get; set; }

//        [Display(Name = "Department Building")]
//        public string DEPT_BLDG { get; set; }

//        [Display(Name = "Department Floor")]
//        public string DEPT_FLOOR { get; set; }
//    }
//}