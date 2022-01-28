using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToolsManagementSystem.Models.View_Model
{
    public class AFEmployeeDetailViewModel
    {
        //AFEmployeeID,ItemID_R,SerialNo_R,PO,Quantity,UoM,UnitCost,Amount,DateIssued,TransType
        public int id { get; set; }
        public int AFEmployeeID { get; set; }
        public int ItemID { get; set; }
        public int ItemID_R { get; set; }
        public string SerialNo { get; set; }
        public string SerialNo_R { get; set; }
        public string PO { get; set; }
        public int Quantity { get; set; }
        public string UoM { get; set; }
        public decimal UnitCost { get; set; }
        public decimal Amount { get; set; }
        public string DateIssued { get; set; }
        public string DateReturned { get; set; }
        public string TransType { get; set; }
       
        public string Status { get; set; }
    }
}