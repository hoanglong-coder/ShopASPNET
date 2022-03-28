using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJECT_WEBSITE.Data.ModelCustom
{
    public class MReceiptDetail
    {
        public int STT { get; set; }

        public int ReceitpDetailID { get; set; }

        public int ReceiptID { get; set; }

        public int ProductID { get; set; }

        public string ProductDVT { get; set; }

        public string ProductName { get; set; }

        public decimal? PriceIput { get; set; }

        public int? ReceiptCount { get; set; }

        public decimal? TotalPrice { get; set; }

        public string KyTuCaterory { get; set; }

        public string NameCategory { get; set; }
    }
}
