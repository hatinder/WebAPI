using System.Data.Entity.Migrations;

namespace WebAPI.data1
{
    public class StockMigrationConfiguration : DbMigrationsConfiguration<StockContext>
    {
        public StockMigrationConfiguration()
        {
            this.AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(StockContext context)
        {
            new StockSeeder(context).Seed();
        }
    }
}