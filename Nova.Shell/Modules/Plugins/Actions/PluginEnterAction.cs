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

using System.Collections.Generic;
using System.Linq;
using Nova.Library.Actions;
using NuGet;

namespace Nova.Shell.Modules.Plugins.Actions
{
    internal class PluginEnterAction : EnterAction<PluginView, PluginViewModel>
    {
        private List<IPackage> _plugins;

        public override bool Enter()
        {
            var repository = PackageRepositoryFactory.Default.CreateRepository(@"http://novamodules.apphb.com/nuget");

            //TODO: Filter compatible packages.
            _plugins = repository.GetPackages().Where(x => x.IsLatestVersion).ToList();

            _plugins.AddRange(_plugins);
            _plugins.AddRange(_plugins);
            _plugins.AddRange(_plugins);
            _plugins.AddRange(_plugins);

            return base.Enter();
        }

        public override void EnterCompleted()
        {
            ViewModel.Plugins = _plugins;
            base.EnterCompleted();
        }
    }
}