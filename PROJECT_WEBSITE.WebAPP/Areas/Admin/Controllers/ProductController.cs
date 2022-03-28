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
using System.Xml.Linq;

namespace PROJECT_WEBSITE.WebAPP.Areas.Admin.Controllers
{
    public class ProductController : RoleKhoHangController
    {
        DbWebsite db = new DbWebsite();
        // GET: Admin/Product
        public ActionResult Index()
        {

            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            ViewBag.UserName = session.FullName;

            ViewBag.Role = session.RoleString;

            return View();
        }

        [HttpPost]
        public JsonResult ListProduct(SearchProduct search,int page = 1, int pageSize = 10)
        {
            var dao = new ProductDAO();

            IPagedList model = (IPagedList)dao.ListProductAdmin(page, pageSize, search);

            int c = model.TotalItemCount;

            int tongkho = db.Products.Where(t => t.ProductComboID.HasValue == false&&t.ProductStatus==true).Sum(t => t.CountProduct.Value);

            return Json(new { code = 200, data = model, total = c, TongKho = tongkho }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ListProductDVTCoBan()
        {
            var dao = new ProductDAO();

            var lstproduct = dao.ListProductDVTCoBan();

            return Json(new { code = 200, data = lstproduct }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult ListProductThemPhieuNhap()
        {
            var dao = new ProductDAO();

            var lstproduct = dao.ListProductThemPhieuNhap();

            return Json(new { code = 200, data = lstproduct }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult AddProduct(MProduct mproduct)
        {
            var dao = new ProductDAO();
            var check = dao.CreateProduct(mproduct);

            return Json(new { code = 200, data = check }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DetailProduct(int id)
        {
            var dao = new ProductDAO();

            var mproduct = dao.DetailProduct(id);

            List<string> lst = new List<string>();
            if (mproduct.MoreImage != null&& mproduct.MoreImage!= "[]")
            {
                XElement ImageMore = XElement.Parse(mproduct.MoreImage);
                
                foreach (XElement item in ImageMore.Elements())
                {
                    lst.Add(item.Value);
                }
            }

            return Json(new { code = 200, data = mproduct,images = lst }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult UpdateProduct(MProduct mproduct)
        {
            var dao = new ProductDAO();

            var check = dao.UpdateProduct(mproduct);

            return Json(new { code = 200, data = check }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddProductDVT(MProduct mproduct)
        {
            var dao = new ProductDAO();
            var check = dao.CreateProductDVT(mproduct);

            return Json(new { code = 200, data = check }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult UpdateProductDVT(MProduct mproduct)
        {
            var dao = new ProductDAO();
            var check = dao.UpdateProductDVT(mproduct);

            return Json(new { code = 200, data = check }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ActionProduct(int id)
        {
            var dao = new ProductDAO();

            var check = dao.ActionProduct(id);

            return Json(new { code = 200, data = check }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteProduct(int id)
        {
            var dao = new ProductDAO();
            var model = dao.DeleteProduct(id);

            return Json(new { code = 200, data = model }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ListProductAddCombo()
        {
            var dao = new ProductDAO();

            var lstproduct = dao.ListProductComboAdd();

            return Json(new { code = 200, data = lstproduct }, JsonRequestBehavior.AllowGet);
        }
    }
}