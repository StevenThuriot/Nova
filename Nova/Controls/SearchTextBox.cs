﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Nova.Controls
{
    /// <summary>
    /// Interaction logic for SearchTextBox.xaml
    /// </summary>
    [TemplatePart(Name = "PART_ContentHost", Type = typeof(ScrollViewer))]
    [TemplatePart(Name = "PART_SearchButton", Type = typeof(Button))]
    public class SearchTextBox : TextBox
    {
        /// <summary>
        /// Initializes the <see cref="SearchTextBox" /> class.
        /// </summary>
        static SearchTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (SearchTextBox), new FrameworkPropertyMetadata(typeof (SearchTextBox)));
        }

        private ScrollViewer _contentHost;
        private Button _searchButton;


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
            DependencyProperty.Register("SearchWhileTyping", typeof(bool), typeof(SearchTextBox), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether to [search while typing].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [search while typing]; otherwise, <c>false</c>.
        /// </value>
        public bool SearchWhileTyping
        {
            get { return (bool)GetValue(SearchWhileTypingProperty); }
            set { SetValue(SearchWhileTypingProperty, value); }
        }

        /// <summary>
        /// The command to execute.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(SearchTextBox));

        /// <summary>
        /// Gets or sets the execution command.
        /// </summary>
        /// <value>
        /// The command.
        /// </value>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// The command that gets executed when a clear happens.
        /// </summary>
        public static readonly DependencyProperty ClearCommandProperty = DependencyProperty.Register("ClearCommand", typeof(ICommand), typeof(SearchTextBox));

        /// <summary>
        /// Gets or sets the clear command.
        /// </summary>
        /// <value>
        /// The clear command.
        /// </value>
        public ICommand ClearCommand
        {
            get { return (ICommand)GetValue(ClearCommandProperty); }
            set { SetValue(ClearCommandProperty, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchTextBox"/> class.
        /// </summary>
        public SearchTextBox()
        {
            TextChanged += SearchTextChanged;
        }

        /// <summary>
        /// Is called when a control template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_contentHost != null)
                _contentHost.KeyUp -= CheckTyping;

            _contentHost = (ScrollViewer)GetTemplateChild("PART_ContentHost");

            if (_contentHost != null)
                _contentHost.KeyUp += CheckTyping;

            if (_searchButton != null)
                _searchButton.Click -= HandleSearch;

            _searchButton = (Button)GetTemplateChild("PART_SearchButton");

            if (_searchButton != null)
                _searchButton.Click += HandleSearch;
        }

        /// <summary>
        /// Triggered when the search text changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.TextChangedEventArgs"/> instance containing the event data.</param>
        private void SearchTextChanged(object sender, TextChangedEventArgs e)
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
