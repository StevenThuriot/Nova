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
using System.Threading;

namespace Nova.Shell.Library
{
    /// <summary>
    /// The Stack Handle
    /// </summary>
    internal abstract class StackHandle : StackInfo, IDisposable
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="StackHandle"/> is disposed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if disposed; otherwise, <c>false</c>.
        /// </value>
        protected bool Disposed { get; set; }
        private ManualResetEvent _handle;

        /// <summary>
        /// Initializes a new instance of the <see cref="StackHandle"/> class.
        /// </summary>
        /// <param name="stackId">The stack id.</param>
        protected StackHandle(Guid stackId)
            : base(stackId)
        {
            _handle = new ManualResetEvent(false);
        }

        /// <summary>
        /// Blocks the current thread until the current <see cref="T:System.Threading.WaitHandle"/> receives a signal.
        /// </summary>
        public void Wait()
        {
            if (Disposed) return;

            _handle.WaitOne();
        }

        public void Release()
        {
            if (Disposed) return;
            _handle.Set();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="StackHandle"/> class.
        /// </summary>
        ~StackHandle()
        {
            Dispose(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (Disposed) return;

            if (disposing)
            {
                using (_handle)
                {
                    _handle.Set();
                }

                _handle = null;
            }

            Disposed = true;
        }
    }

    /// <summary>
    /// Stack Handle
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class StackHandle<T> : StackHandle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StackHandle{T}"/> class.
        /// </summary>
        /// <param name="stackId">The stack id.</param>
        public StackHandle(Guid stackId) : base(stackId)
        {
        }

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public T Result { get; private set; }


        /// <summary>
        /// Sets the state of the event to signaled, allowing one or more waiting threads to proceed.
        /// </summary>
        /// <param name="result">The result.</param>
        public void Release(T result)
        {
            if (Disposed) return;

            Result = result; 
            Release();
        }
    }
}
