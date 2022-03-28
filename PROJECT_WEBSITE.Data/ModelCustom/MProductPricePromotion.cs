using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJECT_WEBSITE.Data.ModelCustom
{
    public class MProductPricePromotion
    {
        public int STT { get; set; }

        public int PricePromotionID { get; set; }

        public int ProductID { get; set; }

        public DateTime? CreateDate { get; set; }

        public decimal? PricePromotion { get; set; }

        public decimal? PricewholesalePromotion { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool? PromotionStatus { get; set; }
    }
}
