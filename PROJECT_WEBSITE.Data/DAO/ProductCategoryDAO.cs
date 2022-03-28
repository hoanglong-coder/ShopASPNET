using PagedList;
using PROJECT_WEBSITE.Data.EF;
using PROJECT_WEBSITE.Data.ModelCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PROJECT_WEBSITE.Data.DAO
{
    public class ProductCategoryDAO
    {
        DbWebsite db;
        public ProductCategoryDAO()
        {
            db = new DbWebsite();
        }

        //Cho menu
        public List<ProductCategory> ListCategory()
        {
            return db.ProductCategories.Where(t => t.ShowOnHome == true && t.ProductCategoryStatus == true).OrderBy(t => t.Display).ToList();
        }
        public List<ProductCategory> ListCate()
        {
            return db.ProductCategories.Where(t => t.ProductCategoryStatus == true).OrderBy(t => t.Display).ToList();
        }

        /// <summary>
        /// Admin
        /// </summary>
        /// <returns></returns>
        public List<MProductCategory> ListCategoryAdmin()
        {
            var lst = db.ProductCategories.Where(t => t.ProductCategoryStatus == true).OrderBy(t => t.Display).Select(t => new MProductCategory() { 
            ProductCategoryID = t.ProductCategoryID,
            Name = t.Name,
            CreateDate = t.CreateDate,
            Display = t.Display,
            Image = t.Image,
            MetaTitle = t.MetaTitle,
            ParentID = t.ParentID,
            ProductCategoryStatus = t.ProductCategoryStatus,
            ShowOnHome = t.ShowOnHome                       
            }).ToList();

            return lst;
        }

        public IEnumerable<MProductCategory> ListCategoryBase(SearchNews search,int page, int pageSize)
        {
            var lst = db.ProductCategories.Where(t => t.ProductCategoryStatus == true && t.ParentID == 0).OrderBy(t => t.Display).AsQueryable();

            var listrs = new List<MProductCategory>();

            if (!string.IsNullOrEmpty(search.SearchName))
            {

                var productcategory = CutProductCategory(search.SearchName);

                if (productcategory.HasValue)
                {
                    lst = lst.Where(t => t.ProductCategoryID == productcategory.Value);
                }
                else
                {
                    lst = lst.Where(t => t.Name.Contains(search.SearchName));
                }
                
            }
            if (search.TuNgay.HasValue)
            {
                lst = lst.Where(t => DateTime.Compare(search.TuNgay.Value, t.CreateDate.Value) < 0);
            }
            if (search.DenNgay.HasValue)
            {
                lst = lst.Where(t => DateTime.Compare(t.CreateDate.Value, search.DenNgay.Value) < 0);
            }

            foreach (var item in lst)
            {
                MProductCategory m = new MProductCategory();
                m.STT = lst.ToList().IndexOf(item) + 1;
                m.ProductCategoryID = item.ProductCategoryID;
                m.Name = item.Name;
                m.CreateDate = item.CreateDate;
                m.Display = item.Display;
                m.Image = item.Image;
                m.MetaTitle = item.MetaTitle;
                m.ParentID = item.ParentID;
                m.ProductCategoryStatus = item.ProductCategoryStatus;
                m.ShowOnHome = item.ShowOnHome;
                m.SLProduct = TongSLsanPham(item.ProductCategoryID);
                listrs.Add(m);

            }
           

            return listrs.ToPagedList(page, pageSize);
        }

        public int? CutProductCategory(string input)
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

        public IEnumerable<MProductCategory> ListCategoryCap2(SearchNews search,int page, int pageSize,int id)
        {
            var lst = db.ProductCategories.Where(t => t.ProductCategoryStatus == true && t.ParentID == id).OrderBy(t => t.Display).AsQueryable();

            var listrs = new List<MProductCategory>();

            if (!string.IsNullOrEmpty(search.SearchName))
            {

                var productcategory = CutProductCategory(search.SearchName);

                if (productcategory.HasValue)
                {
                    lst = lst.Where(t => t.ProductCategoryID == productcategory.Value);
                }
                else
                {
                    lst = lst.Where(t => t.Name.Contains(search.SearchName));
                }

            }
            if (search.TuNgay.HasValue)
            {
                lst = lst.Where(t => DateTime.Compare(search.TuNgay.Value, t.CreateDate.Value) < 0);
            }
            if (search.DenNgay.HasValue)
            {
                lst = lst.Where(t => DateTime.Compare(t.CreateDate.Value, search.DenNgay.Value) < 0);
            }


            foreach (var item in lst)
            {
                MProductCategory m = new MProductCategory();
                m.STT = lst.ToList().IndexOf(item) + 1;
                m.ProductCategoryID = item.ProductCategoryID;
                m.Name = item.Name;
                m.CreateDate = item.CreateDate;
                m.Display = item.Display;
                m.Image = item.Image;
                m.MetaTitle = item.MetaTitle;
                m.ParentID = item.ParentID;
                m.ProductCategoryStatus = item.ProductCategoryStatus;
                m.ShowOnHome = item.ShowOnHome;
                m.SLProduct = TongSLsanPhamCap2(item.ProductCategoryID);
                listrs.Add(m);

            }


            return listrs.ToPagedList(page, pageSize);
        }


        public int? DemSLProduct(int ProductCategoryID)
        {
            int Rs = db.Products.Where(t => t.ProductCategoryID == ProductCategoryID).Count();

            return Rs;
        }

        public int? TongSLsanPham(int ProductCategoryID)
        {
            var category = db.ProductCategories.Where(t => t.ParentID == ProductCategoryID).ToList();

            int? sum1 = DemSLProduct(ProductCategoryID);

            int? sum2 = 0;

            foreach (var item in category)
            {
                sum2+= DemSLProduct(item.ProductCategoryID);
            }

            return sum1 + sum2;
        }
        public int? TongSLsanPhamCap2(int ProductCategoryID)
        {
            var category = db.ProductCategories.Where(t => t.ProductCategoryID == ProductCategoryID).ToList();

            int? sum2 = 0;

            foreach (var item in category)
            {
                sum2 += DemSLProduct(item.ProductCategoryID);
            }

            return sum2;
        }


        public bool CreateCategoryBase(MProductCategory mProductCategory)
        {
            try
            {
                var category = new ProductCategory();

                category.Name = mProductCategory.Name;
                category.MetaTitle = GenMetaTitle(mProductCategory.Name);
                category.Display = mProductCategory.Display;
                category.ParentID = 0;
                category.CreateDate = DateTime.Now;
                category.Image = mProductCategory.Image;
                category.ShowOnHome = false;
                category.ProductCategoryStatus = true;

                db.ProductCategories.Add(category);

                db.SaveChanges();


                return true;
            }
            catch (System.Exception)
            {

                return false;
            }
        }

        public bool CreateCategory(MProductCategory mProductCategory)
        {
            try
            {
                var category = new ProductCategory();

                category.Name = mProductCategory.Name;
                category.MetaTitle = GenMetaTitle(mProductCategory.Name);
                category.Display = mProductCategory.Display;
                category.CreateDate = DateTime.Now;
                category.ParentID = mProductCategory.ParentID;
                category.ShowOnHome = false;
                category.ProductCategoryStatus = true;

                db.ProductCategories.Add(category);

                db.SaveChanges();


                return true;
            }
            catch (System.Exception)
            {

                return false;
            }
        }

        public string GenMetaTitle(string Name)
        {

            var convert = RemoveVietnameseTone(Name.ToLower());
            string[] Phase1 = convert.Split(' ');
            string Phase2 = "";
            foreach (var item in Phase1)
            {
                Phase2 += item.ToLower() + "-";
            }
            string rs = Phase2.TrimEnd('-');

            return rs;
        }

        public static string RemoveVietnameseTone(string text)
        {
            string result = text.ToLower();
            result = Regex.Replace(result, "à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ|/g", "a");
            result = Regex.Replace(result, "è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ|/g", "e");
            result = Regex.Replace(result, "ì|í|ị|ỉ|ĩ|/g", "i");
            result = Regex.Replace(result, "ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ|/g", "o");
            result = Regex.Replace(result, "ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ|/g", "u");
            result = Regex.Replace(result, "ỳ|ý|ỵ|ỷ|ỹ|/g", "y");
            result = Regex.Replace(result, "đ", "d");
            return result;
        }


        public MProductCategory GetCategoryByID(int id)
        {
            var category = db.ProductCategories.Find(id);

            var rs = new MProductCategory();

            rs.ProductCategoryID = category.ProductCategoryID;
            rs.Name = category.Name;
            rs.Display = category.Display;
            rs.Image = category.Image;

            return rs;
        }
    
        public bool UpdateCateogryBase(MProductCategory mProductCategory)
        {
            try
            {
                var category = db.ProductCategories.Find(mProductCategory.ProductCategoryID);
                category.Name = mProductCategory.Name;
                category.Display = mProductCategory.Display;
                category.Image = mProductCategory.Image;

                db.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public int? UpdateCateogry(MProductCategory mProductCategory)
        {
            try
            {
                var category = db.ProductCategories.Find(mProductCategory.ProductCategoryID);
                category.Name = mProductCategory.Name;
                category.Display = mProductCategory.Display;
                category.Image = mProductCategory.Image;

                db.SaveChanges();

                return category.ParentID;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public MDelete DeleteCategoryBase(int id)
        {
            var check = db.Products.Where(t => t.ProductCategoryID == id).Count();
            var lst = db.ProductCategories.Where(t => t.ParentID == id).ToList();
            if (check != 0)
            {
                var rs1 = new MDelete();
                rs1.Check = false;
                rs1.Result = "Không thể xóa do có sản phẩm đang thuộc loại này";
                return rs1;
            }
            foreach (var item in lst)
            {
                var check1 = db.Products.Where(t => t.ProductCategoryID == item.ProductCategoryID).Count();
                if (check1 != 0)
                {
                    var rs1 = new MDelete();
                    rs1.Check = false;
                    rs1.Result = "Không thể xóa do có sản phẩm đang thuộc loại này";
                    return rs1;
                }
            }
            var category = db.ProductCategories.Find(id);

            category.ProductCategoryStatus = false;

            db.SaveChanges();

            var rs = new MDelete();
            rs.Check = true;
            return rs;
        }

        public MDelete DeleteCategoryCap2(int id)
        {
            var check = db.Products.Where(t => t.ProductCategoryID == id).Count();
            if (check != 0)
            {
                var rs1 = new MDelete();
                rs1.Check = false;
                rs1.Result = "Không thể xóa do có sản phẩm đang thuộc loại này";
                return rs1;
            }
            var category = db.ProductCategories.Find(id);

            category.ProductCategoryStatus = false;

            db.SaveChanges();

            var rs = new MDelete();
            rs.Check = true;
            return rs;
        }
    }
}
