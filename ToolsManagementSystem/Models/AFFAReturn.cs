using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ToolsManagementSystem.Models
{
    public class AFFAReturn
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string FAARFNo { get; set; }
        public int AFFAIssueID { get; set; }
        public int Quantity { get; set; }
        public DateTime DateReturned { get; set; }
        public DateTime DatePosted { get; set; }
        public string ToolStatus { get; set; }
        public string Recommendation { get; set; }
        public string Remarks { get; set; }
        public string FindingsObservation { get; set; }
        public string Status { get; set; }
        public int DocStatus { get; set; }
        public AFFAReturn()
        {
            Status = "Active";
            DocStatus = 0;
            DatePosted = DateTime.Now.Date;
        }
        public virtual AFFAIssue AFFAIssues { get; set; }
    }
}