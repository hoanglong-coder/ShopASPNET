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
    //Lợi nhuận
    public class StatisticLNController : RoleBaoCaoLoiNhuanController
    {
        // GET: Admin/StatisticLN
        public ActionResult Index()
        {

            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            ViewBag.UserName = session.FullName;

            ViewBag.Role = session.RoleString;

            return View();
        }

        //List1
        [HttpPost]
        public JsonResult GetThongKeTheoNgay(SearchThongKe search, int page = 1, int pageSize = 3)
        {
            var dao = new ThongKeDAO();

            IPagedList model = (IPagedList)dao.GetThongKeLoiNhuanThang(search, page, pageSize);

            int c = model.TotalItemCount;

            var lst = dao.GetThongKeLoiNhuanThang(search, 1, 10000000);

            return Json(new { code = 200, data = model, total = c, tienhang =lst.Sum(t=>t.TienHang), doanthu = lst.Sum(t=>t.DoanhThu), von = lst.Sum(t=>t.Von), lai = lst.Sum(t=>t.Lai)}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetThongKeDetailTheoNgay(DateTime createDate, int page = 1, int pageSize = 3)
        {
            var dao = new ThongKeDAO();

            SearchThongKe search = new SearchThongKe();

        IPagedList model = (IPagedList)dao.ListOrderLoiNhuanTheoNgay(search, createDate, page, pageSize);

            var lst = dao.ListOrderLoiNhuanTheoNgay(search, createDate, 1, 10000000).ToList();

            int c = model.TotalItemCount;

            return Json(new { code = 200, data = model, total = c, tienhang = lst.Sum(t => t.TienHang), doanthu = lst.Sum(t => t.DoanhThu), von = lst.Sum(t => t.Von), lai = lst.Sum(t => t.Lai) }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Xuất doanh số hóa đơn lợi nhuận theo ngày
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ExportExcelListLoiNhuanNgay(DateTime createDate)
        {
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            var dao = new ThongKeDAO();

            SearchThongKe search = new SearchThongKe();

            var model = dao.ListOrderLoiNhuanTheoNgay(search, createDate, 1, 10000000).ToList();

            var wb = new XLWorkbook();

            var ws = wb.Worksheets.Add("LoiNhuanNgay");
            ws.Column("A").Width = 4.18;
            ws.Column("B").Width = 14;
            ws.Column("C").Width = 16;
            ws.Column("D").Width = 16;
            ws.Column("E").Width = 15;
            ws.Column("F").Width = 15;
            ws.Column("G").Width = 17;
            ws.Column("H").Width = 20;

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
            ws.Cell("A3").Value = "THỐNG KÊ LỢI NHUẬN";
            ws.Range("A3:H3").Row(1).Merge();

            ws.Cell("A4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A4").Style.Font.Bold = true;
            ws.Cell("A4").Style.Font.FontSize = 11;
            ws.Cell("A4").Value = $"(Ngày: {createDate.ToString("dd/MM/yyyy")})";
            ws.Range("A4:H3").Row(1).Merge();


            var rangeHeader = ws.Range("A5:H5");
            rangeHeader.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangeHeader.Style.Font.Bold = true;
            rangeHeader.Style.Fill.BackgroundColor = XLColor.Orange;
            rangeHeader.Style.Font.FontColor = XLColor.White;
            rangeHeader.Style.Font.FontSize = 12;

            ws.Cell("A5").Value = "STT";

            ws.Cell("B5").Value = "Mã hóa đơn";

            ws.Cell("C5").Value = "Ngày tạo";
            ws.Cell("D5").Value = "Khách hàng";
            ws.Cell("E5").Value = "Tiền hàng";
            ws.Cell("F5").Value = "Doanh thu";
            ws.Cell("G5").Value = "Vốn";
            ws.Cell("H5").Value = "Lãi gộp";

            int row = 6;
            double tienhang = 0;
            double doanhthu = 0;
            double von = 0;
            double laigop = 0;
            for (int i = 0; i < model.Count; i++)
            {
                ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("A" + row).Value = model[i].STT;

                ws.Cell("B" + row).Value = "HD" + ChangeIDProduct(model[i].OrderID);

                ws.Cell("C" + row).Value = model[i].CreateDate;
                ws.Cell("D" + row).Value = model[i].CustomerName;

                ws.Cell("E" + row).Style.NumberFormat.Format = "#,##0";
                ws.Cell("E" + row).Value = model[i].TienHang;

                ws.Cell("F" + row).Style.NumberFormat.Format = "#,##0";
                ws.Cell("F" + row).Value = model[i].DoanhThu;

                ws.Cell("G" + row).Style.NumberFormat.Format = "#,##0";
                ws.Cell("G" + row).Value = (model[i].Von);

                ws.Cell("H" + row).Style.NumberFormat.Format = "#,##0";
                ws.Cell("H" + row).Value = model[i].Lai;

                tienhang += (double)(model[i].TienHang);
                doanhthu += (double)(model[i].DoanhThu);
                von += (double)(model[i].Von);
                laigop += (double)(model[i].Lai);
                row++;
            }

            var rangeborder = ws.Range("A5:H" + (row));
            rangeborder.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rangeborder.Style.Border.InsideBorder = XLBorderStyleValues.Thin;


            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Value = "Tổng cộng";
            ws.Range($"B{row}:D{row}").Row(1).Merge();

            ws.Cell("E" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("E" + row).Style.NumberFormat.Format = "#,##0";
            ws.Cell("E" + row).Style.Font.Bold = true;
            ws.Cell("E" + row).Value = tienhang.ToString("N0") + "VNĐ";

            ws.Cell("F" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("F" + row).Style.NumberFormat.Format = "#,##0";
            ws.Cell("F" + row).Style.Font.Bold = true;
            ws.Cell("F" + row).Value = doanhthu.ToString("N0") + "VNĐ";

            ws.Cell("G" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("G" + row).Style.NumberFormat.Format = "#,##0";
            ws.Cell("G" + row).Style.Font.Bold = true;
            ws.Cell("G" + row).Value = von.ToString("N0") + "VNĐ";

            ws.Cell("H" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("H" + row).Style.NumberFormat.Format = "#,##0";
            ws.Cell("H" + row).Style.Font.Bold = true;
            ws.Cell("H" + row).Value = laigop.ToString("N0") + "VNĐ";

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


            string namefile = "LoiNhuanNgay_" + DateTime.Now.Date.ToString("dd_MM_yyyy") + " " + DateTime.Now.Ticks + ".xlsx";
            string fileLPath = Server.MapPath(@"~/Data/ExportExcel/" + namefile);
            wb.SaveAs(fileLPath);

            return Json(new { code = 200, NameFile = namefile }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Xuất doanh số pdf hóa đơn lợi nhuận theo ngày
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ExportPDFListLoiNhuanNgay(DateTime createDate)
        {
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            var dao = new ThongKeDAO();

            SearchThongKe search = new SearchThongKe();

            var model = dao.ListOrderLoiNhuanTheoNgay(search, createDate, 1, 10000000).ToList();

            HtmlToPdf htmlToPdf = new HtmlToPdf();

            //set option
            htmlToPdf.Options.PdfPageSize = PdfPageSize.A4;
            htmlToPdf.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
            htmlToPdf.Options.MarginLeft = 30;
            htmlToPdf.Options.MarginRight = 30;
            htmlToPdf.Options.MarginTop = 20;
            htmlToPdf.Options.MarginBottom = 20;


            string ret = RenderPartialToStringNgay("~/Areas/Admin/Views/StatisticLN/ExportPDFNgay.cshtml", model, ControllerContext, createDate, NguoiLapPhieu(session.UserID));

            PdfDocument pdf = htmlToPdf.ConvertHtmlString(ret);

            string namefile = "LoiNhuanNgay_" + DateTime.Now.Ticks + ".pdf";
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
        /// Xuất danh sách thống kê lợi nhuận hóa đơn ngày
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

            var model = dao.GetThongKeLoiNhuanThang(search, 1, 10000000).ToList();

            var wb = new XLWorkbook();

            var ws = wb.Worksheets.Add("DanhSachLoiNhuanNgay");
            ws.Column("A").Width = 4.18;
            ws.Column("B").Width = 14;
            ws.Column("C").Width = 16;
            ws.Column("D").Width = 16;
            ws.Column("E").Width = 15;
            ws.Column("F").Width = 15;

            ws.Cell("A1").Style.Font.Bold = true;
            ws.Cell("A1").Style.Font.FontSize = 13;
            ws.Cell("A1").Value = "CỬA HÀNG CHUỔI THỰC PHẨM TH True Milk";
            ws.Range("A1:F1").Row(1).Merge();

            ws.Cell("A2").Style.Font.Bold = true;
            ws.Cell("A2").Style.Font.FontSize = 11;
            ws.Cell("A2").Value = "SĐT:028 3602 1133 - ĐC:204/16/5 Quốc Lộ 13, P.26, Q.Bình Thạnh, Tp.HCM";
            ws.Range("A2:F2").Row(1).Merge();

            ws.Cell("A3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A3").Style.Font.Bold = true;
            ws.Cell("A3").Style.Font.FontSize = 16;
            ws.Cell("A3").Value = "THỐNG KÊ LỢI NHUẬN";
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
            ws.Cell("B5").Value = "Ngày";
            ws.Cell("C5").Value = "Tiền hàng";
            ws.Cell("D5").Value = "Doanh thu";
            ws.Cell("E5").Value = "Vốn";
            ws.Cell("F5").Value = "Lãi gộp";

            int row = 6;
            decimal tienhang = 0;
            decimal doanhthu = 0;
            decimal von = 0;
            decimal laigop = 0;
            for (int i = 0; i < model.Count; i++)
            {
                ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("A" + row).Value = model[i].STT;

                ws.Cell("B" + row).Value = model[i].CreateDate.Value.ToString("dd/MM/yyyy");

                ws.Cell("C" + row).Style.NumberFormat.Format = "#,##0";
                ws.Cell("C" + row).Value = model[i].TienHang;

                ws.Cell("D" + row).Style.NumberFormat.Format = "#,##0";
                ws.Cell("D" + row).Value = model[i].DoanhThu;

                ws.Cell("E" + row).Style.NumberFormat.Format = "#,##0";
                ws.Cell("E" + row).Value = model[i].Von;

                ws.Cell("F" + row).Style.NumberFormat.Format = "#,##0";
                ws.Cell("F" + row).Value = model[i].Lai;

                tienhang += model[i].TienHang.Value;
                doanhthu += model[i].DoanhThu.Value;
                von += model[i].Von.Value;
                laigop += model[i].Lai;

                row++;
            }

            var rangeborder = ws.Range("A5:F" + (row));
            rangeborder.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rangeborder.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Value = "Tổng cộng";



            ws.Cell("C" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("C" + row).Style.NumberFormat.Format = "#,##0";
            ws.Cell("C" + row).Style.Font.Bold = true;
            ws.Cell("C" + row).Value = tienhang.ToString("N0")+"VNĐ";

            ws.Cell("D" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("D" + row).Style.NumberFormat.Format = "#,##0";
            ws.Cell("D" + row).Style.Font.Bold = true;
            ws.Cell("D" + row).Value = doanhthu.ToString("N0") + "VNĐ";

            ws.Cell("E" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("E" + row).Style.NumberFormat.Format = "#,##0";
            ws.Cell("E" + row).Style.Font.Bold = true;
            ws.Cell("E" + row).Value = von.ToString("N0") + "VNĐ";

            ws.Cell("F" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("F" + row).Style.NumberFormat.Format = "#,##0";
            ws.Cell("F" + row).Style.Font.Bold = true;
            ws.Cell("F" + row).Value = laigop.ToString("N0") + "VNĐ";


            //Ký tên
            ws.Cell("E" + (row + 2)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("E" + (row + 2)).Style.Font.Italic = true;
            ws.Cell("E" + (row + 2)).Value = $"TP.HCM, ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}";
            ws.Range($"E{row + 2}:F{row + 2}").Row(1).Merge();


            ws.Cell("E" + (row + 3)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("E" + (row + 3)).Style.Font.Bold = true;
            ws.Cell("E" + (row + 3)).Value = "Người lập phiếu";
            ws.Range($"E{row + 3}:F{row + 3}").Row(1).Merge();

            ws.Cell("E" + (row + 4)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("E" + (row + 4)).Style.Font.Italic = true;
            ws.Cell("E" + (row + 4)).Value = "(Ký, họ tên, đóng dấu)";
            ws.Range($"E{row + 4}:F{row + 4}").Row(1).Merge();

            ws.Cell("E" + (row + 6)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("E" + (row + 6)).Style.Font.Bold = true;
            ws.Cell("E" + (row + 6)).Value = NguoiLapPhieu(session.UserID);
            ws.Range($"E{row + 6}:F{row + 6}").Row(1).Merge();


            string namefile = "DSLoiNhuanNgay_" + DateTime.Now.Ticks + ".xlsx";
            string fileLPath = Server.MapPath(@"~/Data/ExportExcel/" + namefile);
            wb.SaveAs(fileLPath);

            return Json(new { code = 200, NameFile = namefile }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Xuất danh sách thống kê lợi nhuận hóa đơn ngày
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

            var model = dao.GetThongKeLoiNhuanThang(search, 1, 10000000).ToList();

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

            string ret = RenderPartialToStringDSNgay("~/Areas/Admin/Views/StatisticLN/ExportPDFDSNgay.cshtml", model, ControllerContext, createDate, NguoiLapPhieu(session.UserID));

            PdfDocument pdf = htmlToPdf.ConvertHtmlString(ret);


            string namefile = "DSDoanhSoNgay_" + DateTime.Now.Ticks + ".pdf";
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
        public JsonResult GetThongKeTheoKhachHang(SearchThongKe search, int page = 1, int pageSize = 3)
        {
            var dao = new ThongKeDAO();

            IPagedList model = (IPagedList)dao.GetThongKeLoiNhuanKhachHang(search, page, pageSize);

            var lst = dao.GetThongKeLoiNhuanKhachHang(search, 1, 10000000);

            int c = model.TotalItemCount;

            return Json(new { code = 200, data = model, total = c, tienhang = lst.Sum(t => t.TienHang), doanthu = lst.Sum(t => t.DoanhThu), von = lst.Sum(t => t.Von), lai = lst.Sum(t => t.Lai)}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetThongKeDetailTheoKhachHang(SearchThongKe search, int id, int page = 1, int pageSize = 3)
        {
            var dao = new ThongKeDAO();

            IPagedList model = (IPagedList)dao.ListOrderLoiNhuanTheoKhachHang(search, id, page, pageSize);


            var lst = dao.ListOrderLoiNhuanTheoKhachHang(search, id, 1, 10000000).ToList();

            int c = model.TotalItemCount;

            return Json(new { code = 200, data = model, total = c, tienhang = lst.Sum(t => t.TienHang), doanthu = lst.Sum(t => t.DoanhThu), von = lst.Sum(t => t.Von), lai = lst.Sum(t => t.Lai) }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Export danh sách thống kê lợi nhuận theo khách hàng
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

            var model = dao.ListOrderLoiNhuanTheoKhachHang(search, id, 1, 10000000).ToList();

            var wb = new XLWorkbook();

            var ws = wb.Worksheets.Add("LoiNhuanKH");
            ws.Column("A").Width = 4.18;
            ws.Column("B").Width = 12;
            ws.Column("C").Width = 16;
            ws.Column("D").Width = 15;
            ws.Column("E").Width = 19;
            ws.Column("F").Width = 18;
            ws.Column("G").Width = 18;
            ws.Column("H").Width = 18;
            ws.Column("I").Width = 18;

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
            ws.Cell("A3").Value = "THỐNG KÊ LỢI NHUẬN KHÁCH HÀNG: KH" + ChangeIDProduct(id);
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

            ws.Cell("C5").Value = "Ngày tạo";
            ws.Cell("D5").Value = "Mã khách hàng";
            ws.Cell("E5").Value = "Khách hàng";
            ws.Cell("F5").Value = "Tiền hàng";
            ws.Cell("G5").Value = "Doanh thu";
            ws.Cell("H5").Value = "Vốn";
            ws.Cell("I5").Value = "Lãi gộp";

            int row = 6;
            double tienhang = 0;
            double doanhthu = 0;
            double von = 0;
            double laigop = 0;
            for (int i = 0; i < model.Count; i++)
            {
                ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("A" + row).Value = model[i].STT;

                ws.Cell("B" + row).Value = "HD" + ChangeIDProduct(model[i].OrderID);

                ws.Cell("C" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("C" + row).Value = model[i].CreateDate;

                ws.Cell("D" + row).Value = "KH" + ChangeIDProduct(model[i].CustomerID.Value);
                ws.Cell("E" + row).Value = model[i].CustomerName;

                ws.Cell("F" + row).Style.NumberFormat.Format = "#,##0";
                ws.Cell("F" + row).Value = model[i].TienHang;

                ws.Cell("G" + row).Style.NumberFormat.Format = "#,##0";
                ws.Cell("G" + row).Value = (model[i].DoanhThu);

                ws.Cell("H" + row).Style.NumberFormat.Format = "#,##0";
                ws.Cell("H" + row).Value = model[i].Von;

                ws.Cell("I" + row).Style.NumberFormat.Format = "#,##0";
                ws.Cell("I" + row).Value = model[i].Lai;

                tienhang += (double)(model[i].TienHang);
                doanhthu += (double)(model[i].DoanhThu);
                von += (double)(model[i].Von);
                laigop += (double)(model[i].Lai);
                row++;
            }

            var rangeborder = ws.Range("A5:I" + (row));
            rangeborder.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rangeborder.Style.Border.InsideBorder = XLBorderStyleValues.Thin;


            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Value = "Tổng cộng";
            ws.Range($"B{row}:D{row}").Row(1).Merge();


            ws.Cell("F" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("F" + row).Style.NumberFormat.Format = "#,##0";
            ws.Cell("F" + row).Style.Font.Bold = true;
            ws.Cell("F" + row).Value = tienhang.ToString("N0")+"VNĐ";

            ws.Cell("G" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("G" + row).Style.NumberFormat.Format = "#,##0";
            ws.Cell("G" + row).Style.Font.Bold = true;
            ws.Cell("G" + row).Value = doanhthu.ToString("N0") + "VNĐ";

            ws.Cell("H" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("H" + row).Style.NumberFormat.Format = "#,##0";
            ws.Cell("H" + row).Style.Font.Bold = true;
            ws.Cell("H" + row).Value = von.ToString("N0") + "VNĐ";

            ws.Cell("I" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("I" + row).Style.NumberFormat.Format = "#,##0";
            ws.Cell("I" + row).Style.Font.Bold = true;
            ws.Cell("I" + row).Value = laigop.ToString("N0") + "VNĐ";

            //Ký tên
            ws.Cell("H" + (row + 2)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("H" + (row + 2)).Style.Font.Italic = true;
            ws.Cell("H" + (row + 2)).Value = $"TP.HCM, ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}";
            ws.Range($"H{row + 2}:I{row + 2}").Row(1).Merge();


            ws.Cell("H" + (row + 3)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("H" + (row + 3)).Style.Font.Bold = true;
            ws.Cell("H" + (row + 3)).Value = "Người lập phiếu";
            ws.Range($"H{row + 3}:I{row + 3}").Row(1).Merge();

            ws.Cell("H" + (row + 4)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("H" + (row + 4)).Style.Font.Italic = true;
            ws.Cell("H" + (row + 4)).Value = "(Ký, họ tên, đóng dấu)";
            ws.Range($"H{row + 4}:I{row + 4}").Row(1).Merge();

            ws.Cell("H" + (row + 6)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("H" + (row + 6)).Style.Font.Bold = true;
            ws.Cell("H" + (row + 6)).Value = NguoiLapPhieu(session.UserID);
            ws.Range($"H{row + 6}:I{row + 6}").Row(1).Merge();


            string namefile = "LoiNhuanKH_KH" + ChangeIDProduct(id) + " " + DateTime.Now.Ticks + ".xlsx";
            string fileLPath = Server.MapPath(@"~/Data/ExportExcel/" + namefile);
            wb.SaveAs(fileLPath);

            return Json(new { code = 200, NameFile = namefile }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Xuất lợi nhuận pdf hóa đơn theo khách hàng
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ExportPDFListThongkeKhachhang(SearchThongKe search, int id)
        {

            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            var dao = new ThongKeDAO();

            var model = dao.ListOrderLoiNhuanTheoKhachHang(search, id, 1, 10000000).ToList();

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

            string ret = RenderPartialToStringNhanVien("~/Areas/Admin/Views/StatisticLN/ExportPDFKhachHang.cshtml", model, ControllerContext, ngay, "KH" + ChangeIDProduct(id), NguoiLapPhieu(session.UserID));

            PdfDocument pdf = htmlToPdf.ConvertHtmlString(ret);


            string namefile = "LoiNhuanKH_KH" + ChangeIDProduct(id) + " " + DateTime.Now.Ticks + ".pdf";
            string fileLPath = Server.MapPath(@"~/Data/ExportPDF/" + namefile);
            pdf.Save(fileLPath);
            pdf.Close();

            return Json(new { code = 200, NameFile = namefile }, JsonRequestBehavior.AllowGet);
        }
        
        public string RenderPartialToStringNhanVien(string viewName, object model, ControllerContext ControllerContext, string Ngay, string nhanvien, string nguoilap)
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
        /// Xuất danh sách thống kê lợi nhuận hóa đơn ngày
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

            var model = dao.GetThongKeLoiNhuanKhachHang(search, 1, 10000000).ToList();

            var wb = new XLWorkbook();

            var ws = wb.Worksheets.Add("DanhSachLoiNhuanKH");
            ws.Column("A").Width = 4.18;
            ws.Column("B").Width = 14;
            ws.Column("C").Width = 16;
            ws.Column("D").Width = 16;
            ws.Column("E").Width = 15;
            ws.Column("F").Width = 15;
            ws.Column("G").Width = 15;

            ws.Cell("A1").Style.Font.Bold = true;
            ws.Cell("A1").Style.Font.FontSize = 13;
            ws.Cell("A1").Value = "CỬA HÀNG CHUỔI THỰC PHẨM TH True Milk";
            ws.Range("A1:F1").Row(1).Merge();

            ws.Cell("A2").Style.Font.Bold = true;
            ws.Cell("A2").Style.Font.FontSize = 11;
            ws.Cell("A2").Value = "SĐT:028 3602 1133 - ĐC:204/16/5 Quốc Lộ 13, P.26, Q.Bình Thạnh, Tp.HCM";
            ws.Range("A2:G2").Row(1).Merge();

            ws.Cell("A3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A3").Style.Font.Bold = true;
            ws.Cell("A3").Style.Font.FontSize = 16;
            ws.Cell("A3").Value = "THỐNG KÊ LỢI NHUẬN";
            ws.Range("A3:G3").Row(1).Merge();

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
            ws.Range("A4:G3").Row(1).Merge();


            var rangeHeader = ws.Range("A5:G5");
            rangeHeader.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangeHeader.Style.Font.Bold = true;
            rangeHeader.Style.Fill.BackgroundColor = XLColor.Orange;
            rangeHeader.Style.Font.FontColor = XLColor.White;
            rangeHeader.Style.Font.FontSize = 12;

            ws.Cell("A5").Value = "STT";
            ws.Cell("B5").Value = "Mã khách hàng";
            ws.Cell("C5").Value = "Khách hàng";
            ws.Cell("D5").Value = "Tiền hàng";
            ws.Cell("E5").Value = "Doanh thu";
            ws.Cell("F5").Value = "Vốn";
            ws.Cell("G5").Value = "Lãi gộp";

            int row = 6;
            decimal tienhang = 0;
            decimal doanhthu = 0;
            decimal von = 0;
            decimal laigop = 0;
            for (int i = 0; i < model.Count; i++)
            {
                ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("A" + row).Value = model[i].STT;

                ws.Cell("B" + row).Value = "KH"+ChangeIDProduct(model[i].CustomerID.Value);

                ws.Cell("C" + row).Value = model[i].CustomerName;

                ws.Cell("D" + row).Style.NumberFormat.Format = "#,##0";
                ws.Cell("D" + row).Value = model[i].TienHang;

                ws.Cell("E" + row).Style.NumberFormat.Format = "#,##0";
                ws.Cell("E" + row).Value = model[i].DoanhThu;

                ws.Cell("F" + row).Style.NumberFormat.Format = "#,##0";
                ws.Cell("F" + row).Value = model[i].Von;

                ws.Cell("G" + row).Style.NumberFormat.Format = "#,##0";
                ws.Cell("G" + row).Value = model[i].Lai;

                tienhang += model[i].TienHang.Value;
                doanhthu += model[i].DoanhThu.Value;
                von += model[i].Von.Value;
                laigop += model[i].Lai;

                row++;
            }

            var rangeborder = ws.Range("A5:G" + (row));
            rangeborder.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rangeborder.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Value = "Tổng cộng";



            ws.Cell("D" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("D" + row).Style.NumberFormat.Format = "#,##0";
            ws.Cell("D" + row).Style.Font.Bold = true;
            ws.Cell("D" + row).Value = tienhang.ToString("N0") + "VNĐ";

            ws.Cell("E" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("E" + row).Style.NumberFormat.Format = "#,##0";
            ws.Cell("E" + row).Style.Font.Bold = true;
            ws.Cell("E" + row).Value = doanhthu.ToString("N0") + "VNĐ";

            ws.Cell("F" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("F" + row).Style.NumberFormat.Format = "#,##0";
            ws.Cell("F" + row).Style.Font.Bold = true;
            ws.Cell("F" + row).Value = von.ToString("N0") + "VNĐ";

            ws.Cell("G" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("G" + row).Style.NumberFormat.Format = "#,##0";
            ws.Cell("G" + row).Style.Font.Bold = true;
            ws.Cell("G" + row).Value = laigop.ToString("N0") + "VNĐ";


            //Ký tên
            ws.Cell("F" + (row + 2)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F" + (row + 2)).Style.Font.Italic = true;
            ws.Cell("F" + (row + 2)).Value = $"TP.HCM, ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}";
            ws.Range($"F{row + 2}:G{row + 2}").Row(1).Merge();


            ws.Cell("F" + (row + 3)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F" + (row + 3)).Style.Font.Bold = true;
            ws.Cell("F" + (row + 3)).Value = "Người lập phiếu";
            ws.Range($"F{row + 3}:G{row + 3}").Row(1).Merge();

            ws.Cell("F" + (row + 4)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F" + (row + 4)).Style.Font.Italic = true;
            ws.Cell("F" + (row + 4)).Value = "(Ký, họ tên, đóng dấu)";
            ws.Range($"F{row + 4}:G{row + 4}").Row(1).Merge();

            ws.Cell("F" + (row + 6)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F" + (row + 6)).Style.Font.Bold = true;
            ws.Cell("F" + (row + 6)).Value = NguoiLapPhieu(session.UserID);
            ws.Range($"F{row + 6}:G{row + 6}").Row(1).Merge();


            string namefile = "DSLoiNhuanKH_" + DateTime.Now.Ticks + ".xlsx";
            string fileLPath = Server.MapPath(@"~/Data/ExportExcel/" + namefile);
            wb.SaveAs(fileLPath);

            return Json(new { code = 200, NameFile = namefile }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Xuất danh sách thống kê lợi nhuận hóa đơn theo khách hàng
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

            var model = dao.GetThongKeLoiNhuanKhachHang(search, 1, 10000000).ToList();

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

            string ret = RenderPartialToStringDSNgay("~/Areas/Admin/Views/StatisticLN/ExportPDFDSKhachHang.cshtml", model, ControllerContext, createDate, NguoiLapPhieu(session.UserID));

            PdfDocument pdf = htmlToPdf.ConvertHtmlString(ret);


            string namefile = "DSDoanhSoKH_" + DateTime.Now.Ticks + ".pdf";
            string fileLPath = Server.MapPath(@"~/Data/ExportPDF/" + namefile);
            pdf.Save(fileLPath);
            pdf.Close();

            return Json(new { code = 200, NameFile = namefile }, JsonRequestBehavior.AllowGet);

        }

        //Báo cáo lãi lổ
        [HttpPost]
        public JsonResult BaoCaoLaiLo(SearchThongKe search)
        {
            var dao = new ThongKeDAO();

            var model = dao.BaoCaoLaiLo(search);

            return Json(new { code = 200, data = model}, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Xuất danh sách báo cáo
        /// </summary>
        /// <param name="createDate"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ExportExcelBaoCao(SearchThongKe search)
        {
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            var dao = new ThongKeDAO();

            var model = dao.BaoCaoLaiLo(search);

            var wb = new XLWorkbook();

            var ws = wb.Worksheets.Add("DanhSachLoiNhuanKH");
            ws.Column("A").Width = 36;
            ws.Column("B").Width = 35;
            ws.Column("C").Width = 35;
            ws.Column("D").Width = 16;

            ws.Cell("A1").Style.Font.Bold = true;
            ws.Cell("A1").Style.Font.FontSize = 13;
            ws.Cell("A1").Value = "CỬA HÀNG CHUỔI THỰC PHẨM TH True Milk";
            ws.Range("A1:D1").Row(1).Merge();

            ws.Cell("A2").Style.Font.Bold = true;
            ws.Cell("A2").Style.Font.FontSize = 11;
            ws.Cell("A2").Value = "SĐT:028 3602 1133 - ĐC:204/16/5 Quốc Lộ 13, P.26, Q.Bình Thạnh, Tp.HCM";
            ws.Range("A2:D2").Row(1).Merge();

            ws.Cell("A3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A3").Style.Font.Bold = true;
            ws.Cell("A3").Style.Font.FontSize = 16;
            ws.Cell("A3").Value = "BÁO CÁO LÃI LỖ";
            ws.Range("A3:D3").Row(1).Merge();

            var rangeHeader = ws.Range("A4:D5");
            rangeHeader.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangeHeader.Style.Font.Bold = true;
            rangeHeader.Style.Fill.BackgroundColor = XLColor.Orange;
            rangeHeader.Style.Font.FontColor = XLColor.White;
            rangeHeader.Style.Font.FontSize = 12;

            var rangeborder1 = ws.Range("B4:C5");
            rangeborder1.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rangeborder1.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            var rangeborder2 = ws.Range("A4:A5");
            rangeborder2.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            var rangeborder3 = ws.Range("D4:D5");
            rangeborder3.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            ws.Cell("A4").Value = "Hạng mục";
            ws.Cell("A4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A4").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Range("A4:A5").Column(1).Merge();

            ws.Cell("B4").Value = "Kỳ trước ";
            ws.Cell("B5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B5").Value = $"({model.KyTruocTu.ToString("dd/MM/yyyy")} -{model.KyTruocDen.ToString("dd/MM/yyyy")})";

            ws.Cell("C4").Value = "Kỳ báo cáo";
            ws.Cell("C5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C5").Value = $"({model.KyBaoCaoTu.ToString("dd/MM/yyyy")} -{model.KyBaoCaoDen.ToString("dd/MM/yyyy")})";

            ws.Cell("D4").Value = "Thai đổi";
            ws.Cell("D4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D4").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Range("D4:D5").Column(1).Merge();


            //Nội dung
            ws.Cell("A6").Value = "1. "+model.DoanhSoBanHang;
            ws.Cell("B6").Style.NumberFormat.Format = "#,##0";
            ws.Cell("B6").Value = model.KyTruocDoanhSoBanHang.HasValue? model.KyTruocDoanhSoBanHang.Value:0;

            ws.Cell("C6").Style.NumberFormat.Format = "#,##0";
            ws.Cell("C6").Value = model.KyBaoCaoDoanhSoBanHang.HasValue ? model.KyBaoCaoDoanhSoBanHang.Value:0;

            ws.Cell("D6").Style.NumberFormat.Format = "#,##0";
            ws.Cell("D6").Value = model.ThaydoiDoanhSoBanHang.HasValue? model.ThaydoiDoanhSoBanHang.Value:0;

            ws.Cell("A7").Value = "2. " + model.GiamGia;
            ws.Cell("B7").Style.NumberFormat.Format = "#,##0";
            ws.Cell("B7").Value = model.KyTruocGiamGia.HasValue ? model.KyTruocGiamGia.Value : 0;

            ws.Cell("C7").Style.NumberFormat.Format = "#,##0";
            ws.Cell("C7").Value = model.KyBaoCaoGiamGia.HasValue ? model.KyBaoCaoGiamGia.Value : 0;

            ws.Cell("D7").Style.NumberFormat.Format = "#,##0";
            ws.Cell("D7").Value = model.ThaydoiGiamGia.HasValue ? model.ThaydoiGiamGia.Value : 0;

            ws.Cell("A8").Value = "3. " + model.PhiVanChuyen;
            ws.Cell("B8").Style.NumberFormat.Format = "#,##0";
            ws.Cell("B8").Value = model.KyTruocPhiVanChuyen.HasValue ? model.KyTruocPhiVanChuyen.Value : 0;

            ws.Cell("C8").Style.NumberFormat.Format = "#,##0";
            ws.Cell("C8").Value = model.KyBaoCaoPhiVanChuyen.HasValue ? model.KyBaoCaoPhiVanChuyen.Value : 0;

            ws.Cell("D8").Style.NumberFormat.Format = "#,##0";
            ws.Cell("D8").Value = model.ThaydoiPhiVanChuyen.HasValue ? model.ThaydoiPhiVanChuyen.Value : 0;


            ws.Cell("A9").Value = "4. " + model.DoanhThu;
            ws.Cell("B9").Style.NumberFormat.Format = "#,##0";
            ws.Cell("B9").Value = model.KyTruocDoanhThu.HasValue ? model.KyTruocDoanhThu.Value : 0;

            ws.Cell("C9").Style.NumberFormat.Format = "#,##0";
            ws.Cell("C9").Value = model.KyBaoCaoDoanhThu.HasValue ? model.KyBaoCaoDoanhThu.Value : 0;

            ws.Cell("D9").Style.NumberFormat.Format = "#,##0";
            ws.Cell("D9").Value = model.ThaydoiDoanhThu.HasValue ? model.ThaydoiDoanhThu.Value : 0;


            ws.Cell("A10").Value = "5. " + model.VonHangHoa;
            ws.Cell("B10").Style.NumberFormat.Format = "#,##0";
            ws.Cell("B10").Value = model.KyTruocVonHangHoa.HasValue ? model.KyTruocVonHangHoa.Value : 0;

            ws.Cell("C10").Style.NumberFormat.Format = "#,##0";
            ws.Cell("C10").Value = model.KyBaoCaoVonHangHoa.HasValue ? model.KyBaoCaoVonHangHoa.Value : 0;

            ws.Cell("D10").Style.NumberFormat.Format = "#,##0";
            ws.Cell("D10").Value = model.ThaydoiVonHangHoa.HasValue ? model.ThaydoiVonHangHoa.Value : 0;


            ws.Cell("A11").Value = "6. " + model.LaiGop;
            ws.Cell("B11").Style.NumberFormat.Format = "#,##0";
            ws.Cell("B11").Value = model.KyTruocLaiGop.HasValue ? model.KyTruocLaiGop.Value : 0;

            ws.Cell("C11").Style.NumberFormat.Format = "#,##0";
            ws.Cell("C11").Value = model.KyBaoCaoLaiGop.HasValue ? model.KyBaoCaoLaiGop.Value : 0;

            ws.Cell("D11").Style.NumberFormat.Format = "#,##0";
            ws.Cell("D11").Value = model.ThaydoiLaigop.HasValue ? model.ThaydoiLaigop.Value : 0;


            ws.Cell("A12").Value = "6. " + model.TienGiamChiaDoanhThu;
            ws.Cell("B12").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("B12").Value = model.KyTruocTienGiamChiaDoanhThu.HasValue ? model.KyTruocTienGiamChiaDoanhThu.Value+"%" : "0%";


            ws.Cell("C12").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("C12").Value = model.KyBaoCaoTienGiamChiaDoanhThu.HasValue ? model.KyBaoCaoTienGiamChiaDoanhThu.Value + "%" : "0%";

            ws.Cell("D12").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("D12").Value = model.ThaydoiTienGiamChiaDoanhThu.HasValue ? model.ThaydoiTienGiamChiaDoanhThu.Value + "%" : "0%";


            ws.Cell("A13").Value = "7. " + model.LaiGopChiaDoanhThu;
            ws.Cell("B13").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("B13").Value = model.KyTruocTienGiamChiaLaiGop.HasValue ? model.KyTruocTienGiamChiaLaiGop.Value + "%" : "0%";


            ws.Cell("C13").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("C13").Value = model.KyBaoCaoTienGiamChiaLaiGop.HasValue ? model.KyBaoCaoTienGiamChiaLaiGop.Value + "%" : "0%";

            ws.Cell("D13").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("D13").Value = model.ThaydoiLaiGopChiaDoanhThu.HasValue ? model.ThaydoiLaiGopChiaDoanhThu.Value + "%" : "0%";


            var rangeborder = ws.Range("A6:D13");
            rangeborder.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rangeborder.Style.Border.InsideBorder = XLBorderStyleValues.Thin;


            //Ký tên
            ws.Cell("C14").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C14").Style.Font.Italic = true;
            ws.Cell("C14").Value = $"TP.HCM, ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}";
            ws.Range($"C14:D14").Row(1).Merge();


            ws.Cell("C15").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C15").Style.Font.Bold = true;
            ws.Cell("C15").Value = "Người lập phiếu";
            ws.Range("C15:D15").Row(1).Merge();

            ws.Cell("C16").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C16").Style.Font.Italic = true;
            ws.Cell("C16").Value = "(Ký, họ tên, đóng dấu)";
            ws.Range("C16:D16").Row(1).Merge();

            ws.Cell("C17").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C17").Style.Font.Bold = true;
            ws.Cell("C17").Value = NguoiLapPhieu(session.UserID);
            ws.Range("C17:D17").Row(1).Merge();


            string namefile = "BaoCaoLaiLo_" + DateTime.Now.Ticks + ".xlsx";
            string fileLPath = Server.MapPath(@"~/Data/ExportExcel/" + namefile);
            wb.SaveAs(fileLPath);

            return Json(new { code = 200, NameFile = namefile }, JsonRequestBehavior.AllowGet);

        }

    }
}