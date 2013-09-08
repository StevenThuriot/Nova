#region License
//  
// Copyright 2013 Steven Thuriot
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
    /// Multi step builder interface.
    /// </summary>
    public interface IMultiStepBuilder
    {
        /// <summary>
        /// Adds a navigational action which will populate the tree.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        /// <param name="title">The title.</param>
        /// <returns>
        /// The module builder instance.
        /// </returns>
        IMultiStepBuilder AddStep<TPageView, TPageViewModel>(string title = null)
            where TPageViewModel : MultistepContentViewModel<TPageView, TPageViewModel>, new()
            where TPageView : ExtendedContentControl<TPageView, TPageViewModel>, new();

        /// <summary>
        /// Adds a navigational action which will populate the tree.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        /// <param name="id">The id.</param>
        /// <param name="title">The title.</param>
        /// <returns>
        /// The module builder instance.
        /// </returns>
        IMultiStepBuilder AddStep<TPageView, TPageViewModel>(Guid id, string title = null)
            where TPageViewModel : MultistepContentViewModel<TPageView, TPageViewModel>, new()
            where TPageView : ExtendedContentControl<TPageView, TPageViewModel>, new();
    }
}
