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
    public class SignatoryController : Controller
    {
        private ToolManagementContext db = new ToolManagementContext();

        // GET: /Signatory/
        public ActionResult Index()
        {
            return View(db.Signatories.ToList());
        }

        // GET: /Signatory/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Signatory signatory = db.Signatories.Find(id);
            if (signatory == null)
            {
                return HttpNotFound();
            }
            return View(signatory);
        }

        // GET: /Signatory/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Signatory/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="id,SignatoryLabel,Name,Designation,Report,Company")] Signatory signatory)
        {
            if (ModelState.IsValid)
            {
                db.Signatories.Add(signatory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(signatory);
        }

        // GET: /Signatory/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Signatory signatory = db.Signatories.Find(id);
            if (signatory == null)
            {
                return HttpNotFound();
            }
            return View(signatory);
        }

        // POST: /Signatory/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="id,SignatoryLabel,Name,Designation,Report,Company")] Signatory signatory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(signatory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(signatory);
        }

        // GET: /Signatory/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Signatory signatory = db.Signatories.Find(id);
            if (signatory == null)
            {
                return HttpNotFound();
            }
            return View(signatory);
        }

        // POST: /Signatory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //Signatory signatory = db.Signatories.Find(id);
            //db.Signatories.Remove(signatory);
            //db.SaveChanges();
            Signatory signatory = db.Signatories.Find(id);
            signatory.Status = "Deleted";
            db.Entry(signatory).State = EntityState.Modified;
            db.SaveChanges();

            Log log = new Log();
            log.descriptions = "Deleted Signatory. Name:" + signatory.Name;
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

       
        [HttpGet]
        public JsonResult SearchSignatory(string comp, string report)
        {
            var model = db.Signatories.Where(i => i.Company == comp).Where(i => i.Report == report).Select(b => new
            {
                id = b.id,
                SignatoryLabel =b.SignatoryLabel,
                Name = b.Name,
                Designation = b.Designation,
            });

           
            var modelItem = new
            {
                items = model.ToList(),
            };
            return Json(model, JsonRequestBehavior.AllowGet);

        }
    }
}
