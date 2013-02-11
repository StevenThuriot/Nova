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
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Nova.Controls;

namespace Nova.Base
{
	/// <summary>
	/// The action manager, to dynamically create and cache RoutedActions.
	/// </summary>
	public class ActionManager<TView, TViewModel> : DynamicObject, IDisposable
		where TView : class, IView
		where TViewModel : ViewModel<TView, TViewModel>, new()
	{
		private bool _IsDisposed;

		private IDictionary<string, ICommand> _Actions;

		private List<Type> _KnownTypes;
		private List<Type> _ViewAndViewModelTypes;
		private List<Type> _LoadedTypes;

		private TView _View;
		private TViewModel _ViewModel;

		private MethodInfo _CreateAction;

		private readonly Type _ViewType;
		private readonly Type _ViewModelType;

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionManager&lt;TView, TViewModel&gt;"/> class.
		/// </summary>
		/// <param name="view">The view.</param>
		/// <param name="viewModel">The view model.</param>
		public ActionManager(TView view, TViewModel viewModel)
		{
			_IsDisposed = false;

			_View = view;
			_ViewModel = viewModel;

			_ViewType = typeof (TView);
			_ViewModelType = typeof (TViewModel);

			_Actions = new Dictionary<string, ICommand>();
			_KnownTypes = new List<Type>();
		}

		/// <summary>
		/// Provides the implementation for operations that set member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as setting a value for a property.
		/// </summary>
		/// <returns>
		/// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
		/// </returns>
		/// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member to which the value is being assigned. For example, for the statement sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param><param name="value">The value to set to the member. For example, for sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, the <paramref name="value"/> is "Test".</param>
		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
		    var name = binder.Name;

		    if (string.IsNullOrWhiteSpace(name))
		        return false;

		    if (_Actions.ContainsKey(name))
                throw new NotSupportedException("Resolved values cannot be set.");

		    var command = value as ICommand;
            if (command == null)
            {
                return false;
            }

            _Actions.Add(name, command);

		    return true;
		}

		/// <summary>
		/// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
		/// </summary>
		/// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
		/// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result"/>.</param>
		/// <returns>
		/// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)
		/// </returns>
		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			ICommand action;

			var name = binder.Name;
			if (!_Actions.TryGetValue(name, out action))
			{
				var equivalentTypeName = name.EndsWith("action", StringComparison.OrdinalIgnoreCase)
				                         	? name.Substring(0, name.Length - 6)
				                         	: name + "Action";

				var actionType = SearchType(equivalentTypeName, name);

				if (actionType != null)
				{
					action = CreateAction(actionType, name);
				}
			}

			result = action;

			return result != null;
		}

		private ICommand CreateAction(Type actionType, string name)
		{
			if (_CreateAction == null)
			{
				_CreateAction =
					typeof (RoutedAction).GetMethods().First(x => string.Equals(x.Name, "New", StringComparison.OrdinalIgnoreCase) &&
					                                              x.GetParameters().Count() == 2);
			}

			var generic = _CreateAction.MakeGenericMethod(actionType, _ViewType, _ViewModelType);

			var action = (ICommand) generic.Invoke(null, new object[] {_View, _ViewModel});

			_Actions.Add(name, action);
			return action;
		}

		#region Search Type

		private Type SearchType(string equivalentTypeName, string name)
		{
			var actionType = GetActionType(name, equivalentTypeName, _KnownTypes) ??
			                 (SearchVVMTypes(equivalentTypeName, name) ??
			                  SearchAllTypes(equivalentTypeName, name));

			return actionType;
		}

		private Type SearchVVMTypes(string equivalentTypeName, string name)
		{
			if (_ViewAndViewModelTypes == null)
			{
				InitVVMTypes();
			}

			var actionType = GetActionType(name, equivalentTypeName, _ViewAndViewModelTypes);
			return actionType;
		}

		private Type SearchAllTypes(string equivalentTypeName, string name)
		{
			if (_LoadedTypes == null)
			{
				InitAllTypes();
			}

			var actionType = GetActionType(name, equivalentTypeName, _LoadedTypes);
			return actionType;
		}

		private static Type GetActionType(string name, string equivalentTypeName, IEnumerable<Type> types)
		{
			if (types == null)
				return null;

			var actionType = types.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase) ||
			                                           x.Name.Equals(equivalentTypeName, StringComparison.OrdinalIgnoreCase));

			return actionType;
		}

		#endregion Search Type

		#region Init

		/// <summary>
		/// Sets the known types.
		/// The action manager will choose from these types to initiate an action.
		/// </summary>
		/// <param name="knownTypes">The known types.</param>
		public void SetKnownTypes(params Type[] knownTypes)
		{
			_KnownTypes.AddRange(knownTypes);
		}

		private void InitVVMTypes()
		{
			var viewAssembly = _ViewType.Assembly;
			IEnumerable<Type> viewTypes = viewAssembly.GetTypes();

			var viewModelAssembly = _ViewModelType.Assembly;
			if (viewModelAssembly == viewAssembly)
			{
				_ViewAndViewModelTypes = viewTypes.ToList();
			}
			else
			{
				IEnumerable<Type> viewModelTypes = viewModelAssembly.GetTypes();
				_ViewAndViewModelTypes = new List<Type>(viewTypes.Union(viewModelTypes));
			}
		}

		private void InitAllTypes()
		{
			var allTypes = AppDomain.CurrentDomain.GetAssemblies()
				.Where(x =>
				       	{
				       		if (x.FullName.StartsWith("system", StringComparison.OrdinalIgnoreCase) ||
				       		    x.FullName.StartsWith("microsoft", StringComparison.OrdinalIgnoreCase))
				       		{
				       			return false;
				       		}

				       		var name = x.GetName().Name;

				       		if (string.IsNullOrEmpty(name))
				       			return false;

				       		return !name.Equals("mscorlib", StringComparison.OrdinalIgnoreCase) &&
				       		       !name.Equals("presentationcore", StringComparison.OrdinalIgnoreCase) &&
				       		       !name.Equals("presentationframework", StringComparison.OrdinalIgnoreCase) &&
				       		       !name.Equals("windowsbase", StringComparison.OrdinalIgnoreCase);
				       	})
				.SelectMany(x => x.GetTypes());

			_LoadedTypes = new List<Type>(allTypes);
		}

		#endregion Init

		#region Dispose

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="ActionManager&lt;TView, TViewModel&gt;"/> is reclaimed by garbage collection.
		/// </summary>
		~ActionManager()
		{
			Dispose(false);
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		private void Dispose(bool disposing)
		{
			if (_IsDisposed)
				return;

			if (disposing)
			{
				if (_Actions != null)
				{
					foreach (var disposableAction in _Actions.Values.OfType<IDisposable>().Where(x => x != null))
					{
						disposableAction.Dispose();
					}

					_Actions.Clear();
					_Actions = null;
				}

				if (_KnownTypes != null)
				{
					_KnownTypes.Clear();
					_KnownTypes = null;
				}

				if (_ViewAndViewModelTypes != null)
				{
					_ViewAndViewModelTypes.Clear();
					_ViewAndViewModelTypes = null;
				}

				if (_LoadedTypes != null)
				{
					_LoadedTypes.Clear();
					_LoadedTypes = null;
				}

				_View = null;
				_ViewModel = null;

				_CreateAction = null;
			}

			_IsDisposed = true;
		}

		#endregion Dispose
	}
}
