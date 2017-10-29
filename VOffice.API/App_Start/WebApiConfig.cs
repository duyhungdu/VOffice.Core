using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using Autofac;
using Autofac.Integration.Mvc;
using System.Web.Http;
using Autofac.Integration.WebApi;
using System.Web.Http.Cors;

namespace VOffice.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
            
            config.Formatters.JsonFormatter
           .SerializerSettings
           .ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            //Web API DataType
            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);
            //config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

            //Cross Domain Config
            //var cors = new EnableCorsAttribute("http://localhost:8886,http://localhost:5233,http://api.veph.vn,http://localhost:82,http://localhost:56242,http://localhost:42008,http://localhost:60904,http://192.168.0.3:82,http://localhost:92", "*", "*");
            //config.EnableCors(cors);



        }
    }
}
