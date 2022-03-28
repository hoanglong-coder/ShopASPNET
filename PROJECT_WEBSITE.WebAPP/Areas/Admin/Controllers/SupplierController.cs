using PROJECT_WEBSITE.Data.DAO;
using PROJECT_WEBSITE.WebAPP.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PROJECT_WEBSITE.WebAPP.Areas.Admin.Controllers
{
    public class SupplierController : Controller
    {
        // GET: Admin/Supplier
        public ActionResult Index()
        {

            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            ViewBag.UserName = session.FullName;

            ViewBag.Role = session.RoleString;

            return View();
        }


        [HttpGet]
        public JsonResult ListSupplier()
        {
            var dao = new SupplierDAO();

            var model = dao.GetAll();

            return Json(new { code = 200, data = model}, JsonRequestBehavior.AllowGet);
        }
    }
}