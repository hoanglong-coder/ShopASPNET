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
    public class ProductUnitController : Controller
    {
        // GET: Admin/ProductUnit
        public ActionResult Index()
        {

            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            ViewBag.UserName = session.FullName;

            ViewBag.Role = session.RoleString;

            return View();
        }
        [HttpGet]
        public JsonResult ListProductUnitBase()
        {
            var dao = new ProductUnitDAO();

            var lstproductUnit = dao.ListProductUnitBase();

            return Json(new { code = 200, data = lstproductUnit }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ListProductUnit()
        {
            var dao = new ProductUnitDAO();

            var lstproductUnit = dao.ListProductUnit();

            return Json(new { code = 200, data = lstproductUnit }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteUnit(int id)
        {
            var dao = new ProductUnitDAO();

            var model = dao.DeleteUnitByID(id);

            return Json(new { code = 200, data = model }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ListProductUnitAll(SearchUnit search,int page = 1, int pageSize = 10)
        {
            var dao = new ProductUnitDAO();

            IPagedList model = (IPagedList)dao.ListProductUnitAll(search,page,pageSize);

            int c = model.TotalItemCount;

            return Json(new { code = 200, data = model, total = c }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ListProductUnitselectSearch()
        {
            var dao = new ProductUnitDAO();

            SearchUnit search = new SearchUnit();

            var model = dao.ListProductUnitAll(search, 1, 1000000);

            return Json(new { code = 200, data = model }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetUnitByID(int id)
        {
            var dao = new ProductUnitDAO();

            var model = dao.GetUnitByID(id);

            return Json(new { code = 200, data = model}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CreateUnit(MProductUnit mProductUnit)
        {
            var dao = new ProductUnitDAO();

            var lstproductUnit = dao.CreateUnit(mProductUnit);

            return Json(new { code = 200, data = lstproductUnit }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateUnit(MProductUnit mProductUnit)
        {
            var dao = new ProductUnitDAO();

            var lstproductUnit = dao.UpdateUnit(mProductUnit);

            return Json(new { code = 200, data = lstproductUnit }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetExchangeByID(int id)
        {
            var dao = new ProductUnitDAO();

            var model = dao.GetExchangeByID(id);

            return Json(new { code = 200, data = model }, JsonRequestBehavior.AllowGet);
        }
    }
}