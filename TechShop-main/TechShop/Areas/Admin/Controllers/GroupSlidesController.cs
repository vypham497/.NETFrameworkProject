using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TechShop.Models;

namespace TechShop.Areas.Admin.Controllers
{
    [HandleError]
    public class GroupSlidesController : BaseController
    {
        private TechShopDbContext db = new TechShopDbContext();

        // GET: Admin/GroupSlides
        public ActionResult Index()
        {
            return View(db.GroupSlides.ToList());
        }

        // GET: Admin/GroupSlides/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupSlide groupSlide = db.GroupSlides.Find(id);
            if (groupSlide == null)
            {
                return HttpNotFound();
            }
            return View(groupSlide);
        }

        // GET: Admin/GroupSlides/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/GroupSlides/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Description,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,IsActived,IsDeleted")] GroupSlide groupSlide)
        {
            if (ModelState.IsValid)
            {
                db.GroupSlides.Add(groupSlide);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(groupSlide);
        }

        // GET: Admin/GroupSlides/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupSlide groupSlide = db.GroupSlides.Find(id);
            if (groupSlide == null)
            {
                return HttpNotFound();
            }
            return View(groupSlide);
        }

        // POST: Admin/GroupSlides/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Description,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,IsActived,IsDeleted")] GroupSlide groupSlide)
        {
            if (ModelState.IsValid)
            {
                db.Entry(groupSlide).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(groupSlide);
        }

        // GET: Admin/GroupSlides/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupSlide groupSlide = db.GroupSlides.Find(id);
            if (groupSlide == null)
            {
                return HttpNotFound();
            }
            return View(groupSlide);
        }

        // POST: Admin/GroupSlides/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            GroupSlide groupSlide = db.GroupSlides.Find(id);
            db.GroupSlides.Remove(groupSlide);
            db.SaveChanges();
            return RedirectToAction("Index");
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
