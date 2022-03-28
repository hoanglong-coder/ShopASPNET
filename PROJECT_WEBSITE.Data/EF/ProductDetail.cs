namespace PROJECT_WEBSITE.Data.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ProductDetail
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProductDetailID { get; set; }

        public string Discription { get; set; }

        [StringLength(50)]
        public string TradeMark { get; set; }

        [StringLength(50)]
        public string TradeOrigin { get; set; }

        public string Ingredient { get; set; }

        [StringLength(50)]
        public string Production { get; set; }

        [StringLength(50)]
        public string Expiry { get; set; }

        public string UserManual { get; set; }

        public string CareInstructions { get; set; }

        [StringLength(50)]
        public string Packing { get; set; }

        public virtual Product Product { get; set; }
    }
}
