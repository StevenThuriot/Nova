﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Nova.Controls
{
    /// <summary>
    ///     Class to show an animated loading image.
    /// </summary>
    public class AnimatedLoader : Image
    {
        /// <summary>
        ///     The frame index.
        /// </summary>
        public static readonly DependencyProperty FrameIndexProperty =
            DependencyProperty.Register("FrameIndex", typeof (int), typeof (AnimatedLoader), new PropertyMetadata(0, ChangingFrameIndex));

        private readonly Int32Animation _animation;
        private readonly GifBitmapDecoder _gifBitmapDecoder;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AnimatedLoader" /> class.
        /// </summary>
        public AnimatedLoader()
        {
            Visibility = Visibility.Hidden;
            IsVisibleChanged += LoaderVisibilityChanged;

            var imageSource = Application.Current.Resources["LoaderImage"] as ImageSource;
            var loaderUri = imageSource == null
                                ? new Uri("pack://application:,,,/Nova;component/Resources/loader.gif", UriKind.Absolute)
                                : new Uri(imageSource.ToString(), UriKind.Absolute);

            _gifBitmapDecoder = new GifBitmapDecoder(loaderUri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);

            var seconds = _gifBitmapDecoder.Frames.Count/10;
            var milliseconds = (int) (((_gifBitmapDecoder.Frames.Count/10.0) - (_gifBitmapDecoder.Frames.Count/10.0))*1000d);

            var timespan = new TimeSpan(0, 0, 0, seconds, milliseconds);
            var duration = new Duration(timespan);

            _animation = new Int32Animation(0, _gifBitmapDecoder.Frames.Count - 1, duration)
                {
                    RepeatBehavior = RepeatBehavior.Forever
                };

            Source = _gifBitmapDecoder.Frames[0];
            Height = Source.Height;
            Width = Source.Width;
        }

        /// <summary>
        ///     Gets or sets the index of the frame.
        /// </summary>
        /// <value>
        ///     The index of the frame.
        /// </value>
        public int FrameIndex
        {
            get { return (int) GetValue(FrameIndexProperty); }
            set { SetValue(FrameIndexProperty, value); }
        }

        /// <summary>
        ///     Caleed when the frameindex changed.
        /// </summary>
        /// <param name="dependencyObject">The dependencyObject.</param>
        /// <param name="eventArguments">
        ///     The <see cref="System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event data.
        /// </param>
        private static void ChangingFrameIndex(DependencyObject dependencyObject,
                                               DependencyPropertyChangedEventArgs eventArguments)
        {
            var animatedLoader = dependencyObject as AnimatedLoader;
            if (animatedLoader == null) return;

            var index = (int) eventArguments.NewValue;
            animatedLoader.Source = animatedLoader._gifBitmapDecoder.Frames[index];
            animatedLoader.InvalidateVisual();
        }

        /// <summary>
        ///     Starts animating.
        /// </summary>
        public void Start()
        {
            Visibility = Visibility.Visible;
        }

        /// <summary>
        ///     Stops animating.
        /// </summary>
        public void Stop()
        {
            Visibility = Visibility.Hidden;
        }

        /// <summary>
        ///     Called when the loaders visibility has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="dependencyPropertyChangedEventArgs">
        ///     The <see cref="System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event data.
        /// </param>
        private void LoaderVisibilityChanged(object sender,
                                             DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var isVisible = Visibility == Visibility.Visible;
            var animation = isVisible ? _animation : null;

            BeginAnimation(FrameIndexProperty, animation);
        }
    }
}