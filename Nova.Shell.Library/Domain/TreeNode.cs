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

namespace Nova.Shell.Library.Domain
{
    /// <summary>
    /// Tree Node Item for the navigational tree.
    /// </summary>
    internal class TreeNode
    {
        private readonly Action<INavigatablePage> _Navigate;

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNode" /> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="navigate">The navigate.</param>
        /// <exception cref="System.ArgumentNullException">title</exception>
        public TreeNode(string title, Action<INavigatablePage> navigate)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException("title");

            if (navigate == null)
                throw new ArgumentNullException("navigate");

            Title = title;
            _Navigate = navigate;
        }

        /// <summary>
        /// Creates a new tree node.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        /// <param name="title">The title.</param>
        /// <returns></returns>
        public static TreeNode New<TPageView, TPageViewModel>(string title)
            where TPageView : ExtendedPage<TPageView, TPageViewModel>, new()
            where TPageViewModel : ContentViewModel<TPageView, TPageViewModel>, new()
        {
            return new TreeNode(title, x => x.Navigate<TPageView, TPageViewModel>());
        }

        /// <summary>
        /// Navigates from the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        public void Navigate(INavigatablePage page)
        {
            _Navigate(page);
        }
    }
}
