using System;
using System.Windows.Media;

namespace FreakinStocksUI.ViewModels
{
    /// <summary>
    /// Record allowing to get a <see cref="SolidColorBrush"/> appropriately to the provided value
    /// </summary>
    public record ValueChange
    {
        /// <summary>
        /// The <see cref="string"/> representation of the provided percent of change
        /// </summary>
        public string Change { get; init; }

        /// <summary>
        /// The color set appropriately to the provided percent of change
        /// </summary>
        public SolidColorBrush Color { get; init; }

        /// <summary>
        /// Constructs a record with the color set appropriately to the provided percent of value change
        /// </summary>
        /// <param name="change">The value change to be represented with an appropriate color</param>
        public ValueChange(double change)
        {
            var percent = Math.Round(change, 2);
            Change = $"{ GetSign(percent) }{ percent }%";
            Color = GetColorForValue(change);
        }

        /// <summary>
        /// Gets a <see cref="SolidColorBrush"/> with a color appropriate to the provided value change
        /// </summary>
        /// <param name="value">The value change to be represented with a color</param>
        /// <returns><see cref="SolidColorBrush"/> which color is set appropriately to the provided value change</returns>
        public static SolidColorBrush GetColorForValue(double value)
        {
            return value < 0 ? new(Colors.IndianRed) : new(Colors.ForestGreen);
        }

        /// <summary>
        /// Gets the sign for the provided percent of value change
        /// </summary>
        /// <param name="percent">The percent of value change</param>
        /// <returns>+ if the percent is positive, otherwise a new <see cref="char"/></returns>
        public static char GetSign(double percent)
        {
            return percent > 0 ? '+' : new();
        }
    }
}
