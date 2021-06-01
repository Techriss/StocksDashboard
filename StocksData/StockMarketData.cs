using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using StocksData.Models;
using YahooFinanceApi;

namespace StocksData
{
    /// <summary>
    /// Collection of methods allowing to get live data from the Yahoo Finance API
    /// </summary>
    public static class StockMarketData
    {
        /// <summary>
        /// Provides current price for the provided stock symbol asynchronously
        /// </summary>
        /// <param name="symbol">The symbol of a company</param>
        /// <returns>The current price of single stock share for the provided company</returns>
        public static async Task<double> GetStockPrice(string symbol)
        {
            var quotes = await Yahoo.Symbols(symbol).Fields(Field.RegularMarketPrice).QueryAsync();
            var data = quotes[symbol].RegularMarketPrice;
            return data;
        }

        /// <summary>
        /// Provides current basic information for the provided company including <see cref="Field.RegularMarketPrice"/>, <see cref="Field.RegularMarketChangePercent"/>, <see cref="Field.RegularMarketChange"/>, <see cref="Field.MarketCap"/> and <see cref="Field.PostMarketChangePercent"/> asynchronously
        /// </summary>
        /// <param name="symbol">The symbol of the company</param>
        /// <returns><see cref="Security"/> containing 5 basic information fields</returns>
        public static async Task<Security> GetStockData(string symbol)
        {
            try
            {
                var data = await Yahoo.Symbols(symbol).Fields(
                    Field.RegularMarketChangePercent,
                    Field.RegularMarketChange,
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

        /// <summary>
        /// Provides all available stock information for the provided company asynchronously
        /// </summary>
        /// <param name="symbol">The symbol of the company</param>
        /// <returns><see cref="Security"/> containing all available information fields</returns>
        public static async Task<Security> GetAllStockData(string symbol)
        {
            var data = await Yahoo.Symbols(symbol).Fields(AllFields).QueryAsync();

            return data[symbol];
        }

        /// <summary>
        /// Provides a read-only list of stock prices asynchronously
        /// </summary>
        /// <param name="symbol">The symbol of the company</param>
        /// <returns><see cref="List{T}"/> of stock prices from a specific time</returns>
        public static async Task<IReadOnlyList<Candle>> GetStockHistory(string symbol)
        {
            var history = await Yahoo.GetHistoricalAsync(symbol);

            return history;
        }

        /// <summary>
        /// Provides a read-only list of stock prices for a specified time period asynchronously
        /// </summary>
        /// <param name="symbol">The symbol of the company</param>
        /// <param name="start">The date from which the data will be provided</param>
        /// <param name="period">The time period to get the price history from</param>
        /// <returns>A read-only list of stock prices</returns>
        public static async Task<IReadOnlyList<Candle>> GetStockHistoryForTime(string symbol, DateTime start, Period period)
        {
            var data = await Yahoo.GetHistoricalAsync(symbol, start, DateTime.Now, period);
            return data;
        }


        /// <summary>
        /// Provides a <see langword="true"/> value when a company with a provided symbol exists and <see langword="false"/> when it does not after checking the response from the Yahoo Finance API made synchronously
        /// </summary>
        /// <param name="symbol">The symbol to check if it exists</param>
        /// <returns><see langword="true"/> if a provided symbol exists and <see langword="false"/> if it does not</returns>
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

        /// <summary>
        /// Provides a collection of <see langword="true"/> or <see langword="false"/> values for each symbol provided representing their existance
        /// </summary>
        /// <param name="symbols">Array of symbols to check if they exist</param>
        /// <returns>A read-only dictionary of stock symbols as keys and booleans as values representing whether a specified symbol exists or not. <see langword="true"/> if a symbol exists, otherwise <see langword="false"/></returns>
        public static IReadOnlyDictionary<string, bool> CheckSymbolsExist(params string[] symbols)
        {
            var dictionary = new Dictionary<string, bool>();
            foreach (var symbol in symbols)
            {
                dictionary.Add(symbol, CheckSymbolExists(symbol));
            }

            return dictionary;
        }


        /// <summary>
        /// Provides a history of daily stock prices from last week
        /// </summary>
        /// <param name="symbol">The symbol of a company</param>
        /// <returns><see cref="IEnumerable{T}"/> of <see cref="StockPrice"/> for every day in the last week</returns>
        public static async Task<IEnumerable<StockPrice>> GetLastWeek(string symbol)
        {
            var data = await Yahoo.GetHistoricalAsync(symbol, DateTime.Now - TimeSpan.FromDays(7), DateTime.Now, Period.Daily);
            var prices = data.Select(x => new StockPrice(symbol, x.Close, x.DateTime));
            return prices;
        }

        /// <summary>
        /// Provides a history of daily stock prices from last month
        /// </summary>
        /// <param name="symbol">The symbol of a company</param>
        /// <returns><see cref="IEnumerable{T}"/> of <see cref="StockPrice"/> for every day in the last month</returns>
        public static async Task<IEnumerable<StockPrice>> GetLastMonth(string symbol)
        {
            var data = await Yahoo.GetHistoricalAsync(symbol, DateTime.Now - TimeSpan.FromDays(31), DateTime.Now, Period.Daily);
            var prices = data.Select(x => new StockPrice(symbol, x.Close, x.DateTime));
            return prices;
        }

        /// <summary>
        /// Provides a history of daily stock prices from last year
        /// </summary>
        /// <param name="symbol">The symbol of a company</param>
        /// <returns><see cref="IEnumerable{T}"/> of <see cref="StockPrice"/> for every day in the last year</returns>
        public static async Task<IEnumerable<StockPrice>> GetLastYear(string symbol)
        {
            var data = await Yahoo.GetHistoricalAsync(symbol, DateTime.Now - TimeSpan.FromDays(365), DateTime.Now, Period.Daily);
            var prices = data.Select(x => new StockPrice(symbol, x.Close, x.DateTime));

            return prices;
        }

        /// <summary>
        /// Provides a history of weekly stock prices from all time the company was public
        /// </summary>
        /// <param name="symbol">The symbol of a company</param>
        /// <returns><see cref="IEnumerable{T}"/> of <see cref="StockPrice"/> for every week when the company was public</returns>
        public static async Task<IEnumerable<StockPrice>> GetAllTime(string symbol)
        {
            var data = await Yahoo.GetHistoricalAsync(symbol, null, null, Period.Weekly);
            var prices = data.Select(x => new StockPrice(symbol, x.Close, x.DateTime));

            return prices;
        }

        /// <summary>
        /// Provides a read-only list of current live stock prices for every provided symbol
        /// </summary>
        /// <param name="symbols">Symbols of all the companies to get stock prices for</param>
        /// <returns>A read-only list of current stock prices. <see langword="null"/> when prices not available.</returns>
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
                        prices.Add(new(c.Key, Convert.ToDecimal(c.Value.RegularMarketPrice), DateTime.Now));
                    }
                }

                return prices;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Provides the current live price for the provided symbol
        /// </summary>
        /// <param name="symbol">The symbol of a company</param>
        /// <returns>A <see cref="StockPrice"/> for a specific company which symbol was provided. <see langword="null"/> when price not available.</returns>
        public static async Task<StockPrice> GetLivePrice(string symbol)
        {
            try
            {
                var quotes = await Yahoo.Symbols(symbol).Fields(Field.RegularMarketPrice, Field.MarketState).QueryAsync();
                var data = quotes[symbol];
                var price = data.RegularMarketPrice;

                if (data.MarketState == "REGULAR")
                {
                    var priceEntry = new StockPrice(symbol, Convert.ToDecimal(price), DateTime.Now);
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


        /// <summary>
        /// Provides the current state of internet connection for the user synchronously
        /// </summary>
        /// <returns><see langword="true"/> if the user has internet connection, otherwise <see langword="false"/></returns>
        public static bool CheckInternetConnection()
        {
            using (var p = new Ping())
            {
                var response = p.Send("8.8.8.8");
                return response.Status == IPStatus.Success;
            }
        }

        /// <summary>
        /// Provides the current state of internet connection for the user asynchronously
        /// </summary>
        /// <returns><see langword="true"/> if the user has internet connection, otherwise <see langword="false"/></returns>
        public static async Task<bool> CheckInternetConnectionAsync()
        {
            using (var p = new Ping())
            {
                var response = await p.SendPingAsync("8.8.8.8");
                return response.Status == IPStatus.Success;
            }
        }


        /// <summary>
        /// All available stock information fields from the Yahoo Finance API
        /// </summary>
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