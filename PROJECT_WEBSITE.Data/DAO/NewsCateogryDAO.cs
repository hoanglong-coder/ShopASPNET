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
    public class NewsCateogryDAO
    {
        DbWebsite db;
        public NewsCateogryDAO()
        {
            db = new DbWebsite();
        }

        /// <summary>
        /// Load trang client
        /// </summary>
        /// <returns></returns>
        public List<CategoryNew> GetAll()
        {
            return db.CategoryNews.Where(t=>t.CategoryStatus==true).Select(t => t).ToList();
        }

        /// <summary>
        /// Load select thêm tin tức
        /// </summary>
        /// <returns></returns>
        public List<MCategoryNews> GetAllAdmin()
        {
            var lst = db.CategoryNews.Where(t => t.CategoryStatus == true).Select(t => new MCategoryNews() { 
            CategoryNewID = t.CategoryNewID,
            Name = t.Name                   
            }).ToList();

            return lst;
        }

        /// <summary>
        /// Danh sách loại tin tức
        /// </summary>
        /// <param name="searchcategory"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<MCategoryNews> GetAllAdminPaging(SearchNews searchcategory, int page, int pageSize)
        {
            var lst = db.CategoryNews.Where(t => t.CategoryStatus == true).OrderBy(t=>t.Display).AsQueryable();

            if (!string.IsNullOrEmpty(searchcategory.SearchName))
            {
                lst = lst.Where(t => t.Name.Contains(searchcategory.SearchName));
            }
            if (searchcategory.TuNgay.HasValue)
            {
                lst = lst.Where(t => DateTime.Compare(searchcategory.TuNgay.Value, t.CreateDate.Value) < 0);
            }
            if (searchcategory.DenNgay.HasValue)
            {
                lst = lst.Where(t => DateTime.Compare(t.CreateDate.Value, searchcategory.DenNgay.Value) < 0);
            }

            var lstrs = new List<MCategoryNews>();
            foreach (var item in lst.ToList())
            {
                MCategoryNews mNews = new MCategoryNews();
                mNews.STT = lst.ToList().IndexOf(item) + 1;
                mNews.Display = item.Display;
                mNews.CategoryNewID = item.CategoryNewID;
                mNews.Name = item.Name;
                mNews.CreateDate = item.CreateDate;
                mNews.CategoryStatus = item.CategoryStatus;
                mNews.CountNews = db.News.Where(t => t.CategoryNewID == item.CategoryNewID&&t.NewsStatus==true).Count();
                lstrs.Add(mNews);
            }
            return lstrs.ToPagedList(page, pageSize);
        }

        public bool CreateNewCateogry(MCategoryNews mCategoryNews)
        {
            try
            {
                var category = new CategoryNew();
                category.Name = mCategoryNews.Name;
                category.Display = mCategoryNews.Display;
                category.CreateDate = DateTime.Now;
                category.CategoryStatus = true;

                db.CategoryNews.Add(category);

                db.SaveChanges();

                return true;
            }
            catch (Exception)
            {

                return false;
            }
            
        }

        public MCategoryNews GetDetail(int id)
        {
            var category = db.CategoryNews.Find(id);
            MCategoryNews mCategoryNews = new MCategoryNews();
            mCategoryNews.CategoryNewID = category.CategoryNewID;
            mCategoryNews.Name = category.Name;
            mCategoryNews.Display = category.Display;

            return mCategoryNews;
        }

        public bool EditCategory(MCategoryNews mCategoryNews)
        {
            try
            {
                var category = db.CategoryNews.Find(mCategoryNews.CategoryNewID);
                category.Name = mCategoryNews.Name;
                category.Display = mCategoryNews.Display;

                db.SaveChanges();
                

                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool DeleteCategory(int id)
        {
            try
            {
                var category = db.CategoryNews.Find(id);
                category.CategoryStatus = false;

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
