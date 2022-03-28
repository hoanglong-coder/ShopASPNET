using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJECT_WEBSITE.Data.ModelCustom
{
    public class MProductUnit
    {
        public int STT { get; set; }

        public int UnitID { get; set; }

        public string Name { get; set; }

        public double? ValueUnit { get; set; }
    }

    public class SearchUnit
    {
        public string query { get; set; }

        public bool? LoaiDVT { get; set; }

        public int? GiaTri { get; set; }
    }
    public class DeleteUnit
    {
        public bool Check { get; set; }

        public string Result { get; set; }
    }
}
