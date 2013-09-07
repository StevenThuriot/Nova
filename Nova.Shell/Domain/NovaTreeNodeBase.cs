#region License
//  
// Copyright 2013 Steven Thuriot
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
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Nova.Shell.Domain
{
    public abstract class NovaTreeNodeBase : INotifyPropertyChanged, IEquatable<NovaTreeNodeBase>
    {
        private bool _isCurrentNode;

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; private set; }

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
        /// Gets a value indicating whether this instance is startup node.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is startup node; otherwise, <c>false</c>.
        /// </value>
        public bool IsStartupNode { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NovaTreeNodeBase" /> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="isStartupNode"></param>
        /// <exception cref="System.ArgumentNullException">title</exception>
        protected NovaTreeNodeBase(string title, bool isStartupNode)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException("title");

            Title = title;
            IsStartupNode = isStartupNode;
        }

        /// <summary>
        /// Reevaluates the state.
        /// </summary>
        /// <param name="pageType">Type of the page.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        internal bool ReevaluateState(Type pageType, Type viewModelType)
        {
            return IsCurrentNode = CheckIfCurrent(pageType, viewModelType);
        }

        /// <summary>
        /// Checks if current.
        /// </summary>
        /// <param name="pageType">Type of the page.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <returns></returns>
        protected abstract bool CheckIfCurrent(Type pageType, Type viewModelType);

        /// <summary>
        /// Navigates this instance.
        /// </summary>
        public abstract void Navigate();

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }


        public bool Equals(NovaTreeNodeBase other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Title, other.Title);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((NovaTreeNodeBase) obj);
        }

        public override int GetHashCode()
        {
            return (Title != null ? Title.GetHashCode() : 0);
        }

        public static bool operator ==(NovaTreeNodeBase left, NovaTreeNodeBase right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(NovaTreeNodeBase left, NovaTreeNodeBase right)
        {
            return !Equals(left, right);
        }
    }
}