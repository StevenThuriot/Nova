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

using Nova.Controls;

namespace Nova.Shell.Library.ModuleBuilder
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
        /// <param name="title">The title.</param>
        /// <returns></returns>
        IModuleBuilder AddNavigation<TPageView, TPageViewModel>(string title)
            where TPageViewModel : ContentViewModel<TPageView, TPageViewModel>, new()
            where TPageView : ExtendedPage<TPageView, TPageViewModel>, new();

        /// <summary>
        /// Marks the previously added navigational action as the startup page.
        /// </summary>
        /// <remarks>Only allowed to be used once per module.</remarks>
        /// <exception cref="System.NotSupportedException">A default use case has already been set and can only be set once.</exception>
        /// <returns></returns>
        IModuleBuilder AsStartup();

        /// <summary>
        /// Sets the module ranking.
        /// Used to determine startup module when there are multiple independant modules. (Highest ranking wins)
        /// </summary>
        /// <remarks>The ranking can only be set once. Default value is 10.</remarks>
        /// <param name="ranking">The ranking.</param>
        /// <returns></returns>
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