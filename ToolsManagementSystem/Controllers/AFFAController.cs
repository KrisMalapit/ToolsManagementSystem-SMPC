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
using Newtonsoft.Json;
using ToolsManagementSystem.Models.View_Model;
using System.Linq.Dynamic;
using Microsoft.AspNet.Identity;
using static ToolsManagementSystem.Models.View_Model.GroupViewModel;
using System.Data.Entity.Validation;

namespace ToolsManagementSystem.Controllers
{
    [CustomAuthorize]
    public class AFFAController : Controller
    {
        private ToolManagementContext db = new ToolManagementContext();
        ReportParameter[] p;
        String[] param;

        // GET: /AFFA/
        public ActionResult Index(string sortOrder, int? page, string searchString, string searchByStatus)
        {
            ViewBag.FAAFNoSortParm = String.IsNullOrEmpty(sortOrder) ? "FAAFNo" : "";
            ViewBag.SearchStr = searchString;

            var affas = db.AFFAs.Include(a => a.Employees);

            if (!String.IsNullOrEmpty(searchString))
            {
                affas = affas.Where(i => i.Employees.LastName.Contains(searchString) || i.Employees.FirstName.Contains(searchString)
                    || i.FAAFNo.Contains(searchString));
            }
            

            int pageSize = 15;
            int pageNumber = (page ?? 1);


            TempData.Remove("Status");
            if (!string.IsNullOrEmpty(searchByStatus))
            {
                TempData["Status"] = searchByStatus;
            }

            return View(affas.OrderByDescending(s => s.id).ToPagedList(pageNumber, pageSize));

        }
        public ActionResult Index_Warehouse()
        {
            return View();

        }
        // GET: /AFFA/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AFFA affa = db.AFFAs.Find(id);
            if (affa == null)
            {
                return HttpNotFound();
            }
            return View(affa);
        }

        // GET: /AFFA/Create
        public ActionResult Create(string returnurl)
        {
            ViewBag.DepartmentID = db.UserDepts.Where(a => a.Users.Username == User.Identity.Name).Select(a => a.DepartmentID).FirstOrDefault();
            ViewBag.UserID = db.UserDepts.Where(a => a.Users.Username == User.Identity.Name).Select(a => a.UserID).FirstOrDefault();
            ViewBag.EmployeeID = new SelectList(db.Employees, "id", "EmpId");
            ViewBag.ReturnUrl = returnurl;
            return View();
        }

        // POST: /AFFA/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]

        public ActionResult Create(AFFA affa, GroupMemberViewModel[] _arry, string trans)
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

                    affa.EmployeeID = emp.id;//attached for group
                    affa.TransType = "Group";

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
                    affa.EmployeeID = _arry[0].EmployeeID;
                    affa.TransType = "Individual";
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
                string lno = db.NoSeries.Where(i => i.Code == "FAAF").Select(n => n.LastNo).FirstOrDefault();
                string s = lno;
                int number = Convert.ToInt32(s);
                number += 1;
                string str = number.ToString("D7");

                affa.FAAFNo = "FAAF-" + str;
                affa.UserID = db.Users.Where(a => a.Username == User.Identity.Name).Select(a => a.id).FirstOrDefault();

                db.AFFAs.Add(affa);
                db.SaveChanges();

                int getid = affa.id;


                NoSeries ns = db.NoSeries.SingleOrDefault(v => v.Code == "FAAF");
                ns.LastNo = str;
                db.SaveChanges();

                res.status = "success";
                Log log = new Log();
                log.descriptions = "Added new AFFA Record. FAAFNo:" + affa.FAAFNo;
                db.Logs.Add(log);
                db.SaveChanges();
             
                res.message = getid.ToString();
                ViewBag.EmployeeID = new SelectList(db.Employees, "id", "EmpId", affa.EmployeeID);

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

        // GET: /AFFA/Edit/5
        public ActionResult Edit(int? id, int? user)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AFFA affa = db.AFFAs.Find(id);
            if (affa == null)
            {
                return HttpNotFound();
            }

            var emp = db.Employees.Select(e => new SelectListItem
            {
                Value = e.id.ToString(),
                Text = e.EntryType == "Individual" ? e.LastName + "," + e.FirstName : e.FirstName
            });

            if (affa.TransType == "Group")
            {
                ViewBag.GroupName = db.GroupAccountabilityMembers.Where(c => c.EmployeeID == affa.EmployeeID)
                                                           .Select(c => c.GroupAccountabilities.Code).FirstOrDefault();
            }
            else
            {
                ViewBag.GroupName = "";
            }

            ViewBag.EmployeeID = new SelectList(emp, "Value", "Text", affa.EmployeeID);
            ViewBag.ID = id;
            ViewBag.DocStatus = affa.DocStatus;
            ViewBag.UserType = user;
            return View(affa);
        }

        // POST: /AFFA/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="id,FAAFNo,EmployeeID,Status,Created_At")] AFFA affa)
        {
            if (ModelState.IsValid)
            {
                db.Entry(affa).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EmployeeID = new SelectList(db.Employees, "id", "EmpId", affa.EmployeeID);
            return View(affa);
        }

        // GET: /AFFA/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AFFA affa = db.AFFAs.Find(id);
            if (affa == null)
            {
                return HttpNotFound();
            }
            return View(affa);
        }

        // POST: /AFFA/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //AFFA affa = db.AFFAs.Find(id);
            //db.AFFAs.Remove(affa);
            //db.SaveChanges();
            //return RedirectToAction("Index");



            AFFA affa = db.AFFAs.Find(id);
            affa.Status = "Deleted";
            db.Entry(affa).State = EntityState.Modified;
            db.SaveChanges();

            Log log = new Log();
            log.descriptions = "Deleted AFFA Record. FAAFNO id:" + id;
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
        public  JsonResult GetNoSeriesFromAFFA(int AFFAID) {
            int cnt = 0;
            string faarfno = "";
           
            var result = db.AFFAIssues
                .GroupJoin(db.AFFAReturns
                //.Where(e => e.Status == "Active")
                //.Where(e => e.DocStatus != 0)
                        , i => i.id
                        , r => r.AFFAIssueID,
                        (i, r) => new
                        {
                            Issue = i,
                            Returns = r.DefaultIfEmpty()
                        })
                        .SelectMany(
                         a => a.Returns
                         .Select(b =>
                            new 
                            {
                                a.Issue.AFFAID

                               ,IssueID = b.AFFAIssues.id
                               ,b.FAARFNo
                               ,b.AFFAIssueID
                               ,a.Issue.Status
                            }
                        ))
                        .Where(b => b.Status == "Active" || b.Status == "Transferred")
                        .Where(b => b.AFFAID == AFFAID)
                        .Where(b => b.AFFAIssueID != null).FirstOrDefault();



            if (result != null)
            {
                faarfno = result.FAARFNo;
            }
            else
            {
                faarfno = "-"; 
            }

            var model = new
            {
                message = faarfno,
                status = "success"
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

        public JsonResult GetAFFADetails(int offset, int limit, string search, string sort, string order, int afid, string filter)
        {
            var totalcount = 0;
            var model = db.AFFAIssues.Select(b => new
            {
                b.id,
                b.AFFAID,
                b.ItemID,
                b.Items.ItemCode,
                b.Items.Description,
                b.Items.ModelType,
                b.SerialNo,
                b.PO,
                b.Quantity,
                b.UoM,
                UnitCost = b.UnitCost.ToString(),
                b.Amount,
                FACardNo = b.Items.ItemDetails.Where(x => x.SerialNo == b.SerialNo && x.PO == b.PO).Select(a => a.FACardNo).FirstOrDefault(),
                b.PropertyNo,
                DateIssued = b.DateIssued.Month.ToString() + "/" + b.DateIssued.Day.ToString() + "/" + b.DateIssued.Year.ToString(),
                b.Status,
                b.Remarks,
                Created_At = b.Created_At,
                qtyret = db.AFFAReturns
                        .Where(a => a.Status == "Active")
                        .Where(a => a.AFFAIssueID == b.id)
                        .Select(a => a.Quantity)
                        .DefaultIfEmpty(0)
                        .Sum(),
                rembal = b.Quantity - b.QuantityTransferred - db.AFFAReturns
                       .Where(a => a.Status == "Active")
                       .Where(a => a.AFFAIssueID == b.id)
                       .Select(a => a.Quantity)
                       .DefaultIfEmpty(0)
                       .Sum(),
                qtytrans = b.QuantityTransferred,
                b.AccountCode

            })
            .Where(b => b.Status == "Active" || b.Status == "Transferred" || b.Status == "Cancelled");
            model = model.Where(i => i.AFFAID == afid).OrderBy(i => i.id);


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

                        if (colSearch == "Quantity" || colSearch == "qtyret" || colSearch == "rembal" || colSearch == "qtytrans")
                        {
                            colval = colval.Replace("\"", ""); //remove escape character "\"
                        }


                        if (!string.IsNullOrEmpty(strFilter))
                        {
                            strFilter = (colSearch != "Quantity" & colSearch != "qtyret" & colSearch != "rembal" & colSearch != "qtytrans") ? (strFilter + " && " + colSearch + ".Contains(" + colval + ")") : (strFilter + " && " + "(" + colSearch + "=" + colval + ")");
                        }
                        else
                        {
                            strFilter = (colSearch != "Quantity" & colSearch != "qtyret" & colSearch != "rembal" & colSearch != "qtytrans") ? (colSearch + ".Contains(" + colval + ")") : "(" + colSearch + "=" + colval + ")";

                        }

                    }

                }

                else
                {

                    o = "";
                    o = q.Replace(':', '=').Replace("{", "").Replace("}", "").Replace(@"\", "");
                    string colSearch = o.Before("=").Replace('"', ' ').Replace(" ", "");
                    string colval = o.After("=");
                    if (colSearch == "Quantity" || colSearch == "qtyret" || colSearch == "rembal" || colSearch == "qtytrans")
                    {
                        colval = colval.Replace("\"", ""); //remove escape character "\"
                    }


                    strFilter = (colSearch != "Quantity" & colSearch != "qtyret" & colSearch != "rembal" & colSearch != "qtytrans") ? (colSearch + ".Contains(" + colval + ")") : "(" + colSearch + "=" + colval + ")";
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
            var xxxx = model.ToList();
            if (!string.IsNullOrEmpty(filterQuery))
            {
                ///dynamic LINQ
                model = model.Where(filterQuery);


            }

           xxxx = model.ToList();

            totalcount = model.Count();
            model = model.OrderBy(i => i.id).Skip(offset).Take(limit);

            var modelItem = new
            {
                total = totalcount,
                rows = model.ToList(),
            };
            return Json(modelItem, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAFFAReturn(int offset, int limit, string search, string sort, string order, int afid)
        {
            var totalcount = 0;
            var model = db.AFFAReturns.Include(r => r.AFFAIssues).Select(b => new
            {
                b.id,
                b.AFFAIssues.AFFAID,
                b.AFFAIssueID,
                b.FAARFNo,
                //b.AFFAIssues.FACardNo,
                b.AFFAIssues.PropertyNo,
                b.AFFAIssues.Items.ItemCode,
                b.AFFAIssues.Items.Description,
                b.AFFAIssues.Items.ModelType,
                b.AFFAIssues.SerialNo,
                b.AFFAIssues.PO,
                b.Quantity,
                b.AFFAIssues.UoM,

                UnitCost = b.AFFAIssues.UnitCost.ToString(),
                FACardNo = b.AFFAIssues.Items.ItemDetails.Where(x => x.SerialNo == b.AFFAIssues.SerialNo && x.PropertyNo == b.AFFAIssues.PropertyNo).Select(a => a.FACardNo),

                b.DateReturned,
                b.ToolStatus,
                b.Recommendation,
                b.Remarks,
                b.Status,
                b.DocStatus,
                b.FindingsObservation
            }
            ).Where(b => b.Status == "Active");
            model = model.Where(i => i.AFFAIssueID == afid).OrderBy(i => i.id);

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
                AFFA afemployee = db.AFFAs.Find(id);
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
            }
            catch (Exception e)
            {
                res.message = e.Message;
                res.status = "fail";

            }

            Log log = new Log();
            log.descriptions = "Modified record id:" + id + " Table [AFFA]";
            log.otherinfo = "Old Stat : " + old_status + "New Stat : " + status;
            db.Logs.Add(log);
            db.SaveChanges();

            res.status = "success";
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllAFFAReturn(int offset, int limit, string search, string sort, string order, int afid)
        {

            var model = db.AFFAReturns.Include(r => r.AFFAIssues).Select(b => new
            {
                b.id,
                b.AFFAIssues.AFFAID,
                b.AFFAIssueID,
                b.FAARFNo,
                b.AFFAIssues.FACardNo,
                b.AFFAIssues.PropertyNo,
                b.AFFAIssues.Items.ItemCode,
                b.AFFAIssues.Items.Description,
                b.AFFAIssues.Items.ModelType,
                b.AFFAIssues.SerialNo,
                b.AFFAIssues.PO,
                b.Quantity,
                b.AFFAIssues.UoM,

                UnitCost = b.AFFAIssues.UnitCost.ToString(),
                b.DateReturned,
                b.ToolStatus,
                b.Recommendation,
                b.Remarks,
                b.Status
                ,
                b.DocStatus
                ,b.FindingsObservation
            }
            ).Where(b => b.Status == "Active");

            model = model.Where(i => i.AFFAID == afid).OrderBy(i => i.id);
            //model = model.Where(i => i.AFFAIssueID == afid).OrderBy(i => i.id);

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
        public ActionResult PrintFAAF_Issue(string FAAFNo, int EmpID)
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
            viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\FAAF.rdlc";
            dt = new ReportController().PrintFAAFNo_Issue(FAAFNo);
            viewer.LocalReport.DataSources.Add(new ReportDataSource("FAAF", dt));
            viewer.LocalReport.DataSources.Add(new ReportDataSource("EBCMembers", dt2));
            viewer.LocalReport.Refresh();



            param = new String[6];

            var reportparameters = new SignatoryController().SearchSignatory("SLPGC", "FAISSUE");
            string xstring = JsonConvert.SerializeObject(reportparameters);
            dynamic dynJson = JsonConvert.DeserializeObject(xstring);

            foreach (var item in dynJson.Data)
            {
                if (item.SignatoryLabel == "PREPARED BY")
                {
                    param[0] = item.Name;
                    param[1] = item.Designation;
                }
                if (item.SignatoryLabel == "RECOMMENDING APPROVAL")
                {
                    param[2] = item.Name;
                    param[3] = item.Designation;
                }
                if (item.SignatoryLabel == "APPROVED BY")
                {
                    param[4] = item.Name;
                    param[5] = item.Designation;
                }
            }



            p = new ReportParameter[6];
            p[0] = new ReportParameter("PreparedByName", param[0]);
            p[1] = new ReportParameter("PreparedByDesignation", param[1]);
            p[2] = new ReportParameter("RecommendingApprovalName", param[2]);
            p[3] = new ReportParameter("RecommendingApprovalDesignation", param[3]);
            p[4] = new ReportParameter("ApprovedByName", param[4]);
            p[5] = new ReportParameter("ApprovedByDesignation", param[5]);

            viewer.LocalReport.SetParameters(p[0]);
            viewer.LocalReport.SetParameters(p[1]);
            viewer.LocalReport.SetParameters(p[2]);
            viewer.LocalReport.SetParameters(p[3]);
            viewer.LocalReport.SetParameters(p[4]);
            viewer.LocalReport.SetParameters(p[5]);

            var bytes = viewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

            return new FileContentResult(bytes, mimeType);

        }
        public ActionResult PrintFAAF_Return(string FAAFNo)
        {
            //PDF VERSION
            DataTable dt = new DataTable();
            Warning[] warnings;
            string mimeType;
            string[] streamids;
            string encoding;
            string filenameExtension;

            var viewer = new ReportViewer();
            viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\FAAF_Return.rdlc";
            dt = new ReportController().PrintFAAFNo_Return(FAAFNo); 
            viewer.LocalReport.DataSources.Add(new ReportDataSource("FAAF_Return", dt));
            viewer.LocalReport.Refresh();


            param = new String[6];

            var reportparameters = new SignatoryController().SearchSignatory("SLPGC", "FARETURN");
            string xstring = JsonConvert.SerializeObject(reportparameters);
            dynamic dynJson = JsonConvert.DeserializeObject(xstring);
            foreach (var item in dynJson.Data)
            {
                if (item.SignatoryLabel == "RECOMMENDING APPROVAL")
                {
                    param[0] = item.Name;
                    param[1] = item.Designation;
                }
                if (item.SignatoryLabel == "APPROVED BY")
                {
                    param[2] = item.Name;
                    param[3] = item.Designation;
                }
                if (item.SignatoryLabel == "RECEIVED BY")
                {
                    param[4] = item.Name;
                    param[5] = item.Designation;
                }
            }



            p = new ReportParameter[6];
            p[0] = new ReportParameter("RecommendingApprovalName", param[0]);
            p[1] = new ReportParameter("RecommendingApprovalDesignation", param[1]);
            p[2] = new ReportParameter("ApprovedByName", param[2]);
            p[3] = new ReportParameter("ApprovedByDesignation", param[3]);
            p[4] = new ReportParameter("ReceivedByName", param[4]);
            p[5] = new ReportParameter("ReceivedByDesignation", param[5]);

            viewer.LocalReport.SetParameters(p[0]);
            viewer.LocalReport.SetParameters(p[1]);
            viewer.LocalReport.SetParameters(p[2]);
            viewer.LocalReport.SetParameters(p[3]);
            viewer.LocalReport.SetParameters(p[4]);
            viewer.LocalReport.SetParameters(p[5]);


            var bytes = viewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

            return new FileContentResult(bytes, mimeType);

        }
        public ActionResult JsonReportParameters(string FAAFNo) {
            var reportparameters = new SignatoryController().SearchSignatory("SLPGC", "FAISSUE");
            string xstring = JsonConvert.SerializeObject(reportparameters);
            dynamic dynJson = JsonConvert.DeserializeObject(xstring);

            return Json(dynJson, JsonRequestBehavior.AllowGet);
        
        }
        [HttpPost]
        public ActionResult CancelAC(int itemid, string remarks)
        {
            JsonArray res = new JsonArray();
            if (itemid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AFFA af = db.AFFAs.Find(itemid);
            var old_status = af.DocStatus;

            if (af == null)
            {
                return HttpNotFound();
            }

            af.DocStatus = 3;
            db.Entry(af).State = EntityState.Modified;
            db.SaveChanges();



            var afdetails = db.AFFAIssues.Where(e => e.AFFAID == itemid).Where(a => a.Status == "Active").ToList();
            afdetails.ForEach(a => a.Status = "Cancelled");
            afdetails.ForEach(a => a.Remarks = remarks);
            db.SaveChanges();




            Log log = new Log();
            log.descriptions = "Cancelled record id:" + itemid + " Table [AFFA]";
            log.otherinfo = "Old Stat : " + old_status + "New Stat : " + 3;
            db.Logs.Add(log);
            db.SaveChanges();

            res.status = "success";
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

            //var v = db.AFFAIssues
            //    .Where(b => b.Status == "Active" || b.Status == "Transferred" || b.Status == "Cancelled")
            //    .Select(b => new
            //{
            //    b.AFFAs.id,
            //    RefNo = b.AFFAs.FAAFNo,
            //    EmployeeName = b.AFFAs.Employees.LastName + ", " + b.AFFAs.Employees.FirstName,
            //    b.ItemID,
            //    b.Items.ItemCode,
            //    b.Items.Description,
            //    b.Items.Description2,
            //    DateCreated = b.AFFAs.Created_At,
            //    IssuedQty = b.Quantity,
            //    Status = b.AFFAs.DocStatus == 0 ? "Open" : (b.AFFAs.DocStatus == 1 ? "Released" : (b.AFFAs.DocStatus == 2 ? "Closed" : "Cancelled"))
            //});

            var v = db.AFFAs
                    .Where(b => b.Status == "Active" || b.Status == "Transferred" || b.Status == "Cancelled")
                    .GroupJoin(db.AFFAIssues
                        , i => i.id
                        , r => r.AFFAID,
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
                                RefNo = a.Header.FAAFNo,
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
        public ActionResult SetToRelease(int id)
        {
            JsonArray res = new JsonArray();
            try
            {
                AFFA ae = db.AFFAs.Find(id);
                ae.DocStatus = 1;
                db.Entry(ae).State = EntityState.Modified;

                Log log = new Log();
                log.descriptions = "Change AFFA Record Status to Released [ Admin ]. AFFANo:" + ae.FAAFNo;
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

            var v = db.AFFAs
                .Where(b => b.Status == "Active" || b.Status == "Transferred" || b.Status == "Cancelled")
                .Select(b => new
                {
                    b.id
                   ,
                    RefNo = b.FAAFNo
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
                //TempData.Remove("Status");
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
