using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using StocksData.Models;

namespace StocksData
{
    public class SQLiteDataAccess : IDataAccess
    {
        private string ConnectionString { get; set; } = @"Data Source=.\StocksData.db;Version=3;";

        public SQLiteDataAccess()
        {

        }

        public SQLiteDataAccess(string path)
        {
            SetDatabase(path);
        }



        private void SetDatabase(string path)
        {
            ConnectionString = GetConnectionString(path);
        }

        private string GetConnectionString(string path)
        {
            return @$"Data Source={ path };Version=3;";
        }



        public void SavePrice(StockPrice stockPrice)
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                cnn.Execute("INSERT INTO LiveData (Symbol, Price, Time) VALUES (@Symbol, @Price, @Time)", stockPrice);
            }
        }

        public async Task SavePriceAsync(StockPrice stockPrice)
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                await cnn.ExecuteAsync($"INSERT INTO LiveData (Symbol, Price, Time) VALUES (@Symbol, @Price, @Time)", stockPrice);
            }
        }



        public List<StockPrice> LoadAllPrices()
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                var data = cnn.Query<StockPrice>("SELECT * FROM LiveData").ToList();
                return data;
            }
        }

        public async Task<List<StockPrice>> LoadAllPricesAsync()
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                var data = (await cnn.QueryAsync<StockPrice>("SELECT * FROM LiveData")).ToList();
                return data;
            }
        }



        public void ClearDatabase()
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                cnn.Execute("DELETE FROM LiveData");
            }
        }

        public async Task ClearDatabaseAsync()
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                await cnn.ExecuteAsync("DELETE FROM LiveData");
            }
        }



        public void RepairDatabase()
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                cnn.Execute("CREATE TABLE IF NOT EXISTS LiveData (Symbol TEXT NOT NULL, Price NUMERIC NOT NULL, Time TEXT NOT NULL");
            }
        }

        public async Task RepairDatabaseAsync()
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                await cnn.ExecuteAsync("CREATE TABLE IF NOT EXISTS LiveData (Symbol TEXT NOT NULL, Price NUMERIC NOT NULL, Time TEXT NOT NULL");
            }
        }
    }
}