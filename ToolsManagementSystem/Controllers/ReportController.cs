using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using ToolsManagementSystem.DAL;
using ToolsManagementSystem.Models.View_Model;
using System.Linq.Dynamic;
//using System.Data.Objects.SqlClient;

namespace ToolsManagementSystem.Controllers
{
    [CustomAuthorize]
    public class ReportController : Controller
    {
        private ToolManagementContext db = new ToolManagementContext();
        //
        // GET: /Report/
        public ActionResult Index()
        {
            return View();
        }
        public DataTable PrintEAC(string EACNo)
        {

            var result = db.AFEmployeeIssues
                .GroupJoin(db.AFEmployeeReturns
                .Where(e => e.Status == "Active")
                .Where(e => e.DocStatus != 0)
                        , i => i.id
                        , r => r.AFEmployeeIssueID,
                        (i, r) => new
                        {
                            Issue = i,
                            Returns = r.DefaultIfEmpty()
                        })
                        .SelectMany(
                         a => a.Returns
                             .Select(b =>
                            new AccountabilityCardViewModel()
                            {
                                ItemID = a.Issue.ItemID,
                                ItemCode = a.Issue.Items.ItemCode,
                                ItemDescription = a.Issue.Items.Description + "," + a.Issue.Items.Description2,
                                UnitCost = a.Issue.UnitCost,
                                UOM = a.Issue.Items.UOM,
                                DateIssued = a.Issue.DateIssued,
                                RefNo = a.Issue.AFEmployees.EACNo,
                                SerialNo = a.Issue.SerialNo,
                                EmpID = a.Issue.AFEmployees.Employees.EmpId,
                                EmployeeName = a.Issue.AFEmployees.Employees.LastName + "," + a.Issue.AFEmployees.Employees.FirstName,
                                Designation = a.Issue.AFEmployees.Employees.Designations.Name,
                                Department = a.Issue.AFEmployees.Employees.Departments.Name,
                                Approver = a.Issue.AFEmployees.Employees.Departments.Approver,
                                ApproverPosition = a.Issue.AFEmployees.Employees.Departments.ApproverPosition,
                                QtyIssued = a.Issue.Quantity,
                                QtyReturn =
                                      (
                                     b.DocStatus == 0 ? 0 : (b.Quantity == null ? 0 : b.Quantity)
                                      )
                                    ,
                                QtyTransferred = a.Issue.QuantityTransferred,
                                DateReturned = b.DateReturned == null ? a.Issue.DateTransferred : b.DateReturned,
                                Status = a.Issue.Status,
                                PO = a.Issue.PO,
                                ShelfNo = a.Issue.Items.ShelfNo

                                ,
                                DateCreated = a.Issue.Created_At
                                ,
                                Description2 = a.Issue.Items.Description
                                ,
                                Remarks = a.Issue.Remarks
                                ,
                                Purpose = a.Issue.AFEmployees.Purpose

                                ,
                                StockQty = 0
                                ,ToolStatus = b.ToolStatus
                                //,GroupName = db.GroupAccountabilityMembers.Where(c=>c.EmployeeID == a.Issue.AFEmployees.EmployeeID)
                                //            .Select(c=>c.GroupAccountabilities.Code).FirstOrDefault()

                                ,
                                GroupName = a.Issue.AFEmployees.Employees.EntryType == "Individual" ? a.Issue.AFEmployees.Employees.LastName + "," + a.Issue.AFEmployees.Employees.FirstName : a.Issue.AFEmployees.Employees.FirstName
                                ,
                                TransType = a.Issue.AFEmployees.TransType
                                ,PreparedBy = a.Issue.AFEmployees.Users.Name
                                

                            }
                        ))
                        .Where(b => b.Status == "Active" || b.Status == "Transferred")
                        .Where(b => b.RefNo == EACNo);

            //var m = ToDataTable(result.ToList());
            var l = result.ToList();
            var m = ToDataTable(ListWithInventory(result));
            DataTable dt = m;
            return dt;
        }
        public DataTable PrintEBC(string EBCNo)
        {

            var result = db.AFBorrowerIssues
                .GroupJoin(db.AFBorrowerReturns
                .Where(e => e.Status == "Active")
                .Where(a => a.DocStatus != 0)
                        , i => i.id
                        , r => r.AFBorrowerIssueID,
                        (i, r) => new
                        {
                            Issue = i,
                            Returns = r.DefaultIfEmpty()
                        })
                        .SelectMany(
                         a => a.Returns
                             .Select(b =>
                            new AccountabilityCardViewModel()
                            {
                                ItemCode = a.Issue.Items.ItemCode,
                                ItemID = a.Issue.Items.id,
                                ItemDescription = a.Issue.Items.Description + "," + a.Issue.Items.Description2,
                                UnitCost = a.Issue.UnitCost,
                                UOM = a.Issue.Items.UOM,
                                DateIssued = a.Issue.DateIssued,
                                RefNo = a.Issue.AFBorrowers.EBCNo,
                                SerialNo = a.Issue.SerialNo,
                                EmpID = a.Issue.AFBorrowers.Employees.EmpId,
                                EmployeeName = a.Issue.AFBorrowers.Employees.LastName + "," + a.Issue.AFBorrowers.Employees.FirstName,
                                Designation = a.Issue.AFBorrowers.Employees.Designations.Name,
                                Department = a.Issue.AFBorrowers.Employees.Departments.Name,
                                Approver = a.Issue.AFBorrowers.Employees.Departments.Approver,
                                ApproverPosition = a.Issue.AFBorrowers.Employees.Departments.ApproverPosition,
                                QtyIssued = a.Issue.Quantity,
                                QtyReturn =
                                      (
                                     b.DocStatus == 0 ? 0 : (b.Quantity == null ? 0 : b.Quantity)
                                      )
                                    ,
                                QtyTransferred = a.Issue.QuantityTransferred,
                                DateReturned = b.DateReturned == null ? a.Issue.DateTransferred : b.DateReturned,
                                Status = a.Issue.Status,
                                ShelfNo = a.Issue.Items.ShelfNo,
                                PO = a.Issue.PO
                                ,
                                DateCreated = a.Issue.Created_At
                                ,
                                DueDate = a.Issue.DueDate
                                ,
                                Description2 = a.Issue.Items.Description
                                ,
                                Remarks = a.Issue.Remarks
                                ,
                                Purpose = a.Issue.AFBorrowers.Purpose
                                ,
                                WorkOrder = a.Issue.AFBorrowers.WorkOrder
                                ,
                                StockQty = 0
                                ,
                                DocStatus = b.DocStatus == null ? 0 : b.DocStatus
                                ,
                                ToolStatus = b.ToolStatus

                                //,GroupName = db.GroupAccountabilityMembers.Where(c => c.EmployeeID == a.Issue.AFBorrowers.EmployeeID)
                                //            .Select(c => c.GroupAccountabilities.Code).FirstOrDefault()
                                ,
                                GroupName = a.Issue.AFBorrowers.Employees.EntryType == "Individual" ? a.Issue.AFBorrowers.Employees.LastName + "," + a.Issue.AFBorrowers.Employees.FirstName : a.Issue.AFBorrowers.Employees.FirstName
                                ,
                                TransType = a.Issue.AFBorrowers.TransType
                                ,
                                PreparedBy = a.Issue.AFBorrowers.Users.Name
                            }
                        )).Where(b => b.Status == "Active" || b.Status == "Transferred")
                        .Where(b => b.RefNo == EBCNo);



            var xxx = result.ToList();


            var m = ToDataTable(ListWithInventory(result));
            DataTable dt = m;
            return dt;
        }

        static List<AccountabilityCardViewModel> ListWithInventory(IQueryable<AccountabilityCardViewModel> result)
        {
            List<AccountabilityCardViewModel> res = new List<AccountabilityCardViewModel>();

            foreach (var item in result)
            {
                res.Add(new AccountabilityCardViewModel
                {
                    ItemCode = item.ItemCode,
                    ItemID = item.ItemID,
                    ItemDescription = item.ItemDescription,
                    UnitCost = item.UnitCost,
                    UOM = item.UOM,
                    DateIssued = item.DateIssued,
                    RefNo = item.RefNo,
                    SerialNo = item.SerialNo,
                    EmpID = item.EmpID,
                    EmployeeName = item.EmployeeName,
                    Designation = item.Designation,
                    Department = item.Department,
                    Approver = item.Approver,
                    ApproverPosition = item.ApproverPosition,
                    QtyIssued = item.QtyIssued,
                    QtyReturn = item.QtyReturn,
                    DateReturned = item.DateReturned,
                    Status = item.Status,
                    ShelfNo = item.ShelfNo,
                    PO = item.PO,
                    DateCreated = item.DateCreated,
                    DueDate = item.DueDate,
                    Description2 = item.Description2,
                    Remarks = item.Remarks,
                    Purpose = item.Purpose,
                    WorkOrder = item.WorkOrder,
                    StockQty = new ItemController().TotalInv(item.ItemID),
                    QtyTransferred = item.QtyTransferred,
                    DateTransferred = item.DateTransferred,
                    ToolStatus = item.ToolStatus,
                    GroupName = item.GroupName,
                    TransType = item.TransType,
                    PreparedBy = item.PreparedBy
            });

            }

            return res;
        }


        public DataTable PrintFAAFNo_Issue(string FAAFNo)
        {

            var result = db.AFFAIssues.Select(a =>
                            new AccountabilityCardViewModel()
                            {
                                ItemCode = a.Items.ItemCode,
                                ItemDescription = a.Items.Description + "," + a.Items.Description2,
                                UnitCost = a.UnitCost,
                                UOM = a.Items.UOM,
                                DateIssued = a.DateIssued,
                                DateDelivered = db.ItemDetails
                                           .Where(c => c.Status == "Active")
                                           .Where(c => c.ItemID == a.ItemID)
                                           .Where(c => c.SerialNo == a.SerialNo)
                                           .Select(c => c.DateDelivered)
                                           .FirstOrDefault(),
                                RefNo = a.AFFAs.FAAFNo,
                                SerialNo = a.SerialNo,
                                EmpID = a.AFFAs.Employees.EmpId,
                                EmployeeName = a.AFFAs.Employees.FirstName + " " + a.AFFAs.Employees.LastName,
                                Designation = a.AFFAs.Employees.Designations.Name,
                                Department = a.AFFAs.Employees.Departments.Name,

                                Approver = a.AFFAs.Employees.Departments.Approver,
                                ApproverPosition = a.AFFAs.Employees.Departments.ApproverPosition,

                                Approver2 = a.AFFAs.Employees.Departments.Approver2,
                                Approver2Position = a.AFFAs.Employees.Departments.Approver2Position,

                                QtyIssued = a.Quantity,
                                PropertyNo = a.PropertyNo,
                                Remarks = a.Remarks,
                                Model = a.Items.ModelType,
                                Status = a.Status,
                                PO = a.PO
                                //                                ,
                                //GroupName = db.GroupAccountabilityMembers.Where(c => c.EmployeeID == a.AFFAs.EmployeeID)
                                //            .Select(c => c.GroupAccountabilities.Code).FirstOrDefault()
                                ,
                                GroupName = a.AFFAs.Employees.EntryType == "Individual" ? a.AFFAs.Employees.LastName + "," + a.AFFAs.Employees.FirstName : a.AFFAs.Employees.FirstName
                                ,
                                TransType = a.AFFAs.TransType
                                ,
                                PreparedBy = a.AFFAs.Users.Name
                            }
                        ).Where(b => b.Status == "Active" || b.Status == "Transferred")
                        .Where(b => b.RefNo == FAAFNo);
            var x = result.ToList();

            var m = ToDataTable(result.ToList());
            DataTable dt = m;
            return dt;
        }
        public DataTable PrintFAAFNo_Return(string FAAFNo)
        {

            var result = db.AFFAIssues.GroupJoin(db.AFFAReturns
                .Where(e => e.Status == "Active")
                .Where(a => a.DocStatus != 0)
                         , i => i.id
                         , r => r.AFFAIssueID,
                         (i, r) => new
                         {
                             Issue = i,
                             Returns = r.DefaultIfEmpty()
                         })
                         .SelectMany(
                          a => a.Returns
                              .Select(b =>
                             new AccountabilityCardViewModel()
                             {
                                 FACardNo = a.Issue.Items.ItemDetails.Where(x => x.SerialNo == a.Issue.SerialNo && x.PropertyNo == a.Issue.PropertyNo)
                                            .Select(x => x.FACardNo)
                                            .FirstOrDefault(),

                                 PropertyNo = a.Issue.PropertyNo,
                                 FAARFNo = b.FAARFNo,
                                 ToolStatus = b.ToolStatus,
                                 Recommendation = b.Recommendation,
                                 Remarks = b.Remarks,
                                 Model = b.AFFAIssues.Items.ModelType,
                                 ItemID = a.Issue.id,
                                 ItemDescription = a.Issue.Items.Description + "," + a.Issue.Items.Description2,
                                 UnitCost = a.Issue.UnitCost,
                                 UOM = a.Issue.Items.UOM,
                                 DateIssued = a.Issue.DateIssued,
                                 RefNo = a.Issue.AFFAs.FAAFNo,
                                 SerialNo = a.Issue.SerialNo,
                                 EmpID = a.Issue.AFFAs.Employees.EmpId,
                                 EmployeeName = a.Issue.AFFAs.Employees.LastName + "," + a.Issue.AFFAs.Employees.FirstName,
                                 Designation = a.Issue.AFFAs.Employees.Designations.Name,
                                 Department = a.Issue.AFFAs.Employees.Departments.Name,

                                 Approver = a.Issue.AFFAs.Employees.Departments.Approver,
                                 ApproverPosition = a.Issue.AFFAs.Employees.Departments.ApproverPosition,

                                 QtyIssued = a.Issue.Quantity,
                                 //QtyReturn = b.Quantity?null,
                                 QtyReturn =
                                       (
                                      b.DocStatus == 0 ? 0 : (b.Quantity == null ? 0 : b.Quantity)
                                       )
                                     ,
                                 //DateReturned = b.DateReturned,
                                 DateReturned = b.DateReturned == null ? a.Issue.DateTransferred : b.DateReturned,
                                 Status = a.Issue.Status
                                 ,
                                 DocStatus = b.DocStatus == null ? 0 : b.DocStatus
                                 ,FindingsObservation = b.FindingsObservation
                                 ,ShelfNo = a.Issue.Items.ShelfNo



                                 ,Approver2 = a.Issue.AFFAs.Employees.Departments.Approver2
                                 ,Approver2Position = a.Issue.AFFAs.Employees.Departments.Approver2Position
                                 //,
                                 //GroupName = db.GroupAccountabilityMembers.Where(c => c.EmployeeID == a.Issue.AFFAs.EmployeeID)
                                 //           .Select(c => c.GroupAccountabilities.Code).FirstOrDefault()
                                 ,
                                 GroupName = a.Issue.AFFAs.Employees.EntryType == "Individual" ? a.Issue.AFFAs.Employees.LastName + "," + a.Issue.AFFAs.Employees.FirstName : a.Issue.AFFAs.Employees.FirstName
                                ,
                                 TransType = a.Issue.AFFAs.TransType
                             }
                         ))
                         .Where(b => b.QtyReturn > 0)
                         .Where(b => b.Status == "Active" || b.Status == "Transferred")
                         .Where(b => b.RefNo == FAAFNo);

            var xx = result.ToList();
            var m = ToDataTable(result.ToList());
            DataTable dt = m;
            return dt;
        }
        public DataTable PrintInventory()
        {
            var v = db.Items.Where(i => i.Status == "Active" || i.Status == "InActive")
                .Select(a => new
                {
                    a.id,
                    a.ItemCode,
                    a.Description,
                    a.Description2,
                    StockQty = 0,
                    a.UOM,
                    UnitCost = a.ItemDetails.Select(b => b.UnitCost).FirstOrDefault(),
                    a.ShelfNo,
                    ToolStatus = a.ItemDetails.Select(b=>b.ToolStatus).FirstOrDefault(),
                    Remarks =  a.ItemDetails.Select(b => b.Remarks).FirstOrDefault(),
                    Status = a.Status == "Active" ? "No" : "Yes"
                });

            var result = v.ToList().Select(a => new
            {
                a.id,
                a.ItemCode,
                a.Description,
                a.Description2,
                StockQty = new ItemController().TotalInv(a.id),
                a.UOM,
                a.ShelfNo,
                a.ToolStatus,
                a.Remarks
            });

            var l = result.ToList();
            var m = ToDataTable(l);
            DataTable dt = m;
            return dt;
        }
        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
        public DataTable PrintMLE()
        {

            var groupName = db.Employees.Where(e => e.EntryType == "Group")
              .Where(e => e.Status == "Active")
              .Select(b => new
              {
                  id = b.id,
                  empid = b.EmpId,
                  groupname = b.FirstName,
              });
            List<GroupViewModel> lst = new List<GroupViewModel>();
            foreach (var item in groupName.ToList())
            {
                var groupList = db.GroupAccountabilityMembers
                    .Where(g => g.GroupAccountabilities.Status == "Active")
                    .Where(g => g.GroupAccountabilities.Code == item.empid);

                var groupLists = groupList.ToList();
                if (groupList.Count() > 0)
                {
                    string members = "";
                    foreach (var item2 in groupLists)
                    {
                        members = members + item2.Employees.LastName + ", " + item2.Employees.FirstName;

                    }
                    var x = new GroupViewModel()
                    {
                        id = item.id,
                        empid = item.empid,
                        groupname = item.groupname,
                        employeename = members
                    };
                    lst.Add(x);
                }
            }

            var qryGroupMembers = lst.AsEnumerable().Select(l => new
            {
                EmployeeCode = l.empid,
                GroupName = l.groupname,
                MemberName = l.employeename
            });




            var result = db.AFEmployeeIssues
                .Where(b => b.Status == "Active" || b.Status == "Transferred")
                .Where(a => a.AFEmployees.DocStatus != 0)
                .AsEnumerable()
                .Select(
                         a =>
                            new ReportViewModel()
                            {
                                ItemID = a.id,
                                DateIssued = a.DateIssued,
                                ItemCode = a.Items.ItemCode,
                                Description = a.Items.Description,
                                Description2 = a.Items.Description2,
                                PO = a.PO,
                                SerialNo = a.SerialNo,
                                Qty = a.Quantity,
                                QtyReturn = db.AFEmployeeReturns
                                           .Where(c => c.DocStatus == 1)
                                           .Where(c => c.Status == "Active")
                                           .Where(c => c.AFEmployeeIssueID == a.id)
                                           .Select(c => c.Quantity)
                                           .DefaultIfEmpty(0)
                                           .Sum(),
                                QtyTransferred = a.QuantityTransferred,
                                UnitCost = a.UnitCost,
                                UOM = a.UoM,
                                RefNo = a.AFEmployees.EACNo,
                                EmpID = a.AFEmployees.Employees.EmpId,
                                EmployeeName = a.AFEmployees.Employees.EntryType == "Individual" ? a.AFEmployees.Employees.LastName + "," + a.AFEmployees.Employees.FirstName : a.AFEmployees.Employees.FirstName,
                                Member = a.AFEmployees.Employees.EntryType == "Individual" ? "" : (qryGroupMembers.Where(b => b.EmployeeCode == a.AFEmployees.Employees.EmpId).Select(b => b.MemberName).FirstOrDefault()),
                                Department = a.AFEmployees.Employees.Departments.DeptCode,
                                PropertyNo = "",
                                Category = a.Items.Category,
                                Remarks = a.Remarks,

                                DateReturned = db.AFEmployeeReturns
                                               .Where(c => c.DocStatus == 1)
                                               .Where(b => b.Status == "Active")
                                               .Where(b => b.AFEmployeeIssueID == a.id)
                                               .Max(b => (DateTime?)b.DateReturned),

                                ShelfNo = a.Items.ShelfNo,
                                DeptCode = a.AFEmployees.Employees.Departments.Name,
                                AccountCode = a.AccountCode,

                                Status = a.Status
                            }
                        )
                        .Concat(db.AFFAIssues
                        .Where(b => b.AFFAs.DocStatus != 0)
                        .Where(b => b.Status == "Active" || b.Status == "Transferred")
                        .AsEnumerable()
                        .Select(
                         a =>
                            new ReportViewModel()
                            {
                                ItemID = a.id,
                                DateIssued = a.DateIssued,
                                ItemCode = a.Items.ItemCode,
                                Description = a.Items.Description,
                                Description2 = a.Items.Description2,
                                PO = a.PO,
                                SerialNo = a.SerialNo,
                                Qty = a.Quantity,
                                QtyReturn = db.AFFAReturns
                                           .Where(c => c.DocStatus == 1)
                                           .Where(c => c.Status == "Active")
                                           .Where(c => c.AFFAIssueID == a.id)
                                           .Select(c => c.Quantity)
                                           .DefaultIfEmpty(0)
                                           .Sum(),
                                QtyTransferred = a.QuantityTransferred,
                                UnitCost = a.UnitCost,
                                UOM = a.UoM,
                                RefNo = a.AFFAs.FAAFNo,
                                EmpID = a.AFFAs.Employees.EmpId,
                                EmployeeName = a.AFFAs.Employees.EntryType == "Individual" ? a.AFFAs.Employees.LastName + "," + a.AFFAs.Employees.FirstName : a.AFFAs.Employees.FirstName,
                                Member = a.AFFAs.Employees.EntryType == "Individual" ? "" : (qryGroupMembers.Where(b => b.EmployeeCode == a.AFFAs.Employees.EmpId).Select(b => b.MemberName).FirstOrDefault()),
                                Department = a.AFFAs.Employees.Departments.DeptCode,
                                PropertyNo = a.PropertyNo + "/" + a.FACardNo,
                                Category = a.Items.Category,
                                Remarks = a.Remarks,

                                DateReturned = db.AFFAReturns
                                               .Where(c => c.DocStatus == 1)
                                               .Where(b => b.Status == "Active")
                                               .Where(b => b.AFFAIssueID == a.id)
                                               .Max(b => (DateTime?)b.DateReturned), 

                                ShelfNo = a.Items.ShelfNo,
                                DeptCode = a.AFFAs.Employees.Departments.Name,
                                AccountCode = a.AccountCode,
                                Status = a.Status
                            }
                        ));

            var m = ToDataTable(result.ToList());
            DataTable dt = m;
            return dt;
        }

        public DataTable PrintES()
        {

            var result = db.AFEmployeeIssues
                .Where(a => a.AFEmployees.DocStatus != 0)
                .Where(b => b.Status == "Active" || b.Status == "Transferred")
                .Select(
                         a =>
                            new ReportViewModel()
                            {
                                ItemID = a.id,
                                DateIssued = a.DateIssued,
                                ItemCode = a.Items.ItemCode,
                                Description = a.Items.Description,
                                Description2 = a.Items.Description2,
                                PO = a.PO,
                                SerialNo = a.SerialNo,
                                Qty = a.Quantity,
                                QtyReturn = db.AFEmployeeReturns
                                           .Where(c => c.DocStatus == 1)
                                           .Where(c => c.Status == "Active")
                                           .Where(c => c.AFEmployeeIssueID == a.id)
                                           .Select(c => c.Quantity)
                                           .DefaultIfEmpty(0)
                                           .Sum(),
                                UnitCost = a.UnitCost,
                                UOM = a.UoM,
                                RefNo = a.AFEmployees.EACNo,
                                EmpID = a.AFEmployees.Employees.EmpId
                                //,
                                //EmployeeName = a.AFEmployees.Employees.LastName + "," + a.AFEmployees.Employees.FirstName
                                ,
                                EmployeeName = a.AFEmployees.Employees.EntryType == "Individual" ? a.AFEmployees.Employees.LastName + "," + a.AFEmployees.Employees.FirstName : a.AFEmployees.Employees.FirstName
                                ,
                                Department = a.AFEmployees.Employees.Departments.Name,
                                PropertyNo = "",
                                Category = a.Items.Category,
                                Remarks = a.Remarks,
                                Contractor = "",
                                DateReturned = db.AFEmployeeReturns
                                               .Where(c => c.DocStatus == 1)
                                               .Where(b => b.Status == "Active")
                                               .Where(b => b.AFEmployeeIssueID == a.id)
                                               .Max(b => b.DateReturned),

                                ShelfNo = a.Items.ShelfNo,
                                DeptCode = a.AFEmployees.Employees.Departments.Name,

                                Status = a.Status
                            }
                        )

                        .Concat(db.AFBorrowerIssues
                        .Where(a => a.AFBorrowers.DocStatus != 0)
                        .Where(b => b.Status == "Active" || b.Status == "Transferred")
                        .Select(
                         a =>
                            new ReportViewModel()
                            {
                                ItemID = a.id,
                                DateIssued = a.DateIssued,
                                ItemCode = a.Items.ItemCode,
                                Description = a.Items.Description,
                                Description2 = a.Items.Description2,
                                PO = a.PO,
                                SerialNo = a.SerialNo,
                                Qty = a.Quantity,
                                QtyReturn = db.AFBorrowerReturns
                                           .Where(c => c.DocStatus == 1)
                                           .Where(c => c.Status == "Active")
                                           .Where(c => c.AFBorrowerIssueID == a.id)
                                           .Select(c => c.Quantity)
                                           .DefaultIfEmpty(0)
                                           .Sum(),
                                UnitCost = a.UnitCost,
                                UOM = a.UoM,
                                RefNo = a.AFBorrowers.EBCNo,
                                EmpID = a.AFBorrowers.Employees.EmpId,
                                //EmployeeName = a.AFBorrowers.Employees.LastName + "," + a.AFBorrowers.Employees.FirstName,
                                EmployeeName = a.AFBorrowers.Employees.EntryType == "Individual" ? a.AFBorrowers.Employees.LastName + "," + a.AFBorrowers.Employees.FirstName : a.AFBorrowers.Employees.FirstName
                                ,Department = a.AFBorrowers.Employees.Departments.Name,
                                PropertyNo = "",
                                Category = a.Items.Category,
                                Remarks = a.Remarks,
                                Contractor = a.AFBorrowers.Contractors.Name,
                                DateReturned = db.AFBorrowerReturns
                                               .Where(c => c.DocStatus == 1)
                                               .Where(b => b.Status == "Active")
                                               .Where(b => b.AFBorrowerIssueID == a.id)
                                               .Max(b => b.DateReturned),

                                ShelfNo = a.Items.ShelfNo,
                                DeptCode = "",

                                Status = a.Status



                            }
                        ))

                        .Concat(db.AFFAIssues
                        .Where(a => a.AFFAs.DocStatus != 0)
                        .Where(b => b.Status == "Active" || b.Status == "Transferred")
                        .Select(
                         a =>
                            new ReportViewModel()
                            {
                                ItemID = a.id,
                                DateIssued = a.DateIssued,
                                ItemCode = a.Items.ItemCode,
                                Description = a.Items.Description,
                                Description2 = a.Items.Description2,
                                PO = a.PO,
                                SerialNo = a.SerialNo,
                                Qty = a.Quantity,
                                QtyReturn = db.AFFAReturns
                                           .Where(c => c.DocStatus == 1)
                                           .Where(c => c.Status == "Active")
                                           .Where(c => c.AFFAIssueID == a.id)
                                           .Select(c => c.Quantity)
                                           .DefaultIfEmpty(0)
                                           .Sum(),
                                UnitCost = a.UnitCost,
                                UOM = a.UoM,
                                RefNo = a.AFFAs.FAAFNo,
                                EmpID = a.AFFAs.Employees.EmpId,
                                //EmployeeName = a.AFFAs.Employees.LastName + "," + a.AFFAs.Employees.FirstName,
                                EmployeeName = a.AFFAs.Employees.EntryType == "Individual" ? a.AFFAs.Employees.LastName + "," + a.AFFAs.Employees.FirstName : a.AFFAs.Employees.FirstName,
                                Department = a.AFFAs.Employees.Departments.Name,
                                PropertyNo = a.PropertyNo + "/" + a.FACardNo,
                                Category = a.Items.Category,
                                Remarks = a.Remarks,
                                Contractor = "",
                                DateReturned = db.AFFAReturns
                                               .Where(c => c.DocStatus == 1)
                                               .Where(b => b.Status == "Active")
                                               .Where(b => b.AFFAIssueID == a.id)
                                               .Max(b => b.DateReturned),

                                ShelfNo = a.Items.ShelfNo,
                                DeptCode = a.AFFAs.Employees.Departments.Name,

                                Status = a.Status
                            }
                        ));




            var qry = db.GroupAccountabilities
                     .GroupJoin(db.GroupAccountabilityMembers
                     , foo => foo.id
                     , bar => bar.GroupAccountabilityID,
                     (h, d) => new
                     {
                         GroupName = h,
                         Members = d
                     })
                     .SelectMany(
                         x => x.Members.DefaultIfEmpty(),
                         (x, y) => new { 
                         Header=x.GroupName
                         , Detail=y
                         
                         }
                     );



            var r = qry.ToList();



            var m = ToDataTable(result.ToList());

            DataTable dt = m;
            return dt;
        }
        public DataTable FilterPrintES(FormCollection form)
        {
            int aa = 0;
            int bb = 0;
            int cnt = form.AllKeys.Count() / 2;

            string[] col = new string[cnt];
            string[] colval = new string[cnt];



            foreach (var key in form.AllKeys)
            {
                if (key.StartsWith("keyfld"))
                {
                    col[aa] = form[key];
                    aa++;
                }
                if (key.StartsWith("keyval"))
                {
                    colval[bb] = form[key];
                    bb++;
                }
            }




            var result = db.AFEmployeeIssues
                .Where(a => a.AFEmployees.DocStatus != 0)
                .Select(
                         a =>
                            new ReportViewModel()
                            {
                                ItemID = a.id,
                                DateIssued = a.DateIssued,
                                ItemCode = a.Items.ItemCode,
                                Description = a.Items.Description,
                                Description2 = a.Items.Description2,
                                PO = a.PO,
                                SerialNo = a.SerialNo,
                                Qty = a.Quantity,

                                QtyReturn = db.AFEmployeeReturns
                                           .Where(c => c.DocStatus == 1)
                                           .Where(c => c.Status == "Active")
                                           .Where(c => c.AFEmployeeIssueID == a.id)
                                           .Select(c => c.Quantity)
                                           .DefaultIfEmpty(0)
                                           .Sum(),

                                UnitCost = a.UnitCost,
                                UOM = a.UoM,
                                RefNo = a.AFEmployees.EACNo,
                                EmpID = a.AFEmployees.Employees.EmpId,
                                EmployeeName = a.AFEmployees.Employees.LastName + "," + a.AFEmployees.Employees.FirstName,
                                Department = a.AFEmployees.Employees.Departments.Name,
                                PropertyNo = "",
                                Category = a.Items.Category,
                                Remarks = a.Remarks,
                                Contractor = "",
                                WorkOrder = "",

                                DateReturned = db.AFEmployeeReturns
                                               .Where(c => c.DocStatus == 1)
                                               .Where(b => b.Status == "Active")
                                               .Where(b => b.AFEmployeeIssueID == a.id)
                                               .Max(b => b.DateReturned),

                                ShelfNo = a.Items.ShelfNo,
                                DeptCode = a.AFEmployees.Employees.Departments.Name,

                                Status = a.Status
                            }
                        ).Where(b => b.Status == "Active" || b.Status == "Transferred")

                        .Concat(db.AFBorrowerIssues.Select(
                         a =>
                            new ReportViewModel()
                            {
                                ItemID = a.id,
                                DateIssued = a.DateIssued,
                                ItemCode = a.Items.ItemCode,
                                Description = a.Items.Description,
                                Description2 = a.Items.Description2,
                                PO = a.PO,
                                SerialNo = a.SerialNo,
                                Qty = a.Quantity,
                                QtyReturn = db.AFBorrowerReturns
                                           .Where(c => c.DocStatus == 1)
                                           .Where(c => c.Status == "Active")
                                           .Where(c => c.AFBorrowerIssueID == a.id)
                                           .Select(c => c.Quantity)
                                           .DefaultIfEmpty(0)
                                           .Sum(),
                                UnitCost = a.UnitCost,
                                UOM = a.UoM,
                                RefNo = a.AFBorrowers.EBCNo,
                                EmpID = a.AFBorrowers.Employees.EmpId,
                                EmployeeName = a.AFBorrowers.Employees.LastName + "," + a.AFBorrowers.Employees.FirstName,
                                Department = a.AFBorrowers.Employees.Departments.Name,
                                PropertyNo = "",
                                Category = a.Items.Category,
                                Remarks = a.Remarks,
                                Contractor = a.AFBorrowers.Contractors.Name,
                                WorkOrder = a.WorkOrder,
                                DateReturned = db.AFBorrowerReturns
                                                .Where(c => c.DocStatus == 1)
                                               .Where(b => b.Status == "Active")
                                               .Where(b => b.AFBorrowerIssueID == a.id)
                                               .Max(b => b.DateReturned),

                                ShelfNo = a.Items.ShelfNo,
                                DeptCode = "",

                                Status = a.Status



                            }
                        ).Where(b => b.Status == "Active" || b.Status == "Transferred"))

                        .Concat(db.AFFAIssues.Select(
                         a =>
                            new ReportViewModel()
                            {
                                ItemID = a.id,
                                DateIssued = a.DateIssued,
                                ItemCode = a.Items.ItemCode,
                                Description = a.Items.Description,
                                Description2 = a.Items.Description2,
                                PO = a.PO,
                                SerialNo = a.SerialNo,
                                Qty = a.Quantity,
                                QtyReturn = db.AFFAReturns
                                           .Where(c => c.DocStatus == 1)
                                           .Where(c => c.Status == "Active")
                                           .Where(c => c.AFFAIssueID == a.id)
                                           .Select(c => c.Quantity)
                                           .DefaultIfEmpty(0)
                                           .Sum(),
                                UnitCost = a.UnitCost,
                                UOM = a.UoM,
                                RefNo = a.AFFAs.FAAFNo,
                                EmpID = a.AFFAs.Employees.EmpId,
                                EmployeeName = a.AFFAs.Employees.LastName + "," + a.AFFAs.Employees.FirstName,
                                Department = a.AFFAs.Employees.Departments.Name,
                                PropertyNo = a.PropertyNo + "/" + a.FACardNo,
                                Category = a.Items.Category,
                                Remarks = a.Remarks,
                                Contractor = "",
                                WorkOrder = "",
                                DateReturned = db.AFFAReturns
                                                .Where(c => c.DocStatus == 1)
                                               .Where(b => b.Status == "Active")
                                               .Where(b => b.AFFAIssueID == a.id)
                                               .Max(b => b.DateReturned),

                                ShelfNo = a.Items.ShelfNo,
                                DeptCode = a.AFFAs.Employees.Departments.Name,

                                Status = a.Status
                            }
                        ).Where(b => b.Status == "Active" || b.Status == "Transferred")).ToList();


            var o = result.ToList();




            for (int j = 0; j < cnt; j++)
            {
                switch (col[j])
                {
                    case "EmployeeName":
                        result = result.Where(p => (p.EmployeeName.ToUpper() ?? "").Contains(colval[j].ToUpper())).ToList();
                        break;
                    //case "DateIssued":
                    //    result = result.Where(p => (p.DateIssued.ToUpper() ?? "").Contains(colval[j].ToUpper())).ToList();
                    //    break;
                    case "Description":
                        result = result.Where(p => (p.Description.ToUpper() ?? "").Contains(colval[j].ToUpper())).ToList();
                        break;
                    case "Description2":
                        result = result.Where(p => (p.Description2.ToUpper() ?? "").Contains(colval[j].ToUpper())).ToList();
                        break;
                    case "ItemCode":
                        result = result.Where(p => (p.ItemCode.ToUpper() ?? "").Contains(colval[j].ToUpper())).ToList();
                        break;
                    case "PONo":
                        result = result.Where(p => (p.PO.ToUpper() ?? "").Contains(colval[j].ToUpper())).ToList();
                        break;
                    case "SerialNo":
                        result = result.Where(p => (p.SerialNo.ToUpper() ?? "").Contains(colval[j].ToUpper())).ToList();
                        break;
                    case "Remarks":
                        result = result.Where(p => p.Remarks != null && p.Remarks.ToUpper().Contains(colval[j].ToUpper())).ToList();
                        //result = result.Where(p => (p.Remarks.ToUpper() ?? "").Contains(colval[j].ToUpper())).ToList();
                        break;
                    case "PropertyNo":
                        result = result.Where(p => (p.PropertyNo.ToUpper() ?? "").Contains(colval[j].ToUpper())).ToList();
                        break;
                    case "ShelfNo":
                        result = result.Where(p => (p.ShelfNo.ToUpper() ?? "").Contains(colval[j].ToUpper())).ToList();
                        break;
                    case "Contractor":
                        result = result.Where(p => (p.Contractor.ToUpper() ?? "").Contains(colval[j].ToUpper())).ToList();
                        break;
                    case "WorkOrder":
                        result = result.Where(p => (p.WorkOrder.ToUpper() ?? "").Contains(colval[j].ToUpper())).ToList();
                        break;
                    case "Category":
                        result = result.Where(p => (p.Category.ToUpper() ?? "").Contains(colval[j].ToUpper())).ToList();
                        break;
                    //case "DateReturned":
                    //    result = result.Where(p => (p.DateReturned.ToUpper() ?? "").Contains(colval[j].ToUpper())).ToList();
                    //    break;
                    case "Department":
                        result = result.Where(p => (p.Department.ToUpper() ?? "").Contains(colval[j].ToUpper())).ToList();
                        break;


                }

            }

            var m = ToDataTable(result.ToList());

            DataTable dt = m;
            return dt;
        }
        public DataTable PrintBDM()
        {

            var groupName = db.Employees.Where(e => e.EntryType == "Group")
              .Where(e => e.Status == "Active")
              .Select(b => new
              {
                  id = b.id,
                  empid = b.EmpId,
                  groupname = b.FirstName,
              });
            List<GroupViewModel> lst = new List<GroupViewModel>();
            foreach (var item in groupName.ToList())
            {
                var groupList = db.GroupAccountabilityMembers
                    .Where(g => g.GroupAccountabilities.Status == "Active")
                    .Where(g => g.GroupAccountabilities.Code == item.empid);

                var groupLists = groupList.ToList();
                if (groupList.Count() > 0)
                {
                    string members = "";
                    foreach (var item2 in groupLists)
                    {
                        members = members + item2.Employees.LastName + ", " + item2.Employees.FirstName;

                    }
                    var x = new GroupViewModel()
                    {
                        id = item.id,
                        empid = item.empid,
                        groupname = item.groupname,
                        employeename = members
                    };
                    lst.Add(x);
                }
            }

            var qryGroupMembers = lst.AsEnumerable().Select(l => new
            {
                EmployeeCode = l.empid,
                GroupName = l.groupname,
                MemberName = l.employeename
            });



            var result = db.AFBorrowerIssues
                .Where(a => a.AFBorrowers.DocStatus != 0)
                .AsEnumerable()
                .Select(
                         a =>
                            new ReportViewModel()
                            {
                                ItemID = a.id,
                                DateIssued = a.DateIssued,
                                ItemCode = a.Items.ItemCode,
                                Description = a.Items.Description,
                                Description2 = a.Items.Description2,
                                PO = a.PO,
                                SerialNo = a.SerialNo,
                                Qty = a.Quantity,
                                UnitCost = a.UnitCost,
                                UOM = a.UoM,
                                RefNo = a.AFBorrowers.EBCNo,
                                EmpID = a.AFBorrowers.Employees.EmpId,
                                EmployeeName = a.AFBorrowers.Employees.EntryType == "Individual" ? a.AFBorrowers.Employees.LastName + "," + a.AFBorrowers.Employees.FirstName : a.AFBorrowers.Employees.FirstName,
                                Member = a.AFBorrowers.Employees.EntryType == "Individual" ? "" : (qryGroupMembers.Where(b => b.EmployeeCode == a.AFBorrowers.Employees.EmpId).Select(b => b.MemberName).FirstOrDefault()),
                                Department = a.AFBorrowers.Employees.Departments.DeptCode,
                                PropertyNo = "",
                                Category = a.Items.Category,
                                Remarks = a.Remarks,
                                Contractor = a.AFBorrowers.Contractors.Name,
                                DateReturned =  db.AFBorrowerReturns
                                               .Where(b => b.DocStatus != 0)
                                               .Where(b => b.Status == "Active")
                                               .Where(b => b.AFBorrowerIssueID == a.id)
                                               .Max(b => (DateTime?)b.DateReturned), 
                                Status = a.Status,
                                WorkOrder = a.AFBorrowers.WorkOrder,
                                QtyReturn = db.AFBorrowerReturns
                                           .Where(b => b.DocStatus != 0)
                                           .Where(c => c.Status == "Active")
                                           .Where(c => c.AFBorrowerIssueID == a.id)
                                           .Select(c => c.Quantity)
                                           .DefaultIfEmpty(0)
                                           .Sum(),
                                QtyTransferred = a.QuantityTransferred

                            }
                        ).Where(a => a.Status == "Active" || a.Status == "Transferred");


            var m = ToDataTable(result.ToList());
            DataTable dt = m;
            return dt;
        }
        public DataTable StockPosition(DateTime? AsOf, DateTime? FromDate, DateTime? ToDate)
        {
            var result = db.Items
            .Where(a => a.Status == "Active")
            .ToList()
            .Select(a => new ItemStockViewModel()
            {
                ItemCode = a.ItemCode
                ,
                Description = a.Description
                ,
                Description2 = a.Description2
                ,
                UOM = a.UOM
                ,
                ShelfNo = a.ShelfNo
                ,
                Available = QtyResult(a.id, "0", AsOf, FromDate, ToDate)
                ,
                Damaged = QtyResult(a.id, "1", AsOf, FromDate, ToDate)
                ,
                UnReturned = QtyResult(a.id, "2", AsOf, FromDate, ToDate)
                ,
                Total = 0
            }).ToList();


            var m = ToDataTable(result);
            DataTable dt = m;
            return dt;
        }
        public int QtyResult(int id, string type, DateTime? AsOf, DateTime? FromDate, DateTime? ToDate)
        {
            int res = 0;
            var items = new ItemController().ViewItemTracking(id, AsOf, FromDate, ToDate);


            var entryList = new string[] { "Positive Adjustment", "Negative Adjustment" };
            DateTime endDate = DateTime.Now.Date;
            if (AsOf != null)
            {
                endDate = DateTime.Parse(AsOf.ToString());
            }
            else
            {
                endDate = DateTime.Parse(ToDate.ToString());
            }


            int warehouseInv = items
                    .Where(i => entryList.Contains(i.entrytype))
                    .Where("TransDate <= @0", endDate).ToList()
                    .Select(a => a.Qty)
                    .DefaultIfEmpty(0)
                    .Sum();
            int warehouseDamaged = items
                     .Where(a => a.entrytype == "Negative Adjustment")
                     .Where("TransDate <= @0", endDate).ToList()
                     .Select(a => a.Qty * (-1))
                     .DefaultIfEmpty(0)
                     .Sum();


            if (FromDate != null && ToDate != null)
            {
                var d1 = DateTime.Parse(FromDate.ToString());
                var d2 = DateTime.Parse(ToDate.ToString());
                items = items.Where("TransDate >= @0 AND TransDate <= @1", d1, d2).ToList();

            }

            //var m = items.ToList();


            var model = new
            {
                SumQty = items
                       .Where(i => !entryList.Contains(i.entrytype))
                       .Select(a => a.Qty)
                       .DefaultIfEmpty(0)
                       .Sum(),
                SumQtyAdj = items
                        .Where(i => !entryList.Contains(i.entrytype))
                        .Where(i => i.ToolStatus != "Lost_Unreturned")
                        .Select(a => a.QtyAdj)
                        .DefaultIfEmpty(0)
                        .Sum(),
                SumQtyUnreturn = items
                        .Where(i => !entryList.Contains(i.entrytype))
                        .Select(a => a.Qty)
                        .DefaultIfEmpty(0)
                        .Sum(),
                AccountabilityDamaged = items
                        .Where(i => !entryList.Contains(i.entrytype))
                        .Where(a => a.entrytype == "Un-Returned" || a.entrytype == "Damaged")
                        .Select(a => a.QtyAdj)
                        .DefaultIfEmpty(0)
                        .Sum(),



            };
            switch (type)
            {
                case "0":
                    res = warehouseInv + model.SumQty;
                    break;
                case "1":
                    res = (model.SumQtyAdj + warehouseDamaged);
                    break;
                case "2":
                    res = Math.Abs(model.SumQtyUnreturn) - model.AccountabilityDamaged;
                    break;
            }




            return res;
        }


        public DataTable PrintUnreturnItems(int? id)
        {

            var model = 

            //db.AFEmployeeIssues
            //    .Where(b => b.Status == "Active" || b.Status == "Transferred")
            //    .Where(a => a.AFEmployees.DocStatus != 0)
            //    .Select(b => new ItemUnReturnViewModel()
            //{
            //    ItemID = b.ItemID,
            //    ItemCode = b.Items.ItemCode,
            //    Description = b.Items.Description + " " + b.Items.Description2,
            //    DateIssued = b.DateIssued,
            //    RefNo = b.AFEmployees.EACNo,
            //    SerialNo = b.SerialNo,
            //    EmpID = b.AFEmployees.Employees.EmpId,
            //    EmployeeName = b.AFEmployees.Employees.LastName + "," + b.AFEmployees.Employees.FirstName,
            //    QtyIssued = (
            //               b.Status == "Active" ? b.Quantity : b.QuantityAdj
            //         ),
            //    QtyReturn = db.AFEmployeeReturns
            //                .Where(a => a.DocStatus == 1)
            //               .Where(a => a.Status == "Active")
            //               .Where(a => a.ToolStatus != "Lost_Unreturned")
            //               .Where(a => a.AFEmployeeIssueID == b.id)
            //               .Select(a => a.Quantity)
            //               .DefaultIfEmpty(0)
            //               .Sum(),
            //    QtyLostUnreturned = db.AFEmployeeReturns
            //              .Where(a => a.DocStatus == 1)
            //              .Where(a => a.Status == "Active")
            //              .Where(a => a.ToolStatus == "Lost_Unreturned")
            //              .Where(a => a.AFEmployeeIssueID == b.id)
            //              .Select(a => a.Quantity)
            //              .DefaultIfEmpty(0)
            //              .Sum(),
            //    Status = b.Status,
            //})

            ////AFBorrower
            //.Concat(
            //db.AFBorrowerIssues
            //.Where(b => b.Status == "Active" || b.Status == "Transferred")
            //.Where(b => b.AFBorrowers.DocStatus != 0)
            //.Select(b => new ItemUnReturnViewModel()
            //{
            //    ItemID = b.ItemID,
            //    ItemCode = b.Items.ItemCode,
            //    Description = b.Items.Description + " " + b.Items.Description2,
            //    DateIssued = b.DateIssued,
            //    RefNo = b.AFBorrowers.EBCNo,
            //    SerialNo = b.SerialNo,
            //    EmpID = b.AFBorrowers.Employees.EmpId,
            //    EmployeeName = b.AFBorrowers.Employees.LastName + "," + b.AFBorrowers.Employees.FirstName,
            //    QtyIssued = (
            //               b.Status == "Active" ? b.Quantity : b.QuantityAdj
            //         ),
            //    QtyReturn = db.AFBorrowerReturns
            //                .Where(a => a.DocStatus == 1)
            //               .Where(a => a.Status == "Active")
            //               .Where(a => a.ToolStatus != "Lost_Unreturned")
            //               .Where(a => a.AFBorrowerIssueID == b.id)
            //               .Select(a => a.Quantity)
            //               .DefaultIfEmpty(0)
            //               .Sum(),
            //    QtyLostUnreturned = db.AFBorrowerReturns
            //                .Where(a => a.DocStatus == 1)
            //                .Where(a => a.Status == "Active")
            //                .Where(a => a.ToolStatus == "Lost_Unreturned")
            //                .Where(a => a.AFBorrowerIssueID == b.id)
            //                .Select(a => a.Quantity)
            //                .DefaultIfEmpty(0)
            //                .Sum(),
            //    Status = b.Status,
            //})
            //)

            ////AFFA
            // .Concat(
             db.AFFAIssues
             .Where(b => b.Status == "Active" || b.Status == "Transferred")
             .Where(b => b.AFFAs.DocStatus != 0)
             .Select(b => new ItemUnReturnViewModel()
             {
                 ItemID = b.ItemID,
                 ItemCode = b.Items.ItemCode,
                 Description = b.Items.Description + " " + b.Items.Description2,
                 DateIssued = b.DateIssued,
                 RefNo = b.AFFAs.FAAFNo,
                 SerialNo = b.SerialNo,
                 EmpID = b.AFFAs.Employees.EmpId,
                 EmployeeName = b.AFFAs.Employees.LastName + "," + b.AFFAs.Employees.FirstName,
                 QtyIssued = (
                            b.Status == "Active" ? b.Quantity : b.QuantityAdj
                      ),
                 QtyReturn = db.AFFAReturns
                            .Where(a => a.DocStatus == 1)
                            .Where(a => a.Status == "Active")
                            .Where(a => a.ToolStatus != "Lost_Unreturned")
                            .Where(a => a.AFFAIssueID == b.id)
                            .Select(a => a.Quantity)
                            .DefaultIfEmpty(0)
                            .Sum(),
                 QtyLostUnreturned = db.AFFAReturns
                             .Where(a => a.DocStatus == 1)
                             .Where(a => a.Status == "Active")
                             .Where(a => a.ToolStatus == "Lost_Unreturned")
                             .Where(a => a.AFFAIssueID == b.id)
                             .Select(a => a.Quantity)
                             .DefaultIfEmpty(0)
                             .Sum(),
                 Status = b.Status,
             })
             .Concat(
             db.AFEmployeeIssues
                .Where(b => b.Status == "Active" || b.Status == "Transferred")
                .Where(a => a.AFEmployees.DocStatus != 0)
                .Select(b => new ItemUnReturnViewModel()
            {
                ItemID = b.ItemID,
                ItemCode = b.Items.ItemCode,
                Description = b.Items.Description + " " + b.Items.Description2,
                DateIssued = b.DateIssued,
                RefNo = b.AFEmployees.EACNo,
                SerialNo = b.SerialNo,
                EmpID = b.AFEmployees.Employees.EmpId,
                EmployeeName = b.AFEmployees.Employees.LastName + "," + b.AFEmployees.Employees.FirstName,
                QtyIssued = (
                           b.Status == "Active" ? b.Quantity : b.QuantityAdj
                     ),
                QtyReturn = db.AFEmployeeReturns
                            .Where(a => a.DocStatus == 1)
                           .Where(a => a.Status == "Active")
                           .Where(a => a.ToolStatus != "Lost_Unreturned")
                           .Where(a => a.AFEmployeeIssueID == b.id)
                           .Select(a => a.Quantity)
                           .DefaultIfEmpty(0)
                           .Sum(),
                QtyLostUnreturned = db.AFEmployeeReturns
                          .Where(a => a.DocStatus == 1)
                          .Where(a => a.Status == "Active")
                          .Where(a => a.ToolStatus == "Lost_Unreturned")
                          .Where(a => a.AFEmployeeIssueID == b.id)
                          .Select(a => a.Quantity)
                          .DefaultIfEmpty(0)
                          .Sum(),
                Status = b.Status,
            })
            )
            .Concat(
            db.AFBorrowerIssues
            .Where(b => b.Status == "Active" || b.Status == "Transferred")
            .Where(b => b.AFBorrowers.DocStatus != 0)
            .Select(b => new ItemUnReturnViewModel()
            {
                ItemID = b.ItemID,
                ItemCode = b.Items.ItemCode,
                Description = b.Items.Description + " " + b.Items.Description2,
                DateIssued = b.DateIssued,
                RefNo = b.AFBorrowers.EBCNo,
                SerialNo = b.SerialNo,
                EmpID = b.AFBorrowers.Employees.EmpId,
                EmployeeName = b.AFBorrowers.Employees.LastName + "," + b.AFBorrowers.Employees.FirstName,
                QtyIssued = (
                           b.Status == "Active" ? b.Quantity : b.QuantityAdj
                     ),
                QtyReturn = db.AFBorrowerReturns
                            .Where(a => a.DocStatus == 1)
                           .Where(a => a.Status == "Active")
                           .Where(a => a.ToolStatus != "Lost_Unreturned")
                           .Where(a => a.AFBorrowerIssueID == b.id)
                           .Select(a => a.Quantity)
                           .DefaultIfEmpty(0)
                           .Sum(),
                QtyLostUnreturned = db.AFBorrowerReturns
                            .Where(a => a.DocStatus == 1)
                            .Where(a => a.Status == "Active")
                            .Where(a => a.ToolStatus == "Lost_Unreturned")
                            .Where(a => a.AFBorrowerIssueID == b.id)
                            .Select(a => a.Quantity)
                            .DefaultIfEmpty(0)
                            .Sum(),
                Status = b.Status,
            })
            )
            .ToList();


            if (id != null)
            {
                model = model.Where(b => b.ItemID == id).ToList();
            }


            model = model.Where(b => b.QtyIssued != (b.QtyReturn + b.QtyLostUnreturned)).ToList();

            var m = ToDataTable(model);
            DataTable dt = m;
            return dt;


        }
        public DataTable PrintItemLogs(int? id)
        {

            var result = db.ItemLogs.Select(
                         a =>
                            new ReportViewModel()
                            {
                                ItemID = a.Items.id,
                                ItemCode = a.Items.ItemCode,
                                Description = a.Items.Description,
                                Description2 = a.Items.Description2,
                                RefNo = a.Module,
                                Remarks = a.EntryType,
                                Qty = a.Quantity,
                                Date = a.Created_At
                            }
                        ).ToList();

            if (id != null)
            {
                result = result.Where(b => b.ItemID == id).ToList();
            }

            var m = ToDataTable(result.ToList());
            DataTable dt = m;
            return dt;

        }
        public DataTable PrintItemTracking(int? id)
        {
            //Items
            var result = db.ItemDetails
                .Where(i => i.Status == "Active")
                .Select(i => new ItemTrackingViewModel()
            {
                ItemID = i.ItemID
               ,
                RefNo = i.ReferenceNo
               ,
                EmployeeName = ""
               ,
                Date = i.DateCreated
               ,
                DateAdjusted = i.DateAdjusted
               ,
                ItemCode = i.Items.ItemCode
               ,
                Description = i.Items.Description
               ,
                SerialNo = i.SerialNo
               ,
                PO = i.PO
                ,
                PropertyNo = i.PropertyNo
                ,
                Location = i.Items.Location
                ,
                UoM = i.UoM
                ,
                Qty =
                 (
                i.ToolStatus == "Serviceable" ? i.Qty : i.Qty * (-1)
                 )
                ,
                QtyAdj = 0
                ,
                QtyLostUnreturned = 0
                ,
                UnitCost = i.UnitCost
                ,
                Amount = i.UnitCost * i.Qty
                ,
                ToolStatus = i.ToolStatus
                ,
                entrytype =
                 (
                i.ToolStatus == "Serviceable" ? "Positive Adjustment" : "Negative Adjustment"
                 )
                ,
                Remarks = i.Notes
                ,
                Status = i.Status
                ,
                HeadStatus = i.Status
                ,
                DocStatus = 1
            })
            .Concat(db.AFEmployeeIssues
            .Where(a => a.AFEmployees.Status == "Active" || a.AFEmployees.Status == "Transferred")
            .Where(a => a.Status == "Active" || a.Status == "Transferred")
            .Where(a => (a.Quantity * (-1)) != 0)
            .Where(a => a.AFEmployees.DocStatus != 0)
            .Select(i => new ItemTrackingViewModel()
            {
                ItemID = i.ItemID
                ,
                RefNo = i.AFEmployees.EACNo
                ,
                EmployeeName = i.AFEmployees.Employees.LastName + ", " + i.AFEmployees.Employees.FirstName
                ,
                Date = i.DateIssued
                ,
                DateAdjusted = i.Created_At
               ,
                ItemCode = i.Items.ItemCode
                ,
                Description = i.Items.Description
                ,
                SerialNo = i.SerialNo
                ,
                PO = i.PO
                ,
                PropertyNo = (
                    i.Items.Category == "CME" ? "" : i.Items.ItemDetails.Select(a => a.PropertyNo).FirstOrDefault()
                )
                    //PropertyNo = "Prop"
                ,
                Location = i.Items.Location
                ,
                UoM = i.UoM
                ,
                Qty = i.Quantity * (-1)
                ,
                QtyAdj = 0
                ,
                QtyLostUnreturned = 0
                ,
                UnitCost = i.UnitCost
                ,
                Amount = i.Quantity * i.UnitCost
                ,
                ToolStatus = "Serviceable"
                ,
                entrytype = "Issued"
                ,
                Remarks = i.Remarks
                ,
                Status = i.Status
                ,
                HeadStatus = i.AFEmployees.Status
                ,
                DocStatus = 1
            }))
            .Concat(db.AFEmployeeReturns
            .Where(a => a.Status == "Active")
            .Where(a => a.DocStatus == 1)
            .Select(a => new ItemTrackingViewModel()
            {
                ItemID = a.AFEmployeeIssues.ItemID
                ,
                RefNo = a.AFEmployeeIssues.AFEmployees.EACNo
                ,
                EmployeeName = a.AFEmployeeIssues.AFEmployees.Employees.LastName + ", " + a.AFEmployeeIssues.AFEmployees.Employees.FirstName
                ,
                Date = a.DateReturned
                ,
                DateAdjusted = a.DateReturned
               ,
                ItemCode = a.AFEmployeeIssues.Items.ItemCode
                ,
                Description = a.AFEmployeeIssues.Items.Description
                ,
                SerialNo = a.AFEmployeeIssues.SerialNo
                ,
                PO = a.AFEmployeeIssues.PO
                ,
                PropertyNo = (
                    a.AFEmployeeIssues.Items.Category == "CME" ? "" : a.AFEmployeeIssues.Items.ItemDetails.Select(x => x.PropertyNo).FirstOrDefault()
                )
                ,
                Location = a.AFEmployeeIssues.Items.Location
                ,
                UoM = a.AFEmployeeIssues.UoM
                ,
                Qty =
                 (
                a.ToolStatus == "Serviceable" ? a.Quantity : 0
                 )
                ,
                QtyAdj = (
                    a.ToolStatus == "Serviceable" ? 0 : (a.ToolStatus == "Lost_Unreturned" ? 0 : a.Quantity)
                    )
                ,
                QtyLostUnreturned = (a.ToolStatus == "Lost_Unreturned" ? a.Quantity : 0)
                ,
                UnitCost = a.AFEmployeeIssues.UnitCost
                ,
                Amount = a.Quantity * a.AFEmployeeIssues.UnitCost
                ,
                ToolStatus = a.ToolStatus
                ,
                entrytype = (
                        a.ToolStatus == "Serviceable" ? "Returned" : (a.ToolStatus == "Lost_Unreturned" ? "Un-Returned" : "Damaged")
                 )
                ,
                Remarks = a.Remarks
                ,
                Status = a.Status
                ,
                HeadStatus = a.Status
                ,
                DocStatus = a.DocStatus
            }))
            .Concat(db.AFEmployeeIssues
            .Where(a => a.AFEmployees.Status == "Transferred" || a.AFEmployees.Status == "Active")
            .Where(a => a.Status == "Transferred")
            .Where(a => (a.Quantity - a.QuantityAdj) != 0)
            .Where(a => a.AFEmployees.DocStatus != 0)
            .Select(i => new ItemTrackingViewModel() //for transfer offsetting EAC
            {
                ItemID = i.ItemID
                ,
                RefNo = i.AFEmployees.EACNo
                ,
                EmployeeName = i.AFEmployees.Employees.LastName + ", " + i.AFEmployees.Employees.FirstName
                ,
                Date = i.DateIssued
                ,
                DateAdjusted = i.Created_At
               ,
                ItemCode = i.Items.ItemCode
                ,
                Description = i.Items.Description
                ,
                SerialNo = i.SerialNo
                ,
                PO = i.PO
                ,
                PropertyNo = (
                    i.Items.Category == "CME" ? "" : i.Items.ItemDetails.Select(a => a.PropertyNo).FirstOrDefault()
                )
                ,
                Location = i.Items.Location
                ,
                UoM = i.UoM
                ,
                Qty = i.Quantity - i.QuantityAdj
                ,
                QtyAdj = 0
                ,
                QtyLostUnreturned = 0
                ,
                UnitCost = i.UnitCost
                ,
                Amount = i.Amount
                ,
                ToolStatus = "Serviceable"
                ,
                entrytype = "Transferred"
                ,
                Remarks = i.Remarks
                ,
                Status = i.Status
                ,
                HeadStatus = i.AFEmployees.Status
                ,
                DocStatus = 1
            }))
                //AFBorrower
            .Concat(db.AFBorrowerIssues
            .Where(a => a.AFBorrowers.Status == "Active" || a.AFBorrowers.Status == "Transferred")
            .Where(a => a.Status == "Active" || a.Status == "Transferred")
            .Where(a => (a.Quantity * (-1)) != 0)
            .Where(a => a.AFBorrowers.DocStatus != 0)
            .Select(i => new ItemTrackingViewModel()
            {
                ItemID = i.ItemID
                ,
                RefNo = i.AFBorrowers.EBCNo
                ,
                EmployeeName = i.AFBorrowers.Employees.LastName + ", " + i.AFBorrowers.Employees.FirstName
                ,
                Date = i.DateIssued
                ,
                DateAdjusted = i.Created_At
               ,
                ItemCode = i.Items.ItemCode
                ,
                Description = i.Items.Description
                ,
                SerialNo = i.SerialNo
                ,
                PO = i.PO
                ,
                PropertyNo = (
                     i.Items.Category == "CME" ? "" : i.Items.ItemDetails.Select(a => a.PropertyNo).FirstOrDefault()
                 )
                ,
                Location = i.Items.Location
                ,
                UoM = i.UoM
                ,
                Qty = i.Quantity * (-1)
                ,
                QtyAdj = 0
                ,
                QtyLostUnreturned = 0
                ,
                UnitCost = i.UnitCost
                ,
                Amount = i.Quantity * i.UnitCost
                ,
                ToolStatus = "Serviceable"
                ,
                entrytype = "Issued"
                ,
                Remarks = i.Remarks
                ,
                Status = i.Status
                ,
                HeadStatus = i.AFBorrowers.Status
                ,
                DocStatus = 1
            }))

            .Concat(db.AFBorrowerReturns
            .Where(a => a.Status == "Active")
            .Where(a => a.DocStatus == 1)
            .Select(a => new ItemTrackingViewModel()
            {
                ItemID = a.AFBorrowerIssues.ItemID
                ,
                RefNo = a.AFBorrowerIssues.AFBorrowers.EBCNo
                ,
                EmployeeName = a.AFBorrowerIssues.AFBorrowers.Employees.LastName + ", " + a.AFBorrowerIssues.AFBorrowers.Employees.FirstName
                ,
                Date = a.DateReturned
                ,
                DateAdjusted = a.DateReturned
               ,
                ItemCode = a.AFBorrowerIssues.Items.ItemCode
                ,
                Description = a.AFBorrowerIssues.Items.Description
                ,
                SerialNo = a.AFBorrowerIssues.SerialNo
                ,
                PO = a.AFBorrowerIssues.PO
                ,
                PropertyNo = (
                    a.AFBorrowerIssues.Items.Category == "CME" ? "" : a.AFBorrowerIssues.Items.ItemDetails.Select(x => x.PropertyNo).FirstOrDefault()
                )
                    //PropertyNo = "Prop"
                ,
                Location = a.AFBorrowerIssues.Items.Location
                ,
                UoM = a.AFBorrowerIssues.UoM
                ,
                Qty =
                  (
                 a.ToolStatus == "Serviceable" ? a.Quantity : 0
                  )
                 ,

                QtyAdj = (
                    a.ToolStatus == "Serviceable" ? 0 : (a.ToolStatus == "Lost_Unreturned" ? 0 : a.Quantity)
                    )
                    ,
                QtyLostUnreturned = (a.ToolStatus == "Lost_Unreturned" ? a.Quantity : 0)
                    ,

                UnitCost = a.AFBorrowerIssues.UnitCost
                ,
                Amount = a.Quantity * a.AFBorrowerIssues.UnitCost
                ,
                ToolStatus = a.ToolStatus
                ,
                entrytype = (
                    a.ToolStatus == "Serviceable" ? "Returned" : (a.ToolStatus == "Lost_Unreturned" ? "Un-Returned" : "Damaged")
                 )
                ,
                Remarks = a.Remarks
                ,
                Status = a.Status
                ,
                HeadStatus = a.Status
                ,
                DocStatus = a.DocStatus
            }))

            .Concat(db.AFBorrowerIssues
            .Where(a => a.AFBorrowers.Status == "Active" || a.AFBorrowers.Status == "Transferred")
            .Where(a => a.Status == "Transferred")
            .Where(a => (a.Quantity - a.QuantityAdj) != 0)
            .Where(a => a.AFBorrowers.DocStatus != 0)
            .Select(i => new ItemTrackingViewModel() // for transfer offsetting EBC
            {
                ItemID = i.ItemID
                ,
                RefNo = i.AFBorrowers.EBCNo
                ,
                EmployeeName = i.AFBorrowers.Employees.LastName + ", " + i.AFBorrowers.Employees.FirstName
                ,
                Date = i.DateIssued
                ,
                DateAdjusted = i.Created_At
               ,
                ItemCode = i.Items.ItemCode
                ,
                Description = i.Items.Description
                ,
                SerialNo = i.SerialNo
                ,
                PO = i.PO
                ,
                PropertyNo = (
                     i.Items.Category == "CME" ? "" : i.Items.ItemDetails.Select(a => a.PropertyNo).FirstOrDefault()
                 )
                ,
                Location = i.Items.Location
                ,
                UoM = i.UoM
                ,
                Qty = i.Quantity - i.QuantityAdj
                ,
                QtyAdj = 0
                ,
                QtyLostUnreturned = 0
                ,
                UnitCost = i.UnitCost
                ,
                Amount = i.Quantity * i.UnitCost
                ,
                ToolStatus = "Serviceable"
                ,
                entrytype = "Issued"
                ,
                Remarks = i.Remarks
                ,
                Status = i.Status
                ,
                HeadStatus = i.AFBorrowers.Status
                ,
                DocStatus = 1
            }))
            .Concat(db.AFBorrowerIssues
            .Where(a => a.AFBorrowers.Status == "Transferred")
            .Where(a => a.Status == "Transferred")
            .Where(a => (a.Quantity - a.QuantityAdj) != 0)
            .Where(a => a.AFBorrowers.DocStatus != 0)
            .Select(i => new ItemTrackingViewModel() //for transferred offsetting
            {
                ItemID = i.ItemID
                ,
                RefNo = i.AFBorrowers.EBCNo
                ,
                EmployeeName = i.AFBorrowers.Employees.LastName + ", " + i.AFBorrowers.Employees.FirstName
                ,
                Date = i.DateIssued
                ,
                DateAdjusted = i.Created_At
               ,
                ItemCode = i.Items.ItemCode
                ,
                Description = i.Items.Description
                ,
                SerialNo = i.SerialNo
                ,
                PO = i.PO
                ,
                PropertyNo = (
                     i.Items.Category == "CME" ? "" : i.Items.ItemDetails.Select(a => a.PropertyNo).FirstOrDefault()
                 )
                    //PropertyNo = "Prop"
                ,
                Location = i.Items.Location
                ,
                UoM = i.UoM
                ,
                Qty = i.Quantity - i.QuantityAdj
                ,
                QtyAdj = 0
                ,
                QtyLostUnreturned = 0
                ,
                UnitCost = i.UnitCost
                ,
                Amount = i.Amount
                ,
                ToolStatus = "Serviceable"
                ,
                entrytype = "Transferred"
                ,
                Remarks = i.Remarks
                ,
                Status = i.Status
                ,
                HeadStatus = i.AFBorrowers.Status
                ,
                DocStatus = 1
            }))
                //AFFA

            .Concat(db.AFFAIssues
            .Where(a => a.AFFAs.Status == "Active" || a.AFFAs.Status == "Transferred")
            .Where(a => a.Status == "Active" || a.Status == "Transferred")
            .Where(a => (a.Quantity * (-1)) != 0)
            .Where(a => a.AFFAs.DocStatus != 0)
            .Select(i => new ItemTrackingViewModel()
            {
                ItemID = i.ItemID
                ,
                RefNo = i.AFFAs.FAAFNo
                ,
                EmployeeName = i.AFFAs.Employees.LastName + ", " + i.AFFAs.Employees.FirstName
                ,
                Date = i.DateIssued
                ,
                DateAdjusted = i.Created_At
               ,
                ItemCode = i.Items.ItemCode
                ,
                Description = i.Items.Description
                ,
                SerialNo = i.SerialNo
                ,
                PO = i.PO
                ,
                PropertyNo = (
                     i.Items.Category == "CME" ? "" : i.Items.ItemDetails.Select(a => a.PropertyNo).FirstOrDefault()
                 )
                    //PropertyNo = "Prop"
                ,
                Location = i.Items.Location
                ,
                UoM = i.UoM
                ,
                Qty = i.Quantity * (-1)
                ,
                QtyAdj = 0
                ,
                QtyLostUnreturned = 0
                ,

                UnitCost = i.UnitCost
                ,
                Amount = i.Quantity * i.UnitCost
                ,
                ToolStatus = "Serviceable"
                ,
                entrytype = "Issued"
                ,
                Remarks = i.Remarks
                ,
                Status = i.Status
                ,
                HeadStatus = i.AFFAs.Status
                ,
                DocStatus = 1
            }))
            .Concat(db.AFFAReturns
            .Where(a => a.Status == "Active")
            .Where(a => a.DocStatus == 1)
            .Select(a => new ItemTrackingViewModel()
            {
                ItemID = a.AFFAIssues.ItemID
                ,
                RefNo = a.AFFAIssues.AFFAs.FAAFNo
                ,
                EmployeeName = a.AFFAIssues.AFFAs.Employees.LastName + ", " + a.AFFAIssues.AFFAs.Employees.FirstName
                ,
                Date = a.DateReturned
                ,
                DateAdjusted = a.DateReturned
               ,
                ItemCode = a.AFFAIssues.Items.ItemCode
                ,
                Description = a.AFFAIssues.Items.Description
                ,
                SerialNo = a.AFFAIssues.SerialNo
                ,
                PO = a.AFFAIssues.PO
                ,
                PropertyNo = (
                     a.AFFAIssues.Items.Category == "CME" ? "" : a.AFFAIssues.Items.ItemDetails.Select(x => x.PropertyNo).FirstOrDefault()
                 )
                    //PropertyNo = "Prop"
                ,
                Location = a.AFFAIssues.Items.Location
                ,
                UoM = a.AFFAIssues.UoM
                ,
                Qty =
                 (
                a.ToolStatus == "Serviceable" ? a.Quantity : 0
                 )
                ,
                QtyAdj = (
                    a.ToolStatus == "Serviceable" ? 0 : (a.ToolStatus == "Lost_Unreturned" ? 0 : a.Quantity)
                    )
                    ,
                QtyLostUnreturned = (a.ToolStatus == "Lost_Unreturned" ? a.Quantity : 0)
                ,

                UnitCost = a.AFFAIssues.UnitCost
                ,
                Amount = a.Quantity * a.AFFAIssues.UnitCost
                ,
                ToolStatus = a.ToolStatus
                ,
                entrytype = (
                a.ToolStatus == "Serviceable" ? "Returned" : (a.ToolStatus == "Lost_Unreturned" ? "Un-Returned" : "Damaged")
                 )
                ,
                Remarks = a.Remarks
                ,
                Status = a.Status
                ,
                HeadStatus = a.Status
                ,
                DocStatus = a.DocStatus
            }))
            .Concat(db.AFFAIssues
            .Where(a => a.AFFAs.Status == "Transferred")
            .Where(a => a.Status == "Transferred")
            .Where(a => (a.Quantity - a.QuantityAdj) != 0)
            .Where(a => a.AFFAs.DocStatus != 0)
            .Select(i => new ItemTrackingViewModel() //for transferred offsetting
            {
                ItemID = i.ItemID
                ,
                RefNo = i.AFFAs.FAAFNo
                ,
                EmployeeName = i.AFFAs.Employees.LastName + ", " + i.AFFAs.Employees.FirstName
                ,
                Date = i.DateIssued
                ,
                DateAdjusted = i.Created_At
               ,
                ItemCode = i.Items.ItemCode
                ,
                Description = i.Items.Description
                ,
                SerialNo = i.SerialNo
                ,
                PO = i.PO
                ,
                //PropertyNo = "Prop"
                PropertyNo = (
                i.Items.Category == "CME" ? "" : i.Items.ItemDetails.Select(a => a.PropertyNo).FirstOrDefault()
                 )
                ,
                Location = i.Items.Location
                ,
                UoM = i.UoM
                ,
                Qty = i.Quantity - i.QuantityAdj
                ,
                QtyAdj = 0
                ,
                QtyLostUnreturned = 0
                ,
                UnitCost = i.UnitCost
                ,
                Amount = i.Amount
                ,
                ToolStatus = "Serviceable"
                ,
                entrytype = "Transferred"
                ,
                Remarks = i.Remarks
                ,
                Status = i.Status
                ,
                HeadStatus = i.AFFAs.Status
                ,
                DocStatus = 1
            }))
            .ToList();

            if (id != null)
            {
                result = result.Where(b => b.ItemID == id).ToList();
            }
            var xxx = result.ToList();
            var m = ToDataTable(result.OrderByDescending(i => i.Date).ToList());
            DataTable dt = m;
            return dt;

        }
    }
}