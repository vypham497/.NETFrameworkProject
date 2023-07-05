using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TechShop.Models;

namespace TechShop.Common
{
    public class ProductView
    {
        public Product Product { get; set; }
        public ProductCategory category { get; set; }
    }
}