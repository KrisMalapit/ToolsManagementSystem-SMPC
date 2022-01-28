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


namespace ToolsManagementSystem.Controllers
{
    public class GroupAccountabilityController : Controller
    {
        private ToolManagementContext db = new ToolManagementContext();

        // GET: /GroupAccountability/
        public ActionResult Index(int? page, string searchString)
        {
            ViewBag.SearchStr = searchString;
            var group = db.GroupAccountabilities.Where(e => e.Status == "Active").Where(i => i.id > 0)
                .Select(g => new GroupAccountabilityViewModel()
                { 
                 id = g.id,
                   Code= g.Code,
                   Name = g.Name,
                   Description = g.Description,
                   DepartmentName = (db.Departments.Where(d=>d.id == g.DepartmentID).Select(d => d.Name).FirstOrDefault())
                    }
                )
                ;
            
            if (!String.IsNullOrEmpty(searchString))
            {
                group = group.Where(i => i.Name.Contains(searchString));
            }

            int pageSize = 15;
            int pageNumber = (page ?? 1);
            return View(group.OrderBy(i => i.Name).ToPagedList(pageNumber, pageSize));

        }

        // GET: /GroupAccountability/Details/5
        public ActionResult Details(int? id)
        {

            ViewBag.ID = id;
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupAccountability groupaccountability = db.GroupAccountabilities.Find(id);
            if (groupaccountability == null)
            {
                return HttpNotFound();
            }
    
            return View(groupaccountability);


        }

        // GET: /GroupAccountability/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /GroupAccountability/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,Code,Name,Description,DepartmentID")] GroupAccountability groupaccountability)
        {
            if (ModelState.IsValid)
            {
                groupaccountability.Status = "Active";
                db.GroupAccountabilities.Add(groupaccountability);
                

                Employee emp = new Employee();
                emp.EmpId = groupaccountability.Code;
                emp.LastName = groupaccountability.Description;
                emp.FirstName = groupaccountability.Name;
                emp.DepartmentID = groupaccountability.DepartmentID; 
                emp.DesignationID = 62; // for N/A
                emp.EntryType = "Group";
                //emp.Status = "Active";

                db.Employees.Add(emp);
                
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(groupaccountability);
        }

        // GET: /GroupAccountability/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupAccountability groupaccountability = db.GroupAccountabilities.Find(id);
            if (groupaccountability == null)
            {
                return HttpNotFound();
            }
            ViewBag.Department = groupaccountability.DepartmentID;
            ViewBag.DepartmentName = db.Departments.Where(a => a.id == groupaccountability.DepartmentID).Select(a => a.Name).FirstOrDefault().ToString();
            return View(groupaccountability);
        }

        // POST: /GroupAccountability/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,Code,Name,Description,DepartmentID")] GroupAccountability groupaccountability)
        {
            if (ModelState.IsValid)
            {
                groupaccountability.Status = "Active";
                db.Entry(groupaccountability).State = EntityState.Modified;

                int empid = db.Employees.Where(e => e.EmpId == groupaccountability.Code).Select(e=>e.id).FirstOrDefault();
                Employee emp = db.Employees.Find(empid);
                emp.LastName = groupaccountability.Description;
                emp.FirstName = groupaccountability.Name;
                emp.EntryType = "Group";
                emp.Status = "Active";

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(groupaccountability);
        }

        // GET: /GroupAccountability/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupAccountability groupaccountability = db.GroupAccountabilities.Find(id);
            if (groupaccountability == null)
            {
                return HttpNotFound();
            }
            return View(groupaccountability);
        }

        // POST: /GroupAccountability/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GroupAccountability groupaccountability = db.GroupAccountabilities.Find(id);
            groupaccountability.Status = "Deleted";
            db.Entry(groupaccountability).State = EntityState.Modified;


            int empid = db.Employees.Where(e => e.EmpId == groupaccountability.Code).Select(e => e.id).FirstOrDefault();
            Employee emp = db.Employees.Find(empid);
            emp.Status = "Deleted";
            db.Entry(emp).State = EntityState.Modified;

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
        //for bootstrap table
        [HttpGet]
        public ActionResult GroupMember(int offset, int limit, string search, string sort, string order, int? groupid)
        {
            var totalcount = 0;

            JsonArray res = new JsonArray();
            var groupmember = db.GroupAccountabilityMembers.Select(a => new
            {
                id = a.id,
                Employee = a.Employees.LastName + ", "  +a.Employees.FirstName,
                GroupName = a.GroupAccountabilities.Name,
                GroupID = a.GroupAccountabilityID
            }).Where(a => a.GroupID == groupid);

            if (!string.IsNullOrEmpty(search))
            {
                groupmember = groupmember.Where(b => b.Employee.Contains(search));
            }

            totalcount = groupmember.Count();
            groupmember = groupmember.OrderBy(i => i.id).Skip(offset).Take(limit);

            var model = new
            {
                total = totalcount,
                rows = groupmember.ToList(),
            };
            return Json(model, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult SaveGroupMember(GroupAccountabilityMember groupmember)
        {
            JsonArray arr = new JsonArray();
            try
            {
                db.GroupAccountabilityMembers.Add(groupmember);
                db.SaveChanges();
                arr.status = "success";
            }
            catch (Exception e)
            {
                arr.status = e.ToString();
                arr.message = e.InnerException.InnerException.Message;

            }
            return Json(arr, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteMember(int id)
        {

            JsonArray arr = new JsonArray();
            try
            {
                GroupAccountabilityMember groupmember = db.GroupAccountabilityMembers.Find(id);
                db.GroupAccountabilityMembers.Remove(groupmember);
                db.SaveChanges();
                arr.status = "success";


            }
            catch (Exception e)
            {

                arr.status = e.ToString();
            }


            return Json(arr, JsonRequestBehavior.AllowGet);

           
        }
        public JsonResult SearchGroup_Json(string q)
        {

            var groupName = db.Employees.Where(e => e.EntryType == "Group")
                .Where(e => e.Status == "Active")
                .Select(b => new
                {
                    id = b.id,
                    empid = b.EmpId,
                    groupname = b.FirstName,
                });

            var groupnames = groupName.ToList();

            List<GroupViewModel>lst = new List<GroupViewModel>();

            foreach (var item in groupnames)
            {
                var groupList = db.GroupAccountabilityMembers
                    .Where(g => g.GroupAccountabilities.Status == "Active")
                    .Where(g => g.GroupAccountabilities.Code == item.empid);

                var groupLists = groupList.ToList();
                if (groupList.Count() > 0)
                {
                    foreach (var item2 in groupLists)
                    {
                        var x = new GroupViewModel()
                        {
                            id = item.id,
                            empid = item.empid,
                            groupname = item.groupname,
                            employeename = item2.Employees.LastName + ", " + item2.Employees.FirstName,
                        };
                        lst.Add(x);
                    }
                }
            }

            var group = lst.AsEnumerable().Select(l => new {
                id = l.id,
                empid = l.empid,
                groupname = l.groupname,
                employeename = l.employeename
            });

            var x1 = group.ToList();
            if (!string.IsNullOrEmpty(q))
            {
                group = group.Where(b => b.employeename.ToUpper().Contains(q.ToUpper()) || b.groupname.ToUpper().Contains(q.ToUpper()));
            }

            var groupss = group.Select(g => new
            {
                id = g.id,
                text = g.groupname
            }).GroupBy(x=>x.id).Select(x=>x.First());

            var modelEmp = new
            {
                total_count = groupss.Count(),
                incomplete_results = false,
                items = groupss.ToList(),
            };
            return Json(modelEmp, JsonRequestBehavior.AllowGet);
        }
    }
}
