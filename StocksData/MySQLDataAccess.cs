using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FreakinStocksUI.Helpers;
using MySqlConnector;
using StocksData.Models;

namespace StocksData
{
    public class MySQLDataAccess : IDataAccess
    {
        private string ConnectionString { get; set; }

        public MySQLDataAccess(string server, string database, string username, byte[] cipher, byte[] entropy)
        {
            SetDatabase(server, database, username, cipher, entropy);
        }



        private void SetDatabase(string server, string database, string username, byte[] cipher, byte[] entropy)
        {
            ConnectionString = GetConnectionString(server, database, username, cipher, entropy);
        }

        private string GetConnectionString(string server, string database, string username, byte[] cipher, byte[] entropy)
        {
            return $"SERVER={ server };DATABASE={ database };UID={ username };PASSWORD={ Encryption.Decrypt(cipher, entropy) };";
        }



        public void SavePrice(StockPrice stockPrice)
        {
            using (IDbConnection cnn = new MySqlConnection(ConnectionString))
            {
                cnn.Execute("INSERT INTO LiveData (Symbol, Price, Time) VALUES (@Symbol, @Price, @Time)", stockPrice);
            }
        }

        public async Task SavePriceAsync(StockPrice stockPrice)
        {
            using (IDbConnection cnn = new MySqlConnection(ConnectionString))
            {
                await cnn.ExecuteAsync($"INSERT INTO LiveData (Symbol, Price, Time) VALUES (@Symbol, @Price, @Time)", stockPrice);
            }
        }



        public List<StockPrice> LoadAllPrices()
        {
            using (IDbConnection cnn = new MySqlConnection(ConnectionString))
            {
                var data = cnn.Query<StockPrice>("SELECT * FROM LiveData").ToList();
                return data;
            }
        }

        public async Task<List<StockPrice>> LoadAllPricesAsync()
        {
            using (IDbConnection cnn = new MySqlConnection(ConnectionString))
            {
                var data = (await cnn.QueryAsync<StockPrice>("SELECT * FROM LiveData")).ToList();
                return data;
            }
        }



        public void ClearDatabase()
        {
            using (IDbConnection cnn = new MySqlConnection(ConnectionString))
            {
                cnn.Execute("DELETE FROM LiveData");
            }
        }

        public async Task ClearDatabaseAsync()
        {
            using (IDbConnection cnn = new MySqlConnection(ConnectionString))
            {
                await cnn.ExecuteAsync("DELETE FROM LiveData");
            }
        }



        public void RepairDatabase()
        {
            using (IDbConnection cnn = new MySqlConnection(ConnectionString))
            {
                //cnn.Execute("CREATE TABLE IF NOT EXISTS LiveData (Symbol TEXT NOT NULL, Price NUMERIC NOT NULL, Time TEXT NOT NULL");
            }
        }

        public async Task RepairDatabaseAsync()
        {
            using (IDbConnection cnn = new MySqlConnection(ConnectionString))
            {
                await Task.Delay(100);
                // await cnn.ExecuteAsync("CREATE TABLE IF NOT EXISTS LiveData (Symbol TEXT NOT NULL, Price NUMERIC NOT NULL, Time TEXT NOT NULL");
            }
        }
    }
}
