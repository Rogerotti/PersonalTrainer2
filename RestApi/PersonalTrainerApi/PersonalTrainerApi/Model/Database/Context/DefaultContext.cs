using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PersonalTrainerApi.Model.Database.Entity;
using System.Linq;

namespace PersonalTrainerApi.Model.Database.Context
{
    /// <summary>
    /// Podstawoy kontekst bazodanowy.
    /// </summary>
    public class DefaultContext : DbContext
    {
        public DbSet<Product> Product { get; set; }

        public DbSet<ProductDetails> ProductsDetails { get; set; }

        public DbSet<User> User { get; set; }

        public DbSet<UserDetails> UsersDetails { get; set; }

        public DbSet<UserGoal> UserGoal { get; set; }

        public DbSet<DayFoodDiary> DailyFood { get; set; }

        public DbSet<DiaryProduct> DiaryProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DiaryProduct>().HasKey(x => x.DiaryProductId);
            modelBuilder.Entity<DiaryProduct>()
                .HasOne(p => p.Day)
                .WithMany(dp => dp.DiaryProducts)
                .HasForeignKey(fk => fk.DayId);

            modelBuilder.Entity<DiaryProduct>()
                .HasOne(p => p.Product)
                .WithMany(t => t.DiaryProducts)
                .HasForeignKey(fk => fk.ProductId);

            foreach (IMutableForeignKey mutableForeignKey in modelBuilder.Model.GetEntityTypes().SelectMany(x => x.GetForeignKeys()))
            {
                mutableForeignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        public DefaultContext(DbContextOptions<DefaultContext> options)
            : base(options)
        {
        }
    }
}
