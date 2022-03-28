using PROJECT_WEBSITE.WebAPP.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PROJECT_WEBSITE.WebAPP.Areas.Admin.Controllers
{
    //admin
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];
     
            if (session == null)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "UserLogin", action = "Index", areas = "Admin" }));
            }
            base.OnActionExecuting(filterContext);
        }
    }

    public class RoleDashboardController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];
            
            if (session == null||session.Roles.Where(t=>t ==(int)Role.Dashboard).Count()==0)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "UserLogin", action = "Index", areas = "Admin" }));
            }
            base.OnActionExecuting(filterContext);
        }
    }

    public class RoleDonHangController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            var lst = session.Roles.Where(t => t == (int)Role.QuanLyDonhang).ToList();

            if (session == null|| lst.Count() == 0)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "UserLogin", action = "Index", areas = "Admin" }));
            }
            base.OnActionExecuting(filterContext);
        }
    }

    public class RoleKhoHangController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            if (session == null || session.Roles.Where(t => t == (int)Role.QuanLyKhoHang).Count() == 0)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "UserLogin", action = "Index", areas = "Admin" }));
            }
            base.OnActionExecuting(filterContext);
        }
    }

    public class RoleNhapHangController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            var lst2 = session.Roles.ToList();

            var lst = session.Roles.Where(t => t == (int)Role.QuanLyNhapHang).ToList();

            if (session == null || session.Roles.Where(t => t == (int)Role.QuanLyNhapHang).Count() == 0)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "UserLogin", action = "Index", areas = "Admin" }));
            }
            base.OnActionExecuting(filterContext);
        }
    }

    public class RoleComBoController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            if (session == null || session.Roles.Where(t => t == (int)Role.QuanLyCombo).Count() == 0)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "UserLogin", action = "Index", areas = "Admin" }));
            }
            base.OnActionExecuting(filterContext);
        }
    }

    public class RoleMaKhuyenMaiController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            if (session == null || session.Roles.Where(t => t == (int)Role.QuanLyMaKhuyenMai).Count() == 0)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "UserLogin", action = "Index", areas = "Admin" }));
            }
            base.OnActionExecuting(filterContext);
        }
    }

    public class RoleQuanLyKhachHangKhachHangController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            if (session == null || session.Roles.Where(t => t == (int)Role.QuanLyKhachHang).Count() == 0)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "UserLogin", action = "Index", areas = "Admin" }));
            }
            base.OnActionExecuting(filterContext);
        }
    }
    
    public class RoleQuanLyTinTucController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            if (session == null || session.Roles.Where(t => t == (int)Role.QuanLyTinTuc).Count() == 0)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "UserLogin", action = "Index", areas = "Admin" }));
            }
            base.OnActionExecuting(filterContext);
        }
    }
    
    public class RoleQuanLyGioiThieuController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            if (session == null || session.Roles.Where(t => t == (int)Role.QuanLyGioiThieu).Count() == 0)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "UserLogin", action = "Index", areas = "Admin" }));
            }
            base.OnActionExecuting(filterContext);
        }
    }
    
    public class RoleQuanLySlideController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            if (session == null || session.Roles.Where(t => t == (int)Role.QuanLySlide).Count() == 0)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "UserLogin", action = "Index", areas = "Admin" }));
            }
            base.OnActionExecuting(filterContext);
        }
    }

    public class RoleQuanLyChanTrangController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            if (session == null || session.Roles.Where(t => t == (int)Role.QuanLyChanTrang).Count() == 0)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "UserLogin", action = "Index", areas = "Admin" }));
            }
            base.OnActionExecuting(filterContext);
        }
    }

    public class RoleBaoCaoDoanhSoController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            if (session == null || session.Roles.Where(t => t == (int)Role.BaoCaoDoanhSo).Count() == 0)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "UserLogin", action = "Index", areas = "Admin" }));
            }
            base.OnActionExecuting(filterContext);
        }
    }

    public class RoleBaoCaoLoiNhuanController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            if (session == null || session.Roles.Where(t => t == (int)Role.BaoCaoLoiNhuan).Count() == 0)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "UserLogin", action = "Index", areas = "Admin" }));
            }
            base.OnActionExecuting(filterContext);
        }
    }

    public class RoleTOPKController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            if (session == null || session.Roles.Where(t => t == (int)Role.BaoCaoLoiNhuan).Count() == 0)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "UserLogin", action = "Index", areas = "Admin" }));
            }
            base.OnActionExecuting(filterContext);
        }
    }

    public class RoleQuanLyNhanVienController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            if (session == null || session.Roles.Where(t => t == (int)Role.QuanLyNhanVien).Count() == 0)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "UserLogin", action = "Index", areas = "Admin" }));
            }
            base.OnActionExecuting(filterContext);
        }
    }
}