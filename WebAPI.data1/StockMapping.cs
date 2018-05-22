using System.Data.Entity;
using WebAPI.data1.Entities;

namespace WebAPI.data1
{
    public class StockMapping
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            MapPrices(modelBuilder);
            MapSecurities(modelBuilder);
        }

        static void MapPrices(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Prices>().ToTable("Prices", "WebAPI");
        }
        static void MapSecurities(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Security>().ToTable("Security", "WebAPI");
        }
    }
}