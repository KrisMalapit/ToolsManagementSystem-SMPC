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
    [CustomAuthorize]
    public class DesignationController : Controller
    {
        private ToolManagementContext db = new ToolManagementContext();

        // GET: /Designation/
        public ActionResult Index(int? page, string searchString)
        {
            ViewBag.SearchStr = searchString;

            var desigs = db.Designations.Where(a => a.Status == "Active").Where(i => i.id > 0);

            if (!String.IsNullOrEmpty(searchString))
            {
                desigs = desigs.Where(i => i.Name.Contains(searchString));
            }

            int pageSize = 15;
            int pageNumber = (page ?? 1);

            //if (!String.IsNullOrEmpty(searchString))
            //{
            //    if (pageNumber > 1)
            //    {
            //        pageNumber = 1;
            //    }
            //}


            return View(desigs.OrderBy(i => i.Name).ToPagedList(pageNumber, pageSize));
            //return View(db.Designations.ToList());
        }

        // GET: /Designation/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Designation designation = db.Designations.Find(id);
            if (designation == null)
            {
                return HttpNotFound();
            }
            return View(designation);
        }

        // GET: /Designation/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Designation/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="id,Name")] Designation designation)
        {
            if (ModelState.IsValid)
            {
                db.Designations.Add(designation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(designation);
        }

        // GET: /Designation/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Designation designation = db.Designations.Find(id);
            if (designation == null)
            {
                return HttpNotFound();
            }
            return View(designation);
        }

        // POST: /Designation/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="id,Name")] Designation designation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(designation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(designation);
        }

        // GET: /Designation/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Designation designation = db.Designations.Find(id);
            if (designation == null)
            {
                return HttpNotFound();
            }
            return View(designation);
        }

        // POST: /Designation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //Designation designation = db.Designations.Find(id);
            //db.Designations.Remove(designation);
            //db.SaveChanges();


            Designation designation = db.Designations.Find(id);
            designation.Status = "Deleted";
            db.Entry(designation).State = EntityState.Modified;
            db.SaveChanges();


            Log log = new Log();
            log.descriptions = "Deleted Designation. Name:" + designation.Name;
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
