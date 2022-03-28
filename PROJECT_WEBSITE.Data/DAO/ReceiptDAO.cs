using PagedList;
using PROJECT_WEBSITE.Data.EF;
using PROJECT_WEBSITE.Data.ModelCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJECT_WEBSITE.Data.DAO
{
    public class ReceiptDAO
    {
        DbWebsite db; 


        public ReceiptDAO()
        {
            db = new DbWebsite();
        }


        public bool CreateReceipt(MReceipt mReceipt, List<MReceiptDetail> receiptDetails)
        {
            try
            {
                var Receipt = new Receipt();
                Receipt.UserID = mReceipt.UserID;
                Receipt.SupplierID = mReceipt.SupplierID;
                Receipt.CreateDate = mReceipt.CreateDate;
                Receipt.ReceiptStatus = true;
                Receipt.Description = mReceipt.Description;
                Receipt.TotalReceiptPrice = 0;
                db.Receipts.Add(Receipt);
        
                foreach (var item in receiptDetails)
                {
                    var ReceiptDetail = new ReceiptDetail();

                    ReceiptDetail.ReceiptID = Receipt.ReceiptID;

                    ReceiptDetail.ProductID = item.ProductID;

                    ReceiptDetail.PriceIput = item.PriceIput;

                    ReceiptDetail.ReceiptCount = item.ReceiptCount;

                    db.ReceiptDetails.Add(ReceiptDetail);

                    UpdateCountProduct(item.ProductID, item.ReceiptCount);

                    Receipt.TotalReceiptPrice += item.PriceIput * item.ReceiptCount;

                }

                Receipt.TotalCount = receiptDetails.Sum(t => t.ReceiptCount);

                db.SaveChanges();

                return true;
            }
            catch (Exception)
            {

                return false;
            }          
        }


        public void UpdateCountProduct(int idproduct, int? count)
        {
            var product = db.Products.Find(idproduct);

            product.CountProduct = product.CountProduct + count.Value;

            db.SaveChanges();
        }


        public IEnumerable<MReceipt> GetAll(SearchNews search, int page, int pageSize)
        {
            var lst = db.Receipts.Select(t => t).OrderByDescending(t => t.ReceiptID).ToList();

            var lstrs = new List<MReceipt>();

            foreach (var item in lst)
            {
                MReceipt m = new MReceipt();
                m.STT = lst.IndexOf(item) + 1;
                m.ReceiptID = item.ReceiptID;
                m.UserID = item.UserID;
                m.SupplierID = item.SupplierID;
                m.CreateDate = item.CreateDate;
                m.TotalCount = item.TotalCount;
                m.TotalReceiptPrice = item.TotalReceiptPrice;
                m.ReceiptStatus = item.ReceiptStatus;
                m.Description = item.Description;
                m.UserName = db.UserRoleGroups.Where(t => t.UserRoleGroupID == db.Users.Where(e => e.UserID == m.UserID).FirstOrDefault().UserRoleGroupID).FirstOrDefault().Name + " - " + db.Users.Where(t => t.UserID == m.UserID).FirstOrDefault().FullName;
                m.SupplierName = db.ProductSuppliers.Where(t => t.SupplierID == m.SupplierID).FirstOrDefault().Name;
                lstrs.Add(m);
            }
            var lstrs1 = lstrs.AsQueryable();

            if (!string.IsNullOrEmpty(search.SearchName))
            {

                var maphieu = CutMaPhieuNhap(search.SearchName);

                if (maphieu.HasValue)
                {
                    lstrs1 = lstrs1.Where(t => t.ReceiptID==maphieu.Value);
                }else
                {
                    lstrs1 = lstrs1.Where(t => t.UserName.Contains(search.SearchName));
                }
            }
            if (search.TuNgay.HasValue)
            {
                lstrs1 = lstrs1.Where(t => DateTime.Compare(search.TuNgay.Value, t.CreateDate.Value) <= 0);
            }
            if (search.DenNgay.HasValue)
            {
                lstrs1 = lstrs1.Where(t => DateTime.Compare(t.CreateDate.Value, search.DenNgay.Value) <= 0);
            }
            var lstrs2 = new List<MReceipt>();
            foreach (var item in lstrs1)
            {
                MReceipt m = new MReceipt();
                m.STT = lstrs1.ToList().IndexOf(item) + 1;
                m.ReceiptID = item.ReceiptID;
                m.UserID = item.UserID;
                m.SupplierID = item.SupplierID;
                m.CreateDate = item.CreateDate;
                m.TotalCount = item.TotalCount;
                m.TotalReceiptPrice = item.TotalReceiptPrice;
                m.ReceiptStatus = item.ReceiptStatus;
                m.Description = item.Description;
                m.UserName = db.UserRoleGroups.Where(t => t.UserRoleGroupID == db.Users.Where(e => e.UserID == m.UserID).FirstOrDefault().UserRoleGroupID).FirstOrDefault().Name + " - " + db.Users.Where(t => t.UserID == m.UserID).FirstOrDefault().FullName;
                m.SupplierName = db.ProductSuppliers.Where(t => t.SupplierID == m.SupplierID).FirstOrDefault().Name;
                lstrs2.Add(m);
            }
            return lstrs2.ToPagedList(page, pageSize);
        }

        public IEnumerable<MReceipt> GetAllTotal(SearchNews search)
        {
            var lst = db.Receipts.Select(t => t).OrderByDescending(t => t.ReceiptID).ToList();

            var lstrs = new List<MReceipt>();

            foreach (var item in lst)
            {
                MReceipt m = new MReceipt();
                m.STT = lst.IndexOf(item) + 1;
                m.ReceiptID = item.ReceiptID;
                m.UserID = item.UserID;
                m.SupplierID = item.SupplierID;
                m.CreateDate = item.CreateDate;
                m.TotalCount = item.TotalCount;
                m.TotalReceiptPrice = item.TotalReceiptPrice;
                m.ReceiptStatus = item.ReceiptStatus;
                m.Description = item.Description;
                m.UserName = db.UserRoleGroups.Where(t => t.UserRoleGroupID == db.Users.Where(e => e.UserID == m.UserID).FirstOrDefault().UserRoleGroupID).FirstOrDefault().Name + " - " + db.Users.Where(t => t.UserID == m.UserID).FirstOrDefault().FullName;
                m.SupplierName = db.ProductSuppliers.Where(t => t.SupplierID == m.SupplierID).FirstOrDefault().Name;
                lstrs.Add(m);
            }
            var lstrs1 = lstrs.AsQueryable();
            if (!string.IsNullOrEmpty(search.SearchName))
            {              
                var maphieu = CutMaPhieuNhap(search.SearchName);

                if (maphieu.HasValue)
                {
                    lstrs1 = lstrs1.Where(t => t.ReceiptID == maphieu.Value);
                }else
                {
                    lstrs1 = lstrs1.Where(t => t.UserName.Contains(search.SearchName));
                }
            }
            if (search.TuNgay.HasValue)
            {
                lstrs1 = lstrs1.Where(t => DateTime.Compare(search.TuNgay.Value, t.CreateDate.Value) <= 0);
            }
            if (search.DenNgay.HasValue)
            {
                lstrs1 = lstrs1.Where(t => DateTime.Compare(t.CreateDate.Value, search.DenNgay.Value) <= 0);
            }

            return lstrs1;
        }


        public List<MReceiptDetail> GetReceiptDetailsByID(int idreceipt)
        {
            var lst = db.ReceiptDetails.Where(t => t.ReceiptID == idreceipt).ToList();


            var lstrs = new List<MReceiptDetail>();

            foreach (var item in lst)
            {
                var idcatgory = db.Products.Find(item.ProductID).ProductCategoryID;
                MReceiptDetail m = new MReceiptDetail();
                m.STT = lst.IndexOf(item)+1;
                m.ReceitpDetailID = item.ReceitpDetailID;
                m.ReceiptID = item.ReceiptID;
                m.ProductID = item.ProductID;
                m.ProductName = db.Products.Find(item.ProductID).Name;
                m.ProductDVT = db.ProductUnits.Find(db.Products.Find(item.ProductID).UnitID).Name;
                m.PriceIput = item.PriceIput;
                m.ReceiptCount = item.ReceiptCount;
                m.TotalPrice = item.PriceIput * item.ReceiptCount;
                m.NameCategory = db.ProductCategories.Where(e => e.ProductCategoryID == idcatgory).FirstOrDefault().Name;
                m.KyTuCaterory = getnamecategorykytu(m.NameCategory);
                lstrs.Add(m);
            }

            return lstrs;
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

        public int? CutMaPhieuNhap(string input)
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
    }
}
