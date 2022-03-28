namespace PROJECT_WEBSITE.Data.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ReceiptDetail")]
    public partial class ReceiptDetail
    {
        [Key]
        public int ReceitpDetailID { get; set; }

        public int ReceiptID { get; set; }

        public int ProductID { get; set; }

        public decimal? PriceIput { get; set; }

        public int? ReceiptCount { get; set; }

        public virtual Product Product { get; set; }

        public virtual Receipt Receipt { get; set; }
    }
}
