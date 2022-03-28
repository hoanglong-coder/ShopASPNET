using PROJECT_WEBSITE.Data.ModelCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PROJECT_WEBSITE.WebAPP.Models
{
    [Serializable]
    public class CartItem
    {
        public MProduct Product { get; set; }
        public int Quantity { get; set; }
    }

    public class MDiscountCode
    {
        public int DiscountID { get; set; }

        public string Name { get; set; }

        public int? Percent { get; set; }

        public decimal? Total { get; set; }
    }
}