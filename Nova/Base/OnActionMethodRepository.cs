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
using Nova.Controls;

namespace Nova.Base
{
	/// <summary>
	/// A repository containing the "on-" action methods.
	/// </summary>
	internal static partial class OnActionMethodRepository
	{
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
		/// <typeparam name="TViewModel">The view</typeparam>
		/// <typeparam name="TView">The viewmodel</typeparam>
		/// <typeparam name="TAction">The action</typeparam>
		/// <returns>A read-only list of methods.</returns>
		private static IEnumerable<OnAction> GetOnBeforeViewMethods<TView, TViewModel, TAction>()
			where TView : class, IView
			where TViewModel : BaseViewModel<TView, TViewModel>, new()
			where TAction : BaseAction<TView, TViewModel>
		{
			var onBeforeViewMethods = GetOnBeforeMethods<TView>();
			return GetRelevantMethods<TAction>("OnBefore", onBeforeViewMethods);
		}

		/// <summary>
		/// Gets the OnBefore methods for a defined type.
		/// </summary>
		/// <typeparam name="TViewModel">The view</typeparam>
		/// <typeparam name="TView">The viewmodel</typeparam>
		/// <typeparam name="TAction">The action</typeparam>
		/// <returns>A read-only list of methods.</returns>
		private static IEnumerable<OnAction> GetOnBeforeViewModelMethods<TView, TViewModel, TAction>()
			where TView : class, IView
			where TViewModel : BaseViewModel<TView, TViewModel>, new()
			where TAction : BaseAction<TView, TViewModel>
		{
			var onBeforeViewModelMethods = GetOnBeforeMethods<TViewModel>();
			return GetRelevantMethods<TAction>("OnBefore", onBeforeViewModelMethods);
		}

		/// <summary>
		/// Gets the OnAfter methods for a defined type.
		/// </summary>
		/// <typeparam name="TViewModel">The view</typeparam>
		/// <typeparam name="TView">The viewmodel</typeparam>
		/// <typeparam name="TAction">The action</typeparam>
		/// <returns>A read-only list of methods.</returns>
		private static IEnumerable<OnAction> GetOnAfterViewMethods<TView, TViewModel, TAction>()
			where TView : class, IView
			where TViewModel : BaseViewModel<TView, TViewModel>, new()
			where TAction : BaseAction<TView, TViewModel>
		{
			var onAfterViewMethods = GetOnAfterMethods<TView>();
			return GetRelevantMethods<TAction>("OnAfter", onAfterViewMethods);
		}

		/// <summary>
		/// Gets the OnAfter methods for a defined type.
		/// </summary>
		/// <typeparam name="TViewModel">The view</typeparam>
		/// <typeparam name="TView">The viewmodel</typeparam>
		/// <typeparam name="TAction">The action</typeparam>
		/// <returns>A read-only list of methods.</returns>
		private static IEnumerable<OnAction> GetOnAfterViewModelMethods<TView, TViewModel, TAction>()
			where TView : class, IView
			where TViewModel : BaseViewModel<TView, TViewModel>, new()
			where TAction : BaseAction<TView, TViewModel>
		{
			var onAfterViewModelMethods = GetOnAfterMethods<TViewModel>();
			return GetRelevantMethods<TAction>("OnAfter", onAfterViewModelMethods);
		}


		/// <summary>
		/// Gets the relevant methods.
		/// </summary>
		/// <typeparam name="TAction">The type of the action.</typeparam>
		/// <param name="prefix">The OnBefore or OnAfter prefix, depending on the situation.</param>
		/// <param name="methods">The methods.</param>
		/// <returns></returns>
		private static IEnumerable<OnAction> GetRelevantMethods<TAction>(string prefix, IEnumerable<OnAction> methods)
		{
			var name = prefix + typeof(TAction).Name.ToString(CultureInfo.InvariantCulture);
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
		private static IEnumerable<OnAction> GetOnBeforeMethods<T>() where T : class
		{
			IEnumerable<OnAction> methods;
			return OnBeforeMethods.TryGetValue(typeof (T), out methods)
			       	? methods
			       	: CacheMethods<T>().Item1;
		}

		/// <summary>
		/// Gets the OnAfter methods.
		/// </summary>
		/// <typeparam name="T">The type to get the methods for.</typeparam>
		/// <returns>A list of OnBefore methods.</returns>
		private static IEnumerable<OnAction> GetOnAfterMethods<T>() where T : class
		{
			IEnumerable<OnAction> methods;
			return OnAfterMethods.TryGetValue(typeof (T), out methods)
			       	? methods
			       	: CacheMethods<T>().Item2;
		}

		/// <summary>
		/// Caches the methods.
		/// </summary>
		/// <typeparam name="T">The type to cache the methods for.</typeparam>
		private static Tuple<IEnumerable<OnAction>, IEnumerable<OnAction>> CacheMethods<T>() 
			where T : class
		{
			var type = typeof(T);

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

		/// <summary>
		/// Called when an action starts its execution.
		/// </summary>
		/// <typeparam name="TAction">The type of action.</typeparam>
		/// <typeparam name="TView">The type of the view.</typeparam>
		/// <typeparam name="TViewModel">The type of the view model.</typeparam>
		/// <param name="action">The action to run.</param>
		public static void OnBefore<TAction, TView, TViewModel>(TAction action)
			where TAction : BaseAction<TView, TViewModel>
			where TViewModel : BaseViewModel<TView, TViewModel>, new()
			where TView : class, IView
		{
			var viewMethods = GetOnBeforeViewMethods<TView, TViewModel, TAction>();
			foreach (var viewMethod in viewMethods)
			{
				viewMethod.Invoke(action.View, action.ActionContext);
			}

			var viewModelMethods = GetOnBeforeViewModelMethods<TView, TViewModel, TAction>();
			foreach (var viewModelMethod in viewModelMethods)
			{
				viewModelMethod.Invoke(action.ViewModel, action.ActionContext);
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
			where TAction : BaseAction<TView, TViewModel>
			where TViewModel : BaseViewModel<TView, TViewModel>, new()
			where TView : class, IView
		{
			var viewMethods = GetOnAfterViewMethods<TView, TViewModel, TAction>();
			foreach (var viewMethod in viewMethods)
			{
				viewMethod.Invoke(action.View, action.ActionContext);
			}

			var viewModelMethods = GetOnAfterViewModelMethods<TView, TViewModel, TAction>();
			foreach (var viewModelMethod in viewModelMethods)
			{
				viewModelMethod.Invoke(action.ViewModel, action.ActionContext);
			}
		}
	}
}
