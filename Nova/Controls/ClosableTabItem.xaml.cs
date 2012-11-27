using System.Windows;
using System.Windows.Controls;

namespace Nova.Controls
{
    /// <summary>
    /// Interaction logic for ClosableTabItem.xaml
    /// </summary>
    public partial class ClosableTabItem : TabItem
    {
        /// <summary>
        /// The close button
        /// </summary>
        private Button _CloseButton;

        /// <summary>
        /// The close tab event
        /// </summary>
        public static readonly RoutedEvent CloseTabEvent = EventManager.RegisterRoutedEvent("CloseTab", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ClosableTabItem));

        /// <summary>
        /// Occurs when a tab item needs to be closed.
        /// </summary>
        public event RoutedEventHandler CloseTab
        {
            add { AddHandler(CloseTabEvent, value); }
            remove { RemoveHandler(CloseTabEvent, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClosableTabItem" /> class.
        /// </summary>
        public ClosableTabItem()
        {
            InitializeComponent();
            AddHandler(CloseTabEvent, new RoutedEventHandler(CloseCurrentTabItem));
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_CloseButton != null)
            {
                _CloseButton.Click -= RaiseCloseEvent;
            }

            _CloseButton = GetTemplateChild("PART_CloseTab") as Button;
            if (_CloseButton != null)
            {
                _CloseButton.Click += RaiseCloseEvent;
            }
        }

        /// <summary>
        /// Raises the close event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void RaiseCloseEvent(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            RaiseEvent(new RoutedEventArgs(CloseTabEvent, this));
        }

        /// <summary>
        /// Closes the current tab item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private static void CloseCurrentTabItem(object sender, RoutedEventArgs e)
        {
            var tabItem = sender as TabItem;
            if (tabItem == null) return;

            var tabControl = tabItem.Parent as TabControl;
            if (tabControl == null) return;

            tabControl.Items.Remove(tabItem);
            e.Handled = true;
        }
    }
}
