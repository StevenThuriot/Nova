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

using Nova.Library;
using Nova.Shell.Views;

namespace Nova.Shell.Actions.Folder
{
    /// <summary>
    /// Return action
    /// </summary>
    internal abstract class ReturnAction : Actionflow<WizardView, WizardViewModel>
    {
        /// <summary>
        /// Gets a value indicating whether this instance is cancelled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is cancelled; otherwise, <c>false</c>.
        /// </value>
        protected abstract bool IsCancelled { get; }

        public sealed override bool Execute()
        {
            var id = ViewModel.ID;

            var actionContextEntry = ActionContextEntry.Create("Cancelled", IsCancelled, false);
            ActionContext.Add(actionContextEntry);

            ViewModel.SessionViewModel.UnstackWizard(id, ActionContext);

            return base.Execute();
        }
    }
}