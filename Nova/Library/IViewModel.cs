using System;
using System.Threading.Tasks;
using Nova.Controls;
using Nova.Validation;

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

using System.ComponentModel;

namespace Nova.Library
{
    /// <summary>
    /// The ViewModel interface.
    /// </summary>
    public interface IViewModel : INotifyPropertyChanged, IChangeTracking, IDisposable
    {
        /// <summary>
        /// Gets the view.
        /// </summary>
        IView View { get; }

        /// <summary>
        /// Gets or sets the ViewModel ID.
        /// </summary>
        /// <value>
        /// The ID.
        /// </value>
        /// <exception cref="System.ArgumentNullException">value</exception>
        Guid ID { get; }

        /// <summary>
        /// Gets the action manager.
        /// </summary>
        dynamic ActionManager { get; }

        /// <summary>
        /// Gets the error collection.
        /// </summary>
        ReadOnlyErrorCollection ErrorCollection { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        bool IsValid { get; }

        /// <summary>
        /// Called to trigger all the Entering logic for this ViewModel.
        /// </summary>
        Task<bool> Enter();

        /// <summary>
        /// Called to trigger all the Leaving logic for this ViewModel.
        /// </summary>
        /// <returns>True if leaving was successful.</returns>
        Task<bool> Leave();

        /// <summary>
        /// Saves this instance.
        /// </summary>
        void Save();

        /// <summary>
        /// Loads this instance.
        /// Load(value) will only trigger in case the ViewModel has been saved before.
        /// </summary>
        void Load();

        /// <summary>
        /// Initializes the change tracking.
        /// </summary>
        void InitializeChangeTracking();

        /// <summary>
        /// Stops the change tracking.
        /// </summary>
        void StopChangeTracking();
    }
}