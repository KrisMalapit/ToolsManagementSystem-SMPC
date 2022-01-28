using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace ToolsManagementSystem.Models.View_Model
{
    public class ItemStockViewModel {
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public string Description2 { get; set; }
        public string UOM { get; set; }
        public string ShelfNo { get; set; }
        public int Available { get; set; }
        public int Damaged { get; set; }
        public int UnReturned { get; set; }
        public int Total { get; set; }
        public ItemStockViewModel() { }
        public ItemStockViewModel (ItemStockViewModel i){
                ItemCode = i.ItemCode;
                Description = i.Description;
                Description2 = i.Description2;
                UOM = i.UOM;
                ShelfNo = i.ShelfNo;
                Available = i.Available;
                Damaged = i.Damaged;
                UnReturned = i.UnReturned;
                Total = i.Total;
        }
        
    }

    public class ReturnViewModel
    {
        public int rowid { get; set; }
        public int refno { get; set; }
        public int qty { get; set; }
        public int qtyreturn { get; set; }
        public DateTime datereturned { get; set; }
        public string toolstatus { get; set; }
        public string recstatus { get; set; }
        public string finstatus { get; set; }
        public string remarks { get; set; }
        public string faarfno { get; set; }
     
            
    }


    public class ItemViewModel
    {

        public int id { get; set; }
        [Display(Name = "Item Code")]
        public string ItemCode { get; set; }
        public string Description { get; set; }
        [Display(Name = "Description 2")]
        public string Description2 { get; set; }
        [Display(Name = "Model")]
        public string ModelType { get; set; }
        [Display(Name = "Unit of Measurement")]
        public string UOM { get; set; }
        [Display(Name = "Serial No")]
        public string SerialNo { get; set; }
        [Display(Name = "Shelf No")]
        public string ShelfNo { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
        public int Inventory { get; set; }
        public string EquipmentType { get; set; }
        public string Blocked { get; set; }
        public ItemViewModel() { }
        public ItemViewModel(ItemViewModel item, int inv)
        {

            id = item.id;
            ItemCode = item.ItemCode;
            Description = item.Description;
            Description2 = item.Description2;
            ModelType = item.ModelType;
            UOM = item.UOM;
            SerialNo = item.SerialNo;
            ShelfNo = item.ShelfNo;
            Category = item.Category;
            Location = item.Location;
            Status = item.Status;
            Inventory = inv;
            EquipmentType = item.EquipmentType;
            Blocked = item.Blocked;


        }
        


    }
}