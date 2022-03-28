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
    public class ProductPromotionController : Controller
    {
        // GET: Admin/ProductPromotion
        public ActionResult Index()
        {

            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            ViewBag.UserName = session.FullName;

            ViewBag.Role = session.RoleString;

            return View();
        }

        [HttpGet]
        public JsonResult ListPricePromotion(int id, int page = 1, int pageSize = 6)
        {
            var dao = new ProductPromotionDAO();

            IPagedList model = (IPagedList)dao.ListPromotionByProduct(id,page,pageSize);

            int c = model.TotalItemCount;

            return Json(new { code = 200, data = model, total = c }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CreatePricePromotion(MProductPricePromotion productPricePromotion)
        {
            var dao = new ProductPromotionDAO();

            var lstproduct = dao.CreatePricePromotion(productPricePromotion);

            return Json(new { code = 200, data = lstproduct }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult Detail(string id)
        {
            var dao = new ProductPromotionDAO();

            var lstproduct = dao.DetailPromotion(int.Parse(id));

            return Json(new { code = 200, data = lstproduct }, JsonRequestBehavior.AllowGet);
        }
    }
}