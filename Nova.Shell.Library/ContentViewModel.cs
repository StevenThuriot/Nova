﻿#region License

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
using System.Windows.Input;
using Nova.Base;
using Nova.Controls;
using Nova.Shell.Library.Interfaces;

namespace Nova.Shell.Library
{
    /// <summary>
    /// A viewmodel for pages that belong to a session.
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    public abstract class ContentViewModel<TView, TViewModel> : ViewModel<TView, TViewModel>, INavigatablePage
        where TView : class, IView
        where TViewModel : ViewModel<TView, TViewModel>, new()
    {
        private IDisposable _Deferral;
        private ISessionViewModel _Session;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentViewModel{TView, TViewModel}"/> class.
        /// </summary>
        protected ContentViewModel()
        {
            _Deferral = DeferCreated(); //Defer Created logic so we can call it manually in our extended initialize method.
        }

        /// <summary>
        /// Initializes the ContentViewModel using the parent session instance.
        /// </summary>
        /// <param name="session">The parent session.</param>
        internal void Initialize(ISessionViewModel session)
        {
            _Session = session;

            _Deferral.Dispose();
            _Deferral = null;
        }

        /// <summary>
        /// Gets the application model.
        /// </summary>
        /// <value>
        /// The application model.
        /// </value>
        public dynamic ApplicationModel
        {
            get { return _Session.ApplicationModel; }
        }

        /// <summary>
        /// Gets the session model.
        /// </summary>
        /// <value>
        /// The session model.
        /// </value>
        public dynamic SessionModel
        {
            get { return _Session.Model; }
        }
        
        /// <summary>
        /// Creates a navigational action that navigates the parent session to the specified page.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        public ICommand CreateNavigationalAction<TPageView, TPageViewModel>()
            where TPageViewModel : ContentViewModel<TPageView, TPageViewModel>, new()
            where TPageView : ExtendedPage<TPageView, TPageViewModel>, new()
        {
            return _Session.NavigationActionManager.New<TPageView, TPageViewModel>();
        }

        /// <summary>
        /// Disposes the managed resources.
        /// </summary>
        protected override void DisposeManagedResources()
        {
            _Session = null;

            if (_Deferral == null) return;

            _Deferral.Dispose();
            _Deferral = null;
        }
    }
}
