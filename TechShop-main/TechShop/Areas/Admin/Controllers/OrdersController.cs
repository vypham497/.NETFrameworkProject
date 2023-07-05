using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TechShop.Models;
using TechShop.Areas.Admin.Models;

namespace TechShop.Areas.Admin.Controllers
{
    [HandleError]
    public class OrdersController : BaseController
    {
        private TechShopDbContext db = new TechShopDbContext();

        // GET: Admin/Orders
        public ActionResult Index()
        {
            return View(db.Orders.OrderByDescending(x=>x.CreatedDate).ToList());
        }

        [HttpPost]
        public ActionResult Index(int Status)
        {

            var model = db.Orders.OrderByDescending(x => x.CreatedDate).ToList();
           
            if(Status == 1)
            {
                model = db.Orders.Where(x => x.Status == "Chưa xử lý").OrderByDescending(x => x.CreatedDate).ToList();
            }
            else if(Status == 2)
            {
                model = db.Orders.Where(x => x.Status == "Đã xử lý").OrderByDescending(x => x.CreatedDate).ToList();
            }

            return View(model);

        }
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            var query = from p in db.Products
                        join o in db.OrderDetails on p.ID equals o.ProductID
                        select new OrderCustom { product = p, orderDetail=o };
            ViewBag.orderDetail = query.Where(x => x.orderDetail.OrderID == id).ToList();
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

       
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CreatedDate,CustomerID,ShipName,ShipEmail,ShipMobile,ShipAddress,Status,Note,Total,TotalPrice")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(order);
        }

        public JsonResult Delete(long id)
        {

            Order order = db.Orders.Find(id);
            db.OrderDetails.RemoveRange(db.OrderDetails.Where(c => c.OrderID == id));
            db.Orders.Remove(order);
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
