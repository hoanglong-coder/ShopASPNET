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
    public class CustomerController : RoleQuanLyKhachHangKhachHangController
    {
        // GET: Admin/Customer
        public ActionResult Index()
        {

            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            ViewBag.UserName = session.FullName;

            ViewBag.Role = session.RoleString;

            return View();
        }

        [HttpPost]
        public JsonResult GetAllPaging(SearchNews search, int page = 1, int pageSize = 10)
        {

            var dao = new CustomerDAO();

            IPagedList model = (IPagedList)dao.GetCustomerPaging(search, page, pageSize);

            int c = model.TotalItemCount;

            return Json(new { code = 200, data = model, total = c }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ChangeStatusCustomer(int id)
        {
            var dao = new CustomerDAO();
            var model = dao.ChangCustomer(id);

            return Json(new { code = 200, data = model }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteCustomer(int id)
        {
            var dao = new CustomerDAO();

            var model = dao.DeleteCustomer(id);

            return Json(new { code = 200, data = model }, JsonRequestBehavior.AllowGet);
        }
    }
}