namespace PROJECT_WEBSITE.Data.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Order")]
    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            DetailDiscountCodes = new HashSet<DetailDiscountCode>();
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int OrderID { get; set; }

        public int? UserID { get; set; }

        public int? CustomerID { get; set; }

        [StringLength(250)]
        public string ShipName { get; set; }

        [StringLength(250)]
        public string ShipAddress { get; set; }

        [StringLength(50)]
        public string ShipPhone { get; set; }

        public DateTime? CreateDate { get; set; }

        public string Discription { get; set; }

        public bool? PaymentStatus { get; set; }

        public int? OrderStatus { get; set; }

        public decimal? PriceShip { get; set; }

        public decimal? TotalPrice { get; set; }

        public decimal? PriceDiscount { get; set; }

        [StringLength(250)]
        public string ShipEmail { get; set; }

        public virtual Customer Customer { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DetailDiscountCode> DetailDiscountCodes { get; set; }

        public virtual User User { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
