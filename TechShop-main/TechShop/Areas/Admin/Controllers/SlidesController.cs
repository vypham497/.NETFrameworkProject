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
    public class SlidesController : BaseController
    {
        private TechShopDbContext db = new TechShopDbContext();

        // GET: Admin/Slides
        public ActionResult Index()
        {
            var slides = db.Slides.Include(s => s.GroupSlide);
            return View(slides.ToList());
        }

        // GET: Admin/Slides/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Slide slide = db.Slides.Find(id);
            if (slide == null)
            {
                return HttpNotFound();
            }
            return View(slide);
        }

        // GET: Admin/Slides/Create
        public ActionResult Create()
        {
            ViewBag.GroupID = new SelectList(db.GroupSlides, "ID", "Name");
            return View();
        }

        // POST: Admin/Slides/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Description,Link,Images,Order,GroupID,Status")] Slide slide)
        {
            if (ModelState.IsValid)
            {
                db.Slides.Add(slide);
                db.SaveChanges();
                SetAlert("Thêm thành công", "success");
                return RedirectToAction("Index");
            }

            ViewBag.GroupID = new SelectList(db.GroupSlides, "ID", "Name", slide.GroupID);
            return View(slide);
        }

        // GET: Admin/Slides/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Slide slide = db.Slides.Find(id);
            if (slide == null)
            {
                return HttpNotFound();
            }
            ViewBag.GroupID = new SelectList(db.GroupSlides, "ID", "Name", slide.GroupID);
            return View(slide);
        }

        // POST: Admin/Slides/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Description,Link,Images,Order,GroupID,Status")] Slide slide)
        {
            if (ModelState.IsValid)
            {
                db.Entry(slide).State = EntityState.Modified;
                db.SaveChanges();
                SetAlert("Sửa slide thành công", "success");
                return RedirectToAction("Index");
            }
            ViewBag.GroupID = new SelectList(db.GroupSlides, "ID", "Name", slide.GroupID);
            return View(slide);
        }

        public ActionResult Delete(int id)
        {

            Slide slide = db.Slides.Find(id);
            db.Slides.Remove(slide);
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
