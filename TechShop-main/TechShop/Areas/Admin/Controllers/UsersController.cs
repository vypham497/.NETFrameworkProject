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
    public class UsersController : BaseController
    {
        private TechShopDbContext db = new TechShopDbContext();

        public int CheckUserName(string userName)
        {
            var result = db.Users.SingleOrDefault(x => x.UserName == userName);
            if (result != null)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        public ActionResult Index()
        {
            return View(db.Users.Where(x=>x.GroupID== "MEMBER").ToList());
        }

        // GET: Admin/Users/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            if (user.GroupID == "ADMIN")
            {
                return RedirectToAction("Index");
            }
            else
            {
                user.Password = Encryptor.MD5Hash(user.Password);
                return View(user);
            }
        }

        // GET: Admin/Users/Create
        public ActionResult Create()
        {
            ViewBag.GroupID = new SelectList(db.UserGroups.Where(x => x.IsActived == true && x.IsDeleted == false).ToList(), "ID", "Name");
            return View();
        }

        // POST: Admin/Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserName,Password,PasswordLevel2,Email,Mobile,Name,Address,Sex,UpdatedDate,UpdatedBy,LastLoginDate,LastChangePassword,GroupID,CreatedDate,CreatedBy")] User user)
        {
            if (ModelState.IsValid)
            {
                var result = CheckUserName(user.UserName);
                if (result == 1)
                {
                    
                    ModelState.AddModelError("", "User Name đã tồn tại");
                }
                else
                {
                    DateTime now = DateTime.Now;
                    user.UpdatedDate = now;
                    user.Password = Encryptor.MD5Hash(user.Password);
                    user.UpdatedBy = Session[CommonConstants.USER_SESSION].ToString();
                    db.Users.Add(user);
                    db.SaveChanges();
                    SetAlert("Thêm user thành công", "alert-success");
                    return RedirectToAction("Index");
                }

            }
            ViewBag.GroupID = new SelectList(db.UserGroups.Where(x => x.IsActived == true && x.IsDeleted == false).ToList(), "ID", "Name");
            return View(user);
        }

        // GET: Admin/Users/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            if (user.GroupID == "ADMIN")
            {
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.GroupID = new SelectList(db.UserGroups.Where(x => x.IsActived == true && x.IsDeleted == false).ToList(), "ID", "Name");
                return View(user);
            }
            
        }

        // POST: Admin/Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserName,Password,PasswordLevel2,Email,Mobile,Name,Address,Sex,UpdatedDate,UpdatedBy,LastLoginDate,LastChangePassword,GroupID,CreatedDate,CreatedBy")] User user)
        {

            if (ModelState.IsValid)
            {
                DateTime now = DateTime.Now;
                user.UpdatedDate = now;
                user.Password = Encryptor.MD5Hash(user.Password);
                user.UpdatedBy = Session[CommonConstants.USER_SESSION].ToString();
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                SetAlert("Sửa user thành công", "success");
                return RedirectToAction("Index");
            }
            ViewBag.GroupID = new SelectList(db.UserGroups.Where(x => x.IsActived == true && x.IsDeleted == false).ToList(), "ID", "Name");
            return View(user);
        }
        public ActionResult Delete(string id)
        {

            User user = db.Users.Find(id);
            db.Users.Remove(user);
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
