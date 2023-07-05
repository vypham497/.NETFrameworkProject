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
    public class FootersController : BaseController
    {
        private TechShopDbContext db = new TechShopDbContext();

        // GET: Admin/Footers
        public ActionResult Index()
        {
            return View(db.Footers.ToList());
        }

        // GET: Admin/Footers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Footer footer = db.Footers.Find(id);
            if (footer == null)
            {
                return HttpNotFound();
            }
            footer.ContentHtml = WebUtility.HtmlDecode(footer.ContentHtml);
            return View(footer);
        }

        // GET: Admin/Footers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Footers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "ID,Title,ContentHtml,Status")] Footer footer)
        {
            if (ModelState.IsValid)
            {
                db.Footers.Add(footer);
                db.SaveChanges();
                SetAlert("Thêm footer thành công", "success");
                return RedirectToAction("Index");
            }

            return View(footer);
        }

        // GET: Admin/Footers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Footer footer = db.Footers.Find(id);
            if (footer == null)
            {
                return HttpNotFound();
            }
            return View(footer);
        }

        // POST: Admin/Footers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "ID,Title,ContentHtml,Status")] Footer footer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(footer).State = EntityState.Modified;
                db.SaveChanges();
                SetAlert("Sửa footer thành công", "success");
                return RedirectToAction("Index");
            }
            return View(footer);
        }

        public ActionResult Delete(string id)
        {

            Footer footer = db.Footers.Find(id);
            db.Footers.Remove(footer);
            db.SaveChanges();
            return Json(new { status= true });
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
