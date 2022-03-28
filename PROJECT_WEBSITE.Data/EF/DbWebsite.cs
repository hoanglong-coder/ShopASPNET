using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace PROJECT_WEBSITE.Data.EF
{
    public partial class DbWebsite : DbContext
    {
        public DbWebsite()
            : base("name=DbWebsite")
        {
        }

        public virtual DbSet<CategoryNew> CategoryNews { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<DetailDiscountCode> DetailDiscountCodes { get; set; }
        public virtual DbSet<DiscountCode> DiscountCodes { get; set; }
        public virtual DbSet<ExchangeUnit> ExchangeUnits { get; set; }
        public virtual DbSet<Footer> Footers { get; set; }
        public virtual DbSet<FooterCategory> FooterCategories { get; set; }
        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
        public virtual DbSet<ProductCombo> ProductComboes { get; set; }
        public virtual DbSet<ProductComboDetail> ProductComboDetails { get; set; }
        public virtual DbSet<ProductDetail> ProductDetails { get; set; }
        public virtual DbSet<ProductPricePromotion> ProductPricePromotions { get; set; }
        public virtual DbSet<ProductSupplier> ProductSuppliers { get; set; }
        public virtual DbSet<ProductUnit> ProductUnits { get; set; }
        public virtual DbSet<Receipt> Receipts { get; set; }
        public virtual DbSet<ReceiptDetail> ReceiptDetails { get; set; }
        public virtual DbSet<Slide> Slides { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<UserRoleGroup> UserRoleGroups { get; set; }
        public virtual DbSet<UserRoleGroupDetail> UserRoleGroupDetails { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .Property(e => e.Phone)
                .IsUnicode(false);

            modelBuilder.Entity<Customer>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<Customer>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<DiscountCode>()
                .Property(e => e.TotalCart)
                .HasPrecision(18, 0);

            modelBuilder.Entity<DiscountCode>()
                .HasMany(e => e.DetailDiscountCodes)
                .WithRequired(e => e.DiscountCode)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<News>()
                .Property(e => e.Image)
                .IsUnicode(false);

            modelBuilder.Entity<Order>()
                .Property(e => e.ShipPhone)
                .IsUnicode(false);

            modelBuilder.Entity<Order>()
                .Property(e => e.PriceShip)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Order>()
                .Property(e => e.TotalPrice)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Order>()
                .Property(e => e.PriceDiscount)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Order>()
                .HasMany(e => e.DetailDiscountCodes)
                .WithRequired(e => e.Order)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Order>()
                .HasMany(e => e.OrderDetails)
                .WithRequired(e => e.Order)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<OrderDetail>()
                .Property(e => e.OrderPrice)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Product>()
                .Property(e => e.MetaTitle)
                .IsUnicode(false);

            modelBuilder.Entity<Product>()
                .Property(e => e.Image)
                .IsUnicode(false);

            modelBuilder.Entity<Product>()
                .Property(e => e.PriceOut)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Product>()
                .Property(e => e.Pricewholesale)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.ExchangeUnits)
                .WithOptional(e => e.Product)
                .HasForeignKey(e => e.ProductIDIn);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.OrderDetails)
                .WithRequired(e => e.Product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.ProductComboDetails)
                .WithRequired(e => e.Product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasOptional(e => e.ProductDetail)
                .WithRequired(e => e.Product);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.ReceiptDetails)
                .WithRequired(e => e.Product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProductCategory>()
                .Property(e => e.MetaTitle)
                .IsUnicode(false);

            modelBuilder.Entity<ProductCategory>()
                .Property(e => e.Image)
                .IsUnicode(false);

            modelBuilder.Entity<ProductCombo>()
                .HasMany(e => e.ProductComboDetails)
                .WithRequired(e => e.ProductCombo)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProductPricePromotion>()
                .Property(e => e.PricePromotion)
                .HasPrecision(18, 0);

            modelBuilder.Entity<ProductPricePromotion>()
                .Property(e => e.PricewholesalePromotion)
                .HasPrecision(18, 0);

            modelBuilder.Entity<ProductSupplier>()
                .Property(e => e.Phone)
                .IsUnicode(false);

            modelBuilder.Entity<ProductSupplier>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<Receipt>()
                .Property(e => e.TotalReceiptPrice)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Receipt>()
                .HasMany(e => e.ReceiptDetails)
                .WithRequired(e => e.Receipt)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ReceiptDetail>()
                .Property(e => e.PriceIput)
                .HasPrecision(18, 0);

            modelBuilder.Entity<User>()
                .Property(e => e.UserName)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Passsword)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Phone)
                .IsUnicode(false);

            modelBuilder.Entity<UserRole>()
                .HasMany(e => e.UserRoleGroupDetails)
                .WithRequired(e => e.UserRole)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserRoleGroup>()
                .HasMany(e => e.UserRoleGroupDetails)
                .WithRequired(e => e.UserRoleGroup)
                .WillCascadeOnDelete(false);
        }
    }
}
