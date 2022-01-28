using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ToolsManagementSystem.Models;
using ToolsManagementSystem.DAL;

namespace ToolsManagementSystem.Controllers
{
    [CustomAuthorize]
    public class ItemDetailController : Controller
    {
        private ToolManagementContext db = new ToolManagementContext();

        // GET: /ItemDetail/
        public ActionResult Index()
        {
            var itemdetails = db.ItemDetails.Include(i => i.Items);
            return View(itemdetails.ToList());
        }

        // GET: /ItemDetail/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemDetail itemdetail = db.ItemDetails.Find(id);
            if (itemdetail == null)
            {
                return HttpNotFound();
            }
            return View(itemdetail);
        }

        // GET: /ItemDetail/Create
        public ActionResult Create()
        {
            ViewBag.ItemID = new SelectList(db.Items, "id", "ItemCode");
            return View();
        }

        // POST: /ItemDetail/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="id,ItemID,FACardNo,PropertyNo,Description,Description2,PO,SerialNo,Qty,ModelType,UoM,UnitCost,ShelfNo,DateAdjusted,ToolStatus,Notes,ReferenceNo")] ItemDetail itemdetail)
        {
            if (ModelState.IsValid)
            {
                itemdetail.DateCreated = DateTime.Now;
                db.ItemDetails.Add(itemdetail);
                db.SaveChanges();

                


                ItemLog itl = new ItemLog();
                itl.ItemID = itemdetail.ItemID;
                itl.Module = "ItemDetails";
                itl.EntryType = "Create";
                itl.Quantity = itemdetail.Qty;
                db.SaveChanges();

                Log log = new Log();
                log.descriptions = "Added record id:" + itemdetail.id + " Table [ItemDetail]";
                log.otherinfo = "Qty : " + itemdetail.Qty.ToString();
                db.Logs.Add(log);
                db.SaveChanges();


                return RedirectToAction("Index");
            }






            ViewBag.ItemID = new SelectList(db.Items, "id", "ItemCode", itemdetail.ItemID);
            return View(itemdetail);
        }

        // GET: /ItemDetail/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemDetail itemdetail = db.ItemDetails.Find(id);
            if (itemdetail == null)
            {
                return HttpNotFound();
            }



            ViewBag.ItemID = new SelectList(db.Items, "id", "ItemCode", itemdetail.ItemID);
            return View(itemdetail);
        }

        // POST: /ItemDetail/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="id,ItemID,FACardNo,PropertyNo,Description,Description2,PO,SerialNo,Qty,ModelType,UoM,UnitCost,ShelfNo,DateAdjusted,ToolStatus,Notes")] ItemDetail itemdetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(itemdetail).State = EntityState.Modified;
                db.SaveChanges();

                ItemLog itl = new ItemLog();
                itl.ItemID = itemdetail.ItemID;
                itl.Module = "ItemDetails";
                itl.EntryType = "Modify";
                itl.Quantity = itemdetail.Qty;
                db.SaveChanges();

                Log log = new Log();
                log.descriptions = "Modified record id:" + itemdetail.id + " Table [ItemDetail]";
                log.otherinfo = "Qty : " + itemdetail.Qty.ToString();
                db.Logs.Add(log);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            ViewBag.ItemID = new SelectList(db.Items, "id", "ItemCode", itemdetail.ItemID);
            return View(itemdetail);
        }

        // GET: /ItemDetail/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemDetail itemdetail = db.ItemDetails.Find(id);
            if (itemdetail == null)
            {
                return HttpNotFound();
            }
            return View(itemdetail);
        }

        // POST: /ItemDetail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ItemDetail itemdetail = db.ItemDetails.Find(id);
            db.ItemDetails.Remove(itemdetail);
            db.SaveChanges();

            ItemLog itl = new ItemLog();
            itl.ItemID = itemdetail.ItemID;
            itl.Module = "ItemDetails";
            itl.EntryType = "Delete";
            itl.Quantity = itemdetail.Qty;
            db.SaveChanges();

            Log log = new Log();
            log.descriptions = "Deleted record id:" + itemdetail.id + " Table [ItemDetail]";
            log.otherinfo = "Qty : " + itemdetail.Qty.ToString();
            db.Logs.Add(log);
            db.SaveChanges();


            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public JsonResult SearchItemSerial_Json(string q, int? id)
        {

            var model = db.ItemDetails.Where(i=>i.ItemID==id).Where(i=>i.Status=="Active").Select(b => new
            {
                id = b.SerialNo,
                text = b.SerialNo,
            });

            if (!string.IsNullOrEmpty(q))
            {
                model = model.Where(b => b.text.Contains(q));
            }

            var modelItem = new
            {
                total_count = model.Count(),
                incomplete_results = false,
                items = model.ToList(),
            };
            return Json(modelItem, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SearchItemPO_Json(string q, int? id)
        {

            var model = db.ItemDetails.Where(i => i.ItemID == id).Where(i => i.Status == "Active").Select(b => new
            {
                id = b.PO,
                text = b.PO,
            });

            if (!string.IsNullOrEmpty(q))
            {
                model = model.Where(b => b.text.Contains(q));
            }

            var modelItem = new
            {
                total_count = model.Count(),
                incomplete_results = false,
                items = model.ToList(),
            };
            return Json(modelItem, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SearchItemProperty_Json(string q, int? id)
        {

            var model = db.ItemDetails.Where(i => i.ItemID == id).Where(i => i.Status == "Active").Select(b => new
            {
                id = b.PropertyNo,
                text = b.PropertyNo,
            });

            if (!string.IsNullOrEmpty(q))
            {
                model = model.Where(b => b.text.Contains(q));
            }

            var modelItem = new
            {
                total_count = model.Count(),
                incomplete_results = false,
                items = model.ToList(),
            };
            return Json(modelItem, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetSerialDetails(int afid, string sn)
        {

            JsonArray res = new JsonArray();
            var item = db.ItemDetails.Where(i => i.ItemID == afid).Where(i => i.SerialNo == sn).Select(i => new
            {
                PO = i.PO,
                UnitCost = i.UnitCost,
                UOM = i.UoM,
                PropertyNo = i.PropertyNo,
                FACardNo = i.FACardNo
            });
            return Json(item, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public ActionResult GetPODetails(int afid, string po)
        {
            JsonArray res = new JsonArray();
            var item = db.ItemDetails.Where(i => i.ItemID == afid).Where(i => i.PO == po).Select(i => new
            {
                SerialNo = i.SerialNo,
                UnitCost = i.UnitCost,
                UOM = i.UoM,
                PropertyNo = i.PropertyNo,
                FACardNo = i.FACardNo
            });
            return Json(item, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public ActionResult GetPropertyDetails(int afid, string prop)
        {
            JsonArray res = new JsonArray();
            var item = db.ItemDetails.Where(i => i.ItemID == afid).Where(i => i.PropertyNo == prop).Select(i => new
            {
                SerialNo = i.SerialNo,
                UnitCost = i.UnitCost,
                UOM = i.UoM,
                PO = i.PO,
                FACardNo = i.FACardNo
            });
            return Json(item, JsonRequestBehavior.AllowGet);

        }
    }
}
