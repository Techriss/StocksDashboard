using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using StocksData.Models;
using YahooFinanceApi;

namespace StocksData
{
    public static class StockMarketData
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

        public static async Task<Security> GetAllStockData(string symbol)
        {
            var data = await Yahoo.Symbols(symbol).Fields(
            #region Fields
            Field.Ask,
            Field.AskSize,

            Field.AverageDailyVolume10Day,
            Field.AverageDailyVolume3Month,

            Field.Bid,
            Field.BidSize,
            Field.BookValue,
            Field.Currency,
            Field.EpsForward,
            Field.EpsTrailingTwelveMonths,
            Field.Exchange,
            Field.ExchangeDataDelayedBy,
            Field.ExchangeTimezoneName,
            Field.ExchangeTimezoneShortName,


            Field.FiftyDayAverage,
            Field.FiftyDayAverageChange,
            Field.FiftyDayAverageChangePercent,

            Field.FiftyTwoWeekHigh,
            Field.FiftyTwoWeekHighChange,
            Field.FiftyTwoWeekHighChangePercent,

            Field.FiftyTwoWeekLow,
            Field.FiftyTwoWeekLowChange,
            Field.FiftyTwoWeekLowChangePercent,


            Field.FinancialCurrency,
            Field.ForwardPE,
            Field.FullExchangeName,
            Field.GmtOffSetMilliseconds,
            Field.Language,
            Field.LongName,
            Field.Market,
            Field.MarketCap,
            Field.MarketState,
            Field.MessageBoardId,
            Field.PriceHint,
            Field.PriceToBook,
            Field.QuoteSourceName,

            Field.QuoteType,
            Field.RegularMarketChange,
            Field.RegularMarketChangePercent,
            Field.RegularMarketDayHigh,
            Field.RegularMarketDayLow,
            Field.RegularMarketOpen,
            Field.RegularMarketPreviousClose,
            Field.RegularMarketPrice,
            Field.RegularMarketTime,

            Field.RegularMarketVolume,
            Field.SharesOutstanding,
            Field.ShortName,
            Field.SourceInterval,
            Field.Symbol,
            Field.Tradeable,

            Field.TrailingPE,
            Field.TwoHundredDayAverage,
            Field.TwoHundredDayAverageChange,
            Field.TwoHundredDayAverageChangePercent
            #endregion
            ).QueryAsync();

            return data[symbol];
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


        public static bool CheckSymbolExists(string symbol)
        {
            try
            {
                var data = Task.Run(async () => await Yahoo.Symbols(symbol).Fields(Field.Symbol).QueryAsync()).GetAwaiter().GetResult();

                return data is not null && data.Count > 0;
            }
            catch
            {
                return false;
            }
        }


        public static async Task<IEnumerable<StockPrice>> GetLastWeek(string symbol)
        {
            var data = await Yahoo.GetHistoricalAsync(symbol, DateTime.Now - TimeSpan.FromDays(7), DateTime.Now, Period.Daily);
            var prices = data.Select(x => new StockPrice(symbol, x.Close, x.DateTime));
            return prices;
        }

        public static async Task<IEnumerable<StockPrice>> GetLastMonth(string symbol)
        {
            var data = await Yahoo.GetHistoricalAsync(symbol, DateTime.Now - TimeSpan.FromDays(31), DateTime.Now, Period.Daily);
            var prices = data.Select(x => new StockPrice(symbol, x.Close, x.DateTime));
            return prices;
        }

        public static async Task<IEnumerable<StockPrice>> GetLastYear(string symbol)
        {
            var data = await Yahoo.GetHistoricalAsync(symbol, DateTime.Now - TimeSpan.FromDays(365), DateTime.Now, Period.Daily);
            var prices = data.Select(x => new StockPrice(symbol, x.Close, x.DateTime));

            return prices;
        }

        public static async Task<IEnumerable<StockPrice>> GetAllTime(string symbol)
        {
            var data = await Yahoo.GetHistoricalAsync(symbol, null, null, Period.Weekly);
            var prices = data.Select(x => new StockPrice(symbol, x.Close, x.DateTime));

            return prices;
        }


        public static async Task<List<StockPrice>> GetLivePrice(params string[] symbols)
        {
            try
            {
                var data = await Yahoo.Symbols(symbols).Fields(Field.RegularMarketPrice).QueryAsync();
                var prices = new List<StockPrice>();
                data.ToList().ForEach(c => prices.Add(new(c.Key, Convert.ToDecimal(c.Value.RegularMarketPrice), DateTime.Now)));

                return prices;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new();
            }
        }

        public static async Task<StockPrice> GetLivePrice(string symbol)
        {
            try
            {
                var data = await Yahoo.Symbols(symbol).Fields(Field.RegularMarketPrice).QueryAsync();
                var price = new StockPrice(symbol, Convert.ToDecimal(data[symbol].RegularMarketPrice), DateTime.Now);

                return price;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
    }
}