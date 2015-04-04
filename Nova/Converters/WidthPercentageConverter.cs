using System;
using System.Globalization;
using System.Windows.Data;

namespace Nova.Converters
{
    /// <summary>
    /// Converter to calculate the width.
    /// </summary>
    [ValueConversion(typeof(double), typeof(double))]
    public class WidthPercentageConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var parameterString = System.Convert.ToString(parameter);
            var percentage = Double.Parse(parameterString, CultureInfo.InvariantCulture);
            var doubleValue = System.Convert.ToDouble(value);

            return doubleValue * percentage;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
