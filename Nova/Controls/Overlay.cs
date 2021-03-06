﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Nova.Controls
{
    /// <summary>
    /// Interaction logic for Overlay
    /// </summary>
    public class Overlay : ContentControl
    {
        private DateTime? _lastShown;
        private DispatcherTimer _fadeInTimer;
        private DispatcherTimer _fadeOutTimer;

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
            overlay._lastShown = null;

            if (StopFadeOutTimer(overlay))
                return;


            var delay = overlay.Delay;

            if (delay <= 0)
            {
                overlay.IsActive = true;

                CreateStoryboard(overlay, 0, 1).Begin();
                return;
            }

            overlay._fadeInTimer = new DispatcherTimer(DispatcherPriority.Normal, overlay.Dispatcher)
            {
                Interval = TimeSpan.FromMilliseconds(delay)
            };
            
            overlay._fadeInTimer.Tick += (sender, args) =>
            {
                overlay.IsActive = true;

                CreateStoryboard(overlay, 0, 1).Begin();
                overlay._lastShown = DateTime.Now;

                StopFadeInTimer(overlay);
            };

            overlay._fadeInTimer.Start();
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
            
            if (!overlay._lastShown.HasValue)
            {
                storyboard.Begin();
                return;
            }
            
            var elapsedMilliseconds = (DateTime.Now - overlay._lastShown.Value).Duration().TotalMilliseconds;
            var minimumDuration = overlay.MinimumDuration;

            if (elapsedMilliseconds < minimumDuration)
            {
                var duration = minimumDuration - elapsedMilliseconds;

                overlay._fadeOutTimer = new DispatcherTimer(DispatcherPriority.Normal, overlay.Dispatcher)
                    {
                        Interval = TimeSpan.FromMilliseconds(duration)
                    };

                overlay._fadeOutTimer.Tick += (sender, args) =>
                    {
                        storyboard.Begin();
                        StopFadeOutTimer(overlay);
                    };

                overlay._fadeOutTimer.Start();
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
            if (overlay._fadeInTimer == null) return false;

            overlay._fadeInTimer.Stop();
            overlay._fadeInTimer = null;

            return true;
        }

        private static bool StopFadeOutTimer(Overlay overlay)
        {
            if (overlay._fadeOutTimer == null) return false;

            overlay._fadeOutTimer.Stop();
            overlay._fadeOutTimer = null;

            return true;
        }
    }
}
