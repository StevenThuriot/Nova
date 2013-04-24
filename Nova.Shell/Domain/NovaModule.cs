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

namespace Nova.Shell.Domain
{
    /// <summary>
    /// A Nova Module
    /// </summary>
    [DebuggerDisplay("Title = {Title}, Ranking = {Ranking}", Name = "Nova Module")]
    internal class NovaModule
    {
        private TreeNode _StartUpTreeNode;

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the ranking.
        /// </summary>
        /// <value>
        /// The ranking.
        /// </value>
        public int Ranking { get; private set; }

        /// <summary>
        /// Gets the start up tree node.
        /// </summary>
        /// <value>
        /// The start up tree node.
        /// </value>
        public TreeNode StartUpTreeNode
        {
            get { return _StartUpTreeNode ?? (_StartUpTreeNode = TreeNodes.First()); }
            private set { _StartUpTreeNode = value; }
        }

        /// <summary>
        /// Gets the tree nodes.
        /// </summary>
        /// <value>
        /// The tree nodes.
        /// </value>
        public IEnumerable<TreeNode> TreeNodes { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NovaModule"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="ranking">The ranking.</param>
        /// <param name="treeNodes">The tree nodes.</param>
        public NovaModule(string title, int ranking, IEnumerable<TreeNode> treeNodes)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException("title");

            if (treeNodes == null)
                throw new ArgumentNullException("treeNodes");

            if (!treeNodes.Any())
                throw new ArgumentException(@"There haven't been any tree nodes defined.");

            Title = title;
            Ranking = ranking;
            TreeNodes = treeNodes;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NovaModule"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="ranking">The ranking.</param>
        /// <param name="startUpTreeNode">The start up tree node.</param>
        /// <param name="treeNodes">The tree nodes.</param>
        public NovaModule(string title, int ranking, TreeNode startUpTreeNode, IEnumerable<TreeNode> treeNodes)
            : this (title, ranking, treeNodes)
        {
            StartUpTreeNode = startUpTreeNode;
        }
    }
}
