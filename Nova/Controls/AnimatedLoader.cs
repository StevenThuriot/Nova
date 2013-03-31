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

using System;
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

        private readonly Int32Animation _Animation;
        private readonly GifBitmapDecoder _GifBitmapDecoder;

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

            _GifBitmapDecoder = new GifBitmapDecoder(loaderUri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);

            var seconds = _GifBitmapDecoder.Frames.Count/10;
            var milliseconds = (int) (((_GifBitmapDecoder.Frames.Count/10.0) - (_GifBitmapDecoder.Frames.Count/10.0))*1000d);

            var timespan = new TimeSpan(0, 0, 0, seconds, milliseconds);
            var duration = new Duration(timespan);

            _Animation = new Int32Animation(0, _GifBitmapDecoder.Frames.Count - 1, duration)
                {
                    RepeatBehavior = RepeatBehavior.Forever
                };

            Source = _GifBitmapDecoder.Frames[0];
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
            var ob = dependencyObject as AnimatedLoader;
            if (ob == null) return;

            ob.Source = ob._GifBitmapDecoder.Frames[(int) eventArguments.NewValue];
            ob.InvalidateVisual();
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
            var animation = isVisible ? _Animation : null;

            BeginAnimation(FrameIndexProperty, animation);
        }
    }
}