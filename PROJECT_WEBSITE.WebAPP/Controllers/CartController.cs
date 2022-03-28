using PROJECT_WEBSITE.Data.DAO;
using PROJECT_WEBSITE.Data.EF;
using PROJECT_WEBSITE.Data.ModelCustom;
using PROJECT_WEBSITE.WebAPP.Common;
using PROJECT_WEBSITE.WebAPP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml.Linq;

namespace PROJECT_WEBSITE.WebAPP.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            DbWebsite db = new DbWebsite();
            
            var lst = db.FooterCategories.ToList();
            ViewBag.ListCategoryFooter = lst;

            var cart = Session[Common.CommonConstants.CART_SESSION];
            var list = new List<CartItem>();
            if (cart != null)
            {
                list = (List<CartItem>)cart;
            }
            var session = (CustomerLogin)Session[Common.CommonConstants.CUSTOMER_SESSION];
            if (session != null)
            {
                var model = new CustomerDAO().GetById(session.Phone);
                ViewBag.User = model.Name;
                ViewBag.Id = model.CustomerID;

                var cart1 = Session["CartSession"];
                if (cart1 != null)
                {
                    var list1 = (List<CartItem>)cart1;
                    if (list1 != null)
                    {
                        XElement xElement = new XElement("Cart");
                        foreach (var item in list1)
                        {
                            XElement cartItem = new XElement("CartItem");
                            XElement Id = new XElement("ID", item.Product.ProductID);
                            XElement Quan = new XElement("Quantity", item.Quantity);
                            cartItem.Add(Id);
                            cartItem.Add(Quan);
                            xElement.Add(cartItem);
                        }
                        CustomerDAO cartupdate = new CustomerDAO();
                        cartupdate.UpdateCart(session.Phone, xElement.ToString());
                    }

                }


            }
            return View(list);
        }

        public ActionResult AddItem(int productId, int quantity)
        {
            

           
            var product = new ProductDAO().DetailProduct(productId);
            var cart = Session[Common.CommonConstants.CART_SESSION];
            if (cart != null)
            {
                var list = (List<CartItem>)cart;
                if (list.Exists(x => x.Product.ProductID == productId))
                {
                    foreach (var item in list)
                    {
                        if (item.Product.ProductID == productId)
                        {
                            item.Quantity += quantity;
                        }
                    }
                }
                else
                {
                    //khởi tạo đối tượng giỏ hàng
                    var item = new CartItem();
                    item.Product = product;
                    item.Quantity = quantity;
                    list.Add(item);
                }
                //Gán list vào ss
                Session[Common.CommonConstants.CART_SESSION] = list;
            }
            else
            {
                //khởi tạo đối tượng giỏ hàng
                var item = new CartItem();
                item.Product = product;
                item.Quantity = quantity;
                var list = new List<CartItem>();
                list.Add(item);
                //Gán list vào session
                Session[Common.CommonConstants.CART_SESSION] = list;
            }


            return RedirectToAction("Index");
        }

        public JsonResult GetCountCart()
        {
            var cart = Session[Common.CommonConstants.CART_SESSION];
            if (cart == null)
            {
                var list1 = new List<CartItem>();
                //Gán list vào session
                Session[Common.CommonConstants.CART_SESSION] = list1;
            }
            var list = new List<CartItem>();
            list = (List<CartItem>)Session[Common.CommonConstants.CART_SESSION];
            if (list != null)
            {
                decimal total = 0;
                foreach (var item in list)
                {
                    decimal price = new decimal();
                    if (item.Product.PricePromotion.HasValue)
                    {
                        price = item.Product.PricePromotion.Value;
                    }
                    else
                    {
                        price = item.Product.PriceOut.Value;

                    }
                    total += (price * item.Quantity);
                }
                return Json(new { code = 200, count = list.Count, total = total }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { code = 500 }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Update(string cartModel)
        {
            var db = new DbWebsite();
            var cart = new JavaScriptSerializer().Deserialize<List<CartItem>>(cartModel);
            var ss = (List<CartItem>)Session[Common.CommonConstants.CART_SESSION];
            foreach (var item in ss)
            {
                var jsonitem = cart.SingleOrDefault(x => x.Product.ProductID == item.Product.ProductID);
                if (jsonitem != null)
                {
                    if (jsonitem.Quantity > db.Products.Find(item.Product.ProductID).CountProduct)
                    {
                        return Json(new { code = 500, msg = $"số lượng sản phẩm{jsonitem.Product.Name} không đủ" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        item.Quantity = jsonitem.Quantity;
                    }
                }
            }

            return Json(new { code = 200, msg = "thành công" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteAll()
        {
            var cart = Session[Common.CommonConstants.CART_SESSION];
            var list = (List<CartItem>)cart;
            list.Clear();
            Session[Common.CommonConstants.CART_SESSION] = list;
            return RedirectToAction("Index");
        }
        public ActionResult DeleteItem(int id)
        {
            var cart = Session[Common.CommonConstants.CART_SESSION];
            var list = (List<CartItem>)cart;
            foreach (var item in list)
            {
                if (item.Product.ProductID == id)
                {
                    list.Remove(item);
                    Session[Common.CommonConstants.CART_SESSION] = list;
                    return RedirectToAction("Index");
                }
            }
            Session[Common.CommonConstants.CART_SESSION] = list;
            return RedirectToAction("Index");
        }

        public ActionResult Payment()
        {
            DbWebsite db = new DbWebsite();

            var lst2 = db.FooterCategories.ToList();

            ViewBag.ListCategoryFooter = lst2;

            var session = (CustomerLogin)Session[Common.CommonConstants.CUSTOMER_SESSION];
            if (session == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var model = new CustomerDAO().GetById(session.Phone);
            ViewBag.User = model.Name;
            ViewBag.Id = model.CustomerID;

            var sessionkhuyenmai = Session[Common.CommonConstants.DISCOUNT_SESSION];
            var lstdiscount = (List<WebAPP.Models.MDiscountCode>)sessionkhuyenmai;

            if (sessionkhuyenmai != null)
            {
                if (lstdiscount.Count != 0 && lstdiscount != null)
                {
                    ViewBag.KhuyenMai = lstdiscount;
                }

            }
            else
            {
                var lst = new List<WebAPP.Models.MDiscountCode>();
                ViewBag.KhuyenMai = lstdiscount;
            }

            ViewBag.Cart = (List<CartItem>)Session[Common.CommonConstants.CART_SESSION];
            return View(model);
        }

        public void InsertOrder(MOrder ordert, bool paymentStatus)
        {
            var session = (CustomerLogin)Session[Common.CommonConstants.CUSTOMER_SESSION];
            var model = new CustomerDAO().GetById(session.Phone);
            var dao = new OrderDAO();
            var id = dao.Insert(ordert, (int)session.CustomerID, session.Phone, paymentStatus);
            var cart = (List<CartItem>)Session[Common.CommonConstants.CART_SESSION];
            var detailDao = new OrderDetailDAO();
            foreach (var item in cart)
            {
                var orderdetail = new OrderDetail();
                orderdetail.ProductID = item.Product.ProductID;
                orderdetail.OrderID = (int)id;
                if (item.Quantity < 10)
                {
                    if (item.Product.PricePromotion.HasValue)
                    {
                        orderdetail.OrderPrice = item.Product.PricePromotion;
                    }
                    else
                    {
                        orderdetail.OrderPrice = item.Product.PriceOut;
                    }
                }
                else
                {
                    if (item.Product.PricewholesalePromotion.HasValue)
                    {
                        orderdetail.OrderPrice = item.Product.PricewholesalePromotion;
                    }
                    else
                    {
                        orderdetail.OrderPrice = item.Product.Pricewholesale;
                    }
                }
                orderdetail.OrderDetailCount = item.Quantity;
                detailDao.Insert(orderdetail, item.Quantity);
            }

            using (var db = new DbWebsite())
            {
                var sessionkhuyenmai = Session[Common.CommonConstants.DISCOUNT_SESSION];
                var lstdiscount = (List<WebAPP.Models.MDiscountCode>)sessionkhuyenmai;

                if (lstdiscount != null)
                {
                    if (lstdiscount.Count != 0)
                    {
                        foreach (var item in lstdiscount)
                        {
                            dao.AddDetailDiscountCode((int)id, item.DiscountID);
                        }
                    }                   
                }

            }

            using (var db = new DbWebsite())
            {
                var order = db.Orders.Find(id);

                var totalprice = db.OrderDetails.Where(t => t.OrderID == id).Sum(t => t.OrderDetailCount.Value * t.OrderPrice.Value);


                var sessionkhuyenmai = Session[Common.CommonConstants.DISCOUNT_SESSION];
                var lstdiscount = (List<WebAPP.Models.MDiscountCode>)sessionkhuyenmai;

                if (lstdiscount != null)
                {
                    if (lstdiscount.Count != 0)
                    {
                        var t = totalprice;
                        order.PriceDiscount = totalprice - GetDiscountPrice(lstdiscount, ref t);

                        foreach (var item in lstdiscount)
                        {
                            var discount = db.DiscountCodes.Find(item.DiscountID);

                            discount.DistcountCount -= 1;
                        }
                    }
                    
                }
                else
                {
                    order.PriceDiscount = 0;
                }

                if (totalprice - order.PriceDiscount < 250000)
                {
                    order.PriceShip = 25000;
                    order.TotalPrice = totalprice;
                }
                else
                {
                    order.PriceShip = 0;
                    order.TotalPrice = totalprice;
                }

                db.SaveChanges();
            }

            Session.Remove(Common.CommonConstants.CART_SESSION);
            Session.Remove(Common.CommonConstants.DISCOUNT_SESSION);

            var clearcart = new CustomerDAO();
            clearcart.ClearCart(session.Phone);
        }
        public ActionResult FinalOrder()
        {
            DbWebsite db = new DbWebsite();
            var lst2 = db.FooterCategories.ToList();

            ViewBag.ListCategoryFooter = lst2;

            if (Request.QueryString.Count > 0)
            {
                string hashSecret = "XOFPHCYYIKHSBRFVRTQENUNVRLSMRMMK"; //Chuỗi bí mật
                var vnpayData = Request.QueryString;
                PayLib pay = new PayLib();

                //lấy toàn bộ dữ liệu được trả về
                foreach (string s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        pay.AddResponseData(s, vnpayData[s]);
                    }
                }

                long orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef")); //mã hóa đơn
                long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo")); //mã giao dịch tại hệ thống VNPAY
                string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode"); //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                string vnp_SecureHash = Request.QueryString["vnp_SecureHash"]; //hash của dữ liệu trả về

                bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret); //check chữ ký đúng hay không?

                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00")
                    {
                        var order = (MOrder)Session[Common.CommonConstants.ORDER_SESSION];

                        InsertOrder(order, true);

                        //Thanh toán thành công                       
                        var session = (CustomerLogin)Session[Common.CommonConstants.CUSTOMER_SESSION];
                        if (session == null)
                        {
                            return RedirectToAction("Index", "Login");
                        }
                        var model = new CustomerDAO().GetById(session.Phone);
                        ViewBag.User = model.Name;
                        ViewBag.Id = model.CustomerID;
                        ViewBag.Message = "Thanh toán thành công hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId;
                    }
                    else if (vnp_ResponseCode == "24")
                    {
                        //Thanh toán không thành công. Mã lỗi: vnp_ResponseCode
                        ViewBag.Message = "Khách hàng hủy giao dịch " + orderId + " | Mã giao dịch: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;
                    }
                    else
                    {
                        //Thanh toán không thành công. Mã lỗi: vnp_ResponseCode
                        ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý " + orderId + " | Mã giao dịch: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;
                    }
                }
                else
                {
                    ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
                }
            }
            return View();
        }

        [HttpPost]
        public JsonResult PaymentCOD(MOrder ordert)
        {
            try
            {
                InsertOrder(ordert, false);

                return Json(new { code = 200, status = true }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {

                return Json(new { code = 200, status = false }, JsonRequestBehavior.AllowGet);
            }

        }


        public ActionResult FinalOrderCOD(MOrder ordert)
        {
            DbWebsite db = new DbWebsite();
            var lst2 = db.FooterCategories.ToList();

            ViewBag.ListCategoryFooter = lst2;
            var session = (CustomerLogin)Session[Common.CommonConstants.CUSTOMER_SESSION];
            if (session != null)
            {
                var model = new CustomerDAO().GetById(session.Phone);
                ViewBag.User = model.Name;
                ViewBag.Id = model.CustomerID;
            }
            return View();
        }

        public JsonResult Paymentonline(MOrder ordert)
        {

            Session.Add(Common.CommonConstants.ORDER_SESSION, ordert);

            string url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
            string returnUrl = "https://localhost:44344/Cart/FinalOrder";
            string tmnCode = "Q6W34VIL";
            string hashSecret = "XOFPHCYYIKHSBRFVRTQENUNVRLSMRMMK";

            PayLib pay = new PayLib();


            pay.AddRequestData("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
            pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            pay.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            pay.AddRequestData("vnp_Amount", GetPriceOrder()); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
            pay.AddRequestData("vnp_BankCode", ""); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
            pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            pay.AddRequestData("vnp_IpAddr", Util.GetIpAddress()); //Địa chỉ IP của khách hàng thực hiện giao dịch
            pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            pay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang "); //Thông tin mô tả nội dung thanh toán
            pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
            pay.AddRequestData("vnp_ReturnUrl", returnUrl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString()); //mã hóa đơn

            string paymentUrl = pay.CreateRequestUrl(url, hashSecret);

            return Json(new { code = 200, PayUrl = paymentUrl }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Tính tổng giá tất cả sản phẩm
        /// </summary>
        /// <returns></returns>
        public string GetPriceOrder()
        {
            decimal pricetotal = 0;

            var cart = (List<CartItem>)Session[Common.CommonConstants.CART_SESSION];

            var sessionkhuyenmai = Session[Common.CommonConstants.DISCOUNT_SESSION];

            var lstdiscount = (List<WebAPP.Models.MDiscountCode>)sessionkhuyenmai;

            if (sessionkhuyenmai != null)
            {
                if (lstdiscount.Count != 0 && lstdiscount != null)
                {
                    foreach (var item in cart)
                    {
                        decimal price = new decimal();
                        if (item.Quantity < 10)
                        {
                            if (item.Product.PricePromotion.HasValue)
                            {
                                price = item.Product.PricePromotion.Value;
                            }
                            else
                            {
                                price = item.Product.PriceOut.Value;

                            }
                        }
                        else
                        {
                            if (item.Product.PricewholesalePromotion.HasValue)
                            {
                                price = item.Product.PricewholesalePromotion.Value;
                            }
                            else
                            {
                                price = item.Product.Pricewholesale.Value;

                            }
                        }

                        pricetotal += price * item.Quantity;
                    }

                    if (lstdiscount != null && lstdiscount.Count() != 0)
                    {
                        foreach (var m in lstdiscount)
                        {
                            if (m.Percent.HasValue)
                            {
                                pricetotal = pricetotal - (pricetotal * (decimal)((double)m.Percent.Value / 100));
                            }
                            else
                            {
                                pricetotal = pricetotal - m.Total.Value;
                            }
                        }

                    }

                    if (pricetotal < 300000)
                    {
                        pricetotal += 25000;
                    }

                    var rs1 = ((int)pricetotal * 100).ToString();
                    return rs1;

                }

            }
            foreach (var item in cart)
            {
                decimal price = new decimal();
                if (item.Quantity < 10)
                {
                    if (item.Product.PricePromotion.HasValue)
                    {
                        price = item.Product.PricePromotion.Value;
                    }
                    else
                    {
                        price = item.Product.PriceOut.Value;

                    }
                }
                else
                {
                    if (item.Product.PricewholesalePromotion.HasValue)
                    {
                        price = item.Product.PricewholesalePromotion.Value;
                    }
                    else
                    {
                        price = item.Product.Pricewholesale.Value;

                    }
                }

                pricetotal += price * item.Quantity;
            }
            if (pricetotal < 300000)
            {
                pricetotal += 25000;
            }
            var rs = ((int)pricetotal * 100).ToString();
            return rs;
        }

        [HttpPost]
        public JsonResult AddMaKhuyenMai(string MaKhuyenMai, decimal TotalPrice)
        {
            var db = new DbWebsite();
            var check = db.DiscountCodes.Where(t => t.Name == MaKhuyenMai&&t.DistcountCount!=0&&t.DiscountStatus==true);
            if (check.Count() != 0)
            {
                var session = Session[Common.CommonConstants.DISCOUNT_SESSION];
                var lstdiscount = (List<WebAPP.Models.MDiscountCode>)session;
                if (lstdiscount != null)
                {
                    foreach (var item in lstdiscount)
                    {
                        if (item.Name == MaKhuyenMai)
                        {
                            return Json(new { code = 200, msg = "Đã áp dụng mã này", data = lstdiscount, status = true, total = -1 }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    var m = new WebAPP.Models.MDiscountCode();
                    m.DiscountID = check.FirstOrDefault().DiscountCodeID;
                    m.Name = MaKhuyenMai;
                    m.Percent = check.FirstOrDefault().PercentCart;
                    m.Total = check.FirstOrDefault().TotalCart;
                    lstdiscount.Add(m);
                    Session[Common.CommonConstants.DISCOUNT_SESSION] = lstdiscount;
                    var total = TotalPrice;
                    if (m.Percent.HasValue)
                    {
                        total = TotalPrice - (TotalPrice * (decimal)((double)m.Percent.Value / 100));
                    }
                    else
                    {
                        total = total - m.Total.Value;
                    }
                    return Json(new { code = 200, msg = "thành công", data = lstdiscount, status = true, total = total }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var lst = new List<WebAPP.Models.MDiscountCode>();
                    var m = new WebAPP.Models.MDiscountCode();
                    m.DiscountID = check.FirstOrDefault().DiscountCodeID;
                    m.Name = MaKhuyenMai;
                    m.Percent = check.FirstOrDefault().PercentCart;
                    m.Total = check.FirstOrDefault().TotalCart;
                    lst.Add(m);
                    Session[Common.CommonConstants.DISCOUNT_SESSION] = lst;
                    decimal total = 0;
                    if (m.Percent.HasValue)
                    {
                        total = TotalPrice - (TotalPrice * (decimal)((double)m.Percent.Value / 100));
                    }
                    else
                    {
                        total = TotalPrice - m.Total.Value;
                    }
                    return Json(new { code = 200, msg = "thành công", data = lst, status = true, total = total }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { code = 200, msg = "không có mã này hoặc đã hết", status = false, total = -1 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteMaKhuyenMai(int id, decimal TotalPrice, decimal Old)
        {
            var session = Session[Common.CommonConstants.DISCOUNT_SESSION];
            var lstdiscount = (List<WebAPP.Models.MDiscountCode>)session;

            var countcode = lstdiscount.Where(t => t.DiscountID == id).FirstOrDefault();

            var total = TotalPrice;
            if (countcode.Percent.HasValue)
            {
                total = TotalPrice + (TotalPrice * (decimal)((double)countcode.Percent.Value / 100));
            }
            else
            {
                total = total + countcode.Total.Value;
            }

            lstdiscount.Remove(lstdiscount.Where(t => t.DiscountID == id).FirstOrDefault());

            Session[Common.CommonConstants.DISCOUNT_SESSION] = lstdiscount;

            if (lstdiscount.Count() == 0)
            {
                total = Old;
            }

            return Json(new { code = 200, msg = "thành công", data = lstdiscount, status = true, total = total }, JsonRequestBehavior.AllowGet);
        }


        public decimal? GetDiscountPrice(List<WebAPP.Models.MDiscountCode> lst, ref decimal totalPrice)
        {
            foreach (var m in lst)
            {
                if (m.Percent.HasValue)
                {
                    totalPrice = totalPrice - (totalPrice * (decimal)((double)m.Percent.Value / 100));
                }
                else
                {
                    totalPrice = totalPrice - m.Total.Value;
                }
            }
            return totalPrice;
        }

    }
}