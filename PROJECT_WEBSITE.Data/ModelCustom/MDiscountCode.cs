using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJECT_WEBSITE.Data.ModelCustom
{
    public class MDiscountCode
    {
        public int STT { get; set; }

        public int DiscountCodeID { get; set; }

        public string Name { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? CreateDate { get; set; }

        public int? PercentCart { get; set; }

        public decimal? TotalCart { get; set; }

        public int? DistcountCount { get; set; }

        public bool? DiscountStatus { get; set; }
    }
}
