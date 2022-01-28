using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;

namespace ToolsManagementSystem.Models
{
    public class Log
    {
  
    

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        HttpBrowserCapabilities bc = HttpContext.Current.Request.Browser;

        public int id { get; set; }
        public string descriptions { get; set; }
        public string otherinfo { get; set; }
        public DateTime created_date { get; set; }
        public string user_id { get; set; }
        public string browser { get; set; }
        public Log() {
            created_date = DateTime.Now;
            user_id = HttpContext.Current.User.Identity.Name‌;
            browser = bc.Browser + " , Version : " + bc.Version;

            
        }
    }
}