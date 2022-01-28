using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ToolsManagementSystem.Models
{
    public class GroupAccountabilityMember
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        [Index("IX_EmployeeGroup", 1, IsUnique = true)]
        public int EmployeeID { get; set; }
        [Required]
        [Index("IX_EmployeeGroup", 2, IsUnique = true)]
        public int GroupAccountabilityID { get; set; }

        public virtual Employee Employees { get; set; }
        public virtual GroupAccountability GroupAccountabilities { get; set; }
    }
}