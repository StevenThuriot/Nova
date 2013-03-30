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
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Nova.Controls
{
    /// <summary>
    /// Interaction logic for Overlay.xaml
    /// </summary>
    public class Overlay : UserControl
    {
        private DateTime? _LastShown;

        private DispatcherTimer _FadeInTimer;
        private DispatcherTimer _FadeOutTimer;

        /// <summary>
        /// Initializes the <see cref="Overlay" /> class.
        /// </summary>
        static Overlay()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Overlay), new FrameworkPropertyMetadata(typeof(Overlay)));
        }


        /// <summary>
        /// The overlay brush property
        /// </summary>
        public static readonly DependencyProperty OverlayBrushProperty =
            DependencyProperty.Register("OverlayBrush", typeof (Brush), typeof (Overlay), new PropertyMetadata(Brushes.Black));

        /// <summary>
        /// The is loading property
        /// </summary>
        public static readonly DependencyProperty IsLoadingProperty = 
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(Overlay), new PropertyMetadata(false, IsLoadingChanged));

        /// <summary>
        /// The animation speed property, set in milliseconds.
        /// </summary>
        public static readonly DependencyProperty AnimationSpeedProperty =
            DependencyProperty.Register("AnimationSpeed", typeof(double), typeof(Overlay), new PropertyMetadata(300d));

        /// <summary>
        /// The minimum duration property
        /// </summary>
        public static readonly DependencyProperty MinimumDurationProperty =
            DependencyProperty.Register("MinimumDuration", typeof(double), typeof(Overlay), new PropertyMetadata(1800d));

        /// <summary>
        /// The delay property
        /// </summary>
        public static readonly DependencyProperty DelayProperty =
            DependencyProperty.Register("Delay", typeof (double), typeof (Overlay), new PropertyMetadata(0d));

        /// <summary>
        /// The is active property
        /// </summary>
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof (bool), typeof (Overlay), new PropertyMetadata(false));

        




        /// <summary>
        /// Gets or sets the overlay brush.
        /// </summary>
        /// <value>
        /// The overlay brush.
        /// </value>
        public Brush OverlayBrush
        {
            get { return (Brush)GetValue(OverlayBrushProperty); }
            set { SetValue(OverlayBrushProperty, value); }
        }

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

        /// <summary>
        /// Gets or sets the animation speed.
        /// </summary>
        /// <value>
        /// The animation speed.
        /// </value>
        /// <remarks>
        /// This is set in milliseconds.
        /// </remarks>
        public double AnimationSpeed
        {
            get { return (double) GetValue(AnimationSpeedProperty); }
            set { SetValue(AnimationSpeedProperty, value); }
        }

        /// <summary>
        /// Gets or sets the minimum duration.
        /// </summary>
        /// <value>
        /// The minimum duration.
        /// </value>
        /// <remarks>
        /// This is set in milliseconds.
        /// </remarks>
        public double MinimumDuration
        {
            get { return (double) GetValue(MinimumDurationProperty); }
            set { SetValue(MinimumDurationProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets the delay.
        /// </summary>
        /// <value>
        /// The delay.
        /// </value>
        /// <remarks>
        /// This is set in milliseconds.
        /// </remarks>
        public double Delay
        {
            get { return (double)GetValue(DelayProperty); }
            set { SetValue(DelayProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }









        /// <summary>
        /// The IsLoading property has changed.
        /// </summary>
        /// <param name="dependencyObject">The dependencyObject.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void IsLoadingChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var overlay = (Overlay) dependencyObject;
            var isLoading = (bool)e.NewValue;

            if (isLoading)
            {
                BeginOverlay(overlay);
            }
            else
            {
                StopOverlay(overlay);
            }
        }

        /// <summary>
        /// Begins enabling the overlay.
        /// </summary>
        /// <param name="overlay">The overlay.</param>
        private static void BeginOverlay(Overlay overlay)
        {
            overlay._LastShown = null;

            if (StopFadeOutTimer(overlay))
                return;


            var delay = overlay.Delay;

            if (delay <= 0)
            {
                overlay.IsActive = true;

                CreateStoryboard(overlay, 0, 1).Begin();
                return;
            }

            overlay._FadeInTimer = new DispatcherTimer(DispatcherPriority.Normal, overlay.Dispatcher)
            {
                Interval = TimeSpan.FromMilliseconds(delay)
            };
            
            overlay._FadeInTimer.Tick += (sender, args) =>
            {
                overlay.IsActive = true;

                CreateStoryboard(overlay, 0, 1).Begin();
                overlay._LastShown = DateTime.Now;

                StopFadeInTimer(overlay);
            };

            overlay._FadeInTimer.Start();
        }

        /// <summary>
        /// Stops the overlay.
        /// </summary>
        /// <param name="overlay">The overlay.</param>
        private static void StopOverlay(Overlay overlay)
        {
            if (StopFadeInTimer(overlay))
                return;

            var storyboard = CreateStoryboard(overlay, 1, 0); 
            
            storyboard.Completed += (sender, args) => overlay.IsActive = false;
            
            if (!overlay._LastShown.HasValue)
            {
                storyboard.Begin();
                return;
            }
            
            var elapsedMilliseconds = (DateTime.Now - overlay._LastShown.Value).Duration().TotalMilliseconds;
            var minimumDuration = overlay.MinimumDuration;

            if (elapsedMilliseconds < minimumDuration)
            {
                var duration = minimumDuration - elapsedMilliseconds;

                overlay._FadeOutTimer = new DispatcherTimer(DispatcherPriority.Normal, overlay.Dispatcher)
                    {
                        Interval = TimeSpan.FromMilliseconds(duration)
                    };

                overlay._FadeOutTimer.Tick += (sender, args) =>
                    {
                        storyboard.Begin();
                        StopFadeOutTimer(overlay);
                    };

                overlay._FadeOutTimer.Start();
            }
            else
            {
                storyboard.Begin();
            }
        }

        private static Storyboard CreateStoryboard(Overlay overlay, double @from, double to)
        {
            var storyboard = new Storyboard();
            var doubleAnimation = new DoubleAnimation
                {
                    From = @from,
                    To = to,
                    Duration = TimeSpan.FromMilliseconds(overlay.AnimationSpeed)
                };

            storyboard.Children.Add(doubleAnimation);

            Storyboard.SetTarget(doubleAnimation, overlay);
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("Opacity"));

            return storyboard;
        }

        private static bool StopFadeInTimer(Overlay overlay)
        {
            if (overlay._FadeInTimer == null) return false;

            overlay._FadeInTimer.Stop();
            overlay._FadeInTimer = null;

            return true;
        }

        private static bool StopFadeOutTimer(Overlay overlay)
        {
            if (overlay._FadeOutTimer == null) return false;

            overlay._FadeOutTimer.Stop();
            overlay._FadeOutTimer = null;

            return true;
        }
    }
}
