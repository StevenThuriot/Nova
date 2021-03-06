﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
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
        private readonly dynamic _model = new ExpandoObject();
        private readonly LinkedList<StepInfo> _steps;

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


        /// <summary>
        /// The view model property
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof (IViewModel), typeof (MultiStepView), new PropertyMetadata(null));


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
        /// Gets the session view model.
        /// </summary>
        /// <value>
        /// The session view model.
        /// </value>
        private readonly ISessionViewModel _sessionViewModel;

        /// <summary>
        /// Gets or sets the validation control.
        /// </summary>
        /// <value>
        /// The validation control.
        /// </value>
        public ValidationControl ValidationControl { get; set; }
        
        /// <summary>
        /// Gets the view model.
        /// </summary>
        /// <value>
        /// The view model.
        /// </value>
        public IViewModel ViewModel
        {
            get
            {
                if (Dispatcher.CheckAccess())
                    return (IViewModel)GetValue(ViewModelProperty);

                return Dispatcher.Invoke(() => ViewModel, DispatcherPriority.Send);
            }
            private set { SetValue(ViewModelProperty, value); }
        }

        /// <summary>
        /// Gets the group id.
        /// </summary>
        /// <value>
        /// The group id.
        /// </value>
        public Guid GroupId
        {
            get
            {
                return _groupId;
            }
        }

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
            private set { SetValue(IsLoadingProperty, value); }
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
            private set { SetValue(TitleProperty, value); }
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

                ViewModel = null;
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
            private set { SetValue(CurrentViewProperty, value); }
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

            var node = (LinkedListNode<NovaStep>) e.NewValue;
            var step = node.Value;

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

            _sessionViewModel = sessionViewModel;
            
            SnapsToDevicePixels = true;
            FocusVisualStyle = null;

            TextOptions.SetTextRenderingMode(this, TextRenderingMode.ClearType);
            TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);

            VisualTextRenderingMode = TextRenderingMode.ClearType;


            var stepInfos = initialView.List.Select(x => new StepInfo(x.Title, x.ViewType, x.ViewModelType, x.NodeId));
            _steps = new LinkedList<StepInfo>(stepInfos);
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
        /// <param name="entityId">The entity ID.</param>
        /// <returns></returns>
        public bool FocusControl(string fieldName, Guid entityId)
        {
            if (FocusHelper.FocusControl(this, fieldName, entityId))
                return true;

            var node = CurrentView.List.First;

            while (node != null && node.Value != null)
            {
                var step = node.Value;
                if (!step.IsViewInitialized) continue;

                var view = step.View;
                if (view.FocusControl(fieldName, entityId))
                {
                    return DoStep(node);
                }

                node = node.Next;
            }

            return false;
        }



















        /// <summary>
        /// Attempts to do a step to the specified step.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// True is successful.
        /// </returns>
        public bool DoStep(Guid id)
        {
            var node = CurrentView.List.First;

            while (node != null)
            {
                if (node.Value.NodeId == id)
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


        internal bool HasStep(NovaTreeNodeStep treeStep)
        {
            if (treeStep.GroupId != _groupId)
                return false;

            var stepId = treeStep.Id;
            return _steps.Any(x => x.NodeId == stepId);
        }

        internal bool GetOrCreateStep(NovaTreeNodeStep treeStep, out IView view)
        {
            view = null;

            if (treeStep.GroupId != _groupId)
                return false;

            var viewType = treeStep.PageType;
            var viewModelType = treeStep.ViewModelType;

            var node = CurrentView.List.First;

            while (node != null)
            {
                var novaStep = node.Value;
                if (novaStep.ViewType == viewType && novaStep.ViewModelType == viewModelType)
                {

                    break;
                }

                node = node.Next;
            }

            if (node == null || node.Value == null) 
                return false;

            view = node.Value.GetOrCreateView(this);
            return true;
        }

        /// <summary>
        /// Creates a page specifically for the content zone and fills in the session model.
        /// </summary>
        /// <typeparam name="TView">The type of the view.</typeparam>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="novaStep">The nova step.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">parent</exception>
        /// <exception cref="System.NotSupportedException"></exception>
        internal TView CreateStep<TView, TViewModel>(NovaStep novaStep)
            where TView : class, IView, new()
            where TViewModel : ContentViewModel<TView, TViewModel>, new()
        {
            if (!Dispatcher.CheckAccess())
                return Dispatcher.Invoke(() => CreateStep<TView, TViewModel>(novaStep), DispatcherPriority.Send);

            var view = _sessionViewModel.CreateView<TView, TViewModel>(this, false);

            var nodeId = novaStep.NodeId;
            var node = _steps.First;
            
            while (node != null)
            {
                if (node.Value.NodeId == nodeId)
                    break;

                node = node.Next;
            }

            var initializer = new Dictionary<string, object>
            {
                {"Session", _sessionViewModel},
                {"Node", node},
                {"Model", _model}
            };

            var wizard = _parent as WizardView;
            if (wizard != null)
            {
                initializer.Add("Wizard", wizard.ViewModel);
            }

            ((ContentViewModel<TView, TViewModel>)view.ViewModel).Initialize(initializer);

            return view;
        }
        
        public NovaStep GetNovaStep(StepInfo step)
        {
            return GetNovaStep(step.NodeId);
        }

        public NovaStep GetNovaStep(Guid nodeId)
        {
            var node = CurrentView.List.First;
            
            while (node != null && node.Value != null)
            {
                if (node.Value.NodeId == nodeId)
                    return node.Value;

                node = node.Next;
            }

            throw new ArgumentOutOfRangeException("step");
        }

        public LinkedListNode<StepInfo> GetStepInfoNode(NovaStep current)
        {
            return GetStepInfoNode(current.NodeId);
        }

        public LinkedListNode<StepInfo> GetStepInfoNode(Guid nodeId)
        {
            var node = _steps.First;

            while (node != null)
            {
                if (node.Value.NodeId == nodeId)
                    break;

                node = node.Next;
            }

            return node;
        }
    }
}
