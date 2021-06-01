using System.Collections.Generic;
using System.Threading.Tasks;
using StocksData.Models;

namespace StocksData
{
    /// <summary>
    /// Interface for implementing Stocks Data Data Access Layer for various databases
    /// </summary>
    public interface IDataAccess
    {
        /// <summary>
        /// Saves the given <see cref="StockPrice"/> to the database
        /// </summary>
        /// <param name="stockPrice">The <see cref="StockPrice"/> that will be saved in the database</param>
        public void SavePrice(StockPrice stockPrice);
        /// <summary>
        /// Saves the given <see cref="StockPrice"/> to the database asynchronously
        /// </summary>
        /// <param name="stockPrice">The <see cref="StockPrice"/> that will be saved in the database</param>
        public Task SavePriceAsync(StockPrice stockPrice);

        /// <summary>
        /// Loads all stored prices from the database
        /// </summary>
        /// <returns>A <see cref="List{T}"/> of <see cref="StockPrice"/></returns>
        public List<StockPrice> LoadAllPrices();
        /// <summary>
        /// Loads all stored prices from the database asynchronously
        /// </summary>
        /// <returns>A <see cref="List{T}"/> of <see cref="StockPrice"/></returns>
        public Task<List<StockPrice>> LoadAllPricesAsync();

        /// <summary>
        /// Deletes all data in the database
        /// </summary>
        public void ClearDatabase();
        /// <summary>
        /// Deletes all data in the database asynchronously
        /// </summary>
        /// <returns></returns>
        public Task ClearDatabaseAsync();

        /// <summary>
        /// Creates all needed tables in the database in attempt to check or restore its integrity
        /// </summary>
        public void RepairDatabase();
        /// <summary>
        /// Creates all needed tables in the database in attempt to check or restore its integrity asynchronously
        /// </summary>
        public Task RepairDatabaseAsync();

        /// <summary>
        /// Gets the current number of all stored entries in the database
        /// </summary>
        /// <returns>The number of all database entries as a <see cref="long"/></returns>
        public long GetEntriesNumber();
        /// <summary>
        /// Gets the current number of all stored entries in the database asynchronously
        /// </summary>
        /// <returns>The number of all database entries as a <see cref="long"/></returns>
        public Task<long> GetEntriesNumberAsync();
    }
}
