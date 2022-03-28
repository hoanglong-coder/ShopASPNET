using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJECT_WEBSITE.Data.ModelCustom
{
    public class MRole
    {
        public int STT { get; set; }

        public int UserRoleID { get; set; }

        public string Name { get; set; }

        public DateTime? CreateDate { get; set; }

        public bool? UserRoleStatus { get; set; }
    }
}
