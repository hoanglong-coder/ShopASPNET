namespace PROJECT_WEBSITE.Data.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProductPricePromotion")]
    public partial class ProductPricePromotion
    {
        [Key]
        public int PricePromotionID { get; set; }

        public int? ProductID { get; set; }

        public DateTime? CreateDate { get; set; }

        public decimal? PricePromotion { get; set; }

        public decimal? PricewholesalePromotion { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool? PromotionStatus { get; set; }

        public virtual Product Product { get; set; }
    }
}
