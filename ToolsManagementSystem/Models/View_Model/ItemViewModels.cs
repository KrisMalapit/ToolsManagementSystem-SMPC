using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ToolsManagementSystem.Models.View_Model
{

    public enum CategoryType
    {
        CME, GPE
    }
    public enum ToolStatus
    {
        Serviceable, Un_Serviceable,Lost_Unreturned
    }
    public enum FindingsObservation
    {
        Operational, NonOperational, Obsolete, BeyondEconomicRepair
    }
    public class FixedAssetInput {
        public int appid { get; set; }
        public int ItemID { get; set; }
        [Required]
        [Display(Name = "FA Card No")]
        public string SubFACardNo { get; set; }
        public string SubPropertyNo { get; set; }
        public string SubDescription { get; set; }
        public string SubDescription2 { get; set; }
        public string SubPO { get; set; }
        public string SubSerialNo { get; set; }
        [Required]
        public int SubQty { get; set; }
        public string SubModelType { get; set; }
        public string SubUoM { get; set; }
        public string SubUnitCost { get; set; }
        public string SubShelfNo { get; set; }
        public string SubDateAdjusted { get; set; }
        public string SubToolStatus { get; set; }
        public string Notes { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime DateDelivered { get; set; }
        
    }
    public class CMEInput
    {
        public int id { get; set; }
        public int ItemID { get; set; }
        public string FACardNo { get; set; }
        public string PropertyNo { get; set; }
        public string Description { get; set; }
        public string Description2 { get; set; }
        public string PO { get; set; }
        public string SerialNo { get; set; }
        public int Qty { get; set; }
        public string Model { get; set; }
        public string UoM { get; set; }
        public int UnitCost { get; set; }
        public string ShelfNo { get; set; }
        public DateTime DateAdjusted { get; set; }
        public string ToolStatus { get; set; }
    }
    
}