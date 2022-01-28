using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ToolsManagementSystem.Models
{
    public class AFBorrowerAccountable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int AFBorrowerID { get; set; }
        public int AccountableID { get; set; }
        public string AccountableType { get; set; }
        public virtual AFBorrower AFBorrowers { get; set; }

    }
}