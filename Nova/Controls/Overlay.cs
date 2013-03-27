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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Nova.Controls
{
    /// <summary>
    /// Interaction logic for Overlay.xaml
    /// </summary>
    public class Overlay : Control
    {
        /// <summary>
        /// Initializes the <see cref="Overlay" /> class.
        /// </summary>
        static Overlay()
        {
            VisibilityProperty.OverrideMetadata(typeof(Overlay), new FrameworkPropertyMetadata(Visibility.Collapsed));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Overlay), new FrameworkPropertyMetadata(typeof(Overlay)));
            ForegroundProperty.OverrideMetadata(typeof(Overlay), new FrameworkPropertyMetadata(Brushes.WhiteSmoke));
        }

        /// <summary>
        /// The overlay brush property
        /// </summary>
        public static readonly DependencyProperty OverlayBrushProperty =
            DependencyProperty.Register("OverlayBrush", typeof (Brush), typeof (Overlay), new PropertyMetadata(Brushes.Black));

        /// <summary>
        /// Gets or sets the overlay brush.
        /// </summary>
        /// <value>
        /// The overlay brush.
        /// </value>
        public Brush OverlayBrush
        {
            get { return (Brush) GetValue(OverlayBrushProperty); }
            set { SetValue(OverlayBrushProperty, value); }
        }
        
        /// <summary>
        /// The is loading property
        /// </summary>
        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register("IsLoading", typeof (bool), typeof (Overlay), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether this instance is loading.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is loading; otherwise, <c>false</c>.
        /// </value>
        public bool IsLoading
        {
            get { return (bool) GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }
    }
}
