using System.Web.Mvc;
using System.Web.Routing;

namespace TfsBot
{
    public static class MvcConfig
    {
        public static void Configure()
        {
            RegisterRoutes(RouteTable.Routes);
        }

        private static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                "Default",                                             
                "{controller}/{action}/{id}",                          
                new { controller = "Home", action = "Index", id = "" }  
            );
        }
    }
}