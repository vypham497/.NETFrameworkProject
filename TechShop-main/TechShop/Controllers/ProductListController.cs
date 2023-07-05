using TechShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace TechShop.Controllers
{
    [HandleError]
    public class ProductListController : Controller
    {
        private TechShopDbContext db = new TechShopDbContext();
        // GET: ProductList
        public ActionResult Index(string metatitle,string sortOrder,long id, int? page = 1)
        {
            var products = from s in db.Products
                           select s;
            int pageSize = 12;
            int pageNumber = (page ?? 1);
            ViewBag.CurrentSort = sortOrder;
            ViewBag.SortLabel = "Mới nhất";
            products = db.Products.Where(x => x.CategoryID == id);
            ViewBag.totalRecord = products.Count();
            switch (sortOrder)
            {
                case "Name":
                    products = products.OrderBy(s => s.Title);
                    ViewBag.SortLabel = "Tên(A - Z)";
                    break;
                case "name_desc":
                    products = products.OrderByDescending(s => s.Title);
                    ViewBag.SortLabel = "Tên(Z - A)";
                    break;
                case "Price":
                    products = products.OrderBy(s => s.Price);
                    ViewBag.SortLabel = "Giá (Thấp - cao)";
                    break;
                case "price_desc":
                    products = products.OrderByDescending(s => s.Price);
                    ViewBag.SortLabel = "Giá (Cao - thấp)";
                    break;
                case "date":
                    products = products.OrderByDescending(s => s.CreatedDate);
                    ViewBag.SortLabel = "Mới nhất";
                    break;
                default:
                    products = products.OrderByDescending(x => x.CreatedDate);
                    ViewBag.SortLabel = "Mới nhất";
                    break;
            }

            ProductCategory productCategory = db.ProductCategories.Find(id);
            ViewBag.CategoryName = productCategory.Title;
            ViewBag.CategoryMeta = productCategory.MetaTitle;
            ViewBag.CategoryID = productCategory.ID;
            ViewBag.ShortDesc = productCategory.ShortDesc;
            ViewBag.Desc = productCategory.Description;
            if (productCategory.MetaTitle == metatitle)
            {
                return View(products.ToPagedList(pageNumber, pageSize));
            }
            return RedirectToAction("Error404", "Error");
        }
    }
}