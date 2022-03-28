namespace PROJECT_WEBSITE.Data.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class News
    {
        public int NewsID { get; set; }

        [StringLength(250)]
        public string Name { get; set; }

        public string Detail { get; set; }

        [StringLength(500)]
        public string Image { get; set; }

        public DateTime? CreateDate { get; set; }

        public int? UserID { get; set; }

        public int? CategoryNewID { get; set; }

        public bool? NewsStatus { get; set; }

        public virtual CategoryNew CategoryNew { get; set; }

        public virtual User User { get; set; }
    }
}
