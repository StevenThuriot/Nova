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
using Nova.Library;
using Nova.Shell.Builders;
using Nova.Shell.Domain;
using Nova.Shell.Library;

namespace Nova.Shell.Views
{
    /// <summary>
    /// The Wizard's ViewModel
    /// </summary>
    public class WizardViewModel : ViewModel<WizardView, WizardViewModel>
    {
        private IDisposable _deferral;

        /// <summary>
        /// Initializes a new instance of the <see cref="WizardViewModel"/> class.
        /// </summary>
        public WizardViewModel()
        {
            _deferral = DeferCreated(); //Defer Created logic so we can call it manually in our extended initialize method.
        }

        internal void Initialize(ISessionViewModel sessionViewModel, WizardBuilder builder)
        {
            var steps = builder.Build(ID);
            var linkedSteps = new LinkedList<NovaStep>(steps);
            Initialize(sessionViewModel, linkedSteps.First);
        }

        internal void Initialize(ISessionViewModel sessionViewModel, LinkedListNode<NovaStep> initialView)
        {
            using (_deferral)
            {
                if (sessionViewModel == null)
                    throw new ArgumentNullException("sessionViewModel");

                if (initialView == null)
                    throw new ArgumentNullException("initialView");

                var multiStepView = new MultiStepView(View, sessionViewModel, ID, initialView);

                View.Content = multiStepView;
            }

            _deferral = null;
        }
    }
}