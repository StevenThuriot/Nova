using System;
using System.Windows.Input;

namespace Nova.Shell.Domain
{
    /// <summary>
    /// A Nova Tree Node.
    /// </summary>
    public class NovaTreeNode
    {
        private readonly Action _Navigate;

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the navigational command.
        /// </summary>
        /// <value>
        /// The navigational command.
        /// </value>
        public ICommand NavigationalCommand { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is startup node.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is startup node; otherwise, <c>false</c>.
        /// </value>
        public bool IsStartupNode { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NovaTreeNode" /> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="navigationalCommand">The navigational command.</param>
        /// <param name="isStartupNode">if set to <c>true</c> [is startup node].</param>
        /// <param name="navigate">The navigate.</param>
        /// <exception cref="System.ArgumentNullException">title</exception>
        public NovaTreeNode(string title, Action navigate, ICommand navigationalCommand, bool isStartupNode)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException("title");

            if (navigationalCommand == null)
                throw new ArgumentNullException("navigationalCommand");

            if (navigate == null)
                throw new ArgumentNullException("navigate");

            Title = title;
            NavigationalCommand = navigationalCommand;
            IsStartupNode = isStartupNode;
            _Navigate = navigate;
        }

        /// <summary>
        /// Navigates this instance.
        /// </summary>
        public void Navigate()
        {
            _Navigate();
        }
    }
}
