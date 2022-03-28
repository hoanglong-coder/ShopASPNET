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
    public class ProductComBoDAO
    {
        DbWebsite db;

        public ProductComBoDAO()
        {
            db = new DbWebsite();
        }

        public IEnumerable<MProductCombo> GetAll(SearchProductCombo search, int page, int pageSize)
       {
            CapNhatTrangThaiComBo();

            var listproduct = db.Products.Where(t => t.ProductComboID.HasValue == true && t.ProductStatus == true).OrderByDescending(t => t.CreateDate).AsQueryable();

            if (!string.IsNullOrEmpty(search.querysearch))
            {
                var masanpham = CutMaSanPham(search.querysearch);

                if (masanpham.HasValue)
                {
                    listproduct = listproduct.Where(t => t.ProductID == masanpham.Value);
                }
                else
                {
                    listproduct = listproduct.Where(t => t.Name.Contains(search.querysearch));
                }
            }
            if (search.TinhTrangCombo.HasValue)
            {
                listproduct = listproduct.Where(t => t.Display.Value == search.TinhTrangCombo.Value);
            }
            if (search.TrangThaiCombo.HasValue)
            {
                var lst = new List<Product>();
                foreach (var item in listproduct)
                {
                    var check = db.ProductComboes.Find(item.ProductComboID);

                    if (check.ComboStatus == search.TrangThaiCombo.Value)
                    {
                        lst = lst.Concat(listproduct.Where(t => t.ProductID == item.ProductID)).ToList();
                    }
                }
                var demo = lst.ToList();
                listproduct = lst.AsQueryable();
            }
            if (search.SoLuong.HasValue)
            {
                if (search.SoLuong.Value == true)
                {
                    listproduct = listproduct.Where(t => t.CountProduct.Value != 0);
                }
                else
                {
                    listproduct = listproduct.Where(t => t.CountProduct.Value == 0);
                }
            }
            if (search.GiaLeTu.HasValue && search.GiaLeDen.HasValue)
            {
                listproduct = listproduct.Where(t => t.PriceOut.Value >= search.GiaLeTu.Value && t.PriceOut.Value <= search.GiaLeDen.Value);
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
                listproduct = listproduct.Where(t => t.Pricewholesale.Value >= search.GiaSiTu.Value);
            }
            if (search.GiaSiDen.HasValue)
            {
                listproduct = listproduct.Where(t => t.Pricewholesale.Value <= search.GiaSiDen.Value);
            }

            var lstrs = new List<MProductCombo>();

            foreach (var item in listproduct)
            {
                MProductCombo m = new MProductCombo();
                m.STT = listproduct.ToList().IndexOf(item) + 1;
                m.ProductID = item.ProductID;
                m.ProductComboID = item.ProductComboID;
                m.Name = item.Name;
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

            return lstrs.ToPagedList(page, pageSize);
        }

        public IEnumerable<MProduct> GetDetailComBo(int id,int page, int pageSize, SearchProduct search)
        {
            CapNhatPricePromotion();

            var listproduct1 = db.ProductComboDetails.Where(t => t.ProductComboID == id);

            var lstProduct = new List<Product>();

            if (listproduct1.Count() != 0)
            {
                foreach (var item in listproduct1)
                {
                    var product = db.Products.Find(item.ProductID);
                    lstProduct.Add(product);
                }
            }
            
            var listproduct = lstProduct.OrderByDescending(x => x.CreateDate).AsQueryable();


            if (!string.IsNullOrEmpty(search.querysearch))
            {
                var masanpham = CutMaSanPham(search.querysearch);

                if (masanpham.HasValue)
                {
                    listproduct = listproduct.Where(t => t.ProductID == masanpham.Value);
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
                    listproduct = listproduct.Where(t => t.CountProduct.Value != 0);
                }
                else
                {
                    listproduct = listproduct.Where(t => t.CountProduct.Value == 0);
                }
            }
            if (search.UnitID.HasValue)
            {
                listproduct = listproduct.Where(t => t.UnitID.Value == search.UnitID.Value);
            }
            if (search.GiaLeTu.HasValue && search.GiaLeDen.HasValue)
            {
                listproduct = listproduct.Where(t => t.PriceOut.Value >= search.GiaLeTu.Value && t.PriceOut.Value <= search.GiaLeDen.Value);
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
                listproduct = listproduct.Where(t => t.Pricewholesale.Value >= search.GiaSiTu.Value);
            }
            if (search.GiaSiDen.HasValue)
            {
                listproduct = listproduct.Where(t => t.Pricewholesale.Value <= search.GiaSiDen.Value);
            }

            List<MProduct> mProducts = new List<MProduct>();

            foreach (var t in listproduct)
            {
                var mproduct = new MProduct();
                mproduct.STT = listproduct.ToList().IndexOf(t) + 1;
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

        public void CapNhatPricePromotion()
        {
            DateTime hientai = DateTime.Now.Date;

            var products = db.Products.Where(t => t.ProductStatus == true).Select(t => t).ToList();

            foreach (var item in products)
            {
                var promotionfalses = db.ProductPricePromotions.Where(t => t.ProductID == item.ProductID).Select(t => t).ToList();
                if (promotionfalses.Count() != 0)
                {
                    foreach (var item1 in promotionfalses)
                    {
                        if (DateTime.Compare(item1.StartDate.Value.Date, hientai) <= 0 && DateTime.Compare(hientai, item1.EndDate.Value.Date) <= 0)
                        {
                            var promotion = db.ProductPricePromotions.Find(item1.PricePromotionID);
                            promotion.PromotionStatus = true;
                        }
                        else
                        {
                            var promotion = db.ProductPricePromotions.Find(item1.PricePromotionID);
                            promotion.PromotionStatus = false;
                        }
                    }
                }
            }
            db.SaveChanges();
        }

        public void CapNhatTrangThaiComBo()
        {
            DateTime hientai = DateTime.Now.Date;

            var productscombo = db.ProductComboes.Select(t => t).ToList();

            foreach (var item in productscombo)
            {
                if (DateTime.Compare(item.StartDate.Value.Date, hientai) <= 0 && DateTime.Compare(hientai, item.EndDate.Value.Date) <= 0)
                {
                    var combo = db.ProductComboes.Find(item.ProductComboID);
                    combo.ComboStatus = true;
                }else
                {
                    var combo = db.ProductComboes.Find(item.ProductComboID);
                    combo.ComboStatus = false;
                }
            }
            db.SaveChanges();
        }

        public string GetNhaSanXuat(int ProductID)
        {

            var lstReceiptDetail = db.ReceiptDetails.Select(t => t).ToList();

            var lstReceipt = db.Receipts.Select(t => t).ToList();

            var lstProductSupplier = db.ProductSuppliers.Select(t => t).ToList();

            var res = (from RD in lstReceiptDetail
                       from R in lstReceipt
                       from PS in lstProductSupplier
                       where RD.ReceiptID == R.ReceiptID && R.SupplierID == PS.SupplierID && RD.ProductID == ProductID
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


        public bool CreateProductCombo(MProductCombo mproduct)
        {
            try
            {
                var productcombo = new ProductCombo();
                productcombo.Name = mproduct.Name;
                productcombo.Display = mproduct.DisplayProductComBo;
                productcombo.StartDate = mproduct.StartDate;
                productcombo.EndDate = mproduct.EndDate;
                productcombo.ComboStatus = false;

                db.ProductComboes.Add(productcombo);

                db.SaveChanges();

                var idcombo = db.ProductComboes.OrderByDescending(t => t.ProductComboID).Take(1).SingleOrDefault().ProductComboID;


                var product = new Product();
                product.Name = mproduct.Name;
                product.MetaTitle = GenMetaTitle(mproduct.Name);
                product.ParentProductID = 0;
                product.Image = mproduct.Image;
                if (mproduct.MoreImage != null)
                {
                    JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();

                    var lstImageMore = scriptSerializer.Deserialize<List<string>>(mproduct.MoreImage);

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
                product.PriceOut = mproduct.PriceOut;
                product.Pricewholesale = mproduct.Pricewholesale;
                product.CountProduct = mproduct.CountProduct;
                product.CreateDate = DateTime.Now;
                product.ProductStatus = true;
                product.Display = false;
                product.ProductComboID = idcombo;
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


        public bool AddProductCombo(int idcombo, int idproduct)
        {
            var lstcombo = db.ProductComboDetails.Where(t => t.ProductComboID == idcombo);

            foreach (var item in lstcombo)
            {
                if (item.ProductID == idproduct)
                {
                    return false;
                }
            }

            var detail = new ProductComboDetail();
            detail.ProductComboID = idcombo;
            detail.ProductID = idproduct;

            db.ProductComboDetails.Add(detail);

            db.SaveChanges();

            return true;
        }
        public bool AddProductComboList(int idcombo, List<int> idproduct)
        {
            var lstcombo = db.ProductComboDetails.Where(t => t.ProductComboID == idcombo);

            if (idproduct != null)
            {
                foreach (var item in idproduct)
                {
                    var check = lstcombo.Where(t => t.ProductID == item);

                    var checkcountproduct = db.Products.Find(item).CountProduct;

                    if (check.Count() != 0)
                    {
                        return false;
                    }
                    if (checkcountproduct.Value == 0)
                    {
                        return false;
                    }
                }

                foreach (var item in idproduct)
                {
                    var detail = new ProductComboDetail();
                    detail.ProductComboID = idcombo;
                    detail.ProductID = item;
                    detail.ProductComboCount = 1;

                    db.ProductComboDetails.Add(detail);

                    db.SaveChanges();
                }
                return true;
            }
            return false;
        }
    }
}
