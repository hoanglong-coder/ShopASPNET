using PagedList;
using PROJECT_WEBSITE.Data.EF;
using PROJECT_WEBSITE.Data.ModelCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml.Linq;

namespace PROJECT_WEBSITE.Data.DAO
{
    public class ProductDAO
    {
        DbWebsite db;
        public ProductDAO()
        {
            db = new DbWebsite();
        }
        public List<MProduct> ListProduct()
        {
            CapNhatPricePromotion();
            var listproduct = db.Products.Where(t => t.ProductStatus == true && t.Display == true && t.ProductComboID.HasValue == false).OrderBy(x => x.CreateDate).Select(t => new MProduct()
            {

                ProductID = t.ProductID,
                Name = t.Name,
                ParentProductID = t.ParentProductID,
                Image = t.Image,
                MoreImage = t.MoreImage,
                PriceOut = t.PriceOut,
                CountProduct = t.CountProduct,
                Pricewholesale = t.Pricewholesale,
                UnitID = t.UnitID,
                CreateDate = t.CreateDate,
                ProductCategoryID = t.ProductCategoryID,
                ProductStatus = t.ProductStatus,
                Display = t.Display,
                MetaTitle = t.MetaTitle,
                PricewholesalePromotion = (db.ProductPricePromotions.Where(e => e.ProductID == t.ProductID && e.PromotionStatus == true).Count() != 0 ? db.ProductPricePromotions.Where(e => e.ProductID == t.ProductID && e.PromotionStatus == true).FirstOrDefault().PricewholesalePromotion : null),
                PricePromotion = (db.ProductPricePromotions.Where(e => e.ProductID == t.ProductID && e.PromotionStatus == true).Count() != 0 ? db.ProductPricePromotions.Where(e => e.ProductID == t.ProductID && e.PromotionStatus == true).FirstOrDefault().PricePromotion : null)

            }).ToList();

            return listproduct;
        }

        //List combo
        public List<MProductCombo> listdeals()
        {

            var listproductcombo = db.Products.Where(t => t.ProductStatus == true && t.Display == true && t.ProductComboID.HasValue == true).OrderBy(x => x.CreateDate).Select(t => t);

            var lstrs = new List<MProductCombo>();


            foreach (var item in listproductcombo)
            {
               if (db.ProductComboes.Find(item.ProductComboID).ComboStatus.Value == true)
                {
                    if (checkcombo(item.ProductComboID))
                    {
                        MProductCombo m = new MProductCombo();
                        m.STT = listproductcombo.ToList().IndexOf(item) + 1;
                        m.ProductID = item.ProductID;
                        m.ProductComboID = item.ProductComboID;
                        m.Name = item.Name;
                        m.MetaTitle = item.MetaTitle;
                        m.Image = item.Image;
                        m.MoreImage = item.MoreImage;
                        m.PriceOut = item.PriceOut;
                        m.PricePromotion = (db.ProductPricePromotions.Where(e => e.ProductID == m.ProductID && e.PromotionStatus == true).Count() != 0 ? db.ProductPricePromotions.Where(e => e.ProductID == m.ProductID && e.PromotionStatus == true).FirstOrDefault().PricePromotion : null);
                        m.Pricewholesale = item.Pricewholesale;
                        m.PricewholesalePromotion = (db.ProductPricePromotions.Where(e => e.ProductID == m.ProductID && e.PromotionStatus == true).Count() != 0 ? db.ProductPricePromotions.Where(e => e.ProductID == m.ProductID && e.PromotionStatus == true).FirstOrDefault().PricewholesalePromotion : null);
                        m.CountProduct = item.CountProduct;
                        m.CreateDate = item.CreateDate;
                        m.ProductStatus = item.ProductStatus;
                        m.ComboStatus = db.ProductComboes.Find(m.ProductComboID).ComboStatus;
                        m.DisplayProduct = item.Display;
                        m.DisplayProductComBo = db.ProductComboes.Find(m.ProductComboID).Display;
                        m.StartDate = db.ProductComboes.Find(m.ProductComboID).StartDate;
                        m.EndDate = db.ProductComboes.Find(m.ProductComboID).EndDate;
                        lstrs.Add(m);
                    }   
                }             
            }

            return lstrs;
        }


        //checkcombo 
        public bool checkcombo(int? idproductcombo)
        {
            var check = db.ProductComboDetails.Where(t => t.ProductComboID == idproductcombo);

            if (check.Count() != 0)
            {
                if (checkproductiscount(idproductcombo))
                {
                    return true;
                }
                return false;              
            }
            return false;
        }

        //checkcombo 
        public bool checkproductiscount(int? idproductcombo)
        {
            var check = db.ProductComboDetails.Where(t => t.ProductComboID == idproductcombo);

            foreach (var item in check)
            {
                var product = db.Products.Find(item.ProductID);
                if (product.CountProduct == 0)
                {
                    return false;
                }
            }
            return true;
        }

        public void CapNhatPricePromotion()
        {
            DateTime hientai = DateTime.Now.Date;

            var products = db.Products.Where(t =>t.ProductStatus == true).Select(t => t).ToList();

            foreach (var item in products)
            {
                var promotionfalses = db.ProductPricePromotions.Where(t =>t.ProductID == item.ProductID).Select(t => t).ToList();
                if (promotionfalses.Count() != 0)
                {
                    foreach (var item1 in promotionfalses)
                    {
                        if (DateTime.Compare(item1.StartDate.Value.Date,hientai)<=0&&DateTime.Compare(hientai,item1.EndDate.Value.Date)<=0)
                        {
                            var promotion = db.ProductPricePromotions.Find(item1.PricePromotionID);
                            promotion.PromotionStatus = true;
                        }else
                        {
                            var promotion = db.ProductPricePromotions.Find(item1.PricePromotionID);
                            promotion.PromotionStatus = false;
                        }
                    }
                }
            }
            db.SaveChanges();
        }

        //Phân trang sản phẩm
        public List<MProduct> PaginationProduct(long id, string query, string sort, string category, string price, ref int totalRecord, int pageIndex = 1, int pageSize = 2)
        {
            var model = new List<MProduct>();
            var te = new List<MProduct>();
            totalRecord = GetListProduct(id).Count();
            int dem = 0;
            if (category != null && category != "null")
            {
                string[] temp = category.Split('_');

                foreach (var item in temp)
                {
                    if (item != "")
                    {
                        var temp1 = GetListProduct(int.Parse(item));
                        te = te.Concat(temp1).ToList();
                        dem += temp1.Count();
                    }
                }
            }
            if (price != null && price != "null")
            {
                string[] temp = price.Split('_');

                if (temp.Length == 2 && te.Count() != 0)
                {
                    int count = GetListProduct(id).Concat(te).Where(t => t.PriceOut >= int.Parse(temp[0]) && t.PriceOut <= int.Parse(temp[1])).Count();
                    if (sort == "AZ")
                    {
                        model = GetListProduct(id).Concat(te).Where(t => t.PriceOut >= int.Parse(temp[0]) && t.PriceOut <= int.Parse(temp[1])).OrderBy(t => t.Name).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                    }
                    if (sort == "ZA")
                    {
                        model = GetListProduct(id).Concat(te).Where(t => t.PriceOut >= int.Parse(temp[0]) && t.PriceOut <= int.Parse(temp[1])).OrderByDescending(t => t.Name).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                    }
                    if (sort == "BS")
                    {
                        model = GetListProduct(id).Concat(te).Where(t => t.PriceOut >= int.Parse(temp[0]) && t.PriceOut <= int.Parse(temp[1])).OrderByDescending(t => t.PriceOut).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                    }
                    if (sort == "SB")
                    {
                        model = GetListProduct(id).Concat(te).Where(t => t.PriceOut >= int.Parse(temp[0]) && t.PriceOut <= int.Parse(temp[1])).OrderBy(t => t.PriceOut).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                    }
                    if (sort == "New" || sort == "" || sort == null)
                    {
                        model = GetListProduct(id).Concat(te).Where(t => t.PriceOut >= int.Parse(temp[0]) && t.PriceOut <= int.Parse(temp[1])).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                    }

                    totalRecord = count;
                    return model;
                }
                if (temp.Length == 2 && te.Count() == 0)
                {
                    int count = GetListProduct(id).Where(t => t.PriceOut >= int.Parse(temp[0]) && t.PriceOut <= int.Parse(temp[1])).Count();
                    if (sort == "AZ")
                    {
                        model = GetListProduct(id).Where(t => t.PriceOut >= int.Parse(temp[0]) && t.PriceOut <= int.Parse(temp[1])).OrderBy(t => t.Name).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                    }
                    if (sort == "ZA")
                    {
                        model = GetListProduct(id).Where(t => t.PriceOut >= int.Parse(temp[0]) && t.PriceOut <= int.Parse(temp[1])).OrderByDescending(t => t.Name).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                    }
                    if (sort == "BS")
                    {
                        model = GetListProduct(id).Where(t => t.PriceOut >= int.Parse(temp[0]) && t.PriceOut <= int.Parse(temp[1])).OrderByDescending(t => t.PriceOut).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                    }
                    if (sort == "SB")
                    {
                        model = GetListProduct(id).Where(t => t.PriceOut >= int.Parse(temp[0]) && t.PriceOut <= int.Parse(temp[1])).OrderBy(t => t.PriceOut).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                    }
                    if (sort == "New" || sort == "" || sort == null)
                    {
                        model = GetListProduct(id).Where(t => t.PriceOut >= int.Parse(temp[0]) && t.PriceOut <= int.Parse(temp[1])).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                    }
                    totalRecord = count;
                    return model;
                }
            }
            int count1 = 0;
            if (sort == "AZ")
            {
                model = GetListProduct(id).Concat(te).OrderBy(t => t.Name).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
            if (sort == "ZA")
            {
                model = GetListProduct(id).Concat(te).OrderByDescending(t => t.Name).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
            if (sort == "BS")
            {
                model = GetListProduct(id).Concat(te).OrderByDescending(t => t.PriceOut).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
            if (sort == "SB")
            {
                model = GetListProduct(id).Concat(te).OrderBy(t => t.PriceOut).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
            if (sort == "New" || sort == "" || sort == null)
            {
                if (query != null)
                {
                    count1 = GetListProduct(id).Concat(te).Where(t => t.Name.Contains(query)).Skip((pageIndex - 1) * pageSize).Take(pageSize).Count();
                    model = GetListProduct(id).Concat(te).Where(t => t.Name.Contains(query)).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                    totalRecord = count1;
                    return model;
                }
                else
                {

                    model = GetListProduct(id).Concat(te).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                }
            }

            totalRecord = totalRecord + dem;
            return model;
        }

        //Danh sách sản phẩm theo loại
        public List<MProduct> GetListProduct(long id)
        {
            var category = new ProductCategoryDAO().ListCate().Where(t => t.ParentID == id);
            if (category.Count() != 0)
            {
                List<MProduct> model1 = new List<MProduct>();
                foreach (var item in category)
                {
                    List<MProduct> temp = ListProduct().Where(t => t.ProductCategoryID == item.ProductCategoryID).ToList();
                    model1 = model1.Concat(temp).ToList();
                }
                return model1;
            }
            var model = ListProduct().Where(t => t.ProductCategoryID == id).ToList();
            return model;
        }

        //Chi tiết sản phẩm
        public MProduct DetailProduct(int id)
        {
            var t = db.Products.Where(p => p.ProductID==id).FirstOrDefault();
            var Mproduct = new MProduct();
            Mproduct.ProductID = t.ProductID;
            Mproduct.Name = t.Name;
            Mproduct.ParentProductID = t.ParentProductID;
            Mproduct.Image = t.Image;
            Mproduct.MoreImage = t.MoreImage;
            Mproduct.PriceOut = t.PriceOut;
            Mproduct.Pricewholesale = t.Pricewholesale;
            Mproduct.CountProduct = t.CountProduct;
            Mproduct.UnitID = t.UnitID;
            Mproduct.CreateDate = t.CreateDate;
            Mproduct.ProductCategoryID = t.ProductCategoryID;
            Mproduct.ProductStatus = t.ProductStatus;
            Mproduct.Display = t.Display;
            Mproduct.MetaTitle = t.MetaTitle;
            Mproduct.PricewholesalePromotion = (db.ProductPricePromotions.Where(e => e.ProductID == t.ProductID && e.PromotionStatus == true).Count() != 0 ? db.ProductPricePromotions.Where(e => e.ProductID == t.ProductID && e.PromotionStatus == true).FirstOrDefault().PricewholesalePromotion : null);
            Mproduct.PricePromotion = (db.ProductPricePromotions.Where(e => e.ProductID == t.ProductID && e.PromotionStatus == true).Count() != 0 ? db.ProductPricePromotions.Where(e => e.ProductID == t.ProductID && e.PromotionStatus == true).FirstOrDefault().PricePromotion : null);
            //Detail
            Mproduct.TradeMark = db.ProductDetails.Where(e => e.ProductDetailID == t.ProductID).Count() != 0 ? db.ProductDetails.Where(e => e.ProductDetailID == t.ProductID).FirstOrDefault().TradeMark : "";
            Mproduct.TradeOrigin = db.ProductDetails.Where(e => e.ProductDetailID == t.ProductID).Count() != 0 ? db.ProductDetails.Where(e => e.ProductDetailID == t.ProductID).FirstOrDefault().TradeOrigin : "";
            Mproduct.Ingredient = db.ProductDetails.Where(e => e.ProductDetailID == t.ProductID).Count() != 0 ? db.ProductDetails.Where(e => e.ProductDetailID == t.ProductID).FirstOrDefault().Ingredient : "";
            Mproduct.Production = db.ProductDetails.Where(e => e.ProductDetailID == t.ProductID).Count() != 0 ? db.ProductDetails.Where(e => e.ProductDetailID == t.ProductID).FirstOrDefault().Production : "";
            Mproduct.Expiry = db.ProductDetails.Where(e => e.ProductDetailID == t.ProductID).Count() != 0 ? db.ProductDetails.Where(e => e.ProductDetailID == t.ProductID).FirstOrDefault().Expiry : "";
            Mproduct.UserManual = db.ProductDetails.Where(e => e.ProductDetailID == t.ProductID).Count() != 0 ? db.ProductDetails.Where(e => e.ProductDetailID == t.ProductID).FirstOrDefault().UserManual : "";
            Mproduct.CareInstructions = db.ProductDetails.Where(e => e.ProductDetailID == t.ProductID).Count() != 0 ? db.ProductDetails.Where(e => e.ProductDetailID == t.ProductID).FirstOrDefault().CareInstructions : "";
            Mproduct.Packing = db.ProductDetails.Where(e => e.ProductDetailID == t.ProductID).Count() != 0 ? db.ProductDetails.Where(e => e.ProductDetailID == t.ProductID).FirstOrDefault().Packing : "";
            Mproduct.Discription = db.ProductDetails.Where(e => e.ProductDetailID == t.ProductID).Count() != 0 ? db.ProductDetails.Where(e => e.ProductDetailID == t.ProductID).FirstOrDefault().Discription : "";
            return Mproduct;
        }





        /// <summary>
        /// admin
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MProduct> ListProductAdmin(int page, int pageSize,SearchProduct search)
        {
            CapNhatPricePromotion();
            var listproduct = db.Products.Where(t => t.ProductComboID.HasValue == false && t.ProductStatus == true).OrderByDescending(x => x.CreateDate).Select(t => t).AsQueryable();


            if (!string.IsNullOrEmpty(search.querysearch))
            {
                var masanpham = CutMaSanPham(search.querysearch);

                if (masanpham.HasValue)
                {
                    listproduct = listproduct.Where(t => t.ProductID==masanpham.Value);
                }
                else
                {
                    listproduct = listproduct.Where(t => t.Name.Contains(search.querysearch));
                }
            }
            if (search.TinhTrang.HasValue)
            {
                listproduct = listproduct.Where(t => t.Display.Value == search.TinhTrang.Value);
            }
            if (search.ProductCategoryID.HasValue)
            {
                listproduct = listproduct.Where(t => t.ProductCategoryID == search.ProductCategoryID.Value);
            }
            if (search.SoLuong.HasValue)
            {
                if (search.SoLuong.Value == true)
                {
                    listproduct = listproduct.Where(t => t.CountProduct.Value!=0);
                }else
                {
                    listproduct = listproduct.Where(t => t.CountProduct.Value == 0);
                }
            }
            if (search.UnitID.HasValue)
            {
                listproduct = listproduct.Where(t => t.UnitID.Value==search.UnitID.Value);
            }
            if (search.GiaLeTu.HasValue && search.GiaLeDen.HasValue)
            {
                listproduct = listproduct.Where(t => t.PriceOut.Value>= search.GiaLeTu.Value&&t.PriceOut.Value<=search.GiaLeDen.Value);
            }
            if (search.GiaLeTu.HasValue)
            {
                listproduct = listproduct.Where(t => t.PriceOut.Value >= search.GiaLeTu.Value);
            }
            if (search.GiaLeDen.HasValue)
            {
                listproduct = listproduct.Where(t => t.PriceOut.Value <= search.GiaLeDen.Value);
            }
            if (search.GiaSiTu.HasValue && search.GiaSiDen.HasValue)
            {
                listproduct = listproduct.Where(t => t.Pricewholesale.Value >= search.GiaSiTu.Value && t.Pricewholesale.Value <= search.GiaSiDen.Value);
            }
            if (search.GiaSiTu.HasValue)
            {
                listproduct = listproduct.Where(t => t.Pricewholesale.Value >= search.GiaSiTu.Value );
            }
            if (search.GiaSiDen.HasValue)
            {
                listproduct = listproduct.Where(t =>t.Pricewholesale.Value <= search.GiaSiDen.Value);
            }

            List<MProduct> mProducts = new List<MProduct>();

            foreach (var t in listproduct)
            {
                var mproduct = new MProduct();
                mproduct.STT = listproduct.ToList().IndexOf(t)+1;                
                mproduct.ProductID = t.ProductID;
                mproduct.Name = t.Name;
                mproduct.ParentProductID = t.ParentProductID;
                mproduct.Image = t.Image;
                mproduct.MoreImage = t.MoreImage;
                mproduct.PriceOut = t.PriceOut;
                mproduct.CountProduct = t.CountProduct;
                mproduct.Pricewholesale = t.Pricewholesale;
                mproduct.UnitID = t.UnitID;
                mproduct.CreateDate = t.CreateDate;
                mproduct.ProductCategoryID = t.ProductCategoryID;
                mproduct.ProductStatus = t.ProductStatus;
                mproduct.Display = t.Display;
                mproduct.MetaTitle = t.MetaTitle;
                mproduct.SupplierName = GetNhaSanXuat(t.ProductID);
                mproduct.UnitName = db.ProductUnits.Where(e => e.UnitID == t.UnitID).FirstOrDefault().Name;
                mproduct.CategoryName = db.ProductCategories.Where(e => e.ProductCategoryID == t.ProductCategoryID).FirstOrDefault().Name;
                mproduct.PricewholesalePromotion = (db.ProductPricePromotions.Where(e => e.ProductID == t.ProductID && e.PromotionStatus == true).Count() != 0 ? db.ProductPricePromotions.Where(e => e.ProductID == t.ProductID && e.PromotionStatus == true).FirstOrDefault().PricewholesalePromotion : null);
                mproduct.PricePromotion = (db.ProductPricePromotions.Where(e => e.ProductID == t.ProductID && e.PromotionStatus == true).Count() != 0 ? db.ProductPricePromotions.Where(e => e.ProductID == t.ProductID && e.PromotionStatus == true).FirstOrDefault().PricePromotion : null);
                mproduct.KyTuCaterory = getnamecategorykytu(mproduct.CategoryName);
                mProducts.Add(mproduct);
            }          
            return mProducts.ToPagedList(page, pageSize);
        }

        public List<MProduct> ProductTOPK(List<int> id)
        {
            List<MProduct> mProducts = new List<MProduct>();
            int dem = 1;
            foreach (var item in id)
            {
                var t = db.Products.Find(item);

                MProduct mproduct = new MProduct();
                mproduct.STT = dem;
                mproduct.ProductID = t.ProductID;
                mproduct.Name = t.Name;
                mproduct.Name = t.Name;
                mproduct.ParentProductID = t.ParentProductID;
                mproduct.Image = t.Image;
                mproduct.MoreImage = t.MoreImage;
                mproduct.PriceOut = t.PriceOut;
                mproduct.CountProduct = t.CountProduct;
                mproduct.Pricewholesale = t.Pricewholesale;
                mproduct.UnitID = t.UnitID;
                mproduct.CreateDate = t.CreateDate;
                mproduct.ProductCategoryID = t.ProductCategoryID;
                mproduct.ProductStatus = t.ProductStatus;
                mproduct.Display = t.Display;
                mproduct.MetaTitle = t.MetaTitle;
                mproduct.SupplierName = GetNhaSanXuat(t.ProductID);
                mproduct.UnitName = db.ProductUnits.Where(e => e.UnitID == t.UnitID).FirstOrDefault().Name;
                mproduct.CategoryName = db.ProductCategories.Where(e => e.ProductCategoryID == t.ProductCategoryID).FirstOrDefault().Name;
                mproduct.PricewholesalePromotion = (db.ProductPricePromotions.Where(e => e.ProductID == t.ProductID && e.PromotionStatus == true).Count() != 0 ? db.ProductPricePromotions.Where(e => e.ProductID == t.ProductID && e.PromotionStatus == true).FirstOrDefault().PricewholesalePromotion : null);
                mproduct.PricePromotion = (db.ProductPricePromotions.Where(e => e.ProductID == t.ProductID && e.PromotionStatus == true).Count() != 0 ? db.ProductPricePromotions.Where(e => e.ProductID == t.ProductID && e.PromotionStatus == true).FirstOrDefault().PricePromotion : null);
                mproduct.KyTuCaterory = getnamecategorykytu(mproduct.CategoryName);
                mProducts.Add(mproduct);
                dem++;
            }
            return mProducts;
        }

        public List<MProduct> ListProductDVTCoBan()
        {
            CapNhatPricePromotion();
            var listproduct = db.Products.Where(t => t.ProductComboID.HasValue == false&&t.ParentProductID==0).OrderBy(x => x.CreateDate).Select(t => t).ToList();

            List<MProduct> mProducts = new List<MProduct>();

            foreach (var t in listproduct)
            {
                var mproduct = new MProduct();
                mproduct.ProductID = t.ProductID;
                mproduct.Name = t.Name;
                mproduct.ParentProductID = t.ParentProductID;
                mproduct.UnitName = db.ProductUnits.Where(e => e.UnitID == t.UnitID).FirstOrDefault().Name;
                mProducts.Add(mproduct);
            }
            return mProducts;
        }


        public List<MProduct> ListProductComboAdd()
        {
            CapNhatPricePromotion();
            var listproduct = db.Products.Where(t => t.ProductComboID.HasValue == false&&t.CountProduct!=0&&t.ProductStatus==true).OrderBy(x => x.CreateDate).Select(t => t).ToList();

            List<MProduct> mProducts = new List<MProduct>();

            foreach (var t in listproduct)
            {
                var mproduct = new MProduct();
                mproduct.ProductID = t.ProductID;
                mproduct.Name = t.Name;
                mproduct.ParentProductID = t.ParentProductID;
                mproduct.UnitName = db.ProductUnits.Where(e => e.UnitID == t.UnitID).FirstOrDefault().Name;
                mProducts.Add(mproduct);
            }
            return mProducts;
        }

        public string GetNhaSanXuat(int ProductID)
        {

            var lstReceiptDetail = db.ReceiptDetails.Select(t => t).ToList();

            var lstReceipt = db.Receipts.Select(t => t).ToList();

            var lstProductSupplier = db.ProductSuppliers.Select(t => t).ToList();

            var res = (from RD in lstReceiptDetail
                       from R in lstReceipt
                       from PS in lstProductSupplier
                       where RD.ReceiptID == R.ReceiptID && R.SupplierID==PS.SupplierID && RD.ProductID==ProductID
                       select PS
                      );

            if (res.Count() != 0)
            {
                return res.FirstOrDefault().Name;
            }

            var lstExchangeUnit = db.ExchangeUnits.Where(t => t.ProductIDOut == ProductID).ToList();

            if (lstExchangeUnit.Count() != 0)
            {
                foreach (var item in lstExchangeUnit)
                {
                    return GetNhaSanXuat(item.ProductIDIn.Value);
                }
            }

            return string.Empty;
                      
        }
    
        public bool CreateProduct(MProduct mproduct)
        {
            try
            {
                var product = new Product();
                product.Name = mproduct.Name;
                product.MetaTitle = GenMetaTitle(mproduct.Name);
                product.ParentProductID = 0;
                product.Image = mproduct.Image;
                if(mproduct.MoreImage !=null)
                {
                    JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();

                    var lstImageMore = scriptSerializer.Deserialize<List<string>>(mproduct.MoreImage);

                    XElement xElement = new XElement("Images");

                    foreach (var item in lstImageMore)
                    {
                        xElement.Add(new XElement("Image",item));
                    }

                    product.MoreImage = xElement.ToString();

                }
                else
                {
                    product.MoreImage = string.Empty;
                }
                product.PriceOut = mproduct.PriceOut;
                product.Pricewholesale = mproduct.Pricewholesale;
                product.CountProduct = 0;
                product.UnitID = mproduct.UnitID;
                product.CreateDate = DateTime.Now;
                product.ProductCategoryID = mproduct.ProductCategoryID;
                product.ProductStatus = true;
                product.Display = false;

                db.Products.Add(product);



                var productdetail = new ProductDetail();
                productdetail.ProductDetailID = product.ProductID;
                productdetail.Discription = mproduct.Discription;
                productdetail.TradeMark = mproduct.TradeMark;
                productdetail.TradeOrigin = mproduct.TradeOrigin;
                productdetail.Ingredient = mproduct.Ingredient;
                productdetail.Production = mproduct.Production;
                productdetail.Expiry = mproduct.Expiry;
                productdetail.UserManual = mproduct.UserManual;
                productdetail.CareInstructions = mproduct.CareInstructions;
                productdetail.Packing = mproduct.Packing;


                

                db.ProductDetails.Add(productdetail);

                db.SaveChanges();

                return true;
            }
            catch (Exception)
            {

                return false;
            }
            
        }

        public bool CreateProductDVT(MProduct mProduct)
        {
            try
            {
                var product = db.Products.Where(t=>t.ProductID==mProduct.ProductID).FirstOrDefault();
                var detail = db.ProductDetails.Where(t => t.ProductDetailID == mProduct.ProductID).FirstOrDefault();

                var productnew = new Product();
                productnew.Name = mProduct.Name;
                productnew.MetaTitle = GenMetaTitle(mProduct.Name);
                productnew.ParentProductID = product.ProductID;
                productnew.Image = product.Image;
                if (mProduct.MoreImage != null)
                {
                    JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();

                    var lstImageMore = scriptSerializer.Deserialize<List<string>>(mProduct.MoreImage);

                    XElement xElement = new XElement("Images");

                    foreach (var item in lstImageMore)
                    {
                        xElement.Add(new XElement("Image", item));
                    }

                    productnew.MoreImage = xElement.ToString();

                }else
                {
                    productnew.MoreImage = string.Empty;
                }
                productnew.PriceOut = mProduct.PriceOut;
                productnew.Pricewholesale = mProduct.Pricewholesale;
                productnew.CountProduct = 0;
                productnew.UnitID = mProduct.UnitID;
                productnew.CreateDate = DateTime.Now;
                productnew.ProductCategoryID = product.ProductCategoryID;
                productnew.ProductStatus = true;
                productnew.Display = false;
                db.Products.Add(productnew);

                if (detail != null)
                {
                    var productdetail = new ProductDetail();
                    productdetail.ProductDetailID = productnew.ProductID;
                    productdetail.Discription = detail.Discription;
                    productdetail.TradeMark = detail.TradeMark;
                    productdetail.TradeOrigin = detail.TradeOrigin;
                    productdetail.Ingredient = detail.Ingredient;
                    productdetail.Production = detail.Production;
                    productdetail.Expiry = detail.Expiry;
                    productdetail.UserManual = detail.UserManual;
                    productdetail.CareInstructions = detail.CareInstructions;
                    productdetail.Packing = detail.Packing;
                    db.ProductDetails.Add(productdetail);
                }
                db.SaveChanges();

                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool UpdateProduct(MProduct mProduct)
        {
            try
            {
                var product = db.Products.Where(t => t.ProductID == mProduct.ProductID).FirstOrDefault();
                product.Name = mProduct.Name;
                product.MetaTitle = GenMetaTitle(mProduct.Name);
                product.Image = mProduct.Image;
                
                if (mProduct.MoreImage != null)
                {
                    JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();

                    var lstImageMore = scriptSerializer.Deserialize<List<string>>(mProduct.MoreImage);

                    XElement xElement = new XElement("Images");

                    foreach (var item in lstImageMore)
                    {
                        xElement.Add(new XElement("Image", item));
                    }

                    product.MoreImage = xElement.ToString();

                }else
                {
                    product.MoreImage = string.Empty;
                }
               
                product.PriceOut = mProduct.PriceOut;
                product.Pricewholesale = mProduct.Pricewholesale;
                product.UnitID = mProduct.UnitID;
                product.ProductCategoryID = mProduct.ProductCategoryID;

                var detail = db.ProductDetails.Find(mProduct.ProductID);
                if (detail != null)
                {
                    detail.Discription = mProduct.Discription;
                    detail.TradeMark = mProduct.TradeMark;
                    detail.TradeOrigin = mProduct.TradeOrigin;
                    detail.Ingredient = mProduct.Ingredient;
                    detail.Expiry = mProduct.Expiry;
                    detail.Production = mProduct.Production;
                    detail.UserManual = mProduct.UserManual;
                    detail.CareInstructions = mProduct.CareInstructions;
                    detail.Packing = mProduct.Packing;
                }else
                {
                    var detailnew = new ProductDetail();
                    detailnew.ProductDetailID = mProduct.ProductID;
                    detailnew.Discription = mProduct.Discription;
                    detailnew.TradeMark = mProduct.TradeMark;
                    detailnew.Expiry = mProduct.Expiry;
                    detailnew.TradeOrigin = mProduct.TradeOrigin;
                    detailnew.Ingredient = mProduct.Ingredient;
                    detailnew.Production = mProduct.Production;
                    detailnew.UserManual = mProduct.UserManual;
                    detailnew.CareInstructions = mProduct.CareInstructions;
                    detailnew.Packing = mProduct.Packing;
                    db.ProductDetails.Add(detailnew);
                }
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                return false;
            }           
        }

        public bool UpdateProductDVT(MProduct mProduct)
        {
            try
            {
                var product = db.Products.Where(t => t.ProductID == mProduct.ProductID).FirstOrDefault();
                product.Name = mProduct.Name;
                product.MetaTitle = GenMetaTitle(mProduct.Name);
                product.Image = mProduct.Image;
                if (mProduct.MoreImage != null)
                {
                    JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();

                    var lstImageMore = scriptSerializer.Deserialize<List<string>>(mProduct.MoreImage);

                    XElement xElement = new XElement("Images");

                    foreach (var item in lstImageMore)
                    {
                        xElement.Add(new XElement("Image", item));
                    }

                    product.MoreImage = xElement.ToString();

                }
                else
                {
                    product.MoreImage = string.Empty;
                }
                product.PriceOut = mProduct.PriceOut;
                product.Pricewholesale = mProduct.Pricewholesale;
                product.UnitID = mProduct.UnitID;

                db.SaveChanges();

                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public DeleteProduct DeleteProduct(int id)
        {
            var checkDVT = db.Products.Where(t => t.ParentProductID == id).Count();
            var checkReceipt = db.ReceiptDetails.Where(t => t.ProductID == id).Count();
            var checkExchange = db.ExchangeUnits.Where(t => t.ProductIDOut == id).Count();
            var checkOrderDetail = db.OrderDetails.Where(t => t.ProductID == id).Count();
            if (checkDVT != 0)
            {
                var rs1 = new DeleteProduct();
                rs1.Check = false;
                rs1.Result = "Không thể xóa do sản phẩm này đang cài ĐVT phụ";
                return rs1;
            }
            if (checkReceipt != 0)
            {
                var rs1 = new DeleteProduct();
                rs1.Check = false;
                rs1.Result = "Không thể xóa do sản phẩm này sử dụng trong báo cáo Nhập - xuất - tồn";
                return rs1;
            }
            if (checkExchange != 0)
            {
                var rs1 = new DeleteProduct();
                rs1.Check = false;
                rs1.Result = "Không thể xóa do sản phẩm này sử dụng trong báo cáo Nhập - xuất - tồn";
                return rs1;
            }
            if (checkOrderDetail != 0)
            {
                var rs1 = new DeleteProduct();
                rs1.Check = false;
                rs1.Result = "Không thể xóa do sản phẩm này sử dụng trong báo cáo đơn hàng";
                return rs1;
            }
            var product = db.Products.Find(id);
            product.ProductStatus = false;
            db.SaveChanges();
            var rs = new DeleteProduct();
            rs.Check = true;
            return rs;
        }

        public int? CutMaSanPham(string input)
        {
            try
            {
                var rs = int.Parse(input.Substring(input.Length - 4, 4));

                return rs;
            }
            catch (Exception)
            {

                return null;
            }

        }

        public string GenMetaTitle(string Name)
        {

            var convert = RemoveVietnameseTone(Name.ToLower());
            string[] Phase1 = convert.Split(' ');
            string Phase2 = "";
            foreach (var item in Phase1)
            {
                Phase2 += item.ToLower()+ "-";
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

        public IEnumerable<MProduct> ListProductThemPhieuNhap()
        {
            CapNhatPricePromotion();
            var listproduct = db.Products.Where(t => t.ProductComboID.HasValue == false&&t.ProductStatus==true).OrderByDescending(x => x.CreateDate).Select(t => t).ToList();

            List<MProduct> mProducts = new List<MProduct>();

            foreach (var t in listproduct)
            {
                var mproduct = new MProduct();
                mproduct.STT = listproduct.IndexOf(t) + 1;
                mproduct.ProductID = t.ProductID;
                mproduct.Name = t.Name;
                mproduct.CategoryName = db.ProductCategories.Where(e => e.ProductCategoryID == t.ProductCategoryID).FirstOrDefault().Name;
                mproduct.KyTuCaterory = getnamecategorykytu(mproduct.CategoryName);
                mProducts.Add(mproduct);
            }
            return mProducts;
        }
       
        public string GetNameProduct(int id)
        {
            return db.Products.Where(t => t.ProductID == id).SingleOrDefault().Name;
        }

        public bool ActionProduct(int productid)
        {
            try
            {
                var product = db.Products.Find(productid);
                product.Display = !product.Display;

                db.SaveChanges();

                return product.Display.Value;
            }
            catch (Exception)
            {

                return false;
            }
        }


        
    }
}
