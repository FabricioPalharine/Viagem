using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

using Newtonsoft.Json.Serialization;
using System.Web.Http.Cors;

namespace CV.UI.Web
{
    public static class WebApiConfig
    {
        public static string UrlPrefixRelative { get { return "~/api"; } }

        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));

            // Web API routes
            config.MapHttpAttributeRoutes();

          

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional },
                constraints: new { id = @"^\d+$" } // Only integers 

                );


            config.Routes.MapHttpRoute(
                    name: "DefaultActionApi",
                    routeTemplate: "api/{controller}/{action}/{id}",
                    defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
        name: "DefaultActionApiEmpty",
        routeTemplate: "api/{controller}/{action}"
);
            config.Routes.MapHttpRoute(
     name: "DefaultEmpty",
     routeTemplate: "api/{controller}"
);


        }
    }
}