using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
//namespace WebApplication1.Models.Extended
namespace WebApplication1.Models
{
    [MetadataType(typeof(AuditLogsData))]
    public partial class AuditLogs
    {
    }
    public class AuditLogsData
    {
        public long MOVEHIST_ID { get; set; }

        //[Display(Name = "MAC ADDRESS")]
        //[DataType(DataType.PhoneNumber)]
        //[MinLength(10, ErrorMessage = "Minimum Of 6 Characters Required")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Line Number Required")]
        public string MAC_ADDRESS { get; set; }

        [Display(Name = "Type")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Type Required")]
        public string TYPE { get; set; }

        [Display(Name = "Old Data")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Old Data Requied")]
        public string OLD_DATA { get; set; }

        [Display(Name = "New Data")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "New Data Required")]
        public string NEW_DATA { get; set; }

        [Display(Name = "Old Salary")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Old Salary Required")]
        public string OLD_SAL { get; set; }

        [Display(Name = "New Salary")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "New Salary Required")]
        public string NEW_SAL { get; set; }

        [Display(Name = "Date Action")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/DD/YYYY}")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Date Action Required")]
        public string D_ACTION { get; set; }

        [Display(Name = "Time Action")]
        [DataType(DataType.DateTime)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Time Action Required")]
        public string T_ACTION { get; set; }

        [Display(Name = "Description")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Description Required")]
        public string DESCRIPTION { get; set; }

        [Display(Name = "Action By")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Action By Required")]
        public string ACTION_BY { get; set; }

        [Display(Name = "Id")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Id Required")]
        public string Id { get; set; }

        public virtual User User { get; set; }
    }
}