using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
namespace ToolsManagementSystem.Models
{
    public class AFFA
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Index("IX_REFCode", 1, IsUnique = true)]
        [StringLength(50)]
        [Display(Name = "FAAF No")]
        public string FAAFNo { get; set; }
        [Required]
        [Display(Name = "Employee Name")]
        public int EmployeeID { get; set; }
        public string Status { get; set; }
        public int DocStatus { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime DateReleased { get; set; }
        public string Remarks { get; set; }
        public string TransType { get; set; }
        public virtual Employee Employees { get; set; }
        public virtual ICollection<AFFAIssue> AFFAIssues { get; set; }
        public int UserID { get; set; } 
        public virtual User Users { get; set; }
        public AFFA()
        {
            Status = "Active";
            Created_At = DateTime.Now.Date;
            DateReleased = DateTime.Now.Date;
            DocStatus = 0;
            
        }
    }
}