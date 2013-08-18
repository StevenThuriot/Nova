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

using System.Collections.Generic;
using System.Dynamic;
using Nova.Controls;

namespace Nova.Shell.Library
{
    /// <summary>
    ///     A viewmodel for pages that belong to a wizard.
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    public abstract class WizardContentViewModel<TView, TViewModel> : ContentViewModel<TView, TViewModel>
        where TView : class, IView
        where TViewModel : WizardContentViewModel<TView, TViewModel>, new()
    {
        //TODO: Link up buttons


        /// <summary>
        /// Gets the current step.
        /// </summary>
        /// <value>
        /// The current step.
        /// </value>
        public LinkedListNode<INovaStep> CurrentStep { get; private set; }
        
        /// <summary>
        /// Initializes the ContentViewModel using the parent session instance.
        /// </summary>
        /// <param name="initializer">The initializer.</param>
        internal override void Initialize(dynamic initializer)
        {
            CurrentStep = initializer.Node;

            //Error	103	The call to method 'Initialize' needs to be dynamically dispatched, but cannot be because it is part of a base access expression.
            //Consider casting the dynamic arguments or eliminating the base access.
            base.Initialize((ExpandoObject)initializer);
        }

        /// <summary>
        /// Gets a value indicating whether this instance can go next.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can go next; otherwise, <c>false</c>.
        /// </value>
        public virtual bool CanGoNext
        {
            get { return CurrentStep.Next != null; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can go previous.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can go previous; otherwise, <c>false</c>.
        /// </value>
        public virtual bool CanGoPrevious
        {
            get { return CurrentStep.Previous != null; }
        }



        /// <summary>
        /// Gets the next step.
        /// </summary>
        /// <value>
        /// The next step.
        /// </value>
        public virtual LinkedListNode<INovaStep> Next
        {
            get { return CurrentStep.Next; }
        }

        /// <summary>
        /// Gets the previous step.
        /// </summary>
        /// <value>
        /// The previous step.
        /// </value>
        public virtual LinkedListNode<INovaStep> Previous
        {
            get { return CurrentStep.Previous; }
        }



        /// <summary>
        /// Gets a value indicating whether this instance can cancel.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can cancel; otherwise, <c>false</c>.
        /// </value>
        public virtual bool CanCancel
        {
            get { return true; }
        }
    }
}