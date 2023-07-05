using TechShop.Models;
using TechShop.Areas.Admin.Models;
using TechShop.Common;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace TechShop.Controllers
{
    [HandleError]
    public class ProductController : Controller
    {
        private TechShopDbContext db = new TechShopDbContext();
        //[OutputCache(Duration =int.MaxValue, VaryByParam ="id",Location =System.Web.UI.OutputCacheLocation.Server)]
        public List<Product> ProductList(long id, long pid)
        {
            return db.Products.Where(x=>x.CategoryID==id && x.ID != pid).OrderByDescending(x => x.CreatedDate).ToList();
        }
        [OutputCache(CacheProfile ="Cache1DayForProduct")]
        public ActionResult Detail(string metatitle, long id)
        {
            var model = db.Products.Find(id);
            // Tăng số lần xem
            model.ViewCount++;
            db.SaveChanges();

            var images = model.Images;
            XElement xImages;
            List<string> listImagesReturn = new List<string>();
            if (images == null)
            {
                listImagesReturn = null;
            }
            else
            {
                xImages = XElement.Parse(images);
                foreach (XElement element in xImages.Elements())
                {
                    listImagesReturn.Add(element.Value);
                }
            }
            ViewBag.Images = listImagesReturn;
            var RecentProduct = Session["RecentProductList"];
            if (RecentProduct != null)
            {
                var list = (List<RecentProduct>)RecentProduct;
                if (list.Exists(x => x.Product.ID == id && x.Product.MetaTitle==metatitle))
                {

                }
                else
                {
                    var item = new RecentProduct();
                    item.Product = model;
                    list.Add(item);
                }
                Session["RecentProductList"] = list;
            }
            else
            {
                //tạo mới cart item
                var item = new RecentProduct();
                item.Product = model;
                var list = new List<RecentProduct>();
                list.Add(item);
                //gán vào session
                Session["RecentProductList"] = list;

            }
            ViewBag.SameProducts = ProductList(model.CategoryID,id);
            if (model.MetaTitle == metatitle)
            {
                return View(model);
            }
            return RedirectToAction("Error404", "Error");
        }
        [ChildActionOnly]
        [OutputCache(Duration = 3600 * 24)]
        public ActionResult CategoriesMenu()
        {
            return PartialView(db.ProductCategories.Where(x => x.Status == true).OrderBy(x => x.Order).ToList());
        }
        public JsonResult ListName(string q)
        {
            var data= db.Products.Where(x => x.Title.Contains(q)).Select(x => x.Title).ToList();
            return Json(new { data = data, status = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Search(string sortOrder,string keyword,long category_id, int? page = 1)
        {
            keyword = ConvertToUnSign.ToUnSign(keyword);
            var products = from s in db.Products
                           select s;
            int pageSize = 12;
            int pageNumber = (page ?? 1);
            ViewBag.Keyword = keyword;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.SortLabel = "Mới nhất";
            ViewBag.CategoryID = category_id;
            if (String.IsNullOrEmpty(keyword))
            {
                ViewBag.totalRecord = 0;
            }
            if(category_id==0)
            {

                products = db.Products.Where(delegate (Product p)
                {
                    if (ConvertToUnSign.ToUnSign(p.Title).IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                        return true;
                    else
                        return false;
                }).AsQueryable();
                ViewBag.totalRecord = products.Count();
            }
            else
            {
                products = db.Products.Where(delegate (Product p)
                {
                    if (ConvertToUnSign.ToUnSign(p.Title).IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0 && p.CategoryID==category_id)
                        return true;
                    else
                        return false;
                }).AsQueryable();
                ViewBag.totalRecord = products.Count();
            }
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
            return View(products.ToPagedList(pageNumber, pageSize));
        }
        public JsonResult Review(Comment d)
        {
            Comment comment = new Comment();
            comment.FullName = d.FullName;
            comment.Email = d.Email;
            comment.ProductID = d.ProductID;
            comment.Review = d.Review;
            comment.Stars = d.Stars;
            DateTime now = DateTime.Now;
            comment.SentDate = now;
            comment.Approved = false;
            db.Comments.Add(comment);
            db.SaveChanges();
            return Json(new { status = true });
        }
    }
}