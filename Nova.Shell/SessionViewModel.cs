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
using System.Windows;
using System.Windows.Input;
using Nova.Base;
using Nova.Base.Actions;
using Nova.Controls;
using Nova.Shell.Actions.Session;
using Nova.Shell.Library;
using Nova.Shell.Managers;

namespace Nova.Shell
{
    /// <summary>
    /// The Session ViewModel
    /// </summary>
    public class SessionViewModel : ViewModel<SessionView, SessionViewModel>, ISessionViewModel
    {
        internal const string CurrentViewConstant = "CurrentSessionContentView";
        internal const string NextViewConstant = "NextSessionContentView";

        private IView _CurrentView;
        private string _Title;
        private readonly dynamic _Model;
        private readonly dynamic _ApplicationModel;

        /// <summary>
        /// Gets the navigation action manager.
        /// </summary>
        /// <value>
        /// The navigation action manager.
        /// </value>
        public INavigationActionManager NavigationActionManager { get; private set; }

        /// <summary>
        /// Gets the application model.
        /// </summary>
        /// <value>
        /// The application model.
        /// </value>
        public dynamic ApplicationModel
        {
            get { return _ApplicationModel; }
        }

        /// <summary>
        /// Gets the session model.
        /// </summary>
        /// <value>
        /// The session model.
        /// </value>
        public dynamic Model
        {
            get { return _Model; }
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
            set { SetValue(ref _Title, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionViewModel" /> class.
        /// </summary>
        public SessionViewModel()
        {
            _Title = SessionViewResources.EmptySession;

            _ApplicationModel = ((App) Application.Current).Model;
            _Model = new ExpandoObject();
        }

        /// <summary>
        /// Called when this viewmodel is created and fully initialized.
        /// </summary>
        protected override void OnCreated()
        {
            SetKnownActionTypes(typeof(SessionLeaveStep), typeof(NavigationAction)); //Optimalization

            var leaveAction = LeaveAction<SessionView, SessionViewModel>.New<SessionLeaveStep>(View, this);
            SetLeaveAction(leaveAction);

            NavigationActionManager = new NavigationActionManager(View);
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
        /// Navigates the parent session to the specified page.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        public void Navigate<TPageView, TPageViewModel>()
            where TPageViewModel : ContentViewModel<TPageView, TPageViewModel>, new()
            where TPageView : ExtendedPage<TPageView, TPageViewModel>, new()
        {
            var createNextView = new Func<IView>(CreatePage<TPageView, TPageViewModel>);
            var next = ActionContextEntry.Create(NextViewConstant, createNextView, false);

            InvokeAction<NavigationAction>(next);
        }

        /// <summary>
        /// Creates the navigational action.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public ICommand CreateNavigationalAction<TPageView, TPageViewModel>() 
            where TPageView : ExtendedPage<TPageView, TPageViewModel>, new() 
            where TPageViewModel : ContentViewModel<TPageView, TPageViewModel>, new()
        {
            return NavigationActionManager.New<TPageView, TPageViewModel>();
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
