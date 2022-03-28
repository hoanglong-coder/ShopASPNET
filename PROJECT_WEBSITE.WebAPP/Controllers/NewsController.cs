using PROJECT_WEBSITE.Data.DAO;
using PROJECT_WEBSITE.Data.EF;
using PROJECT_WEBSITE.WebAPP.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PROJECT_WEBSITE.WebAPP.Controllers
{
    public class NewsController : Controller
    {
        // GET: News
        public ActionResult Index(int id, int page = 1, int pageSize = 1,string search="")
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
            var dao = new NewsDAO();
            int totalRecord = 0;
            var model = dao.GetAll(id, search,ref totalRecord,page,pageSize);
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

            //Loại Tin tức
            ViewBag.lstnew = new NewsCateogryDAO().GetAll();

            return View(model);
        }

        public ActionResult Detail(int id)
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
            var dao = new NewsDAO();
            var model = dao.Detail(id);
            ViewBag.lstnew = new NewsCateogryDAO().GetAll();
            return View(model);
        }

        [HttpPost]
        public ActionResult SearchNews(int id,string query)
        {
            return Redirect($"/tin-tuc/{id}?search={query}");
        }

    }
}