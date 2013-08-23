using System.Collections.Generic;
using System.Windows.Controls;
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
        /// Creates a new view with the current View as parent.
        /// </summary>
        /// <param name="enterOnInitialize">if set to <c>true</c>, the Enter Action will be triggered automatically. Default is true.</param>
        /// <typeparam name="TView">The type of the view.</typeparam>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <remarks>
        /// Supported types:
        ///     - ExtendedContentControl
        ///     - ExtendedControl
        ///     - ExtendedPage
        ///     - ExtendedUserControl
        /// </remarks>
        TView CreateView<TView, TViewModel>(bool enterOnInitialize = true)		
            where TViewModel : ViewModel<TView, TViewModel>, new()
            where TView : class, IView, new();

        /// <summary>
        /// Creates a new view with the passed View as parent.
        /// </summary>
        /// <typeparam name="TView">The type of the view.</typeparam>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="parent">The parent.</param>
        /// <param name="enterOnInitialize">if set to <c>true</c>, the Enter Action will be triggered automatically. Default is true.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <remarks>
        /// Supported types:
        ///     - ExtendedContentControl
        ///     - ExtendedControl
        ///     - ExtendedPage
        ///     - ExtendedUserControl
        /// </remarks>
        TView CreateView<TView, TViewModel>(IView parent, bool enterOnInitialize = true)		
            where TViewModel : ViewModel<TView, TViewModel>, new()
            where TView : class, IView, new();

        /// <summary>
        /// Creates a page specifically for the content zone and fills in the session model.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        /// <returns></returns>
        TPageView Create<TPageView, TPageViewModel>()
            where TPageViewModel : ContentViewModel<TPageView, TPageViewModel>, new()
            where TPageView : class, IView, new();

        /// <summary>
        /// Creates a page specifically for the content zone and fills in the session model.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        /// <param name="parent">The parent.</param>
        /// <returns></returns>
        TPageView Create<TPageView, TPageViewModel>(IView parent)
            where TPageViewModel : ContentViewModel<TPageView, TPageViewModel>, new()
            where TPageView : class, IView, new();



        /// <summary>
        /// Creates a wizard builder.
        /// </summary>
        /// <returns></returns>
        IWizardBuilder CreateWizardBuilder();

        /// <summary>
        /// Stacks a new wizard.
        /// </summary>
        /// <param name="builder">The builder.</param>
        void StackWizard(IWizardBuilder builder);

        /// <summary>
        /// unstacks a wizard.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="entries">The entries.</param>
        void UnstackWizard(Guid id, IEnumerable<ActionContextEntry> entries);
    }
}