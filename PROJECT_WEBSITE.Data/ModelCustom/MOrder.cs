using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJECT_WEBSITE.Data.ModelCustom
{
    public class MOrder
    {
        public int STT { get; set; }

        public int OrderID { get; set; }

        public int? UserID { get; set; }

        public int? CustomerID { get; set; }

        public string ShipName { get; set; }

        public string ShipAddress { get; set; }

        public string ShipPhone { get; set; }

        public string ShipEmail { get; set; }
        
        public DateTime? CreateDate { get; set; }

        public string Discription { get; set; }

        public bool? PaymentStatus { get; set; }

        public string PaymentName { get; set; }

        public int? OrderStatus { get; set; }

        public decimal? PriceShip { get; set; }

        public decimal? TotalPrice { get; set; }

        public int? TotalCount { get; set; }

        public decimal? PriceDiscount { get; set; }

        public string UserName { get; set; }
    }

    public class SearchOrder
    {
        public string query { get; set; }

        public DateTime? TuNgay { get; set; }

        public DateTime? DenNgay { get; set; }

        public bool? TrangThai { get; set; }

        public bool? PTThanhToan { get; set; }

    }
}
