using PROJECT_WEBSITE.Data.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJECT_WEBSITE.Data.ModelCustom
{
    public class MProduct
    {
        public int STT { get; set; }

        public int ProductID { get; set; }

        public string KyTuCaterory { get; set; }

        public string Name { get; set; }

        public string MetaTitle { get; set; }

        public int? ParentProductID { get; set; }

        public string Image { get; set; }

        public string MoreImage { get; set; }

        public decimal? PriceOut { get; set; }

        public decimal? PriceIput { get; set; }

        public decimal? Pricewholesale { get; set; }

        public int? CountProduct { get; set; }

        public int? UnitID { get; set; }

        public string UnitName { get; set; }

        public DateTime? CreateDate { get; set; }

        public int? ProductCategoryID { get; set; }

        public bool? ProductStatus { get; set; }

        public bool? Display { get; set; }

        public decimal? PricePromotion { get; set; }

        public decimal? PricewholesalePromotion { get; set; }

        public string SupplierName { get; set; }

        public string CategoryName { get; set; }

        public string Discription { get; set; }

        public string TradeMark { get; set; }

        public string TradeOrigin { get; set; }

        public string Ingredient { get; set; }

        public string Production { get; set; }

        public string Expiry { get; set; }

        public string UserManual { get; set; }

        public string CareInstructions { get; set; }

        public string Packing { get; set; }
    }

    public class SearchProduct
    {
        public string querysearch { get; set; }

        public decimal? GiaLeTu { get; set; }

        public decimal? GiaLeDen { get; set; }

        public decimal? GiaSiTu { get; set; }

        public decimal? GiaSiDen { get; set; }

        public bool? SoLuong { get; set; }

        public int? UnitID { get; set; }

        public DateTime? TuNgay { get; set; }

        public DateTime? DenNgay { get; set; }

        public bool? TinhTrang { get; set; }

        public int? ProductCategoryID { get; set; }
    }

    public class DeleteProduct
    {
        public bool Check { get; set; }

        public string Result { get; set; }
    }
}
