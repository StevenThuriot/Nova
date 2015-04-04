using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Nova.Controls
{
    /// <summary>
    /// Interaction logic for ClosableTabItem.xaml
    /// </summary>
    [TemplatePart(Name = "PART_CloseTab", Type = typeof(Button))]
    public class ClosableTabItem : TabItem
    {
        /// <summary>
        /// Initializes the <see cref="ClosableTabItem" /> class.
        /// </summary>
        static ClosableTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ClosableTabItem), new FrameworkPropertyMetadata(typeof(ClosableTabItem)));
        }

        /// <summary>
        /// The close button
        /// </summary>
        private Button _closeButton;

        /// <summary>
        /// The close tab event
        /// </summary>
        public static readonly RoutedEvent CloseTabEvent = EventManager.RegisterRoutedEvent("CloseTab", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(ClosableTabItem));

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
            RenderTransform = new ScaleTransform();
            AddHandler(CloseTabEvent, new RoutedEventHandler(CloseCurrentTabItem));

            PreviewMouseUp += OnMiddleClick;
            Loaded += TabItemLoaded;
        }

        /// <summary>
        /// Triggers when the tab item loads.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void TabItemLoaded(object sender, RoutedEventArgs e)
        {
            var storyBoard = new Storyboard();
            var animation = new DoubleAnimation(0.65d, 1d,  TimeSpan.FromMilliseconds(70));

            Storyboard.SetTarget(animation, this);
            Storyboard.SetTargetProperty(animation, new PropertyPath("RenderTransform.(ScaleTransform.ScaleX)"));

            storyBoard.Children.Add(animation);

            storyBoard.Begin();
        }


        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_closeButton != null)
            {
                _closeButton.Click -= RaiseCloseEvent;
            }

            _closeButton = (Button) GetTemplateChild("PART_CloseTab");

            if (_closeButton != null)
            {
                _closeButton.Click += RaiseCloseEvent;
            }
        }

        /// <summary>
        /// Called when middle clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs" /> instance containing the event data.</param>
        private void OnMiddleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Middle || e.ButtonState != MouseButtonState.Released) return;

            RaiseCloseEvent(sender, e);
        }

        /// <summary>
        /// Raises the close event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void RaiseCloseEvent(object sender, RoutedEventArgs e)
        {
            if (e.Handled) return;

            e.Handled = true;
            var routedEventArgs = new RoutedEventArgs(CloseTabEvent, DataContext);
            RaiseEvent(routedEventArgs);
        }

        /// <summary>
        /// Closes the current tab item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private static void CloseCurrentTabItem(object sender, RoutedEventArgs e)
        {
            if (e.Handled) return; //Parent has already handled it.

            var tabItem = sender as TabItem;
            if (tabItem == null) return;

            var tabControl = tabItem.Parent as TabControl;
            if (tabControl != null)
            {
                var items = tabControl.Items as IEditableCollectionView;
                if (items != null && items.CanRemove)
                {
                    items.Remove(tabItem);
                    return;
                }
            }

            if (tabControl == null)
            {
                DependencyObject previous = tabItem;
                for (int i = 0; i < 5; i++)
                {
                    previous = VisualTreeHelper.GetParent(previous);
                    tabControl = previous as TabControl;
                    if (tabControl != null) break;
                }
            }

            if (tabControl != null)
            {
                var items = tabControl.ItemsSource as IList;
                if (items != null) //If null, the bound source is one that doesn't support removing items.
                {
                    items.Remove(tabItem.DataContext);
                }
            }
        }
    }
}
