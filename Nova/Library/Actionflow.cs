using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using Nova.Controls;
using Nova.Library.ActionMethodRepository;
using Nova.Properties;

namespace Nova.Library
{
	/// <summary>
	/// The base action class used for every actions fired in the framework.
	/// </summary>
	/// <typeparam name="TView">The type of the view.</typeparam>
	/// <typeparam name="TViewModel">The type of the view model.</typeparam>
	public abstract class Actionflow<TView, TViewModel> : IDisposable
		where TView : IView
		where TViewModel : IViewModel
	{
		private bool _disposed;

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
			CanComplete = true;
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
			if (_disposed) return;

			if (disposing)
			{
				DisposeManagedResources();
			}

			DisposeUnmanagedResources();
			_disposed = true;
		}

		#endregion

		#region Virtuals

        /// <summary>
        /// Called before the action actually starts executing.
        /// </summary>
        public virtual void OnBefore()
        {
            
        }

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
        /// Called after the action finishes executing.
        /// </summary>
        public virtual void OnAfter()
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
		/// The logic that runs before the action.
		/// </summary>
		internal virtual void InternalOnBefore()
        {
            try
            {
                View.StartLoading();
                
                CommandManager.InvalidateRequerySuggested();
                
                OnActionMethodRepository.OnBefore<Actionflow<TView, TViewModel>, TView, TViewModel>(this);
                OnBefore();
            }
            catch (Exception exception)
            {
                CanComplete = false;
                ExceptionHandler.Handle(exception, Resources.ErrorMessageAsync);
            }
		}
		
		/// <summary>
		/// Runs the execute action.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		internal virtual void InternalExecute()
		{
		    if (!CanComplete) return;

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
	        try
	        {
	            OnAfter();
	            OnActionMethodRepository.OnAfter<Actionflow<TView, TViewModel>, TView, TViewModel>(this);
	        }
	        catch (Exception exception)
	        {
	            ExceptionHandler.Handle(exception, Resources.ErrorMessageMainThread);
	        }
	        finally
	        {
	            View.StopLoading();
	        }
        }

        /// <summary>
        /// Returns a value wheter this instance ran successfully.
        /// </summary>
        /// <returns></returns>
        internal bool RanSuccesfully()
        {
            return ActionContext.IsSuccessful;
        }

	    #endregion



        /// <summary>
        /// Creates a new instance and sets the required data.
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
                ActionContext = actionContext ?? ActionContext.New<TResult>()
            };

            return action;
        }

        /// <summary>
        /// Creates a new instance and sets the required data.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="view">The view.</param>
        /// <param name="viewModel">The view model.</param>
        /// <param name="entries">The entries.</param>
        /// <returns></returns>
        public static TResult New<TResult>(TView view, TViewModel viewModel, params ActionContextEntry[] entries)
            where TResult : Actionflow<TView, TViewModel>, new()
        {
            var actionContext = ActionContext.New<TResult>(entries);
            return New<TResult>(view, viewModel, actionContext);
        }
	}
}