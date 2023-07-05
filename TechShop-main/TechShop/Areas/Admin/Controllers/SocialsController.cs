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
    public class SocialsController : BaseController
    {
        private TechShopDbContext db = new TechShopDbContext();

        // GET: Admin/Socials
        public ActionResult Index()
        {
            return View(db.Socials.ToList());
        }

        // GET: Admin/Socials/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Social social = db.Socials.Find(id);
            if (social == null)
            {
                return HttpNotFound();
            }
            return View(social);
        }

        // GET: Admin/Socials/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Socials/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "ID,Title,Icon,Link,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Order,Status")] Social social)
        {
            if (ModelState.IsValid)
            {

                DateTime now = DateTime.Now;
                social.CreatedDate = now;
                social.CreatedBy = Session[CommonConstants.USER_SESSION].ToString();
                db.Socials.Add(social);
                db.SaveChanges();
                SetAlert("Thêm social thành công", "success");
                return RedirectToAction("Index");
            }

            return View(social);
        }

        // GET: Admin/Socials/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Social social = db.Socials.Find(id);
            if (social == null)
            {
                return HttpNotFound();
            }
            return View(social);
        }

        // POST: Admin/Socials/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "ID,Title,Icon,Link,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Order,Status")] Social social)
        {
            if (ModelState.IsValid)
            {
                db.Entry(social).State = EntityState.Modified;
                DateTime now = DateTime.Now;
                social.UpdatedDate = now;
                social.UpdatedBy = Session[CommonConstants.USER_SESSION].ToString();
                db.Socials.Add(social);
                db.SaveChanges();
                SetAlert("Sửa social thành công", "success");
                return RedirectToAction("Index");
            }
            return View(social);
        }

        // GET: Admin/Socials/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Social social = db.Socials.Find(id);
            if (social == null)
            {
                return HttpNotFound();
            }
            return View(social);
        }

        public ActionResult Delete(int id)
        {

            Social social = db.Socials.Find(id);
            db.Socials.Remove(social);
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
