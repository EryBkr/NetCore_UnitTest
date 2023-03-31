using Microsoft.EntityFrameworkCore;
using UnitTest.WEB.Models;

#nullable disable

namespace UnitTest.WEB.Models
{
    public partial class UnitTestDbContext : DbContext
    {
        public UnitTestDbContext()
        {
        }

        public UnitTestDbContext(DbContextOptions<UnitTestDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Category> Categories { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            });

            //Seed Category Data
            modelBuilder.Entity<Category>().HasData(new Category { Id = 1, Name = "Kırtasiye" }, new Category { Id = 2, Name = "Beyaz Eşya" });
            modelBuilder.Entity<Product>().HasData(new Product { Id = 1, Name = "Çamaşır Makinesi", Price = 1500, Stock = 150, CategoryId = 2 }, new Product { Id = 2, Name = "Kalem Makinesi", Price = 50, Stock = 1500, CategoryId = 1 });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
