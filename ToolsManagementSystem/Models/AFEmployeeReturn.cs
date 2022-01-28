using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
namespace ToolsManagementSystem.Models
{
    public class AFEmployeeReturn
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int AFEmployeeIssueID { get; set; }
        public int Quantity { get; set; }

        public DateTime DateReturned { get; set; }
        public DateTime DatePosted { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public string ToolStatus { get; set; }
        public DateTime Created_At { get; set; }
        public int DocStatus { get; set; }
        public AFEmployeeReturn(){
            Status = "Active";
            Created_At = DateTime.Now.Date;
            DocStatus = 0;
            DatePosted = DateTime.Now.Date;
            
        }
        public virtual AFEmployeeIssue AFEmployeeIssues { get; set; }
    }
}