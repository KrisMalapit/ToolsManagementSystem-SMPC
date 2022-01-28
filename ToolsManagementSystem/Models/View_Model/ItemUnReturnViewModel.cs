using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ToolsManagementSystem.Models.View_Model
{
    public class ItemUnReturnViewModel
    {
        public int id { get; set; }
        public int ItemID { get; set; }
        public string ItemCode{ get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DateIssued { get; set; }
        public string Description { get; set; }
        public string RefNo { get; set; }
        public string SerialNo { get; set; }
        public string EmpID { get; set; }
        public string EmployeeName { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DueDate { get; set; }
        public int QtyIssued { get; set; }
        public int QtyReturn { get; set; }
        public int QtyLostUnreturned { get; set; }
        public string Status { get; set; }
        public ItemUnReturnViewModel() { }
        public ItemUnReturnViewModel (ItemUnReturnViewModel i) {
            ItemID = i.ItemID;
            ItemCode = i.ItemCode;
            DateIssued = i.DateIssued;
            DueDate = i.DueDate;
            Description = i.Description;
            RefNo = i.RefNo;
            SerialNo = i.SerialNo;
            EmpID = i.EmpID;
            EmployeeName = i.EmployeeName;
            QtyIssued = i.QtyIssued;
            QtyReturn = i.QtyReturn;
            QtyLostUnreturned = i.QtyLostUnreturned;
        }
    }

    public class AccountabilityCardViewModel
    {
        public int ItemID { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string PropertyNo { get; set; }
        public string FACardNo { get; set; }
        public string FAARFNo { get; set; }
        public decimal UnitCost { get; set; }
        public string UOM { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? DateIssued { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? DateDelivered { get; set; }
        public string RefNo { get; set; }
        public string SerialNo { get; set; }
        public string EmpID { get; set; }
        public string EmployeeName { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string Approver { get; set; }
        public string ApproverPosition { get; set; }
        public string Approver2 { get; set; }
        public string Approver2Position { get; set; }

        public int QtyIssued { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? DateReturned { get; set; }
        public int QtyReturn { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public string Model { get; set; }
        public string ToolStatus { get; set; }
        public string Recommendation { get; set; }
        public string PO { get; set; }
        public string ShelfNo { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DueDate { get; set; }
        public string Description2 { get; set; } 
        public string Purpose { get; set; }
        public string WorkOrder { get; set; }
        public int StockQty { get; set; }
        public int DocStatus { get; set; }
        public DateTime? DateTransferred { get; set; }
        public int QtyTransferred { get; set; }
        public string FindingsObservation { get; set; }
        public int ReturnID { get; set; }
        public string GroupName { get; set; }
        public string TransType { get; set; }
        public string PreparedBy { get; set; }
        //public int IssueStatus { get; set; }
        //public int ReturnStatus { get; set; }
        public AccountabilityCardViewModel() { }
        public AccountabilityCardViewModel(AccountabilityCardViewModel i)
        {
            ItemID = i.ItemID;
            ItemCode = i.ItemCode;
            DateIssued = i.DateIssued;
            DateDelivered = i.DateDelivered;
            DateReturned = i.DateReturned;
            ItemDescription = i.ItemDescription;
            UnitCost = i.UnitCost;
            RefNo = i.RefNo;
            SerialNo = i.SerialNo;
            EmpID = i.EmpID;
            EmployeeName = i.EmployeeName;
            QtyIssued = i.QtyIssued;
            QtyReturn = i.QtyReturn;
            QtyTransferred = i.QtyTransferred;
            DocStatus = i.DocStatus;
            Status = i.Status;
            PropertyNo = i.PropertyNo;
            Remarks = i.Remarks;
            Model = i.Model;
            FACardNo = i.FACardNo;
            FAARFNo = i.FAARFNo;
            ToolStatus = i.ToolStatus;
            Recommendation = i.Recommendation;
            Approver = i.Approver;
            PO = i.PO;
            ShelfNo = i.ShelfNo;
            DateCreated = i.DateCreated;
            DueDate = i.DueDate;
            Description2 = i.Description2;
            Purpose = i.Purpose;
            WorkOrder = i.WorkOrder;
            StockQty = i.StockQty;
            DateTransferred = i.DateTransferred;
            FindingsObservation = i.FindingsObservation;

            Approver2 = i.Approver2;
            Approver2Position = i.Approver2Position;
            GroupName = i.GroupName;
            TransType = i.TransType;
            PreparedBy = i.PreparedBy;
            //IssueStatus = i.IssueStatus;
            //ReturnStatus = i.ReturnStatus;
        }
    }
    public class ReportViewModel
    {

        public int ItemID { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DateIssued { get; set; }
        public DateTime? Date { get; set; }
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public string Description2 { get; set; }
        public string PO { get; set; }
        public string SerialNo { get; set; }
        public int Qty { get; set; }
        public int QtyReturn { get; set; }
        public int QtyTransferred { get; set; }  
        public decimal UnitCost { get; set; }
        public string UOM { get; set; }
        public string RefNo { get; set; }
        public string EmpID { get; set; }
        public string EmployeeName { get; set; }
        public string EntryType { get; set; }
        public string Department { get; set; }
        public string Category { get; set; }
        public string Remarks { get; set; }
        public string PropertyNo { get; set; }
        public string Status { get; set; }
        public string Contractor { get; set; }
        public DateTime? DateReturned { get; set; }
        public string ShelfNo { get; set; }
        public string DeptCode { get; set; }
        public string WorkOrder { get; set; }
        public string AccountCode { get; set; }
        public string HeaderStatus { get; set; }
        public string Member { get; set; }
        public  ReportViewModel() { }
        public ReportViewModel(ReportViewModel i)
        {
            ItemID = i.ItemID;
            Date = i.Date;
            DateIssued = i.DateIssued;
            ItemCode = i.ItemCode;
            Description = i.Description;
            Description2 = i.Description2;
            PO = i.PO;
            SerialNo = i.SerialNo;
            Qty = i.Qty;
            QtyReturn = i.QtyReturn;
            QtyTransferred = i.QtyTransferred;
            UnitCost = i.UnitCost;
            UOM = i.UOM;
            RefNo = i.RefNo;
            EmpID = i.EmpID;
            EmployeeName = i.EmployeeName;
            Department = i.Department;
            PropertyNo = i.PropertyNo;
            Category = i.Category;
            Remarks = i.Remarks;
            Contractor = i.Contractor;
            Status = i.Status;
            AccountCode = AccountCode;
            DateReturned = (DateTime?)i.DateReturned;
            ShelfNo = i.ShelfNo;
            DeptCode = i.DeptCode;
            WorkOrder = i.WorkOrder;
            HeaderStatus = i.HeaderStatus;
            EntryType = i.EntryType;
            Member = i.Member;
        }
    }
    public class UserDeptViewModel
    {
        public int id { get; set; }
        public string Code { get; set; }
        public string DeptCode { get; set; }
        public string Name { get; set; }
        public string Approver { get; set; }
        public string ApproverPosition { get; set; }
        public int UserID { get; set; }

        public UserDeptViewModel() { }
        public UserDeptViewModel(UserDeptViewModel i) {
            id = i.id;
            Code = i.Code;
            DeptCode = i.DeptCode;
            Name = i.Name;
            Approver = i.Approver;
            ApproverPosition = i.ApproverPosition;
            UserID = i.UserID;
        }
    }
}