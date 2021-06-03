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

        public static string CURRENT_DIR { get; } = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        private static string DB { get; } = CURRENT_DIR + @"\StocksData.db";
        private static string STOCKS { get; } = CURRENT_DIR + @"\Stocks.txt";
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
                _logger.LogError("An Exception has occurred while trying to access the database. Reason: {Exception}", ex);
            }

            //if (DateTime.UtcNow.TimeOfDay.TotalMinutes >= 690 && DateTime.UtcNow.TimeOfDay.TotalMinutes < 710 && await IsDatabaseEmpty() == false)
            //{
            //    _logger.LogInformation("Clearing Database...");
            //    await _dataAccess.ClearDatabaseAsync();
            //    _dataAccess = GetDatabaseConfig();
            //}
        }

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

        private IDataAccess GetDatabaseConfig()
        {
            void handler(Exception ex) => _logger.LogError(ex, "An Exception has occurred.");

            try
            {
                /*
                 *  true/false
                 *  server
                 *  database
                 *  username/UID
                 *  [ENTROPY]
                 *  [CIPHER]
                 */

                var lines = File.ReadAllLines(MYSQL);
                if (lines[0].ToLower() == "true") // if mysql is selected
                {
                    var mysql = new MySQLConfiguration(lines[1], lines[2], lines[3], Convert.FromBase64String(lines[4]), Convert.FromBase64String(lines[5]));

                    if ((_dataAccess is not null && _dataAccess is MySQLDataAccess && !AreConfigsEqual((_dataAccess as MySQLDataAccess).Config, mysql)) ||
                        (_dataAccess is null) ||
                        (_dataAccess is SQLiteDataAccess))
                    {
                        _logger.LogInformation("Configuring MYSQL DATABASE");
                        if (_dataAccess is MySQLDataAccess previous)
                        {
                            _logger.LogInformation($"The previous configuration: [ Server: { previous.Config.Server }, Database: { previous.Config.Database }, Username: { previous.Config.Username } ]");
                            _logger.LogInformation($"The new configuration: [ Server: { mysql.Server }, Database: { mysql.Database }, Username: { mysql.Username } ]");
                        }
                        return new MySQLDataAccess(mysql, handler);
                    }
                    else
                    {
                        return _dataAccess;
                    }
                }
                else if (_dataAccess is null || _dataAccess is MySQLDataAccess)
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

        private static bool AreConfigsEqual(MySQLConfiguration c1, MySQLConfiguration c2)
        {
            return c1.Server == c2.Server && c1.Database == c2.Database && c1.Username == c2.Username;
        }

        private async Task<bool> IsDatabaseEmpty()
        {
            return (await _dataAccess.LoadAllPricesAsync()).Count is 0;
        }
    }
}