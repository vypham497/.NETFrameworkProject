using TechShop.Models;
using TechShop.Common;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace TechShop.Controllers
{
    [HandleError]
    public class CartController : Controller
    {
        private TechShopDbContext db = new TechShopDbContext();
        private const string CartSession = "CartSession";
        public ActionResult Index()
        {
            var sess = Session[SessionMember.UserSession];
            if (sess == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                var cart = Session[CartSession];
                var list = new List<CartItem>();
                if (cart != null)
                {
                    list = (List<CartItem>)cart;
                    return View(list);
                }
                return RedirectToAction("Index", "Home");
            }

        }
        [HttpGet]
        public ActionResult CheckOut()
        {
            var cart = Session[CartSession];
            var list = new List<CartItem>();
            if (cart != null)
            {
                list = (List<CartItem>)cart;
                return View(list);
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult CheckOut(string ShipName, string ShipEmail, string ShipMobile, string ShipAddress, string Note, string payMethod)
        {
            if (payMethod == "cod")
            {
                var order = new TechShop.Models.Order();
                var cart = (List<CartItem>)Session[CartSession];
                order.CreatedDate = DateTime.Now;
                order.ShipName = ShipName;
                order.ShipEmail = ShipEmail;
                order.ShipMobile = ShipMobile;
                order.ShipAddress = ShipAddress;
                order.CustomerID = Session[SessionMember.UserSession].ToString();
                order.Note = Note;
                order.TotalPrice = cart.Sum(x => x.Quantity * x.Product.Price);
                order.Status = "cod";
                db.Orders.Add(order);
                db.SaveChanges();
                var id = order.ID;

                foreach (var item in cart)
                {
                    Product product = db.Products.Find(item.Product.ID);
                    product.Quantity -= item.Quantity;
                    var orderDetail = new OrderDetail();
                    orderDetail.ProductID = item.Product.ID;
                    orderDetail.OrderID = id;
                    orderDetail.Quantity = item.Quantity;
                    orderDetail.Price = item.Product.Price * item.Quantity;
                    db.OrderDetails.Add(orderDetail);
                    db.SaveChanges();
                }
                var total = cart.Sum(x => x.Quantity * x.Product.Price);
                string content = System.IO.File.ReadAllText(Server.MapPath("~/Assets/Client/template/1.cshtml"));
                content = content.Replace("{{CustomerName}}", ShipName);
                content = content.Replace("{{Phone}}", ShipMobile);
                content = content.Replace("{{Email}}", ShipEmail);
                content = content.Replace("{{Address}}", ShipAddress);
                content = content.Replace("{{Total}}", total.Value.ToString("N0"));
                var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();
                new MailHelper().SendEmail(order.ShipEmail, "Xác nhận đơn hàng mới từ TechShop", content);
                new MailHelper().SendEmail(toEmail, "TechShop", content);
                Session.Remove(SessionMember.CartSession);


            }
            else if (payMethod == "paypal")
            {
                var order = new TechShop.Models.Order();
                var cart = (List<CartItem>)Session[CartSession];
                order.CreatedDate = DateTime.Now;
                order.ShipName = ShipName;
                order.ShipEmail = ShipEmail;
                order.ShipMobile = ShipMobile;
                order.ShipAddress = ShipAddress;
                order.CustomerID = Session[SessionMember.UserSession].ToString();
                order.Note = Note;
                order.TotalPrice = cart.Sum(x => x.Quantity * x.Product.Price);
                order.Status = "no paypal";
                db.Orders.Add(order);
                db.SaveChanges();
                var id = order.ID;

                foreach (var item in cart)
                {
                    Product product = db.Products.Find(item.Product.ID);
                    product.Quantity -= item.Quantity;
                    var orderDetail = new OrderDetail();
                    orderDetail.ProductID = item.Product.ID;
                    orderDetail.OrderID = id;
                    orderDetail.Quantity = item.Quantity;
                    orderDetail.Price = item.Product.Price * item.Quantity;
                    db.OrderDetails.Add(orderDetail);
                    db.SaveChanges();
                }
                Session["OrderID"] = id;
                return Redirect("/paypal");
            }
            else
            {
                return Redirect("/vnpay");
            }
            return Redirect("/hoan-thanh");
        }
        public JsonResult Delete(long id)
        {
            var sessionCart = (List<CartItem>)Session[CartSession];
            sessionCart.RemoveAll(x => x.Product.ID == id);
            Session[CartSession] = sessionCart;
            var Qty = sessionCart.Sum(x => x.Quantity);
            var ToTalPrice = sessionCart.Sum(x => x.Quantity * x.Product.Price).Value.ToString("N0") + " đ";
            var info = new { Qty, ToTalPrice };
            return Json(info, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteAll()
        {
            Session[CartSession] = null;
            return Json(new
            {
                status = true

            });
        }
        public JsonResult Update(string cartModel)
        {
            var jsonCart = new JavaScriptSerializer().Deserialize<List<CartItem>>(cartModel);
            var sessionCart = (List<CartItem>)Session[CartSession];
            foreach (var item in sessionCart)
            {
                var jsonItem = jsonCart.SingleOrDefault(x => x.Product.ID == item.Product.ID);
                if (jsonItem != null)
                {
                    item.Quantity = jsonItem.Quantity;
                }
            }
            Session[CartSession] = sessionCart;
            var List = new List<CartItem>();
            List = (List<CartItem>)Session[CartSession];
            var Qty = List.Sum(x => x.Quantity);
            var Total = List.Sum(x => x.Product.Price * x.Quantity).Value.ToString("N0") + " đ";
            var info = new { Qty, Total };
            return Json(info, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Add(int quantity, long id)
        {
            var product = db.Products.Find(id);
            var cart = Session[CartSession];
            var sess = Session[SessionMember.UserSession];
            if (sess == null)
            {
                return Json(new { status = false });
            }
            else
            {
                if (cart != null)
                {
                    var list = (List<CartItem>)cart;
                    if (list.Exists(x => x.Product.ID == id))
                    {
                        foreach (var item in list)
                        {
                            if (item.Product.ID == id)
                            {
                                item.Quantity += quantity;
                            }
                        }
                    }
                    else
                    {
                        var item = new CartItem();
                        item.Product = product;
                        item.Quantity = quantity;
                        list.Add(item);
                    }
                    Session[CartSession] = list;
                }
                else
                {
                    //tạo mới cart item
                    var item = new CartItem();
                    item.Product = product;
                    item.Quantity = quantity;
                    var list = new List<CartItem>();
                    list.Add(item);
                    //gán vào session
                    Session[CartSession] = list;

                }
                var List = new List<CartItem>();
                List = (List<CartItem>)Session[CartSession];
                var Qty = List.Sum(x => x.Quantity);
                var info = new { Qty, status = true };
                return Json(info, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult Success()
        {
            return View();
        }
        public ActionResult Fail()
        {
            return View();
        }
        public ActionResult VNPayDefault()
        {
            return View();
        }
        public ActionResult VNPayReturn()
        {
            return View();
        }
    }
}