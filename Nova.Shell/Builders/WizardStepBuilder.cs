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
using System.Collections.Generic;
using System.Linq;
using Nova.Controls;
using Nova.Library;
using Nova.Shell.Domain;
using Nova.Shell.Library;

namespace Nova.Shell.Builders
{
    /// <summary>
    /// Wizard Step Builder
    /// </summary>
    internal class WizardStepBuilder<TView, TViewModel> : WizardStepBuilder
        where TView : ExtendedContentControl<TView, TViewModel>, new() 
        where TViewModel : WizardContentViewModel<TView, TViewModel>, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WizardStepBuilder{TView,TViewModel}" /> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="parameters">The parameters.</param>
        public WizardStepBuilder(string title, params ActionContextEntry[] parameters)
            : base(title, typeof(TView), typeof(TViewModel), parameters)
        {
        }


        /// <summary>
        /// Builds the step.
        /// </summary>
        /// <param name="groupId">The group Id.</param>
        /// <returns></returns>
        public override NovaStep Build(Guid groupId)
        {
            return new NovaStep<TView, TViewModel>(Title, groupId, Guid.NewGuid(), Parameters.ToArray());
        }
    }

    /// <summary>
    /// Wizard step builder base.
    /// </summary>
    internal abstract class WizardStepBuilder
    {
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
        /// Gets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        public IEnumerable<ActionContextEntry> Parameters { get; private set; }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WizardStepBuilder" /> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="viewType">Type of the view.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="parameters">The parameters.</param>
        protected WizardStepBuilder(string title, Type viewType, Type viewModelType, params ActionContextEntry[] parameters)
        {
            if (string.IsNullOrWhiteSpace(title))
                title = viewType.Name;

            Title = title;
            ViewType = viewType;
            ViewModelType = viewModelType;
            Parameters = parameters;
        }

        /// <summary>
        /// Builds the step.
        /// </summary>
        /// <param name="id">The groupId.</param>
        /// <returns></returns>
        public abstract NovaStep Build(Guid id);
    }
}
