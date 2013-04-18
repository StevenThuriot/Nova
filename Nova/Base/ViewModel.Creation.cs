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
using Nova.Base.ActionMethodRepository;
using Nova.Controls;
using Nova.Threading;

namespace Nova.Base
{
    public abstract partial class ViewModel<TView, TViewModel>
    {
        /// <summary>
        /// Creates the specified viewmodel.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="actionQueueManager">The action queue manager.</param>
        /// <param name="enterOnInitialize">if set to <c>true</c>, the Enter Action will be triggered automatically. Default is true.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">view</exception>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        internal static TViewModel Create(TView view, IActionQueueManager actionQueueManager, bool enterOnInitialize = true)
        {
            if (view == null)
                throw new ArgumentNullException("view");
            if (actionQueueManager == null)
                throw new ArgumentNullException("actionQueueManager");

            OnActionMethodRepository.CacheOnActionLogic<TView, TViewModel>();

            var viewModel = new TViewModel();
            viewModel.Initialize(view, actionQueueManager, enterOnInitialize);

            return viewModel;
        }

        /// <summary>
        /// Initializes the viewmodel and triggers all the needed logic.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="actionQueueManager">The action queue manager.</param>
        /// <param name="enterOnInitialize">if set to <c>true</c>, the Enter Action will be triggered automatically. Default is true.</param>
        /// <exception cref="System.ArgumentNullException">view</exception>
        internal void Initialize(TView view, IActionQueueManager actionQueueManager, bool enterOnInitialize)
        {
            if (view == null)
                throw new ArgumentNullException("view");
            if (actionQueueManager == null)
                throw new ArgumentNullException("actionQueueManager");

            var viewModel = (TViewModel)this;

            _ActionQueueManager = actionQueueManager;
            View = view;
            ActionController = new ActionController<TView, TViewModel>(view, viewModel, actionQueueManager);
            _ActionManager = new ActionManager<TView, TViewModel>(view, viewModel);

            OnCreated();

            if (enterOnInitialize)
            {
                Enter();
            }
        }
        
        /// <summary>
        /// Creates a new page with the current window as parent.
        /// </summary>
        /// <param name="enterOnInitialize">if set to <c>true</c>, the Enter Action will be triggered automatically. Default is true.</param>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        public TPageView CreatePage<TPageView, TPageViewModel>(bool enterOnInitialize = true)
            where TPageViewModel : ViewModel<TPageView, TPageViewModel>, new()
            where TPageView : ExtendedPage<TPageView, TPageViewModel>, new()
        {
            return ExtendedPage<TPageView, TPageViewModel>.Create(View, _ActionQueueManager, enterOnInitialize);
        }
    }
}
