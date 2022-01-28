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
    public class AFFAReturnController : Controller
    {
        //
        private ToolManagementContext db = new ToolManagementContext();
        // GET: /AFFAReturn/
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /AFFAReturn/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /AFFAReturn/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /AFFAReturn/Create
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
        // GET: /AFFAReturn/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /AFFAReturn/Edit/5
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
        // GET: /AFFAReturn/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }


        [HttpPost]
        public ActionResult CreateAF(AFFAReturn item, string DateReturned)
        {
            item.DateReturned = DateTime.ParseExact(DateReturned, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            string FAARFNo = db.AFFAReturns.Where(a => a.Status == "Active").Where(a => a.FAARFNo == item.FAARFNo).Select(a => a.FAARFNo).FirstOrDefault();
            bool updateFAARFNo = false;

            if (string.IsNullOrEmpty(FAARFNo))
            {
                updateFAARFNo = true;
            }


            JsonArray res = new JsonArray();
            try
            {

                db.AFFAReturns.Add(item);
                db.SaveChanges();

                string s = item.FAARFNo;
                var x = s.Split('-');
                string lno = x[1].ToString();
                if (updateFAARFNo)
                {
                    NoSeries ns = db.NoSeries.SingleOrDefault(v => v.Code == "FAARF");
                    ns.LastNo = lno;
                    db.SaveChanges();
                }

                res.status = "success";

                AFFAIssue y = db.AFFAIssues.Find(item.AFFAIssueID);
                ItemLog itl = new ItemLog();
                itl.ItemID = y.ItemID;
                itl.Module = "FA";
                itl.EntryType = "Create Return";
                itl.Quantity = item.Quantity;
                db.ItemLogs.Add(itl);
                db.SaveChanges();

                Log log = new Log();
                log.descriptions = "Created items for returning. Table [AFFAReturn]. AFFAIssueID id: " + item.AFFAIssueID;
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
        public ActionResult EditAF(int id, AFFAReturn item, string DateReturned)
        {
            item.DateReturned = DateTime.ParseExact(DateReturned, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            JsonArray res = new JsonArray();
            try
            {

                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();

                AFFAReturn x = db.AFFAReturns.Find(id);
                var oldqty = x.Quantity;

                AFFAIssue y = db.AFFAIssues.Find(item.AFFAIssueID);
                ItemLog itl = new ItemLog();
                itl.ItemID = y.ItemID;
                itl.Module = "FA";
                itl.EntryType = "Modify Return";
                itl.Quantity = item.Quantity;
                db.ItemLogs.Add(itl);
                db.SaveChanges();


                Log log = new Log();
                log.descriptions = "Modified items in Table [AFFAReturn] id: " + id + " OldQty:" + oldqty + " NewQty: " + item.Quantity;
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
                    AFFAReturn subitem = db.AFFAReturns.Find(i);
                    subitem.Status = "Deleted_" + DateTime.Now.ToString();
                    db.Entry(subitem).State = EntityState.Modified;
                    db.SaveChanges();

                    AFFAIssue y = db.AFFAIssues.Find(subitem.AFFAIssueID);
                    ItemLog itl = new ItemLog();
                    itl.ItemID = y.ItemID;
                    itl.Module = "FA";
                    itl.EntryType = "Delete Return";
                    itl.Quantity = subitem.Quantity;
                    db.ItemLogs.Add(itl);
                    db.SaveChanges();

                    Log log = new Log();
                    log.descriptions = "Deleted items in Table [AFFAReturn] id: " + item;
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
                    AFFAReturn subitem = db.AFFAReturns.Find(i);
                    subitem.DocStatus = 1;
                    subitem.DatePosted = DateTime.Now.Date;
                    db.Entry(subitem).State = EntityState.Modified;
                    db.SaveChanges();

                    Log log = new Log();
                    log.descriptions = "Posted item in Table [AFFAReturn] id: " + item;
                    db.Logs.Add(log);
                    db.SaveChanges();

                    AFFAIssue y = db.AFFAIssues.Find(subitem.AFFAIssueID);
                    ItemLog itl = new ItemLog();
                    itl.ItemID = y.ItemID;
                    itl.Module = "AFFA";
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
        public ActionResult ReturnAll(string itemid, AFFAReturn affareturn, string DateReturned)
        {
            affareturn.DateReturned = DateTime.ParseExact(DateReturned, "MM/dd/yyyy", CultureInfo.InvariantCulture);


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
                        db.AFFAIssues
                        .Where(a => a.id == i)
                        .Select(a => a.Quantity)
                        .DefaultIfEmpty(0)
                        .Sum();

                    var qtyret =
                        db.AFFAReturns
                        .Where(a => a.AFFAIssueID == i)
                        .Select(a => a.Quantity)
                        .DefaultIfEmpty(0)
                        .Sum();

                    affareturn.AFFAIssueID = i;
                    affareturn.Quantity = qtyissued - qtyret;
                    db.AFFAReturns.Add(affareturn);
                    db.SaveChanges();

                    AFFAIssue subitem = db.AFFAIssues.Find(i);
                    subitem.Status = "Active";
                    afemp_id = subitem.AFFAID;
                    db.Entry(subitem).State = EntityState.Modified;
                    db.SaveChanges();


                    ItemLog itl = new ItemLog();
                    itl.ItemID = subitem.ItemID;
                    itl.Module = "FA";
                    itl.EntryType = "Transfer Return";
                    itl.Quantity = subitem.Quantity;
                    db.ItemLogs.Add(itl);
                    db.SaveChanges();


                    Log log = new Log();
                    log.descriptions = "Created items for returning. Table [AFFAReturn]. AFFAIssueID id: " + i;
                    db.Logs.Add(log);
                    db.SaveChanges();

                }

                //AFFA afemp = db.AFFAs.Find(afemp_id);
                //afemp.DocStatus = 2;
                //db.Entry(afemp).State = EntityState.Modified;
                //db.SaveChanges();



                string s = affareturn.FAARFNo;
                var x = s.Split('-');
                string lno = x[1].ToString();
                NoSeries ns = db.NoSeries.SingleOrDefault(v => v.Code == "FAARF");
                ns.LastNo = lno;
                db.SaveChanges();

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
                    AFFAReturn affareturn = new AFFAReturn();
                    affareturn.AFFAIssueID = i;
                    affareturn.Quantity = item.qtyreturn;

                    affareturn.DateReturned = item.datereturned;
                    affareturn.ToolStatus = item.toolstatus;
                    affareturn.Remarks = item.remarks;
                    affareturn.Recommendation = item.recstatus;
                    affareturn.FindingsObservation = item.finstatus;
                    affareturn.FAARFNo = item.faarfno;

                    db.AFFAReturns.Add(affareturn);
                    db.SaveChanges();

                    AFFAIssue subitem = db.AFFAIssues.Find(i);
                    subitem.Status = "Active";

                    db.Entry(subitem).State = EntityState.Modified;
                    db.SaveChanges();



                    string s = item.faarfno;
                    var x = s.Split('-');
                    string lno = x[1].ToString();

                    NoSeries ns = db.NoSeries.SingleOrDefault(v => v.Code == "FAARF");
                    ns.LastNo = lno;
                    db.SaveChanges();



                    ItemLog itl = new ItemLog();
                    itl.ItemID = subitem.ItemID;
                    itl.Module = "AFFA";
                    itl.EntryType = "Transfer Return";
                    itl.Quantity = subitem.Quantity;
                    db.ItemLogs.Add(itl);
                    db.SaveChanges();


                    Log log = new Log();
                    log.descriptions = "Created items for returning. Table [AFFAReturn]. AFFAIssueID id: " + i;
                    db.Logs.Add(log);
                    db.SaveChanges();
                }
                res.status = "success";
                res.message = "Transaction saved";
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
