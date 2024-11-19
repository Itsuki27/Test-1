using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    public class AuthorizeController : Controller
    {
        // GET: Authorize
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["Username"] == null)
            {
                filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            base.OnActionExecuting(filterContext);
        }
    }
}