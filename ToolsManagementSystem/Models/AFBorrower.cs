using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;

namespace ToolsManagementSystem.Models
{
    public class AFBorrower
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Index("IX_REFCode", 1, IsUnique = true)]
        [StringLength(50)]
        [Display(Name = "EBC No")]
        public string EBCNo { get; set; }
        [Display(Name = "Old Ref No")]
        public string ORefNo { get; set; }
        //[Required]
        [Display(Name = "Employee Name")]
        public int EmployeeID { get; set; }
        public string Status { get; set; }
        public int DocStatus { get; set; }
        [Required]
        public string Purpose { get; set; }
        public string WorkOrder { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Created_At { get; set; }
        public DateTime DateReleased { get; set; }


        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DueDate { get; set; }
        public int ContractorID { get; set; }
        public int UserID { get; set; } 
        public string Remarks { get; set; }
        public string TransType { get; set; }
        public virtual Employee Employees { get; set; }
        public virtual ICollection<AFBorrowerIssue> AFBorrowerIssues { get; set; }
        public virtual Contractor Contractors { get; set; }
        public virtual User Users { get; set; }
        public AFBorrower()
        {
            Status = "Active";
            Created_At = DateTime.Now.Date;
            DateReleased = DateTime.Now.Date;
            DocStatus = 0;
        }
    }
}