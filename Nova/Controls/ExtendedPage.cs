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
using System.Windows.Controls;
using System.Windows.Threading;
using Nova.Base;
using Nova.Threading;

namespace Nova.Controls
{
    /// <summary>
    /// A default Page class that has added logic for MVVM.
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    public abstract class ExtendedPage<TView, TViewModel> : Page, IView, IDisposable
        where TViewModel : BaseViewModel<TView, TViewModel>, new()
        where TView : ExtendedPage<TView, TViewModel>, IView, new()
    {
        /// <summary>
        /// Flag wether this instance is disposed.
        /// </summary>
        private bool _Disposed;
        
        /// <summary>
        /// The parent view
        /// </summary>
        private IView _Parent;

        /// <summary>
        /// Gets the session ID.
        /// </summary>
        /// <value>
        /// The session ID.
        /// </value>
        public Guid SessionID { get; private set; }

        /// <summary>
        /// Gets the unique step ID for this View/ViewModel.
        /// </summary>
        /// <value>
        /// The ID.
        /// </value>
        public Guid ID { get; private set; }

        private TViewModel _ViewModel;
        /// <summary>
        /// Gets the view model.
        /// </summary>
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
        /// Initializes a new instance of the <see cref="ExtendedPage{TViewModel}" /> class.
        /// </summary>
        protected ExtendedPage()
        {
            SessionID = Guid.NewGuid();
            ID = Guid.NewGuid();
        }


        /// <summary>
        /// Creates the specified page.
        /// </summary>
        /// <param name="parent">The parent view.</param>
        /// <param name="actionQueueManager">The action queue manager.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">window</exception>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        internal static TView Create(IView parent, IActionQueueManager actionQueueManager)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            if (actionQueueManager == null)
                throw new ArgumentNullException("actionQueueManager");

            var page = new TView
                {
                    _Parent = parent
                };

            page.ViewModel = BaseViewModel<TView, TViewModel>.Create(page, actionQueueManager);

            return page;
        }

        /// <summary>
        /// Starts the loading.
        /// </summary>
        public void StartLoading()
        {
            _Parent.StartLoading();
        }

        /// <summary>
        /// Stops the loading.
        /// </summary>
        public void StopLoading()
        {
            _Parent.StopLoading();
        }

        /// <summary>
        /// Invokes the specified action on the main thread.
        /// </summary>
        /// <param name="work">The work.</param>
        public void InvokeOnMainThread(Action work)
        {
            _Parent.InvokeOnMainThread(work);
        }

        /// <summary>
        /// Invokes the specified action on the main thread.
        /// </summary>
        /// <param name="work">The work.</param>
        /// <param name="priority">The priority.</param>
        public void InvokeOnMainThread(Action work, DispatcherPriority priority)
        {
            _Parent.InvokeOnMainThread(work, priority);
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
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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
                _Parent = null;

                if (_ViewModel != null)
                {
                    _ViewModel.Dispose();
                    _ViewModel = null;
                }
            }

            _Disposed = true;
        }
    }
}
