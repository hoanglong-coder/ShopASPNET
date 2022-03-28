using PROJECT_WEBSITE.Data.ModelCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PROJECT_WEBSITE.WebAPP.Areas.Admin.Models
{
    public class OrderDetailModel
    {
        public MProduct Product { get; set; }
        public long OrderId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public string DVT { get; set; }

        public OrderDetailModel(MProduct product, long orderId, int quantity, decimal price, decimal totalprice, string dvt)
        {
            Product = product;
            OrderId = orderId;
            Quantity = quantity;
            Price = price;
            TotalPrice = totalprice;
            DVT = dvt;
        }
    }
}