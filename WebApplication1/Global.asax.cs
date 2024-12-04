using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using static System.Net.WebRequestMethods;

namespace WebApplication1
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Configure Anti-Forgery settings
            //AntiForgeryConfig.CookieName = "__RequestVerificationToken"; // Default cookie name
            //AntiForgeryConfig.RequireSsl = true; // Require SSL for Anti-Forgery cookie
        }

        //Ensure cookies are transmitted securely and include SameSite
        //protected void Application_EndRequest(object sender, EventArgs e)
        //{
        //    foreach (string cookieKey in Response.Cookies.AllKeys)
        //    {
        //        HttpCookie cookie = Response.Cookies[cookieKey];
        //        if (cookie != null)
        //        {
        //            cookie.Secure = true; // Enforce secure cookies
        //            cookie.HttpOnly = true; // Ensure cookies are HttpOnly
        //            cookie.SameSite = SameSiteMode.Lax; // Set SameSite attribute
        //        }
        //    }
        //}


        ////Server Leaks Information via "X-Powered-By" HTTP Response Header Field(s)
        ////Remove the X-Powered-By Header 
        ////Remove X-AspNet-Version and X-AspNetMvc-Version Headers
        //protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        //{
        //    HttpContext.Current.Response.Headers.Remove("Server");
        //    HttpContext.Current.Response.Headers.Remove("X-AspNetWebPages-Version");
        //    HttpContext.Current.Response.Headers.Remove("X-AspNet-Version");
        //    HttpContext.Current.Response.Headers.Remove("X-Powered-By");
        //    HttpContext.Current.Response.Headers.Remove("X-AspNetMvc-Version");


        //    //Server Leaks Version Information via "Server" HTTP Response Header Field
        //    if (Response.Headers["Server"] != null)
        //    {
        //        Response.Headers.Remove("Server");
        //    }
        
    }
}