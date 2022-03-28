using PagedList;
using PROJECT_WEBSITE.Data.DAO;
using PROJECT_WEBSITE.Data.EF;
using PROJECT_WEBSITE.Data.ModelCustom;
using PROJECT_WEBSITE.WebAPP.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PROJECT_WEBSITE.WebAPP.Areas.Admin.Controllers
{
    public class FooterController : RoleQuanLyChanTrangController
    {
        // GET: Admin/Footer
        public ActionResult Index()
        {

            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            ViewBag.UserName = session.FullName;

            ViewBag.Role = session.RoleString;

            return View();
        }

        [HttpGet]
        public JsonResult GetAllCategoryFooter()
        {
            SearchNews search = new SearchNews();

            var dao = new FooterDAO();

            var lst = dao.GetCategogryFooter(search,1,1000000);

            return Json(new { code = 200, data = lst }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAllPaging(SearchNews search, int page = 1, int pageSize = 10)
        {

            var dao = new FooterDAO();

            IPagedList model = (IPagedList)dao.GetCategogryFooter(search, page, pageSize);

            int c = model.TotalItemCount;

            return Json(new { code = 200, data = model, total = c }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GetFooter(SearchNews search, int page = 1, int pageSize = 10)
        {
            var dao = new FooterDAO();

            IPagedList model = (IPagedList)dao.GetFooter(search, page, pageSize);

            int c = model.TotalItemCount;

            return Json(new { code = 200, data = model, total = c }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult CreateFooter(MFooter footer)
        {
            var dao = new FooterDAO();

            var check = dao.CreateFooter(footer);

            return Json(new { code = 200, data = check }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDetail(int id)
        {
            var dao = new FooterDAO();

            var model = dao.GetByID(id);

            return Json(new { code = 200, data = model }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult UpdateFooter(MFooter footer)
        {
            var dao = new FooterDAO();

            var check = dao.UpdateFooter(footer);

            return Json(new { code = 200, data = check }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Delete(int id)
        {
            var dao = new FooterDAO();

            var model = dao.DeleteFooter(id);

            return Json(new { code = 200, data = model }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult CreateFooterCate(MFooterCategory footer)
        {
            var dao = new FooterDAO();

            var check = dao.CreateFooterCate(footer);

            return Json(new { code = 200, data = check }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDetailCate(int id)
        {
            var dao = new FooterDAO();

            var model = dao.GetByIDCate(id);

            return Json(new { code = 200, data = model }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateFooterCate(MFooterCategory footer)
        {
            var dao = new FooterDAO();

            var check = dao.UpdateFooterCate(footer);

            return Json(new { code = 200, data = check }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult DeleteCate(int id)
        {
            var dao = new FooterDAO();

            var model = dao.DeleteFooterCate(id);

            return Json(new { code = 200, data = model }, JsonRequestBehavior.AllowGet);
        }
    }
}