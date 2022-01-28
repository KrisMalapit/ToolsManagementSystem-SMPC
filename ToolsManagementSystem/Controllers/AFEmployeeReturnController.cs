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
    public class AFEmployeeReturnController : Controller
    {
        private ToolManagementContext db = new ToolManagementContext();
        //
        // GET: /AFEmployeeReturn/
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /AFEmployeeReturn/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /AFEmployeeReturn/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /AFEmployeeReturn/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {

            var x = 0;

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
        // GET: /AFEmployeeReturn/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /AFEmployeeReturn/Edit/5
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

        //
        // GET: /AFEmployeeReturn/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //

        [HttpPost]
        public ActionResult CreateAF(AFEmployeeReturn item, string DateReturned)
        {
            item.DateReturned = DateTime.ParseExact(DateReturned, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            JsonArray res = new JsonArray();
            try
            {
                db.AFEmployeeReturns.Add(item);
                db.SaveChanges();
                res.status = "success";

                AFEmployeeIssue x = db.AFEmployeeIssues.Find(item.AFEmployeeIssueID);
                ItemLog itl = new ItemLog();
                itl.ItemID = x.ItemID;
                itl.Module = "EAC";
                itl.EntryType = "Create Return";
                itl.Quantity = item.Quantity;
                db.ItemLogs.Add(itl);
                db.SaveChanges();

                Log log = new Log();
                log.descriptions = "Created items for returning. Table [AFEmployeeReturn]. AFEmployeeIssueID id: " + item.AFEmployeeIssueID;
                db.Logs.Add(log);
                db.SaveChanges();
            }
            catch (Exception)
            {
                res.status = "fail";
                throw;
            }
            return Json(res);
        }
        [HttpPost]
        public ActionResult EditAF(int id, AFEmployeeReturn item, string DateReturned)
        {
            item.DateReturned = DateTime.ParseExact(DateReturned, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            JsonArray res = new JsonArray();
            try
            {
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();

                AFEmployeeReturn x = db.AFEmployeeReturns.Find(id);
                var oldqty = x.Quantity;
                Log log = new Log();
                log.descriptions = "Modified items in Table [AFEmployeeReturn] id: " + id + " OldQty:" + oldqty + " NewQty: " + item.Quantity;
                db.Logs.Add(log);
                db.SaveChanges();


                AFEmployeeIssue y = db.AFEmployeeIssues.Find(item.AFEmployeeIssueID);
                ItemLog itl = new ItemLog();
                itl.ItemID = y.ItemID;
                itl.Module = "EAC";
                itl.EntryType = "Modify Return";
                itl.Quantity = item.Quantity;
                db.ItemLogs.Add(itl);
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
                    AFEmployeeReturn subitem = db.AFEmployeeReturns.Find(i);
                    subitem.Status = "Deleted_" + DateTime.Now.ToString();
                    db.Entry(subitem).State = EntityState.Modified;
                    db.SaveChanges();

                    AFEmployeeIssue x = db.AFEmployeeIssues.Find(subitem.AFEmployeeIssueID);
                    ItemLog itl = new ItemLog();
                    itl.ItemID = x.ItemID;
                    itl.Module = "EAC";
                    itl.EntryType = "Delete Return";
                    itl.Quantity = subitem.Quantity;
                    db.ItemLogs.Add(itl);
                    db.SaveChanges();

                    Log log = new Log();
                    log.descriptions = "Deleted items in Table [AFEmployeeReturn] id: " + item;
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
        public ActionResult Post(string itemid)
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
                    AFEmployeeReturn subitem = db.AFEmployeeReturns.Find(i);
                    subitem.DocStatus = 1;
                    subitem.DatePosted = DateTime.Now.Date;
                    db.Entry(subitem).State = EntityState.Modified;
                    db.SaveChanges();

                    Log log = new Log();
                    log.descriptions = "Posted item in Table [AFEmployeeReturn] id: " + item;
                    db.Logs.Add(log);
                    db.SaveChanges();

                    AFEmployeeIssue y = db.AFEmployeeIssues.Find(subitem.AFEmployeeIssueID);
                    ItemLog itl = new ItemLog();
                    itl.ItemID = y.ItemID;
                    itl.Module = "EBC";
                    itl.EntryType = "Posted Return";
                    itl.Quantity = subitem.Quantity;
                    db.ItemLogs.Add(itl);
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
        public ActionResult ReturnAll(string itemid, AFEmployeeReturn afemployeereturn, string DateReturned)
        {
            afemployeereturn.DateReturned = DateTime.ParseExact(DateReturned, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            int i = 0;
            int afemp_id = 0;

            JsonArray res = new JsonArray();
            try
            {
                var elements = itemid.Split(',');
                foreach (var item in elements)
                {
                    i = Convert.ToInt32(item);

                    var qtyissued =
                        db.AFEmployeeIssues
                        .Where(a => a.id == i)
                        .Select(a => a.Quantity)
                        .DefaultIfEmpty(0)
                        .Sum();

                    var qtyret =
                        db.AFEmployeeReturns
                        .Where(a => a.AFEmployeeIssueID == i)
                        .Select(a => a.Quantity)
                        .DefaultIfEmpty(0)
                        .Sum();

                    afemployeereturn.AFEmployeeIssueID = i;
                    afemployeereturn.Quantity = qtyissued - qtyret;
                    db.AFEmployeeReturns.Add(afemployeereturn);
                    db.SaveChanges();

                    AFEmployeeIssue subitem = db.AFEmployeeIssues.Find(i);
                    subitem.Status = "Active";
                    afemp_id = subitem.AFEmployeeID;
                    db.Entry(subitem).State = EntityState.Modified;
                    db.SaveChanges();


                    ItemLog itl = new ItemLog();
                    itl.ItemID = subitem.ItemID;
                    itl.Module = "EAC";
                    itl.EntryType = "Transfer Return";
                    itl.Quantity = subitem.Quantity;
                    db.ItemLogs.Add(itl);
                    db.SaveChanges();


                    Log log = new Log();
                    log.descriptions = "Created items for returning. Table [AFEmployeeReturn]. AFEmployeeIssueID id: " + i;
                    db.Logs.Add(log);
                    db.SaveChanges();

                }

                //AFEmployee afemp = db.AFEmployees.Find(afemp_id);
                //afemp.DocStatus = 2;
                //db.Entry(afemp).State = EntityState.Modified;
                //db.SaveChanges();




                res.status = "success";

            }
            catch (Exception)
            {
                res.status = "fail";
                throw;
            }
            return Json(res);
        }
        [HttpPost]
        public ActionResult ReturnMultiple(ReturnViewModel[] returnitems)
        {
            JsonArray res = new JsonArray();
            try
            {
                int i = 0;
                foreach (var item in returnitems)
                {
                    i = Convert.ToInt32(item.rowid);
                    AFEmployeeReturn afemployeereturn = new AFEmployeeReturn();
                    afemployeereturn.AFEmployeeIssueID = i;
                    afemployeereturn.Quantity = item.qtyreturn;
                    afemployeereturn.DateReturned = item.datereturned;
                    afemployeereturn.ToolStatus = item.toolstatus;
                    afemployeereturn.Remarks = item.remarks;
                    db.AFEmployeeReturns.Add(afemployeereturn);
                    db.SaveChanges();

                    AFEmployeeIssue subitem = db.AFEmployeeIssues.Find(i);
                    subitem.Status = "Active";

                    db.Entry(subitem).State = EntityState.Modified;
                    db.SaveChanges();


                    ItemLog itl = new ItemLog();
                    itl.ItemID = subitem.ItemID;
                    itl.Module = "EAC";
                    itl.EntryType = "Transfer Return";
                    itl.Quantity = subitem.Quantity;
                    db.ItemLogs.Add(itl);
                    db.SaveChanges();


                    Log log = new Log();
                    log.descriptions = "Created items for returning. Table [AFEmployeeReturn]. AFEmployeeIssueID id: " + i;
                    db.Logs.Add(log);
                    db.SaveChanges();
                }
                res.status = "success";
            }
            catch (Exception ex)
            {
                res.status = "fail";
                res.message = ex.InnerException.Message.ToString();
                throw;
            }
            return Json(res);
        }
    }
}
