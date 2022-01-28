using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToolsManagementSystem.Models.View_Model
{
    public class GroupAccountabilityViewModel
    {
        public int id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string DepartmentName { get; set; }
        public GroupAccountabilityViewModel() { }
        public GroupAccountabilityViewModel(GroupAccountabilityViewModel i)
        {
            Code = i.Code;
            Name = i.Name;
            Description = i.Description;
            Status = i.Status;
            DepartmentName = i.DepartmentName;
        }
    }
}