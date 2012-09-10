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
