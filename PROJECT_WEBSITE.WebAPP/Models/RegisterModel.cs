using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PROJECT_WEBSITE.WebAPP.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Chưa nhập tên")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Chưa nhập số điện thoại")]
        [RegularExpression(@"^(0?)(3[2-9]|5[6|8|9]|7[0|6-9]|8[0-6|8|9]|9[0-4|6-9])[0-9]{7}$",
         ErrorMessage = "Sai định dạng số điện thoại.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Chưa nhập Password")]
        public string Password { get; set; }
    }
}