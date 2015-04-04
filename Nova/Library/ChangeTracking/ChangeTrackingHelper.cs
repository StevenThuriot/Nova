using System;

namespace Nova.Library.ChangeTracking
{
    /// <summary>
    /// Helper class to manage classes that implement <see cref="System.ComponentModel.IChangeTracking" />
    /// </summary>
    /// <typeparam name="T">Type of instance</typeparam>
    public class ChangeTrackingHelper<T>
    {
        private readonly Func<T, bool> _isChanged;
        private readonly Action<T> _acceptChanges;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeTrackingHelper{T}" /> class.
        /// </summary>
        /// <param name="isChanged">Func to check for changes.</param>
        /// <param name="acceptChanges">The accept changes method call.</param>
        public ChangeTrackingHelper(Func<T, bool> isChanged, Action<T> acceptChanges)
        {
            if (isChanged == null)
                throw new ArgumentNullException("isChanged");

            if (acceptChanges == null)
                throw new ArgumentNullException("acceptChanges");

            _isChanged = isChanged;
            _acceptChanges = acceptChanges;
        }

        /// <summary>
        /// Determines whether the specified instance has changes.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified instance has changes; otherwise, <c>false</c>.
        /// </returns>
        public bool IsChanged(T instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            return _isChanged(instance);
        }

        /// <summary>
        /// Accepts the changes.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public void AcceptChanges(T instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            _acceptChanges(instance);
        }
    }
}