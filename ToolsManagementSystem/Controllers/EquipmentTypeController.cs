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
using PagedList;

namespace ToolsManagementSystem.Controllers
{
    public class EquipmentTypeController : Controller
    {
        private ToolManagementContext db = new ToolManagementContext();

        // GET: /EquipmentType/
        public ActionResult Index(int? page, string searchString)
        {
            ViewBag.SearchStr = searchString;

            var desigs = db.EquipmentTypes.Where(i => i.id > 0);

            if (!String.IsNullOrEmpty(searchString))
            {
                desigs = desigs.Where(i => i.Description.Contains(searchString));
            }

            int pageSize = 15;
            int pageNumber = (page ?? 1);


            return View(desigs.OrderBy(i => i.Description).ToPagedList(pageNumber, pageSize));

            //return View(db.EquipmentTypes.ToList());
        }

        // GET: /EquipmentType/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EquipmentType equipmenttype = db.EquipmentTypes.Find(id);
            if (equipmenttype == null)
            {
                return HttpNotFound();
            }
            return View(equipmenttype);
        }

        // GET: /EquipmentType/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /EquipmentType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="id,Code,Description")] EquipmentType equipmenttype)
        {
            if (ModelState.IsValid)
            {
                db.EquipmentTypes.Add(equipmenttype);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(equipmenttype);
        }

        // GET: /EquipmentType/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EquipmentType equipmenttype = db.EquipmentTypes.Find(id);
            if (equipmenttype == null)
            {
                return HttpNotFound();
            }
            return View(equipmenttype);
        }

        // POST: /EquipmentType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="id,Code,Description")] EquipmentType equipmenttype)
        {
            if (ModelState.IsValid)
            {
                db.Entry(equipmenttype).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(equipmenttype);
        }

        // GET: /EquipmentType/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EquipmentType equipmenttype = db.EquipmentTypes.Find(id);
            if (equipmenttype == null)
            {
                return HttpNotFound();
            }
            return View(equipmenttype);
        }

        // POST: /EquipmentType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //EquipmentType equipmenttype = db.EquipmentTypes.Find(id);
            //db.EquipmentTypes.Remove(equipmenttype);
            //db.SaveChanges();

            EquipmentType equipmenttype = db.EquipmentTypes.Find(id);
            equipmenttype.Status = "Deleted";
            db.Entry(equipmenttype).State = EntityState.Modified;
            db.SaveChanges();

            Log log = new Log();
            log.descriptions = "Deleted EquipmentType. Name:" + equipmenttype.Description;
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
    }
}
