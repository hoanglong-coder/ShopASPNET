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
    public class ProductCategoryController : Controller
    {
        // GET: Admin/ProductCategory
        public ActionResult Index()
        {

            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            ViewBag.UserName = session.FullName;

            ViewBag.Role = session.RoleString;

            return View();
        }

        [HttpGet]
        public JsonResult ListProductCategory()
        {
            var dao = new ProductCategoryDAO();

            var lstCategory = dao.ListCategoryAdmin();

            return Json(new { code = 200, data = lstCategory }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ListProductCategoryBase(SearchNews search, int page = 1, int pageSize = 3)
        {
            var dao = new ProductCategoryDAO();

            IPagedList model = (IPagedList)dao.ListCategoryBase(search, page,pageSize);

            var total = dao.ListCategoryBase(search, 1, 1000000).ToList();

            int c = model.TotalItemCount;

            return Json(new { code = 200, data = model, total = c,countproduct = total.Sum(t=>t.SLProduct) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ListProductCategoryCap2(SearchNews search,int id,int page = 1, int pageSize = 3)
        {
            var dao = new ProductCategoryDAO();

            IPagedList model = (IPagedList)dao.ListCategoryCap2(search,page, pageSize, id);

            var total = dao.ListCategoryCap2(search, 1, 1000000, id).ToList();

            int c = model.TotalItemCount;

            return Json(new { code = 200, data = model, total = c , countproduct = total.Sum(t => t.SLProduct) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CreateCategoryBase(MProductCategory mProductCategory)
        {
            var dao = new ProductCategoryDAO();

            var check = dao.CreateCategoryBase(mProductCategory);

            return Json(new { code = 200, data = check }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCategoryByID(int id)
        {
            var dao = new ProductCategoryDAO();

            var check = dao.GetCategoryByID(id);

            return Json(new { code = 200, data = check }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteCategoryBase(int id)
        {
            var dao = new ProductCategoryDAO();

            var check = dao.DeleteCategoryBase(id);

            return Json(new { code = 200, data = check }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteCategoryCap2(int id)
        {
            var dao = new ProductCategoryDAO();

            var check = dao.DeleteCategoryCap2(id);

            return Json(new { code = 200, data = check }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateCategoryBase(MProductCategory mProductCategory)
        {
            var dao = new ProductCategoryDAO();

            var check = dao.UpdateCateogryBase(mProductCategory);

            return Json(new { code = 200, data = check }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateCategory(MProductCategory mProductCategory)
        {
            var dao = new ProductCategoryDAO();

            var check = dao.UpdateCateogry(mProductCategory);

            return Json(new { code = 200, data = check }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult CreateCategory(MProductCategory mProductCategory)
        {
            var dao = new ProductCategoryDAO();

            var check = dao.CreateCategory(mProductCategory);

            return Json(new { code = 200, data = check }, JsonRequestBehavior.AllowGet);
        }
    }
}