using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ToolsManagementSystem.Models
{
    //public enum CategoryType
    //{
    //    CME, GPE
    //}

    public class Item
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        [Index("IX_ItemCode", 1, IsUnique = true)]
        [StringLength(50)]
        [Display(Name = "Item Code")]
        public string ItemCode { get; set; }

        //[Index("IX_ItemCodeDescription", 2, IsUnique = true)]
        //[StringLength(100)]
        [Required]
        public string Description { get; set; }

        [Display(Name = "Description 2")]
        //[Index("IX_ItemCodeDescription", 3, IsUnique = true)]
        //[StringLength(100)]
        public string Description2 { get; set; }
        
        [Display(Name = "Model")]
        public string ModelType { get; set; }
        [Display(Name = "Unit of Measurement")]
        public string UOM { get; set; }
        [Display(Name = "Serial No")]
        public string SerialNo { get; set; }

        [Required]
        [Display(Name = "Shelf No")]
        public string ShelfNo { get; set; }

        //[Required]
        public string Category { get; set; }
        [Required]
        public string Location { get; set; }
        [Display(Name = "Equipment Type")]
        public string EquipmentType { get; set; }
        [Index("IX_ItemCode", 2, IsUnique = true)]
        [StringLength(20)]
        public string Status { get; set; }
        public virtual ICollection<ItemDetail> ItemDetails { get; set; }

        [NotMapped]
        public int Inventory { get; set; }
        public Item()
        {
            Status = "Active";
        }
    }
   

    public class JsonArray {
        public string message { get; set; }
        public string status { get; set; }
    }

}