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

using Nova.Base;
using Nova.Controls;

namespace Nova.Shell.Library
{
    /// <summary>
    /// A viewmodel for pages that belong to a session.
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    public abstract class ContentViewModel<TView, TViewModel> : ViewModel<TView, TViewModel>
        where TView : class, IView
        where TViewModel : ViewModel<TView, TViewModel>, new()
    {
        private dynamic _Session;
        private dynamic _SessionModel;

        /// <summary>
        /// Initializes the ContentViewModel using the parent session instance.
        /// </summary>
        /// <param name="session">The parent session.</param>
        internal void Initialize(IViewModel session)
        {
            _Session = session;
            SessionModel = _Session.SessionModel;
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
    }
}
