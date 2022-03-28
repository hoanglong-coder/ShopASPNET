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
    public class CustomerDAO
    {
        DbWebsite db;

        public CustomerDAO()
        {
            db = new DbWebsite();
        }

        public Customer GetById(string phone)
        {
            return db.Customers.SingleOrDefault(x => x.Phone == phone);
        }
        public Customer GetCustomer(int id)
        {
            return db.Customers.SingleOrDefault(x => x.CustomerID == id);
        }
        public IEnumerable<Customer> GetCustomer()
        {
            return db.Customers.Select(t => t);
        }
        public int Login(string phone, string passWord)
        {
            var result = db.Customers.SingleOrDefault(x => x.Phone == phone);
            if (result == null)
            {
                //Không tồn tại
                return 0;
            }
            else
            {
                if (result.CustomerStatus == 0)
                {
                    //Bị khóa
                    return -1;
                }
                else
                {
                    if (result.Password == passWord)
                    {
                        //Đúng
                        return 1;
                    }
                    else
                    {
                        //Sai mật khẩu
                        return -2;
                    }
                }
            }
        }


        public bool Register(string name, string phone, string password)
        {
            var customer = db.Customers.Where(t => t.Phone == phone).SingleOrDefault();
            if (customer != null)
            {
                return false;
            }
            else
            {
                var model = new Customer();
                model.Name = name;
                model.Phone = phone;
                model.Password = password;
                model.CreateDate = DateTime.Now;
                model.CustomerStatus = 1;
                db.Customers.Add(model);
                db.SaveChanges();
                return true;
            }
        }


        public string LoadCart(string phone)
        {
            var customer = db.Customers.Single(t => t.Phone == phone);
            return customer.Cart;
        }

        public void UpdateCart(string phone, string cart)
        {
            var customer = db.Customers.Single(t => t.Phone == phone);
            customer.Cart = cart;
            db.SaveChanges();
        }

        public void ClearCart(string phone)
        {
            var customer = db.Customers.Single(t => t.Phone == phone);
            customer.Cart = null;
            db.SaveChanges();
        }

        /// <summary>
        /// Cập nhật tài khoản khách hàng
        /// </summary>
        /// <param name="customer">Customer</param>
        public void UpdateCustomer(Customer customer)
        {
            var model = db.Customers.Find(customer.CustomerID);
            model.Name = customer.Name;
            model.Email = customer.Email;
            model.Address = customer.Address;
            model.Gender = customer.Gender;
            model.Birth = customer.Birth;
            db.SaveChanges();
        }

        public bool ChangePass(string old, string newpass, int id)
        {
            var model = db.Customers.Find(id);

            if (model.Password != old)
            {
                return false;
            }
            model.Password = newpass;
            db.SaveChanges();
            return true;

        }



        public IEnumerable<MCustomer> GetCustomerPaging(SearchNews search, int page, int pageSize)
        {
            var lstin = db.Customers;     

            var lstrs = new List<MCustomer>();

            foreach (var item in lstin.ToList())
            {
                MCustomer m = new MCustomer();
                m.STT = lstin.ToList().IndexOf(item) + 1;
                m.CustomerID = item.CustomerID;
                m.Name = item.Name;
                m.Phone = item.Phone;
                m.CreateDate = item.CreateDate;
                m.Address = item.Address;
                m.Email = item.Email;
                m.Gender = item.Gender.HasValue ? item.Gender.Value == true ? "Nam" : "Nữ" : "";
                m.Birth = item.Birth;
                m.CustomerStatus = item.CustomerStatus;
                lstrs.Add(m);
            }
            var lst = lstrs.AsEnumerable();
            if (!string.IsNullOrEmpty(search.SearchName))
            {
                var masanpham = CutMaSanPham(search.SearchName);

                if (masanpham.HasValue)
                {
                    lst = lst.Where(t => t.CustomerID == masanpham.Value);
                }
                else
                {
                    lst = lst.Where(t => t.Name.Contains(search.SearchName)||t.Address.Contains(search.SearchName) || t.Phone.Contains(search.SearchName) || t.Email.Contains(search.SearchName));
                }
            }
            if (search.TuNgay.HasValue)
            {
                lst = lst.Where(t => DateTime.Compare(search.TuNgay.Value, DbFunctions.TruncateTime(t.CreateDate.Value).Value) <= 0);
            }
            if (search.DenNgay.HasValue)
            {
                lst = lst.Where(t => DateTime.Compare(DbFunctions.TruncateTime(t.CreateDate.Value).Value, search.DenNgay.Value) <= 0);
            }
            return lst.ToPagedList(page, pageSize);
        }
        public int? CutMaSanPham(string input)
        {
            try
            {
                var rs = int.Parse(input.Substring(3));

                return rs;
            }
            catch (Exception)
            {

                return null;
            }

        }


        public bool ChangCustomer(int id)
        {
            try
            {
                var product = db.Customers.Find(id);
                product.CustomerStatus = product.CustomerStatus==1?0:1;

                db.SaveChanges();

                return product.CustomerStatus == 1 ? true :false;
            }
            catch (Exception)
            {

                return false;
            }
        }
        public DeleteProduct DeleteCustomer(int id)
        {
            var checkorrder = db.Orders.Where(t => t.CustomerID == id).Count();
            if (checkorrder != 0)
            {
                var rs1 = new DeleteProduct();
                rs1.Check = false;
                rs1.Result = "Không thể xóa do khách hàng này sử dụng trong báo cáo đơn hàng";
                return rs1;
            }
            var customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
            db.SaveChanges();
            var rs = new DeleteProduct();
            rs.Check = true;
            return rs;
        }
    }
}
