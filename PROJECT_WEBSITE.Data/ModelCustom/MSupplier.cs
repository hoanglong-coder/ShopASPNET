using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJECT_WEBSITE.Data.ModelCustom
{
    public class MSupplier
    {

        public int SupplierID { get; set; }


        public string Name { get; set; }


        public string Phone { get; set; }


        public string Address { get; set; }

        public string Email { get; set; }

        public DateTime? CreateDate { get; set; }

        public bool? SupplierStatus { get; set; }
    }
}
