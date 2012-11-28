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
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Nova.Controls
{
    /// <summary>
    /// Interaction logic for ClosableTabItem.xaml
    /// </summary>
    public partial class ClosableTabItem : TabItem
    {        /// <summary>
        /// Initializes a new instance of the <see cref="ClosableTabItem" /> class.
        /// </summary>
        public ClosableTabItem()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Closes the current tab item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void CloseTabItem(object sender, RoutedEventArgs e)
        {
            TabControl tabControl = Parent as TabControl;
            if (tabControl != null)
            {
                var items = (IEditableCollectionView)tabControl.Items;
                if (items.CanRemove)
                {
                    items.Remove(this);
                    return;
                }
            }

            if (tabControl == null)
            {
                DependencyObject previous = this;
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
                    items.Remove(this.DataContext);
                }
            }
        }
    }
}
