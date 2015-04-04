using System.Collections.Generic;
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