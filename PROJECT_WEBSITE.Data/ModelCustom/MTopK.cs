using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJECT_WEBSITE.Data.ModelCustom
{
    public class MTopK
    {
        public int STT { get; set; }

        public int PU { get; set; }

        public List<MProduct> MProduct { get; set; }
    }
}
