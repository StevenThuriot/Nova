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
        private Type _pageType;
        private Type _viewModelType;
        private IEnumerable<NovaTreeNodeBase> _treeNodes;
        private bool _showModules;

        /// <summary>
        /// Gets the tree nodes.
        /// </summary>
        /// <value>
        /// The tree nodes.
        /// </value>
        public IEnumerable<NovaTreeNodeBase> TreeNodes
        {
            get { return _treeNodes; }
            private set
            {
                if (Equals(_treeNodes, value)) return;

                _treeNodes = value;
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
            get { return _showModules; }
            set
            {
                if (_showModules == value) return;

                _showModules = value;
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
            ReevaluateState(_pageType, _viewModelType);
        }

        /// <summary>
        /// Reevaluates the state.
        /// </summary>
        /// <param name="pageType">Type of the page.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        public void ReevaluateState(Type pageType, Type viewModelType)
        {
            _pageType = pageType;
            _viewModelType = viewModelType;

            var novaTreeNodes = TreeNodes;
            if (ReevaluateNodes(novaTreeNodes)) return; //Node found in current module
            
            //Search TreeNodes in a different module.
            var nodes = Modules.Select(x => x.TreeNodes)
                               .Where(x => x != novaTreeNodes)
                               .FirstOrDefault(ReevaluateNodes);

            if (nodes == null) return; //Leave the navigational tree intact if the current node was not found.

            TreeNodes = nodes;
        }

        /// <summary>
        /// Reevaluates the nodes.
        /// </summary>
        /// <param name="novaTreeNodes">The nova tree nodes.</param>
        /// <returns>True if one of the nodes is the current node.</returns>
        private bool ReevaluateNodes(IEnumerable<NovaTreeNodeBase> novaTreeNodes)
        {
            return novaTreeNodes.Select(node => node.ReevaluateState(_pageType, _viewModelType))
                                .Aggregate(false, (current, result) => current || result);
        }

        /// <summary>
        /// Finds the node title for the specified types.
        /// </summary>
        /// <typeparam name="TPage">The type of the page.</typeparam>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <returns></returns>
        public string FindTitle<TPage, TViewModel>()
        {
            return FindTitle(typeof(TPage), typeof(TViewModel));
        }

        /// <summary>
        /// Finds the node title for the specified types.
        /// </summary>
        /// <param name="pageType">Type of the page.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <returns></returns>
        public string FindTitle(Type pageType, Type viewModelType)
        {
            var node = TreeNodes.OfType<NovaTreeNode>().FirstOrDefault(x => x.PageType == pageType && x.ViewModelType == viewModelType);

            return node == null ? string.Empty : node.Title;
        }

        /// <summary>
        /// Navigates to the startup page.
        /// </summary>
        public void NavigateToStartupPage()
        {
            var node = TreeNodes.OfType<NovaTreeNode>().FirstOrDefault(x => x.IsStartupNode) ?? TreeNodes.OfType<NovaTreeNode>().First();
            
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

            Modules = modules;
            TreeNodes = Modules.First().TreeNodes;
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
