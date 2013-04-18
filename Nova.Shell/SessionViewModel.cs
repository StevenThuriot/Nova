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
using Nova.Shell.Library.Interfaces;
using Nova.Shell.Managers;

namespace Nova.Shell
{
    public class SessionViewModel : ViewModel<SessionView, SessionViewModel>
    {
        internal const string CurrentViewConstant = "CurrentSessionContentView";
        internal const string NextViewConstant = "NextSessionContentView";

        /// <summary>
        /// Gets the navigation action manager.
        /// </summary>
        /// <value>
        /// The navigation action manager.
        /// </value>
        public INavigationActionManager NavigationActionManager { get; private set; }

        /// <summary>
        /// Gets the session model.
        /// </summary>
        /// <value>
        /// The session model.
        /// </value>
        public dynamic SessionModel { get; private set; }

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
            SetKnownActionTypes(typeof(SessionLeaveStep), typeof(NavigationAction));
            
            var leaveAction = Actionflow<SessionView, SessionViewModel>.New<SessionLeaveStep>(View, this);
            SetLeaveAction(leaveAction);

            NavigationActionManager = new NavigationActionManager(View);
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
                View.ContentZone.Navigate(_CurrentView);
            }
        }

        /// <summary>
        /// Called after enter.
        /// </summary>
        public void OnAfterEnter()
        {
            //TODO: Temporary default
            var createNextView = new Func<IView>(CreatePage<TestPage, TestPageViewModel>);
            var next = ActionContextEntry.Create(NextViewConstant, createNextView, false);

            InvokeAction<NavigationAction>(next);
        }

        /// <summary>
        /// Called before navigation.
        /// </summary>
        /// <param name="context">The context.</param>
        public void OnBeforeNavigation(ActionContext context)
        {
            var current = ActionContextEntry.Create(CurrentViewConstant, CurrentView, false);
            context.Add(current);
        }

        /// <summary>
        /// Creates a page specifically for the content zone and fills in the session model.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        /// <returns></returns>
        internal TPageView CreatePage<TPageView, TPageViewModel>()
            where TPageViewModel : ContentViewModel<TPageView, TPageViewModel>, new()
            where TPageView : ExtendedPage<TPageView, TPageViewModel>, new()
        {
            var page = CreatePage<TPageView, TPageViewModel>(false);
            page.ViewModel.Initialize(this);

            return page;
        }
        
        /// <summary>
        /// Determines whether the session is invalid.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the session is invalid; otherwise, <c>false</c>.
        /// </returns>
        public bool IsSessionValid()
        {
            if (!IsValid) //Session level
                return false;

            var currentView = CurrentView;

            return currentView == null || currentView.ViewModel.IsValid; //The content zone level.
        }

        /// <summary>
        /// Disposes the managed resources.
        /// </summary>
        protected override void DisposeManagedResources()
        {
            ((IDisposable) NavigationActionManager).Dispose();
        }
    }
}
