using TechShop.Models;
using TechShop.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TechShop.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        private TechShopDbContext db = new TechShopDbContext();
        // GET: Home
        public List<Product> NewProduct(int top)
        {
            return db.Products.OrderByDescending(x => x.CreatedDate).Take(top).ToList();
        }
        public List<Product> SaleProduct(int top)
        {
            return db.Products.OrderByDescending(x => x.PercentSale).Take(top).ToList();
        }
        [ChildActionOnly]
        [OutputCache(Duration = 3600 * 24)]
        public PartialViewResult SearchOption()
        {
            var model = db.ProductCategories.Where(x => x.Status == true && x.ParentID.HasValue).OrderBy(x => x.Order).ToList();
            return PartialView(model);
        }
        [ChildActionOnly]
        [OutputCache(Duration = 3600 * 24)]
        public PartialViewResult ListNewProduct()
        {
            ViewBag.NewProducts = NewProduct(8);
            return PartialView();
        }
        [ChildActionOnly]
        [OutputCache(Duration = 3600 * 24)]
        public PartialViewResult ListSaleProduct()
        {
            ViewBag.SaleProducts = SaleProduct(8);
            return PartialView();
        }
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult Login()
        {
            if (Session[SessionMember.UserSession] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        public ActionResult Logout()
        {
            User user = db.Users.Find(Session[SessionMember.UserSession].ToString());
            user.LastLoginDate = DateTime.Now;
            db.SaveChanges();
            Session.Remove(SessionMember.UserSession);
            return RedirectToAction("Index", "Home");
        }

        //[ChildActionOnly]
        //[OutputCache(Duration =3600 * 24)]
        public ActionResult MainNavMenu()
        {
            ViewBag.Username = "Đăng nhập";
            ViewBag.Link = "/dang-nhap";
            if (Session[SessionMember.UserSession] != null)
            {
                ViewBag.Username = "Xin chào, " + Session[SessionMember.UserSession].ToString();
            }

            return PartialView(db.Menus.Where(x => x.GroupID == "top" && x.IsLocked == true).OrderBy(x => x.Order).ToList());
        }
        [ChildActionOnly]
        [OutputCache(Duration = 3600 * 24)]
        public ActionResult Footer()
        {
            var model = db.Footers.SingleOrDefault(x => x.Status == true);
            return PartialView(model);
        }
        [ChildActionOnly]
        [OutputCache(Duration = 3600 * 24)]
        public ActionResult Brand()
        {
            var model = db.Brands.Where(x => x.Status == true).OrderBy(x => x.Order).ToList();
            return PartialView(model);
        }
        [ChildActionOnly]
        [OutputCache(Duration = 3600 * 24)]
        public ActionResult Slide()
        {
            return PartialView(db.Slides.Where(x => x.Status == true).OrderBy(x => x.Order).ToList());
        }
        [ChildActionOnly]
        [OutputCache(Duration = 3600 * 24)]
        public ActionResult HomeCategory()
        {
            return PartialView(db.ProductCategories.Where(x => x.Status == true).OrderBy(x => x.Order).ToList());
        }
        [ChildActionOnly]
        public PartialViewResult HeaderCart()
        {
            var cart = Session[SessionMember.CartSession];
            var list = new List<CartItem>();
            if (cart != null)
            {
                list = (List<CartItem>)cart;
            }
            return PartialView(list);
        }
        [ChildActionOnly]
        public PartialViewResult RecentProduct()
        {
            var recentProduct = Session[SessionMember.RecentProduct];
            var list = new List<RecentProduct>();
            if (recentProduct != null)
            {
                list = (List<RecentProduct>)recentProduct;
            }
            return PartialView(list);
        }
        [ChildActionOnly]
        [OutputCache(Duration = 3600 * 24)]
        public PartialViewResult Feedback()
        {
            ViewBag.feedback = db.Feedbacks.Where(x => x.IsReaded == true).OrderBy(x => x.CreatedDate).ToList();
            return PartialView();

        }
        [ChildActionOnly]
        [OutputCache(Duration = 3600 * 24)]
        public PartialViewResult DanhGia()
        {
            ViewBag.DanhGiaMoiNhat = db.Newses.Where(x => x.CategoryID == 1).OrderByDescending(x => x.PublishedDate).Take(1).ToList();
            ViewBag.DanhGia = db.Newses.Where(x => x.CategoryID == 1 && x.ID != 65).OrderByDescending(x => x.PublishedDate).Take(4).ToList();
            return PartialView();
        }
        public User GetById(string userName)
        {
            return db.Users.SingleOrDefault(x => x.UserName == userName && x.GroupID == "MEMBER");
        }
        public int Check(string userName, string passWord)
        {
            var result = db.Users.SingleOrDefault(x => x.UserName == userName && x.GroupID == "MEMBER");
            if (result == null)
            {
                return 0;
            }
            else
            {
                if (result.Password == passWord)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
        }
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
        public JsonResult Log(string Username, string Password)
        {
            var result = Check(Username, Encryptor.MD5Hash(Password));
            string msg = "";
            bool status = false;
            if (result == 1)
            {
                var user = GetById(Username);
                var userSession = new MemberLogin();
                userSession.UserID = user.UserName;
                Session.Add(SessionMember.UserSession, userSession.UserID);
                status = true;
            }
            else if (result == 0)
            {
                msg = "Tài khoản không tồn tại hoặc không có quyền truy cập";
            }
            else
            {
                msg = "Mật khẩu không đúng";
            }
            return Json(new { msg = msg, status = status }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Reg(User d)
        {
            var check = CheckUserName(d.UserName);
            if (check == 0)
            {
                User user = new User();
                user = d;
                user.Password = Encryptor.MD5Hash(d.Password);
                user.GroupID = "MEMBER";
                user.CreatedDate = DateTime.Now;
                db.Users.Add(user);
                db.SaveChanges();
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = false, msg = "Tên đăng nhập này đã tồn tại !" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}