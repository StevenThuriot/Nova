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
using Nova.Shell.Library;
using Nova.Shell.Views;

namespace Nova.Shell.Domain
{
    /// <summary>
    /// Step
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    internal class NovaStep<TView, TViewModel> : NovaStep
        where TView : class, IView, new()
        where TViewModel : ContentViewModel<TView, TViewModel>, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NovaStep{TView,TViewModel}" /> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="group">The group.</param>
        public NovaStep(string title, Guid @group)
            : base(title, @group, typeof(TView), typeof(TViewModel))
        {

        }

        /// <summary>
        /// Gets the or create view.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <returns></returns>
        internal override IView GetOrCreateView(MultiStepView parent)
        {
            if (View != null)
                return View;

            return View = parent.CreateStep<TView, TViewModel>(this);
        }
    }

    /// <summary>
    /// Step
    /// </summary>
    internal abstract class NovaStep : INovaStep
    {
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the group.
        /// </summary>
        /// <value>
        /// The group.
        /// </value>
        public Guid Group { get; private set; }

        /// <summary>
        /// Gets the type of the view.
        /// </summary>
        /// <value>
        /// The type of the view.
        /// </value>
        public Type ViewType { get; private set; }

        /// <summary>
        /// Gets the type of the view model.
        /// </summary>
        /// <value>
        /// The type of the view model.
        /// </value>
        public Type ViewModelType { get; private set; }

        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <value>
        /// The view.
        /// </value>
        public IView View { get; protected set; }

        /// <summary>
        /// Gets the node ID.
        /// </summary>
        /// <value>
        /// The node ID.
        /// </value>
        public Guid NodeId { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NovaStep" /> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="group">The group.</param>
        /// <param name="viewType">Type of the view.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        protected NovaStep(string title, Guid @group, Type viewType, Type viewModelType)
        {
            NodeId = Guid.NewGuid();
            Title = title;
            Group = @group;
            ViewType = viewType;
            ViewModelType = viewModelType;
        }

        internal abstract IView GetOrCreateView(MultiStepView parent);
    }
}
