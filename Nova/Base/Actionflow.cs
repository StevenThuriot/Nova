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

namespace Nova.Base
{
	/// <summary>
	/// The base action class used for every actions fired in the framework.
	/// </summary>
	/// <typeparam name="TView">The type of the view.</typeparam>
	/// <typeparam name="TViewModel">The type of the view model.</typeparam>
	public abstract class Actionflow<TView, TViewModel> : IDisposable
		where TView : class, IView
		where TViewModel : ViewModel<TView, TViewModel>, new()
	{
		private bool _Disposed;

	    /// <summary>
	    /// Gets or sets the ability to complete. For Internal use only!
	    /// </summary>
	    protected bool CanComplete;

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
		/// Initializes a new instance of the <see cref="Actionflow{TView,TViewModel}"/> class.
		/// </summary>
		protected Actionflow()
		{
			CanComplete = false;
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
		/// <see cref="Actionflow{TView,TViewModel}"/> is reclaimed by garbage collection.
		/// </summary>
		~Actionflow()
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
		/// Executes async.
		/// </summary>
		/// <returns>A value indicating wether to continue execution.</returns>
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
		internal static TResult New<TResult>(TView view, TViewModel viewModel, ActionContext actionContext = null)
			where TResult : Actionflow<TView, TViewModel>, new()
		{
            if (view == null)
                throw new ArgumentNullException("view");

            if (viewModel == null)
                throw new ArgumentNullException("viewModel");

			var action = new TResult
			             	{
			             		View = view,
			             		ViewModel = viewModel,
			             		ActionContext = actionContext ?? new ActionContext()
			             	};

			return action;
		}
		
		/// <summary>
		/// The logic that runs before the action.
		/// </summary>
		internal void InternalOnBefore()
		{
            OnActionMethodRepository.OnBefore<Actionflow<TView, TViewModel>, TView, TViewModel>(this);
		}
		
		/// <summary>
		/// Runs the execute action.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		internal virtual void InternalExecute()
		{
			try
			{
                SafeInternalExecute();
			}
			catch (Exception exception)
			{
				CanComplete = false;
				ExceptionHandler.Handle(exception, Resources.ErrorMessageAsync);
			}
		}

        /// <summary>
        /// Method so inheriting classes may add extra internal logic during the InternalExecute stage.
        /// </summary>
	    internal virtual void SafeInternalExecute()
	    {
	        CanComplete = Execute();
	    }

	    /// <summary>
		/// Runs the execute completed action.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		internal virtual void InternalExecuteCompleted()
		{
	            try
                {
                    SafeInternalExecuteCompleted();
	            }
	            catch (Exception exception)
	            {
	                ExceptionHandler.Handle(exception, Resources.ErrorMessageMainThread);
	            }

		}

        /// <summary>
        /// Method so inheriting classes may add extra internal logic during the InternalExecuteCompleted stage.
        /// </summary>
	    internal virtual void SafeInternalExecuteCompleted()
        {
            if (!CanComplete) return;

            ExecuteCompleted();
            ActionContext.IsSuccessful = true;
        }

	    /// <summary>
		/// The logic that runs after the action.
		/// </summary>
		internal void InternalOnAfter()
		{
            OnActionMethodRepository.OnAfter<Actionflow<TView, TViewModel>, TView, TViewModel>(this);
		}

	    #endregion
	}
}