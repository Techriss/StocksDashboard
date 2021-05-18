using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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
                var data = await Yahoo.Symbols(symbol).Fields(
                    Field.RegularMarketChangePercent,
                    Field.RegularMarketPrice,
                    Field.MarketCap,
                    Field.PostMarketChangePercent).QueryAsync();

                return data[symbol];
            }
            catch
            {
                return null;
            }
        }

        public static async Task<Security> GetAllStockData(string symbol)
        {
            var data = await Yahoo.Symbols(symbol).Fields(AllFields).QueryAsync();

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

        public static IReadOnlyDictionary<string, bool> CheckSymbolsExist(params string[] symbols)
        {
            var dictionary = new Dictionary<string, bool>();
            foreach (var symbol in symbols)
            {
                dictionary.Add(symbol, CheckSymbolExists(symbol));
            }

            return dictionary;
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


        public static async Task<IReadOnlyList<StockPrice>> GetLivePrice(params string[] symbols)
        {
            try
            {
                var data = await Yahoo.Symbols(symbols).Fields(Field.RegularMarketPrice, Field.MarketState).QueryAsync();
                var prices = new List<StockPrice>();

                foreach (var c in data.ToList())
                {
                    if (c.Value.MarketState == "REGULAR")
                    {
                        prices.Add(new(c.Key, Convert.ToDecimal(c.Value.RegularMarketPrice), DateTime.UtcNow));
                    }
                }

                return prices;
            }
            catch
            {
                return null;
            }
        }

        public static async Task<StockPrice> GetLivePrice(string symbol)
        {
            try
            {
                var quotes = await Yahoo.Symbols(symbol).Fields(Field.RegularMarketPrice, Field.MarketState).QueryAsync();
                var data = quotes[symbol];
                var price = data.RegularMarketPrice;

                if (data.MarketState == "REGULAR")
                {
                    var priceEntry = new StockPrice(symbol, Convert.ToDecimal(price), DateTime.UtcNow);
                    return priceEntry;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }


        public static bool CheckInternetConnection()
        {
            using (var p = new Ping())
            {
                var response = p.Send("8.8.8.8");
                return response.Status == IPStatus.Success;
            }
        }

        public static async Task<bool> CheckInternetConnectionAsync()
        {
            using (var p = new Ping())
            {
                var response = await p.SendPingAsync("8.8.8.8");
                return response.Status == IPStatus.Success;
            }
        }


        public static readonly Field[] AllFields =
        {
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
        };
    }
}