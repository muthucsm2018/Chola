using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Chola
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("");

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Index", action = "Home", id = UrlParameter.Optional },
            //    new string[] { "MyCompany.MyProject.WebMvc.Controllers" }
            //);

            routes.MapRoute(
                     "Default", // Route name
                     "{controller}/{action}/{id}", // URL with parameters
                     new { controller = "Index", action = "Home", id = UrlParameter.Optional }, // Parameter defaults
                     new string[] { "Chola.Controllers" }
                );
        }
    }
}