#region License
//   
//  Copyright 2013 Steven Thuriot
//   
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//    http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//   
#endregion

using System;
using Nova.Shell.Library;

namespace Nova.Shell.Modules.Plugins
{
    public class PluginModule : IModule
    {
        private static readonly Guid _installedPageKey = new Guid("13333333333333333333333333333336");
        private static readonly Guid _onlinePageKey = new Guid("13333333333333333333333333333337");
        private static readonly Guid _updatePageKey = new Guid("13333333333333333333333333333338");

        public void Configure(IModuleBuilder builder)
        {

            builder.SetModuleRanking(int.MinValue)
                   .SetModuleTitle("Plugin Management")
                   .AddNavigation<PluginView, PluginViewModel>(_installedPageKey, "Installed")
                   .AddNavigation<PluginView, PluginViewModel>(_onlinePageKey, "Online").AsStartup()
                   .AddNavigation<PluginView, PluginViewModel>(_updatePageKey, "Updates");
        }
    }
}
