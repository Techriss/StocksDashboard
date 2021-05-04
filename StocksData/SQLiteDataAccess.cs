using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using StocksData.Models;

namespace StocksData
{
    public static class SQLiteDataAccess
    {
        private static readonly string ConnectionString = @"Data Source=.\StocksData.db;Version=3;";


        public static void SavePrice(StockPrice stockPrice)
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                cnn.Execute("INSERT INTO LiveData (Symbol, Price, Time) VALUES (@Symbol, @Price, @Time)", stockPrice);
            }
        }

        public static async Task SavePriceAsync(StockPrice stockPrice)
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                await cnn.ExecuteAsync($"INSERT INTO LiveData (Symbol, Price, Time) VALUES (@Symbol, @Price, @Time)", stockPrice);
            }
        }


        public static List<StockPrice> LoadAllPrices()
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                var data = cnn.Query<StockPrice>("SELECT * FROM LiveData").ToList();
                return data;
            }
        }

        public static async Task<List<StockPrice>> LoadAllPricesAsync()
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                var data = (await cnn.QueryAsync<StockPrice>("SELECT * FROM LiveData")).ToList();
                return data;
            }
        }



        public static void RepairDatabase()
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                cnn.Execute("CREATE TABLE IF NOT EXISTS LiveData (Symbol TEXT NOT NULL, Price NUMERIC NOT NULL, Time TEXT NOT NULL");
            }
        }

        public static async Task RepairDatabaseAsync()
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                await cnn.ExecuteAsync("CREATE TABLE IF NOT EXISTS LiveData (Symbol TEXT NOT NULL, Price NUMERIC NOT NULL, Time TEXT NOT NULL");
            }
        }
    }
}