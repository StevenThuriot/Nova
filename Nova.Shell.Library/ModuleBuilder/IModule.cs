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

using System.ComponentModel.Composition;

namespace Nova.Shell.Library.ModuleBuilder
{
    /// <summary>
    /// A base class to configure your module and prepare it for usage with the Nova Shell.
    /// </summary>
    [InheritedExport("Modules", typeof(IModule))]
    public interface IModule
    {
        /// <summary>
        /// Configures the module.
        /// </summary>
        /// <param name="builder">The builder.</param>
        void Configure(IModuleBuilder builder);
    }
}
