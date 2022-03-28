using PROJECT_WEBSITE.Data.DAO;
using PROJECT_WEBSITE.Data.EF;
using PROJECT_WEBSITE.WebAPP.Areas.Admin.Models;
using PROJECT_WEBSITE.WebAPP.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PROJECT_WEBSITE.WebAPP.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index(int id = 0)
        {
            DbWebsite db = new DbWebsite();

            var lst = db.FooterCategories.ToList();

            ViewBag.ListCategoryFooter = lst;

            var session = (CustomerLogin)Session[Common.CommonConstants.CUSTOMER_SESSION];
            if (session != null)
            {
                var model1 = new CustomerDAO().GetById(session.Phone);
                ViewBag.User = model1.Name;
                ViewBag.Id = model1.CustomerID;
            }else
            {
                 return RedirectToAction("Index", "Login");
            }
            var model = new CustomerDAO().GetCustomer(id);
            return View(model);
        }
        public ActionResult EditAccount(int id)
        {
            var session = (CustomerLogin)Session[Common.CommonConstants.CUSTOMER_SESSION];
            if (session != null)
            {
                var model1 = new CustomerDAO().GetById(session.Phone);
                ViewBag.User = model1.Name;
                ViewBag.Id = model1.CustomerID;
            }
            var model = new CustomerDAO().GetCustomer(id);
            return View(model);
        }
        [HttpPost]
        public JsonResult EditAccount(Customer customer)
        {
            try
            {
                var dao = new CustomerDAO();
                dao.UpdateCustomer(customer);
                return Json(new { code = 200, msg = "Thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new { code = 500, msg = "Thành công" }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult ChangePasswork(string passold, string passnew, int ID)
        {
            var dao = new CustomerDAO();
            if (dao.ChangePass(passold, passnew, ID))
            {
                return Json(new { code = 200, msg = "Thành công" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { code = 500, msg = "Thành công" }, JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult GetCartById(long id, int page = 1, int pageSize = 3)
        {
            var session = (CustomerLogin)Session[Common.CommonConstants.CUSTOMER_SESSION];
            if (session != null)
            {
                var model1 = new CustomerDAO().GetById(session.Phone);

                int c = 0;

                var listcarrt = new OrderDAO().ListCartById(model1.CustomerID, page, pageSize, ref c);

                if (listcarrt != null)
                {
                    return Json(new { code = 200, data = listcarrt, total = c,}, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { code = 500 }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { code = 500 }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult ListOrderDetail(int id)
        {
            var db = new DbWebsite();

            var dao = new OrderDetailDAO();
            var model = dao.GetbyId(id);

            List<OrderDetailModel> listorderdetail = new List<OrderDetailModel>();

            foreach (var item in model)
            {
                OrderDetailModel t = new OrderDetailModel(new ProductDAO().DetailProduct(item.ProductID), item.OrderID, item.OrderDetailCount.Value, item.OrderPrice.Value, (item.OrderDetailCount.Value * item.OrderPrice.Value), db.ProductUnits.Find(item.Product.UnitID.Value).Name);
                listorderdetail.Add(t);
            }

            decimal khuyenmai = db.Orders.Find(id).PriceDiscount.Value;
            decimal ship = db.Orders.Find(id).PriceShip.Value;


            return Json(new { code = 200, data = listorderdetail,khuyenmai = khuyenmai,ship = ship }, JsonRequestBehavior.AllowGet);
        }
    }
}