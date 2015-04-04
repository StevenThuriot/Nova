using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace Nova.Behaviors
{
    /// <summary>
    /// Allows two-way binding a TreeView's selected item.
    /// </summary>
    public class BindableSelectedItemBehavior : Behavior<TreeView>
    {
        /// <summary>
        /// Selected Item DP
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem",
                                        typeof(object),
                                        typeof(BindableSelectedItemBehavior),
                                        new UIPropertyMetadata(null, OnSelectedItemChanged));

        /// <summary>
        /// The selected item
        /// </summary>
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }

            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// 
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            var treeView = AssociatedObject;

            if (treeView == null) return;

            treeView.SelectedItemChanged += OnTreeViewSelectedItemChanged;
            treeView.Loaded += OnTreeViewOnLoaded;
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// 
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            var treeView = AssociatedObject;

            if (treeView == null) return;

            treeView.SelectedItemChanged -= OnTreeViewSelectedItemChanged;
            treeView.Loaded -= OnTreeViewOnLoaded;
        }

        private static TreeViewItem GetTreeViewItem(ItemsControl container, object item)
        {
            if (container == null) return null;

            if (container.DataContext == item)
                return container as TreeViewItem;

            // Expand the current container
            var treeViewItem = container as TreeViewItem;

            if (treeViewItem != null && !treeViewItem.IsExpanded)
                treeViewItem.SetValue(TreeViewItem.IsExpandedProperty, true);

            // Try to generate the ItemsPresenter and the ItemsPanel.
            // by calling ApplyTemplate.  Note that in the 
            // virtualizing case even if the item is marked 
            // expanded we still need to do this step in order to 
            // regenerate the visuals because they may have been virtualized away.
            container.ApplyTemplate();

            var itemsPresenter = (ItemsPresenter)container.Template.FindName("ItemsHost", container);

            if (itemsPresenter != null)
            {
                itemsPresenter.ApplyTemplate();
            }
            else
            {
                // The Tree template has not named the ItemsPresenter, 
                // so walk the descendents and find the child.
                itemsPresenter = GetVisualDescendent<ItemsPresenter>(container);
                if (itemsPresenter == null)
                {
                    container.UpdateLayout();
                    itemsPresenter = GetVisualDescendent<ItemsPresenter>(container);
                }
            }

            var itemsHostPanel = (Panel)VisualTreeHelper.GetChild(itemsPresenter, 0);

#pragma warning disable 168
            // ReSharper disable once UnusedVariable
            // Ensure that the generator for this panel has been created.
            UIElementCollection children = itemsHostPanel.Children;
#pragma warning restore 168

            for (int i = 0, count = container.Items.Count; i < count; i++)
            {
                TreeViewItem subContainer;

                var panel = itemsHostPanel as VirtualizingStackPanel;
                if (panel != null)
                //if (bringIndexIntoView != null)
                {
                    // Bring the item into view so 
                    // that the container will be generated.
                    panel.BringIndexIntoViewPublic(i);
                    subContainer = (TreeViewItem)container.ItemContainerGenerator.ContainerFromIndex(i);
                }
                else
                {
                    subContainer = (TreeViewItem)container.ItemContainerGenerator.ContainerFromIndex(i);
                }

                if (subContainer == null)
                    continue;

                var expanded = subContainer.IsExpanded;

                // Search the next level for the object.
                TreeViewItem resultContainer = GetTreeViewItem(subContainer, item);

                if (resultContainer != null)
                    return resultContainer;

                // The object is not under this TreeViewItem
                // so collapse it if it was collapsed before.
                subContainer.IsExpanded = expanded;
            }

            return null;
        }

        private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var behavior = (BindableSelectedItemBehavior)sender;

            var item = e.NewValue as TreeViewItem;
            if (item != null)
            {
                item.SetValue(TreeViewItem.IsSelectedProperty, true);
            }
            else
            {
                var treeView = behavior.AssociatedObject;
                ScrollTo(treeView, e.NewValue);
            }
        }

        private static void ScrollTo(ItemsControl treeView, object value)
        {
            if (treeView == null) return;// at designtime the AssociatedObject sometimes seems to be null

            var item = GetTreeViewItem(treeView, value);

            if (item == null) return;

            item.IsSelected = true;

            item.BringIntoView();
        }

        private void OnTreeViewOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var treeview = (TreeView)sender;
            var selectedItem = SelectedItem;

            if (selectedItem == null) return;

            ScrollTo(treeview, selectedItem);
        }

        private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedItem = e.NewValue;
        }






        static T GetVisualDescendent<T>(DependencyObject d) where T : DependencyObject
        {
            return GetVisualDescendents<T>(d).FirstOrDefault();
        }

        static IEnumerable<T> GetVisualDescendents<T>(DependencyObject d) where T : DependencyObject
        {
            var childCount = VisualTreeHelper.GetChildrenCount(d);

            for (var n = 0; n < childCount; n++)
            {
                var child = VisualTreeHelper.GetChild(d, n);

                if (child is T)
                {
                    yield return (T)child;
                }

                foreach (var match in GetVisualDescendents<T>(child))
                {
                    yield return match;
                }
            }
        }
    }
}
