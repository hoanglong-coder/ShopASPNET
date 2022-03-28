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
    public class DiscountCodeDAO
    {
        DbWebsite db;

        public DiscountCodeDAO()
        {
            db = new DbWebsite();
        }

        public IEnumerable<MDiscountCode> GetAll(SearchNews search, int page, int pageSize)
        {
            CapNhatTrangThaiDiscount();
            var lst = db.DiscountCodes.AsQueryable();

            if (!string.IsNullOrEmpty(search.SearchName))
            {
                lst = lst.Where(t => t.Name.Contains(search.SearchName));
            }
            if (search.TuNgay.HasValue)
            {
                lst = lst.Where(t => DateTime.Compare(search.TuNgay.Value, DbFunctions.TruncateTime(t.CreateDate.Value).Value) <= 0);
            }
            if (search.DenNgay.HasValue)
            {
                lst = lst.Where(t => DateTime.Compare(DbFunctions.TruncateTime(t.CreateDate.Value).Value, search.DenNgay.Value) <= 0);
            }

            var lstrs = new List<MDiscountCode>();

            foreach (var item in lst.ToList())
            {
                MDiscountCode m = new MDiscountCode();
                m.STT = lst.ToList().IndexOf(item) + 1;
                m.DiscountCodeID = item.DiscountCodeID;
                m.Name = item.Name;
                m.StartDate = item.StartDate;
                m.EndDate = item.EndDate;
                m.CreateDate = item.CreateDate;
                m.PercentCart = item.PercentCart.HasValue?item.PercentCart.Value:0;
                m.TotalCart = item.TotalCart;
                m.DistcountCount = item.DistcountCount;
                m.DiscountStatus = item.DiscountStatus;
                lstrs.Add(m);
            }
            return lstrs.ToPagedList(page, pageSize);

        }
        
        public void CapNhatTrangThaiDiscount()
        {
            DateTime hientai = DateTime.Now.Date;

            var discountcode = db.DiscountCodes.Select(t => t).ToList();

            foreach (var item in discountcode)
            {
                if (DateTime.Compare(item.StartDate.Value.Date, hientai) <= 0 && DateTime.Compare(hientai, item.EndDate.Value.Date) <= 0)
                {
                    var combo = db.DiscountCodes.Find(item.DiscountCodeID);
                    combo.DiscountStatus = true;
                }
                else
                {
                    var combo = db.DiscountCodes.Find(item.DiscountCodeID);
                    combo.DiscountStatus = false;
                }
            }
            db.SaveChanges();
        }

        public MDelete CreateDiscountCode(MDiscountCode mDiscountCode)
        {
            try
            {
                var check = db.DiscountCodes.Where(t => t.Name == mDiscountCode.Name);

                if (check.Count() != 0)
                {
                    MDelete mDelete1 = new MDelete();
                    mDelete1.Check = false;
                    mDelete1.Result = "Trùng mã giảm giá";
                    return mDelete1;
                }
                var discountcode = new DiscountCode();
                discountcode.Name = mDiscountCode.Name;
                discountcode.StartDate = mDiscountCode.StartDate;
                discountcode.EndDate = mDiscountCode.EndDate;
                discountcode.CreateDate = DateTime.Now;
                discountcode.PercentCart = mDiscountCode.PercentCart;
                discountcode.TotalCart = mDiscountCode.TotalCart;
                discountcode.DistcountCount = mDiscountCode.DistcountCount;
                discountcode.DiscountStatus = false;
                db.DiscountCodes.Add(discountcode);
                db.SaveChanges();

                MDelete mDelete = new MDelete();
                mDelete.Check = true;
                return mDelete;
            }
            catch (Exception)
            {

                MDelete mDelete = new MDelete();
                mDelete.Check = false;
                mDelete.Result = "Thêm thất bại";
                return mDelete;
            }
        }

        public MDiscountCode GetByID(int id)
        {
            var dis = db.DiscountCodes.Find(id);

            var m = new MDiscountCode();
            m.DiscountCodeID = id;
            m.Name = dis.Name;
            m.StartDate = dis.StartDate;
            m.EndDate = dis.EndDate;
            m.CreateDate = dis.CreateDate;
            m.PercentCart = dis.PercentCart;
            m.TotalCart = dis.TotalCart;
            m.DistcountCount = dis.DistcountCount;

            return m;
        }

        public MDelete Update(MDiscountCode mDiscountCode)
        {
            try
            {
                var check = db.DiscountCodes.Where(t => t.Name == mDiscountCode.Name&&t.DiscountCodeID!=mDiscountCode.DiscountCodeID);

                if (check.Count() != 0)
                {
                    MDelete mDelete1 = new MDelete();
                    mDelete1.Check = false;
                    mDelete1.Result = "Trùng mã giảm giá";
                    return mDelete1;
                }

                var discount = db.DiscountCodes.Find(mDiscountCode.DiscountCodeID);
                discount.Name = mDiscountCode.Name;
                discount.StartDate = mDiscountCode.StartDate;
                discount.EndDate = mDiscountCode.EndDate;
                discount.PercentCart = mDiscountCode.PercentCart;
                discount.TotalCart = mDiscountCode.TotalCart;
                discount.DistcountCount = mDiscountCode.DistcountCount;
                db.SaveChanges();

                MDelete mDelete = new MDelete();
                mDelete.Check = true;
                return mDelete;

            }
            catch (Exception)
            {
                MDelete mDelete = new MDelete();
                mDelete.Result = "Thêm thất bại";
                mDelete.Check = false;
                return mDelete;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                var discount = db.DiscountCodes.Find(id);

                db.DiscountCodes.Remove(discount);

                db.SaveChanges();

                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
    }
}
