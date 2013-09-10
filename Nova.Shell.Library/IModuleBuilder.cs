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
using Nova.Library;

namespace Nova.Shell.Library
{
    /// <summary>
    /// Module configuration builder
    /// </summary>
    public interface IModuleBuilder
    {
        /// <summary>
        /// Adds a navigational action which will populate the tree.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        /// <param name="title">The title of the node. Default value is the type name.</param>
        /// <param name="rank">The ranking in the navigational tree. Default value is 10.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>
        /// The module builder instance.
        /// </returns>
        IModuleBuilder AddNavigation<TPageView, TPageViewModel>(string title = null, int rank = 10, params ActionContextEntry[] parameters)
            where TPageViewModel : ContentViewModel<TPageView, TPageViewModel>, new()
            where TPageView : ExtendedContentControl<TPageView, TPageViewModel>, new();
        /// <summary>
        /// Adds a navigational action which will populate the tree.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        /// <param name="id">The id.</param>
        /// <param name="title">The title of the node. Default value is the type name.</param>
        /// <param name="rank">The ranking in the navigational tree. Default value is 10.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>
        /// The module builder instance.
        /// </returns>
        IModuleBuilder AddNavigation<TPageView, TPageViewModel>(Guid id, string title = null, int rank = 10, params ActionContextEntry[] parameters)
            where TPageViewModel : ContentViewModel<TPageView, TPageViewModel>, new()
            where TPageView : ExtendedContentControl<TPageView, TPageViewModel>, new();

        /// <summary>
        /// Adds a navigational action which will populate the tree.
        /// </summary>
        /// <param name="title">The title of the node.</param>
        /// <param name="builder">The multi step builder.</param>
        /// <param name="rank">The ranking in the navigational tree. Default value is 10.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>
        /// The module builder instance.
        /// </returns>
        IModuleBuilder AddNavigation(string title, Action<IMultiStepBuilder> builder, int rank = 10, params ActionContextEntry[] parameters);





        /// <summary>
        /// Sets the module title.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <returns>The module builder instance.</returns>
        /// <exception cref="System.NotSupportedException">A title has already been set and can only be set once.</exception>
        /// <exception cref="System.ArgumentNullException">title</exception>
        IModuleBuilder SetModuleTitle(string title);

        /// <summary>
        /// Marks the previously added navigational action as the startup page.
        /// </summary>
        /// <remarks>Only allowed to be used once per module.</remarks>
        /// <exception cref="System.NotSupportedException">A default use case has already been set and can only be set once.</exception>
        /// <returns>The module builder instance.</returns>
        IModuleBuilder AsStartup();

        /// <summary>
        /// Sets the module ranking.
        /// Used to determine startup module when there are multiple independant modules. (Highest ranking wins)
        /// </summary>
        /// <remarks>The ranking can only be set once. Default value is 10.</remarks>
        /// <param name="ranking">The ranking.</param>
        /// <returns>The module builder instance.</returns>
        IModuleBuilder SetModuleRanking(int ranking);

        /// <summary>
        /// Gets the ranking.
        /// </summary>
        /// <value>
        /// The ranking.
        /// </value>
        int Ranking { get; }
    }
}