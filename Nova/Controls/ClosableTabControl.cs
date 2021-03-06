﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Nova.Controls
{
    /// <summary>
    /// Interaction logic for ClosableTabControl.xaml
    /// </summary>
    public class ClosableTabControl : TabControl
    {
        /// <summary>
        /// Initializes the <see cref="ClosableTabControl" /> class.
        /// </summary>
        static ClosableTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ClosableTabControl), new FrameworkPropertyMetadata(typeof(ClosableTabControl)));
            SelectedItemProperty.AddOwner(typeof(ClosableTabControl), new FrameworkPropertyMetadata(SelectedContentChanged));
        }

        /// <summary>
        /// The selected content has changed.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void SelectedContentChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
        {
            var closableTabControl = control as ClosableTabControl;
            if (closableTabControl == null) return;

            closableTabControl.IsEmpty = closableTabControl.SelectedItem == null;
        }

        /// <summary>
        /// The is empty property
        /// </summary>
        public static readonly DependencyProperty IsEmptyProperty =
            DependencyProperty.Register("IsEmpty", typeof(bool), typeof(ClosableTabControl), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value indicating whether this instance is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty
        {
            get { return (bool)GetValue(IsEmptyProperty); }
            set { SetValue(IsEmptyProperty, value); }
        }

        /// <summary>
        /// Creates or identifies the element used to display the specified item.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Windows.Controls.TabItem" />.
        /// </returns>
        protected sealed override DependencyObject GetContainerForItemOverride()
        {
            return new ClosableTabItem();
        }

        /// <summary>
        /// The add item command property
        /// </summary>
        public static readonly DependencyProperty AddItemCommandProperty = DependencyProperty.Register("AddItemCommand", typeof(ICommand), typeof(ClosableTabControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the add item command.
        /// </summary>
        /// <value>
        /// The add item command.
        /// </value>
        public ICommand AddItemCommand
        {
            get { return (ICommand)GetValue(AddItemCommandProperty); }
            set { SetValue(AddItemCommandProperty, value); }
        }
    }
}
