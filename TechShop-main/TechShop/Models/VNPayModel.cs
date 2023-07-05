using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TechShop.Models
{
    public class VNPayModel
    {
        public string ordertype { set; get; }
        public string Amount { set; get; }
        public string OrderDescription { set; get; }
        public string bankcode { set; get; }
        public string language { set; get; }
    }
}