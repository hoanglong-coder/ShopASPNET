using PROJECT_WEBSITE.Data.DAO;
using PROJECT_WEBSITE.WebAPP.Areas.Admin.Models;
using PROJECT_WEBSITE.WebAPP.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PROJECT_WEBSITE.WebAPP.Areas.Admin.Controllers
{
    public class UserLoginController : Controller
    {
        // GET: Admin/UserLogin
        public ActionResult Index()
        {
            

            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var dao = new UserDAO();
                var result = dao.Login(model.UserName, Encryptor.MD5Hash(model.Password));
                if (result == 1)
                {
                    var user = dao.GetById(model.UserName);
                    var userSession = new UserLogin();
                    userSession.UserName = user.UserName;
                    userSession.UserID = user.UserID;
                    userSession.FullName = user.FullName;
                    userSession.Role = user.UserRoleGroupID.Value;
                    userSession.RoleString = dao.GetNameRole(userSession.Role);
                    userSession.Roles = dao.GetListRoles(userSession.Role);
                    Session.Add(Common.CommonConstants.USER_SESSION, userSession);
                    return RedirectToAction("Index", "Home");
                }
                else if (result == 0)
                {
                    ModelState.AddModelError("", "Tài khoản không tồn tại.");
                }
                else if (result == -1)
                {
                    ModelState.AddModelError("", "Tài khoản bị khóa.");
                }
                else if (result == -2)
                {
                    ModelState.AddModelError("", "Mật khẩu không đúng.");
                }
                else
                {
                    ModelState.AddModelError("", "Đăng nhập không thành công.");
                }
            }
            return View("Index");

        }
        
        [HttpGet]
        public JsonResult GetUser()
        {
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            return Json(new { code = 200, data = session }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ClearSessuion()
        {
            Session.Remove(Common.CommonConstants.USER_SESSION);

            return View("Index");
        }
    }
}