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
using System.Windows.Input;
using Nova.Controls;
using Nova.Shell.Library;

namespace Nova.Shell.Domain
{
    /// <summary>
    /// Tree Node Item for the navigational tree.
    /// </summary>
    internal class TreeNode
    {
        private readonly Action<INavigatablePage> _Navigate;
        private readonly Func<INavigatablePage, ICommand> _CreateNavigationalAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNode" /> class.
        /// </summary>
        /// <param name="navigate">The navigation action.</param>
        /// <param name="createNavigationalAction">The create navigational command.</param>
        /// <exception cref="System.ArgumentNullException">title</exception>
        private TreeNode(Action<INavigatablePage> navigate, Func<INavigatablePage, ICommand> createNavigationalAction)
        {
            if (navigate == null)
                throw new ArgumentNullException("navigate");

            if (createNavigationalAction == null)
                throw new ArgumentNullException("createNavigationalAction");

            _Navigate = navigate;
            _CreateNavigationalAction = createNavigationalAction;
        }
        
        /// <summary>
        /// Navigates from the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        public void Navigate(INavigatablePage page)
        {
            _Navigate(page);
        }

        /// <summary>
        /// Creates the navigational command.
        /// </summary>
        /// <returns></returns>
        public ICommand CreateNavigationalAction(INavigatablePage page)
        {
            return _CreateNavigationalAction(page);
        }

        /// <summary>
        /// Creates a new tree node.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        /// <returns></returns>
        public static TreeNode New<TPageView, TPageViewModel>()
            where TPageView : ExtendedPage<TPageView, TPageViewModel>, new()
            where TPageViewModel : ContentViewModel<TPageView, TPageViewModel>, new()
        {
            Action<INavigatablePage> navigate = x => x.Navigate<TPageView, TPageViewModel>();
            Func<INavigatablePage, ICommand> navigationalAction = x => x.CreateNavigationalAction<TPageView, TPageViewModel>();
            
            return new TreeNode(navigate, navigationalAction);
        }
    }
}
