using TechShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TechShop.Controllers
{
    [HandleError]
    public class ParentCategoryController : Controller
    {
        private TechShopDbContext db = new TechShopDbContext();
        public ActionResult Category(string metatitle,long id)
        {
            var model = db.ProductCategories.Find(id);
            if (model.MetaTitle == metatitle)
            {
                ViewBag.ChildCategories = db.ProductCategories.Where(x => x.ParentID == id).OrderBy(x => x.Order).ToList();
                return View(model);
            }
            return RedirectToAction("Error404", "Error");

        }
    }
}