using PagedList;
using PROJECT_WEBSITE.Data.DAO;
using PROJECT_WEBSITE.Data.ModelCustom;
using PROJECT_WEBSITE.WebAPP.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PROJECT_WEBSITE.WebAPP.Areas.Admin.Controllers
{
    public class DiscountCodeController : RoleMaKhuyenMaiController
    {
        // GET: Admin/DiscountCode
        public ActionResult Index()
        {

            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            ViewBag.UserName = session.FullName;

            ViewBag.Role = session.RoleString;

            return View();
        }

        [HttpPost]
        public JsonResult ListDiscountCode(SearchNews search, int page = 1, int pageSize = 10)
        {
            var dao = new DiscountCodeDAO();

            IPagedList model = (IPagedList)dao.GetAll(search, page, pageSize);

            int c = model.TotalItemCount;


            return Json(new { code = 200, data = model,total = c }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CreateDiscountCode(MDiscountCode mDiscountCode)
        {
            var dao = new DiscountCodeDAO();

            var check = dao.CreateDiscountCode(mDiscountCode);

            return Json(new { code = 200, data = check }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetByID(int id)
        {
            var dao = new DiscountCodeDAO();

            var check = dao.GetByID(id);

            return Json(new { code = 200, data = check }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateDiscountCode(MDiscountCode mDiscountCode)
        {
            var dao = new DiscountCodeDAO();

            var check = dao.Update(mDiscountCode);

            return Json(new { code = 200, data = check }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteDiscountCode(int id)
        {
            var dao = new DiscountCodeDAO();

            var check = dao.Delete(id);

            return Json(new { code = 200, data = check }, JsonRequestBehavior.AllowGet);
        }
    }


}