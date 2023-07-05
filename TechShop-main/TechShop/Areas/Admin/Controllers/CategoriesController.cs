using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TechShop.Models;
using TechShop.Common;

namespace TechShop.Areas.Admin.Controllers
{
    [HandleError]
    public class CategoriesController : BaseController
    {
        private TechShopDbContext db = new TechShopDbContext();

        // GET: Admin/Categories
        public ActionResult Index()
        {
            return View(db.Categories.ToList());
        }

        // GET: Admin/Categories/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // GET: Admin/Categories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,MetaTitle,Images,Description,Order,ParentID,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,MetaKeywords,MetaDescription,Status")] Category category,HttpPostedFileBase Images)
        {
            if (ModelState.IsValid)
            {
                if (Images != null)
                {
                    string path = Path.Combine(Server.MapPath("~/Assets/Admin/img"), Path.GetFileName(Images.FileName));
                    Images.SaveAs(path);
                    category.Images = Images.FileName;
                }
                DateTime now = DateTime.Now;
                category.CreatedDate = now;
                category.CreatedBy = Session[CommonConstants.USER_SESSION].ToString();
                category.MetaTitle = ConvertToSEO.Convert(category.Title);
                db.Categories.Add(category);
                db.SaveChanges();
                SetAlert("Thêm danh mục bài viết thành công", "success");
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // GET: Admin/Categories/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Admin/Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,MetaTitle,Images,Description,Order,ParentID,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,MetaKeywords,MetaDescription,Status")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                DateTime now = DateTime.Now;
                category.UpdatedDate = now;
                category.UpdatedBy = Session[CommonConstants.USER_SESSION].ToString();
                category.MetaTitle = ConvertToSEO.Convert(category.Title);
                db.SaveChanges();
                SetAlert("Sửa danh mục bài viết thành công", "success");
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public ActionResult Delete(long id)
        {

            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
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
    }
}
