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
using ToolsManagementSystem.Models.View_Model;
using Microsoft.AspNet.Identity;
using System.Security.Claims;

namespace ToolsManagementSystem.Controllers
{
    [CustomAuthorize]
    public class DepartmentController : Controller
    {
        private ToolManagementContext db = new ToolManagementContext();

        // GET: /Department/
        public ActionResult Index(int? page, string searchString)
        {
            ViewBag.SearchStr = searchString;
            string username = User.Identity.GetUserName();
            var user = db.Users.Select(a=> new {a.id,a.Username}).Where(a=>a.Username== username) ;

            int uid = user.Select(a=>a.id).FirstOrDefault();

            if (!User.IsInRole("SuperAdmin"))
            {
                var depts = db.Departments.Where(d=>d.Status=="Active").GroupJoin(db.UserDepts
                          , i => i.id
                          , r => r.DepartmentID,
                          (i, r) => new
                          {
                              Dept = i,
                              UserDept = r.DefaultIfEmpty()
                          })
                          .SelectMany(
                           a => a.UserDept.Select(b =>
                              new UserDeptViewModel
                              {
                                  id = a.Dept.id,
                                  Code =  a.Dept.Code,
                                  DeptCode = a.Dept.DeptCode,
                                  Name = a.Dept.Name,
                                  Approver = a.Dept.Approver,
                                  ApproverPosition = a.Dept.ApproverPosition,
                                  UserID = b.UserID,
                              }
                          )).Where(a=>a.UserID == uid );

                if (!String.IsNullOrEmpty(searchString))
                {
                    depts = depts.Where(i => i.Name.Contains(searchString));
                }
                

                var y = depts.ToList();
                int pageSize = 15;
                int pageNumber = (page ?? 1);

                //if (!String.IsNullOrEmpty(searchString))
                //{
                //    if (pageNumber > 1)
                //    {
                //        pageNumber = 1;
                //    }
                //}

                return View(depts.OrderBy(i => i.Name).ToPagedList(pageNumber, pageSize));

            }
            else
            {
                var depts = db.Departments.Where(d => d.Status == "Active").Select(a =>
                              new UserDeptViewModel
                              {
                                  id = a.id,
                                  Code = a.Code,
                                  DeptCode = a.DeptCode,
                                  Name = a.Name,
                                  Approver = a.Approver,
                                  ApproverPosition = a.ApproverPosition,
                                  UserID = 1,
                              }
                 );

                if (!String.IsNullOrEmpty(searchString))
                {
                    depts = depts.Where(i => i.Name.Contains(searchString));
                }



                var y = depts.ToList();
                int pageSize = 15;
                int pageNumber = (page ?? 1);

                //if (!String.IsNullOrEmpty(searchString))
                //{
                //    if (pageNumber > 1)
                //    {
                //        pageNumber = 1;
                //    }
                //}

                return View(depts.OrderBy(i => i.Name).ToPagedList(pageNumber, pageSize));
            }
            



            




        }

        // GET: /Department/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // GET: /Department/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Department/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,DeptCode,Name,Approver,ApproverPosition,Approver2,Approver2Position")] Department department)
        {
            if (ModelState.IsValid)
            {
                db.Departments.Add(department);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(department);
        }

        // GET: /Department/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: /Department/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,Code,DeptCode,Name,Approver,ApproverPosition,Approver2,Approver2Position")] Department department)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(department).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                string err = e.Message;
                throw;
            }
           
            return View(department);
        }

        // GET: /Department/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: /Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //Department department = db.Departments.Find(id);
            //db.Departments.Remove(department);
            //db.SaveChanges();
            //return RedirectToAction("Index");

            Department employee = db.Departments.Find(id);
            employee.Status = "Deleted";
            db.Entry(employee).State = EntityState.Modified;
            db.SaveChanges();

            Log log = new Log();
            log.descriptions = "Deleted Department. Name:" + employee.Name;
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

        //for select2
        public JsonResult SearchDepartment(string q)
        {
            
            var model = db.Departments.Select(b => new
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
        //for bootstrap table
        [HttpGet]
        public ActionResult GetDepartment(int offset, int limit, string search, string sort, string order, int ? userid)
        {
            var totalcount = 0;

            JsonArray res = new JsonArray();
            var dept = db.UserDepts.Select(a => new {
                id = a.id,
                UserID = a.UserID,
                Name = a.Departments.Name
            }).Where(a => a.UserID == userid);

            if (!string.IsNullOrEmpty(search))
            {
                dept = dept.Where(b => b.Name.Contains(search) );
            }

            totalcount = dept.Count();
            dept = dept.OrderBy(i => i.id).Skip(offset).Take(limit);

            var model = new
            {
                total = totalcount,
                rows = dept.ToList(),
            };
            return Json(model, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult RemoveDepartment(int id)
        {
            JsonArray res = new JsonArray();
            try
            {
                UserDept department = db.UserDepts.Find(id);
                db.UserDepts.Remove(department);
                db.SaveChanges();

               
                res.status = "success";
            }
            catch (Exception ex){
                res.status = ex.ToString();
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        
    }
}
