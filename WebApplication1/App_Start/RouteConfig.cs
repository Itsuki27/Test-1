using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebApplication1
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Users", action = "Login", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ForgotPassword",
                url: "Users/ForgotPassword",
                defaults: new { controller = "Users", action = "ForgotPassword", id = UrlParameter.Optional }
            );

            routes.MapRoute(
               name: "ResetPassword",
               url: "Users/ResetPassword",
               defaults: new { controller = "Users", action = "ResetPassword", id = UrlParameter.Optional }
           );
            routes.MapRoute(
                name: "AuditLog",
                url: "MOVEHISTs/AuditLog",
                defaults: new { controller = "MOVEHISTs", action = "AuditLog", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Reports",
                url: "Users/Reports",
                defaults: new { controller = "Users", action = "Reports", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Edit",
                url: "Users/Edit/{id}",
                defaults: new { controller = "Users", action = "Edit", id = UrlParameter.Optional }
            );
        }
    }
}
