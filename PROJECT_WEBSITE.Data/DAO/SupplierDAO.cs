using PROJECT_WEBSITE.Data.EF;
using PROJECT_WEBSITE.Data.ModelCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJECT_WEBSITE.Data.DAO
{
    public class SupplierDAO
    {
        DbWebsite db;

        public SupplierDAO()
        {
            this.db = new DbWebsite();
        }


        public List<MSupplier> GetAll()
        {
            var lst = db.ProductSuppliers.Where(t => t.SupplierStatus == true).ToList();


            var lstrs = new List<MSupplier>();

            foreach (var item in lst)
            {
                MSupplier m = new MSupplier();
                m.SupplierID = item.SupplierID;
                m.Name = item.Name;
                m.Phone = item.Phone;
                m.Address = item.Address;
                m.Email = item.Email;
                m.CreateDate = item.CreateDate;
                m.SupplierStatus = item.SupplierStatus;

                lstrs.Add(m);
            }

            return lstrs;
        }
    
    
        
    }
}
