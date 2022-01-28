using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ToolsManagementSystem.Models
{
    public class UserDept
    {
        [Key]
        public int id { get; set; }
        public int UserID { get; set; }
        public int DepartmentID { get; set; }
        public virtual User Users { get; set; }
        public virtual Department Departments { get; set; }
    }
}