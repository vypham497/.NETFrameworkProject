using TechShop.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TechShop.Common;
using TechShop.Models;

namespace TechShop.Areas.Admin.Controllers
{
    [HandleError]
    public class LoginController : Controller
    {
        // GET: Admin/Login
        public ActionResult Index()
        {
            return View();
        }
        private TechShopDbContext db = new TechShopDbContext();
        public User GetById(string userName)
        {
            return db.Users.SingleOrDefault(x => x.UserName == userName && x.GroupID=="ADMIN");
        }
        public int Check(string userName, string passWord)
        {
            var result = db.Users.SingleOrDefault(x => x.UserName == userName && x.GroupID == "ADMIN");
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

        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = Check(model.UserName, Encryptor.MD5Hash(model.Password));
                if (result==1)
                {
                    var user = GetById(model.UserName);
                    var userSession = new UserLogin();
                    userSession.UserName = user.UserName;
                    Session.Add(CommonConstants.USER_SESSION,userSession.UserName);
                    return RedirectToAction("Index","Home");
                }
                else if(result==0)
                {
                    ModelState.AddModelError("", "Tài khoản không tồn tại hoặc không có quyền truy cập");
                }
                else
                {
                    ModelState.AddModelError("", "Mật khẩu không đúng");
                }
            }
            return View("Index");
      
            
        }
        public ActionResult Logout()
        {
            User user = db.Users.Find(Session[CommonConstants.USER_SESSION].ToString());
            user.LastLoginDate = DateTime.Now;
            db.SaveChanges();
            Session.Remove(CommonConstants.USER_SESSION);
            return RedirectToAction("Index","Login");
        }
    }
}