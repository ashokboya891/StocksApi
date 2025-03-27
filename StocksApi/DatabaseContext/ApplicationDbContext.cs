using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StocksApi.Models;

namespace StocksApi.DatabaseContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<BuyOrder> BuyOrders { get; set; }
        public DbSet<SellOrder> SellOrders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<BuyOrder>().ToTable("BuyOrders");
            modelBuilder.Entity<SellOrder>().ToTable("SellOrders");

            modelBuilder.Entity<BuyOrder>()
       .ToTable("BuyOrders")
       .HasOne<ApplicationUser>()
       .WithMany()
       .HasForeignKey(b => b.UserID)
       .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SellOrder>()
                .ToTable("SellOrders")
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(s => s.UserID)
                .OnDelete(DeleteBehavior.Cascade);
            //modelBuilder.Entity<BuyOrder>().HasData(
            //    new BuyOrder
            //    {
            //        BuyOrderID = Guid.Parse("E78DAC05-EEF0-4C9B-956C-6597415820CE"),
            //        StockName = "MicroSoft",
            //        StockSymbol = "MSFT",
            //        DateAndTimeOfOrder = DateTime.Now,
            //        Quantity = 100,
            //        Price = 20,
            //        UserID = Guid.Parse("E78DAC05-EEF0-4C9B-956C-6597415820CE")
            //    },
            //    new BuyOrder
            //    {
            //        BuyOrderID = Guid.Parse("66822E66-909A-42B1-9E3D-370436ED104D"),
            //        StockName = "Google",
            //        StockSymbol = "GOOGL",
            //        DateAndTimeOfOrder = DateTime.Now,
            //        Quantity = 120,
            //        Price = 22,
            //        UserID = Guid.Parse("E78DAC05-EEF0-4C9B-956C-6597415820CE")


            //    },

            //    new BuyOrder
            //    {
            //        BuyOrderID = Guid.Parse("8CB73AD6-6665-423F-B706-160CC323D270"),
            //        StockName = "Nvedia",
            //        StockSymbol = "NVDA",
            //        DateAndTimeOfOrder = DateTime.Now,
            //        Quantity = 40,
            //        Price = 200,
            //        UserID = Guid.Parse("E78DAC05-EEF0-4C9B-956C-6597415820CE")

            //    }
            //);
            //modelBuilder.Entity<SellOrder>().HasData(
            //    new SellOrder
            //    {
            //        SellOrderID =Guid.Parse("AFC49B17-A536-4CCA-B5D7-65E43C27A76F"),
            //        StockName = "MicroSoft",
            //        StockSymbol = "MSFT",
            //        DateAndTimeOfOrder = DateTime.Now,
            //        Quantity = 100,
            //        Price = 20,
            //        UserID = Guid.Parse("E78DAC05-EEF0-4C9B-956C-6597415820CE")

            //    },
            //    new SellOrder
            //    {
            //        SellOrderID =Guid.Parse("DD62A22C-02C7-481D-85AF-98153D688848"),
            //        StockName = "Google",
            //        StockSymbol = "GOOGL",
            //        DateAndTimeOfOrder = DateTime.Now,
            //        Quantity = 120,
            //        Price = 22,
            //        UserID = Guid.Parse("E78DAC05-EEF0-4C9B-956C-6597415820CE")

            //    }
            //);
        }
    }
}
