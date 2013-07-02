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
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Nova.Shell.Domain
{
    /// <summary>
    /// A Nova Tree Node.
    /// </summary>
    [DebuggerDisplay("Title = {Title}", Name = "Nova Tree Node")]
    public class NovaTreeNode : INotifyPropertyChanged
    {
        private readonly Type _pageType;
        private readonly Type _viewModelType;
        private bool _isCurrentNode;

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
        /// Gets or sets a value indicating whether this instance is current node.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is current node; otherwise, <c>false</c>.
        /// </value>
        public bool IsCurrentNode
        {
            get { return _isCurrentNode; }
            private set
            {
                if (_isCurrentNode == value) return;

                _isCurrentNode = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NovaTreeNode" /> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="pageType">Type of the page.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="navigationalCommand">The navigational command.</param>
        /// <param name="isStartupNode">if set to <c>true</c> [is startup node].</param>
        /// <exception cref="System.ArgumentNullException">title</exception>
        public NovaTreeNode(string title, Type pageType, Type viewModelType, ICommand navigationalCommand, bool isStartupNode)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException("title");

            if (navigationalCommand == null)
                throw new ArgumentNullException("navigationalCommand");

            _pageType = pageType;
            _viewModelType = viewModelType;

            Title = title;
            NavigationalCommand = navigationalCommand;
            IsStartupNode = isStartupNode;
        }
        
        /// <summary>
        /// Navigates this instance.
        /// </summary>
        public void Navigate()
        {
            //TODO: Pass parameter, if any.
            NavigationalCommand.Execute(null);
        }
        
        /// <summary>
        /// Reevaluates the state.
        /// </summary>
        /// <param name="pageType">Type of the page.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        internal bool ReevaluateState(Type pageType, Type viewModelType)
        {
            var result = pageType == _pageType && viewModelType == _viewModelType;
            IsCurrentNode = result;
            
            return result;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }




        /// <summary>
        /// <see cref="NovaTreeNode" /> Comparer
        /// </summary>
        private sealed class PageTypeViewModelTypeEqualityComparer : IEqualityComparer<NovaTreeNode>
        {
            /// <summary>
            /// Determines whether the specified <see cref="NovaTreeNode" /> are equal.
            /// </summary>
            /// <param name="x">The x.</param>
            /// <param name="y">The y.</param>
            /// <returns></returns>
            public bool Equals(NovaTreeNode x, NovaTreeNode y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x._pageType == y._pageType && x._viewModelType == y._viewModelType;
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <param name="obj">The obj.</param>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
            /// </returns>
            public int GetHashCode(NovaTreeNode obj)
            {
                unchecked
                {
                    return ((obj._pageType != null ? obj._pageType.GetHashCode() : 0)*397) ^ (obj._viewModelType != null ? obj._viewModelType.GetHashCode() : 0);
                }
            }
        }

        /// <summary>
        /// <see cref="NovaTreeNode" /> comparer instance
        /// </summary>
        public static readonly IEqualityComparer<NovaTreeNode> NovaTreeNodeComparer = new PageTypeViewModelTypeEqualityComparer();
    }
}
