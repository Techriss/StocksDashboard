using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MySqlConnector;
using StocksData.Models;

namespace StocksData
{
    public class MySQLDataAccess : IDataAccess
    {
        private string ConnectionString { get; set; }

        public MySQLConfiguration Config { get; init; }

        public Action<Exception> ExceptionHandler { get; set; } = (Exception ex) => Debug.WriteLine(ex);



        public MySQLDataAccess(MySQLConfiguration mysql, Action<Exception> exceptionHandler = null)
        {
            ExceptionHandler = exceptionHandler ?? ExceptionHandler;
            SetDatabase(mysql);
            Config = mysql;
        }



        private void SetDatabase(MySQLConfiguration mysql)
        {
            ConnectionString = GetConnectionString(mysql);
        }

        private string GetConnectionString(MySQLConfiguration mysql)
        {
            return $"SERVER={ mysql.Server };DATABASE={ mysql.Database };UID={ mysql.Username };PASSWORD={ Encryption.Decrypt(mysql.Cipher, mysql.Entropy) };";
        }



        public void SavePrice(StockPrice stockPrice)
        {
            try
            {
                using (IDbConnection cnn = new MySqlConnection(ConnectionString))
                {
                    cnn.Execute("INSERT INTO LiveData (Symbol, Price, Time) VALUES (@Symbol, @Price, @Time)", stockPrice);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler?.Invoke(ex);
            }
        }

        public async Task SavePriceAsync(StockPrice stockPrice)
        {
            try
            {
                using (IDbConnection cnn = new MySqlConnection(ConnectionString))
                {
                    await cnn.ExecuteAsync($"INSERT INTO LiveData (Symbol, Price, Time) VALUES (@Symbol, @Price, @Time)", stockPrice);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler?.Invoke(ex);
            }
        }



        public List<StockPrice> LoadAllPrices()
        {
            try
            {
                using (IDbConnection cnn = new MySqlConnection(ConnectionString))
                {
                    var data = cnn.Query<StockPrice>("SELECT * FROM LiveData").ToList();
                    return data;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler?.Invoke(ex);
                return Enumerable.Empty<StockPrice>().AsList();
            }
        }

        public async Task<List<StockPrice>> LoadAllPricesAsync()
        {
            try
            {
                using (IDbConnection cnn = new MySqlConnection(ConnectionString))
                {
                    var data = (await cnn.QueryAsync<StockPrice>("SELECT * FROM LiveData")).ToList();
                    return data;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler?.Invoke(ex);
                return Enumerable.Empty<StockPrice>().AsList();
            }
        }



        public void ClearDatabase()
        {
            try
            {
                using (IDbConnection cnn = new MySqlConnection(ConnectionString))
                {
                    cnn.Execute("DELETE FROM LiveData");
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler?.Invoke(ex);
            }
        }

        public async Task ClearDatabaseAsync()
        {
            try
            {
                using (IDbConnection cnn = new MySqlConnection(ConnectionString))
                {
                    await cnn.ExecuteAsync("DELETE FROM LiveData");
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler?.Invoke(ex);
            }
        }



        public void RepairDatabase()
        {
            try
            {
                using (IDbConnection cnn = new MySqlConnection(ConnectionString))
                {
                    cnn.Execute("CREATE TABLE LiveData (Symbol TEXT NOT NULL, Price NUMERIC NOT NULL, Time TEXT NOT NULL);");
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler?.Invoke(ex);
            }
        }

        public async Task RepairDatabaseAsync()
        {
            try
            {
                using (IDbConnection cnn = new MySqlConnection(ConnectionString))
                {
                    await cnn.ExecuteAsync("CREATE TABLE IF NOT EXISTS LiveData (Symbol TEXT NOT NULL, Price NUMERIC NOT NULL, Time TEXT NOT NULL);");
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler?.Invoke(ex);
            }
        }
    }
}