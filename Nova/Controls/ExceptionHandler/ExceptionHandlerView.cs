namespace Nova.Controls.ExceptionHandler
{
	/// <summary>
	/// The view for the exception handler.
	/// </summary>
	public class ExceptionHandlerView : BorderlessWindow<ExceptionHandlerView, ExceptionHandlerViewModel>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ExceptionHandlerView"/> class.
		/// </summary>
		/// <param name="title">The title.</param>
		/// <param name="exceptionMessage">The exception message.</param>
		public ExceptionHandlerView(string title, string exceptionMessage)
			: this (title, exceptionMessage, Properties.Resources.UnexpectedError)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ExceptionHandlerView"/> class.
		/// </summary>
		/// <param name="title">The title.</param>
		/// <param name="exceptionMessage">The exception message.</param>
		/// <param name="informationalMessage">The informational message.</param>
		public ExceptionHandlerView(string title, string exceptionMessage, string informationalMessage)
		{
			Title = title;
			ViewModel.FormattedMessage = exceptionMessage;
			ViewModel.Information = string.IsNullOrWhiteSpace(informationalMessage)
			                        	? Properties.Resources.UnexpectedError
			                        	: informationalMessage;
		}
	}
}
