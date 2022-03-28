using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PROJECT_WEBSITE.WebAPP.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Chưa nhập Số điện thoại")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Chưa nhập Password")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}