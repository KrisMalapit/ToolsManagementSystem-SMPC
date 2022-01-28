using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.DirectoryServices;
using ToolsManagementSystem.Models.View_Model;
using System.Text.RegularExpressions;
using System.DirectoryServices.AccountManagement;
using ToolsManagementSystem.DAL;
using ToolsManagementSystem.Models;
using System.Data.Entity;

namespace ToolsManagementSystem.Controllers
{
    [CustomAuthorize]
    [Authorize(Roles = "SuperAdmin")]
    public class UserController : Controller
    {
        private ToolManagementContext db = new ToolManagementContext();

        //
        // GET: /User/
        public ActionResult Index(int? page, string searchString, string domain)
        {
            ViewBag.Department = db.Departments.Select(a => new { a.id,a.Name });
            ViewBag.Domain = domain;
            
            return View();
        }
        
        public JsonResult UserList(int offset, int limit, string search, string sort, string order, string domain)
        {
           
            ViewBag.Domain = domain;
            int totalcount = 0;

            string stat = "";
            int id = 0;
            string roles = "";
            string domainName = "";
            
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);
            PrincipalContext ctx2 = new PrincipalContext(ContextType.Domain);

            if (string.IsNullOrEmpty(domain))
            {
     
                domainName = "semcalaca";
                domain = "SEMCALACA";
                ctx = new PrincipalContext(ContextType.Domain, domainName, "OU=SLPGC PLANT SITE,dc=semcalaca,dc=com", @"semcalaca\qmaster", "M@st3rQ###");

                ctx2 = new PrincipalContext(ContextType.Domain, domainName, "OU=SCPC PLANT SITE,dc=semcalaca,dc=com", @"semcalaca\qmaster", "M@st3rQ###");
            }
            else if (domain == "SEMCALACA")
            {
        
                domainName = "semcalaca";
                ctx = new PrincipalContext(ContextType.Domain, domainName, "OU=SLPGC PLANT SITE,dc=semcalaca,dc=com", @"semcalaca\qmaster", "M@st3rQ###");

                ctx2 = new PrincipalContext(ContextType.Domain, domainName, "OU=SCPC PLANT SITE,dc=semcalaca,dc=com", @"semcalaca\qmaster", "M@st3rQ###");
            }
            else if (domain == "SEMIRARAMINING")
            {

                domainName = "SEMIRARAMINING";
                domain = "SEMIRARAMINING";
                ctx = new PrincipalContext(ContextType.Domain, domainName,
                                                "OU=SEMIRARA MINESITE,DC=semiraramining,DC=net");
            }
            else
            {
                domainName = "smcdacon";
                ctx = new PrincipalContext(ContextType.Domain, domainName,
                                                "OU=MAKATI HEAD OFFICE,dc=smcdacon,dc=com", @"smcdacon\qmaster", "M@st3rQ###");
            }

            UserPrincipal qbeUser = new UserPrincipal(ctx);
            PrincipalSearcher srch = new PrincipalSearcher(qbeUser);
            List<UserViewModel> lst = new List<UserViewModel>();

            var srchList = srch.FindAll().OrderBy(s => s.Name).Skip(offset).Take(limit);

            if (!string.IsNullOrEmpty(search))
            {
                srchList = srch.FindAll().Where(b => b.Name.ToUpper().Contains(search.ToUpper())).OrderBy(s => s.Name).Skip(offset).Take(limit);
            }
            

            int cnt = srch.FindAll().Count();
            foreach (var rs in srchList)
            {
                DirectoryEntry de = rs.GetUnderlyingObject() as DirectoryEntry;

                if (de.Properties["sn"][0].ToString() != "") 
                {
                    string uname = domainName + "\\" + de.Properties["samAccountName"].Value.ToString();

                    //int ustat = db.Users.Where(u => u.Username == uname).Where(u => u.status == "Enabled").Count();
                    var user = db.Users.Where(u => u.Username == uname).Where(u => u.status == "Enabled").FirstOrDefault();

                    

                    if (user!= null)
                    {
                        stat = "Enabled";
                        id = user.id;
                        roles = user.Roles;
                    }
                    else
                    {
                        stat = "Disabled";
                        id = 0;
                        roles = "User";
                    }

                    lst.Add(new UserViewModel()
                    {
                        id = id,
                        Username = de.Properties["samAccountName"].Value.ToString(),
                        Name = rs.DisplayName,
                        Firstname = (de.Properties["givenname"].Count > 0) ? de.Properties["givenname"][0].ToString() : "",
                        Lastname = (de.Properties["sn"].Count > 0) ? de.Properties["sn"][0].ToString() : "",
                        mail = (de.Properties["mail"].Count > 0) ? de.Properties["mail"][0].ToString() : "",
                        sysid = rs.Guid.ToString(),
                        domain = domainName,
                        status = stat,
                        Roles = roles
                    });
                }
                
            }

            if (domain == "SEMCALACA")
            {

                //FOR SEMCALACA USING 2ND OU
                UserPrincipal qbeUser2 = new UserPrincipal(ctx2);
                PrincipalSearcher srch2 = new PrincipalSearcher(qbeUser2);
                var srchList2 = srch2.FindAll().Skip(offset).Take(limit);
                if (!string.IsNullOrEmpty(search))
                {
                    srchList2 = srch2.FindAll().Where(b => b.Name.ToUpper().Contains(search.ToUpper())).Skip(offset).Take(limit).OrderBy(s => s.Name);
                }
                //cnt = cnt + srch2.FindAll().Count();
                foreach (var rs in srchList2)
                {
                    DirectoryEntry de = rs.GetUnderlyingObject() as DirectoryEntry;
                    string hasLName = (de.Properties["sn"].Count > 0) ? de.Properties["sn"][0].ToString() : "";
                    if (hasLName != "")
                    {
                        string uname = domainName + "\\" + de.Properties["samAccountName"].Value.ToString();

                        //int ustat = db.Users.Where(u => u.Username == uname).Where(u => u.status == "Enabled").Count();
                        var user = db.Users.Where(u => u.Username == uname).Where(u => u.status == "Enabled").FirstOrDefault();

                        //if (ustat > 0)
                        //{
                        //    stat = "Enabled";
                        //    id = user.id;
                        //    roles = user.Roles;
                        //}
                        //else
                        //{
                        //    stat = "Disabled";
                        //    id = 0;
                        //    roles = "User";
                        //}
                        if (user != null)
                        {
                            stat = "Enabled";
                            id = user.id;
                            roles = user.Roles;
                        }
                        else
                        {
                            stat = "Disabled";
                            id = 0;
                            roles = "User";
                        }
                        lst.Add(new UserViewModel()
                        {
                            id = id,
                            Username = de.Properties["samAccountName"].Value.ToString(),
                            Name = rs.DisplayName,
                            Firstname = (de.Properties["givenname"].Count > 0) ? de.Properties["givenname"][0].ToString() : "",
                            Lastname = (de.Properties["sn"].Count > 0) ? de.Properties["sn"][0].ToString() : "",
                            mail = (de.Properties["mail"].Count > 0) ? de.Properties["mail"][0].ToString() : "",
                            sysid = rs.Guid.ToString(),
                            domain = domainName,
                            status = stat,
                            Roles = roles
                        });
                    }
                }
            }


            IEnumerable<UserViewModel> model = lst;

            if (!string.IsNullOrEmpty(search))
            {
                //model = model.Where(b => b.Name.ToUpper().Contains(search.ToUpper()));
                cnt = model.Count();
            }

            totalcount = model.Count();
            model = model.OrderBy(i => i.Name);

            var modelItem = new
            {
                total = cnt,
                rows = model.ToList(),
            };
            return Json(modelItem, JsonRequestBehavior.AllowGet);
            
        }

        private string GetCurrentDomainPath()
        {
            DirectoryEntry de =
               new DirectoryEntry("LDAP://RootDSE");
            return "LDAP://" +
               de.Properties["defaultNamingContext"][0].
                   ToString();
        }
        [HttpPost]
        public ActionResult EnableDisableUser(UserViewModel userView)
        {
            JsonArray arr = new JsonArray();
            ViewBag.Domain = userView.domain;
            try
            {
                if (userView.status == "Disabled")
                {
                    var result = db.Users.SingleOrDefault(b => b.sysid == userView.sysid);
                    if (result != null)
                    {
                        result.status = "Disabled";
                        db.Entry(result).State = EntityState.Modified;
                        db.SaveChanges();
                        arr.status = "success";
                    }


                }
                else
                {

                    var result = db.Users.SingleOrDefault(b => b.sysid == userView.sysid);
                    if (result != null)
                    {
                        result.status = "Enabled";
                        db.SaveChanges();

                    }
                    else
                    {
                        User user = new User();
                        user.Username = userView.domain + '\\' + userView.Username;
                        user.Name = userView.Name;
                        user.Firstname = userView.Firstname.ToUpper();
                        user.Lastname = userView.Lastname.ToUpper();
                        user.mail = userView.mail;
                        user.status = "Enabled";
                        user.sysid = userView.sysid;
                        user.Roles = userView.Roles;
                        db.Users.Add(user);
                        db.SaveChanges();

                    }


                    arr.status = "success";

                }

            }
            catch (Exception e)
            {
                arr.status = e.ToString();

            }




            return Json(arr, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult ChangeRoleUser(int? id, string Roles)
        {
            JsonArray arr = new JsonArray();
            try
            {
                var result = db.Users.Find(id);
                if (result != null)
                {
                    result.Roles = Roles;
                    db.Entry(result).State = EntityState.Modified;
                    db.SaveChanges();
                    arr.status = "success";

                }

            }
            catch (Exception e)
            {
                arr.status = e.ToString();

            }



            return Json(arr, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SaveUserDept(UserDept userdept)
        {
            JsonArray arr = new JsonArray();
            try
            {


                    db.UserDepts.Add(userdept);
                    db.SaveChanges();
                    arr.status = "success";


            }
            catch (Exception e)
            {
                arr.status = e.ToString();

            }



            return Json(arr, JsonRequestBehavior.AllowGet);
        }


    }
}