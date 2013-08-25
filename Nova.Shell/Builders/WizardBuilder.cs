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
using Nova.Shell.Domain;
using Nova.Shell.Library;
using Nova.Shell.Views;

namespace Nova.Shell.Builders
{
    /// <summary>
    /// Wizard builder
    /// </summary>
    internal class WizardBuilder : IWizardBuilder
    {
        private readonly List<WizardStepBuilder> _steps;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="WizardBuilder" /> class.
        /// </summary>
        public WizardBuilder()
        {
            _steps = new List<WizardStepBuilder>();
        }

        /// <summary>
        /// Adds a navigational action which will populate the tree.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        /// <param name="title">The title.</param>
        /// <returns>
        /// The module builder instance.
        /// </returns>
        public IWizardBuilder AddStep<TPageView, TPageViewModel>(string title = null) 
            where TPageView : ExtendedContentControl<TPageView, TPageViewModel>, new()
            where TPageViewModel : WizardContentViewModel<TPageView, TPageViewModel>, new()
        {

            if (string.IsNullOrWhiteSpace(title))
                title = typeof(TPageView).Name;

            var step = new WizardStepBuilder<TPageView, TPageViewModel>(title);
            _steps.Add(step);

            return this;
        }

        /// <summary>
        /// Builds the steps.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public IEnumerable<NovaStep> Build(Guid id)
        {
            var steps = _steps.Select(x => x.Build(id)).ToList();
            return steps.AsReadOnly();
        }
    }
}
