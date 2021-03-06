﻿using System;
using System.Windows;
using System.Windows.Controls;

namespace Nova.Controls
{
    /// <summary>
    /// Flow <see cref="Panel"/> class used for the Closable Tab Control's tab items.
    /// </summary>
    public class FlowPanel : Panel
    {
        /// <summary>
        /// The minimum item width property
        /// </summary>
        public static readonly DependencyProperty MinimumItemWidthProperty =
            DependencyProperty.Register("MinimumItemWidth", typeof(double), typeof(FlowPanel), new FrameworkPropertyMetadata(64d, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// The maximum item width property
        /// </summary>
        public static readonly DependencyProperty MaximumItemWidthProperty =
            DependencyProperty.Register("MaximumItemWidth", typeof(double), typeof(FlowPanel), new FrameworkPropertyMetadata(150d, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));
        
        /// <summary>
        /// Gets or sets the minimum width of the item.
        /// </summary>
        /// <value>
        /// The minimum width of the item.
        /// </value>
        public double MinimumItemWidth
        {
            get { return (double)GetValue(MinimumItemWidthProperty); }
            set { SetValue(MinimumItemWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the maximum width of the item.
        /// </summary>
        /// <value>
        /// The maximum width of the item.
        /// </value>
        public double MaximumItemWidth
        {
            get { return (double)GetValue(MaximumItemWidthProperty); }
            set { SetValue(MaximumItemWidthProperty, value); }
        }

        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement" />-derived class.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            double count = InternalChildren.Count;

            var maximumChildrenWidth = count*MaximumItemWidth + count;
            var width = Math.Min(MaxWidth, maximumChildrenWidth);

            var height = double.IsInfinity(availableSize.Height) ? 0d : availableSize.Height;
            var finalSize = new Size(width, height);

            if (Math.Abs(count) > double.Epsilon)
            {
                var averageWidth = GetAverageItemWidth(finalSize);
                var childSize = new Size(averageWidth, availableSize.Height);

                foreach (UIElement child in InternalChildren)
                {
                    child.Measure(childSize);
                }
            }

            return finalSize;
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement" /> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>
        /// The actual size used.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (InternalChildren.Count == 0) return finalSize;

            var width = GetAverageItemWidth(finalSize);

            var x = 0d;
            foreach (UIElement child in InternalChildren)
            {
                var location = new Point(x, 0d);
                var size = new Size(width, finalSize.Height);

                var rect = new Rect(location, size);
                child.Arrange(rect);

                //-2 because we want the items to slightly overlap.
                //This is also why we added count to the maximumChildrenWidth in MeasureOverride.
                x += width - 2d;
            }

            return finalSize;
        }

        /// <summary>
        /// Gets the average width of the item.
        /// </summary>
        /// <param name="availableSize">The size.</param>
        /// <returns></returns>
        private double GetAverageItemWidth(Size availableSize)
        {
            var count = InternalChildren.Count;
            
            var maxWidth = MaxWidth;
            var finalWidth = !double.IsInfinity(maxWidth) ? maxWidth : availableSize.Width;

            finalWidth = Math.Floor(finalWidth);
            var averageWidth = Math.Floor(finalWidth/count); //Floor because we don't want overflow.

            var minimum = Math.Min(averageWidth, MaximumItemWidth);
            var width = Math.Max(minimum, MinimumItemWidth);
            return width;
        }
    } 
}
