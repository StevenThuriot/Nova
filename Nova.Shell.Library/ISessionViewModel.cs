using Nova.Library;

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
using Nova.Controls;

namespace Nova.Shell.Library
{
    /// <summary>
    /// Interface for the session viewmodel.
    /// </summary>
    internal interface ISessionViewModel : INavigatablePage
    {
        /// <summary>
        /// Gets the navigation action manager.
        /// </summary>
        /// <value>
        /// The navigation action manager.
        /// </value>
        INavigationActionManager NavigationActionManager { get; }

        /// <summary>
        /// Gets the session model.
        /// </summary>
        /// <value>
        /// The session model.
        /// </value>
        dynamic ApplicationModel { get; }

        /// <summary>
        /// Gets the session model.
        /// </summary>
        /// <value>
        /// The session model.
        /// </value>
        dynamic Model { get; }
        
        /// <summary>
        /// Gets or sets the Session ViewModel ID.
        /// </summary>
        /// <value>
        /// The ID.
        /// </value>
        Guid ID { get; }

        /// <summary>
        /// Determines whether the session is invalid.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the session is invalid; otherwise, <c>false</c>.
        /// </returns>
        bool IsSessionValid();

        /// <summary>
        /// Creates a new page with the current window as parent.
        /// </summary>
        /// <param name="enterOnInitialize">if set to <c>true</c>, the Enter Action will be triggered automatically. Default is true.</param>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        TPageView CreatePage<TPageView, TPageViewModel>(bool enterOnInitialize = true)
            where TPageViewModel : ViewModel<TPageView, TPageViewModel>, new()
            where TPageView : ExtendedPage<TPageView, TPageViewModel>, new();
    }
}