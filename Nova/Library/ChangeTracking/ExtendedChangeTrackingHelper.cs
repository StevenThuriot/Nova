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
    /// Helper class to manage classes that implement an extended form of IChangeTracking, e.g. the <see cref="ChangeTrackingBase"/> class.
    /// </summary>
    /// <typeparam name="T">Type of instance</typeparam>
    public class ExtendedChangeTrackingHelper<T> : ChangeTrackingHelper<T>
    {
        private readonly Action<T> _stopChangeTracking;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedChangeTrackingHelper{T}" /> class.
        /// </summary>
        /// <param name="isChanged">Func to check for changes.</param>
        /// <param name="acceptChanges">The accept changes method call.</param>
        /// <param name="stopChangeTracking">The stop change tracking method call.</param>
        public ExtendedChangeTrackingHelper(Func<T, bool> isChanged, Action<T> acceptChanges, Action<T> stopChangeTracking)
            : base(isChanged, acceptChanges)
        {
            if (stopChangeTracking == null)
                throw new ArgumentNullException("stopChangeTracking");

            _stopChangeTracking = stopChangeTracking;
        }

        /// <summary>
        /// Stops the change tracking.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public void StopChangeTracking(T instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            _stopChangeTracking(instance);
        }
    }
}