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
using System.Reflection;
using System.Windows.Input;
using Nova.Controls;
using Nova.Properties;
using System.Linq;

namespace Nova.Base
{
	/// <summary>
	/// This enables the usage of actions as commands.
	/// </summary>
	/// <typeparam name="T">The type of action</typeparam>
	/// <typeparam name="TView">The type of the view.</typeparam>
	/// <typeparam name="TViewModel">The type of the view model.</typeparam>
	internal class RoutedAction<T, TView, TViewModel> : ICommand, IDisposable
		where TView : class, IView
		where TViewModel : ViewModel<TView, TViewModel>, new()
		where T : Actionflow<TView, TViewModel>, new()
	{
		private T _Action;
		private ActionController<TView, TViewModel> _Controller;
		private bool _IsExecuting;
		private bool _Disposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="RoutedAction&lt;T, TView, TViewModel&gt;"/> class.
		/// </summary>
		/// <param name="view">The view.</param>
		/// <param name="viewModel">The view model.</param>
		/// <param name="actionController">The action controller.</param>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public RoutedAction(TView view, TViewModel viewModel, ActionController<TView, TViewModel> actionController)
		{
			if (_Controller == null)
				throw new ArgumentNullException("actionController");

			try
			{
				_Action = Actionflow<TView, TViewModel>.New<T>(view, viewModel);
				_Controller = actionController;
			}
			catch (Exception exception)
			{
				ExceptionHandler.Handle(exception, Resources.ErrorMessageAction);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RoutedAction&lt;T, TView, TViewModel&gt;"/> class.
		/// </summary>
		/// <param name="view">The view.</param>
		/// <param name="viewModel">The view model.</param>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public RoutedAction(TView view, TViewModel viewModel)
		{
			try
			{
				_Action = Actionflow<TView, TViewModel>.New<T>(view, viewModel);
				FindController(viewModel);
			}
			catch (Exception exception)
			{
				ExceptionHandler.Handle(exception, Resources.ErrorMessageAction);
			}
			
			if (_Controller == null)
				throw new ArgumentException(Resources.ActionControllerNotFound);
		}

		private void FindController(TViewModel viewModel)
		{
			var extendedView = viewModel as ViewModel<TView, TViewModel>;

			if (extendedView != null)
			{
				_Controller = viewModel.ActionController;
				return;
			}

			var actionControllerType = typeof(ActionController<TView, TViewModel>);
			var viewType = typeof (TView);

			var propertyInfos = viewType.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			var actionContextProperty = propertyInfos.FirstOrDefault(x => x.PropertyType == actionControllerType);
			if (actionContextProperty != null)
			{
				_Controller = actionContextProperty.GetValue(viewModel, null) as ActionController<TView, TViewModel>;
				
				if (_Controller != null)
					return;
			}

			var fieldInfos = viewType.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			var actionContextField = fieldInfos.FirstOrDefault(x => x.FieldType == actionControllerType);
			if (actionContextField != null)
			{
				_Controller = actionContextField.GetValue(viewModel) as ActionController<TView, TViewModel>;
			}
		}

		/// <summary>
		/// Defines the method to be called when the command is invoked.
		/// </summary>
		/// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
		public void Execute(object parameter)
		{
			_IsExecuting = true;
			SetActionContext(parameter);

			_Controller.InternalInvokeAction(_Action, executeCompleted: () =>
			                                                	{
			                                                		_IsExecuting = false;

			                                                		var view = _Action.View;
			                                                		var viewModel = _Action.ViewModel;

			                                                		_Action = Actionflow<TView, TViewModel>.New<T>(view, viewModel);
			                                                	});
		}

		/// <summary>
		/// Defines the method that determines whether the command can execute in its current state.
		/// </summary>
		/// <returns>
		/// true if this command can be executed; otherwise, false.
		/// </returns>
		/// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
		public bool CanExecute(object parameter)
		{
            //Making sure this doesn't throw an exception while being disposed.
            if (_Action == null || _Action.ActionContext == null)
                return false;

			SetActionContext(parameter);
			return !_IsExecuting && _Action.CanExecute();
		}

		/// <summary>
		/// Sets the action context.
		/// </summary>
		/// <param name="parameter">The parameter.</param>
		private void SetActionContext(object parameter)
		{
            _Action.ActionContext.Clear();

		    if (parameter == null) return;

		    var entry = ActionContextEntry.Create(RoutedAction.CommandParameter, parameter);
		    _Action.ActionContext.Add(entry);
		}

		/// <summary>
		/// Occurs when changes occur that affect whether or not the command should execute.
		/// </summary>
		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
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
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="RoutedAction&lt;T, TView, TViewModel&gt;"/> is reclaimed by garbage collection.
		/// </summary>
		~RoutedAction()
		{
			Dispose(false);
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		private void Dispose(bool disposing)
		{
			if (_Disposed) return;

			if (disposing)
			{
				if (_Action != null)
				{
					_Action.Dispose();
				}
			}

			_Disposed = true;
		}
	}
}
