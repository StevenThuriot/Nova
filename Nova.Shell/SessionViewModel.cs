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

using Nova.Base;
using Nova.Controls;
using Nova.Shell.Actions.Session;

namespace Nova.Shell
{
    public class SessionViewModel : ViewModel<SessionView, SessionViewModel>
    {
        internal const string CurrentViewConstant = "CurrentView";
        internal const string NextViewConstant = "NextView";

        private string _Title = SessionViewResources.EmptySession;
        private IView _CurrentView;

        /// <summary>
        /// Called when [created].
        /// </summary>
        protected override void OnCreated()
        {
            SetKnownActionTypes(typeof(SessionLeaveStep));

            LeaveAction = Actionflow<SessionView, SessionViewModel>.New<SessionLeaveStep>(View, this);
        }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title
        {
            get { return _Title; }
            private set { SetValue(ref _Title, value); }
        }

        /// <summary>
        /// Gets or sets the current view.
        /// </summary>
        /// <value>
        /// The current view.
        /// </value>
        internal IView CurrentView
        {
            get { return _CurrentView; }
            set
            {
                if (value == null || !SetValue(ref _CurrentView, value)) return;

                Title = _CurrentView.Title;
                View._ContentZone.Navigate(_CurrentView);
            }
        }

        /// <summary>
        /// Navigates this instance to the specified page.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        public void Navigate<TPageView, TPageViewModel>() 
            where TPageViewModel : ViewModel<TPageView, TPageViewModel>, new() 
            where TPageView : ExtendedPage<TPageView, TPageViewModel>, new()
        {
            var nextView = CreatePage<TPageView, TPageViewModel>(enterOnInitialize: false);

            var current = ActionContextEntry.Create(CurrentViewConstant, _CurrentView, false);
            var next = ActionContextEntry.Create(NextViewConstant, nextView, false);

            InvokeAction<NavigationAction>(current, next);
        }

        public void OnAfterEnter()
        {
            //TODO: Temporary default
            Navigate<TestPage, TestPageViewModel>();
        }
    }
}
