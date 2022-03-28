using PROJECT_WEBSITE.Data.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PROJECT_WEBSITE.WebAPP.Controllers
{
    public class GioiThieuController : Controller
    {
        // GET: GioiThieu
        public ActionResult Index()
        {
            DbWebsite db = new DbWebsite();

            var lstfooter = db.FooterCategories.ToList();

            ViewBag.ListCategoryFooter = lstfooter;

            string fileLPath = Server.MapPath(@"~/Data/GioiThieu.txt");

            var rs = System.IO.File.ReadAllText(fileLPath);

            ViewData["gioithieu"] = rs.ToString();

            return View();
        }
    }
}