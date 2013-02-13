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
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Nova.Base;
using Nova.Base.Actions;
using Nova.Threading;
using System.Threading;

namespace Nova.Controls
{
    /// <summary>
    ///     A default Window class that has added logic for MVVM.
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    public abstract class ExtendedWindow<TView, TViewModel> : Window, IInternalView
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

        /// <summary>
        ///     The action queue manager
        /// </summary>
        private readonly IActionQueueManager _ActionQueueManager;
        private bool _Disposed;
        private TViewModel _ViewModel;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExtendedWindow&lt;TView, TViewModel&gt;" /> class.
        /// </summary>
        protected ExtendedWindow()
        {
            UseLayoutRounding = true;
            TextOptions.SetTextRenderingMode(this, TextRenderingMode.ClearType);
            TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);
            VisualTextRenderingMode = TextRenderingMode.ClearType;

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            
            _ActionQueueManager = new ActionQueueManager();

            ViewModel = ViewModel<TView, TViewModel>.Create((TView) this, _ActionQueueManager);

            Closing += (sender, args) => ViewModel.InvokeAction<LeaveAction<TView, TViewModel>>();
        }

        /// <summary>
        ///     Gets the view model.
        /// </summary>
        public TViewModel ViewModel
        {
            get { return _ViewModel; }
            private set
            {
                if (_ViewModel == value) return;

                _ViewModel = value;
                DataContext = value;
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
            get { return (bool) GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        /// <summary>
        /// Gets the action queue manager.
        /// </summary>
        /// <value>
        /// The action queue manager.
        /// </value>
        IActionQueueManager IInternalView.ActionQueueManager
        {
            get { return _ActionQueueManager; }
        }
        
        private int _LoadingCounter;
        
        /// <summary>
        ///     Starts the animated loading.
        /// </summary>
        public virtual void StartLoading()
        {
            IsLoading = Interlocked.Increment(ref _LoadingCounter) > 0;
            Cursor = Cursors.AppStarting;
        }

        /// <summary>
        ///     Stops the animated loading.
        /// </summary>
        public virtual void StopLoading()
        {
            IsLoading = Interlocked.Decrement(ref _LoadingCounter) > 0;
            Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Leaves the current view.
        /// </summary>
        public void Leave()
        {
            _ViewModel.Leave();
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
            if (_Disposed) return;

            if (disposing)
            {
                if (_ViewModel != null)
                {
                    _ViewModel.Dispose();
                }

                if (_ActionQueueManager != null)
                {
                    _ActionQueueManager.Dispose();
                }
            }

            _Disposed = true;
        }
    }
}