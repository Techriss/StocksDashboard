using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StocksData;
using StocksData.Models;

namespace FreakinStocksLiveService
{
    public class Worker : BackgroundService
    {
        public static string[] Symbols { get; set; }

        private readonly ILogger<Worker> _logger;
        private IDataAccess _dataAccess;

        /// <summary>
        /// The current path to the location of the running 'Freakin Stocks Live Data.exe' executable
        /// </summary>
        public static string CURRENT_DIR { get; } = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        /// <summary>
        /// The full path to the SQLite integrated database
        /// </summary>
        private static string DB { get; } = CURRENT_DIR + @"\StocksData.db";
        /// <summary>
        /// The full path to the Stocks.txt file containing all liked stock symbols
        /// </summary>
        private static string STOCKS { get; } = CURRENT_DIR + @"\Stocks.txt";
        /// <summary>
        /// The full path to the MySQL.txt MySQL configuration file containing the database selection and the secure configuration information for the selected MySQL database if selected
        /// </summary>
        private static string MYSQL { get; } = CURRENT_DIR + @"\MySQL.txt";


        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            _dataAccess = GetDatabaseConfig();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Service running at: {time}", DateTimeOffset.Now);

                try
                {
                    await Task.Delay(60000, stoppingToken);
                    _dataAccess = GetDatabaseConfig();
                    await RefreshSymbols();
                    await SaveLivePrice();
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping the service...");
            return base.StopAsync(cancellationToken);
        }


        /// <summary>
        /// Gets current stock price for every symbol in the public Stocks array and, if they are not null, saves them to the configured database asynchronously
        /// </summary>
        /// <returns></returns>
        private async Task SaveLivePrice()
        {
            try
            {
                var prices = await StockMarketData.GetLivePrice(Symbols);
                if (prices?.Count > 0) _logger.LogInformation($"Saving { prices?.Count } Stock Prices...");

                if (prices is not null)
                {
                    foreach (var p in prices)
                    {
                        if (p is not null)
                        {
                            await _dataAccess.SavePriceAsync(p);
                        }
                        else
                        {
                            _logger.LogWarning("The Live Price is null");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("An Exception has Occured while trying to access the database. Reason: {Exception}", ex);
            }

            //if (DateTime.UtcNow.TimeOfDay.TotalMinutes >= 690 && DateTime.UtcNow.TimeOfDay.TotalMinutes < 710 && await IsDatabaseEmpty() == false)
            //{
            //    _logger.LogInformation("Clearing Database...");
            //    await _dataAccess.ClearDatabaseAsync();
            //    _dataAccess = GetDatabaseConfig();
            //}
        }

        /// <summary>
        /// Loads all the symbols in the Symbols.txt text file, checks if they exist and assigns them to the public array asynchronously
        /// </summary>
        /// <returns></returns>
        private async Task RefreshSymbols()
        {
            try
            {
                if (File.Exists(STOCKS))
                {
                    var lines = await File.ReadAllLinesAsync(STOCKS);
                    var dir = StockMarketData.CheckSymbolsExist(lines);
                    var valid = dir.Where(x => x.Value).Select(x => x.Key);

                    if (valid?.Count() != lines?.Length)
                    {
                        await File.WriteAllLinesAsync(STOCKS, valid);
                        _logger.LogWarning("Invalid Symbol found in text file.");
                    }

                    Symbols = valid?.ToArray();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not save to file { STOCKS } Reason: { ex.Message }");
            }
        }

        /// <summary>
        /// Gets the database selection from the MySQL.txt text file and creates a database instance from the saved MySQL configuration or integrated SQLite one
        /// </summary>
        /// <returns>The <see cref="IDataAccess"/> for the selected database in the text file</returns>
        private IDataAccess GetDatabaseConfig()
        {
            void handler(Exception ex) => _logger.LogError(ex, "An Exception has Occured.");

            try
            {
                var lines = File.ReadAllLines(MYSQL);

                if (lines[0].ToLower() == "true")
                {
                    var mysql = GetConfigFromLines(lines);

                    if ((_dataAccess is MySQLDataAccess prev && !AreConfigsEqual(prev.Config, mysql)) ||
                        (_dataAccess is null or SQLiteDataAccess))
                    {
                        _logger.LogInformation("Configuring MYSQL DATABASE");

                        LogIfMySQLChanged(mysql);

                        return new MySQLDataAccess(mysql, handler);
                    }
                    else
                    {
                        return _dataAccess;
                    }
                }
                else if (_dataAccess is null or MySQLDataAccess)
                {
                    _logger.LogInformation("Configuring SQLITE DATABASE");
                    return new SQLiteDataAccess(DB, handler);
                }
                else
                {
                    return _dataAccess;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not read from MySQL Config { MYSQL } Reason: { ex.Message }");
                _logger.LogInformation("Configuring SQLITE DATABASE");

                try
                {
                    return new SQLiteDataAccess(DB, handler);
                }
                catch
                {
                    return null;
                }
            }
        }


        /// <summary>
        /// Checks if the values of 2 <see cref="MySQLConfiguration"/>s are equal by looking at the server, database and username. This function does not check the database password for security reasons.
        /// </summary>
        /// <param name="c1">The first configuration</param>
        /// <param name="c2">The second configuration</param>
        /// <returns><see langword="true"/> if the configuration is the same, otherwise <see langword="false"/></returns>
        private static bool AreConfigsEqual(MySQLConfiguration c1, MySQLConfiguration c2)
        {
            return c1.Server == c2.Server && c1.Database == c2.Database && c1.Username == c2.Username;
        }

        /// <summary>
        /// Counts all the entries in the database and check if the count is equal to 0
        /// </summary>
        /// <returns><see langword="true"/> if the database does not contain any entries, otherwise <see langword="false"/></returns>
        private async Task<bool> IsDatabaseEmpty()
        {
            return (await _dataAccess.LoadAllPricesAsync()).Count is 0;
        }

        /// <summary>
        /// Retrieves the database configuration from the <paramref name="lines"/> of a MySQL.txt text file and creates a new <see cref="MySQLConfiguration"/> based on the data
        /// </summary>
        /// <param name="lines">The lines from the MySQL.txt text file</param>
        /// <returns>The MySQL configuration retrieved from MySQL.txt <paramref name="lines"/></returns>
        private static MySQLConfiguration GetConfigFromLines(string[] lines)
        {
            /*  -- MySQL CONFIGURATION TEMPLATE --
             * 
             *  true/false
             *  server
             *  database
             *  username/UID
             *  [ENTROPY]
             *  [CIPHER]
             */

            return new MySQLConfiguration(lines[1], lines[2], lines[3], Convert.FromBase64String(lines[4]), Convert.FromBase64String(lines[5]));
        }

        /// <summary>
        /// Checks if the current database is MySQL and logs the differences between the previous and the current one
        /// </summary>
        /// <param name="mysql">The new MySQL configuration</param>
        private void LogIfMySQLChanged(MySQLConfiguration mysql)
        {
            if (_dataAccess is MySQLDataAccess previous)
            {
                _logger.LogInformation($"The previous configuration: [ Server: { previous.Config.Server }, Database: { previous.Config.Database }, Username: { previous.Config.Username } ]");
                _logger.LogInformation($"The new configuration: [ Server: { mysql.Server }, Database: { mysql.Database }, Username: { mysql.Username } ]");
            }
        }
    }
}