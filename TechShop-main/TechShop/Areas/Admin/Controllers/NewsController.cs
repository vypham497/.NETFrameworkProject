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
    public class NewsController : BaseController
    {
        private TechShopDbContext db = new TechShopDbContext();

        // GET: Admin/News
        public ActionResult Index()
        {
            var newses = db.Newses.Include(n => n.Category);
            ViewBag.Category = db.Categories.Where(x => x.Status == true).ToList();
            return View(newses.ToList());
        }
        [HttpPost]
        public ActionResult Index(long Category, string txtSearch)
        {
            var model = db.Newses.Include(n => n.Category).ToList();
            ViewBag.Category = db.Categories.Where(x => x.Status == true).ToList();
            if (String.IsNullOrEmpty(txtSearch) && Category == 0)
            {
                ViewBag.Count = 0;
            }
            else
            {
                if (Category == 0)
                {
                    model = model.Where(x => x.Title.Contains(txtSearch)).ToList();
                }
                else
                {

                    model = model.Where(x => x.Title.Contains(txtSearch) && x.CategoryID==Category).ToList();
                }
                ViewBag.Count = model.Count;
            }
            return View(model);
        }

        // GET: Admin/News/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.Newses.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        // GET: Admin/News/Create
        public ActionResult Create()
        {
            SetViewBag();
            return View();
        }

        // POST: Admin/News/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "ID,Title,ShortTitle,MetaTitle,Description,ContentHtml,Images,MetaKeywords,MetaDescription,Status,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,PublishedDate,RelatedNewes,CategoryID,ViewCount,Source,UpTopNew,UpTopHot,RelatedProduct")] News news)
        {
            if (ModelState.IsValid)
            {
                DateTime now = DateTime.Now;
                news.CreatedDate = now;
                news.CreatedBy = Session[CommonConstants.USER_SESSION].ToString();
                news.MetaTitle = ConvertToSEO.Convert(news.ShortTitle);
                db.Newses.Add(news);
                db.SaveChanges();
                SetAlert("Thêm bài viết thành công", "success");
                return RedirectToAction("Index");
            }

            SetViewBag();
            return View(news);
        }

        // GET: Admin/News/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.Newses.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            SetViewBag(news.CategoryID);
            return View(news);
        }

        // POST: Admin/News/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "ID,Title,ShortTitle,MetaTitle,Description,ContentHtml,Images,MetaKeywords,MetaDescription,Status,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,PublishedDate,RelatedNewes,CategoryID,ViewCount,Source,UpTopNew,UpTopHot,RelatedProduct")] News news)
        {
            if (ModelState.IsValid)
            {
                db.Entry(news).State = EntityState.Modified;
                DateTime now = DateTime.Now;
                news.UpdatedDate = now;
                news.UpdatedBy = Session[CommonConstants.USER_SESSION].ToString();
                news.MetaTitle = ConvertToSEO.Convert(news.ShortTitle);
                db.SaveChanges();
                SetAlert("Sửa bài viết thành công", "success");
                return RedirectToAction("Index");
            }
            SetViewBag(news.CategoryID);
            return View(news);
        }

        public ActionResult Delete(long id)
        {

            News news = db.Newses.Find(id);
            db.Newses.Remove(news);
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
            ViewBag.CategoryID = new SelectList(db.Categories.Where(x => x.Status == true).ToList(), "ID", "Title", selectedID);
        }
    }
}
