using PROJECT_WEBSITE.Data.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJECT_WEBSITE.Data.DAO
{
    public class OrderDetailDAO
    {
        DbWebsite db;
        public OrderDetailDAO()
        {
            db = new DbWebsite();
        }
        public bool Insert(OrderDetail orderdetail,int quantity)
        {
            try
            {
                db.OrderDetails.Add(orderdetail);

                var product = db.Products.Find(orderdetail.ProductID);

                product.CountProduct = product.CountProduct -quantity;

                if (product.ProductComboID.HasValue)
                {
                    var lst = db.ProductComboDetails.Where(t => t.ProductComboID == product.ProductComboID);

                    foreach (var item in lst)
                    {
                        var productcombo = db.Products.Find(item.ProductID);
                        productcombo.CountProduct = productcombo.CountProduct - item.ProductComboCount;
                    }
                }
               
                db.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public List<OrderDetail> GetAll()
        {
            return db.OrderDetails.Select(t => t).ToList();
        }
        public List<OrderDetail> GetbyId(long id)
        {
            return db.OrderDetails.Where(t => t.OrderID == id).ToList();
        }
        public decimal Sum(long idorder)
        {
            return db.OrderDetails.Where(t => t.OrderID == idorder).Sum(t => t.OrderPrice.Value * t.OrderDetailCount.Value);
        }
    }
}
