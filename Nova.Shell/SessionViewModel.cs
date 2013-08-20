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
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Nova.Controls;
using Nova.Shell.Actions.Session;
using Nova.Shell.Builders;
using Nova.Shell.Library;
using Nova.Shell.Managers;
using System.Windows.Input;
using Nova.Shell.Views;
using RESX = Nova.Shell.Properties.Resources;
using System.Windows.Data;
using Nova.Library;


namespace Nova.Shell
{
    /// <summary>
    /// The Session ViewModel
    /// </summary>
    public class SessionViewModel : ViewModel<SessionView, SessionViewModel>, ISessionViewModel
    {
        internal const string CurrentViewConstant = "CurrentSessionContentView";
        internal const string CreateNextViewConstant = "CreateNextSessionContentView";
        internal const string ViewTypeConstant = "ViewTypeConstant";
        internal const string ViewModelTypeConstant = "ViewModelTypeConstant";

        private IView _currentView;
        private readonly dynamic _model;
        private readonly dynamic _applicationModel;

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
            get { return _applicationModel; }
        }

        /// <summary>
        /// Gets the session model.
        /// </summary>
        /// <value>
        /// The session model.
        /// </value>
        public dynamic Model
        {
            get { return _model; }
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SessionViewModel" /> class.
        /// </summary>
        public SessionViewModel()
        {
            _applicationModel = ((App) Application.Current).Model;
            _model = new ExpandoObject();
        }
        
        /// <summary>
        /// Called when this viewmodel is created and fully initialized.
        /// </summary>
        protected override void OnCreated()
        {
            SetKnownActionTypes(typeof(SessionLeaveAction), typeof(NavigationAction)); //Optimalization

            var titleBinding = new Binding("CurrentView.Title") { Mode=BindingMode.OneWay };
            BindingOperations.SetBinding(View, SessionView.TitleProperty, titleBinding);

            NavigationActionManager = new NavigationActionManager(View);

            var enterAction = CreateAction<SessionEnterAction>();
            SetEnterAction(enterAction);

            var leaveAction = CreateAction<SessionLeaveAction>();
            SetLeaveAction(leaveAction);
        }
        
        /// <summary>
        /// Gets or sets the current view.
        /// </summary>
        /// <value>
        /// The current view.
        /// </value>
        public IView CurrentView
        {
            get { return _currentView; }
            internal set
            {
                SetValue(ref _currentView, value);
            }
        }

        /// <summary>
        /// Called after entering this session.
        /// </summary>
        public void OnAfterEnter()
        {
            //TODO: Temporary until more data is passed along (e.g. when the user wants to open a certain page in a new session)
            View._NovaTree.NavigateToStartupPage();
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
        public TPageView Create<TPageView, TPageViewModel>(IView parent)
            where TPageViewModel : ContentViewModel<TPageView, TPageViewModel>, new()
            where TPageView : class, IView, new()
        {
            var page = CreateView<TPageView, TPageViewModel>(parent, false);

            var initializer = new Dictionary<string, object> {{"Session", this}};

            ((ContentViewModel<TPageView, TPageViewModel>)page.ViewModel).Initialize(initializer);


            return page;
        }
        /// <summary>
        /// Creates a page specifically for the content zone and fills in the session model.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        /// <returns></returns>
        public TPageView Create<TPageView, TPageViewModel>()
            where TPageViewModel : ContentViewModel<TPageView, TPageViewModel>, new()
            where TPageView : class, IView, new()
        {
            return Create<TPageView, TPageViewModel>(View);
        }
        
        /// <summary>
        /// Creates the navigational action.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public ICommand CreateNavigationalAction<TPageView, TPageViewModel>()
            where TPageView : ExtendedContentControl<TPageView, TPageViewModel>, new() 
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
        /// Creates a wizard builder.
        /// </summary>
        /// <returns></returns>
        IWizardBuilder ISessionViewModel.CreateWizardBuilder()
        {
            return new WizardBuilder();
        }

        /// <summary>
        /// Creates the wizard.
        /// </summary>
        /// <returns></returns>
        void ISessionViewModel.StackWizard(IWizardBuilder builder)
        {
            var wizardBuilder = (WizardBuilder)builder;

            var overlay = new Overlay();

            Grid.SetRowSpan(overlay, 2);
            Grid.SetColumnSpan(overlay, 2);

            Grid.SetRow(overlay, 0);
            Grid.SetColumn(overlay, 0);

            overlay.Delay = 0;
            overlay.MinimumDuration = 0;
            overlay.AnimationSpeed = 1;

            overlay.IsLoading = true;

            var wizard = CreateContentControl<WizardView, WizardViewModel>(false);

            WizardViewModel wizardViewModel = wizard.ViewModel;

            overlay.Tag = wizardViewModel.ID;
            wizardViewModel.Initialize(this, wizardBuilder);

            var canvas = new Canvas();
            canvas.Children.Add(wizard);

            overlay.Content = canvas;
            View._root.Children.Add(overlay);
        }

        /// <summary>
        /// unstacks a wizard.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="context">The context.</param>
        void ISessionViewModel.UnstackWizard(Guid id, ActionContext context)
        {
            var frameworkElements = View._root.Children.OfType<FrameworkElement>().Where(x => x.Tag != null).ToList();
            for (var i = frameworkElements.Count - 1; i >= 0; i--)
            {
                var child = frameworkElements[i];
                var input = child.Tag.ToString();

                Guid tag;
                if (!Guid.TryParse(input, out tag)) continue;
                if (tag != id) continue;

                View._root.Children.Remove(child);
                break;
            }

           //TODO: CurrentView.ViewModel.InvokeReturn(id, context);
        }

        /// <summary>
        /// Disposes the managed resources.
        /// </summary>
        protected override void DisposeManagedResources()
        {
            ((IDisposable)NavigationActionManager).Dispose();
        }
    }
}
