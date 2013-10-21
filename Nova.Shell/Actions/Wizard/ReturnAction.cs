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

using System;
using System.Collections.Generic;
using System.Linq;
using Nova.Library;
using Nova.Shell.Library;
using Nova.Shell.Views;
using Nova.Threading.Metadata;

namespace Nova.Shell.Actions.Wizard
{
    /// <summary>
    /// Return action
    /// </summary>
    [Terminating]
    internal abstract class ReturnAction : Actionflow<WizardView, WizardViewModel>
    {
        private IEnumerable<ActionContextEntry> _entries;
        private bool _unstackingSessionDialog;
        private string _dialogBoxResult;

        /// <summary>
        /// Gets a value indicating whether this instance is cancelled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is cancelled; otherwise, <c>false</c>.
        /// </value>
        protected abstract bool IsCancelled { get; }

        public sealed override bool Execute()
        {
            var actionContextEntry = ActionContextEntry.Create("Cancelled", IsCancelled, false);
            ActionContext.Add(actionContextEntry);

            _entries = ActionContext.GetEntries();

            if (ActionContext.TryGetValue(ActionContextConstants.SessionDialogBox, out _unstackingSessionDialog) && _unstackingSessionDialog)
            {
                _dialogBoxResult = ActionContext.GetValue<string>(ActionContextConstants.DialogBoxResult);
            }

            return base.Execute();
        }

        public sealed override void ExecuteCompleted()
        {
            var wizardViewModel = ViewModel;
            var sessionViewModel = wizardViewModel.SessionViewModel;
            var guid = wizardViewModel.ID;

            if (_unstackingSessionDialog)
            {
                sessionViewModel.UnstackSessionDialog(guid, _dialogBoxResult);
            }
            else
            {
                sessionViewModel.UnstackWizard(guid, _entries);
            }

            View.Dispose();
        }
    }
}