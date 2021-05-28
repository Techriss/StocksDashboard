using System;
using System.Windows.Media;

namespace FreakinStocksUI.ViewModels
{
    public record ValueChange
    {
        public string Change { get; init; }

        public SolidColorBrush Color { get; init; }

        public ValueChange(double change)
        {
            var percent = Math.Round(change, 2);
            Change = $"{ GetSign(percent) }{ percent }%";
            Color = GetColorForValue(change);
        }

        public static SolidColorBrush GetColorForValue(double value)
        {
            return value < 0 ? new(Colors.IndianRed) : new(Colors.ForestGreen);
        }

        public static char GetSign(double percent)
        {
            return percent > 0 ? '+' : new();
        }
    }
}
