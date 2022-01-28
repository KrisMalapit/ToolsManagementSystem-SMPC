using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
namespace ToolsManagementSystem.Models
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Display(Name = "Employee ID")]
        [Required]
        public string EmpId { get; set; }
        [Display(Name = "Last Name")]
        [Required]
        public string LastName { get; set; }
        [Display(Name = "First Name")]
        [Required]
        public string FirstName { get; set; }
        [Required]
        public int DepartmentID { get; set; }
        [Required]
        public int DesignationID { get; set; }
        public string EntryType { get; set; } 
        public string Status { get; set; }
        public virtual Department Departments{ get; set; }
        public virtual Designation Designations { get; set; }
        
        public Employee() {
            Status = "Active";
            EntryType = "Individual";
           
        }
    }
}