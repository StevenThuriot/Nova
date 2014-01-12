#region License

// 
//  Copyright 2014 Steven Thuriot
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
using System.Collections.ObjectModel;
using System.Linq;
using Nova.Library;
using Nova.Shell.Builders;
using Nova.Shell.Controls;
using Nova.Shell.Domain;
using Nova.Shell.Library;

namespace Nova.Shell.Actions.Session
{
    class RebuildNavigationalTreeAction : Actionflow<SessionView, SessionViewModel>
    {
        private NovaTree _tree;
        private ReadOnlyCollection<NovaTreeModule> _modules;

        public override void OnBefore()
        {
            _tree = View._NovaTree;
            base.OnBefore();
        }

        public override bool Execute()
        {
            var viewModel = ViewModel;
            _modules = ModuleComposer.Compose(true)
                                         .OrderByDescending(x => x.Ranking)
                                         .Select(x => x.Build(_tree, viewModel))
                                         .ToList()
                                         .AsReadOnly();
            return base.Execute();
        }

        public override void ExecuteCompleted()
        {
            ((INovaTree)_tree).InitializeData(_modules);

            var currentView = ViewModel.CurrentView;
            var pageType = currentView.GetType();
            var viewModel = currentView.ViewModel;
            var viewModelType = viewModel.GetType();
            var key = ((IContentViewModel)viewModel).NodeId;

            _tree.ReevaluateState(key, pageType, viewModelType);

            base.ExecuteCompleted();
        }
    }
}
