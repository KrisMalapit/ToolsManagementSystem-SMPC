using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToolsManagementSystem.Models.View_Model
{
    public class GroupViewModel
    {
        public int id { get; set; }
        public string empid { get; set; }
        public string groupname { get; set; }
        public string employeename { get; set; }
        public GroupViewModel() { }
        public GroupViewModel(GroupViewModel item)
        {

            id = item.id;
            empid = item.empid;
            groupname = item.groupname;
            employeename = item.employeename;
            

        }
        
        public class GroupMemberViewModel
        {
            public int EmployeeID { get; set; }
            
        }
       
    }
}