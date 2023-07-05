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
    public class MenusController : BaseController
    {
        private TechShopDbContext db = new TechShopDbContext();

        // GET: Admin/Menus
        public ActionResult Index()
        {
            var menus = db.Menus.Include(m => m.MenuType);
            return View(menus.ToList());
        }

        // GET: Admin/Menus/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Menu menu = db.Menus.Find(id);
            if (menu == null)
            {
                return HttpNotFound();
            }
            return View(menu);
        }

        // GET: Admin/Menus/Create
        public ActionResult Create()
        {
            ViewBag.GroupID = new SelectList(db.MenuTypes, "ID", "Name");
            return View();
        }

        // POST: Admin/Menus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Description,Text,Link,Target,Order,CssClass,IsLocked,IsDeleted,GroupID,ParentID,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] Menu menu)
        {
            if (ModelState.IsValid)
            {
                DateTime now = DateTime.Now;
                menu.CreatedDate = now;
                menu.CreatedBy = Session[CommonConstants.USER_SESSION].ToString();
                db.Menus.Add(menu);
                db.SaveChanges();
                SetAlert("Thêm menu thành công", "success");
                return RedirectToAction("Index");
            }

            ViewBag.GroupID = new SelectList(db.MenuTypes, "ID", "Name", menu.GroupID);
            return View(menu);
        }

        // GET: Admin/Menus/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Menu menu = db.Menus.Find(id);
            if (menu == null)
            {
                return HttpNotFound();
            }
            ViewBag.GroupID = new SelectList(db.MenuTypes, "ID", "Name", menu.GroupID);
            return View(menu);
        }

        // POST: Admin/Menus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Description,Text,Link,Target,Order,CssClass,IsLocked,IsDeleted,GroupID,ParentID,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] Menu menu)
        {
            if (ModelState.IsValid)
            {
                DateTime now = DateTime.Now;
                menu.UpdatedDate = now;
                menu.UpdatedBy = Session[CommonConstants.USER_SESSION].ToString();
                db.Entry(menu).State = EntityState.Modified;
                db.SaveChanges();
                SetAlert("Sửa menu thành công", "success");
                return RedirectToAction("Index");
            }
            ViewBag.GroupID = new SelectList(db.MenuTypes, "ID", "Name", menu.GroupID);
            return View(menu);
        }

        public ActionResult Delete(string id)
        {

            Menu menu = db.Menus.Find(id);
            db.Menus.Remove(menu);
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
