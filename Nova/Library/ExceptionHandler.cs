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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Nova.Controls.ExceptionHandler;
using Nova.Properties;
using System.Text;

namespace Nova.Library
{
	/// <summary>
	/// The Exception Handler
	/// </summary>
	public static class ExceptionHandler
    {
        private static readonly StringBuilder Builder;

        /// <summary>
        /// Initializes the <see cref="ExceptionHandler" /> class.
        /// </summary>
	    static ExceptionHandler()
        {
            Builder = new StringBuilder();
            DefaultExceptionHandler = InternalHandle;
            LogStackTrace = true;
            ShowExceptionInfo = true;
        }

	    /// <summary>
        /// Gets or sets the exception handler.
        /// </summary>
        /// <value>
        /// The exception handler.
        /// </value>
        public static Action<Exception, string, string> HandleException { get; set; }

        /// <summary>
        /// The default exception handler
        /// </summary>
        public static readonly Action<Exception, string, string> DefaultExceptionHandler;

	    /// <summary>
	    /// Gets or sets a value indicating whether to [log stack trace].
	    /// </summary>
	    /// <value>
	    ///   <c>true</c> if [log stack trace]; otherwise, <c>false</c>.
	    /// </value>
	    public static bool LogStackTrace { get; set; }

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
	    public static bool ShowExceptionInfo { get; set; }

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
        /// <param name="informationalMessage">The informational message.</param>
        public static void Handle(Exception exception, string title, string informationalMessage = null)
	    {
	        var handler = HandleException ?? DefaultExceptionHandler;
	        handler(exception, title, informationalMessage);
	    }

	    /// <summary>
		/// Handles the specified exception.
		/// </summary>
		/// <param name="exception">The exception.</param>
		/// <param name="title">The title.</param>
		/// <param name="informationalMessage">The informational message.</param>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static void InternalHandle(Exception exception, string title, string informationalMessage)
		{
			var handler = new Action(() =>
			                         	{
			                         		//TryCatch is here to prevent getting into a loop in case the eventhandling throws an exception.
											try
											{
                                                var aggregateException = exception as AggregateException;

											    Action<Exception> format;
											    
                                                if (ShowStackTrace)
											    {
											        format = FormatMessage;
											    }
											    else
											    {
											        format = FormatException;
											    }

											    format(exception);

                                                if (aggregateException != null)
                                                {
                                                    aggregateException.Handle(x =>
                                                        {
                                                            Builder.AppendLine(Resources.InnerExceptions);
                                                            format(x);
                                                            return true; //Set handled to true to keep the TPL happy.
                                                        });
                                                }

											    var message = Builder.ToString();
                                                Builder.Clear();

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
			Handle(e.Exception, Resources.UnhandledDispatcherException);
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

			Handle(exception, Resources.UnhandledException);
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
				{
				    FormatMessage(exception);
				    message = Builder.ToString();
				    
                    Builder.Clear();
				}
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
		private static void FormatMessage(Exception exception)
		{
			FormatException(exception);

			Builder.AppendLine("\"")
				.AppendLine()
				.AppendLine()
				.AppendLine();

            
		    var stackTrace = exception.StackTrace;

		    if (!string.IsNullOrWhiteSpace(stackTrace))
		    {
		        Builder.AppendLine("StackTrace:")
		               .AppendLine("---------------")
		               .AppendLine()
		               .AppendLine(stackTrace);
		    }
		}


		/// <summary>
		/// Formats the exception.
		/// </summary>
		/// <param name="exception">The exception.</param>
		private static void FormatException(Exception exception)
		{
			Builder.AppendFormat(CultureInfo.CurrentCulture, Resources.ExceptionInfo, exception.GetType(), exception.Message);

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