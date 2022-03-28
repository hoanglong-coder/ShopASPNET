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
    public class HomeController : RoleDashboardController
    {
        // GET: Admin/Home
        public ActionResult Index()
        {

            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            ViewBag.UserName = session.FullName;

            ViewBag.Role = session.RoleString;


            return View();
        }

        [HttpGet]
        public JsonResult GetListChonThoiGian()
        {
            var dao = new ThongKeDAO();

            var model = dao.selectDashBoard();

            return Json(new { code = 200, data = model}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetLabelDay(SearchNews search,int value = 1)
        {
            var dao = new ThongKeDAO();

            var model = dao.getdaydashboard(value);

            var doanhso = dao.GetDoanhSo(value,search);

            var donhang = dao.GetDonHang(value, search);

            var soluong = dao.GetSoluongBan(value,search);

            var lai = dao.GetLai(value,search);

            var tongkho = dao.GetTongKho(value);

            var tonkho = dao.GetTonKho(value);

            var dahet = dao.GetDaHet(value);

            SearchThongKe searchThongKe = new SearchThongKe();

            searchThongKe.TuNgay = search.TuNgay;

            searchThongKe.DenNgay = search.DenNgay;

            var bieudodoanhso = dao.GetBuiDoThongKeDoanhSo(value, searchThongKe);


            //Doanh số
            var labeldoannso = new List<string>();

            var doanhsos = new List<decimal>();

            foreach (var item in bieudodoanhso)
            {
                string label = item.Ngay.Value.ToString("dd/MM/yyyy");
                labeldoannso.Add(label);
            }

            foreach (var item in bieudodoanhso)
            {
                decimal doanhso1 = item.TienHang.Value;
                doanhsos.Add(doanhso1);
            }


            //Số lượng bán

            var soluongs = new List<decimal>();

            foreach (var item in bieudodoanhso)
            {
                decimal doanhso1 = item.SoLuong;
                soluongs.Add(doanhso1);
            }


            var lstpie = new List<string>();

            lstpie.Add("Tổng kho");
            lstpie.Add("Tồn kho");
            lstpie.Add("Hết hàng");

            var lstvalue = new List<int>();

            lstvalue.Add(tongkho.Value);
            lstvalue.Add(tonkho.Value);
            lstvalue.Add(dahet.Value);





            return Json(new { code = 200, data = model
                ,DoanhSo = doanhso,DonHang = donhang,
                Soluong = soluong,LaiGop = lai, TongKho = tongkho, 
                TonKho = tonkho, Dahet = dahet,LabelDoanhSo = labeldoannso,
                ValueDoanhSo = doanhsos, ValueSoluong = soluongs, lstpie = lstpie
            ,
                lstvalue = lstvalue, search = search
            }, JsonRequestBehavior.AllowGet);
        }
    }
}