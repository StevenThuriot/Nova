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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Nova.Library.Actions;
using Nova.Shell.Controls;
using Nova.Shell.Domain;

namespace Nova.Shell.Actions.Session
{
    /// <summary>
    /// The session enter action
    /// </summary>
    public class SessionEnterAction : EnterAction<SessionView, SessionViewModel>
    {
        private ReadOnlyCollection<NovaTreeModule> _modules;
        private INovaTree _tree;

        public override void OnBefore()
        {
            _tree = View._NovaTree;
        }

        public override bool Enter()
        {
            var model = ((App)Application.Current).Model;
            IEnumerable<NovaModule> modules = model.Modules;

            _modules = modules.OrderByDescending(x => x.Ranking)
                              .Select(x => x.Build(_tree, ViewModel))
                              .ToList()
                              .AsReadOnly();
            
            return true;
        }

        public override void EnterCompleted()
        {
            _tree.InitializeData(_modules);
        }
    }
}
