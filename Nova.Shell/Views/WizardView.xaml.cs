#region License
//   
//  Copyright 2013 Steven Thuriot
//  
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
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
        /// <summary>
        /// Initializes a new instance of the <see cref="WizardView"/> class.
        /// </summary>
        public WizardView()
        {
            Loaded += Initialize;
            InitializeComponent();
        }

        private void Initialize(object sender, RoutedEventArgs routedEventArgs)
        {
            Loaded -= Initialize;

            var canvas = (Canvas)Parent;

            var top = (canvas.ActualHeight - Height) / 2d;
            var left = (canvas.ActualWidth - Width) / 2d;

            Canvas.SetTop(this, top);
            Canvas.SetLeft(this, left);

            SubscribeDragEvents();
        }

        private void SubscribeDragEvents()
        {
            RightBorder.DragDelta += (view, args) => HorizontalChange(true, args);
            LeftBorder.DragDelta += (view, args) => HorizontalChange(false, args);

            TopBorder.DragDelta += (view, args) => VerticalChange(true, args);
            BottomBorder.DragDelta += (view, args) => VerticalChange(false, args);

            RightTopCorner.DragDelta += (view, args) =>
                {
                    HorizontalChange(true, args);
                    VerticalChange(true, args);
                };

            RightBottomCorner.DragDelta += (view, args) =>
                {
                    HorizontalChange(true, args);
                    VerticalChange(false, args);
                };

            LeftTopCorner.DragDelta += (view, args) =>
                {
                    HorizontalChange(false, args);
                    VerticalChange(true, args);
                };

            LeftBottomCorner.DragDelta += (view, args) =>
                {
                    HorizontalChange(false, args);
                    VerticalChange(false, args);
                };
        }

        //TODO: Try to optimize when dragging really fast
        //TODO: On Window resize, adjust size/canvas if needed. (Prefer canvas)

        private void HorizontalChange(bool goingRight, DragDeltaEventArgs args)
        {
            double size;
            var left = Canvas.GetLeft(this);

            if (goingRight)
            {
                size = Math.Max(MinWidth, Width + args.HorizontalChange);
            }
            else
            {
                size = Width - args.HorizontalChange;

                if (size > MinWidth)
                {
                    //Special case, adjust canvas!
                    left += args.HorizontalChange; //Plus because HorizontalChange is negative

                    if (left < 3)
                    {
                        size = Width;
                    }
                    else
                    {
                        Canvas.SetLeft(this, left);
                    }
                }
                else
                {
                    size = MinWidth;
                }
            }

            var canvas = (Canvas)Parent;
            var availableWidth = canvas.ActualWidth - left - 3;
            Width = Math.Min(availableWidth, size);
        }

        private void VerticalChange(bool goingUp, DragDeltaEventArgs args)
        {
            double size;
            var top = Canvas.GetTop(this);

            if (goingUp)
            {
                size = Height - args.VerticalChange;

                if (size > MinHeight)
                {
                    //Special case, adjust canvas!
                    top += args.VerticalChange; //Plus because VerticalChange is negative

                    if (top < 3)
                    {
                        size = Height;
                    }
                    else
                    {
                        Canvas.SetTop(this, top);
                    }
                }
                else
                {
                    size = MinHeight;
                }
            }
            else
            {
                size = Math.Max(MinHeight, Height + args.VerticalChange);
            }

            var canvas = (Canvas)Parent;
            var availableHeight = canvas.ActualHeight - top - 3;
            Height = Math.Min(availableHeight, size); ;
        }
    }
}
