namespace PROJECT_WEBSITE.Data.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProductCategory")]
    public partial class ProductCategory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProductCategory()
        {
            Products = new HashSet<Product>();
        }

        public int ProductCategoryID { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(500)]
        public string MetaTitle { get; set; }

        public int? Display { get; set; }

        public int? ParentID { get; set; }

        public DateTime? CreateDate { get; set; }

        [StringLength(500)]
        public string Image { get; set; }

        public bool? ShowOnHome { get; set; }

        public bool? ProductCategoryStatus { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> Products { get; set; }
    }
}
