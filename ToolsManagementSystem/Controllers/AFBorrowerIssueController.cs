using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ToolsManagementSystem.DAL;
using ToolsManagementSystem.Models;
using ToolsManagementSystem.Models.View_Model;

namespace ToolsManagementSystem.Controllers
{
    [CustomAuthorize]
    public class AFBorrowerIssueController : Controller
    {
        private ToolManagementContext db = new ToolManagementContext();
        //
        // GET: /AFBorrowerIssue/
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Delete(string itemid)
        {

            int i = 0;
            int[] res = new int[(itemid.Length) - 1];
            JsonArray result = new JsonArray();

            try
            {
                var elements = itemid.Split(',');
                foreach (var item in elements)
                {
                    i = Convert.ToInt32(item);
                    AFBorrowerIssue subitem = db.AFBorrowerIssues.Find(i);
                    subitem.Status = "Deleted_" + DateTime.Now.ToString();
                    db.Entry(subitem).State = EntityState.Modified;
                    db.SaveChanges();

                    ItemLog itl = new ItemLog();
                    itl.ItemID = subitem.ItemID;
                    itl.Module = "EBC";
                    itl.EntryType = "Delete";
                    itl.Quantity = subitem.Quantity;
                    db.ItemLogs.Add(itl);
                    db.SaveChanges();

                    Log log = new Log();
                    log.descriptions = "Deleted EBC Issue Record. AFBorrowerIssue id:" + item;
                    db.Logs.Add(log);
                    db.SaveChanges();
                }



                result.message = "success";

            }
            catch (Exception e)
            {
                result.message = "fail";
                throw;
            }


            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAFEmDetails(AFBorrowerIssue item, string unitcost_formatted, bool isMulti, string DateIssued, string DueDate)
        {
            item.DateIssued = DateTime.ParseExact(DateIssued, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            item.DueDate = DateTime.ParseExact(DueDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);

            JsonArray res = new JsonArray();
            try
            {
                if (isMulti)
                {
                    int cnt = item.Quantity;
                    for (int i = 1; i <= cnt; i++)
                    {
                        item.Quantity = 1;
                        item.UnitCost = Convert.ToDecimal(unitcost_formatted);
                        db.AFBorrowerIssues.Add(item);
                        db.SaveChanges();
                    }
                }
                else
                {
                    item.UnitCost = Convert.ToDecimal(unitcost_formatted);
                    db.AFBorrowerIssues.Add(item);
                    db.SaveChanges();
                }



                ItemLog itl = new ItemLog();
                itl.ItemID = item.ItemID;
                itl.Module = "EBC";
                itl.EntryType = "Create";
                itl.Quantity = item.Quantity;
                db.ItemLogs.Add(itl);
                db.SaveChanges();


                Log log = new Log();
                log.descriptions = "Created Issue Record. AFBorrowerID:" + item.AFBorrowerID;
                db.Logs.Add(log);
                db.SaveChanges();
                res.status = "success";

            }
            catch (Exception e)
            {

                res.message = e.ToString();
            }

            return Json(res);
            //return RedirectToAction("Edit", "AFBorrower", new { id = item.AFBorrowerID });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAFEmDetails(AFBorrowerIssue item, string unitcost_formatted, bool isMulti, string DateIssued, string DueDate)
        {
            item.DateIssued = DateTime.ParseExact(DateIssued, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            item.DueDate = DateTime.ParseExact(DueDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            JsonArray res = new JsonArray();
            try
            {



                if (isMulti)
                {

                    int cnt = item.Quantity;
                    for (int i = 1; i <= cnt; i++)
                    {
                        if (i == 1)
                        {
                            item.Quantity = 1;
                            item.UnitCost = Convert.ToDecimal(unitcost_formatted);
                            db.Entry(item).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            item.Quantity = 1;
                            item.UnitCost = Convert.ToDecimal(unitcost_formatted);
                            db.AFBorrowerIssues.Add(item);
                            db.SaveChanges();
                        }

                    }

                }
                else
                {
                    item.UnitCost = Convert.ToDecimal(unitcost_formatted);
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();


                }

                res.status = "success";
                ItemLog itl = new ItemLog();
                itl.ItemID = item.ItemID;
                itl.Module = "EBC";
                itl.EntryType = "Modify";
                itl.Quantity = item.Quantity;
                db.ItemLogs.Add(itl);
                db.SaveChanges();

                Log log = new Log();
                log.descriptions = "EBC - Modified Issue Record. id:" + item.id;
                db.Logs.Add(log);
                db.SaveChanges();

            }
            catch (Exception e)
            {
                res.message = e.ToString();

            }
            return Json(res);
        }


        [HttpPost]
        public ActionResult TransferEBC(string itemid, string EBCNo, int EmployeeID, string F_EBCNo, string transAll, DateTime duedate, string Purpose, string WorkOrder, int ContractorID)
        {
            Log log = new Log();
            JsonArray result = new JsonArray();
            try
            {

                string s = EBCNo;
                var x = s.Split('-');
                string lno = x[1].ToString();

                AFBorrower af = new AFBorrower();
                af.EBCNo = EBCNo;
                af.EmployeeID = EmployeeID;
                af.Remarks = "Transferred from " + F_EBCNo;
                af.Purpose = Purpose;
                af.UserID = db.Users.Where(a => a.Username == User.Identity.Name).Select(a => a.id).FirstOrDefault();
                af.WorkOrder = WorkOrder;
                af.DueDate = duedate;
                af.ContractorID = ContractorID;
                db.AFBorrowers.Add(af);
                db.SaveChanges();

                NoSeries ns = db.NoSeries.SingleOrDefault(v => v.Code == "EBC");
                ns.LastNo = lno;
                db.SaveChanges();


                log.descriptions = "Created new EBC No for transfer of items. EBCNo : " + EBCNo;
                db.Logs.Add(log);
                db.SaveChanges();







                int lastid = db.AFBorrowers.Max(a => a.id);
                int i = 0;
                int[] res = new int[(itemid.Length) - 1];
                var elements = itemid.Split(',');


                foreach (var item in elements)
                {
                    //modify
                    i = Convert.ToInt32(item);
                    AFBorrowerIssue subitem = db.AFBorrowerIssues.Find(i);
                    string old_remarks = subitem.Remarks;
                    int afemp_id = subitem.AFBorrowerID;
                    string old_serialno = subitem.SerialNo;
                    int old_issueid = subitem.AFBorrowerID;
                    int issue_qty = subitem.Quantity;
                    int ret_qty = db.AFBorrowerReturns
                        .Where(a => a.Status == "Active")
                        .Where(a => a.AFBorrowerIssueID == i)
                        .Select(a => a.Quantity)
                        .DefaultIfEmpty(0)
                        .Sum();

                    subitem.QuantityAdj = ret_qty;
                    subitem.QuantityTransferred = issue_qty - ret_qty;
                    subitem.Status = "Transferred";
                    subitem.Remarks = subitem.Remarks + " Transferred to " + EBCNo;
                    subitem.DateTransferred = DateTime.Now.Date;
                    //[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
                    db.Entry(subitem).State = EntityState.Modified;
                    db.SaveChanges();


                    ItemLog itl = new ItemLog();
                    itl.ItemID = subitem.ItemID;
                    itl.Module = "EBC";
                    itl.EntryType = "Transfer";
                    itl.Quantity = subitem.Quantity;
                    db.ItemLogs.Add(itl);
                    db.SaveChanges();



                    if (transAll == "true" || transAll == "all")
                    {
                        AFBorrower afemp = db.AFBorrowers.Find(afemp_id);
                        afemp.Status = "Transferred";
                        afemp.DocStatus = 2; //closed status
                        db.Entry(afemp).State = EntityState.Modified;
                        db.SaveChanges();
                    }





                    //transfer
                    AFBorrowerIssue new_subitem = db.AFBorrowerIssues.Find(i);
                    new_subitem.AFBorrowerID = lastid;
                    new_subitem.Quantity = issue_qty - ret_qty;
                    new_subitem.Status = "Active";
                    new_subitem.SerialNo = old_serialno;
                    new_subitem.QuantityAdj = 0;
                    new_subitem.Remarks = old_remarks + " Transferred from " + F_EBCNo;
                    new_subitem.DateIssued = DateTime.Now.Date;
                    new_subitem.DueDate = duedate;
                    new_subitem.QuantityTransferred = 0;
                    db.AFBorrowerIssues.Add(new_subitem);
                    db.SaveChanges();


                    log.descriptions = "Trasferred Issue Record. From AFBorrowerID: " + old_issueid + " to AFBorrowerID: " + lastid + " Table: [AFBorrowerIssue], Row ID: " + item;
                    db.Logs.Add(log);
                    db.SaveChanges();
                }


            }
            catch (Exception)
            {

                throw;
            }


            return Json(result);
        }
    }
}