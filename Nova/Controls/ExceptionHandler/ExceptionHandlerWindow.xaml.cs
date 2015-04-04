using System.Windows;

namespace Nova.Controls.ExceptionHandler
{
	/// <summary>
	/// Interaction logic for ExceptionHandlerWindow.xaml
	/// </summary>
	public partial class ExceptionHandlerWindow
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ExceptionHandlerWindow"/> class.
		/// </summary>
		/// <param name="title">The title.</param>
		/// <param name="exceptionMessage">The exception message.</param>
		public ExceptionHandlerWindow(string title, string exceptionMessage)
			: base(title, exceptionMessage)
		{
			InitializeComponent();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ExceptionHandlerWindow"/> class.
		/// </summary>
		/// <param name="title">The title.</param>
		/// <param name="exceptionMessage">The exception message.</param>
		/// <param name="informationalMessage"> </param>
		public ExceptionHandlerWindow(string title, string exceptionMessage, string informationalMessage)
			: base(title, exceptionMessage, informationalMessage)
		{
			InitializeComponent();
		}

		private void CloseWindow(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}
