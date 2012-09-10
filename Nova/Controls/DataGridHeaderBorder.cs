﻿#region License
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Nova.Controls
{
	/// <summary> 
	///     A Border used to provide the default look of DataGrid headers.
	///     When Background or BorderBrush are set, the rendering will 
	///     revert back to the default Border implementation.
	/// </summary>
	public class DataGridHeaderBorder : Border
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static DataGridHeaderBorder()
		{
			// We always set this to true on these borders, so just default it to true here. 
			SnapsToDevicePixelsProperty.OverrideMetadata(typeof (DataGridHeaderBorder), new FrameworkPropertyMetadata(true));
		}

		#region Header Appearance Properties

		/// <summary> 
		///     DependencyProperty for IsHovered.
		/// </summary> 
		public static readonly DependencyProperty IsHoveredProperty =
			DependencyProperty.Register("IsHovered", typeof (bool), typeof (DataGridHeaderBorder),
			                            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

		/// <summary>
		///     DependencyProperty for IsPressed.
		/// </summary>
		public static readonly DependencyProperty IsPressedProperty =
			DependencyProperty.Register("IsPressed", typeof (bool), typeof (DataGridHeaderBorder),
			                            new FrameworkPropertyMetadata(false,
			                                                          FrameworkPropertyMetadataOptions.AffectsRender |
			                                                          FrameworkPropertyMetadataOptions.AffectsArrange));

		/// <summary> 
		///     DependencyProperty for IsClickable.
		/// </summary> 
		public static readonly DependencyProperty IsClickableProperty =
			DependencyProperty.Register("IsClickable", typeof (bool), typeof (DataGridHeaderBorder),
			                            new FrameworkPropertyMetadata(true,
			                                                          FrameworkPropertyMetadataOptions.AffectsRender |
			                                                          FrameworkPropertyMetadataOptions.AffectsArrange));

		/// <summary> 
		///     DependencyProperty for SortDirection.
		/// </summary> 
		public static readonly DependencyProperty SortDirectionProperty =
			DependencyProperty.Register("SortDirection", typeof (ListSortDirection?), typeof (DataGridHeaderBorder),
			                            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

		/// <summary>
		///     DependencyProperty for IsSelected.
		/// </summary>
		public static readonly DependencyProperty IsSelectedProperty =
			DependencyProperty.Register("IsSelected", typeof (bool), typeof (DataGridHeaderBorder),
			                            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

		/// <summary>
		///     DependencyProperty for Orientation. 
		/// </summary>
		public static readonly DependencyProperty OrientationProperty =
			DependencyProperty.Register("Orientation", typeof (Orientation), typeof (DataGridHeaderBorder),
			                            new FrameworkPropertyMetadata(Orientation.Vertical,
			                                                          FrameworkPropertyMetadataOptions.AffectsRender));

		/// <summary> 
		///     DependencyProperty for SeparatorBrush.
		/// </summary> 
		public static readonly DependencyProperty SeparatorBrushProperty =
			DependencyProperty.Register("SeparatorBrush", typeof (Brush), typeof (DataGridHeaderBorder),
			                            new FrameworkPropertyMetadata(null));

		/// <summary> 
		///     DependencyProperty for SeperatorBrush.
		/// </summary> 
		public static readonly DependencyProperty SeparatorVisibilityProperty =
			DependencyProperty.Register("SeparatorVisibility", typeof (Visibility), typeof (DataGridHeaderBorder),
			                            new FrameworkPropertyMetadata(Visibility.Visible));

		/// <summary> 
		///     Whether the hover look should be applied.
		/// </summary> 
		public bool IsHovered
		{
			get { return (bool) GetValue(IsHoveredProperty); }
			set { SetValue(IsHoveredProperty, value); }
		}

		/// <summary>
		///     Whether the pressed look should be applied.
		/// </summary>
		public bool IsPressed
		{
			get { return (bool) GetValue(IsPressedProperty); }
			set { SetValue(IsPressedProperty, value); }
		}

		/// <summary> 
		///     When false, will not apply the hover look even when IsHovered is true.
		/// </summary> 
		public bool IsClickable
		{
			get { return (bool) GetValue(IsClickableProperty); }
			set { SetValue(IsClickableProperty, value); }
		}

		/// <summary> 
		///     Whether to appear sorted.
		/// </summary> 
		public ListSortDirection? SortDirection
		{
			get { return (ListSortDirection?) GetValue(SortDirectionProperty); }
			set { SetValue(SortDirectionProperty, value); }
		}

		/// <summary>
		///     Whether to appear selected.
		/// </summary>
		public bool IsSelected
		{
			get { return (bool) GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}

		/// <summary> 
		///     Vertical = column header
		///     Horizontal = row header 
		/// </summary>
		public Orientation Orientation
		{
			get { return (Orientation) GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}

		/// <summary>
		///     When there is a Background or BorderBrush, revert to the Border implementation. 
		/// </summary> 
		private bool UsingBorderImplementation
		{
			get { return (Background != null) || (BorderBrush != null); }
		}

		/// <summary> 
		///     Property that indicates the brush to use when drawing seperators between headers.
		/// </summary> 
		public Brush SeparatorBrush
		{
			get { return (Brush) GetValue(SeparatorBrushProperty); }
			set { SetValue(SeparatorBrushProperty, value); }
		}

		/// <summary> 
		///     Property that indicates the Visibility for the header seperators.
		/// </summary> 
		public Visibility SeparatorVisibility
		{
			get { return (Visibility) GetValue(SeparatorVisibilityProperty); }
			set { SetValue(SeparatorVisibilityProperty, value); }
		}

		#endregion

		#region Layout

		/// <summary>
		///     Calculates the desired size of the element given the constraint. 
		/// </summary> 
		protected override Size MeasureOverride(Size constraint)
		{
			if (UsingBorderImplementation)
			{
				// Revert to the Border implementation
				return base.MeasureOverride(constraint);
			}

			UIElement child = Child;
			if (child != null)
			{
				// Use the public Padding property if it's set
				Thickness padding = Padding;
				if (padding.Equals(new Thickness()))
				{
					padding = DefaultPadding;
				}

				double childWidth = constraint.Width;
				double childHeight = constraint.Height;

				// If there is an actual constraint, then reserve space for the chrome
				if (!Double.IsInfinity(childWidth))
				{
					childWidth = Math.Max(0.0, childWidth - padding.Left - padding.Right);
				}

				if (!Double.IsInfinity(childHeight))
				{
					childHeight = Math.Max(0.0, childHeight - padding.Top - padding.Bottom);
				}

				child.Measure(new Size(childWidth, childHeight));
				Size desiredSize = child.DesiredSize;

				// Add on the reserved space for the chrome 
				return new Size(desiredSize.Width + padding.Left + padding.Right, desiredSize.Height + padding.Top + padding.Bottom);
			}

			return new Size();
		}

		/// <summary>
		///     Positions children and returns the final size of the element. 
		/// </summary> 
		protected override Size ArrangeOverride(Size arrangeSize)
		{
			if (UsingBorderImplementation)
			{
				// Revert to the Border implementation
				return base.ArrangeOverride(arrangeSize);
			}

			UIElement child = Child;
			if (child != null)
			{
				// Use the public Padding property if it's set
				Thickness padding = Padding;
				if (padding.Equals(new Thickness()))
				{
					padding = DefaultPadding;
				}

				// Reserve space for the chrome
				double childWidth = Math.Max(0.0, arrangeSize.Width - padding.Left - padding.Right);
				double childHeight = Math.Max(0.0, arrangeSize.Height - padding.Top - padding.Bottom);

				child.Arrange(new Rect(padding.Left, padding.Top, childWidth, childHeight));
			}

			return arrangeSize;
		}

		#endregion

		#region Rendering

		/// <summary> 
		///     Returns a default padding for the various themes for use
		///     by measure and arrange. 
		/// </summary> 
		private Thickness DefaultPadding
		{
			get
			{
				var padding = new Thickness(3.0); // The default padding
				Thickness? themePadding = ThemeDefaultPadding;
				if (themePadding == null)
				{
					if (Orientation == Orientation.Vertical)
					{
						// Reserve space to the right for the arrow 
						padding.Right = 15.0;
					}
				}
				else
				{
					padding = (Thickness) themePadding;
				}

				// When pressed, offset the child 
				if (IsPressed && IsClickable)
				{
					padding.Left += 1.0;
					padding.Top += 1.0;
					padding.Right -= 1.0;
					padding.Bottom -= 1.0;
				}

				return padding;
			}
		}

		/// <summary> 
		///     Called when this element should re-render.
		/// </summary> 
		protected override void OnRender(DrawingContext dc)
		{
			if (UsingBorderImplementation)
			{
				// Revert to the Border implementation
				base.OnRender(dc);
			}
			else
			{
				RenderTheme(dc);
			}
		}

		private static double Max0(double d)
		{
			return Math.Max(0.0, d);
		}

		#endregion

		#region Freezable Cache

		private static List<Freezable> _freezableCache;
		private static readonly object _cacheAccess = new object();

		#region Theme Rendering

		private Thickness? ThemeDefaultPadding
		{
			get
			{
				if (Orientation == Orientation.Vertical)
				{
					return new Thickness(5.0, 4.0, 5.0, 4.0);
				}
				return null;
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		private void RenderTheme(DrawingContext dc)
		{
			Size size = RenderSize;
			bool horizontal = Orientation == Orientation.Horizontal;
			bool isClickable = IsClickable && IsEnabled;
			bool isHovered = isClickable && IsHovered;
			bool isPressed = isClickable && IsPressed;
			ListSortDirection? sortDirection = SortDirection;
			bool isSorted = sortDirection != null;
			bool isSelected = IsSelected;
			bool hasBevel = (!isHovered && !isPressed && !isSorted && !isSelected);

			EnsureCache((int) AeroFreezables.NumFreezables);

			if (horizontal)
			{
				// When horizontal, rotate the rendering by -90 degrees
				var m1 = new Matrix();
				m1.RotateAt(-90.0, 0.0, 0.0);
				var m2 = new Matrix();
				m2.Translate(0.0, size.Height);

				var horizontalRotate = new MatrixTransform(m1*m2);
				horizontalRotate.Freeze();
				dc.PushTransform(horizontalRotate);

				double temp = size.Width;
				size.Width = size.Height;
				size.Height = temp;
			}

			if (hasBevel)
			{
				// This is a highlight that can be drawn by just filling the background with the color. 
				// It will be seen through the gab between the border and the background.
				var bevel = (LinearGradientBrush) GetCachedFreezable((int) AeroFreezables.NormalBevel);
				if (bevel == null)
				{
					bevel = new LinearGradientBrush();
					bevel.StartPoint = new Point();
					bevel.EndPoint = new Point(0.0, 1.0);
					bevel.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF), 0.0));
					bevel.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF), 0.4));
					bevel.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xFC, 0xFC, 0xFD), 0.4));
					bevel.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xFB, 0xFC, 0xFC), 1.0));
					bevel.Freeze();

					CacheFreezable(bevel, (int) AeroFreezables.NormalBevel);
				}

				dc.DrawRectangle(bevel, null, new Rect(0.0, 0.0, size.Width, size.Height));
			}

			// Fill the background 
			AeroFreezables backgroundType = AeroFreezables.NormalBackground;
			if (isPressed)
			{
				backgroundType = AeroFreezables.PressedBackground;
			}
			else if (isHovered)
			{
				backgroundType = AeroFreezables.HoveredBackground;
			}
			else if (isSorted || isSelected)
			{
				backgroundType = AeroFreezables.SortedBackground;
			}

			var background = (LinearGradientBrush) GetCachedFreezable((int) backgroundType);
			if (background == null)
			{
				background = new LinearGradientBrush();
				background.StartPoint = new Point();
				background.EndPoint = new Point(0.0, 1.0);

				switch (backgroundType)
				{
					case AeroFreezables.NormalBackground:
						background.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF), 0.0));
						background.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF), 0.4));
						background.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xF7, 0xF8, 0xFA), 0.4));
						background.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xF1, 0xF2, 0xF4), 1.0));
						break;

					case AeroFreezables.PressedBackground:
						background.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xBC, 0xE4, 0xF9), 0.0));
						background.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xBC, 0xE4, 0xF9), 0.4));
						background.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0x8D, 0xD6, 0xF7), 0.4));
						background.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0x8A, 0xD1, 0xF5), 1.0));
						break;

					case AeroFreezables.HoveredBackground:
						background.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xE3, 0xF7, 0xFF), 0.0));
						background.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xE3, 0xF7, 0xFF), 0.4));
						background.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xBD, 0xED, 0xFF), 0.4));
						background.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xB7, 0xE7, 0xFB), 1.0));
						break;

					case AeroFreezables.SortedBackground:
						background.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xF2, 0xF9, 0xFC), 0.0));
						background.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xF2, 0xF9, 0xFC), 0.4));
						background.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xE1, 0xF1, 0xF9), 0.4));
						background.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xD8, 0xEC, 0xF6), 1.0));
						break;
				}

				background.Freeze();

				CacheFreezable(background, (int) backgroundType);
			}

			dc.DrawRectangle(background, null, new Rect(0.0, 0.0, size.Width, size.Height));

			if (size.Width >= 2.0)
			{
				// Draw the borders on the sides
				AeroFreezables sideType = AeroFreezables.NormalSides;
				if (isPressed)
				{
					sideType = AeroFreezables.PressedSides;
				}
				else if (isHovered)
				{
					sideType = AeroFreezables.HoveredSides;
				}
				else if (isSorted || isSelected)
				{
					sideType = AeroFreezables.SortedSides;
				}

				if (SeparatorVisibility == Visibility.Visible)
				{
					Brush sideBrush;
					if (SeparatorBrush != null)
					{
						sideBrush = SeparatorBrush;
					}
					else
					{
						sideBrush = (Brush) GetCachedFreezable((int) sideType);
						if (sideBrush == null)
						{
							LinearGradientBrush lgBrush = null;
							if (sideType != AeroFreezables.SortedSides)
							{
								lgBrush = new LinearGradientBrush();
								lgBrush.StartPoint = new Point();
								lgBrush.EndPoint = new Point(0.0, 1.0);
								sideBrush = lgBrush;
							}

							switch (sideType)
							{
								case AeroFreezables.NormalSides:
									lgBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xF2, 0xF2, 0xF2), 0.0));
									lgBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xEF, 0xEF, 0xEF), 0.4));
									lgBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xE7, 0xE8, 0xEA), 0.4));
									lgBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xDE, 0xDF, 0xE1), 1.0));
									break;

								case AeroFreezables.PressedSides:
									lgBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0x7A, 0x9E, 0xB1), 0.0));
									lgBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0x7A, 0x9E, 0xB1), 0.4));
									lgBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0x50, 0x91, 0xAF), 0.4));
									lgBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0x4D, 0x8D, 0xAD), 1.0));
									break;

								case AeroFreezables.HoveredSides:
									lgBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0x88, 0xCB, 0xEB), 0.0));
									lgBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0x88, 0xCB, 0xEB), 0.4));
									lgBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0x69, 0xBB, 0xE3), 0.4));
									lgBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0x69, 0xBB, 0xE3), 1.0));
									break;

								case AeroFreezables.SortedSides:
									sideBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x96, 0xD9, 0xF9));
									break;
							}

							sideBrush.Freeze();

							CacheFreezable(sideBrush, (int) sideType);
						}
					}

					dc.DrawRectangle(sideBrush, null, new Rect(0.0, 0.0, 1.0, Max0(size.Height - 0.95)));
					dc.DrawRectangle(sideBrush, null, new Rect(size.Width - 1.0, 0.0, 1.0, Max0(size.Height - 0.95)));
				}
			}

			if (isPressed && (size.Width >= 4.0) && (size.Height >= 4.0))
			{
				// When pressed, there are added borders on the left and top 
				var topBrush = (LinearGradientBrush) GetCachedFreezable((int) AeroFreezables.PressedTop);
				if (topBrush == null)
				{
					topBrush = new LinearGradientBrush();
					topBrush.StartPoint = new Point();
					topBrush.EndPoint = new Point(0.0, 1.0);
					topBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0x86, 0xA3, 0xB2), 0.0));
					topBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0x86, 0xA3, 0xB2), 0.1));
					topBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xAA, 0xCE, 0xE1), 0.9));
					topBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xAA, 0xCE, 0xE1), 1.0));
					topBrush.Freeze();

					CacheFreezable(topBrush, (int) AeroFreezables.PressedTop);
				}

				dc.DrawRectangle(topBrush, null, new Rect(0.0, 0.0, size.Width, 2.0));

				var pressedBevel = (LinearGradientBrush) GetCachedFreezable((int) AeroFreezables.PressedBevel);
				if (pressedBevel == null)
				{
					pressedBevel = new LinearGradientBrush();
					pressedBevel.StartPoint = new Point();
					pressedBevel.EndPoint = new Point(0.0, 1.0);
					pressedBevel.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xA2, 0xCB, 0xE0), 0.0));
					pressedBevel.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xA2, 0xCB, 0xE0), 0.4));
					pressedBevel.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0x72, 0xBC, 0xDF), 0.4));
					pressedBevel.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0x6E, 0xB8, 0xDC), 1.0));
					pressedBevel.Freeze();

					CacheFreezable(pressedBevel, (int) AeroFreezables.PressedBevel);
				}

				dc.DrawRectangle(pressedBevel, null, new Rect(1.0, 0.0, 1.0, size.Height - 0.95));
				dc.DrawRectangle(pressedBevel, null, new Rect(size.Width - 2.0, 0.0, 1.0, size.Height - 0.95));
			}

			if (size.Height >= 2.0)
			{
				// Draw the bottom border 
				AeroFreezables bottomType = AeroFreezables.NormalBottom;
				if (isPressed)
				{
					bottomType = AeroFreezables.PressedOrHoveredBottom;
				}
				else if (isHovered)
				{
					bottomType = AeroFreezables.PressedOrHoveredBottom;
				}
				else if (isSorted || isSelected)
				{
					bottomType = AeroFreezables.SortedBottom;
				}

				var bottomBrush = (SolidColorBrush) GetCachedFreezable((int) bottomType);
				if (bottomBrush == null)
				{
					switch (bottomType)
					{
						case AeroFreezables.NormalBottom:
							bottomBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xD5, 0xD5, 0xD5));
							break;

						case AeroFreezables.PressedOrHoveredBottom:
							bottomBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x93, 0xC9, 0xE3));
							break;

						case AeroFreezables.SortedBottom:
							bottomBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x96, 0xD9, 0xF9));
							break;
					}

					bottomBrush.Freeze();

					CacheFreezable(bottomBrush, (int) bottomType);
				}

				dc.DrawRectangle(bottomBrush, null, new Rect(0.0, size.Height - 1.0, size.Width, 1.0));
			}

			if (isSorted && (size.Width > 14.0) && (size.Height > 10.0))
			{
				// Draw the sort arrow
				var positionTransform = new TranslateTransform((size.Width - 10.0), Math.Round(size.Height / 2) - 4.0);
				positionTransform.Freeze();
				dc.PushTransform(positionTransform);

				bool ascending = (sortDirection == ListSortDirection.Ascending);
				var arrowGeometry =
					(PathGeometry)
					GetCachedFreezable(ascending ? (int) AeroFreezables.ArrowUpGeometry : (int) AeroFreezables.ArrowDownGeometry);
				if (arrowGeometry == null)
				{
					arrowGeometry = new PathGeometry();
					var arrowFigure = new PathFigure();

					if (ascending)
					{
						arrowFigure.StartPoint = new Point(0.0, 4.0);

						var line = new LineSegment(new Point(4.0, 0.0), false);
						line.Freeze();
						arrowFigure.Segments.Add(line);

						line = new LineSegment(new Point(8.0, 4.0), false);
						line.Freeze();
						arrowFigure.Segments.Add(line);
					}
					else
					{
						arrowFigure.StartPoint = new Point(0.0, 0.0);

						var line = new LineSegment(new Point(8.0, 0.0), false);
						line.Freeze();
						arrowFigure.Segments.Add(line);

						line = new LineSegment(new Point(4.0, 4.0), false);
						line.Freeze();
						arrowFigure.Segments.Add(line);
					}

					arrowFigure.IsClosed = true;
					arrowFigure.Freeze();

					arrowGeometry.Figures.Add(arrowFigure);
					arrowGeometry.Freeze();

					CacheFreezable(arrowGeometry,
					               ascending ? (int) AeroFreezables.ArrowUpGeometry : (int) AeroFreezables.ArrowDownGeometry);
				}

				// Draw two arrows, one inset in the other. This is to achieve a double gradient over both the border and the fill. 
				var arrowBorder = (LinearGradientBrush) GetCachedFreezable((int) AeroFreezables.ArrowBorder);
				if (arrowBorder == null)
				{
					arrowBorder = new LinearGradientBrush();
					arrowBorder.StartPoint = new Point();
					arrowBorder.EndPoint = new Point(1.0, 1.0);
					arrowBorder.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0x3C, 0x5E, 0x72), 0.0));
					arrowBorder.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0x3C, 0x5E, 0x72), 0.1));
					arrowBorder.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xC3, 0xE4, 0xF5), 1.0));
					arrowBorder.Freeze();
					CacheFreezable(arrowBorder, (int) AeroFreezables.ArrowBorder);
				}

				dc.DrawGeometry(arrowBorder, null, arrowGeometry);

				var arrowFill = (LinearGradientBrush) GetCachedFreezable((int) AeroFreezables.ArrowFill);
				if (arrowFill == null)
				{
					arrowFill = new LinearGradientBrush();
					arrowFill.StartPoint = new Point();
					arrowFill.EndPoint = new Point(1.0, 1.0);
					arrowFill.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0x61, 0x96, 0xB6), 0.0));
					arrowFill.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0x61, 0x96, 0xB6), 0.1));
					arrowFill.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xCA, 0xE6, 0xF5), 1.0));
					arrowFill.Freeze();
					CacheFreezable(arrowFill, (int) AeroFreezables.ArrowFill);
				}

				// Inset the fill arrow inside the border arrow
				var arrowScale = (ScaleTransform) GetCachedFreezable((int) AeroFreezables.ArrowFillScale);
				if (arrowScale == null)
				{
					arrowScale = new ScaleTransform(0.75, 0.75, 3.5, 4.0);
					arrowScale.Freeze();
					CacheFreezable(arrowScale, (int) AeroFreezables.ArrowFillScale);
				}

				dc.PushTransform(arrowScale);

				dc.DrawGeometry(arrowFill, null, arrowGeometry);

				dc.Pop(); // Scale Transform
				dc.Pop(); // Position Transform 
			}

			if (horizontal)
			{
				dc.Pop(); // Horizontal Rotate 
			}
		}

		private enum AeroFreezables
		{
			NormalBevel,
			NormalBackground,
			PressedBackground,
			HoveredBackground,
			SortedBackground,
			PressedTop,
			NormalSides,
			PressedSides,
			HoveredSides,
			SortedSides,
			PressedBevel,
			NormalBottom,
			PressedOrHoveredBottom,
			SortedBottom,
			ArrowBorder,
			ArrowFill,
			ArrowFillScale,
			ArrowUpGeometry,
			ArrowDownGeometry,
			NumFreezables
		}

		#endregion

		/// <summary>
		///     Creates a cache of frozen Freezable resources for use
		///     across all instances of the border. 
		/// </summary>
		private static void EnsureCache(int size)
		{
			// Quick check to avoid locking
			if (_freezableCache == null)
			{
				lock (_cacheAccess)
				{
					// Re-check in case another thread created the cache 
					if (_freezableCache == null)
					{
						_freezableCache = new List<Freezable>(size);
						for (int i = 0; i < size; i++)
						{
							_freezableCache.Add(null);
						}
					}
				}
			}

			Debug.Assert(_freezableCache.Count == size, "The cache size does not match the requested amount.");
		}

		/// <summary>
		///     Releases all resources in the cache.
		/// </summary>
		private static void ReleaseCache()
		{
			// Avoid locking if necessary 
			if (_freezableCache != null)
			{
				lock (_cacheAccess)
				{
					// No need to re-check if non-null since it's OK to set it to null multiple times
					_freezableCache = null;
				}
			}
		}

		/// <summary>
		///     Retrieves a cached resource. 
		/// </summary>
		private static Freezable GetCachedFreezable(int index)
		{
			lock (_cacheAccess)
			{
				Freezable freezable = _freezableCache[index];
				Debug.Assert((freezable == null) || freezable.IsFrozen, "Cached Freezables should have been frozen.");
				return freezable;
			}
		}

		/// <summary>
		///     Caches a resources. 
		/// </summary>
		private static void CacheFreezable(Freezable freezable, int index)
		{
			Debug.Assert(freezable.IsFrozen, "Cached Freezables should be frozen.");

			lock (_cacheAccess)
			{
				if (_freezableCache[index] != null)
				{
					_freezableCache[index] = freezable;
				}
			}
		}

		#endregion
	}
}

// File provided for Reference Use Only by Microsoft Corporation (c) 2007.
// Copyright (c) Microsoft Corporation. All rights reserved.