using Nova.Base;

namespace Nova.Controls.ExceptionHandler
{
	/// <summary>
	/// The viewmodel for the exception handler.
	/// </summary>
	public class ExceptionHandlerViewModel : BaseViewModel<ExceptionHandlerView, ExceptionHandlerViewModel>
	{
		private string _FormattedMessage;
		/// <summary>
		/// Gets or sets the formatted message.
		/// </summary>
		/// <value>
		/// The formatted message.
		/// </value>
		public string FormattedMessage
		{
			get { return _FormattedMessage; }
			set { SetValue(ref _FormattedMessage, value, "FormattedMessage"); }
		}

		private string _Information;
		/// <summary>
		/// Gets or sets the informational message.
		/// </summary>
		/// <value>
		/// The informational message.
		/// </value>
		public string Information
		{
			get { return _Information; }
			set { SetValue(ref _Information, value, () => Information); }
		}
	}
}
