using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PROJECT_WEBSITE.WebAPP.Common
{
    [Serializable]
    public class CustomerLogin
    {
        public string Phone { get; set; }
        public long CustomerID { get; set; }
    }
}