using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ToolsManagementSystem.Models
{
    public class Department
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Index("IX_Code", 1, IsUnique = true)]
        [StringLength(5)]
        public string Code { get; set; }
        [Display(Name = "Department Code")]
        public string DeptCode { get; set; }
        [Display(Name = "Department Name")]
        public string Name { get; set; }
        public string Approver { get; set; }
        [Display(Name = "Approvers Position")]
        public string ApproverPosition { get; set; }

        [Display(Name = "Approver 2 ")]
        public string Approver2 { get; set; }
        [Display(Name = "Approvers 2 Position")]
        public string Approver2Position { get; set; }


        public string Status { get; set; }

        public  Department()
        {
             Status = "Active";
        }
}
}