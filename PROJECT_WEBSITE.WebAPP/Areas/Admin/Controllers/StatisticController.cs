using ClosedXML.Excel;
using PagedList;
using PROJECT_WEBSITE.Data.DAO;
using PROJECT_WEBSITE.Data.EF;
using PROJECT_WEBSITE.Data.ModelCustom;
using PROJECT_WEBSITE.WebAPP.Common;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PROJECT_WEBSITE.WebAPP.Areas.Admin.Controllers
{
    //Doanh số
    public class StatisticController : RoleBaoCaoDoanhSoController
    {
        // GET: Admin/StatisticControlle
        public ActionResult Index()
        {

            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            ViewBag.UserName = session.FullName;

            ViewBag.Role = session.RoleString;

            return View();
        }
        //List1
        [HttpPost]
        public JsonResult GetThongKeTheoNgay(SearchThongKe search,int page = 1, int pageSize = 3)
        {
            var dao = new ThongKeDAO();

            IPagedList model = (IPagedList)dao.GetThongKeThang(search, page, pageSize);

            var lst = dao.GetThongKeThang(search,1,10000000);

            int c = model.TotalItemCount;

            return Json(new { code = 200, data = model, total = c, countorder = lst.Sum(t=>t.SoDonHang), countproduct = lst.Sum(t=>t.SoLuong), totalprice = lst.Sum(t=>t.TienHang) }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GetThongKeDetailTheoNgay(DateTime createDate, int page = 1, int pageSize = 3)
        {
            var dao = new ThongKeDAO();

            SearchThongKe search = new SearchThongKe();

            IPagedList model = (IPagedList)dao.ListOrderTheoNgay(search, createDate, page, pageSize);


            var totalprice = dao.ListOrderTheoNgay(search, createDate, 1,10000000).ToList();

            int c = model.TotalItemCount;

            return Json(new { code = 200, data = model, total = c, totalprice = totalprice.Sum(t => t.TotalPrice), totalcout = totalprice.Sum(t => t.TotalCount) }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Xuất doanh số hóa đơn theo ngày
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ExportExcelListThongkeNgay(DateTime createDate)
        {
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            SearchThongKe search = new SearchThongKe();

            var dao = new ThongKeDAO();

            var model = dao.ListOrderTheoNgay(search, createDate, 1, 10000000).ToList();

            var wb = new XLWorkbook();

            var ws = wb.Worksheets.Add("DoanhSoNgay");
            ws.Column("A").Width = 4.18;
            ws.Column("B").Width = 14;
            ws.Column("C").Width = 16;
            ws.Column("D").Width = 16;
            ws.Column("E").Width = 15;
            ws.Column("F").Width = 10;
            ws.Column("G").Width = 17;
            ws.Column("H").Width = 20;
            ws.Column("I").Width = 20;

            ws.Cell("A1").Style.Font.Bold = true;
            ws.Cell("A1").Style.Font.FontSize = 13;
            ws.Cell("A1").Value = "CỬA HÀNG CHUỔI THỰC PHẨM TH True Milk";
            ws.Range("A1:E1").Row(1).Merge();

            ws.Cell("A2").Style.Font.Bold = true;
            ws.Cell("A2").Style.Font.FontSize = 11;
            ws.Cell("A2").Value = "SĐT:028 3602 1133 - ĐC:204/16/5 Quốc Lộ 13, P.26, Q.Bình Thạnh, Tp.HCM";
            ws.Range("A2:E2").Row(1).Merge();

            ws.Cell("A3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A3").Style.Font.Bold = true;
            ws.Cell("A3").Style.Font.FontSize = 16;
            ws.Cell("A3").Value = "THỐNG KÊ DOANH SỐ";
            ws.Range("A3:I3").Row(1).Merge();

            ws.Cell("A4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A4").Style.Font.Bold = true;
            ws.Cell("A4").Style.Font.FontSize = 11;
            ws.Cell("A4").Value = $"(Ngày: {createDate.ToString("dd/MM/yyyy")})";
            ws.Range("A4:I3").Row(1).Merge();


            var rangeHeader = ws.Range("A5:I5");
            rangeHeader.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangeHeader.Style.Font.Bold = true;
            rangeHeader.Style.Fill.BackgroundColor = XLColor.Orange;
            rangeHeader.Style.Font.FontColor = XLColor.White;
            rangeHeader.Style.Font.FontSize = 12;

            ws.Cell("A5").Value = "STT";

            ws.Cell("B5").Value = "Mã hóa đơn";

            ws.Cell("C5").Value = "Mã khách hàng";
            ws.Cell("D5").Value = "Khách hàng";
            ws.Cell("E5").Value = "Ngày tạo";
            ws.Cell("F5").Value = "Số lượng";
            ws.Cell("G5").Value = "Tổng tiền";
            ws.Cell("H5").Value = "PT Thanh toán";
            ws.Cell("I5").Value = "Người xác nhận";

            int row = 6;
            double total = 0;
            int soluong = 0;
            for (int i = 0; i < model.Count; i++)
            {
                ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("A" + row).Value = model[i].STT;

                ws.Cell("B" + row).Value = "HD" + ChangeIDProduct(model[i].OrderID);

                ws.Cell("C" + row).Value = "KH" + ChangeIDProduct(model[i].CustomerID.Value);
                ws.Cell("D" + row).Value = model[i].ShipName;
                ws.Cell("E" + row).Value = model[i].CreateDate;
                ws.Cell("F" + row).Value = model[i].TotalCount;
                ws.Cell("G" + row).Style.NumberFormat.Format = "#,##0";
                ws.Cell("G" + row).Value = (model[i].TotalPrice);
                ws.Cell("H" + row).Value = model[i].PaymentName;
                ws.Cell("I" + row).Value = model[i].UserName;
                total += (double)(model[i].TotalPrice);
                soluong += model[i].TotalCount.Value;
                row++;
            }

            var rangeborder = ws.Range("A5:I" + (row));
            rangeborder.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rangeborder.Style.Border.InsideBorder = XLBorderStyleValues.Thin;


            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Value = "Tổng cộng";
            ws.Range($"B{row}:D{row}").Row(1).Merge();

            ws.Cell("F" + row).Style.Font.Bold = true;
            ws.Cell("F" + row).Value = soluong;

            ws.Cell("G" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("G" + row).Style.NumberFormat.Format = "#,##0";
            ws.Cell("G" + row).Style.Font.Bold = true;
            ws.Cell("G" + row).Value = total.ToString("N0") + "VNĐ";

            //tiền chữ
            string value = "A" + (row + 1);
            string valuerange = value + ":I" + (row + 1);
            ws.Cell(value).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell(value).Style.Font.Bold = true;
            ws.Cell(value).Style.Font.FontSize = 11;
            ws.Cell(value).Value = "Bằng chữ: " + new ConvertCurrency().ToUpper(total);
            ws.Range(valuerange).Row(1).Merge();

            //Ký tên
            ws.Cell("F" + (row + 2)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F" + (row + 2)).Style.Font.Italic = true;
            ws.Cell("F" + (row + 2)).Value = $"TP.HCM, ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}";
            ws.Range($"F{row + 2}:H{row + 2}").Row(1).Merge();


            ws.Cell("F" + (row + 3)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F" + (row + 3)).Style.Font.Bold = true;
            ws.Cell("F" + (row + 3)).Value = "Người lập phiếu";
            ws.Range($"F{row + 3}:H{row + 3}").Row(1).Merge();

            ws.Cell("F" + (row + 4)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F" + (row + 4)).Style.Font.Italic = true;
            ws.Cell("F" + (row + 4)).Value = "(Ký, họ tên, đóng dấu)";
            ws.Range($"F{row + 4}:H{row + 4}").Row(1).Merge();

            ws.Cell("F" + (row + 6)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F" + (row + 6)).Style.Font.Bold = true;
            ws.Cell("F" + (row + 6)).Value = NguoiLapPhieu(session.UserID);
            ws.Range($"F{row + 6}:H{row + 6}").Row(1).Merge();


            string namefile = "DoanhSoNgay_"+DateTime.Now.Date.ToString("dd_MM_yyyy")+" " + DateTime.Now.Ticks + ".xlsx";
            string fileLPath = Server.MapPath(@"~/Data/ExportExcel/" + namefile);
            wb.SaveAs(fileLPath);

            return Json(new { code = 200, NameFile = namefile }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Xuất doanh số pdf hóa đơn theo ngày
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ExportPDFListThongkeNgay(DateTime createDate)
        {
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            var dao = new ThongKeDAO();

            SearchThongKe search = new SearchThongKe();

            var model = dao.ListOrderTheoNgay(search, createDate, 1, 10000000).ToList();

            HtmlToPdf htmlToPdf = new HtmlToPdf();

            //set option
            htmlToPdf.Options.PdfPageSize = PdfPageSize.A4;
            htmlToPdf.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
            htmlToPdf.Options.MarginLeft = 30;
            htmlToPdf.Options.MarginRight = 30;
            htmlToPdf.Options.MarginTop = 20;
            htmlToPdf.Options.MarginBottom = 20;


            string ret = RenderPartialToStringNgay("~/Areas/Admin/Views/Statistic/ExportPDFNgay.cshtml", model, ControllerContext, createDate, NguoiLapPhieu(session.UserID));

            PdfDocument pdf = htmlToPdf.ConvertHtmlString(ret);


            string namefile = "DoanhSoNgay_" + DateTime.Now.Date.ToString("dd_MM_yyyy") + " " + DateTime.Now.Ticks + ".pdf";
            string fileLPath = Server.MapPath(@"~/Data/ExportPDF/" + namefile);
            pdf.Save(fileLPath);
            pdf.Close();

            return Json(new { code = 200, NameFile = namefile }, JsonRequestBehavior.AllowGet);
        }

        public string RenderPartialToStringNgay(string viewName, object model, ControllerContext ControllerContext, DateTime Ngay, string username)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");
            ViewDataDictionary ViewData = new ViewDataDictionary();
            TempDataDictionary TempData = new TempDataDictionary();
            ViewData.Model = model;
            ViewData["Ngay"] = Ngay;
            ViewData["username"] = username;
            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }

        }

        /// <summary>
        /// Xuất danh sách thống kê hóa đơn 
        /// </summary>
        /// <param name="createDate"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ExportExcelThongkeNgay(SearchThongKe search)
        {
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            var dao = new ThongKeDAO();

            var model = dao.GetThongKeThang(search, 1, 10000000).ToList();

            var wb = new XLWorkbook();

            var ws = wb.Worksheets.Add("DanhSachDoanhSoNgay");
            ws.Column("A").Width = 4.18;
            ws.Column("B").Width = 14;
            ws.Column("C").Width = 16;
            ws.Column("D").Width = 16;
            ws.Column("E").Width = 15;

            ws.Cell("A1").Style.Font.Bold = true;
            ws.Cell("A1").Style.Font.FontSize = 13;
            ws.Cell("A1").Value = "CỬA HÀNG CHUỔI THỰC PHẨM TH True Milk";
            ws.Range("A1:E1").Row(1).Merge();

            ws.Cell("A2").Style.Font.Bold = true;
            ws.Cell("A2").Style.Font.FontSize = 11;
            ws.Cell("A2").Value = "SĐT:028 3602 1133 - ĐC:204/16/5 Quốc Lộ 13, P.26, Q.Bình Thạnh, Tp.HCM";
            ws.Range("A2:E2").Row(1).Merge();

            ws.Cell("A3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A3").Style.Font.Bold = true;
            ws.Cell("A3").Style.Font.FontSize = 16;
            ws.Cell("A3").Value = "THỐNG KÊ DOANH SỐ";
            ws.Range("A3:E3").Row(1).Merge();

            ws.Cell("A4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A4").Style.Font.Bold = true;
            ws.Cell("A4").Style.Font.FontSize = 11;
            if (search.TuNgay.HasValue && search.DenNgay.HasValue)
            {
                ws.Cell("A4").Value = $"(Từ ngày: {search.TuNgay.Value.ToString("dd/MM/yyyy")} - Đến ngày: {search.DenNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else if (search.TuNgay.HasValue)
            {
                ws.Cell("A4").Value = $"(Từ ngày: {search.TuNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else if (search.DenNgay.HasValue)
            {
                ws.Cell("A4").Value = $"(Đến ngày: {search.DenNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else
            {
                ws.Cell("A4").Value = $"(Tất cả hóa đơn)";
            }
            ws.Range("A4:E3").Row(1).Merge();


            var rangeHeader = ws.Range("A5:E5");
            rangeHeader.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangeHeader.Style.Font.Bold = true;
            rangeHeader.Style.Fill.BackgroundColor = XLColor.Orange;
            rangeHeader.Style.Font.FontColor = XLColor.White;
            rangeHeader.Style.Font.FontSize = 12;

            ws.Cell("A5").Value = "STT";
            ws.Cell("B5").Value = "Ngày";
            ws.Cell("C5").Value = "Số đơn hàng";
            ws.Cell("D5").Value = "Số lượng";
            ws.Cell("E5").Value = "Tiền hàng";

            int row = 6;
            double total = 0;
            int soluong = 0;
            int sodonhang = 0;
            for (int i = 0; i < model.Count; i++)
            {
                ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("A" + row).Value = model[i].STT;

                ws.Cell("B" + row).Value = model[i].Ngay.Value.ToString("dd/MM/yyyy");

                ws.Cell("C" + row).Value = model[i].SoDonHang;
                ws.Cell("D" + row).Value = model[i].SoLuong;
                ws.Cell("E" + row).Style.NumberFormat.Format = "#,##0";
                ws.Cell("E" + row).Value = model[i].TienHang;
                total += (double)(model[i].TienHang);
                soluong += model[i].SoLuong;
                sodonhang += model[i].SoDonHang;
                row++;
            }

            var rangeborder = ws.Range("A5:E" + (row));
            rangeborder.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rangeborder.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Value = "Tổng cộng";

            ws.Cell("C" + row).Style.Font.Bold = true;
            ws.Cell("C" + row).Value = sodonhang;

            ws.Cell("D" + row).Style.Font.Bold = true;
            ws.Cell("D" + row).Value = soluong;

            ws.Cell("E" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("E" + row).Style.NumberFormat.Format = "#,##0";
            ws.Cell("E" + row).Style.Font.Bold = true;
            ws.Cell("E" + row).Value = total.ToString("N0") + "VNĐ";

            //tiền chữ
            string value = "A" + (row + 1);
            string valuerange = value + ":E" + (row + 1);
            ws.Cell(value).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell(value).Style.Font.Bold = true;
            ws.Cell(value).Style.Font.FontSize = 11;
            ws.Cell(value).Value = "Bằng chữ: " + new ConvertCurrency().ToUpper(total);
            ws.Range(valuerange).Row(1).Merge();

            //Ký tên
            ws.Cell("D" + (row + 2)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + (row + 2)).Style.Font.Italic = true;
            ws.Cell("D" + (row + 2)).Value = $"TP.HCM, ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}";
            ws.Range($"D{row + 2}:E{row + 2}").Row(1).Merge();


            ws.Cell("D" + (row + 3)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + (row + 3)).Style.Font.Bold = true;
            ws.Cell("D" + (row + 3)).Value = "Người lập phiếu";
            ws.Range($"D{row + 3}:E{row + 3}").Row(1).Merge();

            ws.Cell("D" + (row + 4)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + (row + 4)).Style.Font.Italic = true;
            ws.Cell("D" + (row + 4)).Value = "(Ký, họ tên, đóng dấu)";
            ws.Range($"D{row + 4}:E{row + 4}").Row(1).Merge();

            ws.Cell("D" + (row + 6)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + (row + 6)).Style.Font.Bold = true;
            ws.Cell("D" + (row + 6)).Value = NguoiLapPhieu(session.UserID);
            ws.Range($"D{row + 6}:E{row + 6}").Row(1).Merge();


            string namefile = "DSDoanhSoNgay_" + DateTime.Now.Ticks + ".xlsx";
            string fileLPath = Server.MapPath(@"~/Data/ExportExcel/" + namefile);
            wb.SaveAs(fileLPath);

            return Json(new { code = 200, NameFile = namefile }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Xuất danh sách thống kê hóa đơn 
        /// </summary>
        /// <param name="createDate"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ExportPDFThongkeNgay(SearchThongKe search)
        {
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            var dao = new ThongKeDAO();

            var model = dao.GetThongKeThang(search, 1, 10000000).ToList();

            HtmlToPdf htmlToPdf = new HtmlToPdf();

            //set option
            htmlToPdf.Options.PdfPageSize = PdfPageSize.A4;
            htmlToPdf.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
            htmlToPdf.Options.MarginLeft = 30;
            htmlToPdf.Options.MarginRight = 30;
            htmlToPdf.Options.MarginTop = 20;
            htmlToPdf.Options.MarginBottom = 20;
            string createDate = "";
            if (search.TuNgay.HasValue && search.DenNgay.HasValue)
            {
                createDate = $"(Từ ngày: {search.TuNgay.Value.ToString("dd/MM/yyyy")} - Đến ngày: {search.DenNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else if (search.TuNgay.HasValue)
            {
                createDate = $"(Từ ngày: {search.TuNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else if (search.DenNgay.HasValue)
            {
                createDate = $"(Đến ngày: {search.DenNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else
            {
                createDate = $"(Tất cả hóa đơn)";
            }

            string ret = RenderPartialToStringDSNgay("~/Areas/Admin/Views/Statistic/ExportPDFDSNgay.cshtml", model, ControllerContext, createDate, NguoiLapPhieu(session.UserID));

            PdfDocument pdf = htmlToPdf.ConvertHtmlString(ret);


            string namefile = "DSDoanhSoNgay_" + DateTime.Now.Ticks+ ".pdf";
            string fileLPath = Server.MapPath(@"~/Data/ExportPDF/" + namefile);
            pdf.Save(fileLPath);
            pdf.Close();

            return Json(new { code = 200, NameFile = namefile }, JsonRequestBehavior.AllowGet);

        }

        public string RenderPartialToStringDSNgay(string viewName, object model, ControllerContext ControllerContext, string Ngay, string username)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");
            ViewDataDictionary ViewData = new ViewDataDictionary();
            TempDataDictionary TempData = new TempDataDictionary();
            ViewData.Model = model;
            ViewData["Ngay"] = Ngay;
            ViewData["username"] = username;
            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }

        }
        
        public string ChangeIDProduct(int id)
        {
            if (id <= 9)
            {
                return "000" + id;
            }
            else if (id <= 99)
            {
                return "00" + id;
            }
            else if (id <= 999)
            {
                return "0" + id;
            }
            else
            {
                return id.ToString();
            }
        }

        public string NguoiLapPhieu(int id)
        {
            var db = new DbWebsite();
            var check = db.Orders.Find(id).UserID;
            if (check.HasValue)
            {
                return db.Orders.Find(id).User.FullName;
            }
            else
            {
                return "";
            }
        }


        //List2
        [HttpPost]
        public JsonResult GetThongKeTheoNhanVien(SearchThongKe search, int page = 1, int pageSize = 3)
        {
            var dao = new ThongKeDAO();

            IPagedList model = (IPagedList)dao.GetThongKeNhanVien(search, page, pageSize);

            var lst = dao.GetThongKeNhanVien(search, 1, 10000000);

            int c = model.TotalItemCount;

            return Json(new { code = 200, data = model, total = c, countorder = lst.Sum(t => t.SoDonHang), countproduct = lst.Sum(t => t.SoLuong), totalprice = lst.Sum(t => t.TienHang) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetThongKeDetailTheoNhanvien(SearchThongKe search, int id, int page = 1, int pageSize = 3)
        {
            var dao = new ThongKeDAO();

            IPagedList model = (IPagedList)dao.ListOrderTheoNhanVien(search, id, page, pageSize);


            var totalprice = dao.ListOrderTheoNhanVien(search, id, 1, 10000000).ToList();

            int c = model.TotalItemCount;

            return Json(new { code = 200, data = model, total = c, totalprice = totalprice.Sum(t => t.TotalPrice), totalcout = totalprice.Sum(t => t.TotalCount) }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Export danh sách thống kê theo nhân viên
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ExportExcelListThongkeNhanVien(SearchThongKe search, int id)
        {

            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            var dao = new ThongKeDAO();

            var model = dao.ListOrderTheoNhanVien(search, id, 1, 10000000).ToList();

            var wb = new XLWorkbook();

            var ws = wb.Worksheets.Add("DoanhSoNV");
            ws.Column("A").Width = 4.18;
            ws.Column("B").Width = 14;
            ws.Column("C").Width = 16;
            ws.Column("D").Width = 16;
            ws.Column("E").Width = 15;
            ws.Column("F").Width = 10;
            ws.Column("G").Width = 17;
            ws.Column("H").Width = 20;
            ws.Column("I").Width = 20;

            ws.Cell("A1").Style.Font.Bold = true;
            ws.Cell("A1").Style.Font.FontSize = 13;
            ws.Cell("A1").Value = "CỬA HÀNG CHUỔI THỰC PHẨM TH True Milk";
            ws.Range("A1:E1").Row(1).Merge();

            ws.Cell("A2").Style.Font.Bold = true;
            ws.Cell("A2").Style.Font.FontSize = 11;
            ws.Cell("A2").Value = "SĐT:028 3602 1133 - ĐC:204/16/5 Quốc Lộ 13, P.26, Q.Bình Thạnh, Tp.HCM";
            ws.Range("A2:E2").Row(1).Merge();

            ws.Cell("A3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A3").Style.Font.Bold = true;
            ws.Cell("A3").Style.Font.FontSize = 16;
            ws.Cell("A3").Value = "THỐNG KÊ DOANH SỐ NHÂN VIÊN: " + NguoiLapPhieu(id);
            ws.Range("A3:I3").Row(1).Merge();

            ws.Cell("A4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A4").Style.Font.Bold = true;
            ws.Cell("A4").Style.Font.FontSize = 11;
            if (search.TuNgay.HasValue && search.DenNgay.HasValue)
            {
                ws.Cell("A4").Value = $"(Từ ngày: {search.TuNgay.Value.ToString("dd/MM/yyyy")} - Đến ngày: {search.DenNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else if (search.TuNgay.HasValue)
            {
                ws.Cell("A4").Value = $"(Từ ngày: {search.TuNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else if (search.DenNgay.HasValue)
            {
                ws.Cell("A4").Value = $"(Đến ngày: {search.DenNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else
            {
                ws.Cell("A4").Value = $"(Tất cả hóa đơn)";
            }
            ws.Range("A4:I3").Row(1).Merge();


            var rangeHeader = ws.Range("A5:I5");
            rangeHeader.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangeHeader.Style.Font.Bold = true;
            rangeHeader.Style.Fill.BackgroundColor = XLColor.Orange;
            rangeHeader.Style.Font.FontColor = XLColor.White;
            rangeHeader.Style.Font.FontSize = 12;

            ws.Cell("A5").Value = "STT";

            ws.Cell("B5").Value = "Mã hóa đơn";

            ws.Cell("C5").Value = "Mã khách hàng";
            ws.Cell("D5").Value = "Khách hàng";
            ws.Cell("E5").Value = "Ngày tạo";
            ws.Cell("F5").Value = "Số lượng";
            ws.Cell("G5").Value = "Tổng tiền";
            ws.Cell("H5").Value = "PT Thanh toán";
            ws.Cell("I5").Value = "Người xác nhận";

            int row = 6;
            double total = 0;
            int soluong = 0;
            for (int i = 0; i < model.Count; i++)
            {
                ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("A" + row).Value = model[i].STT;

                ws.Cell("B" + row).Value = "HD" + ChangeIDProduct(model[i].OrderID);

                ws.Cell("C" + row).Value = "KH" + ChangeIDProduct(model[i].CustomerID.Value);
                ws.Cell("D" + row).Value = model[i].ShipName;
                ws.Cell("E" + row).Value = model[i].CreateDate;
                ws.Cell("F" + row).Value = model[i].TotalCount;
                ws.Cell("G" + row).Style.NumberFormat.Format = "#,##0";
                ws.Cell("G" + row).Value = (model[i].TotalPrice);
                ws.Cell("H" + row).Value = model[i].PaymentName;
                ws.Cell("I" + row).Value = model[i].UserName;
                total += (double)(model[i].TotalPrice);
                soluong += model[i].TotalCount.Value;
                row++;
            }

            var rangeborder = ws.Range("A5:I" + (row));
            rangeborder.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rangeborder.Style.Border.InsideBorder = XLBorderStyleValues.Thin;


            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Value = "Tổng cộng";
            ws.Range($"B{row}:D{row}").Row(1).Merge();

            ws.Cell("F" + row).Style.Font.Bold = true;
            ws.Cell("F" + row).Value = soluong;

            ws.Cell("G" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("G" + row).Style.NumberFormat.Format = "#,##0";
            ws.Cell("G" + row).Style.Font.Bold = true;
            ws.Cell("G" + row).Value = total.ToString("N0") + "VNĐ";

            //tiền chữ
            string value = "A" + (row + 1);
            string valuerange = value + ":I" + (row + 1);
            ws.Cell(value).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell(value).Style.Font.Bold = true;
            ws.Cell(value).Style.Font.FontSize = 11;
            ws.Cell(value).Value = "Bằng chữ: " + new ConvertCurrency().ToUpper(total);
            ws.Range(valuerange).Row(1).Merge();

            //Ký tên
            ws.Cell("F" + (row + 2)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F" + (row + 2)).Style.Font.Italic = true;
            ws.Cell("F" + (row + 2)).Value = $"TP.HCM, ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}";
            ws.Range($"F{row + 2}:H{row + 2}").Row(1).Merge();


            ws.Cell("F" + (row + 3)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F" + (row + 3)).Style.Font.Bold = true;
            ws.Cell("F" + (row + 3)).Value = "Người lập phiếu";
            ws.Range($"F{row + 3}:H{row + 3}").Row(1).Merge();

            ws.Cell("F" + (row + 4)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F" + (row + 4)).Style.Font.Italic = true;
            ws.Cell("F" + (row + 4)).Value = "(Ký, họ tên, đóng dấu)";
            ws.Range($"F{row + 4}:H{row + 4}").Row(1).Merge();

            ws.Cell("F" + (row + 6)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F" + (row + 6)).Style.Font.Bold = true;
            ws.Cell("F" + (row + 6)).Value = NguoiLapPhieu(session.UserID);
            ws.Range($"F{row + 6}:H{row + 6}").Row(1).Merge();


            string namefile = "DoanhSoNV_NV" + session.UserID + " " + DateTime.Now.Ticks + ".xlsx";
            string fileLPath = Server.MapPath(@"~/Data/ExportExcel/" + namefile);
            wb.SaveAs(fileLPath);

            return Json(new { code = 200, NameFile = namefile }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Xuất doanh số pdf hóa đơn theo nhaan vien
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ExportPDFListThongkeNhanVien(SearchThongKe search, int id)
        {
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            var dao = new ThongKeDAO();

            var model = dao.ListOrderTheoNhanVien(search, id, 1, 10000000).ToList();

            HtmlToPdf htmlToPdf = new HtmlToPdf();

            //set option
            htmlToPdf.Options.PdfPageSize = PdfPageSize.A4;
            htmlToPdf.Options.PdfPageOrientation = PdfPageOrientation.Landscape;
            htmlToPdf.Options.MarginLeft = 30;
            htmlToPdf.Options.MarginRight = 30;
            htmlToPdf.Options.MarginTop = 20;
            htmlToPdf.Options.MarginBottom = 20;
            string ngay = "";
            if (search.TuNgay.HasValue && search.DenNgay.HasValue)
            {
                ngay = $"(Từ ngày: {search.TuNgay.Value.ToString("dd/MM/yyyy")} - Đến ngày: {search.DenNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else if (search.TuNgay.HasValue)
            {
                ngay = $"(Từ ngày: {search.TuNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else if (search.DenNgay.HasValue)
            {
                ngay = $"(Đến ngày: {search.DenNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else
            {
                ngay = $"(Tất cả hóa đơn)";
            }

            string ret = RenderPartialToStringNhanVien("~/Areas/Admin/Views/Statistic/ExportPDFNhanVien.cshtml", model, ControllerContext, ngay,NguoiLapPhieu(id) ,NguoiLapPhieu(session.UserID));

            PdfDocument pdf = htmlToPdf.ConvertHtmlString(ret);


            string namefile = "DoanhSoNV_NV" + session.UserID + " " + DateTime.Now.Ticks + ".pdf";
            string fileLPath = Server.MapPath(@"~/Data/ExportPDF/" + namefile);
            pdf.Save(fileLPath);
            pdf.Close();

            return Json(new { code = 200, NameFile = namefile }, JsonRequestBehavior.AllowGet);
        }

        public string RenderPartialToStringNhanVien(string viewName, object model, ControllerContext ControllerContext, string Ngay,string nhanvien, string nguoilap)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");
            ViewDataDictionary ViewData = new ViewDataDictionary();
            TempDataDictionary TempData = new TempDataDictionary();
            ViewData.Model = model;
            ViewData["Ngay"] = Ngay;
            ViewData["nguoilap"] = nguoilap;
            ViewData["nhanvien"] = nhanvien;
            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }

        }

        /// <summary>
        /// Xuất danh sách thống kê hóa đơn theo nhân viên
        /// </summary>
        /// <param name="createDate"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ExportExcelThongkeNhanVien(SearchThongKe search)
        {
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            var dao = new ThongKeDAO();

            var model = dao.GetThongKeNhanVien(search, 1, 10000000).ToList();

            var wb = new XLWorkbook();

            var ws = wb.Worksheets.Add("DanhSachDoanhSoNV");
            ws.Column("A").Width = 4.18;
            ws.Column("B").Width = 23;
            ws.Column("C").Width = 11;
            ws.Column("D").Width = 9;
            ws.Column("E").Width = 23;

            ws.Cell("A1").Style.Font.Bold = true;
            ws.Cell("A1").Style.Font.FontSize = 13;
            ws.Cell("A1").Value = "CỬA HÀNG CHUỔI THỰC PHẨM TH True Milk";
            ws.Range("A1:E1").Row(1).Merge();

            ws.Cell("A2").Style.Font.Bold = true;
            ws.Cell("A2").Style.Font.FontSize = 11;
            ws.Cell("A2").Value = "SĐT:028 3602 1133 - ĐC:204/16/5 Quốc Lộ 13, P.26, Q.Bình Thạnh, Tp.HCM";
            ws.Range("A2:E2").Row(1).Merge();

            ws.Cell("A3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A3").Style.Font.Bold = true;
            ws.Cell("A3").Style.Font.FontSize = 16;
            ws.Cell("A3").Value = "THỐNG KÊ DOANH SỐ BÁN NHÂN VIÊN";
            ws.Range("A3:E3").Row(1).Merge();

            ws.Cell("A4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A4").Style.Font.Bold = true;
            ws.Cell("A4").Style.Font.FontSize = 11;
            if (search.TuNgay.HasValue && search.DenNgay.HasValue)
            {
                ws.Cell("A4").Value = $"(Từ ngày: {search.TuNgay.Value.ToString("dd/MM/yyyy")} - Đến ngày: {search.DenNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else if (search.TuNgay.HasValue)
            {
                ws.Cell("A4").Value = $"(Từ ngày: {search.TuNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else if (search.DenNgay.HasValue)
            {
                ws.Cell("A4").Value = $"(Đến ngày: {search.DenNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else
            {
                ws.Cell("A4").Value = $"(Tất cả hóa đơn)";
            }
            ws.Range("A4:E3").Row(1).Merge();


            var rangeHeader = ws.Range("A5:E5");
            rangeHeader.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangeHeader.Style.Font.Bold = true;
            rangeHeader.Style.Fill.BackgroundColor = XLColor.Orange;
            rangeHeader.Style.Font.FontColor = XLColor.White;
            rangeHeader.Style.Font.FontSize = 12;

            ws.Cell("A5").Value = "STT";
            ws.Cell("B5").Value = "Người xác nhận";
            ws.Cell("C5").Value = "Số đơn hàng";
            ws.Cell("D5").Value = "Số lượng";
            ws.Cell("E5").Value = "Tiền hàng";

            int row = 6;
            double total = 0;
            int soluong = 0;
            int sodonhang = 0;
            for (int i = 0; i < model.Count; i++)
            {
                ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("A" + row).Value = model[i].STT;

                ws.Cell("B" + row).Value = model[i].UserName;

                ws.Cell("C" + row).Value = model[i].SoDonHang;
                ws.Cell("D" + row).Value = model[i].SoLuong;
                ws.Cell("E" + row).Style.NumberFormat.Format = "#,##0";
                ws.Cell("E" + row).Value = model[i].TienHang;
                total += (double)(model[i].TienHang);
                soluong += model[i].SoLuong;
                sodonhang += model[i].SoDonHang;
                row++;
            }

            var rangeborder = ws.Range("A5:E" + (row));
            rangeborder.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rangeborder.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Value = "Tổng cộng";

            ws.Cell("C" + row).Style.Font.Bold = true;
            ws.Cell("C" + row).Value = sodonhang;

            ws.Cell("D" + row).Style.Font.Bold = true;
            ws.Cell("D" + row).Value = soluong;

            ws.Cell("E" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("E" + row).Style.NumberFormat.Format = "#,##0";
            ws.Cell("E" + row).Style.Font.Bold = true;
            ws.Cell("E" + row).Value = total.ToString("N0") + "VNĐ";

            //tiền chữ
            string value = "A" + (row + 1);
            string valuerange = value + ":E" + (row + 1);
            ws.Cell(value).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell(value).Style.Font.Bold = true;
            ws.Cell(value).Style.Font.FontSize = 11;
            ws.Cell(value).Value = "Bằng chữ: " + new ConvertCurrency().ToUpper(total);
            ws.Range(valuerange).Row(1).Merge();

            //Ký tên
            ws.Cell("D" + (row + 2)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + (row + 2)).Style.Font.Italic = true;
            ws.Cell("D" + (row + 2)).Value = $"TP.HCM, ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}";
            ws.Range($"D{row + 2}:E{row + 2}").Row(1).Merge();


            ws.Cell("D" + (row + 3)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + (row + 3)).Style.Font.Bold = true;
            ws.Cell("D" + (row + 3)).Value = "Người lập phiếu";
            ws.Range($"D{row + 3}:E{row + 3}").Row(1).Merge();

            ws.Cell("D" + (row + 4)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + (row + 4)).Style.Font.Italic = true;
            ws.Cell("D" + (row + 4)).Value = "(Ký, họ tên, đóng dấu)";
            ws.Range($"D{row + 4}:E{row + 4}").Row(1).Merge();

            ws.Cell("D" + (row + 6)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + (row + 6)).Style.Font.Bold = true;
            ws.Cell("D" + (row + 6)).Value = NguoiLapPhieu(session.UserID);
            ws.Range($"D{row + 6}:E{row + 6}").Row(1).Merge();


            string namefile = "DSDoanhSoNhanVien_" + DateTime.Now.Ticks + ".xlsx";
            string fileLPath = Server.MapPath(@"~/Data/ExportExcel/" + namefile);
            wb.SaveAs(fileLPath);

            return Json(new { code = 200, NameFile = namefile }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Xuất danh sách thống kê hóa đơn theo nhân viên
        /// </summary>
        /// <param name="createDate"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ExportPDFThongkeNhanVien(SearchThongKe search)
        {
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            var dao = new ThongKeDAO();

            var model = dao.GetThongKeNhanVien(search, 1, 10000000).ToList();

            HtmlToPdf htmlToPdf = new HtmlToPdf();

            //set option
            htmlToPdf.Options.PdfPageSize = PdfPageSize.A4;
            htmlToPdf.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
            htmlToPdf.Options.MarginLeft = 30;
            htmlToPdf.Options.MarginRight = 30;
            htmlToPdf.Options.MarginTop = 20;
            htmlToPdf.Options.MarginBottom = 20;
            string createDate = "";
            if (search.TuNgay.HasValue && search.DenNgay.HasValue)
            {
                createDate = $"(Từ ngày: {search.TuNgay.Value.ToString("dd/MM/yyyy")} - Đến ngày: {search.DenNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else if (search.TuNgay.HasValue)
            {
                createDate = $"(Từ ngày: {search.TuNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else if (search.DenNgay.HasValue)
            {
                createDate = $"(Đến ngày: {search.DenNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else
            {
                createDate = $"(Tất cả hóa đơn)";
            }

            string ret = RenderPartialToStringDSNgay("~/Areas/Admin/Views/Statistic/ExportPDFDSNhanVien.cshtml", model, ControllerContext, createDate, NguoiLapPhieu(session.UserID));

            PdfDocument pdf = htmlToPdf.ConvertHtmlString(ret);


            string namefile = "DSDoanhSoNhanVien_" + DateTime.Now.Ticks+ ".pdf";
            string fileLPath = Server.MapPath(@"~/Data/ExportPDF/" + namefile);
            pdf.Save(fileLPath);
            pdf.Close();

            return Json(new { code = 200, NameFile = namefile }, JsonRequestBehavior.AllowGet);

        }


        //List3
        [HttpPost]
        public JsonResult GetThongKeTheoKhachHang(SearchThongKe search, int page = 1, int pageSize = 3)
        {
            var dao = new ThongKeDAO();

            IPagedList model = (IPagedList)dao.GetThongKeKhachHang(search, page, pageSize);

            var lst = dao.GetThongKeKhachHang(search, 1, 10000000);

            int c = model.TotalItemCount;

            return Json(new { code = 200, data = model, total = c, countorder = lst.Sum(t => t.SoDonHang), countproduct = lst.Sum(t => t.SoLuong), totalprice = lst.Sum(t => t.TienHang) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetThongKeDetailTheoKhachHang(SearchThongKe search, int id, int page = 1, int pageSize = 3)
        {
            var dao = new ThongKeDAO();

            IPagedList model = (IPagedList)dao.ListOrderTheoKhachHang(search, id, page, pageSize);


            var totalprice = dao.ListOrderTheoKhachHang(search, id, 1, 10000000).ToList();

            int c = model.TotalItemCount;

            return Json(new { code = 200, data = model, total = c, totalprice = totalprice.Sum(t => t.TotalPrice), totalcout = totalprice.Sum(t => t.TotalCount) }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Export danh sách thống kê theo khách hàng
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ExportExcelListThongkeKhachhang(SearchThongKe search, int id)
        {

            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            var dao = new ThongKeDAO();

            var model = dao.ListOrderTheoKhachHang(search, id, 1, 10000000).ToList();

            var wb = new XLWorkbook();

            var ws = wb.Worksheets.Add("DoanhSoKH");
            ws.Column("A").Width = 4.18;
            ws.Column("B").Width = 14;
            ws.Column("C").Width = 16;
            ws.Column("D").Width = 16;
            ws.Column("E").Width = 15;
            ws.Column("F").Width = 10;
            ws.Column("G").Width = 17;
            ws.Column("H").Width = 20;
            ws.Column("I").Width = 20;

            ws.Cell("A1").Style.Font.Bold = true;
            ws.Cell("A1").Style.Font.FontSize = 13;
            ws.Cell("A1").Value = "CỬA HÀNG CHUỔI THỰC PHẨM TH True Milk";
            ws.Range("A1:E1").Row(1).Merge();

            ws.Cell("A2").Style.Font.Bold = true;
            ws.Cell("A2").Style.Font.FontSize = 11;
            ws.Cell("A2").Value = "SĐT:028 3602 1133 - ĐC:204/16/5 Quốc Lộ 13, P.26, Q.Bình Thạnh, Tp.HCM";
            ws.Range("A2:E2").Row(1).Merge();

            ws.Cell("A3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A3").Style.Font.Bold = true;
            ws.Cell("A3").Style.Font.FontSize = 16;
            ws.Cell("A3").Value = "THỐNG KÊ DOANH SỐ KHÁCH HÀNG: KH" + ChangeIDProduct(id);
            ws.Range("A3:I3").Row(1).Merge();

            ws.Cell("A4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A4").Style.Font.Bold = true;
            ws.Cell("A4").Style.Font.FontSize = 11;
            if (search.TuNgay.HasValue && search.DenNgay.HasValue)
            {
                ws.Cell("A4").Value = $"(Từ ngày: {search.TuNgay.Value.ToString("dd/MM/yyyy")} - Đến ngày: {search.DenNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else if (search.TuNgay.HasValue)
            {
                ws.Cell("A4").Value = $"(Từ ngày: {search.TuNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else if (search.DenNgay.HasValue)
            {
                ws.Cell("A4").Value = $"(Đến ngày: {search.DenNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else
            {
                ws.Cell("A4").Value = $"(Tất cả hóa đơn)";
            }
            ws.Range("A4:I3").Row(1).Merge();


            var rangeHeader = ws.Range("A5:I5");
            rangeHeader.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangeHeader.Style.Font.Bold = true;
            rangeHeader.Style.Fill.BackgroundColor = XLColor.Orange;
            rangeHeader.Style.Font.FontColor = XLColor.White;
            rangeHeader.Style.Font.FontSize = 12;

            ws.Cell("A5").Value = "STT";

            ws.Cell("B5").Value = "Mã hóa đơn";

            ws.Cell("C5").Value = "Mã khách hàng";
            ws.Cell("D5").Value = "Khách hàng";
            ws.Cell("E5").Value = "Ngày tạo";
            ws.Cell("F5").Value = "Số lượng";
            ws.Cell("G5").Value = "Tổng tiền";
            ws.Cell("H5").Value = "PT Thanh toán";
            ws.Cell("I5").Value = "Người xác nhận";

            int row = 6;
            double total = 0;
            int soluong = 0;
            for (int i = 0; i < model.Count; i++)
            {
                ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("A" + row).Value = model[i].STT;

                ws.Cell("B" + row).Value = "HD" + ChangeIDProduct(model[i].OrderID);

                ws.Cell("C" + row).Value = "KH" + ChangeIDProduct(model[i].CustomerID.Value);
                ws.Cell("D" + row).Value = model[i].ShipName;
                ws.Cell("E" + row).Value = model[i].CreateDate;
                ws.Cell("F" + row).Value = model[i].TotalCount;
                ws.Cell("G" + row).Style.NumberFormat.Format = "#,##0";
                ws.Cell("G" + row).Value = (model[i].TotalPrice);
                ws.Cell("H" + row).Value = model[i].PaymentName;
                ws.Cell("I" + row).Value = model[i].UserName;
                total += (double)(model[i].TotalPrice);
                soluong += model[i].TotalCount.Value;
                row++;
            }

            var rangeborder = ws.Range("A5:I" + (row));
            rangeborder.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rangeborder.Style.Border.InsideBorder = XLBorderStyleValues.Thin;


            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Value = "Tổng cộng";
            ws.Range($"B{row}:D{row}").Row(1).Merge();

            ws.Cell("F" + row).Style.Font.Bold = true;
            ws.Cell("F" + row).Value = soluong;

            ws.Cell("G" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("G" + row).Style.NumberFormat.Format = "#,##0";
            ws.Cell("G" + row).Style.Font.Bold = true;
            ws.Cell("G" + row).Value = total.ToString("N0") + "VNĐ";

            //tiền chữ
            string value = "A" + (row + 1);
            string valuerange = value + ":I" + (row + 1);
            ws.Cell(value).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell(value).Style.Font.Bold = true;
            ws.Cell(value).Style.Font.FontSize = 11;
            ws.Cell(value).Value = "Bằng chữ: " + new ConvertCurrency().ToUpper(total);
            ws.Range(valuerange).Row(1).Merge();

            //Ký tên
            ws.Cell("F" + (row + 2)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F" + (row + 2)).Style.Font.Italic = true;
            ws.Cell("F" + (row + 2)).Value = $"TP.HCM, ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}";
            ws.Range($"F{row + 2}:H{row + 2}").Row(1).Merge();


            ws.Cell("F" + (row + 3)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F" + (row + 3)).Style.Font.Bold = true;
            ws.Cell("F" + (row + 3)).Value = "Người lập phiếu";
            ws.Range($"F{row + 3}:H{row + 3}").Row(1).Merge();

            ws.Cell("F" + (row + 4)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F" + (row + 4)).Style.Font.Italic = true;
            ws.Cell("F" + (row + 4)).Value = "(Ký, họ tên, đóng dấu)";
            ws.Range($"F{row + 4}:H{row + 4}").Row(1).Merge();

            ws.Cell("F" + (row + 6)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F" + (row + 6)).Style.Font.Bold = true;
            ws.Cell("F" + (row + 6)).Value = NguoiLapPhieu(session.UserID);
            ws.Range($"F{row + 6}:H{row + 6}").Row(1).Merge();


            string namefile = "DoanhSoKH_KH" + ChangeIDProduct(id) + " " + DateTime.Now.Ticks + ".xlsx";
            string fileLPath = Server.MapPath(@"~/Data/ExportExcel/" + namefile);
            wb.SaveAs(fileLPath);

            return Json(new { code = 200, NameFile = namefile }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Xuất doanh số pdf hóa đơn theo khách hàng
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ExportPDFListThongkeKhachhang(SearchThongKe search, int id)
        {

            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            var dao = new ThongKeDAO();

            var model = dao.ListOrderTheoKhachHang(search, id, 1, 10000000).ToList();

            HtmlToPdf htmlToPdf = new HtmlToPdf();

            //set option
            htmlToPdf.Options.PdfPageSize = PdfPageSize.A4;
            htmlToPdf.Options.PdfPageOrientation = PdfPageOrientation.Landscape;
            htmlToPdf.Options.MarginLeft = 30;
            htmlToPdf.Options.MarginRight = 30;
            htmlToPdf.Options.MarginTop = 20;
            htmlToPdf.Options.MarginBottom = 20;
            string ngay = "";
            if (search.TuNgay.HasValue && search.DenNgay.HasValue)
            {
                ngay = $"(Từ ngày: {search.TuNgay.Value.ToString("dd/MM/yyyy")} - Đến ngày: {search.DenNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else if (search.TuNgay.HasValue)
            {
                ngay = $"(Từ ngày: {search.TuNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else if (search.DenNgay.HasValue)
            {
                ngay = $"(Đến ngày: {search.DenNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else
            {
                ngay = $"(Tất cả hóa đơn)";
            }

            string ret = RenderPartialToStringNhanVien("~/Areas/Admin/Views/Statistic/ExportPDFKhachHang.cshtml", model, ControllerContext, ngay, "KH"+ChangeIDProduct(id), NguoiLapPhieu(session.UserID));

            PdfDocument pdf = htmlToPdf.ConvertHtmlString(ret);


            string namefile = "DoanhSoKH_KH" + ChangeIDProduct(id) + " " + DateTime.Now.Ticks + ".pdf";
            string fileLPath = Server.MapPath(@"~/Data/ExportPDF/" + namefile);
            pdf.Save(fileLPath);
            pdf.Close();

            return Json(new { code = 200, NameFile = namefile }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Xuất danh sách thống kê hóa đơn theo khách hàng
        /// </summary>
        /// <param name="createDate"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ExportExcelThongkeKhachhang(SearchThongKe search)
        {
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            var dao = new ThongKeDAO();

            var model = dao.GetThongKeKhachHang(search, 1, 10000000).ToList();

            var wb = new XLWorkbook();

            var ws = wb.Worksheets.Add("DanhSachDoanhSoKH");
            ws.Column("A").Width = 4.18;
            ws.Column("B").Width = 15;
            ws.Column("C").Width = 18;
            ws.Column("D").Width = 12;
            ws.Column("E").Width = 9;
            ws.Column("F").Width = 19;

            ws.Cell("A1").Style.Font.Bold = true;
            ws.Cell("A1").Style.Font.FontSize = 13;
            ws.Cell("A1").Value = "CỬA HÀNG CHUỔI THỰC PHẨM TH True Milk";
            ws.Range("A1:E1").Row(1).Merge();

            ws.Cell("A2").Style.Font.Bold = true;
            ws.Cell("A2").Style.Font.FontSize = 11;
            ws.Cell("A2").Value = "SĐT:028 3602 1133 - ĐC:204/16/5 Quốc Lộ 13, P.26, Q.Bình Thạnh, Tp.HCM";
            ws.Range("A2:E2").Row(1).Merge();

            ws.Cell("A3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A3").Style.Font.Bold = true;
            ws.Cell("A3").Style.Font.FontSize = 16;
            ws.Cell("A3").Value = "THỐNG KÊ DOANH SỐ BÁN KHÁCH HÀNG";
            ws.Range("A3:F3").Row(1).Merge();

            ws.Cell("A4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A4").Style.Font.Bold = true;
            ws.Cell("A4").Style.Font.FontSize = 11;
            if (search.TuNgay.HasValue && search.DenNgay.HasValue)
            {
                ws.Cell("A4").Value = $"(Từ ngày: {search.TuNgay.Value.ToString("dd/MM/yyyy")} - Đến ngày: {search.DenNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else if (search.TuNgay.HasValue)
            {
                ws.Cell("A4").Value = $"(Từ ngày: {search.TuNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else if (search.DenNgay.HasValue)
            {
                ws.Cell("A4").Value = $"(Đến ngày: {search.DenNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else
            {
                ws.Cell("A4").Value = $"(Tất cả hóa đơn)";
            }
            ws.Range("A4:F3").Row(1).Merge();


            var rangeHeader = ws.Range("A5:F5");
            rangeHeader.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangeHeader.Style.Font.Bold = true;
            rangeHeader.Style.Fill.BackgroundColor = XLColor.Orange;
            rangeHeader.Style.Font.FontColor = XLColor.White;
            rangeHeader.Style.Font.FontSize = 12;

            ws.Cell("A5").Value = "STT";
            ws.Cell("B5").Value = "Mã khách hàng";
            ws.Cell("C5").Value = "Khách hàng";
            ws.Cell("D5").Value = "Số đơn hàng";
            ws.Cell("E5").Value = "Số lượng";
            ws.Cell("F5").Value = "Tiền hàng";

            int row = 6;
            double total = 0;
            int soluong = 0;
            int sodonhang = 0;
            for (int i = 0; i < model.Count; i++)
            {
                ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("A" + row).Value = model[i].STT;

                ws.Cell("B" + row).Value ="KH"+ChangeIDProduct(model[i].CustomerID);

                ws.Cell("C" + row).Value = model[i].CustomerName;
                ws.Cell("D" + row).Value = model[i].SoDonHang;               
                ws.Cell("E" + row).Value = model[i].SoLuong;
                ws.Cell("F" + row).Style.NumberFormat.Format = "#,##0";
                ws.Cell("F" + row).Value = model[i].TienHang;
                total += (double)(model[i].TienHang);
                soluong += model[i].SoLuong;
                sodonhang += model[i].SoDonHang;
                row++;
            }

            var rangeborder = ws.Range("A5:F" + (row));
            rangeborder.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rangeborder.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Value = "Tổng cộng";

            ws.Cell("D" + row).Style.Font.Bold = true;
            ws.Cell("D" + row).Value = sodonhang;

            ws.Cell("E" + row).Style.Font.Bold = true;
            ws.Cell("E" + row).Value = soluong;

            ws.Cell("F" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("F" + row).Style.NumberFormat.Format = "#,##0";
            ws.Cell("F" + row).Style.Font.Bold = true;
            ws.Cell("F" + row).Value = total.ToString("N0") + "VNĐ";

            //tiền chữ
            string value = "A" + (row + 1);
            string valuerange = value + ":F" + (row + 1);
            ws.Cell(value).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell(value).Style.Font.Bold = true;
            ws.Cell(value).Style.Font.FontSize = 11;
            ws.Cell(value).Value = "Bằng chữ: " + new ConvertCurrency().ToUpper(total);
            ws.Range(valuerange).Row(1).Merge();

            //Ký tên
            ws.Cell("D" + (row + 2)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + (row + 2)).Style.Font.Italic = true;
            ws.Cell("D" + (row + 2)).Value = $"TP.HCM, ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}";
            ws.Range($"D{row + 2}:F{row + 2}").Row(1).Merge();


            ws.Cell("D" + (row + 3)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + (row + 3)).Style.Font.Bold = true;
            ws.Cell("D" + (row + 3)).Value = "Người lập phiếu";
            ws.Range($"D{row + 3}:F{row + 3}").Row(1).Merge();

            ws.Cell("D" + (row + 4)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + (row + 4)).Style.Font.Italic = true;
            ws.Cell("D" + (row + 4)).Value = "(Ký, họ tên, đóng dấu)";
            ws.Range($"D{row + 4}:F{row + 4}").Row(1).Merge();

            ws.Cell("D" + (row + 6)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + (row + 6)).Style.Font.Bold = true;
            ws.Cell("D" + (row + 6)).Value = NguoiLapPhieu(session.UserID);
            ws.Range($"D{row + 6}:F{row + 6}").Row(1).Merge();


            string namefile = "DSDoanhSoNhanVien_" + DateTime.Now.Ticks + ".xlsx";
            string fileLPath = Server.MapPath(@"~/Data/ExportExcel/" + namefile);
            wb.SaveAs(fileLPath);

            return Json(new { code = 200, NameFile = namefile }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Xuất danh sách thống kê hóa đơn theo khách hàng
        /// </summary>
        /// <param name="createDate"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ExportPDFThongkeKhachhang(SearchThongKe search)
        {
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            var dao = new ThongKeDAO();

            var model = dao.GetThongKeKhachHang(search, 1, 10000000).ToList();

            HtmlToPdf htmlToPdf = new HtmlToPdf();

            //set option
            htmlToPdf.Options.PdfPageSize = PdfPageSize.A4;
            htmlToPdf.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
            htmlToPdf.Options.MarginLeft = 30;
            htmlToPdf.Options.MarginRight = 30;
            htmlToPdf.Options.MarginTop = 20;
            htmlToPdf.Options.MarginBottom = 20;
            string createDate = "";
            if (search.TuNgay.HasValue && search.DenNgay.HasValue)
            {
                createDate = $"(Từ ngày: {search.TuNgay.Value.ToString("dd/MM/yyyy")} - Đến ngày: {search.DenNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else if (search.TuNgay.HasValue)
            {
                createDate = $"(Từ ngày: {search.TuNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else if (search.DenNgay.HasValue)
            {
                createDate = $"(Đến ngày: {search.DenNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else
            {
                createDate = $"(Tất cả hóa đơn)";
            }

            string ret = RenderPartialToStringDSNgay("~/Areas/Admin/Views/Statistic/ExportPDFDSKhachHang.cshtml", model, ControllerContext, createDate, NguoiLapPhieu(session.UserID));

            PdfDocument pdf = htmlToPdf.ConvertHtmlString(ret);


            string namefile = "DSDoanhSoKH_" + DateTime.Now.Ticks + ".pdf";
            string fileLPath = Server.MapPath(@"~/Data/ExportPDF/" + namefile);
            pdf.Save(fileLPath);
            pdf.Close();

            return Json(new { code = 200, NameFile = namefile }, JsonRequestBehavior.AllowGet);

        }


        //List4
        [HttpPost]
        public JsonResult GetThongKeTheoSanPham(SearchThongKe search, int page = 1, int pageSize = 3)
        {
            var dao = new ThongKeDAO();

            IPagedList model = (IPagedList)dao.GetThongKeSanPham(search, page, pageSize);

            var lst = dao.GetThongKeSanPham(search, 1, 10000000);

            int c = model.TotalItemCount;

            return Json(new { code = 200, data = model, total = c, countorder = lst.Sum(t => t.SoDonHang), countproduct = lst.Sum(t => t.SoLuong), totalprice = lst.Sum(t => t.TienHang) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetThongKeDetailTheoSanPham(SearchThongKe search, int id, int page = 1, int pageSize = 3)
        {
            var dao = new ThongKeDAO();

            IPagedList model = (IPagedList)dao.ListOrderTheoSanPham(search, id, page, pageSize);


            var totalprice = dao.ListOrderTheoSanPham(search, id, 1, 10000000).ToList();

            int c = model.TotalItemCount;

            return Json(new { code = 200, data = model, total = c, totalprice = totalprice.Sum(t => t.ThanhTien), totalcout = totalprice.Sum(t => t.SoLuong) }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Export danh sách thống kê theo sản phẩm
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ExportExcelListThongkeSanPham(SearchThongKe search, int id)
        {
            DbWebsite db = new DbWebsite();
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            var dao = new ThongKeDAO();

            var model = dao.ListOrderTheoSanPham(search, id, 1, 10000000).ToList();

            var wb = new XLWorkbook();

            var ws = wb.Worksheets.Add("DoanhSoNV");
            ws.Column("A").Width = 4.18;
            ws.Column("B").Width = 14;
            ws.Column("C").Width = 15;
            ws.Column("D").Width = 13;
            ws.Column("E").Width = 50;
            ws.Column("F").Width = 8;
            ws.Column("G").Width = 9;
            ws.Column("H").Width = 10;
            ws.Column("I").Width = 14;
            ws.Column("J").Width = 20;

            ws.Cell("A1").Style.Font.Bold = true;
            ws.Cell("A1").Style.Font.FontSize = 13;
            ws.Cell("A1").Value = "CỬA HÀNG CHUỔI THỰC PHẨM TH True Milk";
            ws.Range("A1:E1").Row(1).Merge();

            ws.Cell("A2").Style.Font.Bold = true;
            ws.Cell("A2").Style.Font.FontSize = 11;
            ws.Cell("A2").Value = "SĐT:028 3602 1133 - ĐC:204/16/5 Quốc Lộ 13, P.26, Q.Bình Thạnh, Tp.HCM";
            ws.Range("A2:E2").Row(1).Merge();

            ws.Cell("A3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A3").Style.Font.Bold = true;
            ws.Cell("A3").Style.Font.FontSize = 16;
            ws.Cell("A3").Value = "THỐNG KÊ DOANH SỐ HÀNG HÓA CHI TIẾT";
            ws.Range("A3:J3").Row(1).Merge();

            ws.Cell("A4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A4").Style.Font.Bold = true;
            ws.Cell("A4").Style.Font.FontSize = 12;
            ws.Cell("A4").Value = db.Products.Find(id).Name;
            ws.Range("A4:J4").Row(1).Merge();


            ws.Cell("A5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A5").Style.Font.Bold = true;
            ws.Cell("A5").Style.Font.FontSize = 11;
            if (search.TuNgay.HasValue && search.DenNgay.HasValue)
            {
                ws.Cell("A5").Value = $"(Từ ngày: {search.TuNgay.Value.ToString("dd/MM/yyyy")} - Đến ngày: {search.DenNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else if (search.TuNgay.HasValue)
            {
                ws.Cell("A5").Value = $"(Từ ngày: {search.TuNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else if (search.DenNgay.HasValue)
            {
                ws.Cell("A5").Value = $"(Đến ngày: {search.DenNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else
            {
                ws.Cell("A5").Value = $"(Tất cả hóa đơn)";
            }
            ws.Range("A5:J5").Row(1).Merge();


            var rangeHeader = ws.Range("A6:J6");
            rangeHeader.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangeHeader.Style.Font.Bold = true;
            rangeHeader.Style.Fill.BackgroundColor = XLColor.Orange;
            rangeHeader.Style.Font.FontColor = XLColor.White;
            rangeHeader.Style.Font.FontSize = 12;

            ws.Cell("A6").Value = "STT";

            ws.Cell("B6").Value = "Mã hóa đơn";

            ws.Cell("C6").Value = "Ngày bán";
            ws.Cell("D6").Value = "Mã sản phẩm";
            ws.Cell("E6").Value = "Tên sản phẩm";
            ws.Cell("F6").Value = "ĐVT";
            ws.Cell("G6").Value = "SL";
            ws.Cell("H6").Value = "Đơn giá";
            ws.Cell("I6").Value = "Thành tiền";
            ws.Cell("J6").Value = "Người bán";

            int row = 7;
            double total = 0;
            int soluong = 0;
            for (int i = 0; i < model.Count; i++)
            {
                ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("A" + row).Value = model[i].STT;

                ws.Cell("B" + row).Value = "HD" + ChangeIDProduct(model[i].OrderID);

                ws.Cell("C" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("C" + row).Value = model[i].CreateDate;

                ws.Cell("D" + row).Value = model[i].ProductIDDisplay;
                ws.Cell("E" + row).Value = model[i].ProductName;
                ws.Cell("F" + row).Value = model[i].DVT;             
                ws.Cell("G" + row).Value = (model[i].SoLuong);

                ws.Cell("H" + row).Style.NumberFormat.Format = "#,##0";
                ws.Cell("H" + row).Value = model[i].DonGia;

                ws.Cell("I" + row).Style.NumberFormat.Format = "#,##0";
                ws.Cell("I" + row).Value = model[i].ThanhTien;

                ws.Cell("J" + row).Value = model[i].NguoiBan;

                total += (double)(model[i].ThanhTien);
                soluong += model[i].SoLuong;
                row++;
            }

            var rangeborder = ws.Range("A6:J" + (row));
            rangeborder.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rangeborder.Style.Border.InsideBorder = XLBorderStyleValues.Thin;


            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Value = "Tổng cộng";
            ws.Range($"B{row}:D{row}").Row(1).Merge();

            ws.Cell("G" + row).Style.Font.Bold = true;
            ws.Cell("G" + row).Value = soluong;

            ws.Cell("I" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("I" + row).Style.NumberFormat.Format = "#,##0";
            ws.Cell("I" + row).Style.Font.Bold = true;
            ws.Cell("I" + row).Value = total.ToString("N0") + "VNĐ";

            //tiền chữ
            string value = "A" + (row + 1);
            string valuerange = value + ":J" + (row + 1);
            ws.Cell(value).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell(value).Style.Font.Bold = true;
            ws.Cell(value).Style.Font.FontSize = 11;
            ws.Cell(value).Value = "Bằng chữ: " + new ConvertCurrency().ToUpper(total);
            ws.Range(valuerange).Row(1).Merge();

            //Ký tên
            ws.Cell("I" + (row + 2)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("I" + (row + 2)).Style.Font.Italic = true;
            ws.Cell("I" + (row + 2)).Value = $"TP.HCM, ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}";
            ws.Range($"I{row + 2}:J{row + 2}").Row(1).Merge();


            ws.Cell("I" + (row + 3)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("I" + (row + 3)).Style.Font.Bold = true;
            ws.Cell("I" + (row + 3)).Value = "Người lập phiếu";
            ws.Range($"I{row + 3}:J{row + 3}").Row(1).Merge();

            ws.Cell("I" + (row + 4)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("I" + (row + 4)).Style.Font.Italic = true;
            ws.Cell("I" + (row + 4)).Value = "(Ký, họ tên, đóng dấu)";
            ws.Range($"I{row + 4}:J{row + 4}").Row(1).Merge();

            ws.Cell("I" + (row + 6)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("I" + (row + 6)).Style.Font.Bold = true;
            ws.Cell("I" + (row + 6)).Value = NguoiLapPhieu(session.UserID);
            ws.Range($"I{row + 6}:J{row + 6}").Row(1).Merge();


            string namefile = "DoanhSoSP" + DateTime.Now.Ticks + ".xlsx";
            string fileLPath = Server.MapPath(@"~/Data/ExportExcel/" + namefile);
            wb.SaveAs(fileLPath);

            return Json(new { code = 200, NameFile = namefile }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Xuất doanh số pdf hóa đơn theo sản phẩm
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ExportPDFListThongkeSanPham(SearchThongKe search, int id)
        {

            DbWebsite db = new DbWebsite();

            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            var dao = new ThongKeDAO();

            var model = dao.ListOrderTheoSanPham(search, id, 1, 10000000).ToList();

            HtmlToPdf htmlToPdf = new HtmlToPdf();

            //set option
            htmlToPdf.Options.PdfPageSize = PdfPageSize.A4;
            htmlToPdf.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
            htmlToPdf.Options.MarginLeft = 30;
            htmlToPdf.Options.MarginRight = 30;
            htmlToPdf.Options.MarginTop = 20;
            htmlToPdf.Options.MarginBottom = 20;
            string ngay = "";
            if (search.TuNgay.HasValue && search.DenNgay.HasValue)
            {
                ngay = $"(Từ ngày: {search.TuNgay.Value.ToString("dd/MM/yyyy")} - Đến ngày: {search.DenNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else if (search.TuNgay.HasValue)
            {
                ngay = $"(Từ ngày: {search.TuNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else if (search.DenNgay.HasValue)
            {
                ngay = $"(Đến ngày: {search.DenNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else
            {
                ngay = $"(Tất cả hóa đơn)";
            }

            string ret = RenderPartialToStringNhanVien("~/Areas/Admin/Views/Statistic/ExportPDFSanPham.cshtml", model, ControllerContext, ngay, db.Products.Find(id).Name, NguoiLapPhieu(session.UserID));

            PdfDocument pdf = htmlToPdf.ConvertHtmlString(ret);


            string namefile = "DoanhSoSP" + DateTime.Now.Ticks + ".pdf";
            string fileLPath = Server.MapPath(@"~/Data/ExportPDF/" + namefile);
            pdf.Save(fileLPath);
            pdf.Close();

            return Json(new { code = 200, NameFile = namefile }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Xuất danh sách thống kê hóa đơn theo sản phẩm
        /// </summary>
        /// <param name="createDate"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ExportExcelThongkeSanPham(SearchThongKe search)
        {
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            var dao = new ThongKeDAO();

            var model = dao.GetThongKeSanPham(search, 1, 10000000).ToList();

            var wb = new XLWorkbook();

            var ws = wb.Worksheets.Add("DanhSachDoanhSoSP");
            ws.Column("A").Width = 4.18;
            ws.Column("B").Width = 15;
            ws.Column("C").Width = 60;
            ws.Column("D").Width = 12;
            ws.Column("E").Width = 9;
            ws.Column("F").Width = 19;

            ws.Cell("A1").Style.Font.Bold = true;
            ws.Cell("A1").Style.Font.FontSize = 13;
            ws.Cell("A1").Value = "CỬA HÀNG CHUỔI THỰC PHẨM TH True Milk";
            ws.Range("A1:E1").Row(1).Merge();

            ws.Cell("A2").Style.Font.Bold = true;
            ws.Cell("A2").Style.Font.FontSize = 11;
            ws.Cell("A2").Value = "SĐT:028 3602 1133 - ĐC:204/16/5 Quốc Lộ 13, P.26, Q.Bình Thạnh, Tp.HCM";
            ws.Range("A2:E2").Row(1).Merge();

            ws.Cell("A3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A3").Style.Font.Bold = true;
            ws.Cell("A3").Style.Font.FontSize = 16;
            ws.Cell("A3").Value = "THỐNG KÊ DOANH SỐ BÁN SẢN PHẨM";
            ws.Range("A3:F3").Row(1).Merge();

            ws.Cell("A4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A4").Style.Font.Bold = true;
            ws.Cell("A4").Style.Font.FontSize = 11;
            if (search.TuNgay.HasValue && search.DenNgay.HasValue)
            {
                ws.Cell("A4").Value = $"(Từ ngày: {search.TuNgay.Value.ToString("dd/MM/yyyy")} - Đến ngày: {search.DenNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else if (search.TuNgay.HasValue)
            {
                ws.Cell("A4").Value = $"(Từ ngày: {search.TuNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else if (search.DenNgay.HasValue)
            {
                ws.Cell("A4").Value = $"(Đến ngày: {search.DenNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else
            {
                ws.Cell("A4").Value = $"(Tất cả hóa đơn)";
            }
            ws.Range("A4:F3").Row(1).Merge();


            var rangeHeader = ws.Range("A5:F5");
            rangeHeader.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangeHeader.Style.Font.Bold = true;
            rangeHeader.Style.Fill.BackgroundColor = XLColor.Orange;
            rangeHeader.Style.Font.FontColor = XLColor.White;
            rangeHeader.Style.Font.FontSize = 12;

            ws.Cell("A5").Value = "STT";
            ws.Cell("B5").Value = "Mã Sản phẩm";
            ws.Cell("C5").Value = "Tên sản phẩm";
            ws.Cell("D5").Value = "ĐVT";
            ws.Cell("E5").Value = "SL bán";
            ws.Cell("F5").Value = "Tiền hàng";

            int row = 6;
            double total = 0;
            int soluong = 0;
            for (int i = 0; i < model.Count; i++)
            {
                ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("A" + row).Value = model[i].STT;

                ws.Cell("B" + row).Value = model[i].ProductIDDisplay;

                ws.Cell("C" + row).Value = model[i].ProductName;
                ws.Cell("D" + row).Value = model[i].DVT;
                ws.Cell("E" + row).Value = model[i].SoLuong;
                ws.Cell("F" + row).Style.NumberFormat.Format = "#,##0";
                ws.Cell("F" + row).Value = model[i].TienHang;
                total += (double)(model[i].TienHang);
                soluong += model[i].SoLuong;
                row++;
            }

            var rangeborder = ws.Range("A5:F" + (row));
            rangeborder.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rangeborder.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Value = "Tổng cộng";


            ws.Cell("E" + row).Style.Font.Bold = true;
            ws.Cell("E" + row).Value = soluong;

            ws.Cell("F" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("F" + row).Style.NumberFormat.Format = "#,##0";
            ws.Cell("F" + row).Style.Font.Bold = true;
            ws.Cell("F" + row).Value = total.ToString("N0") + "VNĐ";

            //tiền chữ
            string value = "A" + (row + 1);
            string valuerange = value + ":F" + (row + 1);
            ws.Cell(value).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell(value).Style.Font.Bold = true;
            ws.Cell(value).Style.Font.FontSize = 11;
            ws.Cell(value).Value = "Bằng chữ: " + new ConvertCurrency().ToUpper(total);
            ws.Range(valuerange).Row(1).Merge();

            //Ký tên
            ws.Cell("D" + (row + 2)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + (row + 2)).Style.Font.Italic = true;
            ws.Cell("D" + (row + 2)).Value = $"TP.HCM, ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}";
            ws.Range($"D{row + 2}:F{row + 2}").Row(1).Merge();


            ws.Cell("D" + (row + 3)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + (row + 3)).Style.Font.Bold = true;
            ws.Cell("D" + (row + 3)).Value = "Người lập phiếu";
            ws.Range($"D{row + 3}:F{row + 3}").Row(1).Merge();

            ws.Cell("D" + (row + 4)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + (row + 4)).Style.Font.Italic = true;
            ws.Cell("D" + (row + 4)).Value = "(Ký, họ tên, đóng dấu)";
            ws.Range($"D{row + 4}:F{row + 4}").Row(1).Merge();

            ws.Cell("D" + (row + 6)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + (row + 6)).Style.Font.Bold = true;
            ws.Cell("D" + (row + 6)).Value = NguoiLapPhieu(session.UserID);
            ws.Range($"D{row + 6}:F{row + 6}").Row(1).Merge();


            string namefile = "DSDoanhSoSP_" + DateTime.Now.Ticks + ".xlsx";
            string fileLPath = Server.MapPath(@"~/Data/ExportExcel/" + namefile);
            wb.SaveAs(fileLPath);

            return Json(new { code = 200, NameFile = namefile }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Xuất danh sách thống kê hóa đơn theo sản phẩm
        /// </summary>
        /// <param name="createDate"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ExportPDFThongkeSanPham(SearchThongKe search)
        {
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            var dao = new ThongKeDAO();

            var model = dao.GetThongKeSanPham(search, 1, 10000000).ToList();

            HtmlToPdf htmlToPdf = new HtmlToPdf();

            //set option
            htmlToPdf.Options.PdfPageSize = PdfPageSize.A4;
            htmlToPdf.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
            htmlToPdf.Options.MarginLeft = 30;
            htmlToPdf.Options.MarginRight = 30;
            htmlToPdf.Options.MarginTop = 20;
            htmlToPdf.Options.MarginBottom = 20;
            string createDate = "";
            if (search.TuNgay.HasValue && search.DenNgay.HasValue)
            {
                createDate = $"(Từ ngày: {search.TuNgay.Value.ToString("dd/MM/yyyy")} - Đến ngày: {search.DenNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else if (search.TuNgay.HasValue)
            {
                createDate = $"(Từ ngày: {search.TuNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else if (search.DenNgay.HasValue)
            {
                createDate = $"(Đến ngày: {search.DenNgay.Value.ToString("dd/MM/yyyy")})";
            }
            else
            {
                createDate = $"(Tất cả hóa đơn)";
            }

            string ret = RenderPartialToStringDSNgay("~/Areas/Admin/Views/Statistic/ExportPDFDSSanPham.cshtml", model, ControllerContext, createDate, NguoiLapPhieu(session.UserID));

            PdfDocument pdf = htmlToPdf.ConvertHtmlString(ret);


            string namefile = "DSDoanhSoSP_" + DateTime.Now.Ticks + ".pdf";
            string fileLPath = Server.MapPath(@"~/Data/ExportPDF/" + namefile);
            pdf.Save(fileLPath);
            pdf.Close();

            return Json(new { code = 200, NameFile = namefile }, JsonRequestBehavior.AllowGet);

        }

    }
}