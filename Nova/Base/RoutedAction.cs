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
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using Nova.Controls;
using Nova.Properties;

namespace Nova.Base
{
	/// <summary>
	/// Helper class to create a new action that can be used as a command.
	/// </summary>
	public static class RoutedAction
	{
		/// <summary>
		/// The key used to inject the ICommand parameter into the actioncontext.
		/// </summary>
        public const string CommandParameter = "#RoutedAction.CommandParameter#";

		/// <summary>
		/// Creates a new action with a wrapper which implements the ICommand interface.
		/// This will try to search for an action controller on the view. If not present, an exception will be thrown.
		/// </summary>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <typeparam name="TView">The type of the view.</typeparam>
		/// <typeparam name="TViewModel">The type of the view model.</typeparam>
		/// <param name="view">The view.</param>
		/// <param name="viewModel">The view model.</param>
		/// <returns>A new action with a wrapper which implements the ICommand interface</returns>
		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"), 
		 SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes"),
		 SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes"), 
		 SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Need TResult to know which action to create.")]
		public static ICommand New<TResult, TView, TViewModel>(TView view, TViewModel viewModel)
			where TView : class, IView
			where TViewModel : BaseViewModel<TView, TViewModel>, new()
			where TResult : BaseAction<TView, TViewModel>, new()
		{
			ICommand routedAction = null;

			try
			{
				routedAction = new RoutedAction<TResult, TView, TViewModel>(view, viewModel);
			}
			catch (Exception exception)
			{
				ExceptionHandler.Handle(exception, Resources.ErrorMessageAction);
			}

			return routedAction;
		}
		
		/// <summary>
		/// Creates a new action with a wrapper which implements the ICommand interface.
		/// </summary>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <typeparam name="TView">The type of the view.</typeparam>
		/// <typeparam name="TViewModel">The type of the view model.</typeparam>
		/// <param name="view">The view.</param>
		/// <param name="viewModel">The view model.</param>
		/// <param name="actionController">The action controller.</param>
		/// <returns>A new action with a wrapper which implements the ICommand interface</returns>
		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"), SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes"), 
		 SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Need TResult to know which action to create.")]
		public static ICommand New<TResult, TView, TViewModel>(TView view, TViewModel viewModel, ActionController<TView, TViewModel> actionController)
			where TView : class, IView
			where TViewModel : BaseViewModel<TView, TViewModel>, new()
			where TResult : BaseAction<TView, TViewModel>, new()
		{
			ICommand routedAction = null;

			try
			{
				routedAction = new RoutedAction<TResult, TView, TViewModel>(view, viewModel, actionController);
			}
			catch (Exception exception)
			{
				ExceptionHandler.Handle(exception, Resources.ErrorMessageAction);
			}

			return routedAction;
		}
	}
}