using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ToolsManagementSystem.Models.View_Model
{
    public class ItemTrackingViewModel
    {
        public int ItemID { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Date { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateAdjusted { get; set; }
        public DateTime TransDate { get; set; }
        public string ItemCode { get; set; }
        public string RefNo { get; set; }
        public string EmployeeName { get; set; }
        public string Description { get; set; }
        public string SerialNo { get; set; }
        public string PO { get; set; }
        public string PropertyNo { get; set; }
        public string Location { get; set; }
        public string UoM { get; set; }
        public int Qty { get; set; }
        public int QtyAdj { get; set; }
        public int QtyLostUnreturned { get; set; }
        public decimal UnitCost { get; set; }
        public decimal Amount { get; set; }
        public string ToolStatus { get; set; }
        public string entrytype { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public string HeadStatus { get; set; }
        public int DocStatus { get; set; }
        public ItemTrackingViewModel() { }
        public ItemTrackingViewModel(ItemTrackingViewModel i)
        {
            TransDate = i.TransDate;
            ItemID = i.ItemID;
            RefNo = i.RefNo;
            EmployeeName = i.EmployeeName;
            Date = i.Date;
            DateAdjusted = i.DateAdjusted;
            ItemCode = i.ItemCode;
            Description = i.Description;
            SerialNo = i.SerialNo;
            PO = i.PO;
            PropertyNo = i.PropertyNo;
            Location = i.Location;
            UoM = i.UoM;
            Qty = i.Qty;
            QtyAdj = i.QtyAdj;
            QtyLostUnreturned = i.QtyLostUnreturned;
            UnitCost = i.UnitCost;
            Amount = i.Amount;
            ToolStatus = i.ToolStatus;
            entrytype = i.entrytype;
            Remarks = i.Remarks;
            Status = i.Status;
            DocStatus = i.DocStatus;

        }
    }
}