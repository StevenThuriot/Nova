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

namespace Nova.Base
{
    public abstract partial class ViewModel<TView, TViewModel>
    {
        private bool _Disposed;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="ViewModel{TView,TViewModel}"/> is reclaimed by garbage collection.
        /// </summary>
        ~ViewModel()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (_Disposed) return;

            _Disposed = true;

            if (disposing)
            {
                DisposeManagedResources();

                if (_ActionManager != null)
                {
                    _ActionManager.Dispose();
                }

                if (_EnterAction != null)
                {
                    _EnterAction.Dispose();
                }

                if (_LeaveAction != null)
                {
                    _LeaveAction.Dispose();
                }

                _ActionQueueManager = null;
            }

            DisposeUnmanagedResources();
        }

        /// <summary>
        /// Disposes the managed resources.
        /// </summary>
        protected virtual void DisposeManagedResources()
        {
        }

        /// <summary>
        /// Disposes the unmanaged resources.
        /// </summary>
        protected virtual void DisposeUnmanagedResources()
        {
        }
    }
}
