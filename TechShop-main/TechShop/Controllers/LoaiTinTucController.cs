using TechShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;

namespace TechShop.Controllers
{
    [HandleError]
    public class LoaiTinTucController : Controller
    {
        private TechShopDbContext db = new TechShopDbContext();
        public ActionResult Index(string metatitle,long id, int? page = 1)
        {
            var model=db.Newses.Where(x => x.CategoryID == id).OrderByDescending(x => x.PublishedDate);
            Category category = db.Categories.Find(id);
            ViewBag.CategoryName = category.Title;
            ViewBag.CategoryMeta = category.MetaTitle;
            ViewBag.CategoryID = category.ID;
            int pageSize = 12;
            int pageNumber = (page ?? 1);
            if (category.MetaTitle == metatitle)
            {
                return View(model.ToPagedList(pageNumber, pageSize));
            }
            return RedirectToAction("Error404", "Error");

        }
    }
}