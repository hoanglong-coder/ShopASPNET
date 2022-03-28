using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJECT_WEBSITE.Data.ModelCustom
{
    public class MReceipt
    {
        public int STT { get; set; }

        public int ReceiptID { get; set; }

        public int? UserID { get; set; }

        public int? SupplierID { get; set; }

        public DateTime? CreateDate { get; set; }

        public int? TotalCount { get; set; }

        public decimal? TotalReceiptPrice { get; set; }

        public bool? ReceiptStatus { get; set; }

        public string Description { get; set; }

        public string UserName { get; set; }

        public string SupplierName { get; set; }
    }
}
