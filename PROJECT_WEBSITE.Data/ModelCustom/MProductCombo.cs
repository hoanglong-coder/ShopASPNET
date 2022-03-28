using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJECT_WEBSITE.Data.ModelCustom
{
    public class MProductCombo
    {
        public int STT { get; set; }

        public int ProductID { get; set; }

        public int? ProductComboID { get; set; }

        public int? ParentProductID { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }

        public string MetaTitle { get; set; }

        public string MoreImage { get; set; }

        public decimal? PriceOut { get; set; }

        public decimal? PricePromotion { get; set; }

        public decimal? Pricewholesale { get; set; }

        public decimal? PricewholesalePromotion { get; set; }

        public int? CountProduct { get; set; }

        public DateTime? CreateDate { get; set; }

        public bool? ProductStatus { get; set; }

        public bool? ComboStatus { get; set; }

        public bool? DisplayProduct { get; set; }

        public int? DisplayProductComBo { get; set; }
    
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        //detail

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

    public class SearchProductCombo
    {
        public string querysearch { get; set; }

        public decimal? GiaLeTu { get; set; }

        public decimal? GiaLeDen { get; set; }

        public decimal? GiaSiTu { get; set; }

        public decimal? GiaSiDen { get; set; }

        public bool? SoLuong { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool? TinhTrangCombo { get; set; }

        public bool? TrangThaiCombo { get; set; }
    }
}
