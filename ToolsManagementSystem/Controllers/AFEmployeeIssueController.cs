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
    public class AFEmployeeIssueController : Controller
    {
        private ToolManagementContext db = new ToolManagementContext();
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Details(int id)
        {
            return View();
        }


        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        public ActionResult Edit(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {


                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
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
                    AFEmployeeIssue subitem = db.AFEmployeeIssues.Find(i);
                    subitem.Status = "Deleted_" + DateTime.Now.ToString();
                    db.Entry(subitem).State = EntityState.Modified;
                    db.SaveChanges();

                    ItemLog itl = new ItemLog();
                    itl.ItemID = subitem.ItemID;
                    itl.Module = "EAC";
                    itl.EntryType = "Delete";
                    itl.Quantity = subitem.Quantity;
                    db.ItemLogs.Add(itl);
                    db.SaveChanges();


                    Log log = new Log();
                    log.descriptions = "Deleted EAC Issue Record. AFEmployeeIssue id:" + item;
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
        public ActionResult CreateAFEmDetails(AFEmployeeIssue item, string unitcost_formatted, bool isMulti, string DateIssued)
        {
            item.DateIssued = DateTime.ParseExact(DateIssued, "MM/dd/yyyy", CultureInfo.InvariantCulture);
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
                        db.AFEmployeeIssues.Add(item);
                        db.SaveChanges();
                    }
                }
                else
                {
                    item.UnitCost = Convert.ToDecimal(unitcost_formatted);
                    db.AFEmployeeIssues.Add(item);
                    db.SaveChanges();
                }


                ItemLog itl = new ItemLog();
                itl.ItemID = item.ItemID;
                itl.Module = "EAC";
                itl.EntryType = "Create";
                itl.Quantity = item.Quantity;
                db.ItemLogs.Add(itl);
                db.SaveChanges();

                Log log = new Log();
                log.descriptions = "Created Issue Record. AFEmployeeID:" + item.AFEmployeeID;
                db.Logs.Add(log);
                db.SaveChanges();
                res.status = "success";

            }
            catch (Exception e)
            {

                res.message = e.ToString();
            }

            return Json(res);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAFEmDetails(AFEmployeeIssue item, string unitcost_formatted, bool isMulti, string DateIssued)
        {
            item.DateIssued = DateTime.ParseExact(DateIssued, "MM/dd/yyyy", CultureInfo.InvariantCulture);
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
                            db.AFEmployeeIssues.Add(item);
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
                itl.Module = "EAC";
                itl.EntryType = "Modify";
                itl.Quantity = item.Quantity;
                db.ItemLogs.Add(itl);
                db.SaveChanges();

                Log log = new Log();
                log.descriptions = "EAC - Modified Issue Record. id:" + item.id;
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
        public ActionResult TransferEAC(string itemid, string EACNo, int EmployeeID, string F_EACNo, string transAll, string Purpose)
        {
            Log log = new Log();
            JsonArray result = new JsonArray();
            try
            {

                string s = EACNo;
                var x = s.Split('-');
                string lno = x[1].ToString();

                AFEmployee af = new AFEmployee();
                af.EACNo = EACNo;
                af.EmployeeID = EmployeeID;
                af.Remarks = "Transferred from " + F_EACNo;
                af.Purpose = Purpose;
                af.UserID = db.Users.Where(a => a.Username == User.Identity.Name).Select(a => a.id).FirstOrDefault();
                db.AFEmployees.Add(af);
                db.SaveChanges();

                NoSeries ns = db.NoSeries.SingleOrDefault(v => v.Code == "EAC");
                ns.LastNo = lno;
                db.SaveChanges();


                log.descriptions = "Created new EAC No for transfer of items. EACNo : " + EACNo;
                db.Logs.Add(log);
                db.SaveChanges();

                int lastid = db.AFEmployees.Max(a => a.id);
                int i = 0;
                int[] res = new int[(itemid.Length) - 1];
                var elements = itemid.Split(',');


                foreach (var item in elements)
                {
                    //modify
                    i = Convert.ToInt32(item);
                    AFEmployeeIssue subitem = db.AFEmployeeIssues.Find(i);
                    string old_remarks = subitem.Remarks;
                    int afemp_id = subitem.AFEmployeeID;
                    string old_serialno = subitem.SerialNo;
                    int old_issueid = subitem.AFEmployeeID;
                    int issue_qty = subitem.Quantity;
                    int ret_qty = db.AFEmployeeReturns
                        .Where(a => a.Status == "Active")
                        .Where(a => a.AFEmployeeIssueID == i)
                        .Select(a => a.Quantity)
                        .DefaultIfEmpty(0)
                        .Sum();

                    subitem.QuantityAdj = ret_qty;
                    subitem.QuantityTransferred = issue_qty - ret_qty;
                    subitem.Status = "Transferred";
                    subitem.Remarks = subitem.Remarks + "Transferred to " + EACNo;
                    subitem.DateTransferred = DateTime.Now.Date;
                    db.Entry(subitem).State = EntityState.Modified;
                    db.SaveChanges();


                    if (transAll == "true" || transAll == "all")
                    {
                        AFEmployee afemp = db.AFEmployees.Find(afemp_id);
                        afemp.Status = "Transferred";
                        afemp.DocStatus = 2; //closed status
                        db.Entry(afemp).State = EntityState.Modified;
                        db.SaveChanges();
                    }






                    //transfer
                    AFEmployeeIssue new_subitem = db.AFEmployeeIssues.Find(i);
                    new_subitem.AFEmployeeID = lastid;
                    new_subitem.Quantity = issue_qty - ret_qty;
                    new_subitem.Status = "Active";
                    new_subitem.SerialNo = old_serialno;
                    new_subitem.QuantityAdj = 0;
                    new_subitem.DateIssued = DateTime.Now.Date;
                    new_subitem.Remarks = old_remarks + " Transferred from " + F_EACNo;
                    new_subitem.QuantityTransferred = 0;
                    db.AFEmployeeIssues.Add(new_subitem);
                    db.SaveChanges();


                    ItemLog itl = new ItemLog();
                    itl.ItemID = new_subitem.ItemID;
                    itl.Module = "EAC";
                    itl.EntryType = "Transfer";
                    itl.Quantity = new_subitem.Quantity;
                    db.ItemLogs.Add(itl);
                    db.SaveChanges();


                    log.descriptions = "Trasferred Issue Record. From AFEmployeeID: " + old_issueid + " to AFEmployeeID: " + lastid + " Table: [AFEmployeeIssue], Row ID: " + item;
                    db.Logs.Add(log);
                    db.SaveChanges();
                    result.status = "success";
                }


            }
            catch (Exception e)
            {
                result.status = "fail";
                result.message = e.InnerException.InnerException.Message;
                throw;
            }


            return Json(result);
        }



    }
}
