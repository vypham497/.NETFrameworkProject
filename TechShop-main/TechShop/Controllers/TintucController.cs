using TechShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TechShop.Controllers
{
    [HandleError]
    public class TintucController : Controller
    {
        private TechShopDbContext db = new TechShopDbContext();
        public ActionResult Index()
        {
            ViewBag.DanhGia = db.Newses.Where(x => x.CategoryID == 1).OrderByDescending(x => x.PublishedDate).Take(4).ToList();
            ViewBag.DanhgiaMoiNhat = db.Newses.Where(x => x.CategoryID == 1).OrderByDescending(x => x.PublishedDate).Take(1).ToList();

            ViewBag.MeoHay = db.Newses.Where(x => x.CategoryID == 2).OrderByDescending(x => x.PublishedDate).Take(3).ToList();

            ViewBag.TuVanMoiNhat = db.Newses.Where(x => x.CategoryID == 3).OrderByDescending(x => x.PublishedDate).Take(1).ToList();
            ViewBag.TuVan = db.Newses.Where(x => x.CategoryID == 3 && x.ID!=65).OrderByDescending(x => x.PublishedDate).Take(4).ToList();

            ViewBag.Socials = db.Socials.OrderBy(x => x.Order).ToList();
            return View();
        }
    }
}