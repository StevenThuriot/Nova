using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using Nova.Controls.ExceptionHandler;
using Nova.Properties;
using RESX=Nova.Properties.Resources;

namespace Nova.Base
{
	/// <summary>
	/// The Exception Handler
	/// </summary>
	public static class ExceptionHandler
	{
		private static readonly StringBuilder Builder = new StringBuilder();

		private static bool _LogStackTrace = true;
		private static bool _ShowExceptionInfo = true;

		/// <summary>
		/// Gets or sets a value indicating whether to [log stack trace].
		/// </summary>
		/// <value>
		///   <c>true</c> if [log stack trace]; otherwise, <c>false</c>.
		/// </value>
		public static bool LogStackTrace
		{
			get { return _LogStackTrace; }
			set { _LogStackTrace = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether to [show stack trace] in the exception handler window.
		/// </summary>
		/// <value>
		///   <c>true</c> if [show stack trace]; otherwise, <c>false</c>.
		/// </value>
		public static bool ShowStackTrace { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether to [show exception info].
		/// </summary>
		/// <value>
		///   <c>true</c> if [show exception info]; otherwise, <c>false</c>.
		/// </value>
		public static bool ShowExceptionInfo
		{
			get { return _ShowExceptionInfo; }
			set { _ShowExceptionInfo = value; }
		}

		/// <summary>
		/// Gets the exception info visibility.
		/// </summary>
		public static Visibility ExceptionInfoVisibility
		{
			get { return ShowExceptionInfo ? Visibility.Visible : Visibility.Collapsed; }
		}

		/// <summary>
		/// Handles the specified exception.
		/// </summary>
		/// <param name="exception">The exception.</param>
		/// <param name="title">The title.</param>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public static void Handle(Exception exception, string title)
		{
			Handle(exception, title, null);
		}

		/// <summary>
		/// Handles the specified exception.
		/// </summary>
		/// <param name="exception">The exception.</param>
		/// <param name="title">The title.</param>
		/// <param name="informationalMessage">The informational message.</param>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static void Handle(Exception exception, string title, string informationalMessage)
		{
			var handler = new Action(() =>
			                         	{
			                         		//TryCatch is here to prevent getting into a loop in case the eventhandling throws an exception.
											try
											{
												string message;
												if (ShowStackTrace)
													message = FormatMessage(exception);
												else
												{
													FormatException(exception);
													message = Builder.ToString();
													Builder.Length = 0;
												}

												Log(exception);

												using (var exceptionHandlerWindow = new ExceptionHandlerWindow(title, message, informationalMessage))
												{
													exceptionHandlerWindow.ShowDialog();
												}
											}
											catch (Exception ex)
											{
												Log(ex);
											}
			                         	});

			var dispatcher = Application.Current.Dispatcher;

			if (dispatcher.CheckAccess())
			{
				handler();
			}
			else
			{
				dispatcher.BeginInvoke(DispatcherPriority.Send, handler);
			}
		}

		/// <summary>
		/// Handles unhandled exceptions in the dispatcher thread.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.Threading.DispatcherUnhandledExceptionEventArgs"/> instance containing the event data.</param>
		internal static void DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			Handle(e.Exception, RESX.UnhandledDispatcherException);
			e.Handled = true;
		}

		/// <summary>
		/// Handles unhandled exceptions in threads different from the dispatcher thread.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.UnhandledExceptionEventArgs"/> instance containing the event data.</param>
		internal static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			var exception = e.ExceptionObject as Exception;
			if (exception == null) return;

			Handle(exception, RESX.UnhandledException);
		}

		/// <summary>
		/// Logs the specified exception
		/// </summary>
		/// <param name="exception">Exception</param>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
			Justification = "Most likely don't have logging rights, we don't want to crash because we can't log an unhandled exception...")]
		public static void Log(Exception exception)
		{
			try
			{
				string message;

				if (LogStackTrace)
					message = FormatMessage(exception);
				else
				{
					FormatException(exception);
					message = Builder.ToString();
					Builder.Length = 0;
				}

				Log(message);
			}
			catch
			{
				//Something went wrong trying to log, can't do much about this...
				//Either catch all or crash, which logging isn't worth.
			}
		}

		/// <summary>
		/// Logs the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Seems to be a false positive.")]
		public static void Log(string message)
		{
			string application = Assembly.GetEntryAssembly().GetName().Name;

			if (!EventLog.SourceExists(application))
				EventLog.CreateEventSource(application, "Application");
		
			using (var eventLog = new EventLog { Source = application })
			{
				eventLog.WriteEntry(message, EventLogEntryType.Error);
			}
		}

		/// <summary>
		/// Formats the message.
		/// </summary>
		/// <param name="exception">The exception.</param>
		/// <returns></returns>
		private static string FormatMessage(Exception exception)
		{
			FormatException(exception);

			Builder.AppendLine("\"")
				.AppendLine()
				.AppendLine()
				.AppendLine()
				.AppendLine("StackTrace:")
				.AppendLine("---------------")
				.AppendLine()
				.AppendLine(exception.StackTrace);

			var message = Builder.ToString();
			Builder.Length = 0;

			return message;
		}


		/// <summary>
		/// Formats the exception.
		/// </summary>
		/// <param name="exception">The exception.</param>
		private static void FormatException(Exception exception)
		{
			Builder.AppendFormat(CultureInfo.CurrentCulture, Resources.ExceptionInfo, exception.Message);

			if (exception.InnerException == null)
			{
				return;
			}

			Builder.AppendLine()
				.AppendLine()
				.AppendLine()
				.Append(Resources.ErrorDetail)
				.Append(":")
				.AppendLine()
				.AppendLine()
				.Append("\"");


			FormatExceptionDetail(exception.InnerException);
		}

		/// <summary>
		/// Formats the exception detail.
		/// </summary>
		/// <param name="exception">The exception.</param>
		private static void FormatExceptionDetail(Exception exception)
		{
			Builder.Append(exception.Message);

			if (exception.InnerException == null) return;

			Builder.Append(", ")
				.AppendLine()
				.AppendLine();

			FormatExceptionDetail(exception.InnerException);
		}
	}
}