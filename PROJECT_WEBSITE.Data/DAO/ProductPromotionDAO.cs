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
    public class ProductPromotionDAO
    {
        DbWebsite db;
        public ProductPromotionDAO()
        {
            db = new DbWebsite();
        }

        public IEnumerable<MProductPricePromotion> ListPromotionByProduct(int ProductID, int page, int pageSize)
        {
            var lst = db.ProductPricePromotions.Where(t => t.ProductID == ProductID).Count();
            var lstrs = new List<MProductPricePromotion>();
            if (lst != 0)
            {
                var rs = db.ProductPricePromotions.Where(t => t.ProductID == ProductID).OrderBy(t => t.CreateDate).ToList();


                foreach (var t in rs)
                {
                    MProductPricePromotion m = new MProductPricePromotion();
                    m.STT = rs.IndexOf(t) + 1;
                    m.PricePromotionID = t.PricePromotionID;
                    m.ProductID = t.ProductID.Value;
                    m.CreateDate = t.CreateDate;
                    m.PricePromotion = t.PricePromotion;
                    m.PricewholesalePromotion = t.PricewholesalePromotion;
                    m.StartDate = t.StartDate;
                    m.EndDate = t.EndDate;
                    m.PromotionStatus = t.PromotionStatus;
                    lstrs.Add(m);


                }
                    
                return lstrs.ToPagedList(page, pageSize);
            }
            return new List<MProductPricePromotion>().ToPagedList(page, pageSize);
        }
    
        public MProductPricePromotion DetailPromotion(int id)
        {
            var productpromotion = db.ProductPricePromotions.Find(id);

            var mproductpromotion = new MProductPricePromotion();

            mproductpromotion.PricePromotionID = productpromotion.PricePromotionID;
            mproductpromotion.ProductID = productpromotion.ProductID.Value;
            mproductpromotion.CreateDate = productpromotion.CreateDate;
            mproductpromotion.PricePromotion = productpromotion.PricePromotion;
            mproductpromotion.PricewholesalePromotion = productpromotion.PricewholesalePromotion;
            mproductpromotion.StartDate = productpromotion.StartDate;
            mproductpromotion.EndDate = productpromotion.EndDate;

            return mproductpromotion;
        }

        public int? CreatePricePromotion(MProductPricePromotion productPricePromotion)
        {

            try
            {
                if (!productPricePromotion.PricePromotion.HasValue&& !productPricePromotion.PricewholesalePromotion.HasValue)
                {
                    return null;
                }

                if (productPricePromotion.PricePromotionID == 0)
                {
                    var pricepromotionlastcount = db.ProductPricePromotions.Where(t => t.ProductID == productPricePromotion.ProductID).Count();
                    if (pricepromotionlastcount != 0)
                    {
                        var pricepromotionlast = db.ProductPricePromotions.Where(t => t.ProductID == productPricePromotion.ProductID).OrderByDescending(t => t.EndDate).FirstOrDefault().EndDate.HasValue;

                        if (pricepromotionlast == true)
                        {
                            var tt = db.ProductPricePromotions.Where(t => t.ProductID == productPricePromotion.ProductID).OrderByDescending(t => t.EndDate).FirstOrDefault().EndDate.Value;
                            if (db.ProductPricePromotions.Where(t => t.ProductID == productPricePromotion.ProductID).OrderByDescending(t => t.EndDate).FirstOrDefault().EndDate.Value < productPricePromotion.StartDate)
                            {
                                var promotion = new ProductPricePromotion();

                                promotion.ProductID = productPricePromotion.ProductID;
                                promotion.CreateDate = DateTime.Now;
                                promotion.PricePromotion = productPricePromotion.PricePromotion;
                                promotion.PricewholesalePromotion = productPricePromotion.PricewholesalePromotion;
                                promotion.StartDate = productPricePromotion.StartDate;
                                promotion.EndDate = productPricePromotion.EndDate;
                                promotion.PromotionStatus = false;

                                db.ProductPricePromotions.Add(promotion);

                                db.SaveChanges();

                                return promotion.ProductID.Value;
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }else
                    {
                        var promotion = new ProductPricePromotion();

                        promotion.ProductID = productPricePromotion.ProductID;
                        promotion.CreateDate = DateTime.Now;
                        promotion.PricePromotion = productPricePromotion.PricePromotion;
                        promotion.PricewholesalePromotion = productPricePromotion.PricewholesalePromotion;
                        promotion.StartDate = productPricePromotion.StartDate;
                        promotion.EndDate = productPricePromotion.EndDate;
                        promotion.PromotionStatus = false;

                        db.ProductPricePromotions.Add(promotion);

                        db.SaveChanges();

                        return promotion.ProductID;
                    }

                    
                }else
                {
                    var promotion = db.ProductPricePromotions.Find(productPricePromotion.PricePromotionID);
                    promotion.PricePromotion = productPricePromotion.PricePromotion;
                    promotion.PricewholesalePromotion = productPricePromotion.PricewholesalePromotion;
                    db.SaveChanges();
                    return promotion.ProductID;
                }
                
                
            }
            catch (Exception)
            {

                return null;
            }                   
        }
    }
}
