#region License
//   
//  Copyright 2013 Steven Thuriot
//   
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//    http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//   
#endregion

using System.Collections.Generic;
using System.Threading.Tasks;
using Nova.Controls;
using Nova.Shell.Library.Actions.Multistep;

namespace Nova.Shell.Library
{
    /// <summary>
    ///     A viewmodel for pages that belong to a multistep.
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    public abstract class MultistepContentViewModel<TView, TViewModel> : ContentViewModel<TView, TViewModel>
        where TView : class, IView
        where TViewModel : MultistepContentViewModel<TView, TViewModel>, new()
    {
        /// <summary>
        /// Gets the current step.
        /// </summary>
        /// <value>
        /// The current step.
        /// </value>
        public LinkedListNode<StepInfo> CurrentStep { get; private set; }

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        public dynamic Model { get; private set; }

        /// <summary>
        /// Initializes the ContentViewModel using the parent session instance.
        /// </summary>
        /// <param name="initializer">The initializer.</param>
        /// <param name="triggerDeferal">if set to <c>true</c> [trigger deferal].</param>
        internal override void Initialize(IDictionary<string, object> initializer, bool triggerDeferal = true)
        {
            base.Initialize(initializer, false);

            CurrentStep = (LinkedListNode<StepInfo>)initializer["Node"];
            Model = initializer["Model"];

            if (triggerDeferal) TriggerDeferal();
        }

        public override Task<bool> Leave()
        {
            return InvokeActionAsync<LeaveMultistepAction<TView, TViewModel>>();
        }
    }
}