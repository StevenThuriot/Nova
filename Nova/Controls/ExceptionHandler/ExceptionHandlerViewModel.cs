using Nova.Library;

namespace Nova.Controls.ExceptionHandler
{
	/// <summary>
	/// The viewmodel for the exception handler.
	/// </summary>
	public class ExceptionHandlerViewModel : ViewModel<ExceptionHandlerView, ExceptionHandlerViewModel>
	{
		private string _formattedMessage;
		/// <summary>
		/// Gets or sets the formatted message.
		/// </summary>
		/// <value>
		/// The formatted message.
		/// </value>
		public string FormattedMessage
		{
			get { return _formattedMessage; }
			set { SetValue(ref _formattedMessage, value); }
		}

		private string _information;
		/// <summary>
		/// Gets or sets the informational message.
		/// </summary>
		/// <value>
		/// The informational message.
		/// </value>
		public string Information
		{
			get { return _information; }
			set { SetValue(ref _information, value); }
		}
	}
}
