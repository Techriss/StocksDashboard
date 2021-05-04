using System;

namespace StocksData.Models
{
    public record StockPrice
    {
        public int Id { get; init; }

        public decimal Price { get; set; }

        public DateTime Time { get; set; }

        public StockPrice(int id, decimal price, DateTime time)
        {
            Id = id;
            Price = price;
            Time = time;
        }
    }
}
