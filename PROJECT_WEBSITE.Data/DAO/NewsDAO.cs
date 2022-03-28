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
    public class NewsDAO
    {
        DbWebsite db;
        public NewsDAO()
        {
            db = new DbWebsite();
        }


        public bool CreateNews(MNews mNews)
        {
            try
            {
                var tintuc = new News();
                tintuc.Name = mNews.Name;
                tintuc.Detail = mNews.Detail;
                tintuc.Image = mNews.Image;
                tintuc.CreateDate = DateTime.Now;
                tintuc.UserID = mNews.UserID;
                tintuc.CategoryNewID = mNews.CategoryNewID;
                tintuc.NewsStatus = true;

                db.News.Add(tintuc);

                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
            
        }

        public List<News> GetAll(int categorynew, string search, ref int totalRecord, int pageIndex, int pageSize)
        {
            if (!string.IsNullOrEmpty(search))
            {
                totalRecord = db.News.Where(t => t.CategoryNewID == categorynew && t.NewsStatus == true&&t.Name.Contains(search)).Count();

                return db.News.Where(t => t.CategoryNewID == categorynew && t.NewsStatus == true && t.Name.Contains(search)).OrderByDescending(t => t.CreateDate).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
            totalRecord = db.News.Where(t => t.CategoryNewID == categorynew && t.NewsStatus == true).Count();

            return db.News.Where(t => t.CategoryNewID == categorynew&&t.NewsStatus==true).OrderByDescending(t=>t.CreateDate).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        public News Detail(int id)
        {
            return db.News.Find(id);
        }

        public IEnumerable<MNews> GetAllAdmin(SearchNews search,int page, int pageSize)
        {
            var lst = db.News.Where(t => t.NewsStatus == true).AsQueryable();

            if (!string.IsNullOrEmpty(search.SearchName))
            {
                lst = lst.Where(t => t.Name.Contains(search.SearchName));
            }if (search.TuNgay.HasValue)
            {
                lst = lst.Where(t => DateTime.Compare(search.TuNgay.Value, DbFunctions.TruncateTime(t.CreateDate.Value).Value) <= 0);
            }
            if (search.DenNgay.HasValue)
            {
                lst = lst.Where(t => DateTime.Compare(DbFunctions.TruncateTime(t.CreateDate.Value).Value, search.DenNgay.Value) <= 0);
            }

            var lstrs = new List<MNews>();

            foreach (var item in lst.ToList())
            {
                MNews mNews = new MNews();
                mNews.STT = lst.ToList().IndexOf(item) + 1;
                mNews.NewsID = item.NewsID;
                mNews.Name = item.Name;
                mNews.CreateDate = item.CreateDate;
                mNews.NameUser = db.Users.Where(t => t.UserID == item.UserID).FirstOrDefault().FullName;
                mNews.CategoryName = db.CategoryNews.Where(t => t.CategoryNewID == item.CategoryNewID).FirstOrDefault().Name;

                lstrs.Add(mNews);
            }
            return lstrs.ToPagedList(page, pageSize);
        }

        public MNews GetDetail(int id)
        {
            var news = db.News.Find(id);

            var newrs = new MNews();
            newrs.NewsID = news.NewsID;
            newrs.Name = news.Name;
            newrs.Image = news.Image;
            newrs.CategoryNewID = news.CategoryNewID;
            newrs.Detail = news.Detail;

            return newrs;
        }

        public bool UpdateNews(MNews mNews)
        {
            try
            {
                var news = db.News.Find(mNews.NewsID);
                news.Name = mNews.Name;
                news.Detail = mNews.Detail;
                news.CategoryNewID = mNews.CategoryNewID;
                news.Image = mNews.Image;

                db.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                var mes = e.ToString();
                return false;
            }
        }

        public bool DeleteNew(int id)
        {
            try
            {
                var news = db.News.Find(id);

                news.NewsStatus = false;

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
