using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using TechShop.Models;
using TechShop.Areas.Admin.Models;
using TechShop.Common;

namespace TechShop.Areas.Admin.Controllers
{
    [HandleError]
    public class ProductsController : BaseController
    {
        private TechShopDbContext db = new TechShopDbContext();

        // GET: Admin/Products
        public ActionResult Index()
        {
            var query = from p in db.Products
                        join c in db.ProductCategories on p.CategoryID equals c.ID
                        join b in db.Brands on p.BrandID equals b.ID
                        select new ProductWithCategory { Product = p, category = c, brand = b };
            var model = query.ToList();
            ViewBag.Category = db.ProductCategories.Where(x=>x.ParentID.HasValue).ToList();
            ViewBag.Brand = db.Brands.Where(x => x.Status==true).ToList();
            return View(model);
        }
        [HttpPost]
        public ActionResult Index(long Category,int Brand,string txtSearch)
        {
            var query = from p in db.Products
                        join c in db.ProductCategories on p.CategoryID equals c.ID
                        join b in db.Brands on p.BrandID equals b.ID
                        select new ProductWithCategory { Product = p, category = c, brand = b };
            var model = query.ToList();
            ViewBag.Category = db.ProductCategories.Where(x => x.ParentID.HasValue).ToList();
            ViewBag.Brand = db.Brands.Where(x => x.Status == true).ToList();
            if (String.IsNullOrEmpty(txtSearch) && Category == 0 && Brand==0)
            {
                ViewBag.Count = 0;
            }
            else
            {
                if (Category == 0 && Brand == 0)
                {
                    model = query.Where(x => x.Product.Title.Contains(txtSearch)).ToList();
                }
                else if (Category == 0)
                {
                    model = query.Where(x => x.Product.Title.Contains(txtSearch) && x.Product.BrandID==Brand).ToList();
                }
                else
                {
                    if (Brand == 0)
                    {
                        model = query.Where(x => x.Product.Title.Contains(txtSearch) && x.Product.CategoryID == Category).ToList();
                    }
                    else
                    {
                        model = query.Where(x =>x.Product.BrandID==Brand && x.Product.CategoryID == Category && x.Product.Title.Contains(txtSearch)).ToList();
                    }
                }
                ViewBag.Count = model.Count;
            }
            
            return View(model);
            
        }
        // GET: Admin/Products/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            Brand brand = db.Brands.Find(product.BrandID);
            ProductCategory productCategory = db.ProductCategories.Find(product.CategoryID);
            if (product == null)
            {
                return HttpNotFound();
            }
            product.Description = WebUtility.HtmlDecode(product.Description);
            product.Detail = WebUtility.HtmlDecode(product.Detail);
            ViewBag.BrandName = brand.Title;
            ViewBag.CategoryName = productCategory.Title;
            return View(product);
        }

        // GET: Admin/Products/Create
        public ActionResult Create()
        {
            SetViewBag();
            return View();
        }

        public JsonResult AddProduct(Product p)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var listImages = serializer.Deserialize<List<string>>(p.Images);
            XElement xElement = new XElement("Images");
            foreach (var item in listImages)
            {
                xElement.Add(new XElement("Images", item));
            }
            var product = new Product();
            product.Images = xElement.ToString();
            product.Price = p.Price;
            product.OldPrice = p.OldPrice;
            long percent = 0;
            if (product.OldPrice.HasValue)
            {
                percent = Convert.ToInt64((product.OldPrice - product.Price) / product.OldPrice * 100);
            }
            DateTime now = DateTime.Now;
            product.ViewCount = 0;
            product.CreatedDate = now;
            product.CreatedBy = Session[CommonConstants.USER_SESSION].ToString();
            product.PercentSale = percent;
            
            product.Title = p.Title;
            product.MetaTitle = ConvertToSEO.Convert(product.Title);
            product.Code = p.Code;
            product.Description = p.Description;
            product.Thumb = p.Thumb;
            product.MetaKeywords = p.MetaKeywords;
            product.MetaDescription = p.MetaDescription;
            product.Quantity = p.Quantity;
            product.CategoryID = p.CategoryID;
            product.BrandID = p.BrandID;
            product.UpTopHot = p.UpTopHot;
            product.Detail = p.Detail;
            product.Guarantee = p.Guarantee;
            product.Specification = p.Specification;
            product.Video = p.Video;
            db.Products.Add(product);

            db.SaveChanges();
            return Json(new { status = true });
        }

        public JsonResult EditProduct(Product p)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var listImages = serializer.Deserialize<List<string>>(p.Images);
            XElement xElement = new XElement("Images");
            foreach (var item in listImages)
            {
                xElement.Add(new XElement("Images", item));
            }
            var product = db.Products.Find(p.ID);
            product.Images = xElement.ToString();
            product.Price = p.Price;
            product.OldPrice = p.OldPrice;
            long percent = 0;
            if (product.OldPrice.HasValue)
            {
                percent = Convert.ToInt64((product.OldPrice - product.Price) / product.OldPrice * 100);
            }
            DateTime now = DateTime.Now;
            product.UpdatedDate = now;
            product.UpdatedBy = Session[CommonConstants.USER_SESSION].ToString();
            product.Title = p.Title;
            product.MetaTitle = ConvertToSEO.Convert(product.Title);
            product.PercentSale = percent;
            product.Code = p.Code;
            product.Description = p.Description;
            product.Thumb = p.Thumb;
            product.Quantity = p.Quantity;
            product.CategoryID = p.CategoryID;
            product.BrandID = p.BrandID;
            product.UpTopHot = p.UpTopHot;
            product.Detail = p.Detail;
            product.Guarantee = p.Guarantee;
            product.Specification = p.Specification;
            product.Video = p.Video;
            db.SaveChanges();
            return Json(new { status = true }); 
        }

        // GET: Admin/Products/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            var images = product.Images;
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
            SetViewBag(product.CategoryID);
            return View(product);
        }

        public JsonResult Delete(long id)
        {

            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return Json(new { status = true });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public void SetViewBag(long? selectedID = null)
        {
            ViewBag.CategoryID = new SelectList(db.ProductCategories.Where(x => x.Status == true && x.ParentID.HasValue).ToList(), "ID", "Title", selectedID);
            ViewBag.BrandID = new SelectList(db.Brands.Where(x => x.Status == true).ToList(), "ID", "Title", selectedID);
        }
    }
}
