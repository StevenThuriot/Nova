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
using System.Threading.Tasks;
using Nova.Library;
using Nova.Shell.Library;
using Nova.Shell.Modules.Plugins.Actions;
using NuGet;

namespace Nova.Shell.Modules.Plugins
{
    public class PluginViewModel : ContentViewModel<PluginView, PluginViewModel>
    {
        private List<IPackage> _plugins;

        protected override void OnCreated()
        {
            base.OnCreated();
            SetKnownActionTypes(typeof(InstallPluginAction));
        }

        public override Task<bool> Enter(params ActionContextEntry[] parameters)
        {
            return InvokeActionAsync<PluginEnterAction>(parameters);
        }

        public List<IPackage> Plugins
        {
            get { return _plugins; }
            set { SetValue(ref _plugins, value); }
        }
    }
}
