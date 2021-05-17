using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using StocksData.Models;

namespace StocksData
{
    public class SQLiteDataAccess : IDataAccess
    {
        private string ConnectionString { get; set; } = @"Data Source=.\StocksData.db;Version=3;";

        public Action<Exception> ExceptionHandler { get; set; } = (Exception ex) => Debug.WriteLine(ex);


        public SQLiteDataAccess(Action<Exception> exceptionHandler = null)
        {
            ExceptionHandler = exceptionHandler ?? ExceptionHandler;
        }

        public SQLiteDataAccess(string path, Action<Exception> exceptionHandler = null)
        {
            ExceptionHandler = exceptionHandler ?? ExceptionHandler;
            SetDatabase(path);
        }



        private void SetDatabase(string path)
        {
            ConnectionString = GetConnectionString(path);
            RepairDatabase();
        }

        private static string GetConnectionString(string path)
        {
            return @$"Data Source={ path };Version=3;";
        }



        public void SavePrice(StockPrice stockPrice)
        {
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
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
                using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
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
                using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
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
                using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
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
                using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
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
                using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
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
                using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
                {
                    cnn.Execute("CREATE TABLE IF NOT EXISTS LiveData (Symbol TEXT NOT NULL, Price NUMERIC NOT NULL, Time TEXT NOT NULL);");
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
                using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
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