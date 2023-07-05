using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using TechShop.Common;
using TechShop.Models;
using Newtonsoft.Json;
namespace TechShop.Controllers
{
    public class OdersController : Controller
    {
        private TechShopDbContext db = new TechShopDbContext();

        // GET: Oders
        public ActionResult Index()
        {          
            var user = Session[SessionMember.UserSession];
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                var CustomerID = user.ToString();
                var model = db.Orders.Where(x => x.CustomerID == CustomerID).OrderByDescending(x => x.CreatedDate).ToList();
                return View(model);
            }


        }
        // GET: Oders/Details/
        public ActionResult Details(long? id)
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
            dynamic model = (from p in db.Products
                             join o in db.OrderDetails
                             on p.ID equals o.ProductID
                             where o.OrderID
                             .Equals(order.ID)
                             select new
                             {
                                 proname = p.Title,
                                 sl = o.Quantity,
                                 gia = o.Price,
                                 ngay = order.CreatedDate,
                                 sdt = order.ShipMobile,
                                 ma = order.ID,
                                 trangthai = order.Status,
                                 image = p.Thumb,
                                 tongtien = order.TotalPrice,
                             }).ToList();
            ViewBag.prd = JsonConvert.SerializeObject(model);
            return View(order);
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
