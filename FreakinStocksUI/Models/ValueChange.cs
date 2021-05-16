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
            Change = $"{ Math.Round(change, 2) }%";
            Color = GetColorForValue(change);
        }

        public static SolidColorBrush GetColorForValue(double value)
        {
            return value < 0 ? new(Colors.IndianRed) : new(Colors.ForestGreen);
        }
    }
}
