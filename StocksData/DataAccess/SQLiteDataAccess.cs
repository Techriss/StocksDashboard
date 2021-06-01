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
        /// <summary>
        /// The connection string to the integrated SQLite self-contained database to allow accessing and managing stored data
        /// </summary>
        private string ConnectionString { get; set; } = @"Data Source=.\StocksData.db;Version=3;";

        /// <summary>
        /// The action which occurrs every time an exception is thrown while attempting to access or modify the data stored in the database. Its use is recommended. When not set does nothing.
        /// </summary>
        public Action<Exception> ExceptionHandler { get; set; } = (Exception ex) => Debug.WriteLine(ex);


        /// <summary>
        /// Constructor for the integrated SQLite database Data Access
        /// </summary>
        /// <param name="exceptionHandler">Action occurring every time an Exception is thrown while attempting to access or modify the data stored in the selected database. Does nothing when not set. The use is highly recommended.</param>
        public SQLiteDataAccess(Action<Exception> exceptionHandler = null)
        {
            ExceptionHandler = exceptionHandler ?? ExceptionHandler;
        }

        /// <summary>
        /// Constructor for an external SQLite database Data Access requiring the path to the external self-contained database
        /// </summary>
        /// <param name="path">The path to an external self-contained SQLite Database</param>
        /// <param name="exceptionHandler">Action occurring every time an Exception is thrown while attempting to access or modify the data stored in the selected database. Does nothing when not set. The use is highly recommended.</param>
        public SQLiteDataAccess(string path, Action<Exception> exceptionHandler = null)
        {
            ExceptionHandler = exceptionHandler ?? ExceptionHandler;
            SetDatabase(path);
        }


        /// <summary>
        /// Configures integrated and external SQLite databases using their connection string and checks their integrity
        /// </summary>
        /// <param name="path">The path to the self-contained SQLite database</param>
        private void SetDatabase(string path)
        {
            ConnectionString = GetConnectionString(path);
            RepairDatabase();
        }

        /// <summary>
        /// Provides a SQLite v3 connection string from the given path to the database
        /// </summary>
        /// <param name="path">The path to the self-contained SQLite database</param>
        /// <returns>The database connection string generated from the provided database path</returns>
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


        public long GetEntriesNumber()
        {
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
                {
                    var entries = cnn.Query<StockPrice>("SELECT * FROM LiveData");
                    return entries.LongCount();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Invoke(ex);
                return 0;
            }
        }

        public async Task<long> GetEntriesNumberAsync()
        {
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
                {
                    var entries = await cnn.QueryAsync<StockPrice>("SELECT * FROM LiveData");
                    return entries.LongCount();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Invoke(ex);
                return 0;
            }
        }
    }
}