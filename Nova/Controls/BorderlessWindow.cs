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
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Nova.Base;
using System.Diagnostics.CodeAnalysis;

namespace Nova.Controls
{
	/// <summary>
	/// Base class for borderless windows.
	/// </summary>
	/// <typeparam name="TView">The type of view.</typeparam>
	/// <typeparam name="TViewModel">The type of ViewModel.</typeparam>
	public abstract class BorderlessWindow<TView, TViewModel> : ExtendedWindow<TView, TViewModel>
		where TViewModel : BaseViewModel<TView, TViewModel>, new()
        where TView : BorderlessWindow<TView, TViewModel>, IView
	{
		private readonly AnimatedLoader _AnimatedLoader;
		private Menu _Menu;
		private WindowControlBox _WindowControlBox;

		#region Initialization

		/// <summary>
		/// Initializes a new instance of the <see cref="BorderlessWindow&lt;TView, TViewModel&gt;"/> class.
		/// </summary>
		protected BorderlessWindow()
		{
			WindowStyle = WindowStyle.None;
			Background = Brushes.Transparent;
			AllowsTransparency = true;
			MouseLeftButtonDown += DragWindow;

		    SetResourceReference(BackgroundImageProperty, "WindowBackgroundImageBrush");
			
			_AnimatedLoader = new AnimatedLoader { Margin = new Thickness(0, 10, 10, 0) };
			
			InitializeWindowControlBox();
			InitializeMenu();
		}

		/// <summary>
		/// Initializes the menu.
		/// </summary>
		private void InitializeMenu()
		{
			_Menu = new Menu
						{
							Margin = new Thickness(10, 10, 0, 0)
						};

			_Menu.SetValue(Grid.RowProperty, 0);
		}

		/// <summary>
		/// Initializes the window control box.
		/// </summary>
		private void InitializeWindowControlBox()
		{
			_WindowControlBox = new WindowControlBox(this)
									{
										CanMaximize = false,
										Margin = new Thickness(0, 10, 10, 0)
									};
			_WindowControlBox.SetValue(Grid.RowProperty, 0);
		}
		
		/// <summary>
		/// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally.
		/// </summary>
		/// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			var content = new ContentPresenter { Content = Content };

			Image logo = CreateLogo();
			Grid topGrid = CreateTopGrid(logo);
			Grid grid = CreateMainGrid(content, topGrid);

			Binding radiusBinding = CreateNewBinding("WindowRadius");

			var innerBorder = new Border { Child = grid };
			Binding backgroundImageBinding = CreateNewBinding("BackgroundImage");
			innerBorder.SetBinding(Border.BackgroundProperty, backgroundImageBinding);
			innerBorder.SetBinding(Border.CornerRadiusProperty, radiusBinding);

			var outerBorder = new Border
								{
									Margin = new Thickness(10, 14, 14, 10),
									BorderThickness = new Thickness(1),
									Child = innerBorder
								};
			outerBorder.SetBinding(Border.CornerRadiusProperty, radiusBinding);
			outerBorder.SetResourceReference(Border.BackgroundProperty, "BackgroundBrush");
			outerBorder.SetResourceReference(Border.BorderBrushProperty, "OutsideBorderBrush");
			outerBorder.SetResourceReference(EffectProperty, "ShadowEffect");

			Content = outerBorder;
		}

		/// <summary>
		/// Creates the main grid.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="topGrid">The top grid.</param>
		/// <returns>A new instance for the main grid.</returns>
		private static Grid CreateMainGrid(UIElement content, Grid topGrid)
		{
			content.SetValue(Grid.RowProperty, 1);
			content.SetValue(Grid.ColumnProperty, 1);

			topGrid.SetValue(Grid.ColumnSpanProperty, 3);

			var grid = new Grid
						{
							RowDefinitions =
			           			{
			           				new RowDefinition {Height = new GridLength(40)},
			           				new RowDefinition {Height = new GridLength(1, GridUnitType.Star)},
			           				new RowDefinition {Height = new GridLength(10)}
			           			},
							ColumnDefinitions =
			           			{
			           				new ColumnDefinition {Width = new GridLength(10)},
			           				new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star)},
			           				new ColumnDefinition {Width = new GridLength(10)}
			           			},
							Children = { topGrid, content }
						};

			return grid;
		}

		/// <summary>
		/// Creates the top grid.
		/// </summary>
		/// <param name="logo">The logo.</param>
		/// <returns>A new instance for the top grid.</returns>
		private Grid CreateTopGrid(UIElement logo)
		{
			var menuPanel = new StackPanel
								{
									HorizontalAlignment = HorizontalAlignment.Left,
									VerticalAlignment = VerticalAlignment.Top,
									Orientation = Orientation.Horizontal,
									Children = { logo, _Menu, }
								};
			var controlBoxPanel = new StackPanel
									{
										HorizontalAlignment = HorizontalAlignment.Right,
										VerticalAlignment = VerticalAlignment.Top,
										Orientation = Orientation.Horizontal,
										Children = { _AnimatedLoader, _WindowControlBox }
									};
			var topGrid = new Grid
							{
								Children = { menuPanel, controlBoxPanel }
							};

			return topGrid;
		}

		/// <summary>
		/// Creates the logo.
		/// </summary>
		/// <returns>A new instance for a logo.</returns>
		private Image CreateLogo()
		{
			var logo = new Image
						{
							Height = 20,
							Margin = new Thickness(10, 10, -5, 0)
						};
			Binding logoBinding = CreateNewBinding("Logo");
			logo.SetBinding(Image.SourceProperty, logoBinding);
			return logo;
		}

		/// <summary>
		/// Creates a new binding.
		/// </summary>
		/// <param name="path">The path to bind to.</param>
		/// <returns>A new binding.</returns>
		private Binding CreateNewBinding(string path)
		{
			return new Binding(path)
					{
						Source = this,
						UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
						NotifyOnSourceUpdated = true,
						NotifyOnTargetUpdated = true
					};
		}

		#endregion Initialization

		#region Dependency Properties

		// ReSharper disable StaticFieldInGenericType
		/// <summary>
		/// The logo property.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static readonly DependencyProperty LogoProperty =
			DependencyProperty.Register("Logo", typeof(ImageSource), typeof(BorderlessWindow<TView, TViewModel>));

		/// <summary>
		/// The BackgroundImage property.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static readonly DependencyProperty BackgroundImageProperty =
			DependencyProperty.Register("BackgroundImage", typeof(Brush), typeof(BorderlessWindow<TView, TViewModel>), new PropertyMetadata(new SolidColorBrush(Colors.White)));

		/// <summary>
		/// The window radius property.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static readonly DependencyProperty WindowRadiusProperty =
			DependencyProperty.Register("WindowRadius", typeof(CornerRadius), typeof(BorderlessWindow<TView, TViewModel>), new PropertyMetadata(new CornerRadius(2, 14, 14, 14)));


		/// <summary>
		/// Gets or sets the logo.
		/// </summary>
		/// <value>
		/// The logo.
		/// </value>
		public ImageSource Logo
		{
			get { return (ImageSource)GetValue(LogoProperty); }
			set { SetValue(LogoProperty, value); }
		}

		/// <summary>
		/// Gets or sets the background image.
		/// </summary>
		/// <value>
		/// The background image.
		/// </value>
		public Brush BackgroundImage
		{
			get { return GetValue(BackgroundImageProperty) as Brush; }
			set { SetValue(BackgroundImageProperty, value); }
		}

		/// <summary>
		/// Gets or sets the window radius.
		/// </summary>
		/// <value>
		/// The window radius.
		/// </value>
		public CornerRadius WindowRadius
		{
			get { return (CornerRadius)GetValue(WindowRadiusProperty); }
			set { SetValue(WindowRadiusProperty, value); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance can maximize.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance can maximize; otherwise, <c>false</c>.
		/// </value>
		public bool CanMaximize
		{
			get { return _WindowControlBox.CanMaximize; }
			set { _WindowControlBox.CanMaximize = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether to close this window when cancel is pressed.
		/// </summary>
		/// <value>
		///   <c>true</c> if [close on cancel]; otherwise, <c>false</c>.
		/// </value>
		public bool CloseOnCancel
		{
			get { return _WindowControlBox.IsCancel; }
			set { _WindowControlBox.IsCancel = value; }
		}

		/// <summary>
		/// Gets or sets the menu.
		/// </summary>
		/// <value>
		/// The menu.
		/// </value>
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public ItemCollection Menu
		{
			get { return _Menu.Items; }
			set
			{
				_Menu.Items.Clear();
				foreach (object menuItem in value)
				{
					_Menu.Items.Add(menuItem);
				}
			}
		}

		// ReSharper restore StaticFieldInGenericType

		#endregion Dependency Properties

		#region  Methods

		/// <summary>
		/// Logic for window dragging.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
		private void DragWindow(object sender, MouseButtonEventArgs e)
		{
			DragMove();
		}

		/// <summary>
		/// Starts the animated loading.
		/// </summary>
		public override void StartLoading()
		{
			base.StartLoading();
			_AnimatedLoader.Start();
		}

		/// <summary>
		/// Stops the animated loading.
		/// </summary>
		public override void StopLoading()
		{
			base.StopLoading();
			_AnimatedLoader.Stop();
		}

		#endregion  Methods
	}
}