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
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Nova.Helpers;
using Nova.Threading;
using System.Threading;
using Nova.Validation;
using Nova.Library;
using Nova.Library.Actions;

namespace Nova.Controls
{
    /// <summary>
    ///     A default Window class that has added logic for MVVM.
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    public abstract class ExtendedWindow<TView, TViewModel> : Window, IView
        where TViewModel : ViewModel<TView, TViewModel>, new()
        where TView : ExtendedWindow<TView, TViewModel>
    {
        /// <summary>
        ///     A value indicating whether this instance is loading.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")] public static readonly
// ReSharper disable StaticFieldInGenericType
            DependencyProperty IsLoadingProperty = DependencyProperty.Register("IsLoading", typeof (bool), typeof (ExtendedWindow<TView, TViewModel>), new PropertyMetadata(false));

// ReSharper restore StaticFieldInGenericType

        private bool _disposed;
        private TViewModel _viewModel;

        private int _loadingCounter;
        private ActionQueueManager _actionQueueManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExtendedWindow&lt;TView, TViewModel&gt;" /> class.
        /// </summary>
        protected ExtendedWindow()
        {
            SnapsToDevicePixels = true;

            TextOptions.SetTextRenderingMode(this, TextRenderingMode.ClearType);
            TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);
            VisualTextRenderingMode = TextRenderingMode.ClearType;

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            
            _actionQueueManager = new ActionQueueManager();

            ViewModel = ViewModel<TView, TViewModel>.Create((TView) this, _actionQueueManager);

            Closing += (sender, args) => ViewModel.InvokeAction<LeaveAction<TView, TViewModel>>();
        }

        /// <summary>
        /// Gets the view model.
        /// </summary>
        /// <value>
        /// The view model.
        /// </value>
        public TViewModel ViewModel
        {
            get { return _viewModel; }
            private set
            {
                if (_viewModel == value) return;

                _viewModel = value;
                DataContext = value;
            }
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
            return FocusHelper.FocusControl(this, fieldName, entityID);
        }

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
        IViewModel IView.ViewModel
        {
            get { return ViewModel; }
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
            get { return (bool) GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        /// <summary>
        ///     Starts the animated loading.
        /// </summary>
        public virtual void StartLoading()
        {
            IsLoading = Interlocked.Increment(ref _loadingCounter) > 0;
            UpdateCursor(true);
        }

        /// <summary>
        ///     Stops the animated loading.
        /// </summary>
        public virtual void StopLoading()
        {
            IsLoading = Interlocked.Decrement(ref _loadingCounter) > 0;

            if (!IsLoading)
            {
                UpdateCursor(false);
            }
        }

        /// <summary>
        /// Updates the cursor.
        /// </summary>
        /// <param name="isloading">if set to <c>true</c> [isloading].</param>
        protected virtual void UpdateCursor(bool isloading)
        {
            Cursor = isloading ? Cursors.AppStarting : Cursors.Arrow;
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Releases unmanaged resources and performs other cleanup operations before the
        ///     <see cref="ExtendedWindow&lt;TView, TViewModel&gt;" /> is reclaimed by garbage collection.
        /// </summary>
        ~ExtendedWindow()
        {
            Dispose(false);
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (_viewModel != null)
                {
                    _viewModel.Dispose();
                }

                if (_actionQueueManager != null)
                {
                    _actionQueueManager.Dispose();
                    _actionQueueManager = null;
                }
            }

            _disposed = true;
        }
    }
}