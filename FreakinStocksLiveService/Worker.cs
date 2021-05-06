using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StocksData;

namespace FreakinStocksLiveService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private static readonly string[] _symbols = { "TSLA", "NDAQ", "AAPL" };
        private readonly IDataAccess _dataAccess;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            _dataAccess = new SQLiteDataAccess();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                try
                {
                    await Task.Delay(60000, stoppingToken);
                    await SaveLivePriceSQLite();
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }
        }

        public async Task SaveLivePriceMySQL()
        {
            if (DateTime.Now.TimeOfDay.TotalMinutes >= 930 && DateTime.Now.TimeOfDay.TotalMinutes <= 1320)
            {
                var prices = await StockMarketData.GetLivePrice(_symbols);
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
                var prices = await StockMarketData.GetLivePrice(_symbols);
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

        public async Task SaveLivePriceDebugTest()
        {
            var prices = await StockMarketData.GetLivePrice(_symbols);
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
}
