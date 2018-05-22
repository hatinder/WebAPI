using System.Data.Entity;
using WebAPI.data1.Entities;

namespace WebAPI.data1
{
    public class StockContext: DbContext
    {
        public StockContext() : base("DefaultConnection")
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        static StockContext()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<StockContext, StockMigrationConfiguration>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            StockMapping.Configure(modelBuilder);
        }
        public DbSet<Security> SecuritySet { get; set; }
        public DbSet<Prices> PricesSet { get; set; }
    }
}