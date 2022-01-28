using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ToolsManagementSystem.Models
{
    public class GroupAccountability
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        [Index("IX_GroupAccountabilityCode", 1, IsUnique = true)]
        [StringLength(50)]
        [Display(Name = "Code")]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Department Name")]
        public int DepartmentID { get; set; }
        public string Status { get; set; }


    }
}