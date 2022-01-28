using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using ToolsManagementSystem.App_Start;
using ToolsManagementSystem.DAL;

namespace ToolsManagementSystem
{
    public class MvcApplication : System.Web.HttpApplication
    {
        void Application_BeginRequest(object sender, EventArgs e)
        {
            if (ConfigurationManager.AppSettings["MaintenanceMode"] == "true")
            {
                if (!Request.IsLocal)
                {
                    HttpContext.Current.RewritePath("~/Home/Maintenance");

                }
            }
        }


        protected void Application_Start()
        {
            Database.SetInitializer<ToolManagementContext>(null);
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            try
            {
                var authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authCookie != null)
                {
                    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                    if (authTicket != null && !authTicket.Expired)
                    {
                        var roles = authTicket.UserData.Split(',');
                        HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(new FormsIdentity(authTicket), roles);
                    }
                }
            }
            catch (CryptographicException cex)
            {
                FormsAuthentication.SignOut();
            }
        }
    }
}
