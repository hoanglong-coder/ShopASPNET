using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
namespace PROJECT_WEBSITE.WebAPP
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "Product Category",
                url: "dang-nhap",
                defaults: new { controller = "Login", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "PROJECT_WEBSITE.WebAPP.Controllers" }
            );
            routes.MapRoute(
                name: "My Account",
                url: "tai-khoan/{id}",
                defaults: new { controller = "Account", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "PROJECT_WEBSITE.WebAPP.Controllers" }
            );
            routes.MapRoute(
                name: "Login",
                url: "san-pham/{metatitle}-{id}",
                defaults: new { controller = "Product", action = "Products", id = UrlParameter.Optional },
                namespaces: new[] { "PROJECT_WEBSITE.WebAPP.Controllers" }
            );
            routes.MapRoute(
                name: "Edit Account",
                url: "chinh-sua-tai-khoan/{id}",
                defaults: new { controller = "Account", action = "EditAccount", id = UrlParameter.Optional },
                namespaces: new[] { "PROJECT_WEBSITE.WebAPP.Controllers" }
                
            );
            routes.MapRoute(
               name: "Product Detail",
               url: "chi-tiet/{metatitle}-{id}",
               defaults: new { controller = "Product", action = "DetailProduct", id = UrlParameter.Optional },
               namespaces: new[] { "PROJECT_WEBSITE.WebAPP.Controllers" }
           );
            routes.MapRoute(
                name: "News",
                url: "tin-tuc/{id}",
                defaults: new { controller = "News", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "PROJECT_WEBSITE.WebAPP.Controllers" }
            );
            routes.MapRoute(
               name: "News detail",
               url: "tin-tuc-chi-tiet/{id}",
               defaults: new { controller = "News", action = "Detail", id = UrlParameter.Optional },
               namespaces: new[] { "PROJECT_WEBSITE.WebAPP.Controllers" }
           );
            routes.MapRoute(
               name: "Footer detail",
               url: "footer-chi-tiet/{id}",
               defaults: new { controller = "Home", action = "DetailFooter", id = UrlParameter.Optional },
               namespaces: new[] { "PROJECT_WEBSITE.WebAPP.Controllers" }
           );
            routes.MapRoute(
                name: "CartItem",
                url: "them-gio-hang",
                defaults: new { controller = "Cart", action = "AddItem", id = UrlParameter.Optional },
                namespaces: new[] { "PROJECT_WEBSITE.WebAPP.Controllers" }
            );
            routes.MapRoute(
                name: "Cart",
                url: "gio-hang",
                defaults: new { controller = "Cart", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "PROJECT_WEBSITE.WebAPP.Controllers" }
            );
            routes.MapRoute(
                name: "Payment",
                url: "thanh-toan",
                defaults: new { controller = "Cart", action = "Payment", id = UrlParameter.Optional },
                namespaces: new[] { "PROJECT_WEBSITE.WebAPP.Controllers" }
            );
            routes.MapRoute(
                name: "PaymentCOD",
                url: "thanh-toan/PaymentCode",
                defaults: new { controller = "Cart", action = "Payment", id = UrlParameter.Optional },
                namespaces: new[] { "PROJECT_WEBSITE.WebAPP.Controllers" }
            );
            routes.MapRoute(
                name: "FinalOrder",
                url: "hoan-thanh",
                defaults: new { controller = "Cart", action = "FinalOrder", id = UrlParameter.Optional },
                namespaces: new[] { "PROJECT_WEBSITE.WebAPP.Controllers" }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "PROJECT_WEBSITE.WebAPP.Controllers" }
            );
        }
    }
}
