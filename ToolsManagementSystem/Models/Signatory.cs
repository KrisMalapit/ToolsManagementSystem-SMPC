using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ToolsManagementSystem.Models{

    public class Signatory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Display(Name = "Signatory Label")]
        public string SignatoryLabel { get; set; }
        [Display(Name = "Employee Name")]
        public string Name { get; set; }
        [StringLength(25)]
        public string Designation { get; set; }
        public string Report { get; set; }
        public string Company { get; set; }
        public string Status { get; set; }

        public Signatory()
        {
            Status = "Active";
        }
    }


}