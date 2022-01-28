using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ToolsManagementSystem.Models
{
    [Table("V_GroupMembers")]
    public class VGroupMember
    {
        [Key]
        public Guid Id { get; set; }
        public string GroupCode { get; set; }
        public string Members { get; set; }
    }
}