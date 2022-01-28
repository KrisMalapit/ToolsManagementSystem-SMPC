
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
using Newtonsoft.Json;
using PagedList;
using Microsoft.Reporting.WebForms;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using ToolsManagementSystem.Reports;
using ToolsManagementSystem.Models.View_Model;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq.Dynamic;
using Microsoft.AspNet.Identity;
using static ToolsManagementSystem.Models.View_Model.GroupViewModel;
using System.Data.Entity.Validation;

namespace ToolsManagementSystem.Controllers
{
    [CustomAuthorize]

    public class AFEmployeeController : Controller
    {
        private ToolManagementContext db = new ToolManagementContext();
        private int dataCount;
        //private const int TOTAL_ROWS = 995;


        // GET: /AFEmployee/
        public ActionResult Index(string sortOrder, int? page, string searchString, string searchByStatus)
        {
            TempData.Remove("Status");
            if (!string.IsNullOrEmpty(searchByStatus))
            {
                TempData["Status"] = searchByStatus;
            }

            return View();
        }

        public ActionResult Index_Warehouse()
        {
            return View();

        }




        // GET: /AFEmployee/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AFEmployee afemployee = db.AFEmployees.Find(id);
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
        public ActionResult Create(GroupMemberViewModel[] _arry,string trans,string _purpose, AFEmployee afemployee)
        {

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


                    foreach (var item in _arry)
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
                    afemployee.EmployeeID = _arry[0].EmployeeID;
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
                //new setup for num series
                string lno = db.NoSeries.Where(i => i.Code == "EAC").Select(n => n.LastNo).FirstOrDefault();
                string s = lno;
                int number = Convert.ToInt32(s);

                number += 1;
                string str = number.ToString("D7");

                afemployee.EACNo = "EAC-" + str;
                afemployee.UserID = db.Users.Where(a => a.Username == User.Identity.Name).Select(a => a.id).FirstOrDefault();
                afemployee.Purpose = _purpose;
                db.AFEmployees.Add(afemployee);
                db.SaveChanges();

                NoSeries ns = db.NoSeries.SingleOrDefault(v => v.Code == "EAC");
                ns.LastNo = str;
                db.SaveChanges();


                res.status = "success";
                Log log = new Log();
                log.descriptions = "Added new EAC Record. EACNo:" + afemployee.EACNo;
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
            AFEmployee afemployee = db.AFEmployees.Find(id);
            if (afemployee == null)
            {
                return HttpNotFound();
            }



            var emp = db.Employees.Select(e => new SelectListItem
            {
                Value = e.id.ToString(),
                Text = e.EntryType == "Individual" ? e.LastName + "," + e.FirstName : e.FirstName
            });

            if (afemployee.TransType == "Group")
            {
                ViewBag.GroupName = db.GroupAccountabilityMembers.Where(c => c.EmployeeID == afemployee.EmployeeID)
                                                           .Select(c => c.GroupAccountabilities.Code).FirstOrDefault();
            }
            else
            {
                ViewBag.GroupName = "";
            }


            ViewBag.EmployeeID = new SelectList(emp, "Value", "Text", afemployee.EmployeeID);
            ViewBag.ID = id;
            ViewBag.DocStatus = afemployee.DocStatus;
            //ViewBag.Columns = AccountabilityFilterColumns();
            ViewBag.UserType = user;
            return View(afemployee);
        }

        // POST: /AFEmployee/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,EACNo,EmployeeID,Purpose")] AFEmployee afemployee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(afemployee).State = EntityState.Modified;
                db.SaveChanges();

                Log log = new Log();
                log.descriptions = "Update EAC Record. EACNo:" + afemployee.EACNo;
                db.Logs.Add(log);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(afemployee);
        }

        // GET: /AFEmployee/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AFEmployee afemployee = db.AFEmployees.Find(id);
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
            AFEmployee afemployee = db.AFEmployees.Find(id);
            afemployee.Status = "Deleted";
            db.Entry(afemployee).State = EntityState.Modified;
            db.SaveChanges();

            Log log = new Log();
            log.descriptions = "Deleted EAC Record. EACNO id:" + id;
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
                status = "success",
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


            //string lno = db.NoSeries.Select(n =>n.LastNo).FirstOrDefault();
            //string s = lno;
            //int number = Convert.ToInt32(s);
            //number += 1;
            //string str = number.ToString("D7");

            //NoSeries ns = db.NoSeries.SingleOrDefault(v => v.Code == strcode);
            //ns.LastNo = str;
            //db.SaveChanges();

            return Json(noseries, JsonRequestBehavior.AllowGet);
        }




        public JsonResult GetAFEmpDetails(int offset, int limit, string search, string sort, string order, int afid, string filter)
        {
            var totalcount = 0;
            var model = db.AFEmployeeIssues.Select(b => new
            {
                b.id,
                b.AFEmployeeID,
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
                b.Status,
                b.Remarks,
                Created_At = b.Created_At,
                qtyret = db.AFEmployeeReturns
                        .Where(a => a.Status == "Active")
                        .Where(a => a.AFEmployeeIssueID == b.id)
                        .Select(a => a.Quantity)
                        .DefaultIfEmpty(0)
                        .Sum(),
                rembal = b.Quantity - b.QuantityTransferred - db.AFEmployeeReturns
                        .Where(a => a.Status == "Active")
                        .Where(a => a.AFEmployeeIssueID == b.id)
                        .Select(a => a.Quantity)
                        .DefaultIfEmpty(0)
                        .Sum(),
                qtytrans = b.QuantityTransferred,
                b.AccountCode

            })
            .Where(b => b.Status == "Active" || b.Status == "Transferred" || b.Status == "Cancelled");
            model = model.Where(i => i.AFEmployeeID == afid).OrderBy(i => i.id);

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

                        if (colSearch == "Quantity" || colSearch == "qtyret"|| colSearch == "qtytrans")
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
            var model = db.AFEmployeeReturns.Include(r => r.AFEmployeeIssues).Select(b => new
            {
                b.id,
                b.AFEmployeeIssues.AFEmployeeID,
                b.AFEmployeeIssueID,
                b.AFEmployeeIssues.Items.ItemCode,
                b.AFEmployeeIssues.Items.Description,
                b.AFEmployeeIssues.SerialNo,
                b.AFEmployeeIssues.PO,
                b.Quantity,
                b.AFEmployeeIssues.UoM,

                UnitCost = b.AFEmployeeIssues.UnitCost.ToString(),
                b.DateReturned,
                b.Remarks,
                b.ToolStatus,
                b.Status
                ,
                b.DocStatus
            }
            ).Where(b => b.Status == "Active");
            model = model.Where(i => i.AFEmployeeIssueID == afid).OrderBy(i => i.id);

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
                AFEmployee afemployee = db.AFEmployees.Find(id);
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
            log.descriptions = "Modified record id:" + id + " Table [AFEmployee]";
            log.otherinfo = "Old Stat : " + old_status + "New Stat : " + status;
            db.Logs.Add(log);
            db.SaveChanges();

            
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllAFEmpDetailsReturn(int offset, int limit, string search, string sort, string order, int afid)
        {

            var model = db.AFEmployeeReturns.Include(r => r.AFEmployeeIssues).Select(b => new
            {
                b.id,
                b.AFEmployeeIssues.AFEmployeeID,
                b.AFEmployeeIssueID,
                b.AFEmployeeIssues.Items.ItemCode,
                b.AFEmployeeIssues.Items.Description,
                b.AFEmployeeIssues.SerialNo,
                b.AFEmployeeIssues.PO,
                b.Quantity,
                b.AFEmployeeIssues.UoM,
                UnitCost = b.AFEmployeeIssues.UnitCost.ToString(),
                b.DateReturned,
                b.Remarks,
                b.ToolStatus,
                b.Status
                ,
                b.DocStatus
            }
            ).Where(b => b.Status == "Active");

            model = model.Where(i => i.AFEmployeeID == afid).OrderBy(i => i.id);
            //model = model.Where(i => i.AFEmployeeIssueID == afid).OrderBy(i => i.id);

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

        public ActionResult PrintEAC(string EACNo, int EmpID)
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
            viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\EAC.rdlc";
            dt = new ReportController().PrintEAC(EACNo); 

            viewer.LocalReport.DataSources.Add(new ReportDataSource("AFEmp", dt));
            viewer.LocalReport.DataSources.Add(new ReportDataSource("EBCMembers", dt2));
            viewer.LocalReport.Refresh();

            var bytes = viewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

            return new FileContentResult(bytes, mimeType);

        }


        public ActionResult PrintEACReturn(string EACNo)
        {

            //PDF VERSION
            DataTable dt = new DataTable();
            Warning[] warnings;
            string mimeType;
            string[] streamids;
            string encoding;
            string filenameExtension;

            var viewer = new ReportViewer();
            viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\EACReturn.rdlc";
            dt = new ReportController().PrintEAC(EACNo);

            viewer.LocalReport.DataSources.Add(new ReportDataSource("AFEmp", dt));
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

            AFEmployee afemployee = db.AFEmployees.Find(itemid);

            var old_status = afemployee.DocStatus;
            if (afemployee == null)
            {
                return HttpNotFound();
            }

            afemployee.DocStatus = 3;
            db.Entry(afemployee).State = EntityState.Modified;
            db.SaveChanges();



            var afdetails = db.AFEmployeeIssues
                .Where(e => e.AFEmployeeID == itemid)
                .Where(a => a.Status == "Active")
                .ToList();

            afdetails.ForEach(a => a.Status = "Cancelled");
            afdetails.ForEach(a => a.Remarks = remarks);
            db.SaveChanges();




            Log log = new Log();
            log.descriptions = "Cancelled record id:" + itemid + " Table [AFEmployee]";
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


                AFEmployee ae = db.AFEmployees.Find(id);
                ae.Purpose = purpose;
                db.Entry(ae).State = EntityState.Modified;
                db.SaveChanges();


                Log log = new Log();
                log.descriptions = "Update EAC Record. EACNo:" + ae.EACNo;
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


            var v = db.AFEmployees
                .Where(b => b.Status == "Active" || b.Status == "Transferred" || b.Status == "Cancelled")
                .GroupJoin(db.AFEmployeeIssues.Where(b => b.Status == "Active" || b.Status == "Transferred")
                        , i => i.id
                        , r => r.AFEmployeeID,
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
                                a.Header.id,
                                RefNo = a.Header.EACNo,
                                EmployeeName = a.Header.Employees.EntryType == "Individual" ? a.Header.Employees.LastName + "," + a.Header.Employees.FirstName : a.Header.Employees.FirstName,
                                ItemID = b.ItemID == null ? 0 : b.ItemID,
                                ItemCode = b.Items.ItemCode == null ? "" : b.Items.ItemCode,
                                Description = b.Items.Description == null ? "" : b.Items.Description,
                                Description2 = b.Items.Description2 == null ? "" : b.Items.Description2,
                                DateCreated = a.Header.Created_At,
                                IssuedQty = b.Quantity== null ? 0 :b.Quantity,
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
                //for make sort simpler we will add System.Linq.Dynamic reference
                string col = Request["columns[" + sortColumn + "][data]"];
                v = v.OrderBy(col + " " + sortColumnDir);
            }


            var x = v.ToList();
            if (pageSize < 0)
            {
                pageSize = recordsFiltered;
            }
            var allCustomer = v.Skip(skip).Take(pageSize).ToList();



            //List<AccountabilityHeaderViewModel> list = new List<AccountabilityHeaderViewModel>();
            //foreach (var afemp in allCustomer)
            //{
            //    AccountabilityHeaderViewModel item = new AccountabilityHeaderViewModel();
            //    item.RefNo = afemp.RefNo;
            //    item.EmployeeName = afemp.EmployeeName;
            //    item.DateCreated = afemp.DateCreated.ToString("MM/dd/yyyy");
            //    item.ItemCode = afemp.ItemCode;
            //    item.Description = afemp.Description;
            //    item.Description2 = afemp.Description2;
            //    item.IssuedQty = afemp.IssuedQty;
            //    list.Add(item);
            //}




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
        public ActionResult SetToRelease(int id) {
            JsonArray res = new JsonArray();
            try
            {
                AFEmployee ae = db.AFEmployees.Find(id);
                ae.DocStatus = 1;
                db.Entry(ae).State = EntityState.Modified;

                Log log = new Log();
                log.descriptions = "Change EAC Record Status to Released [ Admin ]. EACNo:" + ae.EACNo;
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
                   
                }
            }

            if (strFilter == "")
            {
                strFilter = "true";
            }

            int recCount =
                db.AFEmployees
                .Where(b => b.Status == "Active" || b.Status == "Transferred" || b.Status == "Cancelled")
               
                .Select(b => new
                {
                    b.id
                   ,
                    RefNo = b.EACNo
                   ,
                    EmployeeName = b.Employees.EntryType == "Individual" ? b.Employees.LastName + "," + b.Employees.FirstName : b.Employees.FirstName
                   ,
                    EmployeeNo = b.Employees.EmpId
                   ,
                   DepartmentName = b.Employees.Departments.Name
                 
                   ,
                    Status = b.DocStatus == 0 ? "OPEN" : (b.DocStatus == 1 ? "RELEASED" : (b.DocStatus == 2 ? "CLOSED" : "CANCELLED"))
                })
                 .Where(strFilter)
                 .Count();

            recordsTotal = recCount;
            int recFilter = recCount;


            var v = db.AFEmployees
                .Where(b => b.Status == "Active" || b.Status == "Transferred" || b.Status == "Cancelled")
                .Select(b => new
                {
                   b.id
                   ,RefNo = b.EACNo
                   ,
                   EmployeeName = b.Employees.EntryType == "Individual" ? b.Employees.LastName + "," + b.Employees.FirstName : b.Employees.FirstName
                   ,
                    EmployeeNo = b.Employees.EmpId
                   
                   ,DepartmentName = b.Employees.Departments.Name
                   ,DateCreated = b.Created_At
                   ,
                   Status = b.DocStatus == 0 ? "OPEN" : (b.DocStatus == 1 ? "RELEASED" : (b.DocStatus == 2 ? "CLOSED" : "CANCELLED"))
                })
               .Where(strFilter)
                .OrderByDescending(b=>b.id)
                .Skip(skip).Take(pageSize);

          
          
            if (TempData["Status"] != null)
            {
                TempData.Keep("Status");
                v = v.Where(a => a.Status == "OPEN");
             
            }




            if (!string.IsNullOrEmpty(searchValue))
            {
                v = v.Where(a =>
                    a.RefNo.ToUpper().Contains(searchValue.ToUpper()) ||
                    a.EmployeeName.ToString().Contains(searchValue.ToUpper()) ||
                    a.EmployeeNo.ToString().Contains(searchValue.ToUpper()) ||
                    
                    a.DepartmentName.ToString().Contains(searchValue.ToUpper()) ||
                    a.DateCreated.ToString().Contains(searchValue.ToUpper()) ||
                    a.Status.ToString().Contains(searchValue.ToUpper())
                );
            }


          

           

            //if (hasDateCreated)
            //{
            //    v = v.Where("DateCreated = @0", dval);
            //}

            recordsFiltered = recFilter;


            //if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            //{
            //    string col = Request["columns[" + sortColumn + "][data]"];
            //    v = v.OrderBy(col + " " + sortColumnDir);
            //}


            var x = v.ToList();
            if (pageSize < 0)
            {
                pageSize = recordsFiltered;
            }
            //var allCustomer = v.Skip(skip).Take(pageSize).ToList();

            var model = new
            {
               draw,
                recordsFiltered,
                recordsTotal,
                data = v
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }


    }
}
