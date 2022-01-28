using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ToolsManagementSystem.Models
{
    public class TempTable
    {
        [Key]

        public int id { get; set; }
        public int ItemID { get; set; }
        public string ItemCode { get; set; }
        public string ItemCategory { get; set; }
        public string FACardNo { get; set; }
        public string PropertyNo { get; set; }
        public string RefNo { get; set; }
    }
}