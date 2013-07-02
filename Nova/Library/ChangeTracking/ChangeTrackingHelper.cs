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