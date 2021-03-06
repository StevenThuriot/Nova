﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Nova.Controls;

namespace Nova.Shell.Views
{
    /// <summary>
    /// Interaction logic for WizardView.xaml
    /// </summary>
    [TemplatePart(Name = "PART_ContentHost", Type = typeof(ContentPresenter))]
    [TemplatePart(Name = "PART_Wizard", Type = typeof(IInputElement))]
    [TemplatePart(Name = "PART_RightBorder", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_LeftBorder", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_TopBorder", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_BottomBorder", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_RightTopCorner", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_RightBottomCorner", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_LeftTopCorner", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_LeftBottomCorner", Type = typeof(Thumb))]
    public class WizardView : ExtendedContentControl<WizardView, WizardViewModel>
    {
        //TODO: Listen to window resizing and restrict wizard if needed.
        
        private Point _clickPosition;
        private IInputElement _inputElement;

        private Thumb _rightBorder;
        private Thumb _leftBorder;
        private Thumb _topBorder;
        private Thumb _bottomBorder;

        private Thumb _rightTopCorner;
        private Thumb _rightBottomCorner;
        private Thumb _leftTopCorner;
        private Thumb _leftBottomCorner;

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

            WidthProperty.AddOwner(typeof(WizardView), widthMetadata);
            HeightProperty.AddOwner(typeof(WizardView), heightMetadata);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WizardView"/> class.
        /// </summary>
        public WizardView()
        {
            Loaded += Initialize;
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
                                         Func<FrameworkElement, double> getParentActualSize,
                                         Func<UIElement, double> getCanvasProperty,
                                         Func<FrameworkElement, double> getMinimumSize)
        {
            var size = (double)baseValue;
            
            var wizard = (WizardView)dependencyObject;
            var element = (Canvas) wizard.Parent;
            
            if (element != null)
            {
                var availableWidth = getParentActualSize(element) - getCanvasProperty(wizard) - 3;

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
            
            var top = (canvas.ActualHeight - ActualHeight) / 2d;
            var left = (canvas.ActualWidth - ActualWidth) / 2d;

            Canvas.SetTop(this, top);
            Canvas.SetLeft(this, left);
        }

        private void HorizontalChange(bool draggingAffectsCanvas, DragDeltaEventArgs args)
        {
            Func<DragDeltaEventArgs, double> getDelta = x => x.HorizontalChange;
            Func<double> getCanvasPosition = () => Canvas.GetLeft(this);
            Action<double> setCanvasPosition = x => Canvas.SetLeft(this, x);
            Func<double> getMinimum = () => MinWidth;
            Func<double> getCurrent = () => ActualWidth;
            Action<double> addToCurrent = x => Width = Math.Min(Math.Max(Width + x, MinWidth), MaxWidth);

            ChangeSize(draggingAffectsCanvas, args, getDelta, getCanvasPosition, getCurrent, getMinimum, addToCurrent, setCanvasPosition);
        }

        private void VerticalChange(bool draggingAffectsCanvas, DragDeltaEventArgs args)
        {
            Func<DragDeltaEventArgs, double> getDelta = x => x.VerticalChange;
            Func<double> getCanvasPosition = () => Canvas.GetTop(this);
            Action<double> setCanvasPosition = x => Canvas.SetTop(this, x);
            Func<double> getMinimum = () => MinHeight;
            Func<double> getCurrent = () => ActualHeight;
            Action<double> addToCurrent = x => Height = Math.Min(Math.Max(Height + x, MinHeight), MaxHeight);

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

                if (size < getMinimum())
                    return;

                if (position <= 3)
                {
                    var positionDelta = 4 - position;
                    position = 4;
                    delta -= positionDelta;
                }

                setCanvasPosition(position);
            }

            addToCurrent(delta);
        }

        private void ContentMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Handled)
                return;
            
            var element = (FrameworkElement)sender;
            _clickPosition = e.GetPosition(element);

            e.Handled = element.CaptureMouse();
        }

        private static void ContentMouseUp(object sender, MouseButtonEventArgs e)
        {
            var element = (FrameworkElement)sender;
            element.ReleaseMouseCapture();
        }

        private void ContentMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Handled)
                return;

            var element = (FrameworkElement)sender;

            if (!element.IsMouseCaptured)
                return;

            e.Handled = true;

            const double margin = 3d;

            var currentPosition = e.GetPosition(element);

            var left = Canvas.GetLeft(this) - (_clickPosition.X - currentPosition.X);
            var top = Canvas.GetTop(this) - (_clickPosition.Y - currentPosition.Y);

            left = Math.Max(margin, left);
            top = Math.Max(margin, top);

            var canvas = (FrameworkElement)Parent;

            var minLeft = canvas.ActualWidth - ActualWidth - margin;
            left = Math.Min(minLeft, left);

            var minTop = canvas.ActualHeight - ActualHeight - margin;
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
            if (_leftBottomCorner != null)
            {
                _leftBottomCorner.DragDelta -= LeftBorderDrag;
                _leftBottomCorner.DragDelta -= BottomBorderDrag;
            }

            _leftBottomCorner = GetTemplateChild("Part_LeftBottomCorner") as Thumb;

            if (_leftBottomCorner != null)
            {
                _leftBottomCorner.DragDelta += LeftBorderDrag;
                _leftBottomCorner.DragDelta += BottomBorderDrag;
            }
        }

        private void ConfigureLeftTopCorner()
        {
            if (_leftTopCorner != null)
            {
                _leftTopCorner.DragDelta -= LeftBorderDrag;
                _leftTopCorner.DragDelta -= TopBorderDrag;
            }

            _leftTopCorner = GetTemplateChild("Part_LeftTopCorner") as Thumb;

            if (_leftTopCorner != null)
            {
                _leftTopCorner.DragDelta += LeftBorderDrag;
                _leftTopCorner.DragDelta += TopBorderDrag;
            }
        }

        private void ConfigureRightBottomCorner()
        {
            if (_rightBottomCorner != null)
            {
                _rightBottomCorner.DragDelta -= RightBorderDrag;
                _rightBottomCorner.DragDelta -= BottomBorderDrag;
            }

            _rightBottomCorner = GetTemplateChild("Part_RightBottomCorner") as Thumb;

            if (_rightBottomCorner != null)
            {
                _rightBottomCorner.DragDelta += RightBorderDrag;
                _rightBottomCorner.DragDelta += BottomBorderDrag;
            }
        }

        private void ConfigureRightTopBorder()
        {
            if (_rightTopCorner != null)
            {
                _rightTopCorner.DragDelta -= RightBorderDrag;
                _rightTopCorner.DragDelta -= TopBorderDrag;
            }

            _rightTopCorner = GetTemplateChild("Part_RightTopCorner") as Thumb;

            if (_rightTopCorner != null)
            {
                _rightTopCorner.DragDelta += RightBorderDrag;
                _rightTopCorner.DragDelta += TopBorderDrag;
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
            if (_inputElement != null)
            {
                _inputElement.MouseLeftButtonDown -= ContentMouseDown;
                _inputElement.MouseLeftButtonUp -= ContentMouseUp;
                _inputElement.MouseMove -= ContentMouseMove;
            }

            _inputElement = (IInputElement) GetTemplateChild("PART_Wizard");

            if (_inputElement != null)
            {
                _inputElement.MouseLeftButtonDown += ContentMouseDown;
                _inputElement.MouseLeftButtonUp += ContentMouseUp;
                _inputElement.MouseMove += ContentMouseMove;
            }
        }
    }
}
