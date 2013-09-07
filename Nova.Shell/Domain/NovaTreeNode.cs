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
using System.Diagnostics;
using System.Windows.Input;
using Nova.Shell.Library;

namespace Nova.Shell.Domain
{
    /// <summary>
    /// A Nova Tree Node.
    /// </summary>
    [DebuggerDisplay("Title = {Title}", Name = "Nova Tree Node")]
    public class NovaTreeNode : NovaTreeNodeBase, IEquatable<NovaTreeNode>
    {
        private readonly Type _pageType;
        private readonly Type _viewModelType;

        /// <summary>
        /// Gets the type of the page.
        /// </summary>
        /// <value>
        /// The type of the page.
        /// </value>
        public Type PageType
        {
            get { return _pageType; }
        }

        /// <summary>
        /// Gets the type of the view model.
        /// </summary>
        /// <value>
        /// The type of the view model.
        /// </value>
        public Type ViewModelType
        {
            get { return _viewModelType; }
        }

        /// <summary>
        /// Gets the navigational command.
        /// </summary>
        /// <value>
        /// The navigational command.
        /// </value>
        public ICommand NavigationalCommand { get; private set; }

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
            : base (title, isStartupNode)
        {

            if (navigationalCommand == null)
                throw new ArgumentNullException("navigationalCommand");

            _pageType = pageType;
            _viewModelType = viewModelType;

            NavigationalCommand = navigationalCommand;
        }
        
        /// <summary>
        /// Navigates this instance.
        /// </summary>
        public override void Navigate()
        {
            NavigationalCommand.Execute(this);
        }


        protected override bool CheckIfCurrent(Type pageType, Type viewModelType)
        {
            var result = pageType == _pageType && viewModelType == _viewModelType;
            return result;
        }


        public bool Equals(NovaTreeNode other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && _pageType == other._pageType && _viewModelType == other._viewModelType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((NovaTreeNode) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode*397) ^ (_pageType != null ? _pageType.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (_viewModelType != null ? _viewModelType.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(NovaTreeNode left, NovaTreeNode right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(NovaTreeNode left, NovaTreeNode right)
        {
            return !Equals(left, right);
        }
    }
}
