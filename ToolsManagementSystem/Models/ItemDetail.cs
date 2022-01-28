using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ToolsManagementSystem.Models
{
    public class ItemDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int id { get; set; }
        public int ItemID { get; set; }
        public string FACardNo { get; set; }
        public string PropertyNo { get; set; }
        public string Description { get; set; }
        public string Description2 { get; set; }
        public string PO { get; set; }
        public string SerialNo { get; set; }
        public int Qty { get; set; }
        [Display(Name = "Model")]
        public string ModelType { get; set; }
        public string UoM { get; set; }
        public decimal UnitCost { get; set; }
        public string ShelfNo { get; set; }
        public DateTime DateAdjusted { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateDelivered { get; set; }
        public DateTime DateCreated { get; set; }
        public string ToolStatus { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; }
        public string ReferenceNo { get; set; }
        public string Remarks { get; set; }
        public virtual Item Items { get; set; }

        public ItemDetail()
        {
            DateAdjusted = DateTime.Now;
            Status = "Active";
        }
    }
}