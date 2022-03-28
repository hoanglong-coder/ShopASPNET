using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJECT_WEBSITE.Data.ModelCustom
{
    public class MUser
    {
        public int STT { get; set; }

        public int UserID { get; set; }

        public string UserName { get; set; }

        public string Passsword { get; set; }

        public string FullName { get; set; }

        public DateTime? Birth { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public DateTime? CreateDate { get; set; }

        public int? UserRoleGroupID { get; set; }

        public string UserRoleGroup { get; set; }

        public int? UserStatus { get; set; }
    }
}
