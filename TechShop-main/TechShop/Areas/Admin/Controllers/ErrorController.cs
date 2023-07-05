using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TechShop.Areas.Admin.Controllers
{
    public class ErrorController : BaseController
    {
        // GET: Admin/Error
        public ActionResult Error404()
        {
            Response.StatusCode = 404;
            return View();
        }
    }
}