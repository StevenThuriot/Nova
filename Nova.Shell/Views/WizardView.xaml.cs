#region License
//  
// Copyright 2013 Steven Thuriot
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
using System.Windows.Controls.Primitives;

namespace Nova.Shell.Views
{
    /// <summary>
    /// Interaction logic for WizardView.xaml
    /// </summary>
    public partial class WizardView
    {
        //TODO: Listen to window resizing

        /// <summary>
        /// Initializes the <see cref="WizardView" /> class.
        /// </summary>
        static WizardView()
        {
            var widthMetadata = new FrameworkPropertyMetadata(640d, FrameworkPropertyMetadataOptions.AffectsMeasure)
                {
                    CoerceValueCallback = WidthCoerceValueCallback
                };
            
            var heightMetadata = new FrameworkPropertyMetadata(480d, FrameworkPropertyMetadataOptions.AffectsMeasure)
                {
                    CoerceValueCallback = HeightCoerceValueCallback
                };

            WidthProperty.AddOwner(typeof (WizardView), widthMetadata);
            HeightProperty.AddOwner(typeof(WizardView), heightMetadata);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WizardView"/> class.
        /// </summary>
        public WizardView()
        {
            Loaded += Initialize;
            InitializeComponent();
        }

        private static object HeightCoerceValueCallback(DependencyObject dependencyObject, object baseValue)
        {
            return CoerceSize(dependencyObject, baseValue, x => x.ActualHeight, Canvas.GetTop, x => x.MinHeight);
        }

        private static object WidthCoerceValueCallback(DependencyObject dependencyObject, object baseValue)
        {
            return CoerceSize(dependencyObject, baseValue, x => x.ActualWidth, Canvas.GetLeft, x => x.MinWidth);
        }

        private static object CoerceSize(DependencyObject dependencyObject, object baseValue, 
                                         Func<Canvas, double> getCanvasActualSize,
                                         Func<UIElement, double> getCanvasProperty,
                                         Func<FrameworkElement, double> getMinimumSize)
        {
            var wizard = (WizardView) dependencyObject;
            var size = (double) baseValue;
            var canvas = (Canvas) wizard.Parent;
            
            if (canvas != null)
            {
                var availableWidth = getCanvasActualSize(canvas) - getCanvasProperty(wizard) - 3;

                if (size > availableWidth)
                    return availableWidth;
            }

            var minimum = getMinimumSize(wizard);

            if (double.IsNaN(size) || double.IsInfinity(size) || size < minimum)
                return minimum;

            return size;
        }

        private void Initialize(object sender, RoutedEventArgs routedEventArgs)
        {
            Loaded -= Initialize;

            //Center!
            var canvas = (Canvas)Parent;

            var top = (canvas.ActualHeight - Height) / 2d;
            var left = (canvas.ActualWidth - Width) / 2d;

            Canvas.SetTop(this, top);
            Canvas.SetLeft(this, left);

            SubscribeDragEvents();
        }

        private void SubscribeDragEvents()
        {
            RightBorder.DragDelta += (view, args) => HorizontalChange(false, args);
            LeftBorder.DragDelta += (view, args) => HorizontalChange(true, args);

            TopBorder.DragDelta += (view, args) => VerticalChange(true, args);
            BottomBorder.DragDelta += (view, args) => VerticalChange(false, args);

            RightTopCorner.DragDelta += (view, args) =>
                {
                    HorizontalChange(false, args);
                    VerticalChange(true, args);
                };

            RightBottomCorner.DragDelta += (view, args) =>
                {
                    HorizontalChange(false, args);
                    VerticalChange(false, args);
                };

            LeftTopCorner.DragDelta += (view, args) =>
                {
                    HorizontalChange(true, args);
                    VerticalChange(true, args);
                };

            LeftBottomCorner.DragDelta += (view, args) =>
                {
                    HorizontalChange(true, args);
                    VerticalChange(false, args);
                };
        }
        
        private void HorizontalChange(bool draggingAffectsCanvas, DragDeltaEventArgs args)
        {
            Func<DragDeltaEventArgs, double> getDelta = x => x.HorizontalChange;
            Func<double> getCanvasPosition = () => Canvas.GetLeft(this);
            Action<double> setCanvasPosition = x => Canvas.SetLeft(this, x);
            Func<double> getMinimum = () => MinWidth;
            Func<double> getCurrent = () => Width;
            Action<double> addToCurrent = x => Width += x;

            ChangeSize(draggingAffectsCanvas, args, getDelta, getCanvasPosition, getCurrent, getMinimum, addToCurrent, setCanvasPosition);
        }

        private void VerticalChange(bool draggingAffectsCanvas, DragDeltaEventArgs args)
        {
            Func<DragDeltaEventArgs, double> getDelta = x => x.VerticalChange;
            Func<double> getCanvasPosition = () => Canvas.GetTop(this);
            Action<double> setCanvasPosition = x => Canvas.SetTop(this, x);
            Func<double> getMinimum = () => MinHeight;
            Func<double> getCurrent = () => Height;
            Action<double> addToCurrent = x => Height += x;

            ChangeSize(draggingAffectsCanvas, args, getDelta, getCanvasPosition, getCurrent, getMinimum, addToCurrent, setCanvasPosition);
        }

        private static void ChangeSize(bool draggingAffectsCanvas, DragDeltaEventArgs args, Func<DragDeltaEventArgs, double> getDelta, Func<double> getCanvasPosition, Func<double> getCurrent, Func<double> getMinimum, Action<double> addToCurrent, Action<double> setCanvasPosition)
        {
            var delta = getDelta(args);

            if (draggingAffectsCanvas)
            {
                delta *= -1;

                var position = getCanvasPosition() + getDelta(args);
                var size = getCurrent() + delta;

                if (position <= 3 || size < getMinimum())
                    return;

                setCanvasPosition(position);
            }

            addToCurrent(delta);
        }
    }
}
