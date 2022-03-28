using PagedList;
using PROJECT_WEBSITE.Data.EF;
using PROJECT_WEBSITE.Data.ModelCustom;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJECT_WEBSITE.Data.DAO
{
    public class ThongKeDAO
    {
        DbWebsite db;

        public ThongKeDAO()
        {
            db = new DbWebsite();
        }

        //Thống kê doanh số
        public IEnumerable<MThongKeThang> GetThongKeThang(SearchThongKe search, int page, int pageSize)
        {
            var lst = db.Orders.Where(t => t.UserID.HasValue).OrderBy(x => x.OrderID).GroupBy(t => DbFunctions.TruncateTime(t.CreateDate.Value));

            var lstrs1 = lst.Select(t => t).AsQueryable();

            if (search.TuNgay.HasValue)
            {
                lstrs1 = lstrs1.Where(t => DateTime.Compare(search.TuNgay.Value, t.Key.Value) <= 0);
            }
            if (search.DenNgay.HasValue)
            {
                lstrs1 = lstrs1.Where(t => DateTime.Compare(t.Key.Value, search.DenNgay.Value) <= 0);
            }


            var lstrs = new List<MThongKeThang>();

            foreach (var item in lstrs1)
            {
                MThongKeThang m = new MThongKeThang();
                m.STT = lstrs1.ToList().FindIndex(t => t.Key == item.Key) + 1;
                m.Ngay = item.Key;
                m.SoDonHang = item.Count();
                m.TienHang = item.Sum(t => (t.TotalPrice - t.PriceDiscount) + t.PriceShip);
                m.GiamGia = item.Sum(t=>t.PriceDiscount);
                m.ShipPrice = item.Sum(t => t.PriceShip);
                var soluong = 0;
                var lst1 = item.Select(t => t.OrderID);

                foreach (var item1 in lst1)
                {
                    soluong += demsl(item1);
                }
                m.SoLuong = soluong;

                lstrs.Add(m);
            }


            return lstrs.ToPagedList(page, pageSize);
        }

        public List<MThongKeThang> GetThongKeThangTotal(SearchThongKe search)
        {
            var lst = db.Orders.Where(t => t.UserID.HasValue).OrderBy(x => x.OrderID).GroupBy(t => DbFunctions.TruncateTime(t.CreateDate.Value));

            var lstrs1 = lst.Select(t => t).AsQueryable();

            if (search.TuNgay.HasValue)
            {
                lstrs1 = lstrs1.Where(t => DateTime.Compare(search.TuNgay.Value, t.Key.Value) < 0);
            }
            if (search.DenNgay.HasValue)
            {
                lstrs1 = lstrs1.Where(t => DateTime.Compare(t.Key.Value, search.DenNgay.Value) < 0);
            }


            var lstrs = new List<MThongKeThang>();

            foreach (var item in lstrs1)
            {
                MThongKeThang m = new MThongKeThang();
                m.STT = lstrs1.ToList().FindIndex(t => t.Key == item.Key) + 1;
                m.Ngay = item.Key;
                m.SoDonHang = item.Count();
                m.TienHang = item.Sum(t => (t.TotalPrice - t.PriceDiscount) + t.PriceShip);

                var soluong = 0;
                var lst1 = item.Select(t => t.OrderID);

                foreach (var item1 in lst1)
                {
                    soluong += demsl(item1);
                }
                m.SoLuong = soluong;

                lstrs.Add(m);
            }


            return lstrs;
        }

        /// <summary>
        /// Hàm lấy danh sách đơn hàng
        /// </summary>
        /// <param name="page">trang hiện tại</param>
        /// <param name="pageSize">tổng số record 1 trang</param>
        /// <returns>Danh sách đơn hàng 1 trang</returns>
        public IEnumerable<MOrder> ListOrderTheoNgay(SearchThongKe search, DateTime ngaytao, int page, int pageSize)
        {
            var lst = db.Orders.Where(t => DbFunctions.TruncateTime(t.CreateDate.Value) == ngaytao && t.UserID.HasValue).OrderBy(x => x.OrderID).Select(t => t).OrderByDescending(t => t.CreateDate).AsQueryable();

            var lstrs = new List<MOrder>();


            foreach (var item in lst)
            {
                MOrder m = new MOrder();
                m.STT = lst.ToList().IndexOf(item) + 1;
                m.OrderID = item.OrderID;
                m.CreateDate = item.CreateDate;
                m.CustomerID = item.CustomerID;
                m.ShipName = item.ShipName;
                m.ShipAddress = CutAddress(item.ShipAddress).Trim('/');
                m.ShipEmail = item.ShipEmail;
                m.ShipPhone = item.ShipPhone;
                m.CreateDate = item.CreateDate;
                m.Discription = item.Discription;
                m.PaymentStatus = item.PaymentStatus;
                m.TotalPrice = (item.TotalPrice - item.PriceDiscount) + item.PriceShip;
                m.OrderStatus = item.OrderStatus;
                m.PriceShip = item.PriceShip;
                m.PriceDiscount = item.PriceDiscount;
                m.TotalCount = db.OrderDetails.Where(t => t.OrderID == m.OrderID).Sum(t => t.OrderDetailCount);
                m.UserID = item.UserID;
                m.UserName = UserName(m.UserID);
                m.PaymentName = Payment(item.PaymentStatus);
                lstrs.Add(m);
            }

            var lstrs1 = lstrs.Select(t => t).AsQueryable();

            if (search.TuNgay.HasValue)
            {
                lstrs1 = lstrs1.Where(t => DateTime.Compare(search.TuNgay.Value, (DateTime)DbFunctions.TruncateTime(t.CreateDate.Value)) <= 0);
            }
            if (search.DenNgay.HasValue)
            {
                lstrs1 = lstrs1.Where(t => DateTime.Compare((DateTime)DbFunctions.TruncateTime(t.CreateDate.Value), search.DenNgay.Value) <= 0);
            }


            return lstrs1.ToPagedList(page, pageSize);
        }

        public string CutAddress(string input)
        {
            return input.Replace('+', '/');
        }

        public string Payment(bool? input)
        {
            if (input.HasValue)
            {
                if (input.Value)
                {
                    return "Thanh toán online";
                }
                else
                {
                    return "Thanh toán COD";
                }
            }
            return "";
        }

        public string UserName(int? userid)
        {
            if (userid.HasValue)
            {
                return db.UserRoleGroups.Where(t => t.UserRoleGroupID == db.Users.Where(e => e.UserID == userid.Value).FirstOrDefault().UserRoleGroupID).FirstOrDefault().Name + " - " + db.Users.Where(t => t.UserID == userid.Value).FirstOrDefault().FullName;
            }
            return "";
        }

        public int? CutMaHD(string input)
        {
            try
            {
                var rs = int.Parse(input.Substring(2));

                return rs;
            }
            catch (Exception)
            {

                return null;
            }

        }

        public int demsl(int idorder)
        {
            var sl = db.OrderDetails.Where(t => t.OrderID == idorder);

            if (sl.Count() != 0)
            {
                return sl.Sum(t => t.OrderDetailCount).Value;
            }
            return 0;
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
        /// Thống kê theo nhân viên
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<MThongKeNhanVien> GetThongKeNhanVien(SearchThongKe search, int page, int pageSize)
        {
            var lst = db.Orders.Where(t => t.UserID.HasValue).AsQueryable();

            var lstrs1 = lst.Select(t => t).AsQueryable();

            if (search.TuNgay.HasValue)
            {
                lstrs1 = lstrs1.Where(t => DateTime.Compare(search.TuNgay.Value, (DateTime)DbFunctions.TruncateTime(t.CreateDate.Value)) <= 0);
            }
            if (search.DenNgay.HasValue)
            {
                lstrs1 = lstrs1.Where(t => DateTime.Compare((DateTime)DbFunctions.TruncateTime(t.CreateDate.Value), search.DenNgay.Value) <= 0);
            }

            var lstrs = new List<MThongKeNhanVien>();

            foreach (var item in lstrs1.OrderBy(x => x.OrderID).GroupBy(t => t.UserID))
            {
                MThongKeNhanVien m = new MThongKeNhanVien();
                m.STT = lstrs1.OrderBy(x => x.OrderID).GroupBy(t => t.UserID).ToList().FindIndex(t => t.Key == item.Key) + 1;
                m.UserID = item.Key.Value;
                m.UserName = db.Users.Find(item.Key.Value).FullName;
                m.SoDonHang = item.Count();
                m.TienHang = item.Sum(t => (t.TotalPrice - t.PriceDiscount) + t.PriceShip);

                var soluong = 0;
                var lst1 = item.Select(t => t.OrderID);

                foreach (var item1 in lst1)
                {
                    soluong += demsl(item1);
                }
                m.SoLuong = soluong;

                lstrs.Add(m);
            }

            return lstrs.ToPagedList(page, pageSize);
        }

        /// <summary>
        /// Hàm lấy danh sách đơn hàng theo nhân viên
        /// </summary>
        /// <param name="page">trang hiện tại</param>
        /// <param name="pageSize">tổng số record 1 trang</param>
        /// <returns>Danh sách đơn hàng 1 trang</returns>
        public IEnumerable<MOrder> ListOrderTheoNhanVien(SearchThongKe search, int id, int page, int pageSize)
        {
            var lst = db.Orders.Where(t => t.UserID == id).Select(t => t).OrderByDescending(t => t.CreateDate).AsQueryable();

            var lstrs1 = lst.Select(t => t).AsQueryable();

            if (search.TuNgay.HasValue)
            {
                lstrs1 = lstrs1.Where(t => DateTime.Compare(search.TuNgay.Value, (DateTime)DbFunctions.TruncateTime(t.CreateDate.Value)) <= 0);
            }
            if (search.DenNgay.HasValue)
            {
                lstrs1 = lstrs1.Where(t => DateTime.Compare((DateTime)DbFunctions.TruncateTime(t.CreateDate.Value), search.DenNgay.Value) <= 0);
            }

            var lstrs = new List<MOrder>();


            foreach (var item in lstrs1)
            {
                MOrder m = new MOrder();
                m.STT = lstrs1.ToList().IndexOf(item) + 1;
                m.OrderID = item.OrderID;
                m.CreateDate = item.CreateDate;
                m.CustomerID = item.CustomerID;
                m.ShipName = item.ShipName;
                m.ShipAddress = CutAddress(item.ShipAddress).Trim('/');
                m.ShipEmail = item.ShipEmail;
                m.ShipPhone = item.ShipPhone;
                m.CreateDate = item.CreateDate;
                m.Discription = item.Discription;
                m.PaymentStatus = item.PaymentStatus;
                m.TotalPrice = (item.TotalPrice - item.PriceDiscount) + item.PriceShip;
                m.OrderStatus = item.OrderStatus;
                m.PriceShip = item.PriceShip;
                m.PriceDiscount = item.PriceDiscount;
                m.TotalCount = db.OrderDetails.Where(t => t.OrderID == m.OrderID).Sum(t => t.OrderDetailCount);
                m.UserID = item.UserID;
                m.UserName = UserName(m.UserID);
                m.PaymentName = Payment(item.PaymentStatus);
                lstrs.Add(m);
            }



            return lstrs.ToPagedList(page, pageSize);
        }

        /// <summary>
        /// Thống kê theo khách hàng
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<MThongKeKhachHang> GetThongKeKhachHang(SearchThongKe search, int page, int pageSize)
        {
            var lstrs1 = db.Orders.Where(t => t.UserID.HasValue).AsQueryable();

            if (search.TuNgay.HasValue)
            {
                lstrs1 = lstrs1.Where(t => DateTime.Compare(search.TuNgay.Value, (DateTime)DbFunctions.TruncateTime(t.CreateDate.Value)) <= 0);
            }
            if (search.DenNgay.HasValue)
            {
                lstrs1 = lstrs1.Where(t => DateTime.Compare((DateTime)DbFunctions.TruncateTime(t.CreateDate.Value), search.DenNgay.Value) <= 0);
            }

            var lstrs = new List<MThongKeKhachHang>();

            foreach (var item in lstrs1.OrderBy(x => x.OrderID).GroupBy(t => t.CustomerID))
            {
                MThongKeKhachHang m = new MThongKeKhachHang();
                m.STT = lstrs1.OrderBy(x => x.OrderID).GroupBy(t => t.CustomerID).ToList().FindIndex(t => t.Key == item.Key) + 1;
                m.CustomerID = item.Key.Value;
                m.CustomerName = db.Customers.Find(item.Key.Value).Name;
                m.SoDonHang = item.Count();
                m.TienHang = item.Sum(t => (t.TotalPrice - t.PriceDiscount) + t.PriceShip);

                var soluong = 0;
                var lst1 = item.Select(t => t.OrderID);

                foreach (var item1 in lst1)
                {
                    soluong += demsl(item1);
                }
                m.SoLuong = soluong;

                lstrs.Add(m);
            }

            return lstrs.ToPagedList(page, pageSize);
        }

        /// <summary>
        /// Hàm lấy danh sách đơn hàng theo khách hàng
        /// </summary>
        /// <param name="page">trang hiện tại</param>
        /// <param name="pageSize">tổng số record 1 trang</param>
        /// <returns>Danh sách đơn hàng 1 trang</returns>
        public IEnumerable<MOrder> ListOrderTheoKhachHang(SearchThongKe search, int id, int page, int pageSize)
        {
            var lst = db.Orders.Where(t => t.CustomerID == id && t.UserID.HasValue).Select(t => t).OrderByDescending(t => t.CreateDate).AsQueryable();

            var lstrs1 = lst.AsQueryable();

            if (search.TuNgay.HasValue)
            {
                lstrs1 = lstrs1.Where(t => DateTime.Compare(search.TuNgay.Value, (DateTime)DbFunctions.TruncateTime(t.CreateDate.Value)) <= 0);
            }
            if (search.DenNgay.HasValue)
            {
                lstrs1 = lstrs1.Where(t => DateTime.Compare((DateTime)DbFunctions.TruncateTime(t.CreateDate.Value).Value, search.DenNgay.Value) <= 0);
            }

            var lstrs = new List<MOrder>();

            foreach (var item in lstrs1)
            {
                MOrder m = new MOrder();
                m.STT = lstrs1.ToList().IndexOf(item) + 1;
                m.OrderID = item.OrderID;
                m.CreateDate = item.CreateDate;
                m.CustomerID = item.CustomerID;
                m.ShipName = item.ShipName;
                m.ShipAddress = CutAddress(item.ShipAddress).Trim('/');
                m.ShipEmail = item.ShipEmail;
                m.ShipPhone = item.ShipPhone;
                m.CreateDate = item.CreateDate;
                m.Discription = item.Discription;
                m.PaymentStatus = item.PaymentStatus;
                m.TotalPrice = (item.TotalPrice - item.PriceDiscount) + item.PriceShip;
                m.OrderStatus = item.OrderStatus;
                m.PriceShip = item.PriceShip;
                m.PriceDiscount = item.PriceDiscount;
                m.TotalCount = db.OrderDetails.Where(t => t.OrderID == m.OrderID).Sum(t => t.OrderDetailCount);
                m.UserID = item.UserID;
                m.UserName = UserName(m.UserID);
                m.PaymentName = Payment(item.PaymentStatus);
                lstrs.Add(m);
            }

            return lstrs.ToPagedList(page, pageSize);
        }

        /// <summary>
        /// Thống kê theo sản phẩm
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<MThongKeHangHoa> GetThongKeSanPham(SearchThongKe search, int page, int pageSize)
        {
            var lst = (from Orders in db.Orders
                       join OrderDetails in db.OrderDetails on Orders.OrderID equals OrderDetails.OrderID
                       select (new MThongKeHangHoa()
                       {
                           ProductID = OrderDetails.ProductID,
                           SoLuong = OrderDetails.OrderDetailCount.Value,
                           TienHang = OrderDetails.OrderPrice * OrderDetails.OrderDetailCount,
                           CreateDate = Orders.CreateDate.Value,
                           UserID = Orders.UserID
                       })
                       ).Where(t => t.UserID.HasValue).OrderByDescending(x => x.CreateDate);

            var lstrs1 = lst.Select(t => t).AsQueryable();


            if (search.TuNgay.HasValue)
            {
                lstrs1 = lstrs1.Where(t => DateTime.Compare(search.TuNgay.Value, (DateTime)DbFunctions.TruncateTime(t.CreateDate).Value) <= 0);
            }
            if (search.DenNgay.HasValue)
            {
                lstrs1 = lstrs1.Where(t => DateTime.Compare((DateTime)DbFunctions.TruncateTime(t.CreateDate).Value, search.DenNgay.Value) <= 0);
            }
            var ac = lstrs1.ToList();

            var lstrs = new List<MThongKeHangHoa>();

            foreach (var item in lstrs1.GroupBy(t => t.ProductID))
            {
                MThongKeHangHoa m = new MThongKeHangHoa();
                m.STT = lstrs1.OrderByDescending(x => x.CreateDate).GroupBy(t => t.ProductID).ToList().FindIndex(t => t.Key == item.Key) + 1;
                m.ProductID = item.Key;
                int? namecategory = db.Products.Find(item.Key).ProductCategoryID.HasValue ? db.Products.Find(item.Key).ProductCategoryID : null;
                m.KyTuCaterory = namecategory.HasValue ? getnamecategorykytu(db.ProductCategories.Where(e => e.ProductCategoryID == namecategory).FirstOrDefault().Name) : "";
                m.ProductIDDisplay = db.Products.Find(item.Key).ProductComboID.HasValue ? "CB" + ChangeIDProduct(db.Products.Find(item.Key).ProductComboID.Value) : "SP" + m.KyTuCaterory + ChangeIDProduct(item.Key);
                m.ProductName = db.Products.Find(item.Key).Name;
                m.DVT = db.Products.Find(item.Key).UnitID.HasValue ? db.ProductUnits.Find(db.Products.Find(item.Key).UnitID).Name : "Combo sản phẩm";
                m.SoDonHang = item.Count();
                m.TienHang = item.Sum(t => t.TienHang);
                m.SoLuong = item.Sum(t => t.SoLuong);

                lstrs.Add(m);
            }

            return lstrs.ToPagedList(page, pageSize);
        }

        /// <summary>
        /// Hàm lấy danh sách đơn hàng theo sản phẩm
        /// </summary>
        /// <param name="page">trang hiện tại</param>
        /// <param name="pageSize">tổng số record 1 trang</param>
        /// <returns>Danh sách đơn hàng 1 trang</returns>
        public IEnumerable<MThongKeHangHoaChitiet> ListOrderTheoSanPham(SearchThongKe search, int id, int page, int pageSize)
        {
            var lst = (from Orders in db.Orders
                       from OrderDetails in db.OrderDetails
                       where Orders.OrderID == OrderDetails.OrderID && OrderDetails.ProductID == id
                       select (new MThongKeHangHoaChitiet()
                       {
                           OrderID = Orders.OrderID,
                           UserID = Orders.UserID,
                           CreateDate = Orders.CreateDate.Value,
                           ProductID = OrderDetails.ProductID,
                           SoLuong = OrderDetails.OrderDetailCount.Value,
                           DonGia = OrderDetails.OrderPrice,
                           ThanhTien = OrderDetails.OrderDetailCount.Value * OrderDetails.OrderPrice,

                       })
                      ).Where(t => t.UserID.HasValue).OrderByDescending(t => t.CreateDate);

            var lstrs1 = lst.AsQueryable();

            var lst1 = lstrs1.ToList();

            if (search.TuNgay.HasValue)
            {
                lstrs1 = lstrs1.Where(t => DateTime.Compare(search.TuNgay.Value, (DateTime)DbFunctions.TruncateTime(t.CreateDate).Value) <= 0);

                var lst4 = lstrs1.ToList();
            }
            if (search.DenNgay.HasValue)
            {
                lstrs1 = lstrs1.Where(t => DateTime.Compare((DateTime)DbFunctions.TruncateTime(t.CreateDate).Value, search.DenNgay.Value) <= 0);
                var lst4 = lstrs1.ToList();
            }

            var lst2 = lstrs1.ToList();

            var lstrs = new List<MThongKeHangHoaChitiet>();

            foreach (var item in lstrs1)
            {
                MThongKeHangHoaChitiet m = new MThongKeHangHoaChitiet();
                m.STT = lstrs1.ToList().FindIndex(t => t.OrderID == item.OrderID) + 1;
                m.OrderID = item.OrderID;
                m.UserID = item.UserID;
                m.CreateDate = item.CreateDate;
                int? namecategory = db.Products.Find(item.ProductID).ProductCategoryID.HasValue ? db.Products.Find(item.ProductID).ProductCategoryID : null;
                m.KyTuCaterory = namecategory.HasValue ? getnamecategorykytu(db.ProductCategories.Where(e => e.ProductCategoryID == namecategory).FirstOrDefault().Name) : "";
                m.ProductIDDisplay = db.Products.Find(item.ProductID).ProductComboID.HasValue ? "CB" + ChangeIDProduct(db.Products.Find(item.ProductID).ProductComboID.Value) : "SP" + m.KyTuCaterory + ChangeIDProduct(item.ProductID); ;
                m.ProductID = item.ProductID;
                m.ProductName = db.Products.Find(m.ProductID).Name;
                var iddvt = db.Products.Find(m.ProductID).UnitID.HasValue ? db.Products.Find(m.ProductID).UnitID : null;
                m.DVT = iddvt.HasValue ? db.ProductUnits.Find(db.Products.Find(m.ProductID).UnitID).Name : "Combo sản phẩm";
                m.SoLuong = item.SoLuong;
                m.DonGia = item.DonGia;
                m.ThanhTien = item.ThanhTien;
                m.NguoiBan = db.Users.Find(m.UserID).FullName;
                lstrs.Add(m);
            }

            return lstrs.ToPagedList(page, pageSize);
        }

        //Thống kê lợi nhuận
        public IEnumerable<MThongKeLoiNhuanThang> GetThongKeLoiNhuanThang(SearchThongKe search, int page, int pageSize)
        {
            var lst1 = db.Orders.Where(t => t.UserID.HasValue).OrderBy(x => x.OrderID).AsQueryable();

            if (search.TuNgay.HasValue)
            {
                lst1 = lst1.Where(t => DateTime.Compare(search.TuNgay.Value, (DateTime)DbFunctions.TruncateTime(t.CreateDate.Value)) <= 0);
            }
            if (search.DenNgay.HasValue)
            {
                lst1 = lst1.Where(t => DateTime.Compare((DateTime)DbFunctions.TruncateTime(t.CreateDate.Value), search.DenNgay.Value) <= 0);
            }

            var lst = lst1
                .GroupBy(t => DbFunctions.TruncateTime(t.CreateDate.Value))
                .Select(t => new
                {
                    Key = t.Key,
                    ListOrder = t.Select(e => e.OrderID).ToList(),
                    TienHang = t.Sum(e => e.TotalPrice),
                    DoanhThu = t.Sum(e => (e.TotalPrice - e.PriceDiscount) + e.PriceShip)
                })
                .ToList();


            var lstrs1 = new List<MThongKeLoiNhuanThang>();


            foreach (var item in lst)
            {
                MThongKeLoiNhuanThang m = new MThongKeLoiNhuanThang();
                m.STT = lst.FindIndex(t => t.Key == item.Key) + 1;
                m.OrderList = item.ListOrder;
                m.CreateDate = item.Key;
                m.Von = GetVonOder(item.ListOrder);
                m.TienHang = item.TienHang;
                m.DoanhThu = item.DoanhThu;
                m.Lai = ((int)m.DoanhThu.Value - (int)m.Von);
                lstrs1.Add(m);
            }

            return lstrs1.ToPagedList(page, pageSize);
        }

        public decimal GetVonOder(List<int> listorder)
        {

            decimal rs = 0;

            foreach (var item in listorder)
            {

                var orderdetail = db.OrderDetails.Where(t => t.OrderID == item).ToList();

                foreach (var product in orderdetail)
                {
                    if (!product.Product.ProductComboID.HasValue)
                    {
                        var act = Math.Round(GetExchangeByProductID(product.ProductID).PriceInput.Value);

                        rs += act * product.OrderDetailCount.Value;
                    }else
                    {

                        var lstinput = db.ProductComboDetails.Where(t => t.ProductComboID == product.Product.ProductComboID.Value).Select(t=>t.ProductID).ToList();


                        rs += GetVonOder(lstinput);

                    }

                }
            }

            return rs;
        }

        public MExchangeUnit GetExchangeByProductID(int ProductID)
        {
            var Rs = new MExchangeUnit();
            var product = db.Products.Find(ProductID);
            Rs.ProductID = ProductID;
            Rs.ProductName = product.Name;
            Rs.UnitName = db.ProductUnits.Find(product.UnitID).Name;

            var CountReceiptProduct = db.ReceiptDetails.Where(t => t.ProductID == ProductID);

            int TotalPriceProductInCheck = db.ReceiptDetails.Where(t => t.ProductID == ProductID).Count();

            decimal TotalPriceProductIn = 0;

            if (TotalPriceProductInCheck != 0)
            {
                TotalPriceProductIn = db.ReceiptDetails.Where(t => t.ProductID == ProductID).Sum(t => t.PriceIput.Value);
                Rs.PriceInput = TotalPriceProductIn / CountReceiptProduct.Count();
            }
            else
            {
                if (product.ParentProductID != 0)
                {
                    var priceinput = db.Products.Where(t => t.ParentProductID == product.ParentProductID && t.ProductID != product.ProductID).FirstOrDefault();

                    decimal a = GetExchangeByProductID(priceinput.ProductID).PriceInput.Value;

                    decimal b = (decimal)db.ProductUnits.Find(product.UnitID).ValueUnit.Value;

                    Rs.PriceInput = (b * a) / (decimal)db.ProductUnits.Find(priceinput.UnitID).ValueUnit.Value;
                }else
                {
                    var priceinput1 = db.Products.Where(t => t.ParentProductID == product.ProductID).OrderByDescending(t => t.PriceOut).FirstOrDefault();

                    decimal a1 = GetExchangeByProductID(priceinput1.ProductID).PriceInput.Value;

                    decimal b1 = (decimal)db.ProductUnits.Find(product.UnitID).ValueUnit.Value;

                    Rs.PriceInput = (b1 * a1) / (decimal)db.ProductUnits.Find(priceinput1.UnitID).ValueUnit.Value;
                }              
            }
            Rs.ValueUnit = db.ProductUnits.Find(product.UnitID).ValueUnit.Value;

            Rs.CountProduct = product.CountProduct.Value;

            Rs.CategoryName = db.ProductCategories.Where(e => e.ProductCategoryID == product.ProductCategoryID).FirstOrDefault().Name;

            Rs.KyTuCaterory = getnamecategorykytu(Rs.CategoryName);
            return Rs;

        }

        public IEnumerable<MThongKeLoiNhuanThang> ListOrderLoiNhuanTheoNgay(SearchThongKe search, DateTime ngaytao, int page, int pageSize)
        {

            var lst = db.Orders.Where(t => t.UserID.HasValue && DbFunctions.TruncateTime(t.CreateDate.Value) == ngaytao).ToList();

            var lstrs1 = new List<MThongKeLoiNhuanThang>();

            foreach (var item in lst)
            {
                MThongKeLoiNhuanThang m = new MThongKeLoiNhuanThang();
                m.STT = lst.IndexOf(item) + 1;
                m.OrderID = item.OrderID;
                m.CreateDate = item.CreateDate;
                m.CustomerName = item.ShipName;                
                m.TienHang = item.TotalPrice;
                var lstorder = new List<int>();
                lstorder.Add(m.OrderID);
                m.Von = GetVonOder(lstorder);
                m.DoanhThu = (item.TotalPrice-item.PriceDiscount)+item.PriceShip;
                m.Lai = ((int)m.DoanhThu.Value - (int)m.Von);
                m.ShipAddress = CutAddress(item.ShipAddress).Trim('/');
                m.ShipEmail = item.ShipEmail;
                m.ShipPhone = item.ShipPhone;
                m.Discription = item.Discription;
                m.UserName = UserName(item.UserID);
                m.UserID = item.UserID;
                lstrs1.Add(m);
            }

            return lstrs1.ToPagedList(page, pageSize);
        }


        /// <summary>
        /// Thống kê theo khách hàng
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<MThongKeLoiNhuanKhachHang> GetThongKeLoiNhuanKhachHang(SearchThongKe search, int page, int pageSize)
        {
            var lstrs1 = db.Orders.Where(t => t.UserID.HasValue).AsQueryable();

            if (search.TuNgay.HasValue)
            {
                lstrs1 = lstrs1.Where(t => DateTime.Compare(search.TuNgay.Value, (DateTime)DbFunctions.TruncateTime(t.CreateDate.Value)) <= 0);
            }
            if (search.DenNgay.HasValue)
            {
                lstrs1 = lstrs1.Where(t => DateTime.Compare((DateTime)DbFunctions.TruncateTime(t.CreateDate.Value), search.DenNgay.Value) <= 0);
            }

            var lst = lstrs1
                .OrderBy(x => x.CreateDate)
                .GroupBy(t => t.CustomerID)
                .Select(t => new
                {
                    Key = t.Key,
                    ListOrder = t.Select(e => e.OrderID).ToList(),
                    TienHang = t.Sum(e => e.TotalPrice),
                    DoanhThu = t.Sum(e => (e.TotalPrice - e.PriceDiscount) + e.PriceShip)
                })
                .ToList();

            var lstrs = new List<MThongKeLoiNhuanKhachHang>();

            foreach (var item in lst)
            {
                MThongKeLoiNhuanKhachHang m = new MThongKeLoiNhuanKhachHang();
                m.STT = lst.FindIndex(t => t.Key == item.Key) + 1;
                m.CustomerID = item.Key.Value;
                m.CustomerName = db.Customers.Find(item.Key.Value).Name;
                m.Von = GetVonOder(item.ListOrder);
                m.TienHang = item.TienHang;
                m.DoanhThu = item.DoanhThu;
                m.Lai = ((int)m.DoanhThu.Value - (int)m.Von);
                lstrs.Add(m);
            }

            return lstrs.ToPagedList(page, pageSize);
        }

        /// <summary>
        /// Hàm lấy danh sách lợi nhuận đơn hàng theo khách hàng
        /// </summary>
        /// <param name="page">trang hiện tại</param>
        /// <param name="pageSize">tổng số record 1 trang</param>
        /// <returns>Danh sách đơn hàng 1 trang</returns>
        public IEnumerable<MThongKeLoiNhuanKhachHang> ListOrderLoiNhuanTheoKhachHang(SearchThongKe search, int id, int page, int pageSize)
        {
            var lstrs1 = db.Orders.Where(t => t.UserID.HasValue&&t.CustomerID==id).AsQueryable();

            if (search.TuNgay.HasValue)
            {
                lstrs1 = lstrs1.Where(t => DateTime.Compare(search.TuNgay.Value, (DateTime)DbFunctions.TruncateTime(t.CreateDate.Value)) <= 0);
            }
            if (search.DenNgay.HasValue)
            {
                lstrs1 = lstrs1.Where(t => DateTime.Compare((DateTime)DbFunctions.TruncateTime(t.CreateDate.Value), search.DenNgay.Value) <= 0);
            }

            var lst = lstrs1.OrderBy(x => x.CreateDate).ToList();

            var lstrs = new List<MThongKeLoiNhuanKhachHang>();

            foreach (var item in lst)
            {
                MThongKeLoiNhuanKhachHang m = new MThongKeLoiNhuanKhachHang();
                m.STT = lst.IndexOf(item) + 1;
                m.CustomerID = item.CustomerID;
                m.OrderID = item.OrderID;
                m.CustomerName = item.ShipName;
                m.CreateDate = item.CreateDate;                
                m.TienHang = item.TotalPrice;
                m.DoanhThu = (item.TotalPrice - item.PriceDiscount) + item.PriceShip;
                var lstorder = new List<int>();
                lstorder.Add(m.OrderID);
                m.Von = GetVonOder(lstorder);
                m.Lai = ((int)m.DoanhThu.Value - (int)m.Von);
                m.ShipAddress = CutAddress(item.ShipAddress).Trim('/');
                m.ShipEmail = item.ShipEmail;
                m.ShipPhone = item.ShipPhone;
                m.Discription = item.Discription;
                m.UserName = UserName(item.UserID);
                m.UserID = item.UserID;
                lstrs.Add(m);
            }

            return lstrs.ToPagedList(page, pageSize);
        }


        public MBaoCaoLaiLaiLo BaoCaoLaiLo(SearchThongKe search)
        {
            if (search.TuNgay.HasValue && search.DenNgay.HasValue)
            {
                MBaoCaoLaiLaiLo m = new MBaoCaoLaiLaiLo();
                m.KyBaoCaoTu = search.TuNgay.Value;
                m.KyBaoCaoDen = search.DenNgay.Value;
                TimeSpan khoancach1 = m.KyBaoCaoDen.Subtract(m.KyBaoCaoTu);
                TimeSpan tru1ngay1 = new System.TimeSpan(1, 0, 0, 0);
                m.KyTruocDen = m.KyBaoCaoTu.Subtract(tru1ngay1);
                m.KyTruocTu = m.KyTruocDen.Subtract(khoancach1);

                m.DoanhSoBanHang = "Doanh số bán hàng";
                m.KyTruocDoanhSoBanHang = BaoCaoDoanhSoBanHang(m.KyTruocTu, m.KyTruocDen);
                m.KyBaoCaoDoanhSoBanHang = BaoCaoDoanhSoBanHang(m.KyBaoCaoTu, m.KyBaoCaoDen);
                m.ThaydoiDoanhSoBanHang = m.KyBaoCaoDoanhSoBanHang - m.KyTruocDoanhSoBanHang;

                m.GiamGia = "Tiền giảm giá";
                m.KyTruocGiamGia = BaoCaoGiamGia(m.KyTruocTu, m.KyTruocDen);
                m.KyBaoCaoGiamGia = BaoCaoGiamGia(m.KyBaoCaoTu, m.KyBaoCaoDen);
                m.ThaydoiGiamGia = m.KyBaoCaoGiamGia - m.KyTruocGiamGia;

                m.PhiVanChuyen = "Phí vận chuyên";
                m.KyTruocPhiVanChuyen = BaoCaoVanChuyen(m.KyTruocTu, m.KyTruocDen);
                m.KyBaoCaoPhiVanChuyen = BaoCaoVanChuyen(m.KyBaoCaoTu, m.KyBaoCaoDen);
                m.ThaydoiPhiVanChuyen = m.KyBaoCaoPhiVanChuyen - m.KyTruocPhiVanChuyen;

                m.DoanhThu = "Doanh thu = [1] - [2] + [3] ";
                m.KyTruocDoanhThu = (m.KyTruocDoanhSoBanHang - m.KyTruocGiamGia) + m.KyTruocPhiVanChuyen;
                m.KyBaoCaoDoanhThu = (m.KyBaoCaoDoanhSoBanHang - m.KyBaoCaoGiamGia) + m.KyBaoCaoPhiVanChuyen;
                m.ThaydoiDoanhThu = m.KyBaoCaoDoanhThu - m.KyTruocDoanhThu;

                m.VonHangHoa = "Tiền vốn mua hàng hóa";
                m.KyTruocVonHangHoa = BaoCaoVonMuaHang(m.KyTruocTu, m.KyTruocDen);
                m.KyBaoCaoVonHangHoa = BaoCaoVonMuaHang(m.KyBaoCaoTu, m.KyBaoCaoDen);
                m.ThaydoiVonHangHoa = m.KyBaoCaoVonHangHoa - m.KyTruocVonHangHoa;

                m.LaiGop = "Lãi gộp = [4] - [5]";
                m.KyTruocLaiGop = (m.KyTruocDoanhThu - m.KyTruocVonHangHoa);
                m.KyBaoCaoLaiGop = (m.KyBaoCaoDoanhThu - m.KyBaoCaoVonHangHoa);
                m.ThaydoiLaigop = m.KyBaoCaoLaiGop - m.KyTruocLaiGop;

                m.TienGiamChiaDoanhThu = "Tiền giảm giá/Doanh thu (%)=[2]/[4]";
                m.KyTruocTienGiamChiaDoanhThu = m.KyTruocDoanhThu.HasValue ? Math.Round((double)(m.KyTruocGiamGia*100 / m.KyTruocDoanhThu),2) : 0;
                m.KyBaoCaoTienGiamChiaDoanhThu = m.KyBaoCaoDoanhThu.HasValue ? Math.Round((double)(m.KyBaoCaoGiamGia*100 / m.KyBaoCaoDoanhThu),2) : 0;
                m.ThaydoiTienGiamChiaDoanhThu = m.KyBaoCaoTienGiamChiaDoanhThu - m.KyTruocTienGiamChiaDoanhThu;


                m.LaiGopChiaDoanhThu = "Lãi gộp/Doanh thu (%)=[6]/[4]";
                m.KyTruocTienGiamChiaLaiGop = m.KyTruocLaiGop.HasValue ? Math.Round((double)(m.KyTruocLaiGop * 100 / m.KyTruocDoanhThu), 2) : 0;
                m.KyBaoCaoTienGiamChiaLaiGop = m.KyBaoCaoLaiGop.HasValue ? Math.Round((double)(m.KyBaoCaoLaiGop * 100 / m.KyBaoCaoDoanhThu), 2) : 0;
                m.ThaydoiLaiGopChiaDoanhThu = m.KyBaoCaoTienGiamChiaLaiGop - m.KyTruocTienGiamChiaLaiGop;
                return m;
            }

            MBaoCaoLaiLaiLo m2 = new MBaoCaoLaiLaiLo();
            m2.KyBaoCaoTu = new DateTime(DateTime.Now.Year, DateTime.Now.Month,1);
            m2.KyBaoCaoDen = DateTime.Now.Date;
            TimeSpan khoancach = m2.KyBaoCaoDen.Subtract(m2.KyBaoCaoTu);
            TimeSpan tru1ngay = new System.TimeSpan(1, 0, 0, 0);            
            m2.KyTruocDen = m2.KyBaoCaoTu.Subtract(tru1ngay);
            m2.KyTruocTu = m2.KyTruocDen.Subtract(khoancach);

            m2.DoanhSoBanHang = "Doanh số bán hàng";
            m2.KyTruocDoanhSoBanHang = BaoCaoDoanhSoBanHang(m2.KyTruocTu, m2.KyTruocDen);
            m2.KyBaoCaoDoanhSoBanHang = BaoCaoDoanhSoBanHang(m2.KyBaoCaoTu, m2.KyBaoCaoDen);
            m2.ThaydoiDoanhSoBanHang = m2.KyBaoCaoDoanhSoBanHang - m2.KyTruocDoanhSoBanHang;

            m2.GiamGia = "Tiền giảm giá";
            m2.KyTruocGiamGia = BaoCaoGiamGia(m2.KyTruocTu, m2.KyTruocDen);
            m2.KyBaoCaoGiamGia = BaoCaoGiamGia(m2.KyBaoCaoTu, m2.KyBaoCaoDen);
            m2.ThaydoiGiamGia = m2.KyBaoCaoGiamGia - m2.KyTruocGiamGia;

            m2.PhiVanChuyen = "Phí vận chuyên";
            m2.KyTruocPhiVanChuyen = BaoCaoVanChuyen(m2.KyTruocTu, m2.KyTruocDen);
            m2.KyBaoCaoPhiVanChuyen = BaoCaoVanChuyen(m2.KyBaoCaoTu, m2.KyBaoCaoDen);
            m2.ThaydoiPhiVanChuyen = m2.KyBaoCaoPhiVanChuyen - m2.KyTruocPhiVanChuyen;

            m2.DoanhThu = "Doanh thu = [1] - [2] + [3] ";
            m2.KyTruocDoanhThu = (m2.KyTruocDoanhSoBanHang - m2.KyTruocGiamGia) + m2.KyTruocPhiVanChuyen;
            m2.KyBaoCaoDoanhThu = (m2.KyBaoCaoDoanhSoBanHang - m2.KyBaoCaoGiamGia) + m2.KyBaoCaoPhiVanChuyen;
            m2.ThaydoiDoanhThu = m2.KyBaoCaoDoanhThu - m2.KyTruocDoanhThu;

            m2.VonHangHoa = "Tiền vốn mua hàng hóa";
            m2.KyTruocVonHangHoa = BaoCaoVonMuaHang(m2.KyTruocTu, m2.KyTruocDen);
            m2.KyBaoCaoVonHangHoa = BaoCaoVonMuaHang(m2.KyBaoCaoTu, m2.KyBaoCaoDen);
            m2.ThaydoiVonHangHoa = m2.KyBaoCaoVonHangHoa - m2.KyTruocVonHangHoa;

            m2.LaiGop = "Lãi gộp = [4] - [5]";
            m2.KyTruocLaiGop = (m2.KyTruocDoanhThu - m2.KyTruocVonHangHoa);
            m2.KyBaoCaoLaiGop = (m2.KyBaoCaoDoanhThu - m2.KyBaoCaoVonHangHoa);
            m2.ThaydoiLaigop = m2.KyBaoCaoLaiGop - m2.KyTruocLaiGop;

            m2.TienGiamChiaDoanhThu = "Tiền giảm giá/Doanh thu (%)=[2]/[4]";
            m2.KyTruocTienGiamChiaDoanhThu = m2.KyTruocDoanhThu.HasValue?Math.Round((double)(m2.KyTruocGiamGia*100 / m2.KyTruocDoanhThu),2):0;
            m2.KyBaoCaoTienGiamChiaDoanhThu = m2.KyBaoCaoDoanhThu.HasValue? Math.Round((double)(m2.KyBaoCaoGiamGia*100 / m2.KyBaoCaoDoanhThu),2) : 0;
            m2.ThaydoiTienGiamChiaDoanhThu = m2.KyBaoCaoTienGiamChiaDoanhThu - m2.KyTruocTienGiamChiaDoanhThu;

            m2.LaiGopChiaDoanhThu = "Lãi gộp/Doanh thu (%)=[6]/[4]";
            m2.KyTruocTienGiamChiaLaiGop = m2.KyTruocLaiGop.HasValue ? Math.Round((double)(m2.KyTruocLaiGop * 100 / m2.KyTruocDoanhThu), 2) : 0;
            m2.KyBaoCaoTienGiamChiaLaiGop = m2.KyBaoCaoLaiGop.HasValue ? Math.Round((double)(m2.KyBaoCaoLaiGop * 100 / m2.KyBaoCaoDoanhThu), 2) : 0;
            m2.ThaydoiLaiGopChiaDoanhThu = m2.KyBaoCaoTienGiamChiaLaiGop - m2.KyTruocTienGiamChiaLaiGop;


            return m2;

        }

        public decimal? BaoCaoDoanhSoBanHang(DateTime tungay, DateTime denngay)
        {
            SearchThongKe search = new SearchThongKe();
            search.TuNgay = tungay;
            search.DenNgay = denngay;

            var lst = GetThongKeLoiNhuanThang(search, 1, 10000000).ToList();

            if (lst.Count != 0)
            {
                return lst.Sum(t => t.TienHang.Value);

            }
            return null;
        }

        public decimal? BaoCaoGiamGia(DateTime tungay, DateTime denngay)
        {
            SearchThongKe search = new SearchThongKe();
            search.TuNgay = tungay;
            search.DenNgay = denngay;

            var lst = GetThongKeThang(search, 1, 10000000).ToList();

            if (lst.Count != 0)
            {
                return lst.Sum(t => t.GiamGia.Value);

            }
            return null;
        }
        public decimal? BaoCaoVanChuyen(DateTime tungay, DateTime denngay)
        {
            SearchThongKe search = new SearchThongKe();
            search.TuNgay = tungay;
            search.DenNgay = denngay;

            var lst = GetThongKeThang(search, 1, 10000000).ToList();

            if (lst.Count != 0)
            {
                return lst.Sum(t => t.ShipPrice.Value);

            }
            return null;
        }


        public decimal? BaoCaoVonMuaHang(DateTime tungay, DateTime denngay)
        {
            SearchThongKe search = new SearchThongKe();
            search.TuNgay = tungay;
            search.DenNgay = denngay;

            var lst = GetThongKeLoiNhuanThang(search, 1, 10000000).ToList();

            if (lst.Count != 0)
            {
                return lst.Sum(t => t.Von.Value);

            }
            return null;
        }


        public List<selectToltal> selectDashBoard()
        {
            //Tháng giá trị 1
            //Năm nay giá trị 2
            //Năm trước giá trị 3
            //List tháng từ giá trị 4

            List<selectToltal> toltals = new List<selectToltal>();

            SelectDasboard selectDasboards = new SelectDasboard();

            selectDasboards.TenThang = $"Tháng này ({DateTime.Now.ToString("MM/yyyy")})";
            selectDasboards.ValueThang = 1;

            selectDasboards.TenNamNay = $"Năm nay ({DateTime.Now.Year.ToString()})";
            selectDasboards.ValueNamNay = 2;

            string namtruoc = DateTime.Now.Year.ToString();
            selectDasboards.TenNamTruoc = $"Năm trước ({int.Parse(namtruoc)-1})";
            selectDasboards.ValueNamTruoc = 3;


            List<SelectThang> lstthang = new List<SelectThang>();
            int demsl = 4;
            for (int i = 0; i < 12; i++)
            {
                SelectThang m = new SelectThang();
                m.TenThang = $"Tháng {i+1}/{DateTime.Now.Year.ToString()}";
                m.ValueThang = demsl;
                lstthang.Add(m);
                demsl++;
            }
            selectDasboards.ListThang = lstthang.ToList();

            selectToltal total = new selectToltal();
            total.Name = selectDasboards.TenThang;
            total.Value = selectDasboards.ValueThang;

            selectToltal total2 = new selectToltal();
            total2.Name = selectDasboards.TenNamNay;
            total2.Value = selectDasboards.ValueNamNay;

            selectToltal total3 = new selectToltal();
            total3.Name = selectDasboards.TenNamTruoc;
            total3.Value = selectDasboards.ValueNamTruoc;

            toltals.Add(total);
            toltals.Add(total2);
            toltals.Add(total3);

            foreach (var item in lstthang)
            {
                selectToltal total4 = new selectToltal();
                total4.Name = item.TenThang;
                total4.Value = item.ValueThang;
                toltals.Add(total4);
            }

            return toltals;

        }

        public string getdaydashboard(int value)
        {
            //Tháng giá trị 1
            //Năm nay giá trị 2
            //Năm trước giá trị 3
            //List tháng từ giá trị 4

            if (value < 4)
            {
                if (value == 1)
                {
                    string date = "";

                    DateTime nowtime = DateTime.Now;

                    DateTime dateTime = new DateTime(nowtime.Year, nowtime.Month, 1);

                    date = $"({dateTime.ToString("dd/MM/yyyy")} - {nowtime.ToString("dd/MM/yyyy")})";

                    return date;
                }else if (value==2)
                {
                    string date = "";

                    DateTime nowtime = DateTime.Now;

                    DateTime dateTime1 = new DateTime(nowtime.Year, 1, 1);

                    DateTime dateTime2 = new DateTime(nowtime.Year,12, 31);

                    date = $"({dateTime1.ToString("dd/MM/yyyy")} - {dateTime2.ToString("dd/MM/yyyy")})";

                    return date;
                }else
                {
                    string date = "";

                    int nowtime = DateTime.Now.Year - 1;

                    DateTime dateTime1 = new DateTime(nowtime, 1, 1);

                    DateTime dateTime2 = new DateTime(nowtime, 12, 31);

                    date = $"({dateTime1.ToString("dd/MM/yyyy")} - {dateTime2.ToString("dd/MM/yyyy")})";

                    return date;
                }
              
            }

            int thang = value - 3;

            string date2 = "";

            int nowtime2 = DateTime.Now.Year;

            DateTime dateTime3 = new DateTime(nowtime2, thang, 1);

            DateTime dateTime4 = GetLastDayOfMonth(new DateTime(nowtime2, thang, 1));

            date2 = $"({dateTime3.ToString("dd/MM/yyyy")} - {dateTime4.ToString("dd/MM/yyyy")})";

            return date2;

        }

        public static DateTime GetLastDayOfMonth(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month));
        }


        public decimal? BaoCaoDoanhSoBanHangTong(DateTime tungay, DateTime denngay)
        {
            SearchThongKe search = new SearchThongKe();
            search.TuNgay = tungay;
            search.DenNgay = denngay;

            var lst = GetThongKeThang(search, 1, 10000000).ToList();

            if (lst.Count != 0)
            {
                return lst.Sum(t => t.TienHang.Value);

            }
            return null;
        }

        public int? BaoCaoDonHangBanHangTong(DateTime tungay, DateTime denngay)
        {
            SearchThongKe search = new SearchThongKe();
            search.TuNgay = tungay;
            search.DenNgay = denngay;

            var lst = GetThongKeThang(search, 1, 10000000).ToList();

            if (lst.Count != 0)
            {
                return lst.Sum(t => t.SoDonHang);

            }
            return null;
        }

        public int? BaoCaoSoluongbanBanHangTong(DateTime tungay, DateTime denngay)
        {
            SearchThongKe search = new SearchThongKe();
            search.TuNgay = tungay;
            search.DenNgay = denngay;

            var lst = GetThongKeThang(search, 1, 10000000).ToList();

            if (lst.Count != 0)
            {
                return lst.Sum(t => t.SoLuong);

            }
            return null;
        }

        public decimal? BaoCaoLaiGopMuaHang(DateTime tungay, DateTime denngay)
        {
            SearchThongKe search = new SearchThongKe();
            search.TuNgay = tungay;
            search.DenNgay = denngay;

            var lst = GetThongKeLoiNhuanThang(search, 1, 10000000).ToList();

            if (lst.Count != 0)
            {
                return lst.Sum(t => t.Lai);

            }
            return null;
        }

        public int? BaoCaoTongKho(DateTime tungay, DateTime denngay)
        {
            SearchProduct search = new SearchProduct();
            search.TuNgay = tungay;
            search.DenNgay = denngay;

            var lst = ListProductAdmin(1, 10000000, search).ToList();

            if (lst.Count != 0)
            {
                return lst.Count();

            }
            return null;
        }

        public int? BaoCaoTonKho(DateTime tungay, DateTime denngay)
        {
            SearchProduct search = new SearchProduct();
            search.TuNgay = tungay;
            search.DenNgay = denngay;

            var lst = ListProductAdmin(1, 10000000, search).ToList();

            if (lst.Count != 0)
            {
                return lst.Count(t => t.CountProduct != 0);

            }
            return null;
        }

        public int? BaoCaoDaHet(DateTime tungay, DateTime denngay)
        {
            SearchProduct search = new SearchProduct();
            search.TuNgay = tungay;
            search.DenNgay = denngay;

            var lst = ListProductAdmin(1, 10000000, search).ToList();

            if (lst.Count != 0)
            {
                return lst.Count(t => t.CountProduct == 0);

            }
            return null;
        }

        public IEnumerable<MProduct> ListProductAdmin(int page, int pageSize, SearchProduct search)
        {
            var listproduct = db.Products.Where(t => t.ProductComboID.HasValue == false && t.ProductStatus == true).OrderByDescending(x => x.CreateDate).Select(t => t).AsQueryable();

            List<MProduct> mProducts = new List<MProduct>();

            foreach (var t in listproduct)
            {
                var mproduct = new MProduct();
                mproduct.STT = listproduct.ToList().IndexOf(t) + 1;
                mproduct.ProductID = t.ProductID;
                mproduct.Name = t.Name;
                mproduct.ParentProductID = t.ParentProductID;
                mproduct.Image = t.Image;
                mproduct.MoreImage = t.MoreImage;
                mproduct.PriceOut = t.PriceOut;
                mproduct.CountProduct = t.CountProduct;
                mproduct.Pricewholesale = t.Pricewholesale;
                mproduct.UnitID = t.UnitID;
                mproduct.CreateDate = t.CreateDate;
                mproduct.ProductCategoryID = t.ProductCategoryID;
                mproduct.ProductStatus = t.ProductStatus;
                mproduct.Display = t.Display;
                mproduct.MetaTitle = t.MetaTitle;
                mproduct.UnitName = db.ProductUnits.Where(e => e.UnitID == t.UnitID).FirstOrDefault().Name;
                mproduct.CategoryName = db.ProductCategories.Where(e => e.ProductCategoryID == t.ProductCategoryID).FirstOrDefault().Name;
                mproduct.PricewholesalePromotion = (db.ProductPricePromotions.Where(e => e.ProductID == t.ProductID && e.PromotionStatus == true).Count() != 0 ? db.ProductPricePromotions.Where(e => e.ProductID == t.ProductID && e.PromotionStatus == true).FirstOrDefault().PricewholesalePromotion : null);
                mproduct.PricePromotion = (db.ProductPricePromotions.Where(e => e.ProductID == t.ProductID && e.PromotionStatus == true).Count() != 0 ? db.ProductPricePromotions.Where(e => e.ProductID == t.ProductID && e.PromotionStatus == true).FirstOrDefault().PricePromotion : null);
                mproduct.KyTuCaterory = getnamecategorykytu(mproduct.CategoryName);
                mProducts.Add(mproduct);
            }
            return mProducts.ToPagedList(page, pageSize);
        }

        public decimal? GetDoanhSo(int value, SearchNews search)
        {
            if (search.TuNgay.HasValue && search.DenNgay.HasValue)
            {
                decimal? t2 = BaoCaoDoanhSoBanHangTong(search.TuNgay.Value, search.DenNgay.Value);

                return t2;
            }
            string output = getdaydashboard(value);

            string a = output.TrimStart('(');

            string b = a.Substring(0,a.Length-1);

            string[] datetime = b.Split('-');

            DateTime tungay = DateTime.Parse(datetime[0]);

            DateTime denngay = DateTime.Parse(datetime[1]);

            decimal? t = BaoCaoDoanhSoBanHangTong(tungay, denngay);

            return t;

        }

        public int? GetDonHang(int value, SearchNews search)
        {
            if (search.TuNgay.HasValue && search.DenNgay.HasValue)
            {
                int? t2 = BaoCaoDonHangBanHangTong(search.TuNgay.Value, search.DenNgay.Value);

                return t2.HasValue ? t2.Value : 0;
            }

            string output = getdaydashboard(value);

            string a = output.TrimStart('(');

            string b = a.Substring(0, a.Length - 1);

            string[] datetime = b.Split('-');

            DateTime tungay = DateTime.Parse(datetime[0]);

            DateTime denngay = DateTime.Parse(datetime[1]);

            int? t = BaoCaoDonHangBanHangTong(tungay, denngay);

            return t.HasValue?t.Value:0;

        }

        public int? GetSoluongBan(int value,SearchNews search)
        {
            if (search.TuNgay.HasValue && search.DenNgay.HasValue)
            {
                int? t2 = BaoCaoSoluongbanBanHangTong(search.TuNgay.Value, search.DenNgay.Value);

                return t2.HasValue ? t2.Value : 0;
            }
            string output = getdaydashboard(value);

            string a = output.TrimStart('(');

            string b = a.Substring(0, a.Length - 1);

            string[] datetime = b.Split('-');

            DateTime tungay = DateTime.Parse(datetime[0]);

            DateTime denngay = DateTime.Parse(datetime[1]);

            int? t = BaoCaoSoluongbanBanHangTong(tungay, denngay);

            return t.HasValue ? t.Value : 0;

        }

        public decimal? GetLai(int value,SearchNews search)
        {
            if (search.TuNgay.HasValue && search.DenNgay.HasValue)
            {
                decimal? t2 = BaoCaoLaiGopMuaHang(search.TuNgay.Value, search.DenNgay.Value);

                return t2;
            }

            string output = getdaydashboard(value);

            string a = output.TrimStart('(');

            string b = a.Substring(0, a.Length - 1);

            string[] datetime = b.Split('-');

            DateTime tungay = DateTime.Parse(datetime[0]);

            DateTime denngay = DateTime.Parse(datetime[1]);

            decimal? t = BaoCaoLaiGopMuaHang(tungay, denngay);

            return t;
        }




        public int? GetTongKho(int value)
        {
            string output = getdaydashboard(value);

            string a = output.TrimStart('(');

            string b = a.Substring(0, a.Length - 1);

            string[] datetime = b.Split('-');

            DateTime tungay = DateTime.Parse(datetime[0]);

            DateTime denngay = DateTime.Parse(datetime[1]);

            int? t = BaoCaoTongKho(tungay, denngay);

            return t;
        }

        public int? GetTonKho(int value)
        {
            string output = getdaydashboard(value);

            string a = output.TrimStart('(');

            string b = a.Substring(0, a.Length - 1);

            string[] datetime = b.Split('-');

            DateTime tungay = DateTime.Parse(datetime[0]);

            DateTime denngay = DateTime.Parse(datetime[1]);

            int? t = BaoCaoTonKho(tungay, denngay);

            return t;
        }

        public int? GetDaHet(int value)
        {
            string output = getdaydashboard(value);

            string a = output.TrimStart('(');

            string b = a.Substring(0, a.Length - 1);

            string[] datetime = b.Split('-');

            DateTime tungay = DateTime.Parse(datetime[0]);

            DateTime denngay = DateTime.Parse(datetime[1]);

            int? t = BaoCaoDaHet(tungay, denngay);

            return t;
        }


        public List<MThongKeThang> GetBuiDoThongKeDoanhSo(int value, SearchThongKe search)
        {
            if (search.TuNgay.HasValue && search.DenNgay.HasValue)
            {
                var lst2 = GetThongKeThang(search, 1, 100000000).ToList();

                return lst2;
            }
            string output = getdaydashboard(value);

            string a = output.TrimStart('(');

            string b = a.Substring(0, a.Length - 1);

            string[] datetime = b.Split('-');

            DateTime tungay = DateTime.Parse(datetime[0]);

            DateTime denngay = DateTime.Parse(datetime[1]);

            SearchThongKe searchtk = new SearchThongKe();

            searchtk.TuNgay = tungay;

            searchtk.DenNgay = denngay;

            var lst = GetThongKeThang(searchtk, 1, 100000000).ToList();

            return lst;

        }

    }
}
