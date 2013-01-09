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
using System;
using System.Windows;
using System.Windows.Documents;

namespace Nova.Base
{
	/// <summary>
	/// A subclass of the Run element that exposes a DependencyProperty property
	/// to allow data binding.
	/// </summary>
	public class BindableRun : Run
	{
		/// <summary>
		/// The bound text.
		/// </summary>
		public static readonly DependencyProperty BoundTextProperty = DependencyProperty.Register("BoundText", typeof(string), typeof(BindableRun), new PropertyMetadata(OnBoundTextChanged));

		/// <summary>
		/// Called when [bound text changed].
		/// </summary>
		/// <param name="d">The Dependency Object.</param>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void OnBoundTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((Run)d).Text = (string)e.NewValue;
		}

		/// <summary>
		/// Gets or sets the bound text.
		/// </summary>
		/// <value>
		/// The bound text.
		/// </value>
		public String BoundText
		{
			get { return (string)GetValue(BoundTextProperty); }
			set { SetValue(BoundTextProperty, value); }
		}
	}
}
