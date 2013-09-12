﻿#region License

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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
        }

        public override Task<bool> Enter(params ActionContextEntry[] parameters)
        {
            return InvokeActionAsync<SessionEnterAction>(parameters);
        }

        public override Task<bool> Leave(params ActionContextEntry[] parameters)
        {
            return InvokeActionAsync<SessionLeaveAction>(parameters);
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
            var current = ActionContextEntry.Create(ActionContextConstants.CurrentViewConstant, CurrentView, false);
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
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public ICommand CreateNavigationalAction<TPageView, TPageViewModel>(params ActionContextEntry[] parameters)
            where TPageView : ExtendedContentControl<TPageView, TPageViewModel>, new() 
            where TPageViewModel : ContentViewModel<TPageView, TPageViewModel>, new()
        {
            return NavigationActionManager.New<TPageView, TPageViewModel>(parameters);
        }

        /// <summary>
        /// Creates the navigational action.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        /// <param name="nodeId">The node id.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public ICommand CreateNavigationalAction<TPageView, TPageViewModel>(Guid nodeId, params ActionContextEntry[] parameters)
            where TPageView : ExtendedContentControl<TPageView, TPageViewModel>, new() 
            where TPageViewModel : ContentViewModel<TPageView, TPageViewModel>, new()
        {
            return NavigationActionManager.New<TPageView, TPageViewModel>(nodeId, parameters);
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
        public IWizardBuilder CreateWizardBuilder()
        {
            return new WizardBuilder();
        }

        /// <summary>
        /// Creates the wizard.
        /// </summary>
        /// <returns></returns>
        public void StackWizard(IWizardBuilder builder)
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

            var wizard = CreateContentControl<WizardView, WizardViewModel>();

            WizardViewModel wizardViewModel = wizard.ViewModel;

            var size = builder.Size;

            wizard.Width = size.Width;
            wizard.Height = size.Height;

            wizard.MinWidth = size.MinWidth;
            wizard.MinHeight = size.MinHeight;

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
        /// <param name="entries">The entries.</param>
        public void UnstackWizard(Guid id, IEnumerable<ActionContextEntry> entries)
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

           ((IContentViewModel)CurrentView.ViewModel).ReturnToUseCase(entries);
        }

        /// <summary>
        /// Shows the dialog box.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="image">The image.</param>
        public void ShowDialogBox(string message, ImageSource image = null)
        {
            var entries = new List<ActionContextEntry>();

            var entry = ActionContextEntry.Create(ActionContextConstants.DialogBoxMessage, message, false);
            entries.Add(entry);

            if (image != null)
            {
                var imageEntry = ActionContextEntry.Create(ActionContextConstants.DialogBoxImage, image, false);
                entries.Add(imageEntry);
            }

            var builder = CreateWizardBuilder();
            builder.AddStep<DialogView, DialogViewModel>(parameters: entries.ToArray());
            builder.Size = new ExtendedSize(480, 120, minWidth: 480, minHeight: 120);

            StackWizard(builder);
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
