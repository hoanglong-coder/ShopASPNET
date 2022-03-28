using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PROJECT_WEBSITE.WebAPP.Areas.Admin.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Chưa nhập UserName")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Chưa nhập Password")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}