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
using Nova.Controls;
using Nova.Shell.Library;
using System.Windows.Input;

namespace Nova.Shell.Domain
{
    /// <summary>
    /// Tree Node Item for the navigational tree.
    /// </summary>
    internal class TreeNode
    {
        private readonly Type _pageType;
        private readonly Type _viewModelType;

        private readonly Func<INavigatablePage, ICommand> _createNavigationalAction;

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
        /// Initializes a new instance of the <see cref="TreeNode" /> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="pageType">Type of the page.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="rank">The rank.</param>
        /// <param name="createNavigationalAction">The create navigational command.</param>
        /// <exception cref="System.ArgumentNullException">title</exception>
        private TreeNode(string title, Type pageType, Type viewModelType, int rank, Func<INavigatablePage, ICommand> createNavigationalAction)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException("title");

            if (createNavigationalAction == null)
                throw new ArgumentNullException("createNavigationalAction");

            Title = title;
            Rank = rank;

            _pageType = pageType;
            _viewModelType = viewModelType;
            _createNavigationalAction = createNavigationalAction;
        }

        /// <summary>
        /// Creates a new tree node.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        /// <param name="title">The title of the node. Default value is the type name.</param>
        /// <param name="rank">The ranking in the navigational tree. Default value is 10.</param>
        /// <returns>A new treenode instance.</returns>
        public static TreeNode New<TPageView, TPageViewModel>(string title, int rank)
            where TPageView : ExtendedUserControl<TPageView, TPageViewModel>, new()
            where TPageViewModel : ContentViewModel<TPageView, TPageViewModel>, new()
        {
            var type = typeof (TPageView);

            if (string.IsNullOrWhiteSpace(title))
                title = type.Name;

            Func<INavigatablePage, ICommand> navigationalAction = x => x.CreateNavigationalAction<TPageView, TPageViewModel>();

            return new TreeNode(title, type, typeof(TPageViewModel), rank, navigationalAction);
        }

        /// <summary>
        /// Builds this instance into a Nova Tree Node
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="isStartupNode">if set to <c>true</c> [is startup node].</param>
        /// <returns></returns>
        internal NovaTreeNode Build(INavigatablePage page, bool isStartupNode)
        {
            var command = _createNavigationalAction(page);

            var node = new NovaTreeNode(Title, _pageType, _viewModelType, command, isStartupNode);
            return node;
        }
    }
}
