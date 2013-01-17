#region License

// 
//  Copyright 2013 Steven Thuriot
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
using System.Windows.Media;
using Nova.Helpers;

namespace Nova.Shell
{
    /// <summary>
    /// Interaction logic for DropShadow.xaml
    /// </summary>
    public partial class DropShadow
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="DropShadow" /> class from being created.
        /// </summary>
        /// <param name="window">The window.</param>
        private DropShadow(Window window)
        {
            DataContext = window;
                        
            InitializeComponent();

            window.Loaded += OnLoaded;
            Left = window.Left + 1;
            Top = window.Top + 1;
        }

        /// <summary>
        /// Drops a shadow behind the passed window.
        /// </summary>
        /// <param name="window">The window.</param>
        public static void ForWindow(Window window)
        {
            bool enableShadow;

            switch (RenderCapability.Tier >> 16)
            {
                // hardware rendering
                case 2:
                    enableShadow = true;
                    break;

                // partially hardware rendering
// ReSharper disable RedundantCaseLabel
                case 1:
// ReSharper restore RedundantCaseLabel
                // software rendering
                default:
                    enableShadow = false;
                    break;
            }

            if (enableShadow)
                new DropShadow(window);
        }

        private void OnClosed(object sender, EventArgs eventArgs)
        {
            Close();
        }


        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            SetVisibilityDependingOn(sender);
        }

        private void OnStateChanged(object sender, EventArgs eventArgs)
        {
            SetVisibilityDependingOn(sender);
        }

        private void SetVisibilityDependingOn(object sender)
        {
            var window = sender as Window;

            if (window == null) return;

            if (window.WindowState == WindowState.Normal)
            {
                Dispatcher.DelayInvoke(TimeSpan.FromMilliseconds(400), new Action(() =>
                {
                    Visibility = Visibility.Visible;
                    OnLocationChanged(sender, null);
                }));
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var window = sender as Window;

            if (window == null) return;

            window.Loaded -= OnLoaded;
            window.Closed += OnClosed;

            Show();
            window.Owner = this;

            window.Focus();
            Dispatcher.DelayInvoke(TimeSpan.FromMilliseconds(250), new Action<Window>(Initialize), window);
        }

        private void Initialize(Window window)
        {
            Visibility = Visibility.Visible;

            Left = window.Left - 5;
            Top = window.Top - 5;

            Width = window.Width + 10;
            Height = window.Height + 10;

            window.LocationChanged += OnLocationChanged;
            window.SizeChanged += OnSizeChanged;
            window.StateChanged += OnStateChanged;
            window.IsVisibleChanged += OnIsVisibleChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            if (Visibility != Visibility.Visible) return;

            var window = sender as Window;

            if (window == null) return;

            Width = window.Width + 10;
            Height = window.Height + 10;
        }

        private void OnLocationChanged(object sender, EventArgs eventArgs)
        {
            if (Visibility != Visibility.Visible) return;

            var window = sender as Window;

            if (window == null) return;

            Left = window.Left - 5;
            Top = window.Top - 5;
        }
    }
}