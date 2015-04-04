using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Nova.Controls;
using Nova.Library;
using Nova.Shell.Builders;
using Nova.Shell.Library;

namespace Nova.Shell.Domain
{
    /// <summary>
    /// TreeNode base.
    /// </summary>
    internal abstract class TreeNodeBase
    {
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the rank in the navigational tree.
        /// </summary>
        /// <value>
        /// The rank.
        /// </value>
        public int Rank { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNodeBase" /> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="rank">The rank.</param>
        protected TreeNodeBase(string title, int rank)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException("title");

            Title = title;
            Rank = rank;
        }

        /// <summary>
        /// Builds the node.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="isStartupNode">if set to <c>true</c> [is startup node].</param>
        /// <returns></returns>
        internal abstract NovaTreeNodeBase Build(INavigatablePage page, bool isStartupNode);
    }

    /// <summary>
    /// Tree Node Item for the navigational tree.
    /// </summary>
    internal class TreeNode : TreeNodeBase
    {
        private readonly Guid _id;
        private readonly Type _pageType;
        private readonly Type _viewModelType;

        private readonly Func<INavigatablePage, ICommand> _createNavigationalAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNode" /> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="title">The title.</param>
        /// <param name="pageType">Type of the page.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="rank">The rank.</param>
        /// <param name="createNavigationalAction">The create navigational command.</param>
        /// <exception cref="System.ArgumentNullException">title</exception>
        private TreeNode(Guid id, string title, Type pageType, Type viewModelType, int rank, Func<INavigatablePage, ICommand> createNavigationalAction)
            :base(title, rank)
        {
            if (createNavigationalAction == null)
                throw new ArgumentNullException("createNavigationalAction");

            _id = id;
            _pageType = pageType;
            _viewModelType = viewModelType;
            _createNavigationalAction = createNavigationalAction;
        }

        /// <summary>
        /// Creates a new tree node.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        /// <param name="id">The id.</param>
        /// <param name="title">The title of the node. Default value is the type name.</param>
        /// <param name="rank">The ranking in the navigational tree. Default value is 10.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>
        /// A new treenode instance.
        /// </returns>
        public static TreeNodeBase New<TPageView, TPageViewModel>(Guid id, string title, int rank, ActionContextEntry[] parameters)
            where TPageView : ExtendedContentControl<TPageView, TPageViewModel>, new()
            where TPageViewModel : ContentViewModel<TPageView, TPageViewModel>, new()
        {
            var type = typeof(TPageView);

            if (string.IsNullOrWhiteSpace(title))
                title = type.Name;

            Func<INavigatablePage, ICommand> navigationalAction = x => x.CreateNavigationalAction<TPageView, TPageViewModel>(id, parameters);

            return new TreeNode(id, title, type, typeof(TPageViewModel), rank, navigationalAction);
        }


        /// <summary>
        /// Builds this instance into a Nova Tree Node
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="isStartupNode">if set to <c>true</c> [is startup node].</param>
        /// <returns></returns>
        internal override NovaTreeNodeBase Build(INavigatablePage page, bool isStartupNode)
        {
            var command = _createNavigationalAction(page);

            var node = new NovaTreeNode(_id, Title, _pageType, _viewModelType, command, isStartupNode);
            return node;
        }
        
    }







    /// <summary>
    /// Multi Step Tree Node Item for the navigational tree.
    /// </summary>
    internal class MultiStepTreeNode : TreeNodeBase
    {
        private readonly IEnumerable<StepBuilder> _steps;

        /// <summary>
        /// Prevents a default instance of the <see cref="MultiStepTreeNode" /> class from being created.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="rank">The rank.</param>
        /// <param name="steps">The steps.</param>
        public MultiStepTreeNode(string title, int rank, IEnumerable<StepBuilder> steps)
            :base (title, rank)
        {
            if (steps == null)
                throw new ArgumentNullException("steps");
            
            if (!steps.Any())
                throw new NotSupportedException("Steps cannot be empty.");

            _steps = steps;
        }

        /// <summary>
        /// Builds the node.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="isStartupNode">if set to <c>true</c> [is startup node].</param>
        /// <returns></returns>
        internal override NovaTreeNodeBase Build(INavigatablePage page, bool isStartupNode)
        {
            var steps = _steps.Select(x => x.Build(page));
            var group = Guid.NewGuid();
            var node = new NovaMultiStepTreeNode(Title, steps, group, isStartupNode);

            return node;
        }
    }
}
