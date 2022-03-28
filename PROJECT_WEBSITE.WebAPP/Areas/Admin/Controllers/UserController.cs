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

namespace PROJECT_WEBSITE.WebAPP.Areas.Admin.Controllers
{
    public class UserController : RoleQuanLyNhanVienController
    {
        // GET: Admin/User
        public ActionResult Index()
        {

            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            ViewBag.UserName = session.FullName;

            ViewBag.Role = session.RoleString;

            return View();
        }

        [HttpGet]
        public JsonResult GetAllCategoryNewPaging(int page = 1, int pageSize = 10)
        {
            var dao = new UserDAO();

            IPagedList model = (IPagedList)dao.GetListUser(page, pageSize);

            int c = model.TotalItemCount;

            return Json(new { code = 200, data = model, total = c }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ChangeUser(int id)
        {
            var dao = new UserDAO();

            var check = dao.ChangeUser(id);

            return Json(new { code = 200, data = check }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CreatUser(User user)
        {
            var dao = new UserDAO();

            var check = dao.CreateUser(user);

            return Json(new { code = 200, data = check }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetChucVuAll()
        {
            var dao = new UserDAO();

            SearchNews searchNews = new SearchNews();

            var model = dao.ListChucVu(searchNews, 1, 1000000).ToList();

            return Json(new { code = 200, data = model}, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetUserByID(int id)
        {
            var dao = new UserDAO();

            var model = dao.GetUserByID(id);

            return Json(new { code = 200, data = model }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult UpdateUser(User mUser)
        {
            var dao = new UserDAO();

            var check = dao.UpdateUser(mUser);

            return Json(new { code = 200, data = check }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteUser(int id)
        {
            var dao = new UserDAO();

            var model = dao.DeleteUser(id);

            return Json(new { code = 200, data = model }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetChucVu(int page = 1,int pageSize = 10)
        {
            var dao = new UserDAO();

            SearchNews searchNews = new SearchNews();

            IPagedList model = (IPagedList)dao.ListChucVu(searchNews, page, pageSize);

            int c = model.TotalItemCount;


            return Json(new { code = 200, data = model, total = c }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetQuyen()
        {
            var dao = new UserDAO();

            var lst = dao.GetQuyen(1, 1000000);


            return Json(new { code = 200, data = lst}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CreateChucVu(string name ,List<int> role)
        {
            var dao = new UserDAO();

            var check = dao.CreateChucVuDetail(role, name);

            return Json(new { code = 200, data = check }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetRoleByID(int id)
        {
            var dao = new UserDAO();

            var lst = dao.GetRoleChucVu(id);


            return Json(new { code = 200, data = lst }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateChucVu(UserRoleGroup userRoleGroup, List<int> role)
        {
            var dao = new UserDAO();

            var check = dao.UpdateChucVu(userRoleGroup, role);

            return Json(new { code = 200, data = check }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetQuyenALL(int page = 1, int pageSize = 10)
        {
            var dao = new UserDAO();

            IPagedList model = (IPagedList)dao.GetQuyen(page, pageSize);

            int c = model.TotalItemCount;


            return Json(new { code = 200, data = model, total = c }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteChucVu(int id)
        {
            var dao = new UserDAO();

            var model = dao.DelelteChucVu(id);

            return Json(new { code = 200, data = model }, JsonRequestBehavior.AllowGet);
        }

    }
}