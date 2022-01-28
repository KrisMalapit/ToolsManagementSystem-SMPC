using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ToolsManagementSystem.Controllers
{
  
    public class CustomAuthorize : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var x = HttpContext.Current.Request.RawUrl.ToString();

            filterContext.Result = new RedirectResult("~/Account/Login?returnUrl="+x);


        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {

            if (this.AuthorizeCore(filterContext.HttpContext))
            {
                base.OnAuthorization(filterContext);
            }
            else
            {
                this.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}