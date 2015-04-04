using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Nova.Shell.Controls;
using Nova.Shell.Library;

namespace Nova.Shell.Domain
{
    /// <summary>
    /// A Nova Module
    /// </summary>
    [DebuggerDisplay("Title = {Title}, Ranking = {Ranking}", Name = "Nova Module")]
    internal class NovaModule
    {
        private readonly TreeNodeBase _startUpTreeNode;
        private readonly IEnumerable<TreeNodeBase> _treeNodes;

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
        /// Initializes a new instance of the <see cref="NovaModule" /> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="ranking">The ranking.</param>
        /// <param name="treeNodes">The tree nodes.</param>
        /// <param name="startUpTreeNode">The start up tree node.</param>
        /// <exception cref="System.ArgumentNullException">title</exception>
        /// <exception cref="System.ArgumentException">@There haven't been any tree nodes defined.</exception>
        public NovaModule(string title, int ranking, IEnumerable<TreeNodeBase> treeNodes, TreeNodeBase startUpTreeNode = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException("title");

            if (treeNodes == null)
                throw new ArgumentNullException("treeNodes");

            if (!treeNodes.Any())
                throw new ArgumentException(@"There haven't been any tree nodes defined.");

            Title = title;
            Ranking = ranking;
            _treeNodes = treeNodes;
            _startUpTreeNode = startUpTreeNode;
        }

        /// <summary>
        /// Builds this instance into a Nova Tree Module.
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        internal NovaTreeModule Build(INovaTree tree, INavigatablePage page)
        {
            var nodes = BuildNovaTreeNodes(page);
            var module = new NovaTreeModule(Title, tree, nodes);

            return module;
        }

        /// <summary>
        /// Builds the nodes.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        internal IEnumerable<NovaTreeNodeBase> BuildNovaTreeNodes(INavigatablePage page)
        {
            var nodes = _treeNodes.OrderByDescending(x => x.Rank)
                                  .Select(x => x.Build(page, x == _startUpTreeNode))
                                  .Distinct()
                                  .ToList()
                                  .AsReadOnly();
            
            return nodes;
        }
    }
}
