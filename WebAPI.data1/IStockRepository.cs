using System.Linq;
using WebAPI.data1.Entities;

namespace WebAPI.data1
{
    public interface IStockRepository
    {
        IQueryable<Security> GetAllSecurities();
    }
}