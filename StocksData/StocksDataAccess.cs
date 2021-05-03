using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YahooFinanceApi;

namespace StocksData
{
    public static class StocksDataAccess
    {
        public static async Task<double> GetStockPrice(string symbol)
        {
            var quotes = await Yahoo.Symbols(symbol).Fields(Field.RegularMarketPrice).QueryAsync();
            var data = quotes[symbol].RegularMarketPrice;
            return data;
        }

        public static async Task<Security> GetStockData(string symbol)
        {
            try
            {
                var data = await Yahoo.Symbols(symbol).Fields(Field.RegularMarketChangePercent, Field.RegularMarketPrice, Field.MarketCap, Field.PostMarketChangePercent).QueryAsync();
                return data[symbol];
            }
            catch
            {
                return null;
            }
        }

        public static async Task<IReadOnlyList<Candle>> GetStockHistory(string symbol)
        {
            var history = await Yahoo.GetHistoricalAsync(symbol);

            return history;
        }

        public static async Task<IReadOnlyList<Candle>> GetStockHistoryForTime(string symbol, DateTime start, Period period)
        {
            var data = await Yahoo.GetHistoricalAsync(symbol, start, DateTime.Now, period);
            return data;
        }


        public static async Task<IEnumerable<decimal>> GetLastWeek(string symbol)
        {
            var data = await Yahoo.GetHistoricalAsync(symbol, DateTime.Now - TimeSpan.FromDays(7), DateTime.Now, Period.Daily);
            var prices = data.Select(c => c.Close);
            return prices;
        }

        public static async Task<IEnumerable<decimal>> GetLastMonth(string symbol)
        {
            var data = await Yahoo.GetHistoricalAsync(symbol, DateTime.Now - TimeSpan.FromDays(31), DateTime.Now, Period.Daily);
            var prices = data.Select(c => c.Close);
            return prices;
        }

        public static async Task<IEnumerable<decimal>> GetLastYear(string symbol)
        {
            var data = await Yahoo.GetHistoricalAsync(symbol, DateTime.Now - TimeSpan.FromDays(365), DateTime.Now, Period.Daily);
            var prices = data.Select(c => c.Close);
            return prices;
        }

        public static async Task<IEnumerable<decimal>> GetAllTime(string symbol)
        {
            var data = await Yahoo.GetHistoricalAsync(symbol, null, null, Period.Weekly);
            var prices = data.Select(c => c.Close);
            return prices;
        }
    }
}
