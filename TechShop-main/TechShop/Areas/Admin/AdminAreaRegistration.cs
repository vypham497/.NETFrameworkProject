using System.Web.Mvc;

namespace TechShop.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                name: "Tim kiem admin",
                url: "Admin/Products/tim-kiem",
                defaults: new { controller = "Products", action = "GetSearch", id = UrlParameter.Optional },
                namespaces: new[] { "TechShop.Areas.Admin.Controllers" }
            );
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { action = "Index", controller="Home", id = UrlParameter.Optional },
                new[] { "TechShop.Areas.Admin.Controllers" }
            );
        }
    }
}