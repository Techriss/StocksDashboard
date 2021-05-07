using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StocksData;

namespace FreakinStocksLiveService
{
    public class Worker : BackgroundService
    {
        public static string[] Symbols { get; set; }

        private readonly ILogger<Worker> _logger;
        private readonly IDataAccess _dataAccess;

        private static readonly string CURRENT_DIR = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        private const string DB = @"\StocksData.db";
        private const string STOCKS = @"\Stocks.txt";

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            _dataAccess = new SQLiteDataAccess(CURRENT_DIR + DB);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Service running at: {time}", DateTimeOffset.Now);

                try
                {
                    await Task.Delay(60000, stoppingToken);
                    await RefreshSymbols();
                    await SaveLivePriceSQLite();
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


        public async Task SaveLivePriceMySQL()
        {
            if (DateTime.Now.TimeOfDay.TotalMinutes >= 930 && DateTime.Now.TimeOfDay.TotalMinutes <= 1320)
            {
                var prices = await StockMarketData.GetLivePrice(Symbols);
                foreach (var p in prices)
                {
                    if (p is not null)
                    {
                        await _dataAccess.SavePriceAsync(p);
                        _logger.LogInformation("Saved Price");
                    }
                    else
                    {
                        _logger.LogWarning("The Live Price is null");
                    }
                }
            }
        }

        public async Task SaveLivePriceSQLite()
        {
            if (DateTime.Now.TimeOfDay.TotalMinutes >= 930 && DateTime.Now.TimeOfDay.TotalMinutes <= 1320)
            {
                var prices = await StockMarketData.GetLivePrice(Symbols);
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
            else if (DateTime.Now.TimeOfDay.TotalMinutes >= 925 && DateTime.Now.TimeOfDay.TotalMinutes < 930)
            {
                _logger.LogInformation("Clearing Database...");
                await _dataAccess.ClearDatabaseAsync();
            }
        }

        public async Task SaveLivePriceDebugTest()
        {
            var prices = await StockMarketData.GetLivePrice(Symbols);
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

        public async Task RefreshSymbols()
        {
            try
            {
                if (File.Exists(CURRENT_DIR + STOCKS))
                {
                    var lines = await File.ReadAllLinesAsync(CURRENT_DIR + STOCKS);
                    var dir = StockMarketData.CheckSymbolsExist(lines);
                    var valid = dir.Where(x => x.Value).Select(x => x.Key);

                    if (valid.Count() != lines.Length)
                    {
                        await File.WriteAllLinesAsync(CURRENT_DIR + STOCKS, valid);
                        _logger.LogWarning("Invalid Symbol found in text file.");
                    }

                    Symbols = valid.ToArray();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not save to file { CURRENT_DIR + STOCKS } Reason: { ex.Message }");
            }
        }
    }
}