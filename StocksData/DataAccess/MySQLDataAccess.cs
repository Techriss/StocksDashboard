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
        /// <summary>
        /// The connection string to the MySQL database to allow accessing and managing stored data
        /// </summary>
        private string ConnectionString { get; set; }

        /// <summary>
        /// The MySQL Configuration which allows accessing configuration information securely
        /// </summary>
        public MySQLConfiguration Config { get; init; }

        /// <summary>
        /// The action which Occurs every time an exception is thrown while attempting to access or modify the data stored in the database. Its use is recommended. When not set does nothing.
        /// </summary>
        public Action<Exception> ExceptionHandler { get; set; } = (Exception ex) => Debug.WriteLine(ex);


        /// <summary>
        /// Constructor for the MySQL Data Access requiring the database configuration encryted with the <see cref="Encryption"/> class
        /// </summary>
        /// <param name="mysql">Secure MySQL configuration for accessing and modifying database data</param>
        /// <param name="exceptionHandler">Action occurring every time an Exception is thrown while attempting to access or modify the data stored in the selected database. Does nothing when not set. The use is highly recommended.</param>
        public MySQLDataAccess(MySQLConfiguration mysql, Action<Exception> exceptionHandler = null)
        {
            ExceptionHandler = exceptionHandler ?? ExceptionHandler;
            SetDatabase(mysql);
            Config = mysql;
        }


        /// <summary>
        /// Configures the database for connections
        /// </summary>
        /// <param name="mysql">The secure MySQL configuration for the database</param>
        private void SetDatabase(MySQLConfiguration mysql)
        {
            ConnectionString = GetConnectionString(mysql);
        }

        /// <summary>
        /// Provides a connection string based on the given secure <see cref="MySQLConfiguration"/>. Required to be run on the same machine as the encryption was performed on in order to properly decrypt the database password.
        /// </summary>
        /// <param name="mysql">The secure MySQL database configuration to generate a connection string from</param>
        /// <returns>The MySQL database connection string based on the provided <see cref="MySQLConfiguration"/></returns>
        private static string GetConnectionString(MySQLConfiguration mysql)
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



        public long GetEntriesNumber()
        {
            try
            {
                using (IDbConnection cnn = new MySqlConnection(ConnectionString))
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
                using (IDbConnection cnn = new MySqlConnection(ConnectionString))
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