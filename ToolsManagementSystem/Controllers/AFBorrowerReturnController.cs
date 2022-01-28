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
    public class AFBorrowerReturnController : Controller
    {
        private ToolManagementContext db = new ToolManagementContext();
        //
        // GET: /AFBorrowerReturn/
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateAF(AFBorrowerReturn item, string DateReturned)
        {
            item.DateReturned = DateTime.ParseExact(DateReturned, "MM/dd/yyyy", CultureInfo.InvariantCulture);

            JsonArray res = new JsonArray();
            try
            {

                db.AFBorrowerReturns.Add(item);
                db.SaveChanges();
                res.status = "success";

                AFBorrowerIssue x = db.AFBorrowerIssues.Find(item.AFBorrowerIssueID);
                ItemLog itl = new ItemLog();
                itl.ItemID = x.ItemID;
                itl.Module = "EBC";
                itl.EntryType = "Create Return";
                itl.Quantity = item.Quantity;
                db.ItemLogs.Add(itl);
                db.SaveChanges();

                Log log = new Log();
                log.descriptions = "Created items for returning. Table [AFBorrowerReturn]. AFBorrowerIssueID id: " + item.AFBorrowerIssueID;
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
        public ActionResult EditAF(int id, AFBorrowerReturn item, string DateReturned)
        {
            item.DateReturned = DateTime.ParseExact(DateReturned, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            JsonArray res = new JsonArray();
            try
            {

                //x.Quantity = Quantity;
                //x.DateReturned = DateReturned;

                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();

                AFBorrowerReturn x = db.AFBorrowerReturns.Find(id);
                var oldqty = x.Quantity;


                AFBorrowerIssue y = db.AFBorrowerIssues.Find(item.AFBorrowerIssueID);
                ItemLog itl = new ItemLog();
                itl.ItemID = y.ItemID;
                itl.Module = "EBC";
                itl.EntryType = "Modify Return";
                itl.Quantity = item.Quantity;
                db.ItemLogs.Add(itl);
                db.SaveChanges();


                Log log = new Log();
                log.descriptions = "Modified items in Table [AFBorrowerReturn] id: " + id + " OldQty:" + oldqty + " NewQty: " + item.Quantity;
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
                    AFBorrowerReturn subitem = db.AFBorrowerReturns.Find(i);
                    subitem.Status = "Deleted_" + DateTime.Now.ToString();
                    db.Entry(subitem).State = EntityState.Modified;
                    db.SaveChanges();

                    Log log = new Log();
                    log.descriptions = "Deleted items in Table [AFBorrowerReturn] id: " + item;
                    db.Logs.Add(log);
                    db.SaveChanges();

                    AFBorrowerIssue y = db.AFBorrowerIssues.Find(subitem.AFBorrowerIssueID);
                    ItemLog itl = new ItemLog();
                    itl.ItemID = y.ItemID;
                    itl.Module = "EBC";
                    itl.EntryType = "Delete Return";
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
                    AFBorrowerReturn subitem = db.AFBorrowerReturns.Find(i);
                    subitem.DocStatus = 1;
                    subitem.DatePosted = DateTime.Now.Date;
                    db.Entry(subitem).State = EntityState.Modified;
                    db.SaveChanges();

                    Log log = new Log();
                    log.descriptions = "Posted item in Table [AFBorrowerReturn] id: " + item;
                    db.Logs.Add(log);
                    db.SaveChanges();

                    AFBorrowerIssue y = db.AFBorrowerIssues.Find(subitem.AFBorrowerIssueID);
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
        public ActionResult ReturnAll(string itemid, AFBorrowerReturn afborrowerreturn, string DateReturned)
        {
            int i = 0;
            int afemp_id = 0;
            afborrowerreturn.DateReturned = DateTime.ParseExact(DateReturned, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            JsonArray res = new JsonArray();
            try
            {
                var elements = itemid.Split(',');
                foreach (var item in elements)
                {
                    i = Convert.ToInt32(item);

                    var qtyissued =
                        db.AFBorrowerIssues
                        .Where(a => a.id == i)
                        .Where(a => a.Status == "Active")
                        .Select(a => a.Quantity)
                        .DefaultIfEmpty(0)
                        .Sum();

                    var qtyret =
                        db.AFBorrowerReturns
                        .Where(a => a.AFBorrowerIssueID == i)
                        .Where(a => a.Status == "Active")
                        .Select(a => a.Quantity)
                        .DefaultIfEmpty(0)
                        .Sum();

                    afborrowerreturn.AFBorrowerIssueID = i;
                    afborrowerreturn.Quantity = qtyissued - qtyret;
                    db.AFBorrowerReturns.Add(afborrowerreturn);
                    db.SaveChanges();

                    AFBorrowerIssue subitem = db.AFBorrowerIssues.Find(i);
                    subitem.Status = "Active";
                    afemp_id = subitem.AFBorrowerID;
                    db.Entry(subitem).State = EntityState.Modified;
                    db.SaveChanges();


                    ItemLog itl = new ItemLog();
                    itl.ItemID = subitem.ItemID;
                    itl.Module = "EBC";
                    itl.EntryType = "Transfer Return";
                    itl.Quantity = subitem.Quantity;
                    db.ItemLogs.Add(itl);
                    db.SaveChanges();


                    Log log = new Log();
                    log.descriptions = "Created items for returning. Table [AFBorrowerReturn]. AFBorrowerIssueID id: " + i;
                    db.Logs.Add(log);
                    db.SaveChanges();

                }

                //AFBorrower afemp = db.AFBorrowers.Find(afemp_id);
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
                    AFBorrowerReturn afborrowereturn = new AFBorrowerReturn();
                    afborrowereturn.AFBorrowerIssueID = i;
                    afborrowereturn.Quantity = item.qtyreturn;
                    afborrowereturn.DateReturned = item.datereturned;
                    afborrowereturn.ToolStatus = item.toolstatus;
                    afborrowereturn.Remarks = item.remarks;
                    db.AFBorrowerReturns.Add(afborrowereturn);
                    db.SaveChanges();

                    AFBorrowerIssue subitem = db.AFBorrowerIssues.Find(i);
                    subitem.Status = "Active";

                    db.Entry(subitem).State = EntityState.Modified;
                    db.SaveChanges();


                    ItemLog itl = new ItemLog();
                    itl.ItemID = subitem.ItemID;
                    itl.Module = "EBC";
                    itl.EntryType = "Transfer Return";
                    itl.Quantity = subitem.Quantity;
                    db.ItemLogs.Add(itl);
                    db.SaveChanges();


                    Log log = new Log();
                    log.descriptions = "Created items for returning. Table [AFBorrowerReturn]. AFBorrowerIssueID id: " + i;
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