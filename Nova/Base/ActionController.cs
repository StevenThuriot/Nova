using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Nova.Controls;
using Nova.Properties;

namespace Nova.Base
{
	/// <summary>
	/// The controller that handles actions.
	/// </summary>
	/// <typeparam name="TView">The View.</typeparam>
	/// <typeparam name="TViewModel">The ViewModel.</typeparam>
	public class ActionController<TView, TViewModel>
		where TViewModel : BaseViewModel<TView, TViewModel>, new()
		where TView : class, IView
	{
		private readonly TView _View;
		private readonly TViewModel _ViewModel;
		private readonly ICollection<BaseAction<TView, TViewModel>> _Actions;

		/// <summary>
		/// Default ctor.
		/// </summary>
		/// <param name="view">The View.</param>
		/// <param name="viewModel">The ViewModel.</param>
		public ActionController(TView view, TViewModel viewModel)
		{
			_View = view;
			_ViewModel = viewModel;
			_Actions = new List<BaseAction<TView, TViewModel>>();
		}


		/// <summary>
		/// Invokes the specified action.
		/// </summary>
		/// <typeparam name="T">The type of action to invoke.</typeparam>
		/// <param name="actionToRun">The action to execute.</param>
		/// <param name="disposeActionDuringCleanup">Dispose the action after execution, during cleanup.</param>
		/// <param name="executeCompleted">The action to invoke when the async execution completes.</param>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		private void Invoke<T>(T actionToRun, bool disposeActionDuringCleanup, Action executeCompleted = null)
			where T : BaseAction<TView, TViewModel>
		{
			if (actionToRun == null)
				return;

			if (actionToRun.CanExecute())
			{
				Action<Task> executedCompleteAction = x =>
				{
					actionToRun.InternalExecuteCompleted();
					CleanUp(actionToRun, executeCompleted, disposeActionDuringCleanup);
				};

				OnActionMethodRepository.OnBefore<T, TView, TViewModel>(actionToRun);

				if (!_Actions.Any())
					_View.StartLoading();

				_Actions.Add(actionToRun);

				Task.Factory.StartNew(actionToRun.InternalExecute)
							.ContinueWith(executedCompleteAction, TaskScheduler.FromCurrentSynchronizationContext());
			}
			else
			{
				CleanUp(actionToRun, executeCompleted, disposeActionDuringCleanup);
			}
		}


		/// <summary>
		/// Invokes the specified action.
		/// </summary>
		/// <typeparam name="T">The type of action to invoke.</typeparam>
		/// <param name="actionContext">The action context.</param>
		/// <param name="executeCompleted">The action to invoke when the async execution completes.</param>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		private void Invoke<T>(ActionContext actionContext, Action executeCompleted = null)
			where T : BaseAction<TView, TViewModel>, new()
		{
			T actionToRun = null;

			try
			{
				actionToRun = BaseAction<TView, TViewModel>.New<T>(_View, _ViewModel, actionContext);
			}
			catch (Exception exception)
			{
				ExceptionHandler.Handle(exception, Resources.ErrorMessageAction);
			}

			Invoke(actionToRun, true, executeCompleted);
		}

		/// <summary>
		/// Cleans up the specified action.
		/// </summary>
		/// <typeparam name="T">The type of action to clean up.</typeparam>
		/// <param name="actionToRun">The action to run.</param>
		/// <param name="executeCompleted">The action to invoke when the async execution completes.</param>
		/// <param name="dispose">Dispose the action.</param>
		private void CleanUp<T>(T actionToRun, Action executeCompleted, bool dispose) 
			where T : BaseAction<TView, TViewModel>
		{
			if (executeCompleted != null)
				executeCompleted();

			OnActionMethodRepository.OnAfter<T, TView, TViewModel>(actionToRun);

			_Actions.Remove(actionToRun);

			if (dispose)
				actionToRun.Dispose();

			if (!_Actions.Any())
				_View.StopLoading();
		}

		/// <summary>
		/// Prepares the action context.
		/// </summary>
		/// <param name="arguments">The arguments.</param>
		/// <returns>The action context.</returns>
		private static ActionContext PrepareActionContext(params KeyValuePair<string, object>[] arguments)
		{
			var actionContext = new ActionContext();

			if (arguments != null)
			{
				foreach (var argument in arguments)
				{
					actionContext.Add(argument.Key, argument.Value);
				}
			}

			return actionContext;
		}

		/// <summary>
		/// Invokes the action.
		/// </summary>
		/// <typeparam name="T">The type of action to invoke.</typeparam>
		/// <param name="arguments">The arguments.</param>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Required by called method.")]
		public void InvokeAction<T>(params KeyValuePair<string, object>[] arguments)
			where T : BaseAction<TView, TViewModel>, new()
		{
			var actionContext = PrepareActionContext(arguments);
			Invoke<T>(actionContext);
		}

		/// <summary>
		/// Invokes the action.
		/// </summary>
		/// <typeparam name="T">The type of action to invoke.</typeparam>
		/// <param name="sender">The sender.</param>
		/// <param name="arguments">The arguments.</param>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Required by called method.")]
		public void InvokeAction<T>(UIElement sender, params KeyValuePair<string, object>[] arguments)
			where T : BaseAction<TView, TViewModel>, new()
		{
			sender.IsEnabled = false;
			var actionContext = PrepareActionContext(arguments);
			Invoke<T>(actionContext, () => sender.IsEnabled = true);
		}

		/// <summary>
		/// Invokes the action. 
		/// This call is for internal means only. It is used to make routed actions work through the action controller's logic.
		/// </summary>
		/// <typeparam name="T">The type of action to invoke.</typeparam>
		/// <param name="actionToRun">The action to run.</param>
		/// <param name="executeCompleted">The action to execute after completion.</param>
		internal void InternalInvokeRoutedAction<T>(T actionToRun, Action executeCompleted)
			where T : BaseAction<TView, TViewModel>, new()
		{
			Invoke(actionToRun, true, executeCompleted);
		}
	}
}
