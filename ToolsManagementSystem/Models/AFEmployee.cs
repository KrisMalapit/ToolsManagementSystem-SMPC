using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
namespace ToolsManagementSystem.Models
{
    public class AFEmployee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        //[Required]

        [Index("IX_REFCode", 1, IsUnique = true)]
        [StringLength(50)]
        [Display(Name = "EAC No")]
        public string EACNo { get; set; }
        [Display(Name = "Old Ref No")]
        public string ORefNo { get; set; }
        [Required]
        [Display(Name = "Employee Name")]
        public int EmployeeID { get; set; }
        public string Status { get; set; }
        public int DocStatus { get; set; }
        [Required]
        public string Purpose { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Created_At { get; set; }
        public DateTime DateReleased { get; set; }
        public virtual Employee Employees { get; set; }
        public virtual ICollection<AFEmployeeIssue> AFEmployeeIssues { get; set; }
        public string Remarks { get; set; }
        public string TransType { get; set; }
        public int UserID { get; set; } 
        public virtual User Users { get; set; }
        public AFEmployee() {
            Status = "Active";
            Created_At = DateTime.Now.Date;
            DateReleased = DateTime.Now.Date;
            DocStatus = 0;
        }

    }
    static class SubstringExtensions
    {
        /// <summary>
        /// Get string value between [first] a and [last] b.
        /// </summary>
        public static string Between(this string value, string a, string b)
        {
            int posA = value.IndexOf(a);
            int posB = value.LastIndexOf(b);
            if (posA == -1)
            {
                return "";
            }
            if (posB == -1)
            {
                return "";
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= posB)
            {
                return "";
            }
            return value.Substring(adjustedPosA, posB - adjustedPosA);
        }

        /// <summary>
        /// Get string value after [first] a.
        /// </summary>
        public static string Before(this string value, string a)
        {
            int posA = value.IndexOf(a);
            if (posA == -1)
            {
                return "";
            }
            return value.Substring(0, posA);
        }

        /// <summary>
        /// Get string value after [last] a.
        /// </summary>
        public static string After(this string value, string a)
        {
            int posA = value.LastIndexOf(a);
            if (posA == -1)
            {
                return "";
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= value.Length)
            {
                return "";
            }
            return value.Substring(adjustedPosA);
        }
    }
}