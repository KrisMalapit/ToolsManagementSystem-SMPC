using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ToolsManagementSystem.DAL;
using ToolsManagementSystem.Models;

namespace ToolsManagementSystem.Controllers
{
    [CustomAuthorize]
    public class AFFAIssueController : Controller
    {
        private ToolManagementContext db = new ToolManagementContext();
        //
        // GET: /AFFAIssue/
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /AFFAIssue/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /AFFAIssue/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /AFFAIssue/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /AFFAIssue/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /AFFAIssue/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

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
                    AFFAIssue subitem = db.AFFAIssues.Find(i);
                    subitem.Status = "Deleted_" + DateTime.Now.ToString();
                    db.Entry(subitem).State = EntityState.Modified;
                    db.SaveChanges();

                    ItemLog itl = new ItemLog();
                    itl.ItemID = subitem.ItemID;
                    itl.Module = "FA";
                    itl.EntryType = "Delete";
                    itl.Quantity = subitem.Quantity;
                    db.ItemLogs.Add(itl);
                    db.SaveChanges();

                    Log log = new Log();
                    log.descriptions = "Deleted AFFA Issue Record. AFFAIssue id:" + item;
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
        public ActionResult CreateAFFADetails(AFFAIssue item, string unitcost_formatted, string DateIssued)
        {
            item.DateIssued = DateTime.ParseExact(DateIssued, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            JsonArray res = new JsonArray();
            try
            {
                item.UnitCost = Convert.ToDecimal(unitcost_formatted);
                db.AFFAIssues.Add(item);
                db.SaveChanges();

                ItemLog itl = new ItemLog();
                itl.ItemID = item.ItemID;
                itl.Module = "FA";
                itl.EntryType = "Create";
                itl.Quantity = item.Quantity;
                db.ItemLogs.Add(itl);
                db.SaveChanges();

                Log log = new Log();
                log.descriptions = "Created Issue Record. AFFAID:" + item.AFFAID;
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
        public ActionResult EditAFFADetails(AFFAIssue item, string unitcost_formatted, string DateIssued)
        {
            item.DateIssued = DateTime.ParseExact(DateIssued, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            JsonArray res = new JsonArray();
            try
            {
                //if (ModelState.IsValid)
                //{
                item.UnitCost = Convert.ToDecimal(unitcost_formatted);
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
                res.status = "success";

                ItemLog itl = new ItemLog();
                itl.ItemID = item.ItemID;
                itl.Module = "FA";
                itl.EntryType = "Modify";
                itl.Quantity = item.Quantity;
                db.ItemLogs.Add(itl);
                db.SaveChanges();

                Log log = new Log();
                log.descriptions = "AFFA - Modified Issue Record. id:" + item.id;
                db.Logs.Add(log);
                db.SaveChanges();

                //}
                //else
                //{
                //    res.status = "failed";
                //}
            }
            catch (Exception e)
            {
                res.message = e.ToString();

            }
            return Json(res);
        }
        [HttpPost]
        public ActionResult TransferFA(string itemid, string FAAFNo, int EmployeeID, string F_FAAFNo, string transAll)
        {
            Log log = new Log();
            JsonArray result = new JsonArray();
            try
            {

                string s = FAAFNo;
                var x = s.Split('-');
                string lno = x[1].ToString();

                AFFA af = new AFFA();
                af.FAAFNo = FAAFNo;
                af.EmployeeID = EmployeeID;
                af.UserID = db.Users.Where(a => a.Username == User.Identity.Name).Select(a => a.id).FirstOrDefault();
                af.Remarks = "Transferred from " + F_FAAFNo;
                db.AFFAs.Add(af);
                db.SaveChanges();

                NoSeries ns = db.NoSeries.SingleOrDefault(v => v.Code == "FAAF");
                ns.LastNo = lno;
                db.SaveChanges();


                log.descriptions = "Created new FAAF No for transfer of items. FANo : " + FAAFNo;
                db.Logs.Add(log);
                db.SaveChanges();

                int lastid = db.AFFAs.Max(a => a.id);
                int i = 0;
                int[] res = new int[(itemid.Length) - 1];
                var elements = itemid.Split(',');


                foreach (var item in elements)
                {
                    //modify
                    i = Convert.ToInt32(item);
                    AFFAIssue subitem = db.AFFAIssues.Find(i);
                    string old_remarks = subitem.Remarks;
                    int afemp_id = subitem.AFFAID;
                    string old_serialno = subitem.SerialNo;
                    int old_issueid = subitem.AFFAID;
                    int issue_qty = subitem.Quantity;
                    int ret_qty = db.AFFAReturns
                        .Where(a => a.Status == "Active")
                        .Where(a => a.AFFAIssueID == i)
                        .Select(a => a.Quantity)
                        .DefaultIfEmpty(0)
                        .Sum();

                    subitem.QuantityAdj = ret_qty;
                    subitem.QuantityTransferred = issue_qty - ret_qty;
                    subitem.Status = "Transferred";
                    subitem.Remarks = "Transferred to " + FAAFNo;
                    subitem.DateTransferred = DateTime.Now.Date;
                    db.Entry(subitem).State = EntityState.Modified;
                    db.SaveChanges();

                    //AFFA afemp = db.AFFAs.Find(afemp_id);
                    //afemp.Status = "Transferred";
                    //afemp.DocStatus = 2; //closed status
                    //db.Entry(afemp).State = EntityState.Modified;
                    //db.SaveChanges();


                    if (transAll == "true" || transAll == "all")
                    {
                        AFFA affa = db.AFFAs.Find(afemp_id);
                        affa.Status = "Transferred";
                        affa.DocStatus = 2; //closed status
                        db.Entry(affa).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    //transfer
                    AFFAIssue new_subitem = db.AFFAIssues.Find(i);
                    new_subitem.AFFAID = lastid;
                    new_subitem.Quantity = issue_qty - ret_qty;
                    new_subitem.Status = "Active";
                    new_subitem.SerialNo = old_serialno;
                    new_subitem.QuantityAdj = 0;
                    new_subitem.DateIssued = DateTime.Now.Date;
                    new_subitem.Remarks = old_remarks + " Transferred from " + F_FAAFNo;
                    db.AFFAIssues.Add(new_subitem);
                    db.SaveChanges();


                    ItemLog itl = new ItemLog();
                    itl.ItemID = new_subitem.ItemID;
                    itl.Module = "FA";
                    itl.EntryType = "Transfer";
                    itl.Quantity = new_subitem.Quantity;
                    db.ItemLogs.Add(itl);
                    db.SaveChanges();

                    log.descriptions = "Trasferred Issue Record. From AFFAID: " + old_issueid + " to AFFAID: " + lastid + " Table: [AFFAIssue], Row ID: " + item;
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
