    using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ToolsManagementSystem.Models
{
    public enum Domain
    {
        SEMCALACA, SMCDACON, SEMIRARAMINING
    }
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        public string Username { get; set; }
        public string Roles { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public string mail { get; set; }
        public string status { get; set; }
        public string sysid { get; set; }
        public string Name { get; set; }

        //public virtual Role Roles { get; set; }
    }

}