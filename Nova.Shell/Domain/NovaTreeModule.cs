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
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Nova.Library;
using Nova.Shell.Controls;

namespace Nova.Shell.Domain
{
    /// <summary>
    /// A Nova Module, used in the navigational tree.
    /// </summary>
    [DebuggerDisplay("Title = {Title}, Nodes = {TreeNodes.Count}", Name = "Nova Tree Module")]
    public class NovaTreeModule
    {
        private readonly INovaTree _Tree;

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; private set; }
        
        /// <summary>
        /// Gets the tree nodes.
        /// </summary>
        /// <value>
        /// The tree nodes.
        /// </value>
        public IEnumerable<NovaTreeNode> TreeNodes { get; private set; }

        /// <summary>
        /// Gets the navigational command.
        /// </summary>
        /// <value>
        /// The navigational command.
        /// </value>
        public ICommand NavigationalCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NovaTreeModule" /> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="tree">The tree.</param>
        /// <param name="nodes">The nodes.</param>
        /// <exception cref="System.ArgumentNullException">nodes</exception>
        /// <exception cref="System.ArgumentException">@A module has to have nodes.;nodes</exception>
        internal NovaTreeModule(string title, INovaTree tree, IEnumerable<NovaTreeNode> nodes)
        {
            if (tree == null)
                throw new ArgumentNullException("tree");

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException("title");

            if (nodes == null)
                throw new ArgumentNullException("nodes");

            if (!nodes.Any())
                throw new ArgumentException(@"A module has to have nodes.", "nodes");

            _Tree = tree;
            Title = title;
            TreeNodes = nodes;

            NavigationalCommand = new RelayCommand(Activate);
        }

        /// <summary>
        /// Activates this instance.
        /// </summary>
        public void Activate()
        {
            _Tree.ActivateModule(this);
        }
    }
}
