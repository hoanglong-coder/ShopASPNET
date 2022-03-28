using PROJECT_WEBSITE.Data.DAO;
using PROJECT_WEBSITE.Data.EF;
using PROJECT_WEBSITE.WebAPP.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace PROJECT_WEBSITE.WebAPP.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            DbWebsite db = new DbWebsite();

            var lst = db.FooterCategories.ToList();

            var session = (CustomerLogin)Session[Common.CommonConstants.CUSTOMER_SESSION];
            if (session != null)
            {
                var model = new CustomerDAO().GetById(session.Phone);
                ViewBag.User = model.Name;
                ViewBag.Id = model.CustomerID;

                var lst2 = db.FooterCategories.ToList();

                ViewBag.ListCategoryFooter = lst2;
            }

          
            ViewBag.ListCategoryFooter = lst;

            return View();
        }

        //Component Menu chính
        [ChildActionOnly]
        public ActionResult MainMenu()
        {
            var model = new ProductCategoryDAO().ListCate();

            ViewBag.lstnew = new NewsCateogryDAO().GetAll();
            return PartialView(model);
        }

        //Component Thanh tìm kiếm

        [ChildActionOnly]
        public ActionResult Search()
        {
            var model = new ProductCategoryDAO().ListCate();
            return PartialView(model);
        }

        //Silder header
        [ChildActionOnly]
        public ActionResult SliderHeader()
        {

            var dao = new SildeDAO();

            var rs = dao.GetID(1);

            List<string> lst = new List<string>();
            if (rs.ImageMore != null && rs.ImageMore != "[]")
            {
                XElement ImageMore = XElement.Parse(rs.ImageMore);

                foreach (XElement item in ImageMore.Elements())
                {
                    lst.Add(item.Value);
                }
            }

            ViewData["Slide"] = lst;
            return PartialView();
        }

        //Component Sản phẩm khuyến mãi
        [ChildActionOnly]
        public ActionResult BestSell()
        {
            var dao = new ProductDAO();
            var model = dao.ListProduct().Where(T => T.ProductCategoryID != null).Take(10).ToList();
            return PartialView(model);
        }

        //Component Danh mục sản phẩm
        [ChildActionOnly]
        public ActionResult CategoryProducts()
        {
            var listDMSP = new ProductCategoryDAO().ListCate();
            var listProduct = new ProductDAO().ListProduct();
            ViewBag.ListProduct = listProduct;
            return PartialView(listDMSP);
        }

        //Component Hot Deal Combo ưu đãi
        [ChildActionOnly]
        public ActionResult HotDeal()
        {
            var model = new ProductDAO().listdeals();
            return PartialView(model);
        }

        //Component Sản phẩm mới
        [ChildActionOnly]
        public ActionResult NewProducts()
        {
            var dao = new ProductDAO();
            var model = dao.ListProduct();
            return PartialView(model);
        }

        //Component Sản phẩm nổi bật
        [ChildActionOnly]
        public ActionResult FeatureProduct()
        {
            var dao = new ProductDAO();
            var model = dao.ListProduct();
            return PartialView(model);
        }

        //Component Slider footer thương hiệu
        [ChildActionOnly]
        public ActionResult BrandSlider()
        {
            var dao = new SildeDAO();

            var rs = dao.GetID(2);

            List<string> lst = new List<string>();
            if (rs.ImageMore != null && rs.ImageMore != "[]")
            {
                XElement ImageMore = XElement.Parse(rs.ImageMore);

                foreach (XElement item in ImageMore.Elements())
                {
                    lst.Add(item.Value);
                }
            }

            ViewData["Slide"] = lst;
            return PartialView();
        }


        public ActionResult DetailFooter(int id)
        {
            var session = (CustomerLogin)Session[Common.CommonConstants.CUSTOMER_SESSION];
            if (session != null)
            {
                var Name = new CustomerDAO().GetById(session.Phone);
                ViewBag.User = Name.Name;
                ViewBag.Id = Name.CustomerID;
            }
            DbWebsite db = new DbWebsite();

            var model = db.Footers.Find(id);

            var dao = new SildeDAO();

            var lst = db.FooterCategories.ToList();

            ViewBag.ListCategoryFooter = lst;

            return View(model);
        }
    }
}