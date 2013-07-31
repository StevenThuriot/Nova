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
using System.Windows.Input;

namespace Nova.Shell.Views
{
    /// <summary>
    /// Interaction logic for WizardView.xaml
    /// </summary>
    [TemplatePart(Name = "PART_ContentHost", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_RightBorder", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_LeftBorder", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_TopBorder", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_BottomBorder", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_RightTopBorder", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_RightBottomBorder", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_LeftTopBorder", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_LeftBottomBorder", Type = typeof(Thumb))]
    public partial class WizardView
    {
        //TODO: Listen to window resizing and restrict wizard if needed.
        
        private Point _clickPosition;
        private FrameworkElement _contentHost;

        private Thumb _rightBorder;
        private Thumb _leftBorder;
        private Thumb _topBorder;
        private Thumb _bottomBorder;
        private Thumb _rightTopBorder;
        private Thumb _rightBottomBorder;
        private Thumb _leftTopBorder;
        private Thumb _leftBottomBorder;

        /// <summary>
        /// Initializes the <see cref="WizardView" /> class.
        /// </summary>
        static WizardView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WizardView), new FrameworkPropertyMetadata(typeof(WizardView)));
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

        private void ContentMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            var element = (FrameworkElement)sender;
            _clickPosition = e.GetPosition(element);
            element.CaptureMouse();
        }

        private static void ContentMouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            var element = (FrameworkElement)sender;
            element.ReleaseMouseCapture();
        }

        private void ContentMouseMove(object sender, MouseEventArgs e)
        {
            var element = (FrameworkElement)sender;

            if (e.Handled || !element.IsMouseCaptureWithin)
                return;

            e.Handled = true;

            var currentPosition = e.GetPosition(element);

            var left = Canvas.GetLeft(this) - (_clickPosition.X - currentPosition.X);
            var top = Canvas.GetTop(this) - (_clickPosition.Y - currentPosition.Y);

            left = Math.Max(8d, left);
            top = Math.Max(30d, top);

            var canvas = (Canvas) Parent;

            var minLeft = canvas.ActualWidth - Width - 8;
            left = Math.Min(minLeft, left);

            var minTop = canvas.ActualHeight - Height - 8;
            top = Math.Min(minTop, top);

            Canvas.SetTop(this, top);
            Canvas.SetLeft(this, left);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ConfigureContentHost();
            ConfigureThumbs();
        }

        private void ConfigureThumbs()
        {
            ConfigureRightBorder();
            ConfigureLeftBorder();
            ConfigureTopBorder();
            ConfigureBottomBorder();
            ConfigureRightTopBorder();
            ConfigureRightBottomCorner();
            ConfigureLeftTopCorner();
            ConfigureLeftBottom();
        }

        private void ConfigureLeftBottom()
        {
            if (_leftBottomBorder != null)
            {
                _leftBottomBorder.DragDelta -= LeftBorderDrag;
                _leftBottomBorder.DragDelta -= BottomBorderDrag;
            }

            _leftBottomBorder = GetTemplateChild("Part_LeftBottomCorner") as Thumb;

            if (_leftBottomBorder != null)
            {
                _leftBottomBorder.DragDelta += LeftBorderDrag;
                _leftBottomBorder.DragDelta += BottomBorderDrag;
            }
        }

        private void ConfigureLeftTopCorner()
        {
            if (_leftTopBorder != null)
            {
                _leftTopBorder.DragDelta -= LeftBorderDrag;
                _leftTopBorder.DragDelta -= TopBorderDrag;
            }

            _leftTopBorder = GetTemplateChild("Part_LeftTopCorner") as Thumb;

            if (_leftTopBorder != null)
            {
                _leftTopBorder.DragDelta += LeftBorderDrag;
                _leftTopBorder.DragDelta += TopBorderDrag;
            }
        }

        private void ConfigureRightBottomCorner()
        {
            if (_rightBottomBorder != null)
            {
                _rightBottomBorder.DragDelta -= RightBorderDrag;
                _rightBottomBorder.DragDelta -= BottomBorderDrag;
            }

            _rightBottomBorder = GetTemplateChild("Part_RightBottomCorner") as Thumb;

            if (_rightBottomBorder != null)
            {
                _rightBottomBorder.DragDelta += RightBorderDrag;
                _rightBottomBorder.DragDelta += BottomBorderDrag;
            }
        }

        private void ConfigureRightTopBorder()
        {
            if (_rightTopBorder != null)
            {
                _rightTopBorder.DragDelta -= RightBorderDrag;
                _rightTopBorder.DragDelta -= TopBorderDrag;
            }

            _rightTopBorder = GetTemplateChild("Part_RightTopBorder") as Thumb;

            if (_rightTopBorder != null)
            {
                _rightTopBorder.DragDelta += RightBorderDrag;
                _rightTopBorder.DragDelta += TopBorderDrag;
            }
        }

        private void ConfigureBottomBorder()
        {
            if (_bottomBorder != null)
                _bottomBorder.DragDelta -= BottomBorderDrag;

            _bottomBorder = GetTemplateChild("Part_BottomBorder") as Thumb;

            if (_bottomBorder != null)
                _bottomBorder.DragDelta += BottomBorderDrag;
        }

        private void ConfigureTopBorder()
        {
            if (_topBorder != null)
                _topBorder.DragDelta -= TopBorderDrag;

            _topBorder = GetTemplateChild("Part_TopBorder") as Thumb;

            if (_topBorder != null)
                _topBorder.DragDelta += TopBorderDrag;
        }

        private void ConfigureLeftBorder()
        {
            if (_leftBorder != null)
                _leftBorder.DragDelta -= LeftBorderDrag;

            _leftBorder = GetTemplateChild("Part_LeftBorder") as Thumb;

            if (_leftBorder != null)
                _leftBorder.DragDelta += LeftBorderDrag;
        }

        private void ConfigureRightBorder()
        {
            if (_rightBorder != null)
                _rightBorder.DragDelta -= RightBorderDrag;

            _rightBorder = GetTemplateChild("Part_RightBorder") as Thumb;

            if (_rightBorder != null)
                _rightBorder.DragDelta += RightBorderDrag;
        }

        private void BottomBorderDrag(object sender, DragDeltaEventArgs e)
        {
            VerticalChange(false, e);
        }

        private void TopBorderDrag(object sender, DragDeltaEventArgs e)
        {
            VerticalChange(true, e);
        }

        private void LeftBorderDrag(object sender, DragDeltaEventArgs e)
        {
            HorizontalChange(true, e);
        }

        private void RightBorderDrag(object sender, DragDeltaEventArgs e)
        {
            HorizontalChange(false, e);
        }

        private void ConfigureContentHost()
        {
            if (_contentHost != null)
            {
                _contentHost.MouseLeftButtonDown -= ContentMouseDown;
                _contentHost.MouseLeftButtonUp -= ContentMouseUp;
                _contentHost.MouseMove -= ContentMouseMove;
            }

            _contentHost = GetTemplateChild("PART_ContentHost") as FrameworkElement;

            if (_contentHost != null)
            {
                _contentHost.MouseLeftButtonDown += ContentMouseDown;
                _contentHost.MouseLeftButtonUp += ContentMouseUp;
                _contentHost.MouseMove += ContentMouseMove;
            }
        }
    }
}
