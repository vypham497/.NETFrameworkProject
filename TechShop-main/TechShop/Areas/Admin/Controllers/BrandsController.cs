using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TechShop.Models;
using TechShop.Common;

namespace TechShop.Areas.Admin.Controllers
{
    [HandleError]
    public class BrandsController : BaseController
    {
        private TechShopDbContext db = new TechShopDbContext();

        // GET: Admin/Brands
        public ActionResult Index()
        {
            return View(db.Brands.ToList());
        }

        // GET: Admin/Brands/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Brand brand = db.Brands.Find(id);
            if (brand == null)
            {
                return HttpNotFound();
            }
            return View(brand);
        }

        // GET: Admin/Brands/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Brands/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,Logo,Description,Order,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Status")] Brand brand)
        {
            if (ModelState.IsValid)
            {
                DateTime now = DateTime.Now;
                brand.CreatedDate = now;
                brand.CreatedBy = Session[CommonConstants.USER_SESSION].ToString();
                db.Brands.Add(brand);
                db.SaveChanges();
                SetAlert("Thêm nhãn hàng thành công", "success");
                return RedirectToAction("Index");
            }

            return View(brand);
        }

        // GET: Admin/Brands/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Brand brand = db.Brands.Find(id);
            if (brand == null)
            {
                return HttpNotFound();
            }
            return View(brand);
        }

        // POST: Admin/Brands/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,Logo,Description,Order,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Status")] Brand brand)
        {
            if (ModelState.IsValid)
            {
                db.Entry(brand).State = EntityState.Modified;
                DateTime now = DateTime.Now;
                brand.UpdatedDate = now;
                brand.UpdatedBy = Session[CommonConstants.USER_SESSION].ToString();
                db.SaveChanges();
                SetAlert("Sửa nhãn hàng thành công", "success");
                return RedirectToAction("Index");
            }
            return View(brand);
        }

        public JsonResult Delete(long id)
        {

            Brand brand = db.Brands.Find(id);
            db.Brands.Remove(brand);
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
