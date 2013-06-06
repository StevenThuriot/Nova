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
using System.Threading;

namespace Nova.Library
{
    /// <summary>
    /// The Change Tracking Scope
    /// </summary>
    public static class ChangeTrackingScope
    {
        /// <summary>
        /// Pauses Change Tracking.
        /// </summary>
        /// <example>
        /// using (ChangeTrackingScope.Pause)
        /// {
        ///     //Changes made here won't alter dirty state.
        /// }
        ///   </example>
        /// <remarks>
        /// Should always be used in a using block.
        /// </remarks>
        public static IDisposable Pause
        {
            get { return new PauseChangeTrackingScope(); }
        }

        /// <summary>
        /// Gets a value indicating whether change tracking is paused.
        /// </summary>
        /// <value>
        ///   <c>true</c> if change tracking is paused; otherwise, <c>false</c>.
        /// </value>
        public static bool IsPaused
        {
            get { return PauseChangeTrackingScope.IsScoped; }
        }

        //Added a private subclass instead of just using the ChangeTrackingScope to be future proof (in case other things than pause are implemented)

        /// <summary>
        /// A Change Tracking Scope that pauses change tracking.
        /// </summary>
        private class PauseChangeTrackingScope : IDisposable
        {
            private static readonly ThreadLocal<Stack<PauseChangeTrackingScope>> LocalScopeStack = new ThreadLocal<Stack<PauseChangeTrackingScope>>(() => new Stack<PauseChangeTrackingScope>());
            private bool _Disposed;
            
            /// <summary>
            /// Gets the scope stack.
            /// </summary>
            /// <value>
            /// The scope stack.
            /// </value>
            private static Stack<PauseChangeTrackingScope> ScopeStack
            {
                get { return LocalScopeStack.Value; }
            }

            /// <summary>
            /// Gets a value indicating whether change tracking is paused.
            /// </summary>
            /// <value>
            ///   <c>true</c> if change tracking is paused; otherwise, <c>false</c>.
            /// </value>
            public static bool IsScoped
            {
                get { return ScopeStack.Count > 0; }
            }

            ///// <summary>
            ///// Gets or sets the current scope.
            ///// </summary>
            ///// <value>
            ///// The current scope.
            ///// </value>
            //public static PauseChangeTrackingScope CurrentScope
            //{
            //    get { return IsScoped ? ScopeStack.Peek() : null; }
            //}

            /// <summary>
            /// Initializes a new instance of the <see cref="PauseChangeTrackingScope" /> class.
            /// </summary>
            public PauseChangeTrackingScope()
            {
                ScopeStack.Push(this);
            }

            /// <summary>
            /// Finalizes an instance of the <see cref="PauseChangeTrackingScope" /> class.
            /// </summary>
            ~PauseChangeTrackingScope()
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
                if (_Disposed) return;

                if (disposing)
                {
                    ScopeStack.Pop();
                }

                _Disposed = true;
            }
        }
    }
}