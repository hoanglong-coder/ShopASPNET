namespace PROJECT_WEBSITE.Data.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ExchangeUnit")]
    public partial class ExchangeUnit
    {
        public int ExchangeUnitID { get; set; }

        public DateTime? Createdate { get; set; }

        public int? ValueCountUnit { get; set; }

        public int? ProductIDIn { get; set; }

        public int? ProductIDOut { get; set; }

        public int? UserID { get; set; }

        public bool? ExchangeStatus { get; set; }

        public virtual Product Product { get; set; }

        public virtual User User { get; set; }
    }
}
