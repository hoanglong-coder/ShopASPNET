using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJECT_WEBSITE.Data.ModelCustom
{
    public class MUserRoleGroup
    {
        public int STT { get; set; }

        public int UserRoleGroupID { get; set; }

        public string Name { get; set; }

        public DateTime? CreateDate { get; set; }

        public bool? GroupStatus { get; set; }

    }
}
