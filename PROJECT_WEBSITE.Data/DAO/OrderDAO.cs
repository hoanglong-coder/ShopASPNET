using PROJECT_WEBSITE.Data.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;
using PROJECT_WEBSITE.Data.ModelCustom;

namespace PROJECT_WEBSITE.Data.DAO
{
    public class OrderDAO
    {
        DbWebsite db;
        public OrderDAO()
        {
            db = new DbWebsite();
        }
        public long Insert(MOrder entity,int customerid, string phone, bool payment)
        {

            Order neworder = new Order();
            neworder.CustomerID = customerid;
            neworder.CreateDate = DateTime.Now;          
            neworder.ShipPhone = phone;
            neworder.ShipName = entity.ShipName;
            neworder.ShipAddress = entity.ShipAddress;
            neworder.ShipEmail = entity.ShipEmail;
            neworder.Discription = entity.Discription;
            neworder.OrderStatus = -1;
            neworder.PaymentStatus = payment;
            db.Orders.Add(neworder);
            db.SaveChanges();
            return neworder.OrderID;
        }
        /// <summary>
        /// Hàm lấy danh sách đơn hàng
        /// </summary>
        /// <param name="page">trang hiện tại</param>
        /// <param name="pageSize">tổng số record 1 trang</param>
        /// <returns>Danh sách đơn hàng 1 trang</returns>
        public IEnumerable<MOrder> ListOrder(SearchOrder search,int page, int pageSize)
        {
            var lst = db.Orders.OrderBy(x => x.OrderID).Select(t => t).OrderByDescending(t=>t.CreateDate).AsQueryable();

            var lstrs = new List<MOrder>();


            foreach (var item in lst)
            {
                MOrder m = new MOrder();
                m.STT = lst.ToList().IndexOf(item) + 1;
                m.OrderID = item.OrderID;
                m.CustomerID = item.CustomerID;
                m.ShipName = item.ShipName;
                m.ShipAddress = CutAddress(item.ShipAddress).Trim('/');
                m.ShipEmail = item.ShipEmail;
                m.ShipPhone = item.ShipPhone;
                m.CreateDate = item.CreateDate;
                m.Discription = item.Discription;
                m.PaymentStatus = item.PaymentStatus;
                m.TotalPrice = (item.TotalPrice-item.PriceDiscount) + item.PriceShip;
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

            if (!string.IsNullOrEmpty(search.query))
            {
                var mahd = CutMaHD(search.query);

                if (mahd.HasValue)
                {
                    lstrs1 = lstrs1.Where(t => t.OrderID == mahd.Value);
                }
                else
                {
                    lstrs1 = lstrs1.Where(t => t.ShipName.Contains(search.query) || t.ShipPhone.Contains(search.query) || t.ShipAddress.Contains(search.query) || t.ShipEmail.Contains(search.query) || t.UserName.Contains(search.query));
                }
            }
            if (search.TuNgay.HasValue)
            {
                lstrs1 = lstrs1.Where(t => DateTime.Compare(search.TuNgay.Value, t.CreateDate.Value) < 0);
            }
            if (search.DenNgay.HasValue)
            {
                lstrs1 = lstrs1.Where(t => DateTime.Compare(t.CreateDate.Value, search.DenNgay.Value) < 0);
            }if (search.TrangThai.HasValue)
            {
                if (search.TrangThai.Value)
                {
                    // xác nhận
                    lstrs1 = lstrs1.Where(t => t.OrderStatus == 0);
                }else
                {
                    //Chưa xác nhận
                    lstrs1 = lstrs1.Where(t => t.OrderStatus == -1);
                }
            }if (search.PTThanhToan.HasValue)
            {

                //true-thanh toán online. False thanh toán cod
                lstrs1 = lstrs1.Where(t => t.PaymentStatus == search.PTThanhToan.Value);
            }


            return lstrs1.ToPagedList(page, pageSize);
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
                }else
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

        /// <summary>
        /// Xác nhận đơn hàng
        /// </summary>
        /// <param name="idorder">mã đơn hàng</param>
        /// <returns>đơn hàng xác nhận</returns>
        public bool OrderConfirma(int idorder,int idusser)
        {
            var order = db.Orders.Find(idorder);
            order.UserID = idusser;
            order.OrderStatus = 0;
            db.SaveChanges();
            return true;
        }
        /// <summary>
        /// Xác nhận giao hàng
        /// </summary>
        /// <param name="idorder">mã đơn hàng</param>
        /// <returns>đơn hàng xác nhận</returns>
        public bool OrderShip(int idorder)
        {
            var order = db.Orders.Find(idorder);
            order.OrderStatus = 1;
            db.SaveChanges();
            return true;
        }
        /// <summary>
        /// Xác nhận giao hàng thành công
        /// </summary>
        /// <param name="idorder">mã đơn hàng</param>
        /// <returns>đơn hàng xác nhận</returns>
        public bool OrderShipSuccsess(int idorder)
        {
            var order = db.Orders.Find(idorder);
            order.OrderStatus = 2;
            db.SaveChanges();
            return true;
        }
        /// <summary>
        /// Danh sách giao hàng
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<Order> ListShip(int page, int pageSize)
        {
            return db.Orders.Where(t => t.OrderStatus != -1).OrderBy(x => x.CreateDate).ToPagedList(page, pageSize);
        }

        public IEnumerable<MOrder> ListCartById(long id, int page, int pageSize, ref int c)
        {
            c = db.Orders.Where(t => t.CustomerID == id).OrderBy(x => x.CreateDate).Count();

            var model = db.Orders.Where(t => t.CustomerID == id).OrderByDescending(x => x.CreateDate);

            var lstrs = new List<MOrder>();

            foreach (var item in model)
            {
                MOrder m = new MOrder();
                m.STT = model.ToList().IndexOf(item) + 1;
                m.OrderID = item.OrderID;
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
                m.TotalCount = db.OrderDetails.Where(e => e.OrderID == m.OrderID).Sum(e => e.OrderDetailCount);
                m.UserID = item.UserID;
                m.UserName = UserName(m.UserID);
                m.PaymentName = Payment(item.PaymentStatus);
                lstrs.Add(m);
            }

            if (lstrs != null)
            {
                return lstrs.ToPagedList(page, pageSize);
            }
            else
            {
                return null;

            }
        }



        public void AddDetailDiscountCode(int OrderID, int idCode)
        {
            var discount = new DetailDiscountCode();
            discount.OrderID = OrderID;
            discount.DiscountCodeID = idCode;

            db.DetailDiscountCodes.Add(discount);

            db.SaveChanges();
        }

    }
}
