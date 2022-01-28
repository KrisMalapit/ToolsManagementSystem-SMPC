using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToolsManagementSystem.Models;

namespace ToolsManagementSystem.Models.View_Model
{
    public class AccountabilityForm
    {

    }
    public class EmployeeAF {
        public string EACNo { get; set; }
        public string Employee { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }

        //public virtual Employee Employees { get; set; }
        //public virtual Designation Designations { get; set; }
        //public virtual Department Departments { get; set; }
       
    
    }
}