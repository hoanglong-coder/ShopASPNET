namespace PROJECT_WEBSITE.Data.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FooterCategory")]
    public partial class FooterCategory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FooterCategory()
        {
            Footers = new HashSet<Footer>();
        }

        public int FooterCategoryID { get; set; }

        [StringLength(250)]
        public string NameCategory { get; set; }

        public int? Display { get; set; }

        [Column(TypeName = "date")]
        public DateTime? CreateDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Footer> Footers { get; set; }
    }
}
