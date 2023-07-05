using TechShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TechShop.Areas.Admin.Models
{
    public class ProductWithCategory
    {
      
        public Product Product { get; set; }
        public ProductCategory category { get; set; }
        public Brand brand { get; set; }
    }
}