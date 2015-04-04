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
        private readonly INovaTree _tree;

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
        public IEnumerable<NovaTreeNodeBase> TreeNodes { get; private set; }

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
        internal NovaTreeModule(string title, INovaTree tree, IEnumerable<NovaTreeNodeBase> nodes)
        {
            if (tree == null)
                throw new ArgumentNullException("tree");

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException("title");

            if (nodes == null)
                throw new ArgumentNullException("nodes");

            if (!nodes.Any())
                throw new ArgumentException(@"A module has to have nodes.", "nodes");

            _tree = tree;
            Title = title;
            TreeNodes = nodes;

            NavigationalCommand = new RelayCommand(Activate);
        }

        /// <summary>
        /// Activates this instance.
        /// </summary>
        public void Activate()
        {
            _tree.ActivateModule(this);
        }
    }
}
