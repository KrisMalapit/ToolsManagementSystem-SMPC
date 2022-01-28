using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ToolsManagementSystem.Models
{
    public class AFBorrowerReturn
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int AFBorrowerIssueID { get; set; }
        public int Quantity { get; set; }
        public DateTime DateReturned { get; set; }
        public DateTime DatePosted { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public string ToolStatus { get; set; }
        public int DocStatus { get; set; }
        public AFBorrowerReturn()
        {
            Status = "Active";
            DocStatus = 0;
            DatePosted = DateTime.Now.Date;
           
        }
        public virtual AFBorrowerIssue AFBorrowerIssues { get; set; }
    }
}