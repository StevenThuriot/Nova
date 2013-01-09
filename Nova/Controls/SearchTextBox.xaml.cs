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
using System.Windows.Input;

namespace Nova.Controls
{
	/// <summary>
	/// Interaction logic for SearchTextBox.xaml
	/// </summary>
	public partial class SearchTextBox
	{
		/// <summary>
		/// Wheter or not a search is in progress.
		/// </summary>
		public static readonly DependencyProperty IsSearchingProperty =
			DependencyProperty.Register("IsSearching", typeof(bool), typeof(SearchTextBox), new PropertyMetadata(false));

		/// <summary>
		/// Gets or sets a value indicating whether this instance is searching.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is searching; otherwise, <c>false</c>.
		/// </value>
		public bool IsSearching
		{
			get { return (bool)GetValue(IsSearchingProperty); }
			set { SetValue(IsSearchingProperty, value); }
		}

		/// <summary>
		/// Whether or not to search while typing.
		/// </summary>
		public static readonly DependencyProperty SearchWhileTypingProperty =
			DependencyProperty.Register("SearchWhileTyping", typeof (bool), typeof (SearchTextBox), new PropertyMetadata(false));

		/// <summary>
		/// Gets or sets a value indicating whether to [search while typing].
		/// </summary>
		/// <value>
		///   <c>true</c> if [search while typing]; otherwise, <c>false</c>.
		/// </value>
		public bool SearchWhileTyping
		{
			get { return (bool) GetValue(SearchWhileTypingProperty); }
			set { SetValue(SearchWhileTypingProperty, value); }
		}

		/// <summary>
		/// The command to execute.
		/// </summary>
		public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof (ICommand), typeof (SearchTextBox));

		/// <summary>
		/// Gets or sets the execution command.
		/// </summary>
		/// <value>
		/// The command.
		/// </value>
		public ICommand Command
		{
			get { return (ICommand) GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}

		/// <summary>
		/// The command that gets executed when a clear happens.
		/// </summary>
		public static readonly DependencyProperty ClearCommandProperty = DependencyProperty.Register("ClearCommand", typeof (ICommand), typeof (SearchTextBox));

		/// <summary>
		/// Gets or sets the clear command.
		/// </summary>
		/// <value>
		/// The clear command.
		/// </value>
		public ICommand ClearCommand
		{
			get { return (ICommand) GetValue(ClearCommandProperty); }
			set { SetValue(ClearCommandProperty, value); }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchTextBox"/> class.
		/// </summary>
		public SearchTextBox()
		{
			InitializeComponent();
			TextChanged += SearchTextChanged;
		}


		/// <summary>
		/// Triggered when the search text changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.Controls.TextChangedEventArgs"/> instance containing the event data.</param>
		private void SearchTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			if (string.IsNullOrEmpty(Text))
			{
				IsSearching = false;
			}
		}

		/// <summary>
		/// Searches.
		/// </summary>
		private void DoSearch()
		{
			var command = Command;
			if (command == null || !command.CanExecute(Text)) return;

			if (!string.IsNullOrEmpty(Text))
			{
				IsSearching = true;
			}

			command.Execute(Text);
		}

		/// <summary>
		/// Handles the search logic.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="routedEventArgs">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void HandleSearch(object sender, RoutedEventArgs routedEventArgs)
		{
			if (!IsSearching)
			{
				DoSearch();
			}
			else
			{
				Text = string.Empty;
				ClearSearchBox();
			}
		}

		/// <summary>
		/// Clears the search box.
		/// </summary>
		private void ClearSearchBox()
		{
			IsSearching = false;
			
			var command = ClearCommand;
			if (command == null || !command.CanExecute(Text)) return;

			command.Execute(Text);
		}

		/// <summary>
		/// Checks for enter.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
		private void CheckTyping(object sender, KeyEventArgs e)
		{
			if (SearchWhileTyping)
			{
				e.Handled = true;
				DoSearch();
			}
			else if (e.Key == Key.Enter)
			{
				e.Handled = true;
				DoSearch();
			}
			else if (Text.Length == 0)
			{
				e.Handled = true;
				ClearSearchBox();
			}
		}
	}
}
