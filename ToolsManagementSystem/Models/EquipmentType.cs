using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ToolsManagementSystem.Models
{
    public class EquipmentType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }

        public EquipmentType()
        {
            Status = "Active";
        }

    }
}