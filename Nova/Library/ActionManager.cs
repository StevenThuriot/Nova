using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Nova.Controls;

namespace Nova.Library
{
	/// <summary>
	/// The action manager, to dynamically create and cache RoutedActions.
	/// </summary>
	public class ActionManager<TView, TViewModel> : DynamicObject, IDisposable
		where TView : IView
		where TViewModel : IViewModel
	{
		private bool _isDisposed;

		private readonly IDictionary<string, ICommand> _actions;

		private readonly List<Type> _knownTypes;
		private List<Type> _viewAndViewModelTypes;
		private List<Type> _loadedTypes;

		private readonly TViewModel _viewModel;

		private readonly Type _viewType;
        private readonly Type _viewModelType;

        private MethodInfo _createAction;

	    /// <summary>
	    /// Initializes a new instance of the <see cref="ActionManager&lt;TView, TViewModel&gt;"/> class.
	    /// </summary>
	    /// <param name="viewModel">The view model.</param>
	    public ActionManager(TViewModel viewModel)
		{
			_isDisposed = false;

			_viewModel = viewModel;

			_viewType = typeof (TView);
			_viewModelType = typeof (TViewModel);

			_actions = new Dictionary<string, ICommand>();
			_knownTypes = new List<Type>();
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

		    if (_actions.ContainsKey(name))
                throw new NotSupportedException("Resolved values cannot be set.");

		    var command = value as ICommand;
            if (command == null)
            {
                return false;
            }

            _actions.Add(name, command);

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
			if (!_actions.TryGetValue(name, out action))
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
			if (_createAction == null)
			{
				_createAction =
                    typeof(TViewModel).GetMethods().First(x => "CreateRoutedAction".Equals(x.Name, StringComparison.OrdinalIgnoreCase) && x.GetParameters().Length == 1);
			}

			var generic = _createAction.MakeGenericMethod(actionType);
			var action = (ICommand) generic.Invoke(_viewModel, new object[] {new ActionContextEntry[] {}});

			_actions.Add(name, action);
			return action;
		}

		#region Search Type

		private Type SearchType(string equivalentTypeName, string name)
		{
			var actionType = GetActionType(name, equivalentTypeName, _knownTypes) ??
			                 (SearchVVMTypes(equivalentTypeName, name) ??
			                  SearchAllTypes(equivalentTypeName, name));

			return actionType;
		}

		private Type SearchVVMTypes(string equivalentTypeName, string name)
		{
			if (_viewAndViewModelTypes == null)
			{
				InitVVMTypes();
			}

			var actionType = GetActionType(name, equivalentTypeName, _viewAndViewModelTypes);
			return actionType;
		}

		private Type SearchAllTypes(string equivalentTypeName, string name)
		{
			if (_loadedTypes == null)
			{
				InitAllTypes();
			}

			var actionType = GetActionType(name, equivalentTypeName, _loadedTypes);
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
		    var distinctTypes = knownTypes.Distinct().Where(x => !_knownTypes.Contains(x)).ToList();
            _knownTypes.AddRange(distinctTypes);
		}

	    private void InitVVMTypes()
		{
			var viewAssembly = _viewType.Assembly;
			IEnumerable<Type> viewTypes = viewAssembly.GetTypes();

			var viewModelAssembly = _viewModelType.Assembly;
			if (viewModelAssembly == viewAssembly)
			{
				_viewAndViewModelTypes = viewTypes.ToList();
			}
			else
			{
				IEnumerable<Type> viewModelTypes = viewModelAssembly.GetTypes();
				_viewAndViewModelTypes = new List<Type>(viewTypes.Union(viewModelTypes));
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

			_loadedTypes = new List<Type>(allTypes);
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
			if (_isDisposed)
				return;

			if (disposing)
			{
				if (_actions != null)
				{
					foreach (var disposableAction in _actions.Values.OfType<IDisposable>().Where(x => x != null))
					{
						disposableAction.Dispose();
					}

					_actions.Clear();
				}

				if (_knownTypes != null)
				{
					_knownTypes.Clear();
				}

				if (_viewAndViewModelTypes != null)
				{
					_viewAndViewModelTypes.Clear();
				}

				if (_loadedTypes != null)
				{
					_loadedTypes.Clear();
				}
			}

			_isDisposed = true;
		}

		#endregion Dispose
	}
}
