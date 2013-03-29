#region License

// 
//  Copyright 2013 Steven Thuriot
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
using System.Threading.Tasks;
using Nova.Controls;
using Nova.Properties;
using Nova.Threading;
using Nova.Threading.Implementations.WPF;
using RESX = Nova.Properties.Resources;

namespace Nova.Base
{
    /// <summary>
    ///     The controller that handles actions.
    /// </summary>
    /// <typeparam name="TView">The View.</typeparam>
    /// <typeparam name="TViewModel">The ViewModel.</typeparam>
    public class ActionController<TView, TViewModel>
        where TViewModel : IViewModel
        where TView : IView
    {
        private readonly IActionQueueManager _ActionQueueManager;
        private readonly TView _View;
        private readonly TViewModel _ViewModel;

        /// <summary>
        ///     Default ctor.
        /// </summary>
        /// <param name="view">The View.</param>
        /// <param name="viewModel">The ViewModel.</param>
        /// <param name="actionQueueManager">The Action Queue Manager</param>
        public ActionController(TView view, TViewModel viewModel, IActionQueueManager actionQueueManager)
        {
            _View = view;
            _ViewModel = viewModel;
            _ActionQueueManager = actionQueueManager;
        }

        /// <summary>
        ///     Invokes the specified action.
        /// </summary>
        /// <typeparam name="T">The type of action to invoke.</typeparam>
        /// <param name="actionToRun">The action to execute.</param>
        /// <param name="disposeActionDuringCleanup">Dispose the action after execution, during cleanup.</param>
        /// <param name="executeCompleted">The action to invoke when the async execution completes.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private IAction Invoke<T>(T actionToRun, bool disposeActionDuringCleanup, Action executeCompleted = null)
            where T : Actionflow<TView, TViewModel>
        {
            if (actionToRun == null)
                return null;

            var action = PrepareAction(actionToRun, disposeActionDuringCleanup, executeCompleted);

            if (!_ActionQueueManager.Enqueue(action))
            {
                CleanUpFailedEnqueue(actionToRun, disposeActionDuringCleanup, executeCompleted);
                return null;
            }

            return action;
        }

        /// <summary>
        /// Cleans up an action that failed to enqueue.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actionToRun">The action to run.</param>
        /// <param name="disposeActionDuringCleanup">if set to <c>true</c> [dispose action during cleanup].</param>
        /// <param name="executeCompleted">The execute completed.</param>
        private static void CleanUpFailedEnqueue<T>(T actionToRun, bool disposeActionDuringCleanup, Action executeCompleted)
            where T : Actionflow<TView, TViewModel>
        {
            Task task = null;

            if (executeCompleted != null)
            {
                task = Task.Run(executeCompleted);
            }

            if (!disposeActionDuringCleanup) return;

            if (task == null)
            {
                actionToRun.Dispose();
            }
            else
            {
                task.ContinueWith(_ => actionToRun.Dispose())
                    .ContinueWith(x => { if (x.Exception != null) x.Exception.Handle(_ => true); }, TaskContinuationOptions.OnlyOnFaulted);
            }
        }

        /// <summary>
        ///     Wraps the action into an IAction format so we can pass it on to the Queue Manager.
        /// </summary>
        /// <typeparam name="T">The type of action to invoke.</typeparam>
        /// <param name="actionToRun">The action to execute.</param>
        /// <param name="disposeActionDuringCleanup">Dispose the action after execution, during cleanup.</param>
        /// <param name="executeCompleted">The action to invoke when the async execution completes.</param>
        /// <returns>The action to run in an IAction format.</returns>
        private static IAction PrepareAction<T>(T actionToRun, bool disposeActionDuringCleanup, Action executeCompleted)
            where T : Actionflow<TView, TViewModel>
        {
            if (actionToRun == null)
                return null;

            var action = actionToRun.Wrap(x => x.ViewModel.ID, x => x.View.StartLoading, x => x.RanSuccesfully, mainThread: true)

                                    .CanExecute(actionToRun.CanExecute)

                                    .ContinueWith(actionToRun.InternalOnBefore, mainThread: true)
                                    .ContinueWith(actionToRun.InternalExecute)
                                    .ContinueWith(actionToRun.InternalExecuteCompleted, mainThread: true)

                                    .ContinueWith(actionToRun.InternalOnAfter, mainThread: true)

                                    .FinishWith(() =>
                                        {
                                            actionToRun.View.StopLoading();

                                            if (disposeActionDuringCleanup)
                                                actionToRun.Dispose();

                                        }, mainThread: true)

                                    .HandleException(x => ExceptionHandler.Handle(x, RESX.UnhandledException)); //Main Thread because view logic in clean up. (e.g. IsLoading)


            if (executeCompleted != null)
            {
                action.ContinueWith(executeCompleted);
            }

            return action;
        }

        /// <summary>
        ///     Invokes the specified action.
        /// </summary>
        /// <typeparam name="T">The type of action to invoke.</typeparam>
        /// <param name="actionContext">The action context.</param>
        /// <param name="executeCompleted">The action to invoke when the async execution completes.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private IAction Invoke<T>(ActionContext actionContext, Action executeCompleted = null)
            where T : Actionflow<TView, TViewModel>, new()
        {
            T actionToRun = null;

            try
            {
                actionToRun = Actionflow<TView, TViewModel>.New<T>(_View, _ViewModel, actionContext);
            }
            catch (Exception exception)
            {
                ExceptionHandler.Handle(exception, Resources.ErrorMessageAction);
            }

            return Invoke(actionToRun, true, executeCompleted);
        }
        
        /// <summary>
        ///     Prepares the action context.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <returns>The action context.</returns>
        private static ActionContext PrepareActionContext(params ActionContextEntry[] arguments)
        {
            var actionContext = new ActionContext();

            if (arguments != null)
            {
                actionContext.AddRange(arguments);
            }

            return actionContext;
        }













        /// <summary>
        ///     Invokes the action.
        /// </summary>
        /// <typeparam name="T">The type of action to invoke.</typeparam>
        /// <param name="arguments">The arguments.</param>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "Required by called method.")]
        public void InvokeAction<T>(params ActionContextEntry[] arguments)
            where T : Actionflow<TView, TViewModel>, new()
        {
            ActionContext actionContext = PrepareActionContext(arguments);
            Invoke<T>(actionContext);
        }
        
        /// <summary>
        ///     Invokes the action.
        ///     This call is for internal means only.
        /// </summary>
        /// <typeparam name="T">The type of action to invoke.</typeparam>
        /// <param name="actionToRun">The action to run.</param>
        /// <param name="disposeActionDuringCleanup">Dispose the action after execution, during cleanup.</param>
        /// <param name="executeCompleted">The action to execute after completion.</param>
        internal void InternalInvokeAction<T>(T actionToRun, bool disposeActionDuringCleanup = true, Action executeCompleted = null)
            where T : Actionflow<TView, TViewModel>, new()
        {
            Invoke(actionToRun, disposeActionDuringCleanup, executeCompleted);
        }













        /// <summary>
        ///     Invokes the action.
        /// </summary>
        /// <typeparam name="T">The type of action to invoke.</typeparam>
        /// <param name="arguments">The arguments.</param>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "Required by called method.")]
        public async Task<bool> InvokeActionAsync<T>(params ActionContextEntry[] arguments)
            where T : Actionflow<TView, TViewModel>, new()
        {
            ActionContext actionContext = PrepareActionContext(arguments);
            var action = Invoke<T>(actionContext);

            if (action == null)
                return false;

            return await action.GetSuccessAsync();
        }

        /// <summary>
        ///     Invokes the action.
        ///     This call is for internal means only.
        /// </summary>
        /// <typeparam name="T">The type of action to invoke.</typeparam>
        /// <param name="actionToRun">The action to run.</param>
        /// <param name="disposeActionDuringCleanup">Dispose the action after execution, during cleanup.</param>
        /// <param name="executeCompleted">The action to execute after completion.</param>
        internal async Task<bool> InternalInvokeActionAsync<T>(T actionToRun, bool disposeActionDuringCleanup = true, Action executeCompleted = null)
            where T : Actionflow<TView, TViewModel>, new()
        {
            var action = Invoke(actionToRun, disposeActionDuringCleanup, executeCompleted);

            if (action == null)
                return false;

            return await action.GetSuccessAsync();
        }
    }
}