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
        private dynamic _Session;
        private dynamic _SessionModel;
        private INavigationActionManager _NavigationActionManager;

        /// <summary>
        /// Initializes the ContentViewModel using the parent session instance.
        /// </summary>
        /// <param name="session">The parent session.</param>
        internal void Initialize(IViewModel session)
        {
            _Session = session;
            SessionModel = _Session.SessionModel;
            _NavigationActionManager = _Session.NavigationActionManager;

            OnSessionInitialized();
        }


        /// <summary>
        /// Gets the session model.
        /// </summary>
        /// <value>
        /// The session model.
        /// </value>
        public dynamic SessionModel
        {
            get { return _SessionModel; }
            private set { SetValue(ref _SessionModel, value); }
        }

        /// <summary>
        /// Navigates the parent session to the specified page.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        public void Navigate<TPageView, TPageViewModel>()
            where TPageViewModel : ContentViewModel<TPageView, TPageViewModel>, new()
            where TPageView : ExtendedPage<TPageView, TPageViewModel>, new()
        {
            _Session.Navigate<TPageView, TPageViewModel>();
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
            return _NavigationActionManager.New<TPageView, TPageViewModel>();
        }


        /// <summary>
        /// Called when this session data is initialized.
        /// </summary>
        protected virtual void OnSessionInitialized()
        {
        }

        /// <summary>
        /// Disposes the managed resources.
        /// </summary>
        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();

            _Session = null;
            _SessionModel = null;
            _NavigationActionManager = null;
        }
    }
}
