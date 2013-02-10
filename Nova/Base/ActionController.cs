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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Windows;
using Nova.Controls;
using Nova.Properties;
using Nova.Threading;
using Nova.Threading.Implementations.WPF;

namespace Nova.Base
{
    /// <summary>
    ///     The controller that handles actions.
    /// </summary>
    /// <typeparam name="TView">The View.</typeparam>
    /// <typeparam name="TViewModel">The ViewModel.</typeparam>
    public class ActionController<TView, TViewModel>
        where TViewModel : BaseViewModel<TView, TViewModel>, new()
        where TView : class, IView
    {
        private readonly IActionQueueManager _ActionQueueManager;
        private readonly ICollection<BaseAction<TView, TViewModel>> _Actions;
        private readonly Mutex _Lock;
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
            _Lock = new Mutex();
            _View = view;
            _ViewModel = viewModel;
            _ActionQueueManager = actionQueueManager;
            _Actions = new List<BaseAction<TView, TViewModel>>();
        }

        /// <summary>
        ///     Invokes the specified action.
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

            var action = PrepareAction(actionToRun, disposeActionDuringCleanup, executeCompleted);

            _ActionQueueManager.Queue(action);
        }

        /// <summary>
        ///     Wraps the action into an IAction format so we can pass it on to the Queue Manager.
        /// </summary>
        /// <typeparam name="T">The type of action to invoke.</typeparam>
        /// <param name="actionToRun">The action to execute.</param>
        /// <param name="disposeActionDuringCleanup">Dispose the action after execution, during cleanup.</param>
        /// <param name="executeCompleted">The action to invoke when the async execution completes.</param>
        /// <returns>The action to run in an IAction format.</returns>
        private IAction PrepareAction<T>(T actionToRun, bool disposeActionDuringCleanup, Action executeCompleted)
            where T : BaseAction<TView, TViewModel>
        {
            if (actionToRun == null)
                return null;
                        
            var action = actionToRun.Wrap(x => x.View.ID, x => GetInitializationMethod(x), mainThread: true)
                                                         
                                                         .CanExecute(actionToRun.CanExecute)
                                                         
                                                         .ContinueWith(actionToRun.InternalOnBefore, mainThread: true)
                                                         .ContinueWith(actionToRun.InternalExecute)
                                                         .ContinueWith(actionToRun.InternalExecuteCompleted, mainThread: true)
                                                         
                                                         .ContinueWith(actionToRun.InternalOnAfter, mainThread: true)
                                                         
                                                         .FinishWith(() => CleanUp(actionToRun, disposeActionDuringCleanup), mainThread: true); //Main Thread because view logic in clean up. (e.g. IsLoading)

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
        /// Initiation method that needs to run when an action starts executing.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actionToRun">The action to run.</param>
        private Func<bool> GetInitializationMethod<T>(T actionToRun)
            where T : BaseAction<TView, TViewModel>
        {

            return () =>
                {
                    lock (_Lock)
                    {
                        actionToRun.View.StartLoading();
                        _Actions.Add(actionToRun);
                    }

                    return true;
                };

        }

        /// <summary>
        ///     Cleans up the specified action.
        /// </summary>
        /// <typeparam name="T">The type of action to clean up.</typeparam>
        /// <param name="actionToRun">The action to run.</param>
        /// <param name="dispose">Dispose the action.</param>
        private void CleanUp<T>(T actionToRun, bool dispose)
            where T : BaseAction<TView, TViewModel>
        {
            lock (_Lock)
            {
                _Actions.Remove(actionToRun);
                actionToRun.View.StopLoading();
            }

            if (dispose)
                actionToRun.Dispose();
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
            where T : BaseAction<TView, TViewModel>, new()
        {
            ActionContext actionContext = PrepareActionContext(arguments);
            Invoke<T>(actionContext);
        }

        /// <summary>
        ///     Invokes the action.
        /// </summary>
        /// <typeparam name="T">The type of action to invoke.</typeparam>
        /// <param name="sender">The sender.</param>
        /// <param name="arguments">The arguments.</param>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "Required by called method.")]
        public void InvokeAction<T>(UIElement sender, params ActionContextEntry[] arguments)
            where T : BaseAction<TView, TViewModel>, new()
        {
            sender.IsEnabled = false;
            ActionContext actionContext = PrepareActionContext(arguments);
            Invoke<T>(actionContext, () => sender.IsEnabled = true);
        }

        /// <summary>
        ///     Invokes the action.
        ///     This call is for internal means only. It is used to make routed actions work through the action controller's logic.
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