using System;

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
        /// </example>
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
            get
            {
                return PauseChangeTrackingScope.CurrentScope != null;
            }
        }



        //Added a private subclass instead of just using the ChangeTrackingScope to be future proof (in case other things than pause are implemented)

        /// <summary>
        /// A Change Tracking Scope that pauses change tracking.
        /// </summary>
        private class PauseChangeTrackingScope : IDisposable
        {
            /// <summary>
            /// Gets or sets the current scope.
            /// </summary>
            /// <value>
            /// The current scope.
            /// </value>
            public static PauseChangeTrackingScope CurrentScope { get; private set; }


            private bool _Disposed;
            private PauseChangeTrackingScope _ParentScope;

            /// <summary>
            /// Initializes a new instance of the <see cref="PauseChangeTrackingScope" /> class.
            /// </summary>
            public PauseChangeTrackingScope()
            {
                _ParentScope = CurrentScope;
                CurrentScope = this;
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
                    CurrentScope = _ParentScope;
                    _ParentScope = null;
                }

                _Disposed = true;
            }
        }
    }
}