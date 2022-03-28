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
    public class ProductComBoController : RoleComBoController
    {
        // GET: Admin/ProductComBo
        public ActionResult Index()
        {

            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            ViewBag.UserName = session.FullName;

            ViewBag.Role = session.RoleString;

            return View();
        }

        [HttpPost]
        public JsonResult ListProductComo(SearchProductCombo search, int page = 1, int pageSize = 10)
        {
            var dao = new ProductComBoDAO();

            IPagedList model = (IPagedList)dao.GetAll(search, page, pageSize);

            int c = model.TotalItemCount;

            return Json(new { code = 200, data = model, total = c}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ListProductComoDetail(int id,int page = 1, int pageSize = 10)
        {
            var dao = new ProductComBoDAO();
            SearchProduct search = new SearchProduct();
            IPagedList model = (IPagedList)dao.GetDetailComBo(id,page, pageSize, search);

            int c = model.TotalItemCount;

            return Json(new { code = 200, data = model, total = c }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult AddProduct(MProductCombo mproduct)
        {
            var dao = new ProductComBoDAO();
            var check = dao.CreateProductCombo(mproduct);

            return Json(new { code = 200, data = check }, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public JsonResult AddProductCombo(int idcombo,int idproduct)
        {
            var dao = new ProductComBoDAO();

            var check = dao.AddProductCombo(idcombo,idproduct);

            return Json(new { code = 200, data = check }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddProductComboList(int idcombo, List<int> idproduct)
        {
            var dao = new ProductComBoDAO();

            var check = dao.AddProductComboList(idcombo,idproduct);

            return Json(new { code = 200, data = check }, JsonRequestBehavior.AllowGet);
        }
    }
}