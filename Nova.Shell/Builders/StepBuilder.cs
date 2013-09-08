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
using Nova.Controls;
using Nova.Shell.Domain;
using Nova.Shell.Library;

namespace Nova.Shell.Builders
{
    /// <summary>
    /// A step
    /// </summary>
    internal class StepBuilder<TView, TViewModel> : StepBuilder, IEquatable<StepBuilder<TView, TViewModel>> 
        where TView : ExtendedContentControl<TView, TViewModel>, new()
        where TViewModel : MultistepContentViewModel<TView, TViewModel>, new()
    {
        private readonly Type _viewType;
        private readonly Type _viewModelType;

        /// <summary>
        /// Initializes a new instance of the StepBuilder class.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title">The title.</param>
        public StepBuilder(Guid id, string title) : base(id, title)
        {
            _viewType = typeof (TView);
            _viewModelType = typeof (TViewModel);
        }
        
        /// <summary>
        /// Determines whether the specified <see cref="StepBuilder" /> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="StepBuilder" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="StepBuilder" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(StepBuilder<TView, TViewModel> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && _viewType == other._viewType && _viewModelType == other._viewModelType;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((StepBuilder<TView, TViewModel>) obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode*397) ^ (_viewType != null ? _viewType.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (_viewModelType != null ? _viewModelType.GetHashCode() : 0);
                return hashCode;
            }
        }

        /// <summary>
        /// Determines whether the specified <param name="left"> </param><see cref="StepBuilder" /> is equal to the  <param name="right"> </param><see cref="StepBuilder" />.
        /// </summary>
        /// <param name="left">The left step.</param>
        /// <param name="right">The right step.</param>
        /// <returns></returns>
        public static bool operator ==(StepBuilder<TView, TViewModel> left, StepBuilder<TView, TViewModel> right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Determines whether the specified <param name="left"> </param><see cref="StepBuilder" /> is not equal to the  <param name="right"> </param><see cref="StepBuilder" />.
        /// </summary>
        /// <param name="left">The left step.</param>
        /// <param name="right">The right step.</param>
        /// <returns></returns>
        public static bool operator !=(StepBuilder<TView, TViewModel> left, StepBuilder<TView, TViewModel> right)
        {
            return !Equals(left, right);
        }





        /// <summary>
        /// Builds the nova treenode step.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        internal override NovaTreeNodeStep Build(INavigatablePage page)
        {
            var command = page.CreateNavigationalAction<TView, TViewModel>(Id);
            var node = new NovaTreeNodeStep<TView, TViewModel>(Id, Title, command);

            return node;
        }
    }

    /// <summary>
    /// A step
    /// </summary>
    internal abstract class StepBuilder : IEquatable<StepBuilder>
    {
        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public Guid Id { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StepBuilder" /> class.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title">The title.</param>
        /// <exception cref="System.ArgumentNullException">title</exception>
        protected StepBuilder(Guid id, string title)
        {
            Id = id;
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException("title");

            Title = title;
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; private set; }


        /// <summary>
        /// Determines whether the specified <see cref="StepBuilder" /> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="StepBuilder" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="StepBuilder" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(StepBuilder other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Title, other.Title);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((StepBuilder) obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return (Title != null ? Title.GetHashCode() : 0);
        }

        /// <summary>
        /// Determines whether the specified <param name="left"> </param><see cref="StepBuilder" /> is equal to the  <param name="right"> </param><see cref="StepBuilder" />.
        /// </summary>
        /// <param name="left">The left step.</param>
        /// <param name="right">The right step.</param>
        /// <returns></returns>
        public static bool operator ==(StepBuilder left, StepBuilder right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Determines whether the specified <param name="left"> </param><see cref="StepBuilder" /> is not equal to the  <param name="right"> </param><see cref="StepBuilder" />.
        /// </summary>
        /// <param name="left">The left step.</param>
        /// <param name="right">The right step.</param>
        /// <returns></returns>
        public static bool operator !=(StepBuilder left, StepBuilder right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Builds the nova treenode step.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        internal abstract NovaTreeNodeStep Build(INavigatablePage page);
    }
}