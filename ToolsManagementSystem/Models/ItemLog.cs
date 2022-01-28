using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ToolsManagementSystem.Models
{
    public class ItemLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int ItemID { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public string Module { get; set; }
        public string EntryType { get; set; }
        public DateTime Created_At { get; set; }
        public int Quantity { get; set; }
        public string user_id { get; set; }
        public virtual Item Items { get; set; }
        public ItemLog()
        {
            Created_At = DateTime.Now;
            user_id = HttpContext.Current.User.Identity.Name;
        }
    }
}