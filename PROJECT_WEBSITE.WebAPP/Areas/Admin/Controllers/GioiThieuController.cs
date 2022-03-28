using PROJECT_WEBSITE.WebAPP.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PROJECT_WEBSITE.WebAPP.Areas.Admin.Controllers
{
    public class GioiThieuController : RoleQuanLyGioiThieuController
    {
        // GET: Admin/GioiThieu
        public ActionResult Index()
        {

            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            ViewBag.UserName = session.FullName;

            ViewBag.Role = session.RoleString;

            return View();
        }


        [HttpPost]
        [ValidateInput(false)]
        public JsonResult EditGioiThieu(string noidung)
        {

            string fileLPath = Server.MapPath(@"~/Data/GioiThieu.txt");

            System.IO.File.WriteAllText(fileLPath, noidung);

            return Json(new { code = 200, data = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult LoadGioiThieu(string noidung)
        {
            string fileLPath = Server.MapPath(@"~/Data/GioiThieu.txt");

            var rs = System.IO.File.ReadAllText(fileLPath);

            return Json(new { code = 200, data = rs }, JsonRequestBehavior.AllowGet);
        }
    }
}