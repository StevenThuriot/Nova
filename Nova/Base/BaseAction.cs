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
using Nova.Controls;
using Nova.Properties;
using Nova.Validation;

namespace Nova.Base
{
	/// <summary>
	/// The base action class used for every actions fired in the framework.
	/// </summary>
	/// <typeparam name="TView">The type of the view.</typeparam>
	/// <typeparam name="TViewModel">The type of the view model.</typeparam>
	public abstract class BaseAction<TView, TViewModel> : IDisposable
		where TView : class, IView
		where TViewModel : BaseViewModel<TView, TViewModel>, new()
	{
		private bool _CanComplete;
		private bool _Disposed;
		private ValidationResults _ValidationResults;

		/// <summary>
		/// Gets the action context.
		/// </summary>
	    public ActionContext ActionContext { get; private set; }

		/// <summary>
		/// Gets the view.
		/// </summary>
		public TView View { get; private set; }

		/// <summary>
		/// Gets the view model.
		/// </summary>
		public TViewModel ViewModel { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseAction&lt;TView, TViewModel&gt;"/> class.
		/// </summary>
		protected BaseAction()
		{
			_CanComplete = false;
			_ValidationResults = new ValidationResults();
		}
		
		#region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="BaseAction&lt;TView, TViewModel&gt;"/> is reclaimed by garbage collection.
		/// </summary>
		~BaseAction()
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
				DisposeManagedResources();

				View = null;
				ViewModel = null;

				if (ActionContext != null)
				{
					ActionContext.Clear();
					ActionContext = null;
				}

				if (_ValidationResults != null)
				{
					_ValidationResults.InternalReset();
					_ValidationResults = null;
				}
			}

			DisposeUnmanagedResources();
			_Disposed = true;
		}

		#endregion

		#region Virtuals 

		/// <summary>
		/// Determines whether this instance can execute.
		/// </summary>
		/// <returns>
		///   <c>true</c> if this instance can execute; otherwise, <c>false</c>.
		/// </returns>
		public virtual bool CanExecute()
		{
			return true;
		}

		/// <summary>
		/// Executes this instance.
		/// </summary>
		/// <returns></returns>
		public virtual bool Execute()
		{
			return true;
		}

		/// <summary>
		/// Executes when the async execution succesfully completed.
		/// </summary>
		public virtual void ExecuteCompleted()
		{
		}

		/// <summary>
		/// Validates this instance.
		/// </summary>
		/// <param name="results">The results.</param>
		public virtual void Validate(ValidationResults results)
		{
		}

		/// <summary>
		/// Disposes the managed resources.
		/// </summary>
		protected virtual void DisposeManagedResources()
		{
		}

		/// <summary>
		/// Disposes the unmanaged resources.
		/// </summary>
		protected virtual void DisposeUnmanagedResources()
		{
		}

		#endregion

		#region Internal logic
		
		/// <summary>
		/// Creates a new instance and sets the required data.
		/// This is used to easily create new instances of baseaction inside of the framework using generics rather than reflection.
		/// </summary>
		/// <param name="view">The view.</param>
		/// <param name="viewModel">The view model.</param>
		/// <param name="actionContext">The action context.</param>
		internal static TResult New<TResult>(TView view, TViewModel viewModel, ActionContext actionContext)
			where TResult : BaseAction<TView, TViewModel>, new()
		{
			var action = new TResult
			             	{
			             		View = view,
			             		ViewModel = viewModel,
			             		ActionContext = actionContext
			             	};

			return action;
		}
		
		/// <summary>
		/// Runs the execute action.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		internal void InternalExecute()
		{
			try
			{
				Validate(_ValidationResults);
				_CanComplete = _ValidationResults.IsValid && Execute();
			}
			catch (Exception exception)
			{
				_CanComplete = false;
				ExceptionHandler.Handle(exception, Resources.ErrorMessageAsync);
			}
		}

		/// <summary>
		/// Runs the execute completed action.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		internal void InternalExecuteCompleted()
		{
			if (_CanComplete)
			{
				try
				{
					ExecuteCompleted();
					ActionContext.IsSuccessful = true;
				}
				catch (Exception exception)
				{
					ExceptionHandler.Handle(exception, Resources.ErrorMessageMainThread);
				}
			}

			var validationMessages = _ValidationResults.InternalGetValidations();
			ViewModel.ErrorCollection = new ReadOnlyErrorCollection(validationMessages);

			_ValidationResults.InternalReset();
		}

		#endregion
	}
}