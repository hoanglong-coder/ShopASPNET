using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJECT_WEBSITE.Data.ModelCustom
{
    public class MCategoryNews
    {
        public int STT { get; set; }

        public int CategoryNewID { get; set; }

        public string Name { get; set; }

        public int? Display { get; set; }

        public DateTime? CreateDate { get; set; }

        public bool? CategoryStatus { get; set; }

        public int? CountNews { get; set; }

    }
}
