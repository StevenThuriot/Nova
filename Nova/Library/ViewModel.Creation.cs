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
using System.Windows.Input;
using Nova.Controls;
using Nova.Threading;
using Nova.Library.ActionMethodRepository;

namespace Nova.Library
{
    public abstract partial class ViewModel<TView, TViewModel>
    {
        private bool _enterOnInitialize;
        private bool _deferCreated;
        private bool _created;

        /// <summary>
        /// Creates the specified viewmodel.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="actionQueueManager">The action queue manager.</param>
        /// <param name="enterOnInitialize">if set to <c>true</c>, the Enter Action will be triggered automatically. Default is true.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">view</exception>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static TViewModel Create(TView view, IActionQueueManager actionQueueManager, bool enterOnInitialize = true)
        {
            if (view == null)
                throw new ArgumentNullException("view");
            if (actionQueueManager == null)
                throw new ArgumentNullException("actionQueueManager");

            //Optimization: Cache OnBefore/After logic on a different thread during creation so it's available when we need it.
            OnActionMethodRepository.CacheOnActionLogic<TView, TViewModel>();

            var viewModel = new TViewModel();
            viewModel.Initialize(view, actionQueueManager, enterOnInitialize);

            return viewModel;
        }

        /// <summary>
        /// Creates a new Actionflow instance and sets the required data.
        /// </summary>
        /// <typeparam name="T">The type of action to create.</typeparam>
        /// <param name="actionContext">The action context.</param>
        /// <returns>A new actionflow instance.</returns>
        public T CreateAction<T>(ActionContext actionContext = null)
            where T : Actionflow<TView, TViewModel>, new()
        {
            return Actionflow<TView, TViewModel>.New<T>(View, (TViewModel) this, actionContext);
        }

        /// <summary>
        /// Creates a new Actionflow instance and sets the required data.
        /// </summary>
        /// <typeparam name="T">The type of action to create.</typeparam>
        /// <param name="entries">The entries.</param>
        /// <returns>
        /// A new actionflow instance.
        /// </returns>
        public T CreateAction<T>(params ActionContextEntry[] entries)
            where T : Actionflow<TView, TViewModel>, new()
        {
            return Actionflow<TView, TViewModel>.New<T>(View, (TViewModel) this, entries);
        }


        /// <summary>
        /// Creates a new action with a wrapper which implements the ICommand interface.
        /// </summary>
        /// <typeparam name="T">The type of action to create.</typeparam>
        /// <param name="entries">The default entries.</param>
        /// <returns>
        /// A new action with a wrapper which implements the ICommand interface
        /// </returns>
        public ICommand CreateRoutedAction<T>(params ActionContextEntry[] entries)
            where T : Actionflow<TView, TViewModel>, new()
        {
            return RoutedAction.New<T, TView, TViewModel>(View, (TViewModel) this, ActionController, entries);
        }



        /// <summary>
        /// Initializes the viewmodel and triggers all the needed logic.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="actionQueueManager">The action queue manager.</param>
        /// <param name="enterOnInitialize">if set to <c>true</c>, the Enter Action will be triggered automatically. Default is true.</param>
        /// <exception cref="System.ArgumentNullException">view</exception>
        private void Initialize(TView view, IActionQueueManager actionQueueManager, bool enterOnInitialize)
        {
            if (view == null)
                throw new ArgumentNullException("view");
            if (actionQueueManager == null)
                throw new ArgumentNullException("actionQueueManager");

            var viewModel = (TViewModel)this;

            _actionQueueManager = actionQueueManager;
            View = view;
            ActionController = new ActionController<TView, TViewModel>(view, viewModel, actionQueueManager);
            _actionManager = new ActionManager<TView, TViewModel>(viewModel);
            _enterOnInitialize = enterOnInitialize;

            if (_deferCreated) return;

            Created();
        }

        /// <summary>
        /// Executes "Created" logic.
        /// </summary>
        private void Created()
        {
            if (_created) return; //Only create once!
            if (_disposed) return;

            _created = true;
            OnCreated();

            if (!_enterOnInitialize) return;

            Enter();
        }

        /// <summary>
        /// Called when this viewmodel is created and fully initialized.
        /// </summary>
        protected virtual void OnCreated()
        {
        }















        /// <summary>
        /// Defers the creational calls.
        /// This method can be used when there is additional initialization logic required.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Creational calls are OnCreated, the Enter Action, ...
        /// This should be called in the ctor of the ViewModel to successfully defer the creational logic.
        /// </remarks>
        public IDisposable DeferCreated()
        {
            return new CreationalDeferral(this);
        }

        /// <summary>
        /// Private class to manage deferral of the creational logic of the viewmodel.
        /// </summary>
        private class CreationalDeferral : IDisposable
        {
            private readonly ViewModel<TView, TViewModel> _viewModel;
            private bool _disposed;

            /// <summary>
            /// Initializes a new instance of the <see cref="CreationalDeferral"/> class.
            /// </summary>
            /// <param name="viewModel">The view model.</param>
            public CreationalDeferral(ViewModel<TView, TViewModel> viewModel)
            {
                _viewModel = viewModel;
                _viewModel._deferCreated = true;
            }

            /// <summary>
            /// Finalizes an instance of the <see cref="CreationalDeferral"/> class.
            /// </summary>
            ~CreationalDeferral()
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
            /// Releases unmanaged and - optionally - managed resources.
            /// </summary>
            /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
            private void Dispose(bool disposing)
            {
                if (_disposed) return;

                if (disposing)
                {
                    _viewModel.Created();
                }

                _disposed = true;
            }
        }
    }
}
