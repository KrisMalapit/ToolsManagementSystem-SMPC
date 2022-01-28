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
    public class ContractorController : Controller
    {
        private ToolManagementContext db = new ToolManagementContext();

        // GET: /Contractor/
        public ActionResult Index(int? page, string searchString)
        {
            ViewBag.SearchStr = searchString;
            var contracts = db.Contractors.Where(d => d.Status == "Active").Where(i => i.id > 0);

            if (!String.IsNullOrEmpty(searchString))
            {
                contracts = contracts.Where(i => i.Name.Contains(searchString));
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

            return View(contracts.OrderBy(i => i.Name).ToPagedList(pageNumber, pageSize));

            //return View(db.Contractors.ToList());
        }

        // GET: /Contractor/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contractor contractor = db.Contractors.Find(id);
            if (contractor == null)
            {
                return HttpNotFound();
            }
            return View(contractor);
        }

        // GET: /Contractor/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Contractor/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="id,Name")] Contractor contractor)
        {
            if (ModelState.IsValid)
            {
                db.Contractors.Add(contractor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(contractor);
        }

        // GET: /Contractor/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contractor contractor = db.Contractors.Find(id);
            if (contractor == null)
            {
                return HttpNotFound();
            }
            return View(contractor);
        }

        // POST: /Contractor/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="id,Name")] Contractor contractor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contractor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(contractor);
        }

        // GET: /Contractor/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contractor contractor = db.Contractors.Find(id);
            if (contractor == null)
            {
                return HttpNotFound();
            }
            return View(contractor);
        }

        // POST: /Contractor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //Contractor contractor = db.Contractors.Find(id);
            //db.Contractors.Remove(contractor);
            //db.SaveChanges();


            Contractor contractor = db.Contractors.Find(id);
            contractor.Status = "Deleted";
            db.Entry(contractor).State = EntityState.Modified;
            db.SaveChanges();

            Log log = new Log();
            log.descriptions = "Deleted Contractor. Name:" + contractor.Name;
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
        public JsonResult SearchContractor_Json(string q)
        {

            var model = db.Contractors.Select(b => new
            {
                id = b.id,
                text = b.Name,
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
    }
}
