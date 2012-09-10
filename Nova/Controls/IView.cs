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
using System.Windows.Threading;

namespace Nova.Controls
{
	/// <summary>
	/// The interface for a view used in this framework.
	/// </summary>
	public interface IView
	{
		/// <summary>
		/// Starts the loading.
		/// </summary>
		void StartLoading();

		/// <summary>
		/// Stops the loading.
		/// </summary>
		void StopLoading();

		/// <summary>
		/// Invokes the specified action on the main thread.
		/// </summary>
		/// <param name="work">The work.</param>
		void InvokeOnMainThread(Action work);

		/// <summary>
		/// Invokes the specified action on the main thread.
		/// </summary>
		/// <param name="work">The work.</param>
		/// <param name="priority">The priority.</param>
		void InvokeOnMainThread(Action work, DispatcherPriority priority);
	}
}