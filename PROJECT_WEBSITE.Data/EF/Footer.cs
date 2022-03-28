namespace PROJECT_WEBSITE.Data.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Footer")]
    public partial class Footer
    {
        public int FooterID { get; set; }

        [StringLength(250)]
        public string Name { get; set; }

        [Column(TypeName = "date")]
        public DateTime? CreateDate { get; set; }

        public int? FooterCategoryID { get; set; }

        public string Detail { get; set; }

        public virtual FooterCategory FooterCategory { get; set; }
    }
}
