using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJECT_WEBSITE.Data.ModelCustom
{
    public class MFooter
    {
        public int STT { get; set; }

        public int FooterID { get; set; }

        public string Name { get; set; }

        public DateTime? CreateDate { get; set; }

        public int? FooterCategoryID { get; set; }

        public string FooterCategoryName { get; set; }

        public string Detail { get; set; }
    }
}
