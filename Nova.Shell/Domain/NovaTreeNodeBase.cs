﻿using System;
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
        /// Gets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public Guid Id { get; private set; }

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
            : this(Guid.NewGuid(), title, isStartupNode)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NovaTreeNodeBase" /> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="title">The title.</param>
        /// <param name="isStartupNode">if set to <c>true</c> [is startup node].</param>
        /// <exception cref="System.ArgumentNullException">title</exception>
        protected NovaTreeNodeBase(Guid id, string title, bool isStartupNode)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException("title");

            Title = title;
            IsStartupNode = isStartupNode;
            Id = id;
        }

        /// <summary>
        /// Reevaluates the state.
        /// </summary>
        /// <param name="pageType">Type of the page.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        internal bool ReevaluateState(Type pageType, Type viewModelType)
        {
            return IsCurrentNode = CheckIfCurrent(pageType, viewModelType);
        }

        /// <summary>
        /// Reevaluates the state.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        internal bool ReevaluateState(Guid key)
        {
            return IsCurrentNode = (Id == key);
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