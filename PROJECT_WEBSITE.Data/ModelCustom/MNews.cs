using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJECT_WEBSITE.Data.ModelCustom
{
    public class MNews
    {
        public int STT { get; set; }

        public int NewsID { get; set; }

        public string Name { get; set; }

        public string Detail { get; set; }

        public string Image { get; set; }

        public DateTime? CreateDate { get; set; }

        public int? UserID { get; set; }

        public int? CategoryNewID { get; set; }

        public bool? NewsStatus { get; set; }

        public string NameUser { get; set; }

        public string CategoryName { get; set; }

    }
}
