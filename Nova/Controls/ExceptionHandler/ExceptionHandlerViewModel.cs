using Nova.Library;

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
	/// The viewmodel for the exception handler.
	/// </summary>
	public class ExceptionHandlerViewModel : ViewModel<ExceptionHandlerView, ExceptionHandlerViewModel>
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
			set { SetValue(ref _FormattedMessage, value); }
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
			set { SetValue(ref _Information, value); }
		}
	}
}
