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
    public class FooterDAO
    {
        DbWebsite db;

        public FooterDAO()
        {
            db = new DbWebsite();
        }

        public IEnumerable<MFooterCategory> GetCategogryFooter(SearchNews search, int page, int pageSize)
        {
            var lst = db.FooterCategories.AsQueryable();

            if (!string.IsNullOrEmpty(search.SearchName))
            {
                lst = lst.Where(t => t.NameCategory.Contains(search.SearchName));
            }
            if (search.TuNgay.HasValue)
            {
                lst = lst.Where(t => DateTime.Compare(search.TuNgay.Value, DbFunctions.TruncateTime(t.CreateDate.Value).Value) <= 0);
            }
            if (search.DenNgay.HasValue)
            {
                lst = lst.Where(t => DateTime.Compare(DbFunctions.TruncateTime(t.CreateDate.Value).Value, search.DenNgay.Value) <= 0);
            }

            List<MFooterCategory> lstrs = new List<MFooterCategory>();

            foreach (var item in lst)
            {
                MFooterCategory m = new MFooterCategory();
                m.STT = lst.ToList().IndexOf(item) + 1;
                m.NameCategory = item.NameCategory;
                m.Display = item.Display;
                m.FooterCategoryID = item.FooterCategoryID;
                m.CreateDate = item.CreateDate;
                lstrs.Add(m);
            }
            return lstrs.OrderBy(t=>t.Display).ToPagedList(page,pageSize);
        }

        public IEnumerable<MFooter> GetFooter(SearchNews search, int page, int pageSize)
        {
            var lst = db.Footers.AsQueryable();

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

            List<MFooter> rs = new List<MFooter>();

            foreach (var item in lst)
            {
                MFooter m = new MFooter();
                m.STT = lst.ToList().IndexOf(item) + 1;
                m.FooterID = item.FooterID;
                m.Name = item.Name;
                m.CreateDate = item.CreateDate;
                m.FooterCategoryName = db.FooterCategories.Find(item.FooterCategoryID).NameCategory;
                rs.Add(m);
            }

            return rs.OrderByDescending(t => t.CreateDate).ToPagedList(page, pageSize);
        }

        public bool CreateFooter(MFooter footer)
        {
            try
            {
                var f = new Footer();
                f.Name = footer.Name;
                f.CreateDate = DateTime.Now;
                f.FooterCategoryID = footer.FooterCategoryID;
                f.Detail = footer.Detail;

                db.Footers.Add(f);

                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public MFooter GetByID(int id)
        {
            var footer = db.Footers.Find(id);

            var f = new MFooter();
            f.FooterID = id;
            f.Name = footer.Name;
            f.CreateDate = DateTime.Now;
            f.FooterCategoryID = footer.FooterCategoryID;
            f.Detail = footer.Detail;

            return f;
        }

        public bool DeleteFooter(int id)
        {
            try
            {
                var footer = db.Footers.Find(id);

                db.Footers.Remove(footer);

                db.SaveChanges();

                return true;

            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool UpdateFooter(MFooter footer)
        {
            try
            {
                var f = db.Footers.Find(footer.FooterID);
                f.Name = footer.Name;
                f.FooterCategoryID = footer.FooterCategoryID;
                f.Detail = footer.Detail;

                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }


        public bool CreateFooterCate(MFooterCategory footer)
        {
            try
            {
                var f = new FooterCategory();
                f.NameCategory = footer.NameCategory;
                f.CreateDate = DateTime.Now;
                f.Display = footer.Display;

                db.FooterCategories.Add(f);

                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public MFooterCategory GetByIDCate(int id)
        {
            var footer = db.FooterCategories.Find(id);

            var f = new MFooterCategory();
            f.FooterCategoryID = id;
            f.NameCategory = footer.NameCategory;
            f.Display = footer.Display;
            return f;
        }

        public bool UpdateFooterCate(MFooterCategory footer)
        {
            try
            {
                var f = db.FooterCategories.Find(footer.FooterCategoryID);
                f.NameCategory = footer.NameCategory;
                f.Display = footer.Display;

                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool DeleteFooterCate(int id)
        {
            try
            {
                var footer = db.FooterCategories.Find(id);

                db.FooterCategories.Remove(footer);

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
