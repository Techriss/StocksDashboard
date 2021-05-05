using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MySqlConnector;
using StocksData.Models;

namespace StocksData
{
    public static class MySQLDataAccess
    {
        private static string ConnectionString { get; set; } = GetConnectionString("localhost", "stocksdata", "root", "75750506200420");


        public static void SetDatabase(string server, string database, string username, string password)
        {
            ConnectionString = $"SERVER={ server };DATABASE={ database };UID={ username };PASSWORD={ password };";
        }

        public static string GetConnectionString(string server, string database, string username, string password)
        {
            return ConnectionString = $"SERVER={ server };DATABASE={ database };UID={ username };PASSWORD={ password };";
        }


        public static void SavePrice(StockPrice stockPrice)
        {
            using (IDbConnection cnn = new MySqlConnection(ConnectionString))
            {
                cnn.Execute("INSERT INTO LiveData (Symbol, Price, Time) VALUES (@Symbol, @Price, @Time)", stockPrice);
            }
        }

        public static async Task SavePriceAsync(StockPrice stockPrice)
        {
            using (IDbConnection cnn = new MySqlConnection(ConnectionString))
            {
                await cnn.ExecuteAsync($"INSERT INTO LiveData (Symbol, Price, Time) VALUES (@Symbol, @Price, @Time)", stockPrice);
            }
        }



        public static List<StockPrice> LoadAllPrices()
        {
            using (IDbConnection cnn = new MySqlConnection(ConnectionString))
            {
                var data = cnn.Query<StockPrice>("SELECT * FROM LiveData").ToList();
                return data;
            }
        }

        public static async Task<List<StockPrice>> LoadAllPricesAsync()
        {
            using (IDbConnection cnn = new MySqlConnection(ConnectionString))
            {
                var data = (await cnn.QueryAsync<StockPrice>("SELECT * FROM LiveData")).ToList();
                return data;
            }
        }



        public static void ClearDatabase()
        {
            using (IDbConnection cnn = new MySqlConnection(ConnectionString))
            {
                cnn.Execute("DELETE FROM LiveData");
            }
        }

        public static async Task ClearDatabaseAsync()
        {
            using (IDbConnection cnn = new MySqlConnection(ConnectionString))
            {
                await cnn.ExecuteAsync("DELETE FROM LiveData");
            }
        }



        public static void RepairDatabase()
        {
            using (IDbConnection cnn = new MySqlConnection(ConnectionString))
            {
                //cnn.Execute("CREATE TABLE IF NOT EXISTS LiveData (Symbol TEXT NOT NULL, Price NUMERIC NOT NULL, Time TEXT NOT NULL");
            }
        }

        public static async Task RepairDatabaseAsync()
        {
            using (IDbConnection cnn = new MySqlConnection(ConnectionString))
            {
                // await cnn.ExecuteAsync("CREATE TABLE IF NOT EXISTS LiveData (Symbol TEXT NOT NULL, Price NUMERIC NOT NULL, Time TEXT NOT NULL");
            }
        }
    }
}
