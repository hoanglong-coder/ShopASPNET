using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJECT_WEBSITE.Data.ModelCustom
{
    public class MFooterCategory
    {
        public int STT { get; set; }

        public int FooterCategoryID { get; set; }

        public string NameCategory { get; set; }

        public int? Display { get; set; }

        public DateTime? CreateDate { get; set; }
    }
}
