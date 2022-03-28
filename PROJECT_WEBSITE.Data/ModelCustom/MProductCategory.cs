using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJECT_WEBSITE.Data.ModelCustom
{
    public class MProductCategory
    {

        public int STT { get; set; }

        public int ProductCategoryID { get; set; }

        public string Name { get; set; }

        public string MetaTitle { get; set; }

        public int? Display { get; set; }

        public int? ParentID { get; set; }

        public DateTime? CreateDate { get; set; }

        public string Image { get; set; }

        public bool? ShowOnHome { get; set; }

        public int? SLProduct { get; set; }

        public bool? ProductCategoryStatus { get; set; }
    }
}
