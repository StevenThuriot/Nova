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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Nova.Controls;
using Nova.Helpers;
using Nova.Library;
using Nova.Shell.Domain;
using Nova.Shell.Library;
using Nova.Validation;

namespace Nova.Shell.Views
{
    /// <summary>
    /// A view that has several steps inside.
    /// </summary>
    /// <remarks>Useful for sharing a model between these steps.</remarks>
    internal class MultiStepView : ContentPresenter, IView
    {
        private int _loadingCounter;
        private readonly object _lock = new object();

        // ReSharper disable StaticFieldInGenericType

        /// <summary>
        ///     A value indicating whether this instance is loading.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register("IsLoading", typeof(bool), typeof(MultiStepView), new PropertyMetadata(false));

        /// <summary>
        /// The title property
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(MultiStepView), new PropertyMetadata(""));

        // ReSharper restore StaticFieldInGenericType

        /// <summary>
        /// Flag wether this instance is disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// The parent view
        /// </summary>
        private IView _parent;

        /// <summary>
        /// The group id
        /// </summary>
        private readonly Guid _groupId;

        /// <summary>
        /// Gets or sets the validation control.
        /// </summary>
        /// <value>
        /// The validation control.
        /// </value>
        public ValidationControl ValidationControl { get; set; }

        /// <summary>
        /// Gets the session view model.
        /// </summary>
        /// <value>
        /// The session view model.
        /// </value>
        public ISessionViewModel SessionViewModel { get; private set; }
        
        /// <summary>
        /// Gets the view model.
        /// </summary>
        /// <value>
        /// The view model.
        /// </value>
        public IViewModel ViewModel { get; private set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is loading.
        ///     This can also be interpreted as "busy".
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is loading; otherwise, <c>false</c>.
        /// </value>
        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        ///     Starts the animated loading.
        /// </summary>
        public void StartLoading()
        {
            lock (_lock)
            {
                var isLoading = ++_loadingCounter > 0;

                if (IsLoading == isLoading) return;

                IsLoading = isLoading;

                if (isLoading && _parent != null)
                {
                    _parent.StartLoading();
                }
            }
        }

        /// <summary>
        ///     Stops the animated loading.
        /// </summary>
        public void StopLoading()
        {
            lock (_lock)
            {
                var isLoading = --_loadingCounter > 0;

                if (IsLoading == isLoading) return;

                IsLoading = isLoading;

                if (!isLoading && _parent != null)
                {
                    _parent.StopLoading();
                }
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// ExtendedContentPresenter is reclaimed by garbage collection.
        /// </summary>
        ~MultiStepView()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (_parent != null)
                {
                    if (IsLoading)
                    {
                        //Make sure the parent doesn't keep thinking the child is still loading yet unexistent.
                        _parent.StopLoading();
                    }

                    _parent = null;
                }

                foreach (var step in CurrentView.List.Select(x => x.View).Where(x => x != null))
                {
                    step.Dispose();
                }
            }

            _disposed = true;
        }















        /// <summary>
        /// Initializes the <see cref="MultiStepView" /> class.
        /// </summary>
        static MultiStepView()
        {
            var propertyMetadata = new PropertyMetadata(null, OnCurrentViewChanged, OnCoerceCurrentView);
            CurrentViewProperty = DependencyProperty.Register("CurrentView", typeof (LinkedListNode<NovaStep>), typeof (MultiStepView), propertyMetadata);
        }

        /// <summary>
        /// The current view property
        /// </summary>
        public static readonly DependencyProperty CurrentViewProperty;


        /// <summary>
        /// Gets or sets the current view.
        /// </summary>
        /// <value>
        /// The current view.
        /// </value>
        public LinkedListNode<NovaStep> CurrentView
        {
            get { return (LinkedListNode<NovaStep>)GetValue(CurrentViewProperty); }
            set { SetValue(CurrentViewProperty, value); }
        }
        
        private static object OnCoerceCurrentView(DependencyObject d, object basevalue)
        {
            if (basevalue != null)
                return basevalue;

            var multiStepView = (MultiStepView) d;
            return multiStepView.CurrentView;
        }

        private static void OnCurrentViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var multiStepView = (MultiStepView) d;

            var step = ((LinkedListNode<NovaStep>) e.NewValue).Value;

            var view = step.GetOrCreateView(multiStepView);

            multiStepView.Content = view;
            multiStepView.ViewModel = view.ViewModel;

            multiStepView.Title = step.Title;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiStepView" /> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="sessionViewModel">The session view model.</param>
        /// <param name="groupId">The group id.</param>
        /// <param name="initialView"></param>
        /// <exception cref="System.ArgumentNullException">steps</exception>
        /// <exception cref="System.ArgumentException">@Steps cannot be empty.;steps</exception>
        public MultiStepView(IView parent, ISessionViewModel sessionViewModel, Guid groupId, LinkedListNode<NovaStep> initialView)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            if (sessionViewModel == null)
                throw new ArgumentNullException("sessionViewModel");

            if (initialView == null)
                throw new ArgumentNullException("initialView");

            _parent = parent;
            _groupId = groupId;

            SessionViewModel = sessionViewModel;
            
            SnapsToDevicePixels = true;
            FocusVisualStyle = null;

            TextOptions.SetTextRenderingMode(this, TextRenderingMode.ClearType);
            TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);

            VisualTextRenderingMode = TextRenderingMode.ClearType;
            
            CurrentView = initialView;
        }
        
        /// <summary>
        /// Focuses the control.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public bool FocusControl(string fieldName)
        {
            return FocusControl(fieldName, (Guid)NovaValidation.EntityIDProperty.DefaultMetadata.DefaultValue);
        }

        /// <summary>
        /// Focuses the control.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="entityID">The entity ID.</param>
        /// <returns></returns>
        public bool FocusControl(string fieldName, Guid entityID)
        {
            //TODO: foreach view in _views
                    //if focus; break

            return FocusHelper.FocusControl(this, fieldName, entityID);
        }




















        /// <summary>
        /// Determines whether this instance can go to next step.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance [can go to next step]; otherwise, <c>false</c>.
        /// </returns>
        public bool CanGoToNextStep()
        {
            return CurrentView.Next != null;
        }

        /// <summary>
        /// Goes to next step.
        /// </summary>
        public void GoToNextStep()
        {
            DoStep(CurrentView.Next);
        }

        /// <summary>
        /// Determines whether this instance can go to previous step.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance [can go to previous step]; otherwise, <c>false</c>.
        /// </returns>
        public bool CanGoToPreviousStep()
        {
            return CurrentView.Previous != null;
        }

        /// <summary>
        /// Goes to previous step.
        /// </summary>
        public void GoToPreviousStep()
        {
            DoStep(CurrentView.Previous);
        }

        /// <summary>
        /// Attempts to do a step to the specified step.
        /// </summary>
        /// <param name="stepName">Name of the step.</param>
        /// <returns>True is successful.</returns>
        public bool DoStep(string stepName)
        {
            var node = CurrentView.List.First;

            while (node != null)
            {
                if (node.Value.Title == stepName)
                    break;

                node = node.Next;
            }
            
            return DoStep(node);
        }
        
        public bool DoStep(IView view)
        {
            var node = CurrentView.List.First;

            while (node != null)
            {
                if (node.Value.View == view)
                    break;

                node = node.Next;
            }

            return DoStep(node);
        }

        private bool DoStep(LinkedListNode<NovaStep> node)
        {
            if (node == null || node.Value == null)
                return false;

            CurrentView = node;
            return true;
        }

        internal bool GetOrCreateStep(NovaTreeNodeStep treeStep, out IView view)
        {
            view = null;

            if (treeStep.GroupId != _groupId)
                return false;

            var viewType = treeStep.PageType;
            var viewModelType = treeStep.ViewModelType;

            var step = CurrentView.List.FirstOrDefault(x => x.ViewType == viewType && x.ViewModelType == viewModelType);

            if (step == null) 
                return false;

            view = step.GetOrCreateView(this);
            return true;
        }

        /// <summary>
        /// Creates a page specifically for the content zone and fills in the session model.
        /// </summary>
        /// <typeparam name="TView">The type of the view.</typeparam>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">parent</exception>
        /// <exception cref="System.NotSupportedException"></exception>
        internal TView CreateStep<TView, TViewModel>()
            where TView : class, IView, new()
            where TViewModel : ContentViewModel<TView, TViewModel>, new()
        {
            var view = SessionViewModel.CreateView<TView, TViewModel>(this, false);
            return view;
        }
    }
}
