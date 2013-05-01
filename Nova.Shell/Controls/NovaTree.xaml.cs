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
    /// Interaction logic for NovaTree.xaml
    /// </summary>
    public partial class NovaTree : INotifyPropertyChanged
    {
        private IEnumerable<NovaTreeNode> _TreeNodes;

        /// <summary>
        /// Initializes a new instance of the <see cref="NovaTree" /> class.
        /// </summary>
        public NovaTree()
        {
            InitializeComponent();
        }

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
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Reevaluates the state.
        /// </summary>
        /// <param name="pageType">Type of the page.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        public void ReevaluateState(Type pageType, Type viewModelType)
        {
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
        /// <param name="nodes">The nodes.</param>
        internal void InitializeData(IEnumerable<NovaTreeNode> nodes)
        {
            //TODO: Get all modules and nodes instead of just nodes when the module switcher has been implemented.
            TreeNodes = nodes;
        }
    }
}
