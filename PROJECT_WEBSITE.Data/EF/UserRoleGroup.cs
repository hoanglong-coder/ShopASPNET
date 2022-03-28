namespace PROJECT_WEBSITE.Data.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserRoleGroup")]
    public partial class UserRoleGroup
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UserRoleGroup()
        {
            Users = new HashSet<User>();
            UserRoleGroupDetails = new HashSet<UserRoleGroupDetail>();
        }

        public int UserRoleGroupID { get; set; }

        [StringLength(250)]
        public string Name { get; set; }

        public DateTime? CreateDate { get; set; }

        public bool? GroupStatus { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<User> Users { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserRoleGroupDetail> UserRoleGroupDetails { get; set; }
    }
}
