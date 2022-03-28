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
    public class ExChangeUnitController : Controller
    {
        // GET: Admin/ExChangeUnit
        public ActionResult Index()
        {

            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            ViewBag.UserName = session.FullName;

            ViewBag.Role = session.RoleString;

            return View();
        }


        public JsonResult GetExchangeByProductID(int id)
        {
            var dao = new ExchangeUnitDAO();

            var rs = dao.GetExchangeByProductID(id);


            return Json(new { code = 200, data = rs}, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult AddExchangeUnit(MExchangeUnit exchangeUnit)
        {
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            exchangeUnit.UserID = session.UserID;

            var dao = new ExchangeUnitDAO();

            var rs = dao.AddExchangeUnit(exchangeUnit);

            return Json(new { code = 200, data = rs }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAll(SearchNews search, int page = 1, int pageSize = 3)
        {
            var dao = new ExchangeUnitDAO();

            IPagedList model = (IPagedList)dao.GetAll(search, page, pageSize);

            int c = model.TotalItemCount;

            return Json(new { code = 200, data = model, total = c }, JsonRequestBehavior.AllowGet);
        }

    }
}