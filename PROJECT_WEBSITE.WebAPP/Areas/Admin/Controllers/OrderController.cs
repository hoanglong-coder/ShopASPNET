using ClosedXML.Excel;
using Newtonsoft.Json;
using PagedList;
using PROJECT_WEBSITE.Data.DAO;
using PROJECT_WEBSITE.Data.EF;
using PROJECT_WEBSITE.Data.ModelCustom;
using PROJECT_WEBSITE.WebAPP.Areas.Admin.Models;
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
    public class OrderController : RoleDonHangController
    {
        // GET: Admin/Order
        public ActionResult Index()
        {

            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            ViewBag.UserName = session.FullName;

            ViewBag.Role = session.RoleString;

            var dao = new OrderDetailDAO();
            ViewBag.listorderdetail = dao.GetAll();
            return View();
        }

        [HttpPost]
        public JsonResult ListOrder(SearchOrder search, int page = 1, int pageSize = 3)
        {
            var dao = new OrderDAO();
            IPagedList model = (IPagedList)dao.ListOrder(search, page, pageSize);
            int c = model.TotalItemCount;

            var totalprice = dao.ListOrder(search, 1, 1000000).ToList();

            return Json(new { code = 200, data = model, total = c, totalprice = totalprice.Sum(t => t.TotalPrice), totalcout = totalprice.Sum(t => t.TotalCount) }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ListOrderDetail(int id)
        {
            var db = new DbWebsite();
            var dao = new OrderDetailDAO();
            var model = dao.GetbyId(id);
            List<OrderDetailModel> listorderdetail = new List<OrderDetailModel>();

            foreach (var item in model)
            {
                OrderDetailModel t = new OrderDetailModel(new ProductDAO().DetailProduct(item.ProductID), item.OrderID, item.OrderDetailCount.Value, item.OrderPrice.Value, (item.OrderDetailCount.Value * item.OrderPrice.Value), item.Product.UnitID.HasValue?db.ProductUnits.Find(item.Product.UnitID.Value).Name:"Combo sản phẩm");
                listorderdetail.Add(t);
            }
            var order = db.Orders.Find(id);

            return Json(new { code = 200, data = listorderdetail, khuyenmai = order.PriceDiscount }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Xác nhận đơn hàng
        /// </summary>
        /// <param name="idorder">Mã đơn hàng</param>
        /// <returns>Đơn hàng xác nhận</returns>
        [HttpPost]
        public JsonResult OrderConfirma(int idorder)
        {
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];
            var dao = new OrderDAO();
            var check = dao.OrderConfirma(idorder, session.UserID);
            if (check)
            {
                return Json(new { code = 200 }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { code = 500 }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult ExportExcelOrder(int id)
        {
            var db = new DbWebsite();
            var dao = new OrderDetailDAO();
            var model = dao.GetbyId(id);
            List<OrderDetailModel> listorderdetail = new List<OrderDetailModel>();

            foreach (var item in model)
            {
                OrderDetailModel t = new OrderDetailModel(new ProductDAO().DetailProduct(item.ProductID), item.OrderID, item.OrderDetailCount.Value, item.OrderPrice.Value, (item.OrderDetailCount.Value * item.OrderPrice.Value), item.Product.UnitID.HasValue ? db.ProductUnits.Find(item.Product.UnitID.Value).Name : "Combo sản phẩm");
                listorderdetail.Add(t);
            }


            var wb = new XLWorkbook();

            var ws = wb.Worksheets.Add("HoaDonBanHang");
            ws.Column("B").Width = 50;
            ws.Column("C").Width = 8;
            ws.Column("D").Width = 5;
            ws.Column("E").Width = 12;
            ws.Column("F").Width = 14;

            ws.Cell("A1").Style.Font.Bold = true;
            ws.Cell("A1").Style.Font.FontSize = 13;
            ws.Cell("A1").Value = "CỬA HÀNG CHUỔI THỰC PHẨM TH True Milk";
            ws.Range("A1:B1").Row(1).Merge();

            ws.Cell("A2").Style.Font.Bold = true;
            ws.Cell("A2").Style.Font.FontSize = 11;
            ws.Cell("A2").Value = "SĐT:028 3602 1133 - ĐC:204/16/5 Quốc Lộ 13, P.26, Q.Bình Thạnh, Tp.HCM";
            ws.Range("A2:C2").Row(1).Merge();


            ws.Cell("A3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A3").Style.Font.Bold = true;
            ws.Cell("A3").Style.Font.FontSize = 16;
            ws.Cell("A3").Value = "HÓA ĐƠN BÁN HÀNG";
            ws.Range("A3:F3").Row(1).Merge();

            ws.Cell("A4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A4").Style.Font.Bold = true;
            ws.Cell("A4").Style.Font.FontSize = 11;
            ws.Cell("A4").Value = $"(Mã hóa đơn: HD{ChangeIDProduct(id)} - Ngày: {db.Orders.Find(id).CreateDate} - Lập phiếu: {NguoiLapPhieu(id)})";
            ws.Range("A4:F3").Row(1).Merge();

            ws.Cell("A5").Style.Font.Bold = true;
            ws.Cell("A5").Style.Font.FontSize = 11;
            ws.Cell("A5").Value = $"Khách hàng: {db.Orders.Find(id).ShipName}";
            ws.Range("A5:F3").Row(1).Merge();

            ws.Cell("A6").Style.Font.Bold = true;
            ws.Cell("A6").Style.Font.FontSize = 11;
            ws.Cell("A6").Value = $"Địa chỉ giao hàng: {db.Orders.Find(id).ShipAddress.Replace('+', '/').Trim('/')}";
            ws.Range("A6:F3").Row(1).Merge();

            ws.Cell("A7").Style.Font.Bold = true;
            ws.Cell("A7").Style.Font.FontSize = 11;
            ws.Cell("A7").Value = $"Số điện thoại: {db.Orders.Find(id).ShipPhone}";
            ws.Range("A7:F3").Row(1).Merge();


            var rangeHeader = ws.Range("A8:F8");
            rangeHeader.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangeHeader.Style.Font.Bold = true;
            rangeHeader.Style.Fill.BackgroundColor = XLColor.Orange;
            rangeHeader.Style.Font.FontColor = XLColor.White;
            rangeHeader.Style.Font.FontSize = 12;


            ws.Cell("A8").Value = "STT";

            ws.Cell("B8").Value = "Tên hàng hóa";

            ws.Cell("C8").Value = "ĐVT";
            ws.Cell("D8").Value = "SL";
            ws.Cell("E8").Value = "Đơn giá";
            ws.Cell("F8").Value = "Thành tiền";


            int row = 9;
            double total = 0;
            int soluong = 0;
            for (int i = 0; i < listorderdetail.Count; i++)
            {
                ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("A" + row).Value = i + 1;

                ws.Cell("B" + row).Value = listorderdetail[i].Product.Name;

                ws.Cell("C" + row).Value = listorderdetail[i].DVT;
                ws.Cell("D" + row).Value = listorderdetail[i].Quantity;
                ws.Cell("E" + row).Style.NumberFormat.Format = "#,##0";
                ws.Cell("E" + row).Value = listorderdetail[i].Price;
                ws.Cell("F" + row).Style.NumberFormat.Format = "#,##0";
                ws.Cell("F" + row).Value = listorderdetail[i].TotalPrice;
                total += (double)listorderdetail[i].TotalPrice;
                soluong += listorderdetail[i].Quantity;
                row++;
            }

            var rangeborder = ws.Range("A8:F" + (row - 1));
            rangeborder.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rangeborder.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            //Tổng tiền
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Value = "Tổng tiền hàng:";
            ws.Range($"A{row}:C{row}").Row(1).Merge();

            ws.Cell("D" + row).Style.Font.Bold = true;
            ws.Cell("D" + row).Value = soluong;

            ws.Cell("F" + row).Style.NumberFormat.Format = "#,##0";
            ws.Cell("F" + row).Style.Font.Bold = true;
            ws.Cell("F" + row).Value = total;



            //Khuyến mãi
            ws.Cell("A" + (row + 1)).Style.Font.Bold = true;
            ws.Cell("A" + (row + 1)).Value = "Khuyến mãi:";
            ws.Range($"A{row + 1}:C{row + 1}").Row(1).Merge();

            ws.Cell("F" + (row + 1)).Style.NumberFormat.Format = "#,##0";
            ws.Cell("F" + (row + 1)).Style.Font.Bold = true;
            ws.Cell("F" + (row + 1)).Value = db.Orders.Find(id).PriceDiscount.Value * (-1);


            //Tiền ship
            ws.Cell("A" + (row+2)).Style.Font.Bold = true;
            ws.Cell("A" + (row+2)).Value = "Tiền ship:";
            ws.Range($"A{row+2}:C{row+2}").Row(1).Merge();

            ws.Cell("F" + (row+2)).Style.NumberFormat.Format = "#,##0";
            ws.Cell("F" + (row+2)).Style.Font.Bold = true;
            ws.Cell("F" + (row+2)).Value = db.Orders.Find(id).PriceShip;


            //Tổng cộng
            ws.Cell("A" + (row + 3)).Style.Font.Bold = true;
            ws.Cell("A" + (row + 3)).Value = "Tổng cộng:";
            ws.Range($"A{row + 3}:C{row + 3}").Row(1).Merge();


            ws.Cell("F" + (row + 3)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("F" + (row + 3)).Style.NumberFormat.Format = "#,##0";
            ws.Cell("F" + (row + 3)).Style.Font.Bold = true;
            ws.Cell("F" + (row + 3)).Value = ((total - (double)db.Orders.Find(id).PriceDiscount.Value) + (double)db.Orders.Find(id).PriceShip.Value).ToString("N0") + " VNĐ";


            //tiền chữ
            string value = "A" + (row + 4);
            string valuerange = value + ":F" + (row + 4);
            ws.Cell(value).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell(value).Style.Font.Bold = true;
            ws.Cell(value).Style.Font.FontSize = 11;
            ws.Cell(value).Value = "Bằng chữ: " + new ConvertCurrency().ToUpper((total - (double)db.Orders.Find(id).PriceDiscount.Value) + (double)db.Orders.Find(id).PriceShip.Value);
            ws.Range(valuerange).Row(1).Merge();


            //Ký tên
            ws.Cell("D" + (row + 5)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + (row + 5)).Style.Font.Italic = true;
            ws.Cell("D" + (row + 5)).Value = $"TP.HCM, ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}";
            ws.Range($"D{row + 5}:F{row + 5}").Row(1).Merge();


            ws.Cell("D" + (row + 6)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + (row + 6)).Style.Font.Bold = true;
            ws.Cell("D" + (row + 6)).Value = "Người lập phiếu";
            ws.Range($"D{row + 6}:F{row + 6}").Row(1).Merge();

            ws.Cell("D" + (row + 7)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + (row + 7)).Style.Font.Italic = true;
            ws.Cell("D" + (row + 7)).Value = "(Ký, họ tên, đóng dấu)";
            ws.Range($"D{row + 7}:F{row + 7}").Row(1).Merge();

            ws.Cell("D" + (row + 9)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + (row + 9)).Style.Font.Bold = true;
            ws.Cell("D" + (row + 9)).Value = NguoiLapPhieu(id);
            ws.Range($"D{row + 9}:F{row + 9}").Row(1).Merge();




            string namefile = "HoaDonBanHang-HD" + ChangeIDProduct(id) + " " + DateTime.Now.Ticks + ".xlsx";
            string fileLPath = Server.MapPath(@"~/Data/ExportExcel/" + namefile);
            wb.SaveAs(fileLPath);

            return Json(new { code = 200, NameFile = namefile }, JsonRequestBehavior.AllowGet);

        }
        
        public JsonResult ExportPDFOrder(int id)
        {
            HtmlToPdf htmlToPdf = new HtmlToPdf();

            //set option
            htmlToPdf.Options.PdfPageSize = PdfPageSize.A4;
            htmlToPdf.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
            htmlToPdf.Options.MarginLeft = 30;
            htmlToPdf.Options.MarginRight = 30;
            htmlToPdf.Options.MarginTop = 20;
            htmlToPdf.Options.MarginBottom = 20;

            var db = new DbWebsite();
            var dao = new OrderDetailDAO();
            var model = dao.GetbyId(id);
            List<OrderDetailModel> listorderdetail = new List<OrderDetailModel>();

            foreach (var item in model)
            {
                OrderDetailModel t = new OrderDetailModel(new ProductDAO().DetailProduct(item.ProductID), item.OrderID, item.OrderDetailCount.Value, item.OrderPrice.Value, (item.OrderDetailCount.Value * item.OrderPrice.Value), item.Product.UnitID.HasValue ? db.ProductUnits.Find(item.Product.UnitID.Value).Name : "Combo sản phẩm");
                listorderdetail.Add(t);
            }


            string ret = RenderPartialToString("~/Areas/Admin/Views/Order/ExportPDFOrder.cshtml", listorderdetail, ControllerContext, id,db.Orders.Find(id));

            PdfDocument pdf = htmlToPdf.ConvertHtmlString(ret);


            string namefile = "HoaDonBanHang-HD" + ChangeIDProduct(id) + " " + DateTime.Now.Ticks + ".pdf";
            string fileLPath = Server.MapPath(@"~/Data/ExportPDF/" + namefile);
            pdf.Save(fileLPath);
            pdf.Close();

            return Json(new { code = 200, NameFile = namefile }, JsonRequestBehavior.AllowGet);

        }

        public string RenderPartialToString(string viewName, object model, ControllerContext ControllerContext, int MaHD,Order mOrder)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");
            ViewDataDictionary ViewData = new ViewDataDictionary();
            TempDataDictionary TempData = new TempDataDictionary();
            ViewData.Model = model;
            ViewData["MaHD"] = "HD" + ChangeIDProduct(MaHD);
            ViewData["CreateDate"] = mOrder.CreateDate;
            ViewData["ShipName"] = mOrder.ShipName;
            ViewData["ShipAddress"] = mOrder.ShipAddress;
            ViewData["ShipPhone"] = mOrder.ShipPhone;
            ViewData["UserName"] = NguoiLapPhieu(MaHD);
            ViewData["PriceShip"] = mOrder.PriceShip;
            ViewData["TotalPrice"] = mOrder.TotalPrice;
            ViewData["PriceDiscount"] = mOrder.PriceDiscount;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }

        }

        [HttpPost]
        public JsonResult ExportExcelListOrder(SearchOrder search)
        {
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            var dao = new OrderDAO();

            var model = dao.ListOrder(search, 1, 1000000).ToList();

            var wb = new XLWorkbook();

            var ws = wb.Worksheets.Add("DSHoaDon");
            ws.Column("A").Width = 4.18;
            ws.Column("B").Width = 14;
            ws.Column("C").Width = 16;
            ws.Column("D").Width = 16;
            ws.Column("E").Width = 50;
            ws.Column("F").Width = 13;
            ws.Column("G").Width = 20;
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
            ws.Cell("A3").Value = "DANH SÁCH HÓA ĐƠN BÁN HÀNG";
            ws.Range("A3:G3").Row(1).Merge();

            ws.Cell("A4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A4").Style.Font.Bold = true;
            ws.Cell("A4").Style.Font.FontSize = 11;
            if (search.TuNgay.HasValue && search.DenNgay.HasValue)
            {
                ws.Cell("A4").Value = $"(Từ ngày: {search.TuNgay.Value.Date} - Đến ngày: {search.DenNgay.Value.Date})";
            }
            else if (search.TuNgay.HasValue)
            {
                ws.Cell("A4").Value = $"(Từ ngày: {search.TuNgay.Value.Date})";
            }
            else if (search.DenNgay.HasValue)
            {
                ws.Cell("A4").Value = $"(Đến ngày: {search.DenNgay.Value.Date})";
            }
            else
            {
                ws.Cell("A4").Value = $"(Tất cả hóa đơn)";
            }
            ws.Range("A4:G3").Row(1).Merge();


            var rangeHeader = ws.Range("A5:H5");
            rangeHeader.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangeHeader.Style.Font.Bold = true;
            rangeHeader.Style.Fill.BackgroundColor = XLColor.Orange;
            rangeHeader.Style.Font.FontColor = XLColor.White;
            rangeHeader.Style.Font.FontSize = 12;

            ws.Cell("A5").Value = "STT";

            ws.Cell("B5").Value = "Mã hóa đơn";

            ws.Cell("C5").Value = "Người nhận";
            ws.Cell("D5").Value = "Số điện thoại";
            ws.Cell("E5").Value = "Địa chỉ";
            ws.Cell("F5").Value = "Số lượng";
            ws.Cell("G5").Value = "Tổng tiền";
            ws.Cell("H5").Value = "Người xác nhận";

            int row = 6;
            double total = 0;
            int soluong = 0;
            for (int i = 0; i < model.Count; i++)
            {
                ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("A" + row).Value = model[i].STT;

                ws.Cell("B" + row).Value = "HD" + ChangeIDProduct(model[i].OrderID);

                ws.Cell("C" + row).Value = model[i].ShipName;
                ws.Cell("D" + row).Value = "(+84)"+model[i].ShipPhone;
                ws.Cell("E" + row).Value = model[i].ShipAddress;
                ws.Cell("F" + row).Value = model[i].TotalCount;
                ws.Cell("G" + row).Style.NumberFormat.Format = "#,##0";
                ws.Cell("G" + row).Value = (model[i].TotalPrice);
                ws.Cell("H" + row).Value = model[i].UserName;
                total += (double)(model[i].TotalPrice);
                soluong += model[i].TotalCount.Value;
                row++;
            }

            var rangeborder = ws.Range("A5:H" + (row));
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
            ws.Cell("G" + row).Value = total.ToString("N0")+"VNĐ";

            //tiền chữ
            string value = "A" + (row + 1);
            string valuerange = value + ":H" + (row + 1);
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


            string namefile = "DSHD_" + DateTime.Now.Ticks + ".xlsx";
            string fileLPath = Server.MapPath(@"~/Data/ExportExcel/" + namefile);
            wb.SaveAs(fileLPath);

            return Json(new { code = 200, NameFile = namefile }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ExportPDFListOrder(SearchOrder search)
        {
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            var dao = new OrderDAO();

            var model = dao.ListOrder(search, 1, 1000000).ToList();

            HtmlToPdf htmlToPdf = new HtmlToPdf();

            //set option
            htmlToPdf.Options.PdfPageSize = PdfPageSize.A4;
            htmlToPdf.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
            htmlToPdf.Options.MarginLeft = 30;
            htmlToPdf.Options.MarginRight = 30;
            htmlToPdf.Options.MarginTop = 20;
            htmlToPdf.Options.MarginBottom = 20;
            string date = "";

            if (search.TuNgay.HasValue && search.DenNgay.HasValue)
            {
                date = $"(Từ ngày: {search.TuNgay.Value.Date} - Đến ngày: {search.DenNgay.Value.Date})";
            }
            else if (search.TuNgay.HasValue)
            {
                date = $"(Từ ngày: {search.TuNgay.Value.Date})";
            }
            else if (search.DenNgay.HasValue)
            {
                date = $"(Đến ngày: {search.DenNgay.Value.Date})";
            }
            else
            {
                date = $"(Tất cả hóa đơn bán hàng)";
            }

            string ret = RenderPartialToString("~/Areas/Admin/Views/Order/ExportPDFDSOrder.cshtml", model, ControllerContext, date,session.FullName);

            PdfDocument pdf = htmlToPdf.ConvertHtmlString(ret);


            string namefile = "DSHD_" + DateTime.Now.Ticks +".pdf";
            string fileLPath = Server.MapPath(@"~/Data/ExportPDF/" + namefile);
            pdf.Save(fileLPath);
            pdf.Close();

            return Json(new { code = 200, NameFile = namefile }, JsonRequestBehavior.AllowGet);
        }

        public string RenderPartialToString(string viewName, object model, ControllerContext ControllerContext, string date, string User)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");
            ViewDataDictionary ViewData = new ViewDataDictionary();
            TempDataDictionary TempData = new TempDataDictionary();
            ViewData.Model = model;

            ViewData["Date"] = date;
            ViewData["UserName"] = User;

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

    }
}