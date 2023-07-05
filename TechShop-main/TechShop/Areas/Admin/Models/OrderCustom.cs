using TechShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TechShop.Areas.Admin.Models
{
    public class OrderCustom
    {
        public Product product { get; set; }
        public OrderDetail orderDetail{ get; set; }
    }
}