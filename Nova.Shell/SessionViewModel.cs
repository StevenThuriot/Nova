using Nova.Library;

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
using Nova.Controls;
using Nova.Shell.Actions.Session;
using Nova.Shell.Library;
using Nova.Shell.Managers;
using Nova.Shell.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using RESX = Nova.Shell.Properties.Resources;


namespace Nova.Shell
{
    /// <summary>
    /// The Session ViewModel
    /// </summary>
    public class SessionViewModel : ViewModel<SessionView, SessionViewModel>, ISessionViewModel
    {
        internal const string CurrentViewConstant = "CurrentSessionContentView";
        internal const string NextViewConstant = "NextSessionContentView";

        private IDisposable _Deferral;
        private IView _CurrentView;
        private string _Title;
        private IEnumerable<NovaTreeNode> _TreeNodes;
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
        /// Gets or sets the tree nodes.
        /// </summary>
        /// <value>
        /// The tree nodes.
        /// </value>
        public IEnumerable<NovaTreeNode> TreeNodes
        {
            get { return _TreeNodes; }
            set { SetValue(ref _TreeNodes, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionViewModel" /> class.
        /// </summary>
        public SessionViewModel()
        {
            _Deferral = DeferCreated(); //Defer Created logic so we can call it manually in our extended initialize method.
            _Title = RESX.EmptySession;

            _ApplicationModel = ((App) Application.Current).Model;
            _Model = new ExpandoObject();
        }

        /// <summary>
        /// Initializes the SessionViewModel.
        /// </summary>
        /// <param name="module">The module.</param>
        /// <exception cref="System.ArgumentNullException">module</exception>
        internal void Initialize(NovaModule module)
        {
            if (module == null)
                throw new ArgumentNullException("module");

            NavigationActionManager = new NavigationActionManager(View);
            TreeNodes = module.BuildNovaTreeNodes(this);

            _Deferral.Dispose();
            _Deferral = null;
        }

        /// <summary>
        /// Called when this viewmodel is created and fully initialized.
        /// </summary>
        protected override void OnCreated()
        {
            SetKnownActionTypes(typeof(SessionLeaveStep), typeof(NavigationAction)); //Optimalization

            var leaveAction = CreateAction<SessionLeaveStep>();
            SetLeaveAction(leaveAction);
        }

        /// <summary>
        /// Called after enter.
        /// </summary>
        protected void OnAfterEnter()
        {
            //TODO: Temporary until more data is passed along (e.g. when the user wants to open a certain page in a new session)
            var startUpNode = TreeNodes.FirstOrDefault(x => x.IsStartupNode) ?? TreeNodes.First();
            startUpNode.Navigate();
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
            ((IDisposable)NavigationActionManager).Dispose();

            if (_Deferral == null) return;

            _Deferral.Dispose();
            _Deferral = null;
        }
    }
}
