using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Nova.Converters
{
    /// <summary>
    /// Converter used in a binding so we can pass a parameter to substract from the value first.
    /// </summary>
    [ValueConversion(typeof(double), typeof(double))]
    public class SubtractionConverter : IValueConverter
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
            return ConvertOperation(value, parameter, (x, y) => x - y);
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
            return ConvertOperation(value, parameter, (x, y) => x + y);
        }

        /// <summary>
        /// Calculates the appropriate value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="operation">The operation.</param>
        /// <returns></returns>
        private static object ConvertOperation(object value, object parameter, Func<double, double, double> operation)
        {
            double doubleValue;
            double doubleParameter;

            if (double.TryParse(System.Convert.ToString(value), out doubleValue) &&
                double.TryParse(System.Convert.ToString(parameter), out doubleParameter))
            {
                var calculatedValue = operation(doubleValue, doubleParameter);
                if (calculatedValue > 0)
                {
                    return calculatedValue;
                }
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
