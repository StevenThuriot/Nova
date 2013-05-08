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
using Nova.Shell.Library;

namespace Nova.TestModule
{
    /// <summary>
    /// Test Module
    /// </summary>
    public class Module : IModule
    {
        /// <summary>
        /// Configures the module.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(IModuleBuilder builder)
        {
            var seed = (int) DateTime.Now.Ticks;
            var random = new Random(seed);

            var randomRanking = random.Next(1, 100);

            builder.SetModuleTitle("Module #" + randomRanking)
                   .SetModuleRanking(randomRanking)
                   .AddNavigation<TestPage2, TestPage2ViewModel>("Another Test Page (" + random.Next(1, 100) + ")")
                   .AddNavigation<TestPage, TestPageViewModel>(rank: 20).AsStartup();
        }
    }
}
