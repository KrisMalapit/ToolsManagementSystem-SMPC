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
using ToolsManagementSystem.Reports;
using Microsoft.Reporting.WebForms;
using System.Web.UI.WebControls;
using ToolsManagementSystem.Models.View_Model;
using System.Linq.Dynamic;
using Microsoft.AspNet.Identity;
using static ToolsManagementSystem.Models.View_Model.GroupViewModel;
using System.Data.Entity.Validation;
using System.Globalization;

namespace ToolsManagementSystem.Controllers
{
    [CustomAuthorize]
    public class AFBorrowerController : Controller
    {
        private ToolManagementContext db = new ToolManagementContext();

        // GET: /AFBorrower/
        public ActionResult Index(string sortOrder, int? page, string searchString, string searchByStatus)
        {


            ViewBag.EBCNoSortParm = String.IsNullOrEmpty(sortOrder) ? "EBCNo" : "";
            ViewBag.SearchStr = searchString;
            var afemployees = db.AFBorrowers.Include(e => e.Employees).Where(b => b.Status == "Active" || b.Status == "Transferred");

            if (!String.IsNullOrEmpty(searchString))
            {
                afemployees = afemployees
                            .Where(i => i.Employees.LastName.Contains(searchString)
                                || i.Employees.FirstName.Contains(searchString)
                                || i.EBCNo.Contains(searchString));
            }
            int pageSize = 15;
            int pageNumber = (page ?? 1);

            TempData.Remove("Status");
            if (!string.IsNullOrEmpty(searchByStatus))
            {
                TempData["Status"] = searchByStatus;
            }


            return View(afemployees.OrderByDescending(s => s.id).ToPagedList(pageNumber, pageSize));

        }
        public ActionResult Index_Warehouse()
        {
            string colval = Request["columns[0][search][value]"];
            return View();

        }
        // GET: /AFEmployee/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AFBorrower afemployee = db.AFBorrowers.Find(id);
            if (afemployee == null)
            {
                return HttpNotFound();
            }
            return View(afemployee);
        }

        // GET: /AFEmployee/Create
        public ActionResult Create(string returnurl)
        {
            ViewBag.DepartmentID = db.UserDepts.Where(a => a.Users.Username == User.Identity.Name).Select(a => a.DepartmentID).FirstOrDefault();
            ViewBag.UserID = db.UserDepts.Where(a => a.Users.Username == User.Identity.Name).Select(a => a.UserID).FirstOrDefault();
            ViewBag.ReturnUrl = returnurl;
            return View();
        }

        // POST: /AFEmployee/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        public ActionResult Create(AFBorrower afemployee, GroupMemberViewModel[] arry, string trans, int conid, string DueDate)
        {
            afemployee.DueDate = DateTime.ParseExact(DueDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            //new modification
            string deptcode = "";
            string deptname = "";
            int deptid = 0;

            JsonArray res = new JsonArray();

            try
            {
                if (trans == "Group")
                {
                    string s = "";
                    var group = db.UserDepts.Where(a => a.Users.Username == User.Identity.Name).FirstOrDefault();
                    deptcode = group.Departments.Code;
                    deptname = group.Departments.Name;
                    deptid = group.DepartmentID;

                    //new setup for num series
                    string lno = db.NoSeries.Where(i => i.Code == deptcode).Select(n => n.LastNo).FirstOrDefault();
                    if (string.IsNullOrEmpty(lno))
                    {
                        NoSeries ns = new NoSeries();
                        ns.Code = deptcode;
                        ns.LastNo = "0000000";
                        s = "0000000";
                    }
                    else
                    {
                        s = lno;
                    }

                    int number = Convert.ToInt32(s);
                    number += 1;
                    string str = number.ToString("D7");



                    GroupAccountability ga = new GroupAccountability
                    {
                        Code = deptcode + str,
                        Name = deptname + " GROUP " + number,
                        Description = deptname + " GROUP " + number,
                        DepartmentID = deptid,
                        Status = "Active"
                    };

                    db.GroupAccountabilities.Add(ga);
                    db.SaveChanges();


                    foreach (var item in arry)
                    {
                        GroupAccountabilityMember gm = new GroupAccountabilityMember
                        {
                            GroupAccountabilityID = ga.id,
                            EmployeeID = item.EmployeeID
                        };
                        db.GroupAccountabilityMembers.Add(gm);
                        db.SaveChanges();
                    }

                    //for saving in employee table 
                    Employee emp = new Employee();
                    emp.EmpId = deptcode + str;
                    emp.LastName = deptname + " GROUP " + number;
                    emp.FirstName = deptname + " GROUP " + number;
                    emp.DepartmentID = deptid;
                    emp.DesignationID = 373; // for N/A
                    emp.EntryType = "Group";
                    db.Employees.Add(emp);
                    db.SaveChanges();
                    //end for saving in employee table 

                    afemployee.EmployeeID = emp.id;//attached for group
                    afemployee.TransType = "Group";

                    if (string.IsNullOrEmpty(lno))
                    {
                        NoSeries ns = new NoSeries();
                        ns.Code = deptcode;
                        ns.LastNo = "0000001";
                        db.NoSeries.Add(ns);
                        db.SaveChanges();
                    }
                    else
                    {

                        NoSeries ns2 = db.NoSeries.SingleOrDefault(v => v.Code == deptcode);
                        ns2.LastNo = str;
                        db.SaveChanges();
                    }


                    res.status = "success";

                }
                else
                {
                    afemployee.EmployeeID = arry[0].EmployeeID;
                    afemployee.TransType = "Individual";
                }
            }
            catch (Exception e)
            {
                res.status = "fail";
                res.message = e.Message.ToString();
            }
            //end new modification




            try
            {

                string lno = db.NoSeries.Where(i => i.Code == "EBC").Select(n => n.LastNo).FirstOrDefault();
                string s = lno;
                int number = Convert.ToInt32(s);

                number += 1;
                string str = number.ToString("D7");
                afemployee.EBCNo = "EBC-" + str;
                afemployee.UserID = db.Users.Where(a => a.Username == User.Identity.Name).Select(a => a.id).FirstOrDefault();
                afemployee.ContractorID = conid;
                afemployee.TransType = trans;
                db.AFBorrowers.Add(afemployee);
                db.SaveChanges();

                NoSeries ns = db.NoSeries.SingleOrDefault(v => v.Code == "EBC");
                ns.LastNo = str;
                db.SaveChanges();


                res.status = "success";
                Log log = new Log();
                log.descriptions = "Added new EBC Record. EBCNo:" + afemployee.EBCNo;
                db.Logs.Add(log);
                db.SaveChanges();

                int getid = afemployee.id;
                res.message = getid.ToString();


                //return RedirectToAction("Edit", new { id = getid });
            }
            catch (DbEntityValidationException e)
            {

                string[] error = new string[20];
                int i = 0;
                foreach (var eve in e.EntityValidationErrors)
                {
                    //Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                    // eve.Entry.Entity.GetType().Name, eve.Entry.State);

                    foreach (var ve in eve.ValidationErrors)
                    {
                        error[i] = "Property: " + ve.PropertyName + ", Error: " + ve.ErrorMessage;



                        i++;
                    }
                }
                res.status = "fail";
                res.message = e.Message.ToString();
            }

            return Json(res);

        }

        // GET: /AFEmployee/Edit/5
        public ActionResult Edit(int? id, int? user)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AFBorrower af = db.AFBorrowers.Find(id);
            if (af == null)
            {
                return HttpNotFound();
            }




            var emp = db.Employees.Select(e => new SelectListItem
            {
                Value = e.id.ToString(),
                Text = e.EntryType == "Individual" ? e.LastName + "," + e.FirstName : e.FirstName

            });

            if (af.TransType == "Group")
            {
                ViewBag.GroupName = db.GroupAccountabilityMembers.Where(c => c.EmployeeID == af.EmployeeID)
                                                           .Select(c => c.GroupAccountabilities.Code).FirstOrDefault();
            }
            else
            {
                ViewBag.GroupName = "";
            }


            ViewBag.ContractorID = af.ContractorID;
            ViewBag.EmployeeID = new SelectList(emp, "Value", "Text", af.EmployeeID);
            ViewBag.ID = id;
            ViewBag.DocStatus = af.DocStatus;
            ViewBag.UserType = user;

            return View(af);
        }

        // POST: /AFEmployee/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,EBCNo,EmployeeID")] AFBorrower afemployee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(afemployee).State = EntityState.Modified;
                db.SaveChanges();

                Log log = new Log();
                log.descriptions = "Update EBC Record. EBCNo:" + afemployee.EBCNo;
                db.Logs.Add(log);
                db.SaveChanges();
                ViewBag.ID = afemployee.id;
                return RedirectToAction("Index");
            }
            ViewBag.ID = afemployee.id;
            return View(afemployee);
        }

        // GET: /AFEmployee/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AFBorrower afemployee = db.AFBorrowers.Find(id);
            if (afemployee == null)
            {
                return HttpNotFound();
            }
            return View(afemployee);
        }

        // POST: /AFEmployee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AFBorrower afemployee = db.AFBorrowers.Find(id);
            afemployee.Status = "Deleted";
            db.Entry(afemployee).State = EntityState.Modified;
            db.SaveChanges();

            Log log = new Log();
            log.descriptions = "Deleted EBC Record. EBCNO id:" + id;
            db.Logs.Add(log);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult GetEmpDetails(int id)
        {
            JsonArray res = new JsonArray();
            var emp = db.Employees.Find(id);
            var model = new
            {
                EmployeeNo = emp.EmpId,
                Designations = emp.Designations.Name,
                Departments = emp.Departments.Name,
                GroupName = db.GroupAccountabilityMembers.Where(c => c.EmployeeID == id)
                            .Select(c => c.GroupAccountabilities.Code).FirstOrDefault(),
                status = "success"
            };
            return Json(model, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public ActionResult GetItemDetails(int id)
        {

            JsonArray res = new JsonArray();
            var item = db.Items.Find(id);
            var model = new
            {
                SerialNo = item.SerialNo,
                UoM = item.UOM,
                Category = item.Category,
                status = "success",
            };
            return Json(model, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public ActionResult GetNoSeries(string strcode)
        {
            var noseries = db.NoSeries.Select(n => new
            {
                code = n.Code,
                text = n.LastNo,
                status = "success"
            }).Where(n => n.code == strcode);

            return Json(noseries, JsonRequestBehavior.AllowGet);
        }




        public JsonResult GetAFEmpDetails(int offset, int limit, string search, string sort, string order, int afid, string filter)
        {
            var totalcount = 0;
            var model = db.AFBorrowerIssues.Select(b => new
            {
                b.id,
                b.AFBorrowerID,
                b.ItemID,
                b.Items.ItemCode,
                b.Items.Description,
                b.SerialNo,
                b.PO,
                b.Quantity,
                b.UoM,
                UnitCost = b.UnitCost.ToString(),
                b.Amount,
                DateIssued = b.DateIssued.Month.ToString() + "/" + b.DateIssued.Day.ToString() + "/" + b.DateIssued.Year.ToString(),
                DueDate = b.DueDate.Month.ToString() + "/" + b.DueDate.Day.ToString() + "/" + b.DueDate.Year.ToString(),
                b.AFBorrowers.ContractorID,
                ContractorName = b.AFBorrowers.Contractors.Name,
                b.Status,
                b.Remarks,
                b.WorkOrder,
                Created_At = b.Created_At,
                qtyret = db.AFBorrowerReturns
                        .Where(a => a.Status == "Active")
                        .Where(a => a.AFBorrowerIssueID == b.id)
                        .Select(a => a.Quantity)
                        .DefaultIfEmpty(0)
                        .Sum(),
                rembal = b.Quantity - b.QuantityTransferred - db.AFBorrowerReturns
                        .Where(a => a.Status == "Active")
                        .Where(a => a.AFBorrowerIssueID == b.id)
                        .Select(a => a.Quantity)
                        .DefaultIfEmpty(0)
                        .Sum(),
                qtytrans = b.QuantityTransferred,

            })
            .Where(b => b.Status == "Active" || b.Status == "Transferred" || b.Status == "Cancelled");
            model = model.Where(i => i.AFBorrowerID == afid).OrderBy(i => i.id);

            //if (!string.IsNullOrEmpty(search))
            //{
            //    model = model.Where(b => b.PO.Contains(search) || b.ItemCode.Contains(search) || b.Description.Contains(search) || b.SerialNo.Contains(search));
            //}



            //dynamic filter starts here
            string o = "";
            string q = filter;
            string strFilter = "";



            if (!string.IsNullOrEmpty(q))
            {
                q = filter.Replace("\\", "");
                string[] res = new string[(q.Length) - 1];

                if (q.Contains(","))
                {
                    var elements = q.Split(',');
                    int j = 0;
                    for (int i = 0; i < elements.Count(); i++)
                    {
                        o = "";
                        o = elements[i].Replace(':', '=').Replace("{", "").Replace("}", "").Replace(@"\", "");
                        string colSearch = o.Before("=").Replace('"', ' ').Replace(" ", "");
                        string colval = o.After("=");

                        if (colSearch == "Quantity" || colSearch == "qtyret" || colSearch == "qtytrans")
                        {
                            colval = colval.Replace("\"", ""); //remove escape character "\"
                        }


                        if (!string.IsNullOrEmpty(strFilter))
                        {
                            strFilter = (colSearch != "Quantity" & colSearch != "qtyret" & colSearch != "qtytrans") ? (strFilter + " && " + colSearch + ".Contains(" + colval + ")") : (strFilter + " && " + "(" + colSearch + "=" + colval + ")");
                        }
                        else
                        {
                            strFilter = (colSearch != "Quantity" & colSearch != "qtyret" & colSearch != "qtytrans") ? (colSearch + ".Contains(" + colval + ")") : "(" + colSearch + "=" + colval + ")";

                        }

                    }

                }

                else
                {

                    o = "";
                    o = q.Replace(':', '=').Replace("{", "").Replace("}", "").Replace(@"\", "");
                    string colSearch = o.Before("=").Replace('"', ' ').Replace(" ", "");
                    string colval = o.After("=");
                    if (colSearch == "Quantity" || colSearch == "qtyret" || colSearch == "qtytrans")
                    {
                        colval = colval.Replace("\"", ""); //remove escape character "\"
                    }


                    strFilter = (colSearch != "Quantity" & colSearch != "qtyret" & colSearch != "qtytrans") ? (colSearch + ".Contains(" + colval + ")") : "(" + colSearch + "=" + colval + ")";
                }

            }
            else
            {
                if (!string.IsNullOrEmpty(search))
                {
                    model = model.Where(b => b.PO.Contains(search) || b.ItemCode.Contains(search)
                        || b.Description.Contains(search) || b.SerialNo.Contains(search));
                }
            }

            string filterQuery = strFilter;
            //dynamic filter ends here

            if (!string.IsNullOrEmpty(filterQuery))
            {
                ///dynamic LINQ
                model = model.Where(filterQuery);


            }





            totalcount = model.Count();
            model = model.OrderBy(i => i.id).Skip(offset).Take(limit);

            var modelItem = new
            {
                total = totalcount,
                rows = model.ToList(),
            };
            return Json(modelItem, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAFEmpDetailsReturn(int offset, int limit, string search, string sort, string order, int afid)
        {
            var totalcount = 0;
            var model = db.AFBorrowerReturns
                .Include(r => r.AFBorrowerIssues)
                .Select(b => new
                {
                    b.id,
                    b.AFBorrowerIssues.AFBorrowerID,
                    b.AFBorrowerIssueID,
                    b.AFBorrowerIssues.Items.ItemCode,
                    b.AFBorrowerIssues.Items.Description,
                    b.AFBorrowerIssues.SerialNo,
                    b.AFBorrowerIssues.PO,
                    b.Quantity,
                    b.AFBorrowerIssues.UoM,

                    UnitCost = b.AFBorrowerIssues.UnitCost.ToString(),
                    b.DateReturned,
                    b.Remarks,
                    b.ToolStatus,
                    b.Status,
                    b.DocStatus
                }
            ).Where(b => b.Status == "Active");
            model = model.Where(i => i.AFBorrowerIssueID == afid).OrderBy(i => i.id);

            if (!string.IsNullOrEmpty(search))
            {
                model = model.Where(b => b.PO.Contains(search));
            }
            totalcount = model.Count();
            model = model.OrderBy(i => i.id).Skip(offset).Take(limit);

            var modelItem = new
            {
                total = totalcount,
                rows = model.ToList(),
            };
            return Json(modelItem, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult ChangeDocumentStatus(int id, int status)
        {
            int old_status = 0;

            JsonArray res = new JsonArray();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                AFBorrower afemployee = db.AFBorrowers.Find(id);
                old_status = afemployee.DocStatus;
                if (afemployee == null)
                {
                    return HttpNotFound();
                }

                afemployee.DocStatus = status;
                if (status == 1)
                {
                    afemployee.DateReleased = DateTime.Now.Date;
                }
                db.Entry(afemployee).State = EntityState.Modified;
                db.SaveChanges();

                res.status = "success";

            }
            catch (Exception e)
            {
                res.message = e.Message;
                res.status = "fail";

            }



            Log log = new Log();
            log.descriptions = "Modified record id:" + id + " Table [AFBorrower]";
            log.otherinfo = "Old Stat : " + old_status + "New Stat : " + status;
            db.Logs.Add(log);
            db.SaveChanges();


            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllAFEmpDetailsReturn(int offset, int limit, string search, string sort, string order, int afid)
        {

            var model = db.AFBorrowerReturns.Include(r => r.AFBorrowerIssues).Select(b => new
            {
                b.id,
                b.AFBorrowerIssues.AFBorrowerID,
                b.AFBorrowerIssueID,
                b.AFBorrowerIssues.Items.ItemCode,
                b.AFBorrowerIssues.Items.Description,
                b.AFBorrowerIssues.SerialNo,
                b.AFBorrowerIssues.PO,
                b.Quantity,
                b.AFBorrowerIssues.UoM,

                UnitCost = b.AFBorrowerIssues.UnitCost.ToString(),
                b.DateReturned,
                b.Remarks,
                b.ToolStatus,
                b.Status,
                b.DocStatus
            }
            ).Where(b => b.Status == "Active");
            model = model.Where(i => i.AFBorrowerID == afid).OrderBy(i => i.id);
            //model = model.Where(i => i.AFBorrowerIssueID == afid).OrderBy(i => i.id);


            int cnt = model.Count();

            if (!string.IsNullOrEmpty(search))
            {
                model = model.Where(b => b.PO.Contains(search));
            }

            model = model.OrderBy(i => i.id).Skip(offset).Take(limit);

            var modelItem = new
            {
                total = model.Count(),
                rows = model.ToList(),
            };
            return Json(modelItem, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PrintEBC(string EBCNo, int EmpID)
        {
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();

            //string code = db.Employees.Where(e => e.id == EmpID).Select(e => e.EmpId).FirstOrDefault();

            //var v = db.GroupAccountabilityMembers.Where(g => g.GroupAccountabilities.Code == code).Select(b => new
            //{
            //    EmpID = b.Employees.EmpId,
            //    EmployeeName = b.Employees.LastName + ", " + b.Employees.FirstName
            //}).ToList();


            //int code = db.GroupAccountabilityMembers.Where(e => e.id == EmpID).Select(e => e.GroupAccountabilityID).FirstOrDefault();
            string code = db.Employees.Find(EmpID).EmpId;
            var v = db.GroupAccountabilityMembers.Where(c => c.GroupAccountabilities.Status == "Active").Where(c => c.GroupAccountabilities.Code == code)
                                            .Select(c => new
                                            {
                                                EmpID = c.Employees.EmpId,
                                                EmployeeName = c.Employees.LastName + ", " + c.Employees.FirstName
                                            }).ToList();



            dt2 = new ReportController().ToDataTable(v);



            //PDF VERSION

            Warning[] warnings;
            string mimeType;
            string[] streamids;
            string encoding;
            string filenameExtension;

            var viewer = new ReportViewer();
            viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\EBC.rdlc";
            dt = new ReportController().PrintEBC(EBCNo);
            viewer.LocalReport.DataSources.Add(new ReportDataSource("EBC", dt));
            viewer.LocalReport.DataSources.Add(new ReportDataSource("EBCMembers", dt2));
            viewer.LocalReport.Refresh();

            var bytes = viewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

            return new FileContentResult(bytes, mimeType);

        }
        public ActionResult PrintEBCReturn(string EBCNo)
        {
            //PDF VERSION
            DataTable dt = new DataTable();
            Warning[] warnings;
            string mimeType;
            string[] streamids;
            string encoding;
            string filenameExtension;

            var viewer = new ReportViewer();
            viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\EBCReturn.rdlc";
            dt = new ReportController().PrintEBC(EBCNo);
            viewer.LocalReport.DataSources.Add(new ReportDataSource("EBC", dt));
            viewer.LocalReport.Refresh();

            var bytes = viewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

            return new FileContentResult(bytes, mimeType);

        }
        [HttpPost]
        public ActionResult CancelAC(int itemid, string remarks)
        {
            JsonArray res = new JsonArray();
            if (itemid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AFBorrower af = db.AFBorrowers.Find(itemid);
            var old_status = af.DocStatus;

            if (af == null)
            {
                return HttpNotFound();
            }

            af.DocStatus = 3;
            db.Entry(af).State = EntityState.Modified;
            db.SaveChanges();



            var afdetails = db.AFBorrowerIssues
                .Where(e => e.AFBorrowerID == itemid)
                .Where(a => a.Status == "Active")
                .ToList();

            afdetails
                .ForEach(a => a.Status = "Cancelled");
            afdetails
                .ForEach(a => a.Remarks = remarks);

            db.SaveChanges();




            Log log = new Log();
            log.descriptions = "Cancelled record id:" + itemid + " Table [AFBorrower]";
            log.otherinfo = "Old Stat : " + old_status + "New Stat : " + 3;
            db.Logs.Add(log);
            db.SaveChanges();

            res.status = "success";
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UpdatePurpose(int id, string purpose)
        {
            JsonArray res = new JsonArray();
            if (purpose == "")
            {
                res.status = "failed";
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            if (ModelState.IsValid)
            {
                AFBorrower ae = db.AFBorrowers.Find(id);
                ae.Purpose = purpose;
                db.Entry(ae).State = EntityState.Modified;
                db.SaveChanges();


                Log log = new Log();
                log.descriptions = "Update EBC Record. EBCNo:" + ae.EBCNo;
                db.Logs.Add(log);
                db.SaveChanges();

                res.status = "success";
            }
            else
            {
                res.status = "failed";
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UpdateDueDate(int id, string duedate)
        {
            DateTime due = DateTime.Parse(duedate);
            JsonArray res = new JsonArray();

            if (ModelState.IsValid)
            {
                AFBorrower ae = db.AFBorrowers.Find(id);
                ae.DueDate = due;
                db.Entry(ae).State = EntityState.Modified;


                var abe = db.AFBorrowerIssues.Where(a => a.AFBorrowerID == id);
                foreach (var item in abe)
                {
                    item.DueDate = due;
                    db.Entry(item).State = EntityState.Modified;
                }

                db.SaveChanges();





                Log log = new Log();
                log.descriptions = "Update EBC Record. EBCNo:" + ae.EBCNo;
                db.Logs.Add(log);
                db.SaveChanges();

                res.status = "success";
            }
            else
            {
                res.status = "failed";
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getData(int draw, int start, int length, string strcode, int noCols)
        {

            var sortColumn = Request["order[0][column]"];
            var sortColumnDir = Request["order[0][dir]"];
            var searchValue = Request["search[value]"];
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsFiltered = 0;
            int recordsTotal = 0;

            //var v = db.AFBorrowerIssues
            //    .Where(b => b.Status == "Active" || b.Status == "Transferred" || b.Status == "Cancelled")
            //    .Select(b => new
            //{
            //    b.AFBorrowers.id,
            //    RefNo = b.AFBorrowers.EBCNo,
            //    EmployeeName = b.AFBorrowers.Employees.LastName + ", " + b.AFBorrowers.Employees.FirstName,
            //    b.ItemID,
            //    b.Items.ItemCode,
            //    b.Items.Description,
            //    b.Items.Description2,
            //    DateCreated = b.AFBorrowers.Created_At,
            //    IssuedQty = b.Quantity,
            //    Status = b.AFBorrowers.DocStatus == 0 ? "Open" : (b.AFBorrowers.DocStatus == 1 ? "Released" : (b.AFBorrowers.DocStatus == 2 ? "Closed" : "Cancelled"))
            //});

            var v = db.AFBorrowers
    .Where(b => b.Status == "Active" || b.Status == "Transferred" || b.Status == "Cancelled")
    .GroupJoin(db.AFBorrowerIssues
                        , i => i.id
                        , r => r.AFBorrowerID,
                        (i, r) => new
                        {
                            Header = i,
                            Details = r.DefaultIfEmpty()
                        })
                        .SelectMany(
                         a => a.Details
                             .Select(b =>
                            new
                            {
                                //ItemID = a.Header.id,
                                a.Header.id,
                                RefNo = a.Header.EBCNo,
                                //EmployeeName = a.Header.Employees.LastName + "," + a.Header.Employees.FirstName,
                                EmployeeName = a.Header.Employees.EntryType == "Individual" ? a.Header.Employees.LastName + "," + a.Header.Employees.FirstName : a.Header.Employees.FirstName,
                                ItemID = b.ItemID == null ? 0 : b.ItemID,
                                ItemCode = b.Items.ItemCode == null ? "" : b.Items.ItemCode,
                                Description = b.Items.Description == null ? "" : b.Items.Description,
                                Description2 = b.Items.Description2 == null ? "" : b.Items.Description2,
                                DateCreated = a.Header.Created_At,
                                IssuedQty = b.Quantity == null ? 0 : b.Quantity,
                                Status = a.Header.DocStatus == 0 ? "Open" : (a.Header.DocStatus == 1 ? "Released" : (a.Header.DocStatus == 2 ? "Closed" : "Cancelled"))
                            }
                        ));

            recordsTotal = v.Count();

            if (!string.IsNullOrEmpty(searchValue))
            {
                v = v.Where(a =>
                    a.RefNo.ToUpper().Contains(searchValue.ToUpper()) ||
                    a.EmployeeName.ToString().Contains(searchValue.ToUpper()) ||
                    a.ItemCode.ToString().Contains(searchValue.ToUpper()) ||
                    a.Description.ToString().Contains(searchValue.ToUpper()) ||
                    a.Description2.ToString().Contains(searchValue.ToUpper()) ||
                    a.DateCreated.ToString().Contains(searchValue.ToUpper()) ||
                    a.IssuedQty.ToString().Contains(searchValue.ToUpper()) ||
                    a.Status.ToString().Contains(searchValue.ToUpper())
                );
            }


            DateTime dval = new DateTime();
            Boolean hasDateCreated = false;

            string strFilter = "";
            for (int i = 0; i < noCols; i++)
            {
                string colval = Request["columns[" + i + "][search][value]"];
                if (colval != "")
                {
                    string colSearch = Request["columns[" + i + "][data]"];
                    if (colSearch == "DateCreated")
                    {
                        dval = DateTime.Parse(colval);
                        hasDateCreated = true;
                    }
                    else
                    {

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
                    }
                    if (strFilter != "")
                    {
                        v = v.Where(strFilter);
                    }
                }
            }

            if (hasDateCreated)
            {
                v = v.Where("DateCreated = @0", dval);
            }

            recordsFiltered = v.Count();


            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                string col = Request["columns[" + sortColumn + "][data]"];
                v = v.OrderBy(col + " " + sortColumnDir);
            }







            var x = v.ToList();
            if (pageSize < 0)
            {
                pageSize = recordsFiltered;
            }
            var allCustomer = v.Skip(skip).Take(pageSize).ToList();



            var model = new
            {
                draw = draw,
                recordsFiltered = recordsFiltered,
                recordsTotal = recordsTotal,
                data = allCustomer
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getGroupMembers(int draw, int start, int length, int strcode)
        {

            var sortColumn = Request["order[0][column]"];
            var sortColumnDir = Request["order[0][dir]"];
            var searchValue = Request["search[value]"];
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsFiltered = 0;
            int recordsTotal = 0;

            //int code = db.Employees.Where(e=>e.id == strcode).Select(e=>e.DepartmentID).FirstOrDefault();
            string code = db.Employees.Where(e => e.id == strcode).Select(e => e.EmpId).FirstOrDefault();



            var v = db.GroupAccountabilityMembers.Where(g => g.GroupAccountabilities.Code == code).Select(b => new
            {
                EmpID = b.Employees.EmpId,
                EmployeeName = b.Employees.LastName + ", " + b.Employees.FirstName
            });



            recordsTotal = v.Count();

            if (!string.IsNullOrEmpty(searchValue))
            {
                v = v.Where(a =>
                    a.EmployeeName.ToUpper().Contains(searchValue.ToUpper())
                );
            }


            recordsFiltered = v.Count();


            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                string col = Request["columns[" + sortColumn + "][data]"];
                v = v.OrderBy(col + " " + sortColumnDir);
            }







            var x = v.ToList();
            if (pageSize < 0)
            {
                pageSize = recordsFiltered;
            }
            var allCustomer = v.Skip(skip).Take(pageSize).ToList();



            var model = new
            {
                draw = draw,
                recordsFiltered = recordsFiltered,
                recordsTotal = recordsTotal,
                data = allCustomer
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SetToRelease(int id)
        {
            JsonArray res = new JsonArray();
            try
            {
                AFBorrower ae = db.AFBorrowers.Find(id);
                ae.DocStatus = 1;
                db.Entry(ae).State = EntityState.Modified;

                Log log = new Log();
                log.descriptions = "Change EBC Record Status to Released [ Admin ]. EBCNo:" + ae.EBCNo;
                db.Logs.Add(log);

                db.SaveChanges();
                res.status = "success";
            }
            catch (Exception)
            {
                res.status = "fail";
                throw;
            }
            return Json(res, JsonRequestBehavior.AllowGet);

        }
        public ActionResult dataAccountabilityIndex(int draw, int start, int length, string strcode, int noCols)
        {
            var sortColumn = Request["order[0][column]"];
            var sortColumnDir = Request["order[0][dir]"];
            var searchValue = Request["search[value]"];
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsFiltered = 0;
            int recordsTotal = 0;

            var v = db.AFBorrowers
                .Where(b => b.Status == "Active" || b.Status == "Transferred" || b.Status == "Cancelled")
                .Select(b => new
                {
                    b.id
                   ,
                    RefNo = b.EBCNo
                   ,
                    //EmployeeName = b.TransType == "Individual" ? b.Employees.LastName + "," + b.Employees.FirstName :
                    //            db.GroupAccountabilityMembers.Where(c => c.EmployeeID == b.EmployeeID)
                    //                        .Select(c => c.GroupAccountabilities.Code).FirstOrDefault()
                    EmployeeName = b.Employees.EntryType == "Individual" ? b.Employees.LastName + "," + b.Employees.FirstName : b.Employees.FirstName

                   ,
                    EmployeeNo = b.Employees.EmpId
                    ,
                    Member = b.Employees.EntryType == "Individual" ? "" : (db.VGroupMembers.Where(c => c.GroupCode == b.Employees.EmpId).Select(c => c.Members).FirstOrDefault())
                   ,

                    DesignationName = b.Employees.Designations.Name
                   ,
                    DepartmentName = b.Employees.Departments.Name
                   ,
                    DateCreated = b.Created_At
                   ,
                    Status = b.DocStatus == 0 ? "OPEN" : (b.DocStatus == 1 ? "RELEASED" : (b.DocStatus == 2 ? "CLOSED" : "CANCELLED"))
                });



            //for dashboard that search automatic for OPEN Docs
            if (TempData["Status"] != null)
            {
                TempData.Keep("Status");
                v = v.Where(a => a.Status == "OPEN");

            }






            recordsTotal = v.Count();


            if (!string.IsNullOrEmpty(searchValue))
            {
                v = v.Where(a =>
                    a.RefNo.ToUpper().Contains(searchValue.ToUpper()) ||
                    a.EmployeeName.ToString().Contains(searchValue.ToUpper()) ||
                    a.EmployeeNo.ToString().Contains(searchValue.ToUpper()) ||
                    a.Member.ToString().Contains(searchValue.ToUpper()) ||
                    a.DesignationName.ToString().Contains(searchValue.ToUpper()) ||
                    a.DepartmentName.ToString().Contains(searchValue.ToUpper()) ||
                    a.DateCreated.ToString().Contains(searchValue.ToUpper()) ||
                    a.Status.ToString().Contains(searchValue.ToUpper())
                );
            }


            DateTime dval = new DateTime();
            Boolean hasDateCreated = false;

            string strFilter = "";
            for (int i = 0; i < noCols; i++)
            {
                string colval = Request["columns[" + i + "][search][value]"];
                if (colval != "")
                {
                    string colSearch = Request["columns[" + i + "][data]"];
                    if (colSearch == "DateCreated")
                    {
                        dval = DateTime.Parse(colval);
                        hasDateCreated = true;
                    }
                    else
                    {
                        if (strFilter == "")
                        {
                            if (colval == "*")
                            {
                                strFilter = "(" + colSearch + " != " + "" + ")";
                            }
                            else
                            {

                                strFilter = colSearch + ".Contains(" + "\"" + colval + "\"" + ")";
                            }
                        }
                        else
                        {
                            strFilter = strFilter + " && " + colSearch + ".Contains(" + "\"" + colval + "\"" + ")";
                        }
                    }
                    if (strFilter != "")
                    {
                        v = v.Where(strFilter);
                    }
                }
            }

            if (hasDateCreated)
            {
                v = v.Where("DateCreated = @0", dval);
            }

            recordsFiltered = v.Count();


            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                string col = Request["columns[" + sortColumn + "][data]"];
                v = v.OrderBy(col + " " + sortColumnDir);
            }


            var x = v.ToList();
            if (pageSize < 0)
            {
                pageSize = recordsFiltered;
            }
            var allCustomer = v.Skip(skip).Take(pageSize).ToList();

            var model = new
            {
                draw = draw,
                recordsFiltered = recordsFiltered,
                recordsTotal = recordsTotal,
                data = allCustomer
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}
