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
    public class FeedbacksController : BaseController
    {
        private TechShopDbContext db = new TechShopDbContext();

        // GET: Admin/Feedbacks
        public ActionResult Index()
        {
            return View(db.Feedbacks.ToList());
        }

        // GET: Admin/Feedbacks/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feedback feedback = db.Feedbacks.Find(id);
            if (feedback == null)
            {
                return HttpNotFound();
            }
            return View(feedback);
        }

        // GET: Admin/Feedbacks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Feedbacks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,Name,Company,Address,Phone,Image,Email,Message,CreatedDate,IsReaded")] Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                DateTime now = DateTime.Now;
                feedback.CreatedDate = now;
                db.Feedbacks.Add(feedback);
                db.SaveChanges();
                SetAlert("Thêm feedback thành công", "success");
                return RedirectToAction("Index");
            }

            return View(feedback);
        }

        // GET: Admin/Feedbacks/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feedback feedback = db.Feedbacks.Find(id);
            if (feedback == null)
            {
                return HttpNotFound();
            }
            return View(feedback);
        }

        // POST: Admin/Feedbacks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,Name,Company,Address,Phone,Image,Email,Message,CreatedDate,IsReaded")] Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                db.Entry(feedback).State = EntityState.Modified;
                DateTime now = DateTime.Now;
                feedback.CreatedDate = now;
                db.SaveChanges();
                SetAlert("Sửa feedback thành công", "success");
                return RedirectToAction("Index");
            }
            return View(feedback);
        }

        public ActionResult Delete(long id)
        {
            Feedback feedback = db.Feedbacks.Find(id);
            db.Feedbacks.Remove(feedback);
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
