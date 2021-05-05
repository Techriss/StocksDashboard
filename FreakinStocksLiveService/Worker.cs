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

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                try
                {
                    await Task.Delay(10000, stoppingToken);
                    await SaveLivePriceDebugTest();
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
                        await MySQLDataAccess.SavePriceAsync(p);
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
                        await SQLiteDataAccess.SavePriceAsync(p);
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
                    await SQLiteDataAccess.SavePriceAsync(p);
                }
                else
                {
                    _logger.LogWarning("The Live Price is null");
                }
            }
        }
    }
}
