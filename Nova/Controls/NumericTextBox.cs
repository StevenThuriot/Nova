#region License

// 
//  Copyright 2013 Steven Thuriot
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 

#endregion

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
        private Regex _CustomValidNumberRegex;

        /// <summary>
        /// The valid number regex
        /// </summary>
        private static readonly Regex ValidNumberRegex;

        /// <summary>
        /// The numeric settings property
        /// </summary>
        public static readonly DependencyProperty NumericSettingsProperty;

        /// <summary>
        /// Gets or sets the numeric settings.
        /// </summary>
        /// <value>
        /// The numeric settings.
        /// </value>
        public NumericSettings NumericSettings
        {
            get { return (NumericSettings)GetValue(NumericSettingsProperty); }
            set { SetValue(NumericSettingsProperty, value); }
        }

        /// <summary>
        /// Initializes the <see cref="NumericTextBox" /> class.
        /// </summary>
        static NumericTextBox()
        {
            ValidNumberRegex = new Regex(@"^[-+]?[0-9]*([\.,][0-9]*)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant);

            var frameworkPropertyMetadata = new FrameworkPropertyMetadata
                {
                    CoerceValueCallback = CoerceTextValueCallback
                };

            TextProperty.AddOwner(typeof(NumericTextBox), frameworkPropertyMetadata);
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericTextBox), new FrameworkPropertyMetadata(typeof(NumericTextBox)));
            NumericSettingsProperty = DependencyProperty.Register("NumericSettings", typeof(NumericSettings), typeof(NumericTextBox), new PropertyMetadata(NumericSettingsChanged));
        }

        /// <summary>
        /// Coerces the text value callback.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns></returns>
        private static object CoerceTextValueCallback(DependencyObject dependencyObject, object baseValue)
        {
            var numericTextBox = (NumericTextBox) dependencyObject;
            
            var regex = numericTextBox._CustomValidNumberRegex ?? ValidNumberRegex;

            var value = (string) baseValue;

            if (regex.IsMatch(value))
                return value;

            var text = ((NumericTextBox) dependencyObject).Text;

            if (regex.IsMatch(text))
                return text;
            
            return string.Empty;
        }

        /// <summary>
        /// Triggers when the numeric settings change.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void NumericSettingsChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
		    var settings = e.NewValue as NumericSettings;
            
            var numericTextBox = (NumericTextBox) dependencyObject;

			if (settings == null)
			{
				numericTextBox._CustomValidNumberRegex = null;
				return;
			}
            
            var regexString = @"^[-+]?[0-9]{0," + settings.MaximumNumbers + "}";

			if (settings.MaximumDecimals > 0)
			{
				regexString += @"([\.,][0-9]{0," + settings.MaximumDecimals + "})?";
			}

			regexString += "$";

            numericTextBox._CustomValidNumberRegex = new Regex(regexString, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant);
        }
    }
}