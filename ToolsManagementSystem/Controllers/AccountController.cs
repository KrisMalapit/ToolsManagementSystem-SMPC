using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ToolsManagementSystem.DAL;
using ToolsManagementSystem.Models;
using ToolsManagementSystem.Models.View_Model;
using PagedList;
using System.ComponentModel;
using System.Net;
using Microsoft.AspNet.Identity;
using System.Web.Configuration;
using System.DirectoryServices.Protocols;
using System.Collections.Specialized;
using Newtonsoft.Json;

namespace ToolsManagementSystem.Controllers
{
    //[Authorize]
    public class AccountController : Controller
    {

        private ToolManagementContext db = new ToolManagementContext();

        public User GetUserDetails(User user)
        {
            //try
            //{
              
                var users = db.Users.ToList(); //Error
                //string.Format("After GetUserDetails").WriteLog();
                return users.Where(u => u.status == "Enabled").Where(u => u.Username.ToLower() == user.Username.ToLower()).FirstOrDefault();

            //}
            //catch (Exception e )
            //{

            //    string.Format(e.InnerException.Message).WriteLog();
            //    return null;
            //}
         }

        //Get
        //Account/Login
        [AllowAnonymous]

        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.BrowserName = Request.Browser.Browser;
       
            ViewBag.BrowserVersion = Request.Browser.Version;
            return View();
        }


        private string GetServer()
        {
            DirectoryEntry de =
               new DirectoryEntry("LDAP://RootDSE");

            return de.Properties["defaultNamingContext"][0].ToString();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {

            
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                string dc = GetServer();
                string[] arr = dc.Split(',');
                string[] d = arr[0].Split('=');
                string server = d[1];


                ////option 1
                //User user = new User() { Username = model.Username };

                ////option 2
                //User user = new User() { Username = server + "\\" + model.Username };
                User user = new User() { Username = model.Domain + "\\" + model.Username };
              
                user = GetUserDetails(user);



                if (user != null)
                {
                    

                    ////option 1
                    //string[] domain_uname = model.Username.Split('\\');
                    //string domain = domain_uname[0].ToString();
                    //string username = domain_uname[1].ToString();
                    //string pword = model.Password;


                    //option 2
                    string domain = model.Domain;
                    string username = model.Username;
                    string pword = model.Password;

                    //if (AuthenticateUser(domain, username, pword))
                    //{
                    var loginresult = CallAPI("http://aluminum/ADAPI/api/values", model.Domain, model.Username, model.Password);

                    if (loginresult == "OK")
                    {
                        FormsAuthentication.SetAuthCookie(model.Username, false);
                        var authTicket = new FormsAuthenticationTicket(1, user.Username, DateTime.Now, DateTime.Now.AddMinutes(2880), true, user.Roles);
                        string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                        var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                        HttpContext.Response.Cookies.Add(authCookie);


                        Log log = new Log();
                        log.descriptions = "User Log-In";
                        log.otherinfo = "Status : Success";
                        log.user_id = user.Username;
                        //log.browser = Request.Browser.Type;
                        db.Logs.Add(log);
                        db.SaveChanges();



                        if (string.IsNullOrEmpty(returnUrl))
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            return this.Redirect(returnUrl);
                        }



                    }
                    else
                    {
                        ModelState.AddModelError("", "You are not recognized in Active Directory");


                        return View(model);
                    }
                }

                else
                {
                 

                    Log log = new Log();
                    log.descriptions = "User Log-In";
                    log.otherinfo = "Status : Failed. Error: User not enabled to log-in.";
                    //log.browser = Request.Browser.Type;
                    log.user_id = model.Domain + "\\" + model.Username;
                    db.Logs.Add(log);
                    db.SaveChanges();


                    ModelState.AddModelError("", "User not enabled to log-in.");
                    return View(model);
                }
            }
            catch (Exception e)
            {
                string.Format("{0}" ,e.InnerException.Message).WriteLog();
                ModelState.AddModelError("", e.Message);
                return View(model);
            }

            
        }
        public string CallAPI(string url, string Domain, string UserName, string Password)

        {

            using (var wb = new WebClient())

            {

                var data = new NameValueCollection();

                data["Domain"] = Domain;

                data["Username"] = UserName;

                data["Password"] = Password;





                var response = wb.UploadValues(url, "POST", data);
                string responseInString = Encoding.UTF8.GetString(response);
                var str2 = JsonConvert.DeserializeObject(responseInString);


                return str2.ToString();

            }



        }

        private bool AuthenticateUser(string domainName, string userName, string password)
        {
            bool ret = false;

            //try
            //{

            //    DirectoryEntry de = new DirectoryEntry("LDAP://" + domainName, userName, password);
            //    DirectorySearcher dsearch = new DirectorySearcher(de);
            //    SearchResult results = null;
            //    results = dsearch.FindOne();
            //    ret = true;
            //}

            try
            {
                string adServer = "";
                var aduser = domainName + "\\" + userName;

                if (domainName == "SEMIRARAMINING")
                {
                    adServer = "LDAP://" + domainName + ".net";
                }
                else
                {
                    adServer = "LDAP://" + domainName + ".com";
                }

                DirectoryEntry entry = new DirectoryEntry(adServer, aduser, password);
                object nativeObject = entry.NativeObject;




                ret = true;





                //var user = db.Users.Include(b => b.Roles).Where(u => u.UserName == model.UserName && u.Domain == model.Domain && u.Inactive == false).FirstOrDefault();

                //if (user != null)
                //{
                //    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                //    var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                //    AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, identity);

                //    TempData["AuthUserID"] = user.Id;
                //    return RedirectToAction("Index", "Home", new { u = Server.UrlEncode(user.Id) });
                //}


            }

            catch (DirectoryServicesCOMException cex)
            {
                string[] err = cex.ExtendedErrorMessage.Split(' ');
                string[] ldapErr = err[7].Split(',');
                if (ldapErr[0].Trim() == "531")  //531 -Authenticated but not permitted to logon at this workstation
                {
                    ret = true;
                }
                //not authenticated due to some other exception [this is optional]
                else if (ldapErr[0].Trim() == "525")
                {
                    this.ModelState.AddModelError(string.Empty, " User not found");
                }
                else if (ldapErr[0].Trim() == "52e")
                {
                    this.ModelState.AddModelError(string.Empty, "Invalid credentials");
                }
                else if (ldapErr[0].Trim() == "532")
                {
                    this.ModelState.AddModelError(string.Empty, "Password expired");
                }
                else if (ldapErr[0].Trim() == "530")
                {
                    this.ModelState.AddModelError(string.Empty, "Not permitted to logon at this time");
                }
                else if (ldapErr[0].Trim() == "533")
                {
                    this.ModelState.AddModelError(string.Empty, "Account disabled");
                }
                else if (ldapErr[0].Trim() == "534")
                {
                    this.ModelState.AddModelError(string.Empty, "The user has not been granted the requested logon type at this machine");
                }
                else if (ldapErr[0].Trim() == "701")
                {
                    this.ModelState.AddModelError(string.Empty, "Account expired");
                }
                else if (ldapErr[0].Trim() == "775")
                {
                    this.ModelState.AddModelError(string.Empty, "User account locked");
                }
                else if (ldapErr[0].Trim() == "773")
                {
                    this.ModelState.AddModelError(string.Empty, "User must reset password");
                }
                else
                {
                    //not authenticated; reason why is in cex
                    this.ModelState.AddModelError(string.Empty, cex.ExtendedErrorMessage);
                }

            }
            catch (Exception ex)
            {

                ret = false;

                Log log = new Log();
                log.descriptions = "User Log-In";
                log.otherinfo = "Status : Failed. Error: " + ex.Message;
                //log.browser = Request.Browser.Type;
                log.user_id = domainName + "\\" + userName;
                db.Logs.Add(log);
                db.SaveChanges();

                this.ModelState.AddModelError(string.Empty, ex.Message);
            }
            return ret;
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult LogOff()
        {
            Log log = new Log();
            log.descriptions = "User Logged-Out";
            log.otherinfo = "Status : Success";
            //log.browser = Request.Browser.Type;
       
            db.Logs.Add(log);
            db.SaveChanges();

            FormsAuthentication.SignOut();
            
            return RedirectToAction("LogIn", "Account");
        }

        //Get
        //Account/Register
        //[AllowAnonymous]
        [Authorize(Users = "webmaster")]
        public ActionResult Register(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [Authorize(Users = "webmaster")]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User user = new User()
            {
                Username = model.Username,
                Roles = model.Roles.ToString(),
                Password = GetSHA1HashData(model.Password)
            };

            db.Users.Add(user);
            db.SaveChanges();


            TempData["result"] = "success";
            return View();



        }
        //static string Hash(string input)
        //{
        //    using (SHA1Managed sha1 = new SHA1Managed())
        //    {
        //        var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
        //        var sb = new StringBuilder(hash.Length * 2);

        //        foreach (byte b in hash)
        //        {
        //            // can be "x2" if you want lowercase
        //            sb.Append(b.ToString("X2"));
        //        }

        //        return sb.ToString();
        //    }
        //}
        //private string GetMD5HashData(string data)
        //{
        //    //create new instance of md5
        //    MD5 md5 = MD5.Create();

        //    //convert the input text to array of bytes
        //    byte[] hashData = md5.ComputeHash(Encoding.Default.GetBytes(data));

        //    //create new instance of StringBuilder to save hashed data
        //    StringBuilder returnValue = new StringBuilder();

        //    //loop for each byte and add it to StringBuilder
        //    for (int i = 0; i < hashData.Length; i++)
        //    {
        //        returnValue.Append(hashData[i].ToString());
        //    }

        //    // return hexadecimal string
        //    return returnValue.ToString();

        //}
        private string GetSHA1HashData(string data)
        {
            //create new instance of md5
            SHA1 sha1 = SHA1.Create();

            //convert the input text to array of bytes
            byte[] hashData = sha1.ComputeHash(Encoding.Default.GetBytes(data));

            //create new instance of StringBuilder to save hashed data
            StringBuilder returnValue = new StringBuilder();

            //loop for each byte and add it to StringBuilder
            for (int i = 0; i < hashData.Length; i++)
            {
                returnValue.Append(hashData[i].ToString());
            }

            // return hexadecimal string
            return returnValue.ToString();
        }

        private bool ValidateSHA1HashData(string inputData, string storedHashData)
        {
            //hash input text and save it string variable
            string getHashInputData = GetSHA1HashData(inputData);

            if (string.Compare(getHashInputData, storedHashData) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}