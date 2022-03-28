using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJECT_WEBSITE.Data.ModelCustom
{
    public class MExchangeUnit
    {
        public int ExchangeUnitID { get; set; }

        public DateTime? Createdate { get; set; }

        public int ProductID { get; set; }

        public string ProductName { get; set; }

        public string UnitName { get; set; }

        public decimal? PriceInput { get; set; }

        public double ValueUnit { get; set; }

        public int CountProduct { get; set; }

        public string KyTuCaterory { get; set; }

        public string CategoryName { get; set; }

        public int? UnitOut{ get; set; }

        public int? ValueCountUnit { get; set; }

        public int? UserID { get; set; }
    }
    public class MDsExchangeUnit
    {
        public int STT { get; set; }

        public int ExchangeUnitID { get; set; }

        public DateTime? Createdate { get; set; }

        public int? ValueCountUnit { get; set; }

        public int? ProductIDIn { get; set; }

        public int? ProductIDOut { get; set; }

        public string ProductNameIn { get; set; }

        public string ProductNameOut { get; set; }

        public string Result { get; set; }

        public int? UserID { get; set; }

        public string UserName { get; set; }

    }
}
