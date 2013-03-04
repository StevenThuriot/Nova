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
using Nova.Base;

namespace Nova.Controls
{
    /// <summary>
    /// The interface for a view used in this framework.
    /// </summary>
    public interface IView : IDisposable
    {
        /// <summary>
        /// Starts the loading.
        /// </summary>
        void StartLoading();

        /// <summary>
        /// Stops the loading.
        /// </summary>
        void StopLoading();

        /// <summary>
        /// Gets the view model.
        /// </summary>
        /// <value>
        /// The view model.
        /// </value>
        IViewModel ViewModel { get; }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        string Title { get; }

        /// <summary>
        /// Focuses the control.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="entityID">The entity ID.</param>
        /// <returns></returns>
        bool FocusControl(string fieldName, Guid entityID = default(Guid));
    }
}