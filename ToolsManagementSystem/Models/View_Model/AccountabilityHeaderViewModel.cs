using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ToolsManagementSystem.Models.View_Model
{
    public class AccountabilityHeaderViewModel
    {
        public string RefNo { get; set; }
        public string EmployeeName { get; set; }
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public string Description2 { get; set; }

        public string DateCreated { get; set; }
        public int IssuedQty { get; set; }
        public string Status { get; set; }
    }
}