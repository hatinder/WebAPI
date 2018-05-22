using System;
using System.Linq;
using WebAPI.data1.Entities;

namespace WebAPI.data1
{
    public class StockRepository : IStockRepository
    {
        private StockContext _ctx;

        public StockRepository(StockContext ctx)
        {
            _ctx = ctx;
        }

        public IQueryable<Security> GetAllSecurities()
        {
            return _ctx.SecuritySet;
        }
        public IQueryable<Security> GetAllSecuritiesWithPrices()
        {
            return _ctx.SecuritySet.Include("PriceCollection");
        }
        //TODO : Pending investigation why datetime comparision isnt successful
        public IQueryable<Security> GetSpecificSecurityBetweenDates(string sym,DateTime startDate,DateTime endDate)
        {
            return _ctx.SecuritySet.Include("PriceCollection").Where(s => s.symbol == sym)
                .Where(p => p.PriceCollection.All(q => q.CloseDate == startDate));
        }
        public IQueryable<Security> GetSpecificSecurity(string sym)
        {
            return _ctx.SecuritySet.Include("PriceCollection").Where(s => s.symbol == sym);
        }

    }
}