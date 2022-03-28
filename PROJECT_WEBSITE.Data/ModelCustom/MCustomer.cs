using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJECT_WEBSITE.Data.ModelCustom
{
    public class MCustomer
    {
        public int STT { get; set; }

        public int CustomerID { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public DateTime? CreateDate { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        public string Gender { get; set; }

        public DateTime? Birth { get; set; }

        public int? CustomerStatus { get; set; }
    }
}
