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
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Nova.Shell.Domain
{
    /// <summary>
    /// A Nova Tree Node.
    /// </summary>
    [DebuggerDisplay("Title = {Title}", Name = "Nova Tree Node")]
    public class NovaTreeNode// : INotifyPropertyChanged
    {
        private bool _IsCurrentNode;

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
            get { return _IsCurrentNode; }
            //set
            //{
            //    if (_IsCurrentNode == value) return;

            //    _IsCurrentNode = value;
            //    OnPropertyChanged();
            //}
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NovaTreeNode" /> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="navigationalCommand">The navigational command.</param>
        /// <param name="isStartupNode">if set to <c>true</c> [is startup node].</param>
        /// <exception cref="System.ArgumentNullException">title</exception>
        public NovaTreeNode(string title, ICommand navigationalCommand, bool isStartupNode)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException("title");

            if (navigationalCommand == null)
                throw new ArgumentNullException("navigationalCommand");

            Title = title;
            NavigationalCommand = navigationalCommand;
            IsStartupNode = isStartupNode;
            _IsCurrentNode = isStartupNode; //TODO: Remove
        }

        /// <summary>
        /// Navigates this instance.
        /// </summary>
        public void Navigate()
        {
            //TODO: Pass parameter, if any.
            NavigationalCommand.Execute(null);
        }

        //public event PropertyChangedEventHandler PropertyChanged;
        
        ///// <summary>
        ///// Called when property changed.
        ///// </summary>
        ///// <param name="propertyName">Name of the property.</param>
        //private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //{
        //    var handler = PropertyChanged;
        //    if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        //}
    }
}
