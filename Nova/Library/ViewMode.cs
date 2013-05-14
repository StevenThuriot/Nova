#region License
// 
//  Copyright 2012 Steven Thuriot
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

using System.Windows;
using System.Globalization;

namespace Nova.Library
{
	/// <summary>
	/// ViewMode class, holding attached properties for added view functionality.
	/// </summary>
    public class ViewMode : DependencyObject
    {
        #region IsReadOnly

		/// <summary>
		/// Whether or not the dependency object is read only.
		/// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.RegisterAttached("IsReadOnly", typeof(bool), typeof(ViewMode), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

		/// <summary>
		/// Gets whether or not the dependency object is read only.
		/// </summary>
		/// <param name="dependencyObject">The dependencyObject.</param>
		/// <returns></returns>
        public static bool GetIsReadOnly(DependencyObject dependencyObject)
        {
            return (bool)dependencyObject.GetValue(IsReadOnlyProperty);
        }

		/// <summary>
		/// Sets whether or not the dependency object is read only.
		/// </summary>
		/// <param name="dependencyObject">The dependency object.</param>
		/// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetIsReadOnly(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(IsReadOnlyProperty, value);
        }

        #endregion IsReadOnly

        #region Watermark

		/// <summary>
		/// A control's watermark.
		/// </summary>
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.RegisterAttached("Watermark", typeof(string), typeof(ViewMode), new PropertyMetadata(string.Empty, WatermarkTextChanged));

		/// <summary>
		/// The text for the watermark changed.
		/// </summary>
		/// <param name="dependencyObject">The dependencyObject.</param>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void WatermarkTextChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var control = dependencyObject as FrameworkElement;
            if (control == null)
                return;

		    var watermark = e.NewValue as string;

            if (string.IsNullOrWhiteSpace(watermark))
                return;

            var style = string.Format(CultureInfo.CurrentCulture, "Watermark{0}Style", dependencyObject.GetType().Name);

            if (Application.Current.Resources.Contains(style))
            {
                control.SetResourceReference(FrameworkElement.StyleProperty, style);
            }
        }

		/// <summary>
		/// Gets the watermark.
		/// </summary>
		/// <param name="dependencyObject">The dependency object.</param>
		/// <returns></returns>
        public static string GetWatermark(DependencyObject dependencyObject)
        {
            return (string)dependencyObject.GetValue(WatermarkProperty);
        }

		/// <summary>
		/// Sets the watermark.
		/// </summary>
		/// <param name="dependencyObject">The dependency object.</param>
		/// <param name="value">The value.</param>
        public static void SetWatermark(DependencyObject dependencyObject, string value)
        {
            dependencyObject.SetValue(WatermarkProperty, value);
        }

        #endregion Watermark
    }
}