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
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult Index()
        {
            ViewBag.Sort = "New";
            ViewBag.Category = "null";

            DbWebsite db = new DbWebsite();

            var lst = db.FooterCategories.ToList();

            ViewBag.ListCategoryFooter = lst;
            return View();
        }

        /// <summary>
        /// Hàm Tìm kiếm sản phẩm
        /// </summary>
        /// <param name="query">Câu truy vấn</param>
        /// <param name="category">Loại sản phẩm</param>
        /// <returns>Danh sách sản phẩm cùng loại tìm kiếm</returns>
        [HttpPost]
        public ActionResult SearchPost(string query, int category)
        {
            var meta = new ProductCategoryDAO().ListCategory().Where(t => t.ProductCategoryID == category).FirstOrDefault().MetaTitle;
            return Redirect($"/san-pham/{meta}-{category}?query={query}&id={category}");
        }

        //Component Danh sách sản phẩm
        public ActionResult Products(long id, string query, string sort, string category, string price, int page = 1, int pageSize = 16)
        {
            DbWebsite db = new DbWebsite();

            var lstfooter = db.FooterCategories.ToList();

            ViewBag.ListCategoryFooter = lstfooter;
            var session = (CustomerLogin)Session[Common.CommonConstants.CUSTOMER_SESSION];
            if (session != null)
            {
                var Name = new CustomerDAO().GetById(session.Phone);
                ViewBag.User = Name.Name;
                ViewBag.Id = Name.CustomerID;
            }
            //Sản phẩm
            int totalRecord = 0;
            var model = new ProductDAO().PaginationProduct(id, query, sort, category, price, ref totalRecord, page, pageSize);
            ViewBag.Name = new ProductCategoryDAO().ListCate().Where(t => t.ProductCategoryID == id).SingleOrDefault().Name.ToString();
            ViewBag.Total = totalRecord;
            ViewBag.Page = page;
            int maxPage = 5;
            int totalPage = 0;
            totalPage = (int)Math.Ceiling(((double)totalRecord / pageSize));
            ViewBag.TotalPage = totalPage;
            ViewBag.MaxPage = maxPage;
            ViewBag.Firts = 1;
            ViewBag.Last = totalPage;
            ViewBag.Next = page + 1;
            ViewBag.Prev = page - 1;
            //Bộ lọc
            ViewBag.CountCategroy = new ProductDAO().ListProduct();
            var parentId = new ProductCategoryDAO().ListCate().Where(t => t.ProductCategoryID == id).SingleOrDefault().ParentID;
            ViewBag.CategorySearch = new ProductCategoryDAO().ListCate().Where(t => t.ParentID == parentId & t.ProductCategoryID != id).ToList();
            ViewBag.ListCategory = new ProductCategoryDAO().ListCate();
            ViewBag.Sort = sort;
            ViewBag.Sorts = "";
            ViewBag.Category = category;
            ViewBag.price = price;

            return View(model);
        }

        //Component Chi tiết sản phẩm
        public ActionResult DetailProduct(int id)
        {
            DbWebsite db = new DbWebsite();

            var lstfooter = db.FooterCategories.ToList();

            ViewBag.ListCategoryFooter = lstfooter;
            var session = (CustomerLogin)Session[Common.CommonConstants.CUSTOMER_SESSION];
            if (session != null)
            {
                var Name = new CustomerDAO().GetById(session.Phone);
                ViewBag.User = Name.Name;
                ViewBag.Id = Name.CustomerID;
            }
            var model = new ProductDAO().DetailProduct(id);
            if (model.ProductCategoryID.HasValue)
            {
                ViewBag.Category = new ProductCategoryDAO().ListCate().Where(t => t.ProductCategoryID == model.ProductCategoryID).SingleOrDefault().Name;
                ViewBag.ListProductSame = new ProductDAO().ListProduct().Where(t => t.ProductCategoryID == model.ProductCategoryID && t.ProductID != id).ToList();
            }
            ViewBag.Category = null;
            ViewBag.ListProductSame = null;
            if (model.MoreImage != null)
            {
                XElement ImageMore = XElement.Parse(model.MoreImage);
                List<string> lst = new List<string>();
                foreach (XElement item in ImageMore.Elements())
                {
                    lst.Add(item.Value);
                }
                ViewBag.MoreImage = lst;
            }

            
            return View(model);
        }

    }
}