using PROJECT_WEBSITE.Data.DAO;
using PROJECT_WEBSITE.Data.EF;
using PROJECT_WEBSITE.WebAPP.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace PROJECT_WEBSITE.WebAPP.Areas.Admin.Controllers
{
    public class SlideController : RoleQuanLySlideController
    {
        // GET: Admin/Slide
        public ActionResult Index()
        {

            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            ViewBag.UserName = session.FullName;

            ViewBag.Role = session.RoleString;

            return View();
        }

        [HttpGet]
        public JsonResult LoadSlide()
        {
            var dao = new SildeDAO();

            var rs = dao.getall();

            return Json(new { code = 200, data = rs }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetID(int id)
        {
            var dao = new SildeDAO();

            var rs = dao.GetID(id);

            List<string> lst = new List<string>();
            if (rs.ImageMore != null && rs.ImageMore != "[]")
            {
                XElement ImageMore = XElement.Parse(rs.ImageMore);

                foreach (XElement item in ImageMore.Elements())
                {
                    lst.Add(item.Value);
                }
            }

            return Json(new { code = 200, data = rs, images = lst }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult Update(Slide slide)
        {
            var dao = new SildeDAO();

            var rs = dao.Update(slide);

            return Json(new { code = 200, data = rs }, JsonRequestBehavior.AllowGet);
        }
    }
}