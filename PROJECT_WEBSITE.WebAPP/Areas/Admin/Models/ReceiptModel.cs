using PROJECT_WEBSITE.Data.ModelCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PROJECT_WEBSITE.WebAPP.Areas.Admin.Models
{
    public class ReceiptModel
    {
        public int STT { get; set; }
        public int Productid { get; set; }
        public string KyTuCaterory { get; set; }
        public string NameCategory { get; set; }
        public string ProductName { get; set; }
        public decimal PriceIput { get; set; }
        public int ReceiptCount { get; set; }
        public decimal Total { get; set; }
    }
}