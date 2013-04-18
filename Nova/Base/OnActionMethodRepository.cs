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
using System.Collections.Generic;
using Nova.Controls;
using Nova.Properties;

namespace Nova.Base
{
	/// <summary>
	/// A repository containing the "on-" action methods.
	/// </summary>
	internal static partial class OnActionMethodRepository
	{
	    /// <summary>
	    /// Gets the OnBefore methods.
	    /// </summary>
	    /// <typeparam name="T">The type to get the methods for.</typeparam>
	    /// <param name="actionType">Type of the action.</param>
	    /// <returns>
	    /// A list of OnBefore methods.
	    /// </returns>
	    private static IEnumerable<OnAction> GetOnBeforeMethods<T>(Type actionType)
	    {
	        var entry = GetEntry<T>();
	        return entry.GetRelevantOnBeforeMethods(actionType);
	    }

	    /// <summary>
	    /// Gets the OnAfter methods.
	    /// </summary>
	    /// <typeparam name="T">The type to get the methods for.</typeparam>
	    /// <param name="actionType">Type of the action.</param>
	    /// <returns>
	    /// A list of OnBefore methods.
	    /// </returns>
	    private static IEnumerable<OnAction> GetOnAfterMethods<T>(Type actionType)
	    {
	        var entry = GetEntry<T>();
	        return entry.GetRelevantOnAfterMethods(actionType);
	    }

	    /// <summary>
		/// Called when an action starts its execution.
		/// </summary>
		/// <typeparam name="TAction">The type of action.</typeparam>
		/// <typeparam name="TView">The type of the view.</typeparam>
		/// <typeparam name="TViewModel">The type of the view model.</typeparam>
		/// <param name="action">The action to run.</param>
		public static void OnBefore<TAction, TView, TViewModel>(TAction action)
			where TAction : Actionflow<TView, TViewModel>
			where TViewModel : IViewModel
			where TView : IView
        {
            if (action == null)
                throw new ArgumentNullException("action");

			try
			{
			    var actionType = action.GetType();

                var viewModelMethods = GetOnBeforeMethods<TViewModel>(actionType);
				foreach (var viewModelMethod in viewModelMethods)
				{
					viewModelMethod.Invoke(action.ViewModel, action.ActionContext);
				}

				var viewMethods = GetOnBeforeMethods<TView>(actionType);
				foreach (var viewMethod in viewMethods)
				{
					viewMethod.Invoke(action.View, action.ActionContext);
				}
			}
			catch (Exception exception)
			{
				ExceptionHandler.Handle(exception, Resources.ErrorMessageMainThread);
			}
		}
		
		/// <summary>
		/// Called when an action finishes its execution.
		/// </summary>
		/// <typeparam name="TAction">The type of the action.</typeparam>
		/// <typeparam name="TView">The type of the view.</typeparam>
		/// <typeparam name="TViewModel">The type of the view model.</typeparam>
		/// <param name="action">The action to run.</param>
		public static void OnAfter<TAction, TView, TViewModel>(TAction action) 
			where TAction : Actionflow<TView, TViewModel>
			where TViewModel : IViewModel
			where TView : IView
        {
            if (action == null)
                throw new ArgumentNullException("action");

			try
            {
                var actionType = action.GetType();

                var viewModelMethods = GetOnAfterMethods<TViewModel>(actionType);
				foreach (var viewModelMethod in viewModelMethods)
				{
					viewModelMethod.Invoke(action.ViewModel, action.ActionContext);
                }

				var viewMethods = GetOnAfterMethods<TView>(actionType);
				foreach (var viewMethod in viewMethods)
				{
					viewMethod.Invoke(action.View, action.ActionContext);
				}
			}
			catch (Exception exception)
			{
                ExceptionHandler.Handle(exception, Resources.ErrorMessageMainThread);
            }
		}
    }
}
