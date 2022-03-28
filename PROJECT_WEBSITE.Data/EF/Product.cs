namespace PROJECT_WEBSITE.Data.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Product")]
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            ExchangeUnits = new HashSet<ExchangeUnit>();
            OrderDetails = new HashSet<OrderDetail>();
            ProductComboDetails = new HashSet<ProductComboDetail>();
            ProductPricePromotions = new HashSet<ProductPricePromotion>();
            ReceiptDetails = new HashSet<ReceiptDetail>();
        }

        public int ProductID { get; set; }

        [StringLength(250)]
        public string Name { get; set; }

        public string MetaTitle { get; set; }

        public int? ParentProductID { get; set; }

        [StringLength(500)]
        public string Image { get; set; }

        [Column(TypeName = "xml")]
        public string MoreImage { get; set; }

        public decimal? PriceOut { get; set; }

        public decimal? Pricewholesale { get; set; }

        public int? CountProduct { get; set; }

        public int? UnitID { get; set; }

        public DateTime? CreateDate { get; set; }

        public int? ProductCategoryID { get; set; }

        public bool? ProductStatus { get; set; }

        public int? ProductComboID { get; set; }

        public bool? Display { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExchangeUnit> ExchangeUnits { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        public virtual ProductCategory ProductCategory { get; set; }

        public virtual ProductUnit ProductUnit { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductComboDetail> ProductComboDetails { get; set; }

        public virtual ProductDetail ProductDetail { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductPricePromotion> ProductPricePromotions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReceiptDetail> ReceiptDetails { get; set; }
    }
}
