using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Nova.Controls
{
    /// <summary>
    /// A textbox that accepts only numeric input.
    /// </summary>
    public class NumericTextBox : TextBox
    {
        /// <summary>
        /// The valid number regex
        /// </summary>
        private static readonly Regex ValidNumberRegex;

        /// <summary>
        /// Initializes the <see cref="NumericTextBox" /> class.
        /// </summary>
        static NumericTextBox()
        {
            ValidNumberRegex = new Regex(@"^[-+]?[0-9]*[\.,]?[0-9]*([eE][-+]?[0-9]+)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

            TextProperty.AddOwner(typeof(NumericTextBox), new FrameworkPropertyMetadata(delegate { }, CoerceTextValueCallback));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericTextBox), new FrameworkPropertyMetadata(typeof(NumericTextBox)));
        }

        /// <summary>
        /// Coerces the text value callback.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns></returns>
        private static object CoerceTextValueCallback(DependencyObject dependencyObject, object baseValue)
        {
            var value = (string) baseValue;
            return ValidNumberRegex.IsMatch(value)
                       ? value
                       : ((NumericTextBox) dependencyObject).Text;
        }
    }
}