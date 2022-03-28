using PagedList;
using PROJECT_WEBSITE.Data.DAO;
using PROJECT_WEBSITE.WebAPP.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PROJECT_WEBSITE.WebAPP.Areas.Admin.Controllers
{
    public class ShipController : BaseController
    {
        // GET: Admin/Ship
        public ActionResult Index()
        {

            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            ViewBag.UserName = session.FullName;

            ViewBag.Role = session.RoleString;

            return View();
        }
        [HttpGet]
        public JsonResult ListShip(int page = 1, int pageSize = 3)
        {
            var dao = new OrderDAO();
            IPagedList model = (IPagedList)dao.ListShip(page, pageSize);
            int c = model.TotalItemCount;
            List<decimal> totalprice = new List<decimal>();

            foreach (var item in dao.ListShip(page, pageSize))
            {
                decimal sum = new OrderDetailDAO().Sum(item.OrderID);
                totalprice.Add(sum);
            }

            return Json(new { code = 200, data = model, total = c, totalprice = totalprice }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult OrderShip(int idorder)
        {
            var dao = new OrderDAO();
            var check = dao.OrderShip(idorder);
            if (check)
            {
                return Json(new { code = 200 }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { code = 500 }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult OrderShipSuccess(int idorder)
        {
            var dao = new OrderDAO();
            var check = dao.OrderShipSuccsess(idorder);
            if (check)
            {
                return Json(new { code = 200 }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { code = 500 }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}