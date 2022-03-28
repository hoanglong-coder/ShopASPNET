namespace PROJECT_WEBSITE.Data.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserRole")]
    public partial class UserRole
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UserRole()
        {
            UserRoleGroupDetails = new HashSet<UserRoleGroupDetail>();
        }

        public int UserRoleID { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public DateTime? CreateDate { get; set; }

        public bool? UserRoleStatus { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserRoleGroupDetail> UserRoleGroupDetails { get; set; }
    }
}
