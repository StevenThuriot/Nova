using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Nova.Library;

namespace Nova
{
	/// <summary>
	/// Entry point for the Nova Framework.
	/// </summary>
	public partial class NovaFramework
	{
		/// <summary>
		/// The default spacer size.
		/// </summary>
		public static readonly GridLength DefaultSpacerGridLength = new GridLength(10);
		
		/// <summary>
		/// Prevents a default instance of the <see cref="Nova"/> class from being created.
		/// </summary>
		private NovaFramework()
		{
			InitializeComponent();
		}
        
	    /// <summary>
		/// StartUp event handler for applications to use.
		/// Loads Nova's resource dictionaries into the application's resource dictionary.
		/// </summary>
		/// <param name="sender">The application</param>
		/// <param name="startupEventArgs">Start up event arguments. These aren't used.</param>
		public static void Initialize(object sender, StartupEventArgs startupEventArgs)
		{
			var app = sender as Application;
			if (app == null) return;

			app.Startup -= Initialize;

			var currentThread = Thread.CurrentThread;
			if (!string.IsNullOrEmpty(currentThread.Name))
			{
				currentThread.Name = "Nova GUI Thread";
			}

	        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CurrentCulture;
	        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CurrentUICulture;
            
            app.DispatcherUnhandledException += ExceptionHandler.DispatcherUnhandledException;
			AppDomain.CurrentDomain.UnhandledException += ExceptionHandler.UnhandledException;

			InitiateEventHandlers();

			var nova = new NovaFramework();

			foreach (var dictionary in nova.MergedDictionaries)
			{
				app.Resources.MergedDictionaries.Add(dictionary);
			}
		}
        
		/// <summary>
		/// Initiates the event handlers.
		/// </summary>
		private static void InitiateEventHandlers()
		{
			EventManager.RegisterClassHandler(typeof(TextBox), UIElement.GotKeyboardFocusEvent, new RoutedEventHandler(BoxGotFocus));
			EventManager.RegisterClassHandler(typeof(TextBox), UIElement.PreviewMouseDownEvent, new MouseButtonEventHandler(SelectivelyIgnoreMouseButton));

			EventManager.RegisterClassHandler(typeof(PasswordBox), UIElement.GotKeyboardFocusEvent, new RoutedEventHandler(BoxGotFocus));
			EventManager.RegisterClassHandler(typeof(PasswordBox), UIElement.PreviewMouseDownEvent, new MouseButtonEventHandler(SelectivelyIgnoreMouseButton));
		}

		/// <summary>
		/// Triggers when the textbox receives focus.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private static void BoxGotFocus(object sender, RoutedEventArgs e)
		{
			var textBox = (sender as TextBox);
			if (textBox != null)
			{
				textBox.SelectAll();
				return;
			}

			var passwordBox = (sender as PasswordBox);
			if (passwordBox != null)
			{
				passwordBox.SelectAll();
			}
		}

		/// <summary>
		/// Selectively ignores the mouse button.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
		private static void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
		{
			var textBox = (sender as TextBox);
			if (textBox != null && !textBox.IsKeyboardFocusWithin)
			{
				e.Handled = true;
				textBox.Focus();
				return;
			}

			var passwordBox = (sender as PasswordBox);
			if (passwordBox != null)
			{
				e.Handled = true;
				passwordBox.Focus();
			}
		}
	}
}