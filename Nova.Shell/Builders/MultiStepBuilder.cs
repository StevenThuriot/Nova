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

namespace Nova.Shell.Builders
{
    /// <summary>
    /// The multi step builder.
    /// </summary>
    internal class MultiStepBuilder : IMultiStepBuilder
    {
        private readonly int _rank;
        private readonly List<StepBuilder> _steps;
        private readonly string _title;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiStepBuilder" /> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="rank">The rank.</param>
        /// <exception cref="System.ArgumentNullException">title</exception>
        public MultiStepBuilder(string title, int rank)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException("title");

            _title = title;
            _rank = rank;
            _steps = new List<StepBuilder>();
        }

        /// <summary>
        /// Adds the step.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        /// <returns></returns>
        public IMultiStepBuilder AddStep<TPageView, TPageViewModel>(string title = null)
            where TPageView : ExtendedContentControl<TPageView, TPageViewModel>, new()
            where TPageViewModel : MultistepContentViewModel<TPageView, TPageViewModel>, new()
        {
            if (string.IsNullOrWhiteSpace(title))
                title = typeof(TPageView).Name;

            var step = new StepBuilder<TPageView, TPageViewModel>(title);
            _steps.Add(step);

            return this;
        }


        /// <summary>
        /// Builds this instance.
        /// </summary>
        internal TreeNodeBase Build()
        {
            return new MultiStepTreeNode(_title, _rank, _steps.AsReadOnly());
        }
    }
}