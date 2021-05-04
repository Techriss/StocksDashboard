using System;

namespace StocksData.Models
{
    public record StockPrice
    {
        public string Symbol { get; init; }

        public decimal Price { get; init; }

        public string Time { get; init; }

        public StockPrice(string symbol, decimal price, DateTime time)
        {
            Symbol = symbol;
            Price = price;
            Time = time.ToString();
        }

        public StockPrice(string symbol, decimal price, string time)
        {
            Symbol = symbol;
            Price = price;
            Time = time;
        }

        public void Deconstruct(out string symbol, out decimal price, out DateTime time)
        {
            symbol = Symbol;
            price = Price;
            time = DateTime.Parse(Time);
        }
    }
}
