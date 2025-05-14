using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bai1.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :base(options)
        {
        }

        public DbSet<Food> Foods { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<FoodImage> FoodImages { get; set; }
        public DbSet<Order> Orders { get; set; }
   
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<PartnerRequest> PartnerRequests { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<DiscountCode> DiscountCodes { get; set; }
        public DbSet<FoodReview> FoodReviews { get; set; }
        public DbSet<Topping> Toppings { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                // Ánh xạ tên bảng và các cột
                entity.ToTable("categories");

                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name");
            });
            base.OnModelCreating(modelBuilder);

            // Thiết lập mối quan hệ 1-nhiều giữa Food và Topping
            modelBuilder.Entity<Topping>()
                .HasOne(t => t.Food)
                .WithMany(f => f.Toppings)
                .HasForeignKey(t => t.FoodId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<FoodImage>()
                .ToTable("foodimages");

            modelBuilder.Entity<FoodImage>()
                .Property(f => f.Url)
                .HasColumnName("Url")
                .HasColumnType("character varying");
            modelBuilder.Entity<Order>()
                .Property(o => o.Id)
                .ValueGeneratedOnAdd(); // Đảm bảo Id tự động tăng và không nhận NULL
            modelBuilder.Entity<PartnerRequest>(entity =>
            {
                entity.ToTable("PartnerRequests");

                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.UserId).HasColumnName("UserId");
                entity.Property(e => e.Email).HasColumnName("Email");
                entity.Property(e => e.BusinessName).HasColumnName("BusinessName");
                entity.Property(e => e.BusinessType).HasColumnName("BusinessType");
                entity.Property(e => e.StreetName).HasColumnName("StreetName");
                entity.Property(e => e.City).HasColumnName("City");
                entity.Property(e => e.District).HasColumnName("District");
                entity.Property(e => e.StreetAddress).HasColumnName("StreetAddress");
                entity.Property(e => e.BusinessPhone).HasColumnName("BusinessPhone");
                entity.Property(e => e.Description).HasColumnName("Description");
                entity.Property(e => e.RequestDate).HasColumnName("RequestDate");
                entity.Property(e => e.IsApproved).HasColumnName("IsApproved");
                entity.Property(e => e.ApprovalDate).HasColumnName("ApprovalDate");
                entity.Property(e => e.ImagePath).HasColumnName("ImagePath");
            });
        }
        public DbSet<UserCartItem> UserCartItems { get; set; }


    }
}
