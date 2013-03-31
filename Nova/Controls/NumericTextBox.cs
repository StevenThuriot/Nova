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

            var frameworkPropertyMetadata = new FrameworkPropertyMetadata
                {
                    CoerceValueCallback = CoerceTextValueCallback
                };

            TextProperty.AddOwner(typeof(NumericTextBox), frameworkPropertyMetadata);
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