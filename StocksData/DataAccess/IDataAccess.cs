using System.Collections.Generic;
using System.Threading.Tasks;
using StocksData.Models;

namespace StocksData
{
    public interface IDataAccess
    {
        public void SavePrice(StockPrice stockPrice);
        public Task SavePriceAsync(StockPrice stockPrice);

        public List<StockPrice> LoadAllPrices();
        public Task<List<StockPrice>> LoadAllPricesAsync();

        public void ClearDatabase();
        public Task ClearDatabaseAsync();

        public void RepairDatabase();
        public Task RepairDatabaseAsync();

        public long GetEntriesNumber();
        public Task<long> GetEntriesNumberAsync();
    }
}
