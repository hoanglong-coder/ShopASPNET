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
    public class NewsController : RoleQuanLyTinTucController
    {
        // GET: Admin/News
        public ActionResult Index()
        {

            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            ViewBag.UserName = session.FullName;

            ViewBag.Role = session.RoleString;

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult CreateNew(MNews news)
        {
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];
            news.UserID = session.UserID;
            var dao = new NewsDAO();
            var check = dao.CreateNews(news);

            return Json(new { code = 200, data = check }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAllCategoryNew()
        {
            var dao = new NewsCateogryDAO();
            var lst = dao.GetAllAdmin();

            return Json(new { code = 200, data = lst }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAllCategoryNewPaging(SearchNews search, int page = 1, int pageSize = 3)
        {
            var dao = new NewsCateogryDAO();

            IPagedList model = (IPagedList)dao.GetAllAdminPaging(search, page, pageSize);

            int c = model.TotalItemCount;

            return Json(new { code = 200, data = model, total = c }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAllAdmin(SearchNews searchnews, int page = 1, int pageSize = 1)
        {
            var dao = new NewsDAO();

            IPagedList model = (IPagedList)dao.GetAllAdmin(searchnews, page, pageSize);

            int c = model.TotalItemCount;

            return Json(new { code = 200, data = model, total = c}, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDetail(int id)
        {
            var dao = new NewsDAO();
            var model = dao.GetDetail(id);

            return Json(new { code = 200, data = model }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult UpdateNews(MNews news)
        {
            var dao = new NewsDAO();

            var check = dao.UpdateNews(news);

            return Json(new { code = 200, data = check }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteNew(int id)
        {
            var dao = new NewsDAO();
            var model = dao.DeleteNew(id);

            return Json(new { code = 200, data = model }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult CreateCategorynew(MCategoryNews mCategoryNews)
        {
            var dao = new NewsCateogryDAO();
            var check = dao.CreateNewCateogry(mCategoryNews);

            return Json(new { code = 200, data = check }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DetailCategory(int id)
        {
            var dao = new NewsCateogryDAO();
            var modal = dao.GetDetail(id);

            return Json(new { code = 200, data = modal }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EditCategorynew(MCategoryNews mCategoryNews)
        {
            var dao = new NewsCateogryDAO();
            var check = dao.EditCategory(mCategoryNews);

            return Json(new { code = 200, data = check }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteCategory(int id)
        {
            var dao = new NewsCateogryDAO();
            var model = dao.DeleteCategory(id);

            return Json(new { code = 200, data = model }, JsonRequestBehavior.AllowGet);
        }
    }
}