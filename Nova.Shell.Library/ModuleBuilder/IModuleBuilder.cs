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

namespace Nova.Shell.Library.ModuleBuilder
{
    /// <summary>
    /// Module configuration builder
    /// </summary>
    public interface IModuleBuilder
    {
        //builder.Add(x => x.Navigate<Page, VM>(), "Title").AsStartup()
        //       .Add(x => x.Navigate<Page2, VM2>(), "Title 2");

        /// <summary>
        /// Adds a navigational action which will populate the tree.
        /// </summary>
        /// <param name="navigate">The navigational action.</param>
        /// <param name="title">The title.</param>
        /// <returns></returns>
        IModuleBuilder AddNavigation(Action<INavigatablePage> navigate, string title);

        /// <summary>
        /// Marks the previously added navigational action as the startup page.
        /// </summary>
        /// <remarks>Only allowed to be used once per module.</remarks>
        /// <returns></returns>
        IModuleBuilder AsStartup();

        /// <summary>
        /// Sets the module ranking.
        /// Used to determine startup module when there are multiple independant modules. (Highest ranking wins)
        /// </summary>
        /// <remarks>Only allowed to be used once per module.</remarks>
        /// <param name="ranking">The ranking.</param>
        /// <returns></returns>
        IModuleBuilder SetModuleRanking(uint ranking); //Only allowed once, 
    }
}