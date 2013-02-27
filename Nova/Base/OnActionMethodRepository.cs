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
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using Nova.Controls;
using Nova.Properties;

namespace Nova.Base
{
	/// <summary>
	/// A repository containing the "on-" action methods.
	/// </summary>
	internal static partial class OnActionMethodRepository
	{
        private static readonly Mutex Lock = new Mutex();

		/// <summary>
		/// A cache for View and ViewModel OnBefore methods so they can be called through reflection.
		/// </summary>
		private static readonly IDictionary<Type, IEnumerable<OnAction>> OnBeforeMethods = new Dictionary<Type, IEnumerable<OnAction>>();

		/// <summary>
		/// A cache for View and ViewModel OnAfter methods so they can be called through reflection.
		/// </summary>
		private static readonly IDictionary<Type, IEnumerable<OnAction>> OnAfterMethods = new Dictionary<Type, IEnumerable<OnAction>>();

	    /// <summary>
	    /// Gets the OnBefore methods for a defined type.
	    /// </summary>
	    /// <typeparam name="TView">The viewmodel</typeparam>
	    /// <param name="actionType">The type of the action.</param>
	    /// <returns>A read-only list of methods.</returns>
	    private static IEnumerable<OnAction> GetOnBeforeViewMethods<TView>(Type actionType)
			where TView : IView
	    {
			var onBeforeViewMethods = GetOnBeforeMethods<TView>();
            return GetRelevantMethods(actionType, "OnBefore", onBeforeViewMethods);
		}

	    /// <summary>
	    /// Gets the OnBefore methods for a defined type.
	    /// </summary>
	    /// <typeparam name="TViewModel">The view</typeparam>
	    /// <param name="actionType">The type of the action.</param>
	    /// <returns>A read-only list of methods.</returns>
	    private static IEnumerable<OnAction> GetOnBeforeViewModelMethods<TViewModel>(Type actionType) 
            where TViewModel : IViewModel
		{
			var onBeforeViewModelMethods = GetOnBeforeMethods<TViewModel>();
            return GetRelevantMethods(actionType, "OnBefore", onBeforeViewModelMethods);
		}

	    /// <summary>
	    /// Gets the OnAfter methods for a defined type.
	    /// </summary>
	    /// <typeparam name="TView">The viewmodel</typeparam>
	    /// <param name="actionType">The type of the action.</param>
	    /// <returns>A read-only list of methods.</returns>
	    private static IEnumerable<OnAction> GetOnAfterViewMethods<TView>(Type actionType)
			where TView : IView
	    {
			var onAfterViewMethods = GetOnAfterMethods<TView>();
			return GetRelevantMethods(actionType, "OnAfter", onAfterViewMethods);
		}

	    /// <summary>
	    /// Gets the OnAfter methods for a defined type.
	    /// </summary>
	    /// <typeparam name="TViewModel">The view</typeparam>
	    /// <param name="actionType">The type of the action.</param>
	    /// <returns>A read-only list of methods.</returns>
	    private static IEnumerable<OnAction> GetOnAfterViewModelMethods<TViewModel>(Type actionType) 
            where TViewModel : IViewModel
		{
			var onAfterViewModelMethods = GetOnAfterMethods<TViewModel>();
			return GetRelevantMethods(actionType, "OnAfter", onAfterViewModelMethods);
		}


		/// <summary>
		/// Gets the relevant methods.
		/// </summary>
		/// <param name="actionType">The type of the action.</param>
		/// <param name="prefix">The OnBefore or OnAfter prefix, depending on the situation.</param>
		/// <param name="methods">The methods.</param>
		/// <returns></returns>
		private static IEnumerable<OnAction> GetRelevantMethods(Type actionType, string prefix, IEnumerable<OnAction> methods)
		{
		    var typeName = actionType.Name;
		    var generic = typeName.IndexOf('`');
            if (generic > 0)
            {
                typeName = typeName.Substring(0, generic);
            }

		    var name = prefix + typeName.ToString(CultureInfo.InvariantCulture);
			if (name.EndsWith("action", StringComparison.OrdinalIgnoreCase))
			{
				var shortName = name.Substring(0, name.Length - 6);

				return methods.Where(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase) ||
										  string.Equals(x.Name, shortName, StringComparison.OrdinalIgnoreCase));
			}

			return methods.Where(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
		}


	    /// <summary>
	    /// Gets the OnBefore methods.
	    /// </summary>
	    /// <typeparam name="T">The type to get the methods for.</typeparam>
	    /// <returns>A list of OnBefore methods.</returns>
	    private static IEnumerable<OnAction> GetOnBeforeMethods<T>() 
	    {
	        lock (Lock)
	        {
	            IEnumerable<OnAction> methods;
	            return OnBeforeMethods.TryGetValue(typeof (T), out methods)
	                       ? methods
	                       : CacheMethods<T>().Item1;
	        }
	    }

	    /// <summary>
		/// Gets the OnAfter methods.
		/// </summary>
		/// <typeparam name="T">The type to get the methods for.</typeparam>
		/// <returns>A list of OnBefore methods.</returns>
		private static IEnumerable<OnAction> GetOnAfterMethods<T>() 
		{
	        lock (Lock)
	        {
	            IEnumerable<OnAction> methods;
	            return OnAfterMethods.TryGetValue(typeof (T), out methods)
	                       ? methods
	                       : CacheMethods<T>().Item2;
	        }
		}

		/// <summary>
		/// Caches the methods.
		/// </summary>
		/// <typeparam name="T">The type to cache the methods for.</typeparam>
		private static Tuple<IEnumerable<OnAction>, IEnumerable<OnAction>> CacheMethods<T>() 
			
		{
		    lock (Lock)
		    {
		        var type = typeof (T);

		        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

		        var onBeforeMethods = methods.Where(x => x.Name.StartsWith("onbefore", StringComparison.OrdinalIgnoreCase))
		                                     .Select(OnAction.Create<T>)
		                                     .Where(x => x != null)
		                                     .ToList();

		        var onAfterMethods = methods.Where(x => x.Name.StartsWith("onafter", StringComparison.OrdinalIgnoreCase))
		                                    .Select(OnAction.Create<T>)
		                                    .Where(x => x != null)
		                                    .ToList();

		        OnBeforeMethods.Add(type, onBeforeMethods);
		        OnAfterMethods.Add(type, onAfterMethods);

		        return new Tuple<IEnumerable<OnAction>, IEnumerable<OnAction>>(onBeforeMethods, onAfterMethods);
		    }
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

				var viewMethods = GetOnBeforeViewMethods<TView>(actionType);
				foreach (var viewMethod in viewMethods)
				{
					viewMethod.Invoke(action.View, action.ActionContext);
				}

                var viewModelMethods = GetOnBeforeViewModelMethods<TViewModel>(actionType);
				foreach (var viewModelMethod in viewModelMethods)
				{
					viewModelMethod.Invoke(action.ViewModel, action.ActionContext);
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

				var viewMethods = GetOnAfterViewMethods<TView>(actionType);
				foreach (var viewMethod in viewMethods)
				{
					viewMethod.Invoke(action.View, action.ActionContext);
				}

                var viewModelMethods = GetOnAfterViewModelMethods<TViewModel>(actionType);
				foreach (var viewModelMethod in viewModelMethods)
				{
					viewModelMethod.Invoke(action.ViewModel, action.ActionContext);
                }
			}
			catch (Exception exception)
			{
                ExceptionHandler.Handle(exception, Resources.ErrorMessageMainThread);
            }
		}
	}
}
