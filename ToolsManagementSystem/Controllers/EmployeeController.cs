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
using System.Linq.Dynamic;
using ToolsManagementSystem.Models.View_Model;

namespace ToolsManagementSystem.Controllers
{
    [CustomAuthorize]
    public class EmployeeController : Controller
    {
        private ToolManagementContext db = new ToolManagementContext();

        // GET: /Employee/

        public ActionResult getData(int draw, int start, int length, string strcode, int noCols)
        {

            var sortColumn = Request["order[0][column]"];
            var sortColumnDir = Request["order[0][dir]"];
            var searchValue = Request["search[value]"];
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsFiltered = 0;
            int recordsTotal = 0;

            var v = db.Employees
                .Where(e => e.EntryType == "Individual").Where(e => e.Status == "Active")
                .Select(e =>
                    new
                    {
                        e.id,
                        RefNo = e.EmpId,
                        e.LastName,
                        e.FirstName,
                        DepartmentsName = e.Departments.Name,
                        DesignationsName = e.Designations.Name
                    }
                );

            recordsTotal = v.Count();

            if (!string.IsNullOrEmpty(searchValue))
            {
                v = v.Where(a =>
                    a.RefNo.ToUpper().Contains(searchValue.ToUpper()) ||
                    a.LastName.ToString().Contains(searchValue.ToUpper()) ||
                    a.FirstName.ToString().Contains(searchValue.ToUpper()) ||
                    a.DepartmentsName.ToString().Contains(searchValue.ToUpper()) ||
                    a.DesignationsName.ToString().Contains(searchValue.ToUpper()) 
                );
            }




            string strFilter = "";
            for (int i = 0; i < noCols; i++)
            {
                string colval = Request["columns[" + i + "][search][value]"];
                if (colval != "")
                {
                    string colSearch = Request["columns[" + i + "][data]"];

                
                        if (strFilter == "")
                        {
                            if (colval == "*")
                            {
                                strFilter = "(" + colSearch + " != " + "" + ")";
                            }
                            else
                            {

                                strFilter = (colSearch != "IssuedQty") ? colSearch + ".Contains(" + "\"" + colval + "\"" + ")" : "(" + colSearch + "=" + colval + ")";
                            }
                        }
                        else
                        {
                            strFilter = (colSearch != "IssuedQty") ? strFilter + " && " + colSearch + ".Contains(" + "\"" + colval + "\"" + ")" : strFilter + " && " + "(" + colSearch + "=" + colval + ")";
                        }
            
                    if (strFilter != "")
                    {
                        v = v.Where(strFilter);
                    }
                }
            }

          

            recordsFiltered = v.Count();


            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                //for make sort simpler we will add System.Linq.Dynamic reference
                string col = Request["columns[" + sortColumn + "][data]"];
                v = v.OrderBy(col + " " + sortColumnDir);
            }


            var x = v.ToList();
            if (pageSize < 0)
            {
                pageSize = recordsFiltered;
            }
            var allEmployees = v.Skip(skip).Take(pageSize).ToList();

            
            var model = new
            {
                draw = draw,
                recordsFiltered = recordsFiltered,
                recordsTotal = recordsTotal,
                data = allEmployees
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Index(int? page, string searchString)
        {
            //ViewBag.SearchStr = searchString;
            //var employees = db.Employees.Where(e => e.EntryType == "Individual").Where(e => e.Status == "Active").Include(e => e.Departments).Include(e => e.Designations);
            //if (!String.IsNullOrEmpty(searchString))
            //{
            //    employees = employees.Where(i => i.FirstName.Contains(searchString) || i.LastName.Contains(searchString));
            //}


            //int pageSize = 15;
            //int pageNumber = (page ?? 1);

            return View();

        }

        // GET: /Employee/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // GET: /Employee/Create
        public ActionResult Create()
        {
            //ViewBag.DepartmentID = new SelectList(db.Departments, "id", "Name");
            //ViewBag.DesignationID = new SelectList(db.Designations, "id", "Name");
            return View();
        }

        // POST: /Employee/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="id,EmpId,LastName,FirstName,DepartmentID,DesignationID")] Employee employee)


        {
            if (ModelState.IsValid)
            {
                
                db.Employees.Add(employee);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DepartmentID = new SelectList(db.Departments, "id", "Name", employee.DepartmentID);
            ViewBag.DesignationID = new SelectList(db.Designations, "id", "Name", employee.DesignationID);
            return View(employee);
        }

        // GET: /Employee/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepartmentID = new SelectList(db.Departments, "id", "Name", employee.DepartmentID);
            ViewBag.DesignationID = new SelectList(db.Designations, "id", "Name", employee.DesignationID);
            return View(employee);
        }

        // POST: /Employee/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="id,EmpId,LastName,FirstName,DepartmentID,DesignationID")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentID = new SelectList(db.Departments, "id", "Name", employee.DepartmentID);
            ViewBag.DesignationID = new SelectList(db.Designations, "id", "Name", employee.DesignationID);
            return View(employee);
        }

        // GET: /Employee/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: /Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Employee employee = db.Employees.Find(id);
            //db.Employees.Remove(employee);
            employee.Status = "Deleted";
            db.Entry(employee).State = EntityState.Modified;
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
        public JsonResult SearchDepartment_Json(string q)
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

            var modelDept = new
            {
                total_count = model.Count(),
                incomplete_results = false,
                items = model.ToList(),
            };
            return Json(modelDept, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SearchDesignation_Json(string q)
        {
            
            var model = db.Designations.Select(b => new
            {

                id = b.id,
                text = b.Name,
            });

            if (!string.IsNullOrEmpty(q))
            {
                model = model.Where(b => b.text.Contains(q));
            }

            var modelDesig = new
            {
                total_count = model.Count(),
                incomplete_results = false,
                items = model.ToList(),
            };
            return Json(modelDesig, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SearchEmployee_Json(string q,int? UserID)
        {
            IEnumerable<EmployeeViewModel> model = null;

            if (UserID == null)
            {
                model = db.Employees.Where(e => e.EntryType == "Individual").Where(e => e.Status == "Active").Select(b => new EmployeeViewModel
                {
                    id = b.id,
                    text = b.LastName + ", " + b.FirstName,
                });
            }
            else
            {
                var deptlist = db.UserDepts.Where(a => a.UserID == UserID).Select(a => a.DepartmentID).ToList();
                model = db.Employees.Where(Employee => deptlist.Contains(Employee.DepartmentID)).Where(e => e.EntryType == "Individual").Where(e => e.Status == "Active").Select(b => new EmployeeViewModel
                {
                    id = b.id,
                    text = b.LastName + ", " + b.FirstName,
                });
            }
            

            if (!string.IsNullOrEmpty(q))
            {
                model = model.Where(b => b.text.ToUpper().Contains(q.ToUpper()));
            }

            var modelEmp = new
            {
                total_count = model.Count(),
                incomplete_results = false,
                items = model.ToList(),
            };
            return Json(modelEmp, JsonRequestBehavior.AllowGet);
        }

    }
}
