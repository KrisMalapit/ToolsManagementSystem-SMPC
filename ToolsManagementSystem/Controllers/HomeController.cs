using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ToolsManagementSystem.DAL;
using ToolsManagementSystem.Models.View_Model;
using System.Linq.Dynamic;
using ToolsManagementSystem.Models;

namespace ToolsManagementSystem.Controllers
{
    public class HomeController : Controller
    {

        private ToolManagementContext db = new ToolManagementContext();
        ReportParameter[] p;

        //
        // GET: /Home/
        public ActionResult Index()
        {

            if (User.Identity.IsAuthenticated)
            {

                Boolean s = User.IsInRole("SuperAdmin");


                int cntAll = new ItemController().OverDueCount("all");
                int cntDay = new ItemController().OverDueCount("today");
                int cntWeek = new ItemController().OverDueCount("week");
                int cntMonth = new ItemController().OverDueCount("month");


                int cntOpenEBC = new ItemController().OpenCount("EBC");
                int cntOpenEAC = new ItemController().OpenCount("EAC");
                int cntOpenFA = new ItemController().OpenCount("FA");



                ViewBag.AllCount = cntAll.ToString();
                ViewBag.DayCount = cntDay.ToString();
                ViewBag.WeekCount = cntWeek.ToString();
                ViewBag.MonthCount = cntMonth.ToString();


                ViewBag.EBCCount = cntOpenEBC.ToString();
                ViewBag.EACCount = cntOpenEAC.ToString();
                ViewBag.FACount = cntOpenFA.ToString();


            }



            return View();
        }
        public ActionResult Maintenance()
        {
            return View();
        }
        public ActionResult Contact()
        {
            return View();
        }
        public ActionResult About()
        {
            return View();
        }

        public ActionResult PrintMLE()
        {
            DataTable dt = new DataTable();
            Warning[] warnings;
            string mimeType;
            string[] streamids;
            string encoding;
            string filenameExtension;

            var viewer = new ReportViewer();
            viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\MLE.rdlc";
            dt = new ReportController().PrintMLE();

            viewer.LocalReport.DataSources.Add(new ReportDataSource("MLE", dt));
            viewer.LocalReport.Refresh();

            var bytes = viewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

            return new FileContentResult(bytes, mimeType);

        }
        public ActionResult PrintMLEExcel()
        {
            DataTable dt = new DataTable();
            Warning[] warnings;
            string mimeType;
            string[] streamids;
            string encoding;
            string filenameExtension;

            var viewer = new ReportViewer();
            viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\MLE.rdlc";
            dt = new ReportController().PrintMLE();

            viewer.LocalReport.DataSources.Add(new ReportDataSource("MLE", dt));
            viewer.LocalReport.Refresh();

            var bytes = viewer.LocalReport.Render("Excel", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

            return new FileContentResult(bytes, mimeType);
        }



        public ActionResult FilterPrintES(FormCollection form)
        {
            DataTable dt = new DataTable();
            Warning[] warnings;
            string mimeType;
            string[] streamids;
            string encoding;
            string filenameExtension;

            var viewer = new ReportViewer();
            viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\ES.rdlc";
            dt = new ReportController().FilterPrintES(form);

            viewer.LocalReport.DataSources.Add(new ReportDataSource("MLE", dt));
            viewer.LocalReport.Refresh();

            var bytes = viewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

            foreach (var key in form.AllKeys)
            {
                if (key.StartsWith("show"))
                {
                    bytes = viewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);
                }
                if (key.StartsWith("excel"))
                {
                    bytes = viewer.LocalReport.Render("Excel", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);
                }
            }
            //var bytes = viewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

            return new FileContentResult(bytes, mimeType);

        }

        public ActionResult PrintBDM()
        {
            DataTable dt = new DataTable();
            Warning[] warnings;
            string mimeType;
            string[] streamids;
            string encoding;
            string filenameExtension;

            var viewer = new ReportViewer();
            viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\BDM.rdlc";
            dt = new ReportController().PrintBDM();

            viewer.LocalReport.DataSources.Add(new ReportDataSource("BDM", dt));
            viewer.LocalReport.Refresh();

            var bytes = viewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

            return new FileContentResult(bytes, mimeType);

        }
        public ActionResult PrintBDMExcel()
        {
            DataTable dt = new DataTable();
            Warning[] warnings;
            string mimeType;
            string[] streamids;
            string encoding;
            string filenameExtension;

            var viewer = new ReportViewer();
            viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\BDM.rdlc";
            dt = new ReportController().PrintBDM();

            viewer.LocalReport.DataSources.Add(new ReportDataSource("BDM", dt));
            viewer.LocalReport.Refresh();

            var bytes = viewer.LocalReport.Render("Excel", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

            return new FileContentResult(bytes, mimeType);

        }
        public ActionResult PrintStockPosition(DateTime? AsOf, DateTime? FromDate, DateTime? ToDate)
        {
            DataTable dt = new DataTable();
            Warning[] warnings;
            string mimeType;
            string[] streamids;
            string encoding;
            string filenameExtension;

            var viewer = new ReportViewer();
            viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\StockPosition.rdlc";
            dt = new ReportController().StockPosition(AsOf, FromDate, ToDate);

            viewer.LocalReport.DataSources.Add(new ReportDataSource("StockPosition", dt));
            viewer.LocalReport.Refresh();

            p = new ReportParameter[1];

            if (AsOf != null)
            {
                p[0] = new ReportParameter("DateRange", DateTime.Parse(AsOf.ToString()).ToString("MM/dd/yyyy"));
            }

            if (FromDate != null)
            {
                p[0] = new ReportParameter("DateRange", DateTime.Parse(FromDate.ToString()).ToString("MM/dd/yyyy") + " - " + DateTime.Parse(ToDate.ToString()).ToString("MM/dd/yyyy"));
            }


            viewer.LocalReport.SetParameters(p[0]);


            var bytes = viewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

            return new FileContentResult(bytes, mimeType);

        }
        public ActionResult StockPosition()
        {
            return View();

        }


        public ActionResult EmployeeSummary()
        {
            //ViewBag.Columns = FilterColumns();
            ViewBag.EmployeeSummary = null;
            return View();
        }
        public List<SelectListItem> FilterColumns()
        {
            List<SelectListItem> itemCol = new List<SelectListItem>();
            itemCol.Add(new SelectListItem { Text = "Employee Name", Value = "EmployeeName", Selected = true });
            itemCol.Add(new SelectListItem { Text = "Date Issued", Value = "DateIssued" });
            itemCol.Add(new SelectListItem { Text = "Description", Value = "Description" });
            itemCol.Add(new SelectListItem { Text = "Description 2", Value = "Description2" });
            itemCol.Add(new SelectListItem { Text = "Item Code", Value = "ItemCode" });
            itemCol.Add(new SelectListItem { Text = "PO No", Value = "PONo" });
            itemCol.Add(new SelectListItem { Text = "Serial No", Value = "SerialNo" });
            itemCol.Add(new SelectListItem { Text = "Remarks", Value = "Remarks" });
            itemCol.Add(new SelectListItem { Text = "Property No/ Fixed Asset No", Value = "PropertyNo" });
            itemCol.Add(new SelectListItem { Text = "Shelf No", Value = "ShelfNo" });
            itemCol.Add(new SelectListItem { Text = "Contractor", Value = "Contractor" });
            itemCol.Add(new SelectListItem { Text = "Work Order", Value = "WorkOrder" });
            itemCol.Add(new SelectListItem { Text = "Category", Value = "Category" });
            itemCol.Add(new SelectListItem { Text = "Date Returned", Value = "DateReturned" });
            itemCol.Add(new SelectListItem { Text = "Department", Value = "Department" });




            return itemCol;
        }


        public ActionResult getData(int draw, int start, int length, string strcode, int noCols)
        {
            var sortColumn = Request["order[0][column]"];
            var sortColumnDir = Request["order[0][dir]"];
            var searchValue = Request["search[value]"];
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsFiltered = 0;
            int recordsTotal = 0;
            DateTime dval = new DateTime();
            Boolean hasDateIssued = false;
            string strFilter = "";

            //var draw = Request.Form["draw"].FirstOrDefault();
            //var start = Request.Form["start"].FirstOrDefault();
            //var length = Request.Form["length"].FirstOrDefault();
            //var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            //var searchValue = Request.Form["search[value]"].FirstOrDefault();
            //int pageSize = length != null ? Convert.ToInt32(length) : 0;
            //int skip = start != null ? Convert.ToInt32(start) : 0;
            //int recordsTotal = 0;

            for (int i = 0; i < noCols; i++)
            {
                string colval = Request.Form["columns[" + i + "][search][value]"];
                if (colval != "")
                {
                    colval = colval.ToUpper();
                    string colSearch = Request.Form["columns[" + i + "][name]"];



                    if (strFilter == "")
                    {

                        strFilter = colSearch + ".ToString().ToUpper().Contains(" + "\"" + colval + "\"" + ")";

                    }
                    else
                    {
                        strFilter = strFilter + " && " + colSearch + ".ToString().ToUpper().Contains(" + "\"" + colval + "\"" + ")";
                    }

                }
            }


            if (strFilter == "")
            {
                strFilter = "true";
            }





            var _list =
                        db.AFBorrowerIssues
                        .Where(b => b.AFBorrowers.DocStatus != 0)
                        .Where(b => b.Status == "Active" || b.Status == "Transferred")
                        
                        .Select(a => 
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
                                EmployeeName = a.AFBorrowers.Employees.EntryType == "Individual" ? a.AFBorrowers.Employees.LastName + "," + a.AFBorrowers.Employees.FirstName : a.AFBorrowers.Employees.FirstName,
                                Member = a.AFBorrowers.Employees.EntryType == "Individual" ? "" : (db.VGroupMembers.Where(b => b.GroupCode == a.AFBorrowers.Employees.EmpId).Select(b => b.Members).FirstOrDefault()),
                                Department = a.AFBorrowers.Employees.Departments.Name,
                                PropertyNo = "",
                                Category = a.Items.Category,
                                Remarks = a.Remarks,
                                Contractor = a.AFBorrowers.Contractors.Name == "-N/A-" ? "" : a.AFBorrowers.Contractors.Name,
                                WorkOrder = a.AFBorrowers.WorkOrder,

                                ShelfNo = a.Items.ShelfNo,
                                DeptCode = "",
                                Status = a.Status,
                                QtyTransferred = a.QuantityTransferred,
                            })
                .Concat(db.AFFAIssues
                .Where(b => b.AFFAs.DocStatus != 0)
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
                        EmployeeName = a.AFFAs.Employees.EntryType == "Individual" ? a.AFFAs.Employees.LastName + "," + a.AFFAs.Employees.FirstName : a.AFFAs.Employees.FirstName,
                        Member = a.AFFAs.Employees.EntryType == "Individual" ? "" : (db.VGroupMembers.Where(b => b.GroupCode == a.AFFAs.Employees.EmpId).Select(b => b.Members).FirstOrDefault()),
                        Department = a.AFFAs.Employees.Departments.Name,
                        PropertyNo = a.PropertyNo + "/" + a.FACardNo,
                        Category = a.Items.Category,
                        Remarks = a.Remarks,
                        Contractor = "",
                        WorkOrder = "",
                 
                        ShelfNo = a.Items.ShelfNo,
                        DeptCode = a.AFFAs.Employees.Departments.Name,
                        Status = a.Status,
                        QtyTransferred = a.QuantityTransferred
                    }
                ))
                .Concat(
                         db.AFEmployeeIssues
                        .Where(b => b.AFEmployees.DocStatus != 0)
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
                                        EmpID = a.AFEmployees.Employees.EmpId,
                                        EmployeeName = a.AFEmployees.Employees.EntryType == "Individual" ? a.AFEmployees.Employees.LastName + "," + a.AFEmployees.Employees.FirstName : a.AFEmployees.Employees.FirstName,
                                        Member = a.AFEmployees.Employees.EntryType == "Individual" ? "" : (db.VGroupMembers.Where(b => b.GroupCode == a.AFEmployees.Employees.EmpId).Select(b => b.Members).FirstOrDefault()),
                                        Department = a.AFEmployees.Employees.Departments.Name,
                                        PropertyNo = "",
                                        Category = a.Items.Category,
                                        Remarks = a.Remarks,
                                        Contractor = "",
                                        WorkOrder = "",


                                        ShelfNo = a.Items.ShelfNo,
                                        DeptCode = a.AFEmployees.Employees.Departments.Name,
                                        Status = a.Status,
                                        QtyTransferred = a.QuantityTransferred
                                    }
                )).Where(strFilter);

            int recCount = _list.Count();
            recordsTotal = recCount;
            int recFilter = recCount;

            var v =_list
             .OrderByDescending(a => a.RefNo)
             //.Skip(skip).Take(pageSize)
             .Select(a => new
             {
                 a.ItemID ,
                 a.DateIssued,
                 a.ItemCode,
                 a.Description,
                 a.Description2,
                 a.PO,
                 a.SerialNo,
                 a.Qty,

                 a.QtyReturn,

                 a.UnitCost,
                 a.UOM,
                 a.RefNo,
                 a.EmpID,
                 a.EmployeeName,
                 a.Member,
                 a.Department,
                 a.PropertyNo,
                 a.Category,
                 a.Remarks,
                 a.Contractor,
                 a.WorkOrder,


                 a.ShelfNo,
                 a.DeptCode,
                 a.Status,
                 a.QtyTransferred
             });


            v = v.Skip(skip).Take(pageSize);

            //var x = v.ToList();
            //bool desc = false;
            //if (sortColumnDirection == "desc")
            //{
            //    desc = true;
            //}
            //v = v.OrderBy(sortColumn + (desc ? " descending" : ""));



            if (pageSize < 0)
            {
                pageSize = recordsTotal;
            }

            db.Database.CommandTimeout = 10000;
            var data = v.OrderByDescending(a => a.RefNo);
            var model = new { draw = draw, recordsFiltered = recFilter, recordsTotal = recordsTotal, data = data };
            return Json(model, JsonRequestBehavior.AllowGet);


            //var model = new
            //{
            //    draw = draw,
            //    recordsFiltered = recordsFiltered,
            //    recordsTotal = recordsTotal,
            //    data = allEmployees
            //};


            //return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult printEmployeeSummary()
        {
            
            DataTable dt = new DataTable();
            Warning[] warnings;
            string mimeType;
            string[] streamids;
            string encoding;
            string filenameExtension;

            var viewer = new ReportViewer();
            viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\ES.rdlc";

            if (TempData["EmpSum"] != null)
            {
                TempData.Keep("EmpSum");
                List<ReportViewModel> lst = (List<ReportViewModel>)TempData["EmpSum"];
                dt = new ReportController().ToDataTable(lst);

            }

            viewer.LocalReport.DataSources.Add(new ReportDataSource("MLE", dt));
            viewer.LocalReport.Refresh();

            var bytes = viewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

            return new FileContentResult(bytes, mimeType);

        }
        public ActionResult excelEmployeeSummary()
        {

            DataTable dt = new DataTable();
            Warning[] warnings;
            string mimeType;
            string[] streamids;
            string encoding;
            string filenameExtension;

            var viewer = new ReportViewer();
            viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\ES.rdlc";

            if (TempData["EmpSum"] != null)
            {
                TempData.Keep("EmpSum");
                List<ReportViewModel> lst = (List<ReportViewModel>)TempData["EmpSum"];
                dt = new ReportController().ToDataTable(lst);

            }

            viewer.LocalReport.DataSources.Add(new ReportDataSource("MLE", dt));
            viewer.LocalReport.Refresh();

            //var bytes = viewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);
            var bytes = viewer.LocalReport.Render("Excel", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

            return new FileContentResult(bytes, mimeType);

        }
    }
}