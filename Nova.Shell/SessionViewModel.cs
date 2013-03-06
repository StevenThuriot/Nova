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

using System;
using System.Dynamic;
using Nova.Base;
using Nova.Controls;
using Nova.Shell.Actions.Session;
using Nova.Shell.Library;

namespace Nova.Shell
{
    public class SessionViewModel : ViewModel<SessionView, SessionViewModel>
    {
        /// <summary>
        /// Gets the session model.
        /// </summary>
        /// <value>
        /// The session model.
        /// </value>
        public dynamic SessionModel { get; private set; }

        internal const string CurrentViewConstant = "CurrentView";
        internal const string NextViewConstant = "NextView";

        private string _Title = SessionViewResources.EmptySession;
        private IView _CurrentView;

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionViewModel" /> class.
        /// </summary>
        public SessionViewModel()
        {
            SessionModel = new ExpandoObject();
        }

        /// <summary>
        /// Called when [created].
        /// </summary>
        protected override void OnCreated()
        {
            SetKnownActionTypes(typeof(SessionLeaveStep));
            
            var leaveAction = Actionflow<SessionView, SessionViewModel>.New<SessionLeaveStep>(View, this);
            SetLeaveAction(leaveAction);
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
        public IView CurrentView
        {
            get { return _CurrentView; }
            internal set
            {
                if (value == null || !SetValue(ref _CurrentView, value)) return;

                Title = _CurrentView.Title;
                View._ContentZone.Navigate(_CurrentView);
            }
        }

        /// <summary>
        /// Navigates this session to the specified page.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        public void Navigate<TPageView, TPageViewModel>() 
            where TPageViewModel : ContentViewModel<TPageView, TPageViewModel>, new() 
            where TPageView : ExtendedPage<TPageView, TPageViewModel>, new()
        {
            var current = ActionContextEntry.Create(CurrentViewConstant, CurrentView, false);

            var createNextView = new Func<IView>(CreatePage<TPageView, TPageViewModel>);
            var next = ActionContextEntry.Create(NextViewConstant, createNextView, false);

            InvokeAction<NavigationAction>(current, next);
        }

        /// <summary>
        /// Creates a page specifically for the content zone and fills in the session model.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        /// <returns></returns>
        private TPageView CreatePage<TPageView, TPageViewModel>()
            where TPageViewModel : ContentViewModel<TPageView, TPageViewModel>, new()
            where TPageView : ExtendedPage<TPageView, TPageViewModel>, new()
        {
            var page = CreatePage<TPageView, TPageViewModel>(false);
            page.ViewModel.Initialize(this);

            return page;
        }

        public void OnAfterEnter()
        {
            //TODO: Temporary default
            Navigate<TestPage, TestPageViewModel>();
        }
    }
}
