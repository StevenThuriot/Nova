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
using System.Windows.Interop;

namespace Nova.Controls
{
	/// <summary>
	/// Interaction logic for MinMaxClose.xaml
	/// </summary>
	public partial class WindowControlBox
	{
		/// <summary>
		/// Wheter or not the maximize button is enabled.
		/// </summary>
		public static readonly DependencyProperty CanMaximizeProperty =
			DependencyProperty.Register("CanMaximize", typeof (bool), typeof (WindowControlBox), new PropertyMetadata(true));

		/// <summary>
		/// Whether or not the cancel button triggers a close event.
		/// </summary>
		public static readonly DependencyProperty IsCancelProperty =
			DependencyProperty.Register("IsCancel", typeof(bool), typeof(WindowControlBox), new PropertyMetadata(false));

		private Window _Parent;

		/// <summary>
		/// Initializes a new instance of the <see cref="WindowControlBox"/> class.
		/// </summary>
		/// <param name="parent">The parent.</param>
		public WindowControlBox(Window parent)
		{
			_Parent = parent;
			InitializeComponent();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WindowControlBox"/> class.
		/// </summary>
		public WindowControlBox()
		{
			Loaded += ControlLoaded;
			InitializeComponent();
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance can maximize.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance can maximize; otherwise, <c>false</c>.
		/// </value>
		public bool CanMaximize
		{
			get { return (bool) GetValue(CanMaximizeProperty); }
			set { SetValue(CanMaximizeProperty, value); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance triggers the close event on cancel.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance triggers the close event on cancel; otherwise, <c>false</c>.
		/// </value>
		public bool IsCancel
		{
			get { return (bool)GetValue(IsCancelProperty); }
			set { SetValue(IsCancelProperty, value); }
		}

		/// <summary>
		/// Triggered when the control finished loading.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="routedEventArgs">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void ControlLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			Loaded -= ControlLoaded;
			_Parent = Window.GetWindow(this);
		}

		/// <summary>
		/// Minimizes the window.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void MinimizeWindow(object sender, RoutedEventArgs e)
		{
			_Parent.WindowState = WindowState.Minimized;
		}

		/// <summary>
		/// Maximizes the window.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void MaximizeWindow(object sender, RoutedEventArgs e)
		{
			_Parent.WindowState = _Parent.WindowState == WindowState.Maximized
			                      	? WindowState.Normal
			                      	: WindowState.Maximized;
		}

		/// <summary>
		/// Closes the window.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void CloseWindow(object sender, RoutedEventArgs e)
		{
            if (ComponentDispatcher.IsThreadModal)
            {
                if (!IsCancel)
                {
                    _Parent.DialogResult = false;
                }
            }
            else
            {
                _Parent.Close();
            }
		}
	}
}