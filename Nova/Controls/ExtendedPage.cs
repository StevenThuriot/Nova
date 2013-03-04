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
using System.Windows.Controls;
using Nova.Base;
using Nova.Helpers;
using Nova.Threading;

namespace Nova.Controls
{
    /// <summary>
    /// A default Page class that has added logic for MVVM.
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    public abstract class ExtendedPage<TView, TViewModel> : Page, IView
        where TViewModel : ViewModel<TView, TViewModel>, new()
        where TView : ExtendedPage<TView, TViewModel>, new()
    {
        /// <summary>
        ///     A value indicating whether this instance is loading.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static readonly
            // ReSharper disable StaticFieldInGenericType
            DependencyProperty IsLoadingProperty = DependencyProperty.Register("IsLoading", typeof(bool), typeof(ExtendedPage<TView, TViewModel>), new PropertyMetadata(false));

        // ReSharper restore StaticFieldInGenericType

        /// <summary>
        /// Flag wether this instance is disposed.
        /// </summary>
        private bool _Disposed;
        
        /// <summary>
        /// The parent view
        /// </summary>
        private IView _Parent;

        private TViewModel _ViewModel;
        /// <summary>
        /// Gets the view model.
        /// </summary>
        /// <value>
        /// The view model.
        /// </value>
        public TViewModel ViewModel
        {
            get { return _ViewModel; }
            private set
            {
                if (_ViewModel != value)
                {
                    _ViewModel = value;
                    DataContext = value;
                }
            }
        }
        
        /// <summary>
        /// Focuses the control.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="entityID">The entity ID.</param>
        /// <returns></returns>
        public bool FocusControl(string fieldName, Guid entityID = new Guid())
        {
            return FocusHelper.FocusControl(this, fieldName, entityID);
        }

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
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }
        
        /// <summary>
        /// Creates the specified page.
        /// </summary>
        /// <param name="parent">The parent view.</param>
        /// <param name="actionQueueManager">The action queue manager.</param>
        /// <param name="enterOnInitialize">if set to <c>true</c>, the Enter Action will be triggered automatically. Default is true.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">window</exception>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        internal static TView Create(IView parent, IActionQueueManager actionQueueManager, bool enterOnInitialize)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            if (actionQueueManager == null)
                throw new ArgumentNullException("actionQueueManager");

            var page = new TView
                {
                    _Parent = parent
                };

            page.ViewModel = ViewModel<TView, TViewModel>.Create(page, actionQueueManager, enterOnInitialize);

            return page;
        }        
        
        
        private int _LoadingCounter;
        private readonly object _Lock = new object();

        /// <summary>
        ///     Starts the animated loading.
        /// </summary>
        public virtual void StartLoading()
        {
            lock (_Lock)
            {
                var isLoading = ++_LoadingCounter > 0;

                if (IsLoading == isLoading) return;

                IsLoading = isLoading;

                if (isLoading && _Parent != null)
                {
                    _Parent.StartLoading();
                }
            }
        }

        /// <summary>
        ///     Stops the animated loading.
        /// </summary>
        public virtual void StopLoading()
        {
            lock (_Lock)
            {
                var isLoading = --_LoadingCounter > 0;

                if (IsLoading == isLoading) return;

                IsLoading = isLoading;

                if (!isLoading && _Parent != null)
                {
                    _Parent.StopLoading();
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
        /// <see cref="ExtendedPage{TViewModel}" /> is reclaimed by garbage collection.
        /// </summary>
        ~ExtendedPage()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_Disposed) return;

            if (disposing)
            {
                if (_Parent != null)
                {
                    if (IsLoading)
                    {
                        //Make sure the parent doesn't keep thinking the child is still loading yet unexistent.
                        _Parent.StopLoading();
                    }

                    _Parent = null;
                }

                if (_ViewModel != null)
                {
                    _ViewModel.Dispose();
                }
            }

            _Disposed = true;
        }
    }
}
