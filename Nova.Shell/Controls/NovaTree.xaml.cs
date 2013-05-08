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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Nova.Shell.Domain;

namespace Nova.Shell.Controls
{
    /// <summary>
    /// Nova Tree Interface
    /// </summary>
    internal interface INovaTree
    {
        /// <summary>
        /// Initializes the tree's data.
        /// </summary>
        /// <param name="modules">The modules.</param>
        void InitializeData(IEnumerable<NovaTreeModule> modules);

        /// <summary>
        /// Activates the module.
        /// </summary>
        /// <param name="module">The module.</param>
        void ActivateModule(NovaTreeModule module);
    }

    /// <summary>
    /// Interaction logic for NovaTree.xaml
    /// </summary>
    public partial class NovaTree : INotifyPropertyChanged, INovaTree
    {
        private Type _PageType;
        private Type _ViewModelType;
        private IEnumerable<NovaTreeNode> _TreeNodes;
        private bool _ShowModules;

        /// <summary>
        /// Gets the tree nodes.
        /// </summary>
        /// <value>
        /// The tree nodes.
        /// </value>
        public IEnumerable<NovaTreeNode> TreeNodes
        {
            get { return _TreeNodes; }
            private set
            {
                if (Equals(_TreeNodes, value)) return;

                _TreeNodes = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show the modules.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the list of modules should be shown; otherwise, <c>false</c>.
        /// </value>
        public bool ShowModules
        {
            get { return _ShowModules; }
            set
            {
                if (_ShowModules == value) return;

                _ShowModules = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the modules.
        /// </summary>
        /// <value>
        /// The modules.
        /// </value>
        public IEnumerable<NovaTreeModule> Modules { get; private set; }

        /// <summary>
        /// Occurs when a property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when a property has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) 
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Reevaluates the state.
        /// </summary>
        public void ReevaluateState()
        {
            ReevaluateState(_PageType, _ViewModelType);
        }

        /// <summary>
        /// Reevaluates the state.
        /// </summary>
        /// <param name="pageType">Type of the page.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        public void ReevaluateState(Type pageType, Type viewModelType)
        {
            _PageType = pageType;
            _ViewModelType = viewModelType;

            foreach (var node in TreeNodes)
            {
                node.ReevaluateState(pageType, viewModelType);
            }
        }

        /// <summary>
        /// Navigates to the startup page.
        /// </summary>
        public void NavigateToStartupPage()
        {
            var node = TreeNodes.FirstOrDefault(x => x.IsStartupNode) ?? TreeNodes.First();
            
            node.Navigate();
        }

        /// <summary>
        /// Initializes the tree's data.
        /// </summary>
        /// <param name="modules">The modules.</param>
        /// <exception cref="System.ArgumentNullException">modules</exception>
        void INovaTree.InitializeData(IEnumerable<NovaTreeModule> modules)
        {
            if (modules == null)
                throw new ArgumentNullException("modules");

            var amountOfModules = modules.Count();

            if (amountOfModules == 0)
                throw new ArgumentException(@"No modules found.", "modules");

            TreeNodes = modules.First().TreeNodes;
            Modules = modules;
            ShowModules = amountOfModules > 1;

            InitializeComponent();
        }


        /// <summary>
        /// Activates the module.
        /// </summary>
        /// <param name="module">The module.</param>
        void INovaTree.ActivateModule(NovaTreeModule module)
        {
            TreeNodes = module.TreeNodes;
            
            ReevaluateState();
        }
    }
}
