using System.Windows.Media;

namespace FreakinStocksUI.ViewModels
{
    public record ValueChange
    {
        public string Change { get; init; }

        public SolidColorBrush Color { get; init; }

        public ValueChange(string change, SolidColorBrush color)
        {
            Change = change;
            Color = color;
        }

        public static SolidColorBrush GetColorForValue(double value)
        {
            return value < 0 ? new(Colors.IndianRed) : new(Colors.ForestGreen);
        }
    }
}
