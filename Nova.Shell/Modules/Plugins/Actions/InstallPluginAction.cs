#region License
//   
//  Copyright 2014 Steven Thuriot
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

using System.IO;
using System.Reflection;
using Nova.Library;
using Nova.Threading.Metadata;
using NuGet;

namespace Nova.Shell.Modules.Plugins.Actions
{
    [Blocking]
    class InstallPluginAction : Actionflow<PluginView, PluginViewModel>
    {
        public override bool Execute()
        {
            var package = ActionContext.GetValue<IPackage>(RoutedAction.CommandParameter);

            if (package == null)
                return false;

            var entryAssembly = Assembly.GetEntryAssembly();
            var location = entryAssembly.Location;
            var pluginDirectory = new FileInfo(location).Directory;
            var pluginFolder = Path.Combine(pluginDirectory.FullName, "Modules");

            var repository = PackageRepositoryFactory.Default.CreateRepository(@"http://novamodules.apphb.com/nuget");
            var manager = new PackageManager(repository, pluginFolder);
            
            manager.InstallPackage(package, false, false);

            return base.Execute();
        }

        public override void ExecuteCompleted()
        {
            ViewModel.Session.RebuildTree();
            base.ExecuteCompleted();
        }
    }
}
