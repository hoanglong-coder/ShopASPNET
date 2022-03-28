using PROJECT_WEBSITE.Data.ModelCustom;
using PROJECT_WEBSITE.WebAPP.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PROJECT_WEBSITE.Data.EF;
using PROJECT_WEBSITE.Data.DAO;
using PROJECT_WEBSITE.WebAPP.Common;
using PagedList;
using ClosedXML.Excel;
using SelectPdf;
using System.Text;
using System.IO;
using System.Web.UI;

namespace PROJECT_WEBSITE.WebAPP.Areas.Admin.Controllers
{
    public class ReceiptController : RoleNhapHangController
    {

        DbWebsite db = new DbWebsite();

        public static string RECEIPT_SESSION = "RECEIPT_SESSION";

        // GET: Admin/Receipt
        public ActionResult Index()
        {

            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            ViewBag.UserName = session.FullName;

            ViewBag.Role = session.RoleString;

            return View();
        }

        public JsonResult AddProductReceipt(int idproduct, decimal inputprice, int countproduct)
        {

            var Receipt = Session[RECEIPT_SESSION];

            if (Receipt != null)
            {
                var lst = (List<ReceiptModel>)Receipt;

                foreach (var item1 in lst)
                {
                    if (item1.Productid == idproduct)
                    {

                        var result1 = new List<ReceiptModel>();

                        foreach (var item in lst)
                        {
                            ReceiptModel m = new ReceiptModel();
                            m.STT = lst.IndexOf(item) + 1;
                            m.Productid = item.Productid;
                            m.ProductName = item.ProductName;
                            m.PriceIput = item.PriceIput;
                            m.ReceiptCount = item.ReceiptCount;
                            m.Total = item.Total;
                            m.KyTuCaterory = item.KyTuCaterory;
                            m.NameCategory = item.NameCategory;
                            result1.Add(m);
                        }

                        return Json(new { code = 200, data = result1, flag = false }, JsonRequestBehavior.AllowGet);
                    }
                }
                var idcateogry = db.Products.Find(idproduct).ProductCategoryID;
                var rc = new ReceiptModel();
                rc.Productid = idproduct;
                rc.PriceIput = inputprice;
                rc.ReceiptCount = countproduct;
                rc.ProductName = new ProductDAO().GetNameProduct(idproduct);
                rc.Total = rc.PriceIput * rc.ReceiptCount;
                rc.NameCategory = db.ProductCategories.Where(e => e.ProductCategoryID == idcateogry).FirstOrDefault().Name;
                rc.KyTuCaterory = getnamecategorykytu(rc.NameCategory);
                lst.Add(rc);

                Session[RECEIPT_SESSION] = lst;

                var result = new List<ReceiptModel>();

                foreach (var item in lst)
                {
                    ReceiptModel m = new ReceiptModel();
                    m.STT = lst.IndexOf(item) + 1;
                    m.Productid = item.Productid;
                    m.ProductName = item.ProductName;
                    m.PriceIput = item.PriceIput;
                    m.ReceiptCount = item.ReceiptCount;
                    m.Total = item.Total;
                    m.KyTuCaterory = item.KyTuCaterory;
                    m.NameCategory = item.NameCategory;
                    result.Add(m);
                }

                return Json(new { code = 200, data = result, flag = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var idcateogry = db.Products.Find(idproduct).ProductCategoryID;
                var rc = new ReceiptModel();
                rc.Productid = idproduct;
                rc.PriceIput = inputprice;
                rc.ReceiptCount = countproduct;
                rc.ProductName = new ProductDAO().GetNameProduct(idproduct);
                rc.Total = rc.PriceIput * rc.ReceiptCount;
                rc.NameCategory = db.ProductCategories.Where(e => e.ProductCategoryID == idcateogry).FirstOrDefault().Name;
                rc.KyTuCaterory = getnamecategorykytu(rc.NameCategory);

                var list = new List<ReceiptModel>();
                list.Add(rc);

                Session[RECEIPT_SESSION] = list;

                var result = new List<ReceiptModel>();

                foreach (var item in list)
                {
                    ReceiptModel m = new ReceiptModel();
                    m.STT = list.IndexOf(item) + 1;
                    m.Productid = item.Productid;
                    m.ProductName = item.ProductName;
                    m.PriceIput = item.PriceIput;
                    m.ReceiptCount = item.ReceiptCount;
                    m.Total = item.Total;
                    m.KyTuCaterory = item.KyTuCaterory;
                    m.NameCategory = item.NameCategory;
                    result.Add(m);
                }

                return Json(new { code = 200, data = result, flag = true }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult DeleteProductReceipt(int idproduct)
        {

            var Receipt = Session[RECEIPT_SESSION];

            var lst = (List<ReceiptModel>)Receipt;

            lst.Remove(lst.Where(t => t.Productid == idproduct).FirstOrDefault());

            Session[RECEIPT_SESSION] = lst;

            var result = new List<ReceiptModel>();

            foreach (var item in lst)
            {
                ReceiptModel m = new ReceiptModel();
                m.STT = lst.IndexOf(item) + 1;
                m.Productid = item.Productid;
                m.ProductName = item.ProductName;
                m.PriceIput = item.PriceIput;
                m.ReceiptCount = item.ReceiptCount;
                m.Total = item.Total;
                m.KyTuCaterory = item.KyTuCaterory;
                m.NameCategory = item.NameCategory;
                result.Add(m);
            }

            return Json(new { code = 200, data = result, flag = true }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult DetailProductReceipt(int idproduct)
        {
            var Receipt = Session[RECEIPT_SESSION];

            var lst = (List<ReceiptModel>)Receipt;

            var item = lst.Where(t => t.Productid == idproduct).FirstOrDefault();

            ReceiptModel m = new ReceiptModel();
            m.Productid = item.Productid;
            m.PriceIput = item.PriceIput;
            m.ReceiptCount = item.ReceiptCount;

            return Json(new { code = 200, data = m }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EditProductReceipt(int idproduct, decimal inputprice, int countproduct)
        {
            var Receipt = Session[RECEIPT_SESSION];

            var lst = (List<ReceiptModel>)Receipt;

            var item1 = lst.Where(t => t.Productid == idproduct).FirstOrDefault();
            item1.PriceIput = inputprice;
            item1.ReceiptCount = countproduct;          

            var result = new List<ReceiptModel>();

            foreach (var item in lst)
            {
                ReceiptModel m = new ReceiptModel();
                m.STT = lst.IndexOf(item) + 1;
                m.Productid = item.Productid;
                m.ProductName = item.ProductName;
                m.PriceIput = item.PriceIput;
                m.ReceiptCount = item.ReceiptCount;
                m.Total = item.PriceIput * item.ReceiptCount;
                m.KyTuCaterory = item.KyTuCaterory;
                m.NameCategory = item.NameCategory;
                result.Add(m);
            }
            Session[RECEIPT_SESSION] = result;
            return Json(new { code = 200, data = result, flag = true }, JsonRequestBehavior.AllowGet);
        }

        public string getnamecategorykytu(string input)
        {
            string[] arraystring = input.Split(' ');
            string KyTu1 = "";
            string KyTu2 = "";

            if (arraystring.Length > 2)
            {
                KyTu1 = arraystring[0].Substring(0, 1);
                KyTu2 = arraystring[1].Substring(0, 1);
            }
            else
            {
                KyTu1 = arraystring[0].Substring(0, 1);
            }

            return KyTu1.ToUpper() + KyTu2.ToUpper();
        }

        [HttpPost]
        public JsonResult CreateReceipt(MReceipt mReceipt)
        {
            var Receipt = Session[RECEIPT_SESSION];
            var lst = (List<ReceiptModel>)Receipt;

            if (lst != null)
            {
                var dao = new ReceiptDAO();

                var mreceiptdetail = new List<MReceiptDetail>();


                foreach (var item in lst)
                {
                    MReceiptDetail m = new MReceiptDetail();
                    m.ProductID = item.Productid;
                    m.PriceIput = item.PriceIput;
                    m.ReceiptCount = item.ReceiptCount;

                    mreceiptdetail.Add(m);

                }
                var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];
                mReceipt.UserID = session.UserID;
                var check = dao.CreateReceipt(mReceipt, mreceiptdetail);

                if (check == true)
                {
                    Session.Remove(RECEIPT_SESSION);
                }

                return Json(new { code = 200, data = check, flag = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { code = 200, flag = false }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult GetAll(SearchNews search, int page=1,int pageSize = 2)
        {
            var dao = new ReceiptDAO();

            IPagedList model = (IPagedList)dao.GetAll(search, page, pageSize);

            var lstreceipt = dao.GetAll(search, page, pageSize);

            int c = model.TotalItemCount;


            var lsttotal = dao.GetAllTotal(search);

            return Json(new { code = 200, data = model, total = c, Totalprice = lsttotal.Sum(t => t.TotalReceiptPrice), TotalCount = lsttotal.Sum(t => t.TotalCount) }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GetReceiptDetailsByID(int id)
        {
            var dao = new ReceiptDAO();

            var lstreceipt = dao.GetReceiptDetailsByID(id);
            var supplierid = db.Receipts.Find(id).SupplierID;

            var Ten = db.ProductSuppliers.Find(supplierid).Name;

            var createdate = db.Receipts.Find(id).CreateDate;

            var nameuser = db.Users.Find(db.Receipts.Find(id).UserID).FullName;

            return Json(new { code = 200, data = lstreceipt, SupplierName = Ten, CreateDate = createdate, NameUser = nameuser, TotalPrice = lstreceipt.Sum(t => t.TotalPrice), CountProduct = lstreceipt.Sum(t => t.ReceiptCount) }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Xuất file excel các sản phẩm trong phiếu nhập theo
        /// </summary>
        /// <param name="id">Mã phiếu nhập</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ExportExcelProductReceipt(int id)
        {

            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            var dao = new ReceiptDAO();

            var lstreceipt = dao.GetReceiptDetailsByID(id);

            var supplierid = db.Receipts.Find(id).SupplierID;

            var Ten = db.ProductSuppliers.Find(supplierid).Name;

            var createdate = db.Receipts.Find(id).CreateDate;

            var nameuser = db.Users.Find(db.Receipts.Find(id).UserID).FullName;


            var wb = new XLWorkbook();

            var ws = wb.Worksheets.Add("PhieuNhap");
            ws.Rows().AdjustToContents();
            ws.Column("B").Width = 50;
            ws.Column("C").Width = 11;
            ws.Column("E").Width = 15;
            ws.Column("F").Width = 16;


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
            ws.Cell("A3").Value = "PHIẾU NHẬP HÀNG";
            ws.Range("A3:F3").Row(1).Merge();

            ws.Cell("A4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A4").Style.Font.Bold = true;
            ws.Cell("A4").Style.Font.FontSize = 11;
            ws.Cell("A4").Value = $"(Mã phiếu nhập: PN{ChangeIDProduct(id)} - Ngày: {createdate} - Lập phiếu: {nameuser})";
            ws.Range("A4:F3").Row(1).Merge();

            ws.Cell("A5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell("A5").Style.Font.Bold = true;
            ws.Cell("A5").Style.Font.FontSize = 11;
            ws.Cell("A5").Value = $"Nhà cung cấp: {Ten} - ĐT: {db.ProductSuppliers.Find(supplierid).Phone} - ĐC: {db.ProductSuppliers.Find(supplierid).Address}";
            ws.Range("A5:F3").Row(1).Merge();


            var rangeHeader = ws.Range("A6:F6");
            rangeHeader.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangeHeader.Style.Font.Bold = true;
            rangeHeader.Style.Fill.BackgroundColor = XLColor.Orange;
            rangeHeader.Style.Font.FontColor = XLColor.White;
            rangeHeader.Style.Font.FontSize = 12;
            

            ws.Cell("A6").Value = "STT";

            ws.Cell("B6").Value = "Tên hàng hóa";

            ws.Cell("C6").Value = "ĐVT";
            ws.Cell("D6").Value = "SL";
            ws.Cell("E6").Value = "Đơn giá(VNĐ)";
            ws.Cell("F6").Value = "Thành tiền(VNĐ)";

            int row = 7;
            double total = 0;
            int soluong = 0;
            for (int i = 0; i < lstreceipt.Count; i++)
            {
                ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("A"+row).Value = lstreceipt[i].STT;

                ws.Cell("B" + row).Value = lstreceipt[i].ProductName;

                ws.Cell("C" + row).Value = lstreceipt[i].ProductDVT;
                ws.Cell("D" + row).Value = lstreceipt[i].ReceiptCount;
                ws.Cell("E" + row).Style.NumberFormat.Format = "#,##0";
                ws.Cell("E" + row).Value = lstreceipt[i].PriceIput.Value;
                ws.Cell("F" + row).Style.NumberFormat.Format = "#,##0";
                ws.Cell("F" + row).Value = lstreceipt[i].TotalPrice.Value;
                total += (double)lstreceipt[i].TotalPrice.Value;
                soluong += lstreceipt[i].ReceiptCount.Value;
                row++;
            }

            var rangeborder = ws.Range("A6:F"+(row-1));
            rangeborder.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rangeborder.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            //Số lượng
            ws.Cell("C" + row).Style.Font.Bold = true;
            ws.Cell("C" + row).Value = "Số lượng:";

            ws.Cell("D" + row).Style.Font.Bold = true;
            ws.Cell("D" + row).Value = soluong;



            //Tổng tiền
            ws.Cell("E" + row).Style.Font.Bold = true;
            ws.Cell("E" + row).Value = "Tổng tiền hàng:";


            ws.Cell("F" + row).Style.NumberFormat.Format = "#,##0";
            ws.Cell("F" + row).Style.Font.Bold = true;
            ws.Cell("F" + row).Value = total;


            //tiền chữ
            string value= "A" + (row + 1);
            string valuerange = value + ":F" + (row + 1);
            ws.Cell(value).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell(value).Style.Font.Bold = true;
            ws.Cell(value).Style.Font.FontSize = 11;
            ws.Cell(value).Value = "Bằng chữ: "+ new ConvertCurrency().ToUpper(total);
            ws.Range(valuerange).Row(1).Merge();

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


            string namefile = "PhieuNhap-PN" + ChangeIDProduct(id) + " " + DateTime.Now.Ticks + ".xlsx";
            string fileLPath = Server.MapPath(@"~/Data/ExportExcel/"+ namefile);
            wb.SaveAs(fileLPath);

            return Json(new { code = 200, NameFile = namefile }, JsonRequestBehavior.AllowGet);
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
        [HttpPost]
        public JsonResult ExportPDFProductReceipt(int id)
        {
            HtmlToPdf htmlToPdf = new HtmlToPdf();

            //set option
            htmlToPdf.Options.PdfPageSize = PdfPageSize.A4;
            htmlToPdf.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
            htmlToPdf.Options.MarginLeft = 30;
            htmlToPdf.Options.MarginRight = 30;
            htmlToPdf.Options.MarginTop = 20;
            htmlToPdf.Options.MarginBottom = 20;

            var dao = new ReceiptDAO();

            var lstreceipt = dao.GetReceiptDetailsByID(id);

            var supplierid = db.Receipts.Find(id).SupplierID;

            var Ten = db.ProductSuppliers.Find(supplierid).Name;

            var createdate = db.Receipts.Find(id).CreateDate;

            var nameuser = db.Users.Find(db.Receipts.Find(id).UserID).FullName;


            string ret = RenderPartialToString("~/Areas/Admin/Views/Receipt/ExportPDF.cshtml", lstreceipt,ControllerContext,id,Ten, db.ProductSuppliers.Find(supplierid).Phone, db.ProductSuppliers.Find(supplierid).Address,createdate, nameuser);

            PdfDocument pdf = htmlToPdf.ConvertHtmlString(ret);

            
            string namefile = "PhieuNhap-PN" + ChangeIDProduct(id) + " " + DateTime.Now.Ticks + ".pdf";
            string fileLPath = Server.MapPath(@"~/Data/ExportPDF/" + namefile);
            pdf.Save(fileLPath);
            pdf.Close();
            
            return Json(new { code = 200, NameFile = namefile }, JsonRequestBehavior.AllowGet);

        }
        public string RenderPartialToString(string viewName, object model, ControllerContext ControllerContext,int MaPN,string TenNCC,string DT, string Adress, DateTime? NgayTao,string username)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");
            ViewDataDictionary ViewData = new ViewDataDictionary();
            TempDataDictionary TempData = new TempDataDictionary();
            ViewData.Model = model;
            ViewData["MaPN"] = "PN" + ChangeIDProduct(MaPN);
            ViewData["TenNCC"] = TenNCC;
            ViewData["DT"] = DT;
            ViewData["DiaChi"] = Adress;
            ViewData["NgayLap"] = NgayTao;
            ViewData["UserName"] = username;


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


        /// <summary>
        /// Xuất Excel danh sách phiếu nhập nhập hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ExportExcelReceipt(SearchNews search)
        {
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            var dao = new ReceiptDAO();

            var model = dao.GetAll(search, 1, 1000000).ToList();

            var wb = new XLWorkbook();

            var ws = wb.Worksheets.Add("DSPhieuNhap");
            ws.Rows().AdjustToContents();
            ws.Column("A").Width = 4.18;
            ws.Column("B").Width = 14;
            ws.Column("C").Width = 16;
            ws.Column("D").Width = 50;
            ws.Column("E").Width = 13;
            ws.Column("F").Width = 16;
            ws.Column("G").Width = 20;

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
            ws.Cell("A3").Value = "DANH SÁCH PHIẾU NHẬP HÀNG";
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
                ws.Cell("A4").Value = $"(Tất cả phiếu nhập hàng)";
            }
            ws.Range("A4:G3").Row(1).Merge();


            var rangeHeader = ws.Range("A5:G5");
            rangeHeader.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangeHeader.Style.Font.Bold = true;
            rangeHeader.Style.Fill.BackgroundColor = XLColor.Orange;
            rangeHeader.Style.Font.FontColor = XLColor.White;
            rangeHeader.Style.Font.FontSize = 12;

            ws.Cell("A5").Value = "STT";

            ws.Cell("B5").Value = "Mã phiếu nhập";

            ws.Cell("C5").Value = "Ngày nhập";
            ws.Cell("D5").Value = "Nhà cung cấp";
            ws.Cell("E5").Value = "Số lượng";
            ws.Cell("F5").Value = "Tổng tiền(VNĐ)";
            ws.Cell("G5").Value = "Người nhập";

            int row = 6;
            double total = 0;
            int soluong = 0;
            for (int i = 0; i < model.Count; i++)
            {
                ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("A" + row).Value = model[i].STT;

                ws.Cell("B" + row).Value = "PN"+ChangeIDProduct(model[i].ReceiptID);

                ws.Cell("C" + row).Value = model[i].CreateDate;
                ws.Cell("D" + row).Value = model[i].SupplierName;
                ws.Cell("E" + row).Value = model[i].TotalCount.Value;
                ws.Cell("F" + row).Style.NumberFormat.Format = "#,##0";
                ws.Cell("F" + row).Value = model[i].TotalReceiptPrice.Value;
                ws.Cell("G" + row).Value = model[i].UserName;
                total += (double)model[i].TotalReceiptPrice.Value;
                soluong += model[i].TotalCount.Value;
                row++;
            }

            var rangeborder = ws.Range("A5:G" + (row));
            rangeborder.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rangeborder.Style.Border.InsideBorder = XLBorderStyleValues.Thin;


            ws.Cell("B"+row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B"+row).Style.Font.Bold = true;
            ws.Cell("B"+row).Value = "Tổng cộng";
            ws.Range($"B{row}:D{row}").Row(1).Merge();

            ws.Cell("E" + row).Style.Font.Bold = true;
            ws.Cell("E" + row).Value = soluong;

            ws.Cell("F" + row).Style.NumberFormat.Format = "#,##0";
            ws.Cell("F" + row).Style.Font.Bold = true;
            ws.Cell("F" + row).Value = total;

            //tiền chữ
            string value = "A" + (row + 1);
            string valuerange = value + ":G" + (row + 1);
            ws.Cell(value).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell(value).Style.Font.Bold = true;
            ws.Cell(value).Style.Font.FontSize = 11;
            ws.Cell(value).Value = "Bằng chữ: " + new ConvertCurrency().ToUpper(total);
            ws.Range(valuerange).Row(1).Merge();


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

            string namefile = "DSPhieuNhap_" + DateTime.Now.Ticks + ".xlsx";
            string fileLPath = Server.MapPath(@"~/Data/ExportExcel/" + namefile);
            wb.SaveAs(fileLPath);

            return Json(new { code = 200, NameFile = namefile }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Xuất Pdf danh sách phiếu nhập hàng
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ExportPDFReceipt(SearchNews search)
        {

            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];

            var dao = new ReceiptDAO();

            var model = dao.GetAll(search, 1, 1000000).ToList();

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
                date = $"(Tất cả phiếu nhập hàng)";
            }

            string ret = RenderPartialToString("~/Areas/Admin/Views/Receipt/ExportPDFReceipt.cshtml", model,ControllerContext,date,NguoiLapPhieu(session.UserID));

            PdfDocument pdf = htmlToPdf.ConvertHtmlString(ret);


            string namefile = "DSPhieuNhap_" + DateTime.Now.Ticks + ".pdf";
            string fileLPath = Server.MapPath(@"~/Data/ExportPDF/" + namefile);
            pdf.Save(fileLPath);
            pdf.Close();

            return Json(new { code = 200, NameFile = namefile }, JsonRequestBehavior.AllowGet);
        }
        public string RenderPartialToString(string viewName, object model, ControllerContext ControllerContext,string date, string username)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");
            ViewDataDictionary ViewData = new ViewDataDictionary();
            TempDataDictionary TempData = new TempDataDictionary();
            ViewData.Model = model;
            ViewData["Date"] = date;
            ViewData["Username"] = username;


            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }

        }
    }
}